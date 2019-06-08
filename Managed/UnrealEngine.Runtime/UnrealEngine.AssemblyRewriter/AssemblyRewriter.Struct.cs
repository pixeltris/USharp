using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    partial class AssemblyRewriter
    {
        private void PreRewriteStruct(TypeDefinition type, ManagedUnrealTypeInfo structInfo)
        {
            MethodDefinition copyMethod;
            MethodDefinition fromNativeMinStaticMethod;
            MethodDefinition toNativeMinStaticMethod;
            MethodDefinition fromNativeStaticMethod;
            MethodDefinition toNativeStaticMethod;
            MethodDefinition toNativeMethod;
            MethodDefinition intPtrConstructorMethod;
            FindStructMethodsToInject(type,
                out copyMethod,
                out fromNativeMinStaticMethod, out toNativeMinStaticMethod,
                out fromNativeStaticMethod, out toNativeStaticMethod,
                out toNativeMethod, out intPtrConstructorMethod);

            if (copyMethod != null ||
                fromNativeMinStaticMethod != null || toNativeMinStaticMethod != null ||
                fromNativeStaticMethod != null || toNativeStaticMethod != null ||
                toNativeMethod != null || intPtrConstructorMethod != null)
            {
                throw new RewriteException(type, "Struct has unexpected marshaling method / constructor which should be generated only");
            }

            // Create empty methods to be filled out at a later point

            copyMethod = new MethodDefinition(codeSettings.VarNames.StructCopy, MethodAttributes.Public, type);
            type.Methods.Add(copyMethod);

            if (!structInfo.IsBlittable)
            {
                fromNativeMinStaticMethod = new MethodDefinition("FromNative", MethodAttributes.Public | MethodAttributes.Static, type);
                fromNativeMinStaticMethod.Parameters.Add(new ParameterDefinition("nativeBuffer", ParameterAttributes.None, intPtrTypeRef));
                type.Methods.Add(fromNativeMinStaticMethod);

                toNativeMinStaticMethod = new MethodDefinition("ToNative", MethodAttributes.Public | MethodAttributes.Static,
                    assembly.MainModule.ImportEx(typeof(void)));
                toNativeMinStaticMethod.Parameters.Add(new ParameterDefinition("nativeBuffer", ParameterAttributes.None, intPtrTypeRef));
                toNativeMinStaticMethod.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, type));
                type.Methods.Add(toNativeMinStaticMethod);

                fromNativeStaticMethod = new MethodDefinition("FromNative", MethodAttributes.Public | MethodAttributes.Static, type);
                fromNativeStaticMethod.Parameters.Add(new ParameterDefinition("nativeBuffer", ParameterAttributes.None, intPtrTypeRef));
                fromNativeStaticMethod.Parameters.Add(new ParameterDefinition("arrayIndex", ParameterAttributes.None, int32TypeRef));
                fromNativeStaticMethod.Parameters.Add(new ParameterDefinition("prop", ParameterAttributes.None, intPtrTypeRef));
                type.Methods.Add(fromNativeStaticMethod);

                toNativeStaticMethod = new MethodDefinition("ToNative", MethodAttributes.Public | MethodAttributes.Static,
                    assembly.MainModule.ImportEx(typeof(void)));
                toNativeStaticMethod.Parameters.Add(new ParameterDefinition("nativeBuffer", ParameterAttributes.None, intPtrTypeRef));
                toNativeStaticMethod.Parameters.Add(new ParameterDefinition("arrayIndex", ParameterAttributes.None, int32TypeRef));
                toNativeStaticMethod.Parameters.Add(new ParameterDefinition("prop", ParameterAttributes.None, intPtrTypeRef));
                toNativeStaticMethod.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, type));
                type.Methods.Add(toNativeStaticMethod);

                toNativeMethod = new MethodDefinition("ToNative", MethodAttributes.Public, assembly.MainModule.ImportEx(typeof(void)));
                toNativeMethod.Parameters.Add(new ParameterDefinition("nativeStruct", ParameterAttributes.None, intPtrTypeRef));
                type.Methods.Add(toNativeMethod);

                intPtrConstructorMethod = new MethodDefinition(".ctor",
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    assembly.MainModule.ImportEx(typeof(void)));
                intPtrConstructorMethod.Parameters.Add(new ParameterDefinition("nativeStruct", ParameterAttributes.None, intPtrTypeRef));
                type.Methods.Add(intPtrConstructorMethod);
            }
        }

        private void RewriteStruct(TypeDefinition type, ManagedUnrealTypeInfo structInfo)
        {
            MethodDefinition copyMethod;
            MethodDefinition fromNativeMinStaticMethod;
            MethodDefinition toNativeMinStaticMethod;
            MethodDefinition fromNativeStaticMethod;
            MethodDefinition toNativeStaticMethod;
            MethodDefinition toNativeMethod;
            MethodDefinition intPtrConstructorMethod;
            FindStructMethodsToInject(type, out copyMethod,
                out fromNativeMinStaticMethod, out toNativeMinStaticMethod,
                out fromNativeStaticMethod, out toNativeStaticMethod,
                out toNativeMethod, out intPtrConstructorMethod);

            if (copyMethod == null)
            {
                throw new RewriteException(type, "Struct Copy method is null");
            }

            WriteStructCopyMethodBody(type, structInfo, copyMethod);

            InjectedMembers injectedMembers = new InjectedMembers(structInfo);

            FieldDefinition structSizeField = new FieldDefinition(structInfo.Name + codeSettings.VarNames.StructSize,
                FieldAttributes.Private | FieldAttributes.Static, int32TypeRef);
            type.Fields.Add(structSizeField);
            injectedMembers.StructSize = structSizeField;

            if (!structInfo.IsBlittable)
            {
                if (fromNativeMinStaticMethod == null || toNativeMinStaticMethod == null ||
                    fromNativeStaticMethod == null || toNativeStaticMethod == null ||
                    toNativeMethod == null || intPtrConstructorMethod == null)
                {
                    throw new RewriteException(type, "Struct marshaling method is null");
                }

                FieldDefinition structIsValidField = new FieldDefinition(structInfo.Name + codeSettings.VarNames.IsValid,
                    FieldAttributes.Private | FieldAttributes.Static, int32TypeRef);
                type.Fields.Add(structIsValidField);
                injectedMembers.StructIsValid = structIsValidField;

                ILProcessor processor = fromNativeMinStaticMethod.Body.GetILProcessor();
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Newobj, intPtrConstructorMethod);
                processor.Emit(OpCodes.Ret);
                FinalizeMethod(fromNativeMinStaticMethod);

                processor = toNativeMinStaticMethod.Body.GetILProcessor();
                processor.Emit(OpCodes.Ldarga, toNativeStaticMethod.Parameters[1]);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, toNativeMethod);
                processor.Emit(OpCodes.Ret);
                FinalizeMethod(toNativeMinStaticMethod);

                processor = fromNativeStaticMethod.Body.GetILProcessor();
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldarg_1);
                processor.Emit(OpCodes.Ldsfld, structSizeField);
                processor.Emit(OpCodes.Mul);
                processor.Emit(OpCodes.Call, intPtrAddMethod);
                processor.Emit(OpCodes.Newobj, intPtrConstructorMethod);
                processor.Emit(OpCodes.Ret);
                FinalizeMethod(fromNativeStaticMethod);

                processor = toNativeStaticMethod.Body.GetILProcessor();
                processor.Emit(OpCodes.Ldarga, toNativeStaticMethod.Parameters[3]);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldarg_1);
                processor.Emit(OpCodes.Ldsfld, structSizeField);
                processor.Emit(OpCodes.Mul);
                processor.Emit(OpCodes.Call, intPtrAddMethod);
                processor.Emit(OpCodes.Call, toNativeMethod);
                processor.Emit(OpCodes.Ret);
                FinalizeMethod(toNativeStaticMethod);

                ILProcessor ctorProcessor = intPtrConstructorMethod.Body.GetILProcessor();
                ILProcessor toNativeProcessor = toNativeMethod.Body.GetILProcessor();
                OpCode loadBufferOp = OpCodes.Ldarg_1;

                Dictionary<string, FieldDefinition> fieldsByName = new Dictionary<string, FieldDefinition>();
                foreach (FieldDefinition fieldDef in type.Fields)
                {
                    fieldsByName.Add(fieldDef.Name, fieldDef);
                }

                foreach (ManagedUnrealPropertyInfo propertyInfo in structInfo.Properties)
                {
                    // Add offset field
                    FieldDefinition isValidField = AddIsValidField(type, propertyInfo);
                    FieldDefinition offsetField = AddOffsetField(type, propertyInfo);
                    FieldDefinition nativePropertyField = AddNativePropertyField(type, propertyInfo);

                    injectedMembers.SetPropertyIsValid(propertyInfo, isValidField);
                    injectedMembers.SetPropertyOffset(propertyInfo, offsetField);
                    injectedMembers.SetPropertyAddress(propertyInfo, nativePropertyField);

                    // Find the property
                    FieldDefinition field = null;
                    fieldsByName.TryGetValue(propertyInfo.Name, out field);
                    VerifyNonNull(field, type, "field " + propertyInfo.Name);

                    // FromNative marshaling
                    {
                        VariableDefinition marshalerVar = EmitCreateMarshaler(ctorProcessor, ctorProcessor.Create(loadBufferOp), 
                            offsetField, nativePropertyField, propertyInfo);
                        if (marshalerVar != null)
                        {
                            intPtrConstructorMethod.Body.Variables.Add(marshalerVar);
                        }

                        EmitLoad(ctorProcessor, ctorProcessor.Create(loadBufferOp), offsetField, nativePropertyField, structInfo, 
                            propertyInfo, marshalerVar, field);
                    }

                    // ToNative marshaling
                    {
                        VariableDefinition marshalerVar = EmitCreateMarshaler(toNativeProcessor, toNativeProcessor.Create(loadBufferOp), 
                            offsetField, nativePropertyField, propertyInfo);
                        if (marshalerVar != null)
                        {
                            toNativeMethod.Body.Variables.Add(marshalerVar);
                        }

                        EmitStore(toNativeProcessor, toNativeProcessor.Create(loadBufferOp), offsetField, nativePropertyField, 
                            structInfo, propertyInfo, marshalerVar, field);
                    }

                    AddPathAttribute(field, propertyInfo);
                }

                ctorProcessor.Emit(OpCodes.Ret);
                toNativeProcessor.Emit(OpCodes.Ret);
                InsertStructMarshalerIsValidSafeguard(type, structInfo, intPtrConstructorMethod, injectedMembers, ctorProcessor, true);
                InsertStructMarshalerIsValidSafeguard(type, structInfo, toNativeMethod, injectedMembers, toNativeProcessor, false);
                FinalizeMethod(intPtrConstructorMethod);
                FinalizeMethod(toNativeMethod);
            }
            else
            {
                foreach (ManagedUnrealPropertyInfo propertyInfo in structInfo.Properties)
                {
                    // Find the property
                    List<FieldDefinition> fieldDefinitions = new List<FieldDefinition>();
                    foreach (FieldDefinition fieldDef in type.Fields)
                    {
                        if (fieldDef.Name == propertyInfo.Name)
                        {
                            fieldDefinitions.Add(fieldDef);
                        }
                    }
                    VerifySingleResult(fieldDefinitions, type, "field " + propertyInfo.Name);

                    AddPathAttribute(fieldDefinitions[0], propertyInfo);
                }
            }

            AddPathAttribute(type, structInfo);

            CreateLoadNativeTypeMethod(type, null, structInfo, injectedMembers);
        }

        /// <summary>
        /// Inserts an IsValid safeguard on the given non-blittable struct marshaling method (ctor (FromNative) / ToNative)
        /// </summary>
        private void InsertStructMarshalerIsValidSafeguard(TypeDefinition type, ManagedUnrealTypeInfo structInfo,
            MethodDefinition method, InjectedMembers injectedMembers, ILProcessor processor, bool isCtor)
        {
            if (!codeSettings.GenerateIsValidSafeguards)
            {
                return;
            }

            FieldDefinition structIsValidField = injectedMembers.StructIsValid;
            if (structIsValidField == null)
            {
                return;
            }

            // If the struct isn't valid create a log and set any out params to default values
            // if (!structName_IsValid)
            // {
            //     NativeReflection.LogInvalidStructAccessed("XXXX");
            //     PropXXX = default(XXXX); (constructor only)
            //     return;
            // }

            List<Instruction> instructions = new List<Instruction>();

            Instruction branchTarget = processor.Body.Instructions[0];

            instructions.Add(processor.Create(OpCodes.Ldsfld, structIsValidField));
            instructions.Add(processor.Create(OpCodes.Brtrue, branchTarget));
            instructions.Add(processor.Create(OpCodes.Ldstr, structInfo.Path));
            instructions.Add(processor.Create(OpCodes.Call, reflectionLogInvalidStructAccessedMethod));

            if (isCtor)
            {
                // Set struct members to default(XXXX)
                instructions.AddRange(CreateStructPropertyDefaults(processor, structInfo, type));
            }

            instructions.Add(processor.Create(OpCodes.Ret));

            // Insert the instructions at the start
            InsertInstructionsAt(processor, 0, instructions.ToArray());
        }

        private void WriteStructCopyMethodBody(TypeDefinition type, ManagedUnrealTypeInfo structInfo, MethodDefinition copyMethod)
        {
            VariableDefinition resultVar = new VariableDefinition(type);
            copyMethod.Body.Variables.Add(resultVar);

            ILProcessor processor = copyMethod.Body.GetILProcessor();
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldobj, type);
            processor.Emit(OpCodes.Stloc, resultVar);

            Instruction lastBranchPosition = null;

            foreach (ManagedUnrealPropertyInfo propertyInfo in structInfo.Properties)
            {
                if (propertyInfo.IsCollection)
                {
                    FieldDefinition propField = null;

                    foreach (FieldDefinition field in type.Fields)
                    {
                        if (field.Name == propertyInfo.Name)
                        {
                            propField = field;
                            break;
                        }
                    }

                    if (propField == null)
                    {
                        continue;
                    }

                    Type collectionType = GetTypeFromTypeDefinition(propField.FieldType.Resolve());
                    collectionType = ManagedUnrealTypeInfo.MakeGenericTypeWithPropertyArgs(collectionType, propertyInfo);

                    System.Reflection.ConstructorInfo collectionCtor = null;
                    foreach (System.Reflection.ConstructorInfo ctor in collectionType.GetConstructors())
                    {
                        System.Reflection.ParameterInfo[] ctorParams = ctor.GetParameters();
                        if (ctorParams.Length == 1)
                        {
                            if (propertyInfo.Type.TypeCode == EPropertyType.Map)
                            {
                                if (ctorParams[0].ParameterType.Name.Contains("IDictionary") ||
                                    ctorParams[0].ParameterType.Name.Contains("IReadOnlyDictionary"))
                                {
                                    collectionCtor = ctor;
                                }
                            }
                            else if (ctorParams[0].ParameterType.Name.Contains("IEnumerable"))
                            {
                                collectionCtor = ctor;
                            }
                        }
                    }

                    if (collectionCtor == null)
                    {
                        continue;
                    }

                    processor.Emit(OpCodes.Ldarg_0);
                    if (lastBranchPosition != null)
                    {
                        // Branch to be inserted with a branch target of the just written instruction
                        processor.InsertAfter(lastBranchPosition, processor.Create(OpCodes.Brfalse, processor.Body.Instructions[processor.Body.Instructions.Count - 1]));
                        lastBranchPosition = null;
                    }
                    processor.Emit(OpCodes.Ldfld, propField);

                    lastBranchPosition = processor.Body.Instructions[processor.Body.Instructions.Count - 1];

                    processor.Emit(OpCodes.Ldloca, resultVar);
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldfld, propField);
                    processor.Emit(OpCodes.Newobj, assembly.MainModule.ImportEx(collectionCtor));
                    processor.Emit(OpCodes.Stfld, propField);
                }
            }

            processor.Emit(OpCodes.Ldloc, resultVar);
            if (lastBranchPosition != null)
            {
                // Branch to be inserted with a branch target of the just written instruction
                processor.InsertAfter(lastBranchPosition, processor.Create(OpCodes.Brfalse, processor.Body.Instructions[processor.Body.Instructions.Count - 1]));
                lastBranchPosition = null;
            }
            processor.Emit(OpCodes.Ret);

            FinalizeMethod(copyMethod);
        }

        private void FindStructMethodsToInject(TypeDefinition type,
            out MethodDefinition copyMethod,
            out MethodDefinition fromNativeMinStaticMethod,
            out MethodDefinition toNativeMinStaticMethod,
            out MethodDefinition fromNativeStaticMethod,
            out MethodDefinition toNativeStaticMethod,
            out MethodDefinition toNativeMethod,
            out MethodDefinition intPtrConstructorMethod)
        {
            copyMethod = null;
            fromNativeMinStaticMethod = null;
            toNativeMinStaticMethod = null;
            fromNativeStaticMethod = null;
            toNativeStaticMethod = null;
            toNativeMethod = null;
            intPtrConstructorMethod = null;

            List<MethodDefinition> intPtrConstructors = new List<MethodDefinition>();
            foreach (MethodDefinition ctor in type.GetConstructors())
            {
                if (ctor.Parameters.Count == 1 && ctor.Parameters[0].ParameterType.FullName == typeof(IntPtr).FullName)
                {
                    intPtrConstructors.Add(ctor);
                }
            }

            List<MethodDefinition> copyMethods = new List<MethodDefinition>();
            List<MethodDefinition> fromNativeMinStaticMethods = new List<MethodDefinition>();
            List<MethodDefinition> toNativeMinStaticMethods = new List<MethodDefinition>();
            List<MethodDefinition> fromNativeStaticMethods = new List<MethodDefinition>();
            List<MethodDefinition> toNativeStaticMethods = new List<MethodDefinition>();
            List<MethodDefinition> toNativeMethods = new List<MethodDefinition>();
            List<MethodDefinition> intPtrCtorMethods = new List<MethodDefinition>();

            foreach (MethodDefinition method in type.Methods)
            {
                if (method.Name == "FromNative")
                {
                    if (method.IsStatic && method.Parameters.Count == marshalerFromNativeParamCountMin &&
                        method.Parameters[0].ParameterType.FullName == typeof(IntPtr).FullName)
                    {
                        fromNativeMinStaticMethods.Add(method);
                    }
                    if (method.IsStatic && method.Parameters.Count == marshalerFromNativeParamCount &&
                        method.Parameters[0].ParameterType.FullName == typeof(IntPtr).FullName &&
                        method.Parameters[1].ParameterType.FullName == typeof(int).FullName &&
                        method.Parameters[2].ParameterType.FullName == typeof(IntPtr).FullName)
                    {
                        fromNativeStaticMethods.Add(method);
                    }
                }
                else if (method.Name == "ToNative")
                {
                    if (!method.IsStatic && method.Parameters.Count == 1 &&
                        method.Parameters[0].ParameterType.FullName == typeof(IntPtr).FullName)
                    {
                        toNativeMethods.Add(method);
                    }
                    if (method.IsStatic && method.Parameters.Count == marshalerToNativeParamCountMin &&
                        method.Parameters[0].ParameterType.FullName == typeof(IntPtr).FullName)
                    {
                        toNativeMinStaticMethods.Add(method);
                    }
                    if (method.IsStatic && method.Parameters.Count == marshalerToNativeParamCount &&
                        method.Parameters[0].ParameterType.FullName == typeof(IntPtr).FullName &&
                        method.Parameters[1].ParameterType.FullName == typeof(int).FullName &&
                        method.Parameters[2].ParameterType.FullName == typeof(IntPtr).FullName &&
                        method.Parameters[3].ParameterType.FullName == type.FullName)
                    {
                        toNativeStaticMethods.Add(method);
                    }
                }
                else if (method.Name == ".ctor")
                {
                    if (method.Parameters.Count == 1 && method.Parameters[0].ParameterType.FullName == typeof(IntPtr).FullName)
                    {
                        intPtrCtorMethods.Add(method);
                    }
                }
                else if (method.Name == codeSettings.VarNames.StructCopy)
                {
                    if (!method.IsStatic && method.Parameters.Count == 0 && method.ReturnType.FullName == type.FullName)
                    {
                        copyMethods.Add(method);
                    }
                }
            }

            if (copyMethods.Count == 1)
            {
                copyMethod = copyMethods[0];
            }
            if (fromNativeMinStaticMethods.Count == 1)
            {
                fromNativeMinStaticMethod = fromNativeMinStaticMethods[0];
            }
            if (toNativeMinStaticMethods.Count == 1)
            {
                toNativeMinStaticMethod = toNativeMinStaticMethods[0];
            }
            if (fromNativeStaticMethods.Count == 1)
            {
                fromNativeStaticMethod = fromNativeStaticMethods[0];
            }
            if (toNativeStaticMethods.Count == 1)
            {
                toNativeStaticMethod = toNativeStaticMethods[0];
            }
            if (toNativeMethods.Count == 1)
            {
                toNativeMethod = toNativeMethods[0];
            }
            if (intPtrCtorMethods.Count == 1)
            {
                intPtrConstructorMethod = intPtrCtorMethods[0];
            }
        }

        private void OverrideStructAsClassGetStructAddress(TypeDefinition type, FieldDefinition structAddressField)
        {
            string getStructAddressMethodName = "GetStructAddress";

            List<MethodDefinition> getInvokerFuncs = new List<MethodDefinition>();
            foreach (MethodDefinition method in type.Methods)
            {
                if (method.Name == getStructAddressMethodName)
                {
                    getInvokerFuncs.Add(method);
                }
            }
            VerifyNoResults(getInvokerFuncs, type, getStructAddressMethodName + " is already implemented in struct");

            MethodDefinition baseGetStructAddressMethod = FindBaseMethodByName(type, getStructAddressMethodName);
            MethodDefinition getStructAddressMethod = CopyMethod(baseGetStructAddressMethod, true);
            type.Methods.Add(getStructAddressMethod);

            // override IntPtr GetStructAddress() { return XXXX_StructAddress; }
            ILProcessor processor = getStructAddressMethod.Body.GetILProcessor();
            processor.Emit(OpCodes.Ldsfld, structAddressField);
            processor.Emit(OpCodes.Ret);
            FinalizeMethod(getStructAddressMethod);
        }
    }
}
