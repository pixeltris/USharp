using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    partial class AssemblyRewriter
    {
        private void RewriteDelegate(TypeDefinition type, ManagedUnrealTypeInfo delegateInfo)
        {
            InjectedMembers injectedMembers = new InjectedMembers(delegateInfo);

            ManagedUnrealFunctionInfo delegateSignature = delegateInfo.Functions[0];

            string getInvokerName = "GetInvoker";

            TypeDefinition delegateSignatureType = null;
            MethodDefinition delegateConstructor = null;
            MethodDefinition delegateInvokeMethod = null;
            List<TypeDefinition> delegateSignatureTypes = new List<TypeDefinition>();
            foreach (TypeDefinition nestedType in type.NestedTypes)
            {
                foreach (MethodDefinition method in nestedType.Methods)
                {
                    if (method.Name == "Invoke")
                    {
                        delegateSignatureType = nestedType;
                        delegateInvokeMethod = method;
                        delegateSignatureTypes.Add(nestedType);
                        break;
                    }
                }

                foreach (MethodDefinition ctor in nestedType.GetConstructors())
                {
                    if (ctor.Parameters.Count == 2)
                    {
                        delegateConstructor = ctor;
                    }
                }
            }
            VerifySingleResult(delegateSignatureTypes, type, "Delegate signature");

            List<MethodDefinition> getInvokerFuncs = new List<MethodDefinition>();
            foreach (MethodDefinition method in type.Methods)
            {
                if (method.Name == getInvokerName)
                {
                    getInvokerFuncs.Add(method);
                }
            }
            VerifyNoResults(getInvokerFuncs, type, getInvokerName + " is already implemented in delegate");

            TypeDefinition baseType = type.BaseType.Resolve();
            MethodDefinition baseGetInvokerMethod = FindBaseMethodByName(baseType, getInvokerName);
            VerifyNonNull(baseGetInvokerMethod, type, "base invoker method on delegate type");

            MethodDefinition invokerMethod = WriteDelegateInvoker(type, delegateInvokeMethod, delegateInfo, delegateSignature, injectedMembers);

            MethodDefinition getInvokerMethod = CopyMethod(baseGetInvokerMethod, true, delegateSignatureType);
            type.Methods.Add(getInvokerMethod);

            ILProcessor processor = getInvokerMethod.Body.GetILProcessor();
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldftn, invokerMethod);
            processor.Emit(OpCodes.Newobj, delegateConstructor);
            processor.Emit(OpCodes.Ret);
            FinalizeMethod(getInvokerMethod);

            AddPathAttribute(type, delegateInfo);

            CreateLoadNativeTypeMethod(type, delegateInfo, injectedMembers);
        }

        private MethodDefinition WriteDelegateInvoker(TypeDefinition type, MethodDefinition signature, ManagedUnrealTypeInfo typeInfo,
            ManagedUnrealFunctionInfo functionInfo, InjectedMembers injectedMembers)
        {
            MethodDefinition method = CopyMethod(signature, false);
            method.HasThis = true;
            method.Attributes = MethodAttributes.Private;
            method.Name = "Invoker";
            type.Methods.Add(method);
            
            MethodReference isBoundGetter = assembly.MainModule.ImportEx(GetTypeFromTypeDefinition(type).GetMethod("get_IsBound"));

            FieldDefinition functionIsValid = AddIsValidField(type, functionInfo);
            FieldDefinition functionAddressField = AddNativeFunctionField(type, functionInfo);
            FieldDefinition paramsSizeField = AddParamsSizeField(type, functionInfo);
            injectedMembers.SetFunctionIsValid(functionInfo, functionIsValid);
            injectedMembers.SetFunctionAddress(functionInfo, functionAddressField);
            injectedMembers.SetFunctionParamsSize(functionInfo, paramsSizeField);

            ILProcessor processor = method.Body.GetILProcessor();
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Callvirt, isBoundGetter);
            Instruction branchPosition = processor.Body.Instructions[processor.Body.Instructions.Count - 1];

            Dictionary<ManagedUnrealPropertyInfo, ParameterDefinition> parameters = GetParamsFromMethod(type, functionInfo, method, injectedMembers, true);

            WriteNativeFunctionInvokerBody(type, typeInfo, functionInfo, method, injectedMembers, parameters, processor, true, false);

            int branchTargetIndex = processor.Body.Instructions.Count;

            // Set out parms / return value to default(XXXX)
            EmitFunctionOutDefaults(processor, functionInfo, method);
            processor.Emit(OpCodes.Ret);

            processor.InsertAfter(branchPosition, processor.Create(OpCodes.Brfalse, processor.Body.Instructions[branchTargetIndex]));

            FinalizeMethod(method);
            return method;
        }

        /// <summary>
        /// Replaces the method body with an invoke to the instanced UFunction for the given UObject to call the top-most override.
        /// Also moves the original method body to a new generated "_Implementation" method and redirects all calls to base.FuncName
        /// to base.FuncName_Implementation (if not using explicit "_Implementation" methods).
        /// </summary>
        private void RewriteMethodAsUFunctionInvoke(TypeDefinition type, ManagedUnrealTypeInfo typeInfo, ManagedUnrealFunctionInfo functionInfo,
            MethodDefinition method, InjectedMembers injectedMembers, bool perInstanceFunctionAddress)
        {
            MethodDefinition implementationMethod = null;

            string implementationMethodName = functionInfo.Name + codeSettings.VarNames.ImplementationMethod;
            List<MethodDefinition> implementationMethods = new List<MethodDefinition>();
            foreach (MethodDefinition methodDef in type.Methods)
            {
                if (methodDef.Name == implementationMethodName)
                {
                    implementationMethods.Add(methodDef);
                }
            }

            if (codeSettings.UseExplicitImplementationMethods)
            {
                if (functionInfo.IsImplementation)
                {
                    implementationMethod = method;
                }
                else
                {
                    // The definition method. Replace the body with a call to the correct UFunction for the given UObject.
                    WriteNativeFunctionInvoker(type, typeInfo, functionInfo, method, injectedMembers, perInstanceFunctionAddress);

                    if (implementationMethods.Count == 0 && (functionInfo.IsBlueprintImplemented ||
                        codeSettings.UseImplicitBlueprintImplementableEvent))
                    {
                        // The "_Implementation" method is being skipped 
                        return;
                    }

                    // Get the "_Implementation" method
                    VerifySingleResult(implementationMethods, type, "implementation method " + implementationMethodName);
                    implementationMethod = implementationMethods[0];
                }
            }
            else
            {
                VerifyNoResults(implementationMethods, type, "implementation method " + implementationMethodName);

                // Move the existing method body to a new method with an _Implementation suffix. We're going to rewrite
                // the body of the original method so that existing managed call sites will instead invoke the UFunction.
                implementationMethod = CopyMethod(method, false);
                implementationMethod.Name = implementationMethodName;
                implementationMethod.Body = method.Body;
                type.Methods.Add(implementationMethod);

                if (functionInfo.IsOverride)
                {
                    // Redirect all base calls from base.XXXX() to base.XXXX_Implementation()
                    foreach (Instruction instruction in implementationMethod.Body.Instructions)
                    {
                        // OpCodes.Call should be base.XXXX(), OpCodes.Callvirt should be XXXX()
                        if (instruction.OpCode == OpCodes.Call)
                        {
                            MethodReference calledMethod = (MethodReference)instruction.Operand;
                            if (calledMethod.Name == functionInfo.Name)
                            {
                                MethodReference baseImplementationMethod = null;
                                foreach (MethodReference baseTypeMethod in calledMethod.DeclaringType.Resolve().Methods)
                                {
                                    if (baseTypeMethod.Name == implementationMethodName)
                                    {
                                        baseImplementationMethod = baseTypeMethod;
                                        break;
                                    }
                                }
                                VerifyNonNull(baseImplementationMethod, type, "Failed to find the base method " + implementationMethod);

                                instruction.Operand = baseImplementationMethod;
                            }
                        }
                    }
                }

                // Replace the original method's body with one that pinvokes to call the UFunction on the native side.
                // For RPCs, this will allow the engine's ProcessEvent logic to route the call to the correct client or server.
                // For BlueprintImplementableEvents, it will call the correct overriden version of the UFunction.
                WriteNativeFunctionInvoker(type, typeInfo, functionInfo, method, injectedMembers, perInstanceFunctionAddress);
            }

            if (functionInfo.WithValidation)
            {
                // Inject a validation call at the beginning of the implementation method.
                // This is slightly different from how UHT handles things, which is to call both the validation
                // and the implementation method from the generated invoker, but this simplifies the IL generation
                // and doesn't affect the relative timing of the calls.
                InjectRPCValidation(type, functionInfo, implementationMethod, injectedMembers);
            }

            // Create a managed invoker, named to match the UFunction but calls our generated
            // _Implementation method with the user's original method body. This is how the engine will actually invoke
            // the UFunction, when this class's version of it needs to run.
            WriteFunctionInvoker(type, typeInfo, functionInfo, implementationMethod, injectedMembers, functionInfo.IsBlueprintEvent);
        }

        private void InjectRPCValidation(TypeDefinition type, ManagedUnrealFunctionInfo functionInfo, MethodDefinition method,
            InjectedMembers injectedMembers)
        {
            string validationMethodName = functionInfo.Name + codeSettings.VarNames.RPCValidate;

            List<MethodDefinition> validationMethods = new List<MethodDefinition>();
            foreach (MethodDefinition methodDef in type.Methods)
            {
                if (methodDef.Name == validationMethodName)
                {
                    validationMethods.Add(methodDef);
                }
            }
            VerifySingleResult(validationMethods, type, "required validation method " + validationMethodName);

            MethodDefinition validationMethod = validationMethods[0];

            ILProcessor processor = method.Body.GetILProcessor();
            Instruction branchTarget = method.Body.Instructions[0];

            // Roughly compare the function params for the method / validation method
            bool invalidParams = false;
            if (method.Parameters.Count != validationMethod.Parameters.Count)
            {
                invalidParams = true;
            }
            //for (int i = 0; i < method.Parameters.Count; ++i)
            //{
            //    // TODO: Compare the params (allow removal of ref/out?)
            //}

            if (invalidParams)
            {
                throw new RewriteException(type, "RPC validation method signature mismatch " + validationMethodName);
            }

            processor.InsertBefore(branchTarget, processor.Create(OpCodes.Ldarg_0));

            for (int i = 0; i < method.Parameters.Count; ++i)
            {
                processor.InsertBefore(branchTarget, processor.Create(OpCodes.Ldarg, i + 1));
            }
            processor.InsertBefore(branchTarget, processor.Create(OpCodes.Callvirt, validationMethod));
            processor.InsertBefore(branchTarget, processor.Create(OpCodes.Brtrue_S, branchTarget));

            processor.InsertBefore(branchTarget, processor.Create(OpCodes.Ldstr, validationMethodName));
            processor.InsertBefore(branchTarget, processor.Create(OpCodes.Call, rpcValidateFailedMethod));

            // Set out parms / return value to default(XXXX)
            InsertInstructionsBefore(processor, branchTarget, CreateFunctionOutDefaults(processor, functionInfo, method));
            processor.InsertBefore(branchTarget, processor.Create(OpCodes.Ret));

            method.Body.OptimizeMacros();
        }

        private void WriteFunctionInvoker(TypeDefinition type, ManagedUnrealTypeInfo typeInfo, ManagedUnrealFunctionInfo functionInfo, 
            MethodDefinition method, InjectedMembers injectedMembers, bool explicitCall)
        {
            string invokerName = functionInfo.Name + codeSettings.VarNames.FunctionInvoker;

            List<MethodDefinition> invokerDefinitions = new List<MethodDefinition>();
            foreach (MethodDefinition methodDef in type.Methods)
            {
                if (methodDef.Name == invokerName)
                {
                    invokerDefinitions.Add(methodDef);
                }
            }
            VerifyNoResults(invokerDefinitions, type, "function invoker " + invokerName);

            MethodReference gchelperFindMethod = MakeGenericMethod(gchelperFindMethodGeneric, type);

            MethodAttributes invokerAttributes = MethodAttributes.Private | MethodAttributes.Static;
            MethodDefinition invoker = new MethodDefinition(invokerName, invokerAttributes, assembly.MainModule.ImportEx(typeof(void)));
            invoker.Parameters.Add(new ParameterDefinition("buffer", ParameterAttributes.None, intPtrTypeRef));
            invoker.Parameters.Add(new ParameterDefinition("obj", ParameterAttributes.None, intPtrTypeRef));
            AddFunctionInvokerAttribute(invoker, functionInfo);
            type.Methods.Add(invoker);

            ILProcessor processor = invoker.Body.GetILProcessor();

            VariableDefinition objVar = null;
            if (!functionInfo.IsStatic)
            {
                objVar = new VariableDefinition(type);
                processor.Body.Variables.Add(objVar);
                processor.Emit(OpCodes.Ldarg_1);
                processor.Emit(OpCodes.Call, gchelperFindMethod);
                processor.Emit(OpCodes.Stloc, objVar);
            }

            Instruction loadBuffer = processor.Create(OpCodes.Ldarg_0);

            // Parameters including the return prop
            List<ManagedUnrealPropertyInfo> parameters = new List<ManagedUnrealPropertyInfo>();
            if (functionInfo.ReturnProp != null)
            {
                parameters.Add(functionInfo.ReturnProp);
            }
            parameters.AddRange(functionInfo.Params);

            VariableDefinition[] marshalerVariables = new VariableDefinition[parameters.Count];
            VariableDefinition[] paramVariables = new VariableDefinition[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                ManagedUnrealPropertyInfo paramInfo = parameters[i];
                Type paramType = ManagedUnrealTypeInfo.GetTypeFromPropertyInfo(paramInfo);

                VariableDefinition paramVar = paramVariables[i] = new VariableDefinition(assembly.MainModule.ImportEx(paramType));
                processor.Body.Variables.Add(paramVar);

                VariableDefinition marshalerVar = EmitCreateMarshaler(processor, loadBuffer,
                        injectedMembers.GetFunctionParamOffset(functionInfo, paramInfo),
                        injectedMembers.GetFunctionParamPropertyAddress(functionInfo, paramInfo), paramInfo);
                if (marshalerVar != null)
                {
                    invoker.Body.Variables.Add(marshalerVar);
                    marshalerVariables[i] = marshalerVar;
                }

                // Load the parameter from the native buffer (skip return prop and out params)
                if (paramInfo != functionInfo.ReturnProp && (!paramInfo.IsOut || paramInfo.IsCollection))
                {
                    FieldDefinition offsetField = injectedMembers.GetFunctionParamOffset(functionInfo, paramInfo);
                    FieldDefinition nativePropertyField = injectedMembers.GetFunctionParamPropertyAddress(functionInfo, paramInfo);

                    EmitLoad(processor, loadBuffer, offsetField, nativePropertyField, typeInfo, paramInfo, marshalerVar, paramVar);
                }
            }

            if (!functionInfo.IsStatic)
            {
                processor.Emit(OpCodes.Ldloc, objVar);
            }

            for (int i = 0; i < parameters.Count; i++)
            {
                ManagedUnrealPropertyInfo paramInfo = parameters[i];
                VariableDefinition paramVar = paramVariables[i];

                if (paramInfo != functionInfo.ReturnProp)
                {
                    OpCode loadCode = paramInfo.IsByRef || paramInfo.IsOut ? OpCodes.Ldloca : OpCodes.Ldloc;
                    processor.Emit(loadCode, paramVar);
                }
            }

            if (functionInfo.IsStatic || explicitCall)
            {
                processor.Emit(OpCodes.Call, assembly.MainModule.ImportEx(method));
            }
            else
            {
                processor.Emit(OpCodes.Callvirt, assembly.MainModule.ImportEx(method));
            }

            // Store the result (if any) in the result value local
            if (parameters.Count > 0 && functionInfo.ReturnProp != null)
            {
                // This assumes assumes ReturnProp is always the first index in the parameters collection
                Debug.Assert(functionInfo.ReturnProp == parameters[0]);
                processor.Emit(OpCodes.Stloc, paramVariables[0]);
            }

            // Marshal out params back to the native parameter buffer.
            for (int i = 0; i < parameters.Count; i++)
            {
                ManagedUnrealPropertyInfo paramInfo = parameters[i];
                VariableDefinition paramVar = paramVariables[i];
                VariableDefinition marshalerVar = marshalerVariables[i];
                FieldDefinition offsetField = injectedMembers.GetFunctionParamOffset(functionInfo, paramInfo);
                FieldDefinition nativePropertyField = injectedMembers.GetFunctionParamPropertyAddress(functionInfo, paramInfo);

                Instruction[] loadBufferInstructions = GetLoadNativeBufferInstructions(typeInfo, processor, loadBuffer, offsetField);
                MethodReference marshaler = GetToNativeMarshaler(ManagedUnrealMarshalerType.Copy, paramInfo);
                
                if (paramInfo.IsByRef || paramInfo.IsOut || functionInfo.ReturnProp == paramInfo)
                {
                    if (marshalerVar != null)
                    {
                        // Instanced marshaler which is stored in a local var (e.g. TArrayCopyMarshaler<>)
                        EmitStore(processor, loadBuffer, offsetField, nativePropertyField, typeInfo, paramInfo, marshalerVar, paramVar);
                    }
                    else
                    {
                        Instruction loadLocal = processor.Create(OpCodes.Ldloc, paramVar);
                        WriteMarshalToNative(processor, nativePropertyField, loadBufferInstructions, null, loadLocal, marshaler);
                    }
                }
            }

            processor.Emit(OpCodes.Ret);

            FinalizeMethod(invoker);
        }

        /// <summary>
        /// Inserts an IsValid safeguard on the given function
        /// </summary>
        private void InsertNativeFunctionInvokerIsValidSafeguard(TypeDefinition type, ManagedUnrealFunctionInfo functionInfo,
            MethodDefinition method, InjectedMembers injectedMembers, ILProcessor processor)
        {
            if (!codeSettings.GenerateIsValidSafeguards)
            {
                return;
            }

            FieldDefinition functionIsValidField = injectedMembers.GetFunctionIsValid(functionInfo);
            if (functionIsValidField == null)
            {
                return;
            }

            // If the function isn't valid create a log and set any out params to default values
            // if (!functionName_IsValid)
            // {
            //     NativeReflection.LogInvalidFunctionAccessed("XXXX");
            //     outParams = default(XXXX);
            //     return default(XXXX);
            // }

            List<Instruction> instructions = new List<Instruction>();

            Instruction branchTarget = processor.Body.Instructions[0];

            instructions.Add(processor.Create(OpCodes.Ldsfld, functionIsValidField));
            instructions.Add(processor.Create(OpCodes.Brtrue, branchTarget));
            instructions.Add(processor.Create(OpCodes.Ldstr, functionInfo.Path));
            instructions.Add(processor.Create(OpCodes.Call, reflectionLogInvalidFunctionAccessedMethod));

            // Set out parms / return value to default(XXXX)
            instructions.AddRange(CreateFunctionOutDefaults(processor, functionInfo, method));

            instructions.Add(processor.Create(OpCodes.Ret));

            // Insert the instructions at the start
            InsertInstructionsAt(processor, 0, instructions.ToArray());
        }

        private void WriteNativeFunctionInvoker(TypeDefinition type, ManagedUnrealTypeInfo typeInfo, ManagedUnrealFunctionInfo functionInfo, 
            MethodDefinition method, InjectedMembers injectedMembers, bool perInstanceFunctionAddress)
        {
            method.Body = new MethodBody(method);
            WriteNativeFunctionInvokerBody(type, typeInfo, functionInfo, method, injectedMembers, GetParamsFromMethod(type, functionInfo, method),
                method.Body.GetILProcessor(), false, perInstanceFunctionAddress);
        }

        private void WriteNativeFunctionInvokerBody(TypeDefinition type, ManagedUnrealTypeInfo typeInfo, ManagedUnrealFunctionInfo functionInfo, 
            MethodDefinition method, InjectedMembers injectedMembers, Dictionary<ManagedUnrealPropertyInfo, ParameterDefinition> parameters, 
            ILProcessor processor, bool isDelegate, bool perInstanceFunctionAddress)
        {
            FieldDefinition functionAddressField = injectedMembers.GetFunctionAddress(functionInfo);
            FieldDefinition paramsSizeField = injectedMembers.GetFunctionParamsSize(functionInfo);

            if (perInstanceFunctionAddress && injectedMembers.GetFunctionAddressPerInstance(functionInfo) == null)
            {
                functionAddressField = AddNativeFunctionField(type, functionInfo, false);
                injectedMembers.SetFunctionAddressPerInstance(functionInfo, functionAddressField);
            }

            VariableDefinition bufferAllocation = new VariableDefinition(bytePtrTypeRef);
            method.Body.Variables.Add(bufferAllocation);

            VariableDefinition paramsBuffer = new VariableDefinition(intPtrTypeRef);
            method.Body.Variables.Add(paramsBuffer);

            Instruction loadBuffer = processor.Create(OpCodes.Ldloc, paramsBuffer);
            Instruction loadOwner = processor.Create(OpCodes.Ldnull);

            // byte* ParamsBufferAllocation = stackalloc byte[XXXX_ParamsSize];
            // IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
            processor.Emit(OpCodes.Ldsfld, paramsSizeField);
            processor.Emit(OpCodes.Conv_U);
            processor.Emit(OpCodes.Localloc);
            processor.Emit(OpCodes.Stloc, bufferAllocation);
            processor.Emit(OpCodes.Ldloca, paramsBuffer);
            processor.Emit(OpCodes.Ldloc, bufferAllocation);
            processor.Emit(OpCodes.Call, intPtrConstructorMethod);

            if (codeSettings.LazyFunctionParamInitDestroy)
            {
                // NativeReflection.InvokeFunction_InitAll(XXXX_FunctionAddress, ParamsBuffer);
                if (functionAddressField.IsStatic)
                {
                    processor.Emit(OpCodes.Ldsfld, functionAddressField);
                }
                else
                {
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, functionAddressField);
                }
                processor.Emit(OpCodes.Ldloc, paramsBuffer);
                processor.Emit(OpCodes.Call, reflectionInitAllMethod);
            }
            else if (codeSettings.MemzeroStackalloc || codeSettings.MemzeroStackallocOnlyIfOut)
            {
                bool requiresMemzero = codeSettings.MemzeroStackalloc;

                if (codeSettings.MemzeroStackallocOnlyIfOut)
                {
                    // Memzero only if there is a return value or a (non ref) out param which doesn't have a zero constructor.
                    // (if the param can't be zero initialized it will be initialized with a call to InitializeValue anyway)
                    foreach (KeyValuePair<ManagedUnrealPropertyInfo, ParameterDefinition> param in parameters)
                    {
                        ManagedUnrealPropertyInfo paramInfo = param.Key;

                        if (paramInfo.IsOut && !ManagedUnrealTypeInfo.PropertyRequiresInit(paramInfo))
                        {
                            requiresMemzero = true;
                            break;
                        }
                    }
                    if (functionInfo.ReturnProp != null && !ManagedUnrealTypeInfo.PropertyRequiresInit(functionInfo.ReturnProp))
                    {
                        requiresMemzero = true;
                    }
                }

                if (requiresMemzero)
                {
                    // FMemory.Memzero(ParamsBuffer, XXXX_ParamsSize
                    processor.Emit(OpCodes.Ldloc, paramsBuffer);
                    processor.Emit(OpCodes.Ldsfld, paramsSizeField);
                    processor.Emit(OpCodes.Call, fmemoryMemzero);
                    processor.Emit(OpCodes.Pop);// Memzero returns a value, pop it off the stack
                }
            }

            Instruction loadParamBufferInstruction = processor.Create(OpCodes.Ldloc, paramsBuffer);

            Dictionary<ManagedUnrealPropertyInfo, FieldDefinition> paramOffsets = new Dictionary<ManagedUnrealPropertyInfo, FieldDefinition>();
            Dictionary<ManagedUnrealPropertyInfo, VariableDefinition> paramMarshalerVars = new Dictionary<ManagedUnrealPropertyInfo, VariableDefinition>();

            foreach (KeyValuePair<ManagedUnrealPropertyInfo, ParameterDefinition> param in parameters)
            {
                ManagedUnrealPropertyInfo paramInfo = param.Key;

                FieldDefinition offsetField = injectedMembers.GetFunctionParamOffset(functionInfo, paramInfo);
                FieldDefinition nativePropertyField = injectedMembers.GetFunctionParamPropertyAddress(functionInfo, paramInfo);

                VariableDefinition marshalerVar = EmitCreateMarshaler(processor, loadBuffer, offsetField, nativePropertyField, paramInfo);
                if (marshalerVar != null)
                {
                    method.Body.Variables.Add(marshalerVar);
                    paramMarshalerVars[paramInfo] = marshalerVar;
                }

                // Init the param memory if required
                EmitFunctionParamInit(processor, functionInfo, paramInfo, paramsBuffer, nativePropertyField);

                // Write the parameter to the native buffer if it isn't an "out" parameter
                // XXXXMarshaler.ToNative(IntPtr.Add(ParamsBuffer, XXXX_YYYY_Offset), YYYY);
                if (!paramInfo.IsOut)
                {
                    EmitStore(processor, loadParamBufferInstruction, offsetField, nativePropertyField, typeInfo, param.Key, marshalerVar, param.Value);
                }
            }

            if (functionInfo.ReturnProp != null)
            {
                // Init the return value memory if required
                EmitFunctionParamInit(processor, functionInfo, functionInfo.ReturnProp, paramsBuffer,
                    injectedMembers.GetFunctionParamPropertyAddress(functionInfo, functionInfo.ReturnProp));
            }

            if (isDelegate)
            {
                // ProcessDelegate(ParamsBuffer);
                MethodReference processDelegateMethod = assembly.MainModule.ImportEx(GetTypeFromTypeDefinition(type).GetMethod("ProcessDelegate"));
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldloc, paramsBuffer);
                processor.Emit(OpCodes.Callvirt, processDelegateMethod);
            }
            else
            {
                // NativeReflection.InvokeFunction(Address, XXXX_FunctionAddress, ParamsBuffer, XXXX_ParamsSize);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, typeInfo.IsInterface ? iinterfaceImplAddressGetter : uobjectAddressGetter);
                if (functionAddressField.IsStatic)
                {
                    processor.Emit(OpCodes.Ldsfld, functionAddressField);
                }
                else
                {
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, functionAddressField);
                }
                processor.Emit(OpCodes.Ldloc, paramsBuffer);
                processor.Emit(OpCodes.Ldsfld, paramsSizeField);
                processor.Emit(OpCodes.Call, reflectionInvokeFunctionMethod);
            }

            VariableDefinition returnVar = null;
            if (functionInfo.ReturnProp != null)
            {
                returnVar = new VariableDefinition(assembly.MainModule.ImportEx(ManagedUnrealTypeInfo.GetTypeFromPropertyInfo(functionInfo.ReturnProp)));
                method.Body.Variables.Add(returnVar);

                VariableDefinition returnVarMarshaler = EmitCreateMarshaler(processor, loadBuffer,
                        injectedMembers.GetFunctionParamOffset(functionInfo, functionInfo.ReturnProp),
                        injectedMembers.GetFunctionParamPropertyAddress(functionInfo, functionInfo.ReturnProp), functionInfo.ReturnProp);
                if (returnVarMarshaler != null)
                {
                    method.Body.Variables.Add(returnVarMarshaler);
                }

                FieldDefinition offsetField = injectedMembers.GetFunctionParamOffset(functionInfo, functionInfo.ReturnProp);
                FieldDefinition nativePropertyField = injectedMembers.GetFunctionParamPropertyAddress(functionInfo, functionInfo.ReturnProp);

                EmitLoad(processor, loadBuffer, offsetField, nativePropertyField, typeInfo, functionInfo.ReturnProp, returnVarMarshaler, returnVar);

                // Destroy the return value memory if required
                EmitFunctionParamDestroy(processor, functionInfo, functionInfo.ReturnProp, paramsBuffer,
                    injectedMembers.GetFunctionParamPropertyAddress(functionInfo, functionInfo.ReturnProp));
            }

            foreach (KeyValuePair<ManagedUnrealPropertyInfo, ParameterDefinition> param in parameters)
            {
                ManagedUnrealPropertyInfo paramInfo = param.Key;

                if (paramInfo.IsByRef || paramInfo.IsOut || paramInfo.IsCollection)
                {
                    FieldDefinition offsetField = injectedMembers.GetFunctionParamOffset(functionInfo, paramInfo);
                    FieldDefinition nativePropertyField = injectedMembers.GetFunctionParamPropertyAddress(functionInfo, paramInfo);
                    VariableDefinition marshalerVar;
                    paramMarshalerVars.TryGetValue(paramInfo, out marshalerVar);

                    EmitLoad(processor, loadBuffer, offsetField, nativePropertyField, typeInfo, paramInfo, marshalerVar, param.Value);
                }

                // Destroy the param memory if required
                EmitFunctionParamDestroy(processor, functionInfo, paramInfo, paramsBuffer,
                    injectedMembers.GetFunctionParamPropertyAddress(functionInfo, paramInfo));
            }

            if (codeSettings.LazyFunctionParamInitDestroy)
            {
                // NativeReflection.InvokeFunction_DestroyAll(XXXX_FunctionAddress, ParamsBuffer);
                if (functionAddressField.IsStatic)
                {
                    processor.Emit(OpCodes.Ldsfld, functionAddressField);
                }
                else
                {
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, functionAddressField);
                }
                processor.Emit(OpCodes.Ldloc, paramsBuffer);
                processor.Emit(OpCodes.Call, reflectionDestroyAllMethod);
            }

            if (returnVar != null)
            {
                processor.Emit(OpCodes.Ldloc, returnVar);
            }
            processor.Emit(OpCodes.Ret);

            if (!functionAddressField.IsStatic)
            {
                // if (XXXX_FunctionAddress == IntPtr.Zero) { XXXX_FunctionAddress = NativeReflection.GetFunctionFromInstance(Address, "XXXX"); }
                Instruction branchTarget = processor.Body.Instructions[0];
                processor.InsertBefore(branchTarget, processor.Create(OpCodes.Ldarg_0));
                processor.InsertBefore(branchTarget, processor.Create(OpCodes.Ldfld, functionAddressField));
                processor.InsertBefore(branchTarget, processor.Create(OpCodes.Ldsfld, intPtrZeroFieldRef));
                processor.InsertBefore(branchTarget, processor.Create(OpCodes.Call, intPtrEqualsMethod));
                Instruction branchPosition = processor.Create(OpCodes.Ldarg_0);
                processor.InsertBefore(branchTarget, branchPosition);
                processor.InsertBefore(branchTarget, processor.Create(OpCodes.Ldarg_0));
                processor.InsertBefore(branchTarget, processor.Create(OpCodes.Call, typeInfo.IsInterface ? iinterfaceImplAddressGetter : uobjectAddressGetter));
                processor.InsertBefore(branchTarget, processor.Create(OpCodes.Ldstr, method.Name));
                processor.InsertBefore(branchTarget, processor.Create(OpCodes.Call, reflectionGetFunctionFromInstance));
                processor.InsertBefore(branchTarget, processor.Create(OpCodes.Stfld, functionAddressField));
                processor.InsertBefore(branchPosition, processor.Create(OpCodes.Brfalse_S, branchTarget));
            }

            InsertNativeFunctionInvokerIsValidSafeguard(type, functionInfo, method, injectedMembers, processor);

            // Add an object destroyed check if this isn't a delegate
            if (!isDelegate)
            {
                InsertObjectDestroyedCheck(typeInfo, processor);
            }
        }

        private void EmitFunctionParamInit(ILProcessor processor, ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo paramInfo,
            VariableDefinition paramsBuffer, FieldDefinition nativePropertyField)
        {
            if (!codeSettings.LazyFunctionParamInitDestroy && ManagedUnrealTypeInfo.PropertyRequiresInit(paramInfo))
            {
                // Ensure the checks to include the native field prop are in sync with this function
                Debug.Assert(nativePropertyField != null);

                // NativeReflection.InitializeValue_InContainer(XXXX_YYYY_PropertyAddress.Address, ParamsBuffer)
                EmitLdNativePropertyFieldAddress(processor, nativePropertyField);
                processor.Emit(OpCodes.Ldloc, paramsBuffer);
                processor.Emit(OpCodes.Call, reflectionInitializeValue_InContainer);
            }
        }

        private void EmitFunctionParamDestroy(ILProcessor processor, ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo paramInfo,
            VariableDefinition paramsBuffer, FieldDefinition nativePropertyField)
        {
            if (!codeSettings.LazyFunctionParamInitDestroy && ManagedUnrealTypeInfo.PropertyRequiresDestroy(paramInfo))
            {
                // Ensure the checks to include the native field prop are in sync with this function
                Debug.Assert(nativePropertyField != null);

                // NativeReflection.DestroyValue_InContainer(XXXX_YYYY_PropertyAddress.Address, ParamsBuffer)
                EmitLdNativePropertyFieldAddress(processor, nativePropertyField);
                processor.Emit(OpCodes.Ldloc, paramsBuffer);
                processor.Emit(OpCodes.Call, reflectionDestroyValue_InContainer);
            }
        }

        private Dictionary<ManagedUnrealPropertyInfo, ParameterDefinition> GetParamsFromMethod(TypeDefinition type,
            ManagedUnrealFunctionInfo functionInfo, MethodDefinition method)
        {
            return GetParamsFromMethod(type, functionInfo, method, null, false);
        }

        private Dictionary<ManagedUnrealPropertyInfo, ParameterDefinition> GetParamsFromMethod(TypeDefinition type,
            ManagedUnrealFunctionInfo functionInfo, MethodDefinition method, InjectedMembers injectedMembers, bool addFields)
        {
            Dictionary<ManagedUnrealPropertyInfo, ParameterDefinition> parameters = new Dictionary<ManagedUnrealPropertyInfo, ParameterDefinition>();
            foreach (ManagedUnrealPropertyInfo param in functionInfo.Params)
            {
                Type paramType = ManagedUnrealTypeInfo.GetTypeFromPropertyInfo(param);

                List<ParameterDefinition> foundParams = new List<ParameterDefinition>();
                foreach (ParameterDefinition paramDef in method.Parameters)
                {
                    if (paramDef.Name == param.Name)
                    {
                        foundParams.Add(paramDef);
                        break;
                    }
                }
                VerifySingleResult(foundParams, type, "Param count mismatch " + param.Name + " in delegate");

                if (addFields)
                {
                    FieldDefinition isValidField = AddIsValidField(type, functionInfo, param);
                    FieldDefinition offsetField = AddOffsetField(type, functionInfo, param);
                    FieldDefinition nativePropertyField = AddNativePropertyField(type, functionInfo, param);
                    injectedMembers.SetFunctionParamIsValid(functionInfo, param, isValidField);
                    injectedMembers.SetFunctionParamOffset(functionInfo, param, offsetField);
                    injectedMembers.SetFunctionParamPropertyAddress(functionInfo, param, nativePropertyField);
                }

                parameters.Add(param, foundParams[0]);
            }

            if (addFields && functionInfo.ReturnProp != null)
            {
                FieldDefinition isValidField = AddIsValidField(type, functionInfo, functionInfo.ReturnProp);
                FieldDefinition offsetField = AddOffsetField(type, functionInfo, functionInfo.ReturnProp);
                FieldDefinition nativePropertyField = AddNativePropertyField(type, functionInfo, functionInfo.ReturnProp);
                injectedMembers.SetFunctionParamIsValid(functionInfo, functionInfo.ReturnProp, isValidField);
                injectedMembers.SetFunctionParamOffset(functionInfo, functionInfo.ReturnProp, offsetField);
                injectedMembers.SetFunctionParamPropertyAddress(functionInfo, functionInfo.ReturnProp, nativePropertyField);
            }

            return parameters;
        }

        private void AddFunctionInvokerAttribute(MethodDefinition method, ManagedUnrealFunctionInfo functionInfo)
        {
            for (int i = method.CustomAttributes.Count - 1; i >= 0; i--)
            {
                // Type comparison doesn't seem to work. TODO: look into
                if (method.CustomAttributes[i].AttributeType.FullName == functionInvokerAttributeTypeRef.FullName)
                {
                    method.CustomAttributes.RemoveAt(i);
                    break;
                }
            }

            CustomAttribute functionInvokerAttribute = new CustomAttribute(functionInvokerAttributeTypeCtor);
            method.CustomAttributes.Add(functionInvokerAttribute);
            functionInvokerAttribute.ConstructorArguments.Clear();
            functionInvokerAttribute.ConstructorArguments.Add(new CustomAttributeArgument(stringTypeRef, functionInfo.Path));
        }
    }
}
