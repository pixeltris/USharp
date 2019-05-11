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
        private void RewriteClass(TypeDefinition type, ManagedUnrealTypeInfo classInfo)
        {
            InjectedMembers injectedMembers = new InjectedMembers(classInfo);

            if (classInfo.IsStructAsClass)
            {
                FieldDefinition structAddressField = new FieldDefinition(classInfo.Name + codeSettings.VarNames.StructAddress,
                    FieldAttributes.Public | FieldAttributes.Static, intPtrTypeRef);
                type.Fields.Add(structAddressField);
                injectedMembers.ClassAddress = structAddressField;

                FieldDefinition structIsValidField = new FieldDefinition(classInfo.Name + codeSettings.VarNames.IsValid,
                    FieldAttributes.Private | FieldAttributes.Static, int32TypeRef);
                type.Fields.Add(structIsValidField);
                injectedMembers.StructIsValid = structIsValidField;

                OverrideStructAsClassGetStructAddress(type, structAddressField);
            }

            // Moving default set has to happen before stripping to keep resolvability
            RemoveFieldDefaultSetterFromConstructor(type, classInfo);

            foreach (ManagedUnrealPropertyInfo propertyInfo in classInfo.Properties)
            {
                PropertyDefinition propertyDefinition = FindPropertyByName(type, propertyInfo.Name);

                if (!propertyInfo.IsField && !propertyInfo.IsBackingFieldPreStripped)
                {
                    StripBackingField(type, propertyInfo);
                }

                // Add offset field
                FieldDefinition isValidField = AddIsValidField(type, propertyInfo);
                FieldDefinition offsetField = AddOffsetField(type, propertyInfo);
                FieldDefinition nativePropertyField = AddNativePropertyField(type, propertyInfo);

                injectedMembers.SetPropertyIsValid(propertyInfo, isValidField);
                injectedMembers.SetPropertyOffset(propertyInfo, offsetField);
                injectedMembers.SetPropertyAddress(propertyInfo, nativePropertyField);

                FieldDefinition fixedSizeArrayField = null;
                if (propertyInfo.IsFixedSizeArray)
                {
                    // Add a field to store the fixed size array for the getter...technically we already had one of these which was
                    // the backing field that was stripped. Maybe just not strip the backing field rather than adding a new field
                    // of what should already exist.
                    fixedSizeArrayField = new FieldDefinition(
                        propertyInfo.Name + codeSettings.VarNames.FixedSizeArrayCached, FieldAttributes.Private, propertyDefinition.PropertyType);
                    type.Fields.Add(fixedSizeArrayField);
                }

                FieldDefinition collectionWrapperField = null;
                if (propertyInfo.IsCollection)
                {
                    // Add a field to store the collection wrapper for the getter
                    collectionWrapperField = new FieldDefinition(
                        propertyInfo.Name + codeSettings.VarNames.CollectionMarshaler, FieldAttributes.Private,
                        assembly.MainModule.ImportEx(ManagedUnrealTypeInfo.GetCollectionType(propertyInfo, ManagedUnrealMarshalerType.Default)));
                    type.Fields.Add(collectionWrapperField);
                }

                FieldDefinition delegateWrapperField = null;
                if (propertyInfo.IsDelegate)
                {
                    // Add a field to store the delegate wrapper for the getter
                    delegateWrapperField = new FieldDefinition(
                        propertyInfo.Name + codeSettings.VarNames.DelegateCached, FieldAttributes.Private,
                        assembly.MainModule.ImportEx(ManagedUnrealTypeInfo.GetTypeFromPropertyInfo(propertyInfo)));
                    type.Fields.Add(delegateWrapperField);
                }

                FieldDefinition cachedFTextField = null;
                if (propertyInfo.Type.TypeCode == EPropertyType.Text)
                {
                    // Add a field to store the cached FText for the getter / setter
                    cachedFTextField = new FieldDefinition(
                        propertyInfo.Name + codeSettings.VarNames.FTextCached, FieldAttributes.Private, ftextTypeRef);
                    type.Fields.Add(cachedFTextField);
                }
                
                if (propertyDefinition.GetMethod != null)
                {
                    if (propertyInfo.IsFixedSizeArray)
                    {
                        WriteGetterFixedSizeArray(type, classInfo, propertyInfo, propertyDefinition.GetMethod, isValidField, offsetField,
                            nativePropertyField, fixedSizeArrayField);
                    }
                    else if (propertyInfo.IsCollection)
                    {
                        // NOTE: To support type downgrading we would need to update any references - this likely wouldn't work well
                        // Downgrade the collection from List to IList, Dictionarty to IDictionary, HashSet to ISet
                        //Type downgradedCollection = GetDowngradedCollectionType(propertyInfo, ManagedUnrealMarshalerType.Default);
                        //propertyDefinition.PropertyType = assembly.MainModule.ImportEx(downgradedCollection);

                        WriteGetterCollection(classInfo, propertyInfo, propertyDefinition.GetMethod, isValidField, offsetField,
                            nativePropertyField, collectionWrapperField);
                    }
                    else if (propertyInfo.IsDelegate)
                    {
                        WriteGetterDelegate(classInfo, propertyInfo, propertyDefinition.GetMethod, isValidField, offsetField, delegateWrapperField);
                    }
                    else if (propertyInfo.Type.TypeCode == EPropertyType.Text)
                    {
                        WriteCachedFTextGetter(classInfo, propertyInfo, propertyDefinition.GetMethod, isValidField, offsetField, cachedFTextField);
                    }
                    else
                    {
                        WriteGetter(classInfo, propertyInfo, propertyDefinition.GetMethod, isValidField, offsetField, nativePropertyField);
                    }
                }
                if (propertyDefinition.SetMethod != null)
                {
                    if (propertyInfo.IsFixedSizeArray)
                    {
                        // Setter for fixed sized arrays isn't currently supported (use the indexer / SetValues method)
                        type.Methods.Remove(propertyDefinition.SetMethod);
                        propertyDefinition.SetMethod = null;
                    }
                    else if (propertyInfo.IsCollection)
                    {
                        // Setter for collections isn't currently supported. Use Clear/AddRange instead (this is what the
                        // setter would need to be recoded to if we ever add this support)
                        type.Methods.Remove(propertyDefinition.SetMethod);
                        propertyDefinition.SetMethod = null;
                    }
                    else if (propertyInfo.IsDelegate)
                    {
                        // Setter for delegates isn't currently supported.
                        type.Methods.Remove(propertyDefinition.SetMethod);
                        propertyDefinition.SetMethod = null;
                    }
                    else if (propertyInfo.Type.TypeCode == EPropertyType.Text)
                    {
                        WriteCachedFTextSetter(classInfo, propertyInfo, propertyDefinition.SetMethod, isValidField, offsetField, cachedFTextField);
                    }
                    else
                    {
                        WriteSetter(classInfo, propertyInfo, propertyDefinition.SetMethod, isValidField, offsetField, nativePropertyField);
                    }
                }

                AddPathAttribute(propertyDefinition, propertyInfo);
            }

            if (!classInfo.IsStructAsClass)
            {
                Dictionary<string, List<MethodDefinition>> methodsByName = new Dictionary<string, List<MethodDefinition>>();
                foreach (MethodDefinition method in type.Methods)
                {
                    List<MethodDefinition> methods;
                    if (!methodsByName.TryGetValue(method.Name, out methods))
                    {
                        methodsByName.Add(method.Name, methods = new List<MethodDefinition>());
                    }
                    methods.Add(method);
                }

                foreach (ManagedUnrealFunctionInfo functionInfo in classInfo.Functions)
                {
                    string functionName = functionInfo.Name;
                    if (codeSettings.UseExplicitImplementationMethods && functionInfo.IsBlueprintEvent && functionInfo.IsOverride)
                    {
                        functionName = functionInfo.Name + codeSettings.VarNames.ImplementationMethod;
                    }

                    List<MethodDefinition> methods;
                    methodsByName.TryGetValue(functionName, out methods);
                    VerifySingleResult(methods, type, "Function " + functionName + " (the reflection system doesn't support overloads");

                    FieldDefinition functionIsValid = AddIsValidField(type, functionInfo);
                    FieldDefinition functionAddressField = AddNativeFunctionField(type, functionInfo);
                    FieldDefinition paramsSizeField = AddParamsSizeField(type, functionInfo);
                    injectedMembers.SetFunctionIsValid(functionInfo, functionIsValid);
                    injectedMembers.SetFunctionAddress(functionInfo, functionAddressField);
                    injectedMembers.SetFunctionParamsSize(functionInfo, paramsSizeField);

                    // Validate the parameters and add the fields for the parameter offsets / addresses
                    GetParamsFromMethod(type, functionInfo, methods[0], injectedMembers, true);

                    if (functionInfo.IsBlueprintEvent || functionInfo.IsRPC)
                    {
                        bool perInstanceFunctionAddress = functionInfo.IsBlueprintEvent;
                        RewriteMethodAsUFunctionInvoke(type, classInfo, functionInfo, methods[0], injectedMembers, perInstanceFunctionAddress);
                    }
                    else
                    {
                        WriteFunctionInvoker(type, classInfo, functionInfo, methods[0], injectedMembers, false);
                    }

                    AddPathAttribute(methods[0], functionInfo);
                }
            }

            AddPathAttribute(type, classInfo);

            CreateLoadNativeTypeMethod(type, classInfo, injectedMembers);
        }

        private void WriteGetter(ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, MethodDefinition getter, 
            FieldDefinition isValidField, FieldDefinition offsetField, FieldDefinition nativePropertyField)
        {
            ILProcessor processor = BeginGetter(getter);
            Instruction[] loadBufferInstructions = GetLoadNativeBufferInstructions(typeInfo, processor, null, offsetField);
            MethodReference marshaler = GetFromNativeMarshaler(ManagedUnrealMarshalerType.Default, propertyInfo);
            WriteMarshalFromNative(processor, nativePropertyField, loadBufferInstructions, null, marshaler);
            EndGetter(typeInfo, propertyInfo, getter, isValidField, processor);
        }

        private void WriteGetterCollection(ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, MethodDefinition getter, 
            FieldDefinition isValidField, FieldDefinition offsetField, FieldDefinition nativePropertyField, FieldDefinition collectionWrapperField)
        {
            ILProcessor processor = BeginGetter(getter);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, collectionWrapperField);

            // Save the position of the branch instruction for later, when we have a reference to its target.
            // (will insert Brtrue_S before this instruction as a null check on the wrapper field)
            processor.Emit(OpCodes.Ldarg_0);
            Instruction branchPosition = processor.Body.Instructions[processor.Body.Instructions.Count - 1];

            processor.Emit(OpCodes.Ldc_I4_1);
            processor.Emit(OpCodes.Ldsfld, nativePropertyField);

            // New delegate marshaling code, this will use a cached delegate
            EmitCachedMarshalingDelegates(processor, propertyInfo, ManagedUnrealMarshalerType.Default);

            System.Reflection.ConstructorInfo collectionCtor;
            System.Reflection.MethodInfo fromNativeMethod;
            System.Reflection.MethodInfo toNativeMethod;
            GetInstancedMarshalerMethods(propertyInfo, ManagedUnrealMarshalerType.Default, out collectionCtor, out fromNativeMethod, out toNativeMethod);

            int numFromNativeParams = fromNativeMethod.GetParameters().Length;

            // Create an instance of the collection marshaler and store in the member field
            processor.Emit(OpCodes.Newobj, assembly.MainModule.ImportEx(collectionCtor));
            processor.Emit(OpCodes.Stfld, collectionWrapperField);

            // Store the branch destination
            processor.Emit(OpCodes.Ldarg_0);
            Instruction branchTarget = processor.Body.Instructions[processor.Body.Instructions.Count - 1];
            processor.Emit(OpCodes.Ldfld, collectionWrapperField);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Call, uobjectAddressGetter);
            processor.Emit(OpCodes.Ldsfld, offsetField);
            processor.Emit(OpCodes.Call, intPtrAddMethod);
            if (numFromNativeParams == marshalerFromNativeParamCount)
            {
                // arrayIndex, prop (IntPtr.Zero as prop is already known by the collection marshaler), this
                processor.Emit(OpCodes.Ldc_I4_0);
                processor.Emit(OpCodes.Ldnull);
                processor.Emit(OpCodes.Ldarg_0);
            }
            else
            {
                System.Diagnostics.Debug.Assert(numFromNativeParams == marshalerFromNativeParamCountMin);
            }
            processor.Emit(OpCodes.Callvirt, assembly.MainModule.ImportEx(fromNativeMethod));
            // Now insert the branch
            Instruction branchInstruction = processor.Create(OpCodes.Brtrue_S, branchTarget);
            processor.InsertBefore(branchPosition, branchInstruction);

            EndGetter(typeInfo, propertyInfo, getter, isValidField, processor);
        }

        private void WriteGetterDelegate(ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, MethodDefinition getter,
            FieldDefinition isValidField, FieldDefinition offsetField, FieldDefinition delegateWrapperField)
        {
            ILProcessor processor = BeginGetter(getter);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, delegateWrapperField);

            // Save the position of the branch instruction for later, when we have a reference to its target.
            // (will insert Brtrue_S before this instruction as a null check on the wrapper field)
            processor.Emit(OpCodes.Ldarg_0);
            Instruction branchPosition = processor.Body.Instructions[processor.Body.Instructions.Count - 1];

            Type delegateType = ManagedUnrealTypeInfo.GetTypeFromPropertyInfo(propertyInfo);

            // Find the default constructor / SetAddress method for the delegate
            System.Reflection.ConstructorInfo delegateCtor = delegateType.GetConstructor(Type.EmptyTypes);
            System.Reflection.MethodInfo setAddressMethod = delegateType.GetMethod("SetAddress");

            // Create an instance of the delegate and store in the member field
            processor.Emit(OpCodes.Newobj, assembly.MainModule.ImportEx(delegateCtor));
            processor.Emit(OpCodes.Stfld, delegateWrapperField);

            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, delegateWrapperField);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Call, uobjectAddressGetter);
            processor.Emit(OpCodes.Ldsfld, offsetField);
            processor.Emit(OpCodes.Call, intPtrAddMethod);
            processor.Emit(OpCodes.Callvirt, assembly.MainModule.ImportEx(setAddressMethod));

            processor.Emit(OpCodes.Ldarg_0);
            Instruction branchTarget = processor.Body.Instructions[processor.Body.Instructions.Count - 1];
            processor.Emit(OpCodes.Ldfld, delegateWrapperField);

            // Insert the branch
            Instruction branchInstruction = processor.Create(OpCodes.Brtrue_S, branchTarget);
            processor.InsertBefore(branchPosition, branchInstruction);

            EndGetter(typeInfo, propertyInfo, getter, isValidField, processor);
        }

        private void WriteCachedFTextGetter(ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, MethodDefinition getter,
            FieldDefinition isValidField, FieldDefinition offsetField, FieldDefinition cachedFTextField)
        {
            ILProcessor processor = BeginGetter(getter);
            WriteCachedFTextGetterSetter(offsetField, cachedFTextField, processor,
                new Instruction[] 
                {
                    processor.Create(OpCodes.Ldfld, cachedFTextField)
                });
            EndGetter(typeInfo, propertyInfo, getter, isValidField, processor);
        }

        private void WriteCachedFTextSetter(ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, MethodDefinition setter,
            FieldDefinition isValidField, FieldDefinition offsetField, FieldDefinition cachedFTextField)
        {
            ILProcessor processor = BeginSetter(setter);
            WriteCachedFTextGetterSetter(offsetField, cachedFTextField, processor,
                new Instruction[]
                {
                    processor.Create(OpCodes.Ldfld, cachedFTextField),
                    processor.Create(OpCodes.Ldarg_1),
                    processor.Create(OpCodes.Callvirt, ftextCopyFrom)
                });
            EndSetter(typeInfo, propertyInfo, setter, isValidField, processor);
        }

        private void WriteCachedFTextGetterSetter(FieldDefinition offsetField, FieldDefinition cachedFTextField, 
            ILProcessor processor, Instruction[] instructions)
        {
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, cachedFTextField);

            // Save the position of the branch instruction for later, when we have a reference to its target.
            // (will insert Brtrue_S before this instruction as a null check on the wrapper field)
            processor.Emit(OpCodes.Ldarg_0);
            Instruction branchPosition = processor.Body.Instructions[processor.Body.Instructions.Count - 1];

            // Create an instance of the FText and store in the member field
            processor.Emit(OpCodes.Ldarg_0);// IntPtr.Add(Address, XXXX_Offset)
            processor.Emit(OpCodes.Call, uobjectAddressGetter);
            processor.Emit(OpCodes.Ldsfld, offsetField);
            processor.Emit(OpCodes.Call, intPtrAddMethod);
            processor.Emit(OpCodes.Ldc_I4_0);// , false);
            processor.Emit(OpCodes.Newobj, ftextCtor);
            processor.Emit(OpCodes.Stfld, cachedFTextField);// XXXX_TextCached = ...

            processor.Emit(OpCodes.Ldarg_0);
            Instruction branchTarget = processor.Body.Instructions[processor.Body.Instructions.Count - 1];
            foreach (Instruction instruction in instructions)
            {
                processor.Append(instruction);
            }

            // Insert the branch
            Instruction branchInstruction = processor.Create(OpCodes.Brtrue_S, branchTarget);
            processor.InsertBefore(branchPosition, branchInstruction);
        }

        private void WriteGetterFixedSizeArray(TypeDefinition type, ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, 
            MethodDefinition getter, FieldDefinition isValidField, FieldDefinition offsetField, FieldDefinition nativePropertyField, 
            FieldDefinition fixedSizeArrayField)
        {
            ILProcessor processor = BeginGetter(getter);
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, fixedSizeArrayField);

            // Save the position of the branch instruction for later, when we have a reference to its target.
            // (will insert Brtrue_S before this instruction as a null check on the wrapper field)
            processor.Emit(OpCodes.Ldarg_0);
            Instruction branchPosition = processor.Body.Instructions[processor.Body.Instructions.Count - 1];

            MethodReference constructor = null;
            foreach (MethodDefinition ctor in fixedSizeArrayField.FieldType.Resolve().GetConstructors())
            {
                if (ctor.Parameters.Count == 3)
                {
                    constructor = ctor;
                    break;
                }
            }
            VerifyNonNull(constructor, type, "constructor for " + fixedSizeArrayField.FieldType.FullName);
            constructor = GetConstrainedGenericTypeCtor(fixedSizeArrayField.FieldType, constructor);

            // new TFixedSizeArray<XXXX>(IntPtr.Add(Address, XXXX_Offset), XXXX_PropertyAddress, this);
            // IntPtr.Add(this.Address, XXXX_Offset)
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Call, uobjectAddressGetter);
            processor.Emit(OpCodes.Ldsfld, offsetField);
            processor.Emit(OpCodes.Call, intPtrAddMethod);
            // XXXX_PropertyAddress
            processor.Emit(OpCodes.Ldsfld, nativePropertyField);
            // this
            processor.Emit(OpCodes.Ldarg_0);

            // Create an instance of the array and store in the member field
            processor.Emit(OpCodes.Newobj, constructor);
            processor.Emit(OpCodes.Stfld, fixedSizeArrayField);

            // return XXXX;
            processor.Emit(OpCodes.Ldarg_0);
            Instruction branchTarget = processor.Body.Instructions[processor.Body.Instructions.Count - 1];
            processor.Emit(OpCodes.Ldfld, fixedSizeArrayField);

            // Insert the branch (before the return statement)
            Instruction branchInstruction = processor.Create(OpCodes.Brtrue_S, branchTarget);
            processor.InsertBefore(branchPosition, branchInstruction);

            EndGetter(typeInfo, propertyInfo, getter, isValidField, processor);
        }

        private ILProcessor BeginGetter(MethodDefinition getter)
        {
            ILProcessor processor = InitPropertyAccessor(getter);            
            return processor;
        }

        private void EndGetter(ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, MethodDefinition getter, 
            FieldDefinition isValidField, ILProcessor processor)
        {
            processor.Emit(OpCodes.Ret);
            InsertPropertyIsValidSafeguard(propertyInfo, getter, true, isValidField, processor);
            InsertObjectDestroyedCheck(typeInfo, processor);
            getter.Body.OptimizeMacros();
        }

        private void WriteSetter(ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, MethodDefinition setter, 
            FieldDefinition isValidField, FieldDefinition offsetField, FieldDefinition nativePropertyField)
        {
            ILProcessor processor = BeginSetter(setter);
            Instruction loadValue = processor.Create(OpCodes.Ldarg_1);
            Instruction[] loadBufferInstructions = GetLoadNativeBufferInstructions(typeInfo, processor, null, offsetField);
            MethodReference marshaler = GetToNativeMarshaler(ManagedUnrealMarshalerType.Default, propertyInfo);
            WriteMarshalToNative(processor, nativePropertyField, loadBufferInstructions, null, loadValue, marshaler);
            EndSetter(typeInfo, propertyInfo, setter, isValidField, processor);
        }

        private ILProcessor BeginSetter(MethodDefinition setter)
        {
            ILProcessor processor = InitPropertyAccessor(setter);
            return processor;
        }

        private void EndSetter(ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, MethodDefinition setter, 
            FieldDefinition isValidField, ILProcessor processor)
        {
            processor.Emit(OpCodes.Ret);            
            InsertPropertyIsValidSafeguard(propertyInfo, setter, false, isValidField, processor);
            InsertObjectDestroyedCheck(typeInfo, processor);
            setter.Body.OptimizeMacros();
        }

        private ILProcessor InitPropertyAccessor(MethodDefinition method)
        {
            method.Body = new MethodBody(method);
            method.CustomAttributes.Clear();
            ILProcessor processor = method.Body.GetILProcessor();
            method.Body.Instructions.Clear();
            return processor;
        }

        /// <summary>
        /// Inserts an IsValid safeguard on the given property getter/setter
        /// </summary>
        private void InsertPropertyIsValidSafeguard(ManagedUnrealPropertyInfo propertyInfo, MethodDefinition getterSetterMethod,
            bool isGetter, FieldDefinition isValidField, ILProcessor processor)
        {
            if (!codeSettings.GenerateIsValidSafeguards || isValidField == null)
            {
                return;
            }

            // If the property isn't valid create a log and return a default value
            // if (!propertyName_IsValid)
            // {
            //     NativeReflection.LogInvalidStructAccessed("XXXX");
            //     return default(XXXX); (setter - return;)
            // }

            List<Instruction> instructions = new List<Instruction>();

            Instruction branchTarget = processor.Body.Instructions[0];

            instructions.Add(processor.Create(OpCodes.Ldsfld, isValidField));
            instructions.Add(processor.Create(OpCodes.Brtrue, branchTarget));
            instructions.Add(processor.Create(OpCodes.Ldstr, propertyInfo.Path));
            instructions.Add(processor.Create(OpCodes.Call, reflectionLogInvalidPropertyAccessedMethod));

            if (isGetter)
            {
                // Push the default(XXXX) on the stack for the return value
                VariableDefinition resultVar = new VariableDefinition(getterSetterMethod.ReturnType);
                getterSetterMethod.Body.Variables.Add(resultVar);
                instructions.AddRange(CreateSetDefaultValue(processor, propertyInfo.Type.TypeCode, resultVar));
                instructions.Add(processor.Create(OpCodes.Ldloc, resultVar));
            }

            instructions.Add(processor.Create(OpCodes.Ret));

            // Insert the instructions at the start
            InsertInstructionsAt(processor, 0, instructions.ToArray());
        }
    }
}
