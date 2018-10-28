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
        private void WriteMarshalFromNative(ILProcessor processor, FieldDefinition nativePropertyField, Instruction[] loadBufferPtr,
            Instruction loadArrayIndex, MethodReference fromNativeMethod)
        {
            foreach (Instruction instruction in loadBufferPtr)
            {
                processor.Append(instruction);
            }
            EmitAdditionalMarshalingArgs(processor, false, fromNativeMethod, nativePropertyField, loadArrayIndex);
            processor.Emit(OpCodes.Call, fromNativeMethod);
        }

        private void WriteMarshalToNative(ILProcessor processor, FieldDefinition nativePropertyField, Instruction[] loadBufferPtr,
            Instruction loadArrayIndex, Instruction loadSource, MethodReference toNativeMethod)
        {
            WriteMarshalToNative(processor, nativePropertyField, loadBufferPtr, loadArrayIndex, new Instruction[] { loadSource }, toNativeMethod);
        }

        private void WriteMarshalToNative(ILProcessor processor, FieldDefinition nativePropertyField, Instruction[] loadBufferPtr,
            Instruction loadArrayIndex, Instruction[] loadSourceInstructions, MethodReference toNativeMethod)
        {
            foreach (Instruction instruction in loadBufferPtr)
            {
                processor.Append(instruction);
            }
            EmitAdditionalMarshalingArgs(processor, true, toNativeMethod, nativePropertyField, loadArrayIndex);
            foreach (Instruction instruction in loadSourceInstructions)
            {
                processor.Append(instruction);
            }
            processor.Emit(OpCodes.Call, toNativeMethod);
        }

        private void EmitAdditionalMarshalingArgs(ILProcessor processor, bool toNativeMarshaler, MethodReference marshalerMethod,
            FieldDefinition nativePropertyField, Instruction loadArrayIndex)
        {
            bool emitAdditionalArgs = false;
            if (toNativeMarshaler)
            {
                if (marshalerMethod.Parameters.Count == marshalerToNativeParamCount)
                {
                    emitAdditionalArgs = true;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(marshalerMethod.Parameters.Count == marshalerToNativeParamCountMin);
                }
            }
            else
            {
                if (marshalerMethod.Parameters.Count == marshalerFromNativeParamCount)
                {
                    emitAdditionalArgs = true;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(marshalerMethod.Parameters.Count == marshalerFromNativeParamCountMin);
                }
            }

            if (emitAdditionalArgs)
            {
                if (loadArrayIndex != null)
                {
                    processor.Append(loadArrayIndex);
                }
                else
                {
                    processor.Emit(OpCodes.Ldc_I4_0);
                }
                EmitLdNativePropertyFieldAddress(processor, nativePropertyField);
            }
        }

        /// <summary>
        /// Emits an Ld of the UFieldAddress.Address member (or IntPtr.Zero if null)
        /// </summary>
        private void EmitLdNativePropertyFieldAddress(ILProcessor processor, FieldDefinition nativePropertyField)
        {
            if (nativePropertyField != null)
            {
                processor.Emit(nativePropertyField.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, nativePropertyField);
                processor.Emit(OpCodes.Ldfld, ufieldAddressAddressFieldRef);
            }
            else
            {
                processor.Emit(OpCodes.Ldsfld, intPtrZeroFieldRef);
            }
        }

        /// <summary>
        /// Loads the static delegates FromNative / ToNative from CachedMarshalingDelegates<,>
        /// e.g. CachedMarshalingDelegates<int, BlittableTypeMarshaler<int>>.FromNative / .ToNative
        /// </summary>
        private void EmitCachedMarshalingDelegates(ILProcessor processor, ManagedUnrealPropertyInfo propertyInfo, ManagedUnrealMarshalerType marshalerType)
        {
            foreach (ManagedUnrealTypeInfoReference genericArg in propertyInfo.GenericArgs)
            {
                Type cachedMarshalingDelegatesType = ManagedUnrealTypeInfo.GetCachedMarshalingDelegatesType(marshalerType, genericArg);

                FieldReference fromNative = assembly.MainModule.ImportEx(
                    ManagedUnrealTypeInfo.GetCachedMarshalingDelegatesDelegate(cachedMarshalingDelegatesType, true));
                FieldReference toNative = assembly.MainModule.ImportEx(
                    ManagedUnrealTypeInfo.GetCachedMarshalingDelegatesDelegate(cachedMarshalingDelegatesType, false));
                processor.Emit(OpCodes.Ldsfld, fromNative);
                processor.Emit(OpCodes.Ldsfld, toNative);
            }
        }

        private VariableDefinition EmitCreateMarshaler(ILProcessor processor, Instruction loadBuffer, FieldDefinition offsetField, FieldDefinition nativePropertyField, ManagedUnrealPropertyInfo propertyInfo)
        {
            VariableDefinition marshalerVar = null;
            if (ManagedUnrealTypeInfo.PropertyRequiresMarshalerInstance(propertyInfo))
            {
                TypeReference marshalerType = assembly.MainModule.ImportEx(ManagedUnrealTypeInfo.GetMarshalerType(ManagedUnrealMarshalerType.Copy, propertyInfo));
                marshalerVar = new VariableDefinition(marshalerType);
            }

            if (marshalerVar != null)
            {
                processor.Emit(OpCodes.Ldc_I4_1);
                processor.Emit(OpCodes.Ldsfld, nativePropertyField);

                // New delegate marshaling code, this will use a cached delegate
                EmitCachedMarshalingDelegates(processor, propertyInfo, ManagedUnrealMarshalerType.Copy);

                System.Reflection.ConstructorInfo ctor;
                System.Reflection.MethodInfo fromNativeMethod;
                System.Reflection.MethodInfo toNativeMethod;
                GetInstancedMarshalerMethods(propertyInfo, ManagedUnrealMarshalerType.Copy, out ctor, out fromNativeMethod, out toNativeMethod);

                processor.Emit(OpCodes.Newobj, assembly.MainModule.ImportEx(ctor));

                processor.Emit(OpCodes.Stloc, marshalerVar);
            }

            return marshalerVar;
        }

        private void EmitLoad(ILProcessor processor, Instruction loadBuffer, FieldDefinition offsetField, FieldDefinition nativePropertyField,
            ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, VariableDefinition marshalerVar)
        {
            if (marshalerVar != null)
            {
                processor.Emit(ManagedUnrealTypeInfo.IsPropertyMarshalerStruct(propertyInfo) ? OpCodes.Ldloca : OpCodes.Ldloc, marshalerVar);
            }
            
            Instruction[] loadBufferInstructions = GetLoadNativeBufferInstructions(typeInfo, processor, loadBuffer, offsetField);

            MethodReference fromNativeMethod = GetFromNativeMarshaler(ManagedUnrealMarshalerType.Copy, propertyInfo);
            WriteMarshalFromNative(processor, nativePropertyField, loadBufferInstructions, null, fromNativeMethod);
        }

        private void EmitLoad(ILProcessor processor, Instruction loadBuffer, FieldDefinition offsetField, FieldDefinition nativePropertyField,
            ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, VariableDefinition marshalerVar, VariableDefinition var)
        {
            EmitLoad(processor, loadBuffer, offsetField, nativePropertyField, typeInfo, propertyInfo, marshalerVar);
            processor.Emit(OpCodes.Stloc, var);
        }

        private void EmitLoad(ILProcessor processor, Instruction loadBuffer, FieldDefinition offsetField, FieldDefinition nativePropertyField,
            ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, VariableDefinition marshalerVar, ParameterDefinition param)
        {
            // This is moving the native buffer into the parameter

            // If the param is "out" or "ref" load the arg and store it afterwards
            // NOTE: Is this a redundant check? Why would we be loading the native buffer into a param if it isn't ref / out?
            //       - Collections were treated as a special case. Look into if it is still required. If not; remove this ref/out check
            //         (or just change it to an assert) and only take the ref/out path.
            bool outOrRef = propertyInfo.IsByRef || propertyInfo.IsOut;
            if (outOrRef)
            {
                System.Diagnostics.Debug.Assert(param.IsOut || param.ParameterType.IsByReference);
                processor.Emit(OpCodes.Ldarg, param);
            }

            EmitLoad(processor, loadBuffer, offsetField, nativePropertyField, typeInfo, propertyInfo, marshalerVar);
            
            if (outOrRef)
            {
                processor.Append(CreateStRefOutParam(processor, param, propertyInfo.Type.TypeCode));
            }
            else
            {
                // Is this useful for anything? Old code used it when not handling ref/out properly.
                processor.Emit(OpCodes.Starg, param);
            }
        }

        private void EmitLoad(ILProcessor processor, Instruction loadBuffer, FieldDefinition offsetField, FieldDefinition nativePropertyField,
            ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, VariableDefinition marshalerVar, FieldDefinition field)
        {
            // Assuming we are targeting "this"
            processor.Emit(OpCodes.Ldarg_0);

            EmitLoad(processor, loadBuffer, offsetField, nativePropertyField, typeInfo, propertyInfo, marshalerVar);

            // Assuming we are storing into a non-static field
            processor.Emit(OpCodes.Stfld, field);
        }

        private void EmitStore(ILProcessor processor, Instruction loadBuffer, FieldDefinition offsetField, FieldDefinition nativePropertyField,
            ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, VariableDefinition marshalerVar, FieldDefinition field)
        {
            Instruction[] loadValue = new Instruction[] { processor.Create(OpCodes.Ldarg_0), processor.Create(OpCodes.Ldfld, field) };
            EmitStore(processor, loadBuffer, offsetField, nativePropertyField, typeInfo, propertyInfo, loadValue, marshalerVar);
        }

        private void EmitStore(ILProcessor processor, Instruction loadBuffer, FieldDefinition offsetField, FieldDefinition nativePropertyField,
            ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, VariableDefinition marshalerVar, VariableDefinition var)
        {
            Instruction[] loadValue = new Instruction[] { processor.Create(OpCodes.Ldloc, var) };
            EmitStore(processor, loadBuffer, offsetField, nativePropertyField, typeInfo, propertyInfo, loadValue, marshalerVar);
        }

        private void EmitStore(ILProcessor processor, Instruction loadBuffer, FieldDefinition offsetField, FieldDefinition nativePropertyField,
            ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, VariableDefinition marshalerVar, ParameterDefinition param)
        {
            // This is moving the parameter into the native buffer

            Instruction[] loadValue = null;
            if (propertyInfo.IsByRef)
            {
                // If this is a "ref" param we need to pass the arg without the ref as no marshalers use by ref args.
                // (if they did support ref we would need to do the opposite on all args which didn't match the ref arg)
                System.Diagnostics.Debug.Assert(param.ParameterType.IsByReference);
                loadValue = new Instruction[]
                {
                    processor.Create(OpCodes.Ldarg, param),
                    CreateLdRefOutParam(processor, param, propertyInfo.Type.TypeCode)
                };
            }
            else
            {
                loadValue = new Instruction[] { processor.Create(OpCodes.Ldarg, param) };
            }
            EmitStore(processor, loadBuffer, offsetField, nativePropertyField, typeInfo, propertyInfo, loadValue, marshalerVar);
        }

        private void EmitStore(ILProcessor processor, Instruction loadBuffer, FieldDefinition offsetField, FieldDefinition nativePropertyField,
            ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, Instruction[] loadValue, VariableDefinition marshalerVar)
        {
            if (marshalerVar != null)
            {
                processor.Emit(ManagedUnrealTypeInfo.IsPropertyMarshalerStruct(propertyInfo) ? OpCodes.Ldloca : OpCodes.Ldloc, marshalerVar);
            }
            
            Instruction[] loadBufferInstructions = GetLoadNativeBufferInstructions(typeInfo, processor, loadBuffer, offsetField);

            MethodReference toNativeMethod = GetToNativeMarshaler(ManagedUnrealMarshalerType.Copy, propertyInfo);
            WriteMarshalToNative(processor, nativePropertyField, loadBufferInstructions, null, loadValue, toNativeMethod);
        }

        private MethodReference GetFromNativeMarshaler(ManagedUnrealMarshalerType marshalerType, ManagedUnrealPropertyInfo propertyInfo)
        {
            ManagedUnrealTypeInfoReference arg1 = propertyInfo.GenericArgs.Count >= 1 ? propertyInfo.GenericArgs[0] : null;
            ManagedUnrealTypeInfoReference arg2 = propertyInfo.GenericArgs.Count >= 2 ? propertyInfo.GenericArgs[1] : null;

            return GetFromNativeMarshaler(marshalerType,
                propertyInfo.Type.TypeCode, propertyInfo.Type.Path,
                arg1 != null ? arg1.TypeCode : EPropertyType.Unknown, arg1 != null ? arg1.Path : null,
                arg2 != null ? arg2.TypeCode : EPropertyType.Unknown, arg2 != null ? arg2.Path : null);
        }

        private MethodReference GetFromNativeMarshaler(ManagedUnrealMarshalerType marshalerType, ManagedUnrealTypeInfo typeInfo)
        {
            return GetFromNativeMarshaler(marshalerType, typeInfo.TypeCode, typeInfo.Path);
        }

        private MethodReference GetFromNativeMarshaler(ManagedUnrealMarshalerType marshalerType, ManagedUnrealTypeInfoReference typeInfo)
        {
            return GetFromNativeMarshaler(marshalerType, typeInfo.TypeCode, typeInfo.Path);
        }

        private MethodReference GetFromNativeMarshaler(ManagedUnrealMarshalerType marshalerType,
            EPropertyType typeCode, string typePath,
            EPropertyType arg1TypeCode = EPropertyType.Unknown, string arg1TypePath = null,
            EPropertyType arg2TypeCode = EPropertyType.Unknown, string arg2TypePath = null)
        {
            ManagedUnrealMarshalerInfo marshalerInfo = new ManagedUnrealMarshalerInfo(
                typeCode, typePath, arg1TypeCode, arg1TypePath, arg2TypeCode, arg2TypePath, marshalerType);

            MethodReference method;
            if (fromNativeMarshalers.TryGetValue(marshalerInfo, out method))
            {
                return method;
            }

            int paramCount = marshalerFromNativeParamCountMin;
            if (!codeSettings.MinimalMarshalingParams || ManagedUnrealTypeInfo.MarshalerRequiresNativePropertyField(typeCode))
            {
                paramCount = marshalerFromNativeParamCount;
            }

            Type type = ManagedUnrealTypeInfo.GetTypeFromMarshalerInfo(marshalerInfo);
            if (type != null)
            {
                if (typeCode == EPropertyType.Struct && !type.IsGenericType)
                {
                    // Struct marshaling methods are generated, find the existing type by path
                    TypeDefinition typeDef = assembly.MainModule.GetType(type.FullName);

                    bool requiresImport = false;
                    if (typeDef == null)
                    {
                        typeDef = assembly.MainModule.ImportEx(type).Resolve();
                        requiresImport = true;
                    }

                    foreach (MethodDefinition methodDef in typeDef.Methods)
                    {
                        if (methodDef.Name == "FromNative" && methodDef.IsPublic && methodDef.IsStatic && methodDef.Parameters.Count == paramCount)
                        {
                            method = methodDef;
                            if (requiresImport)
                            {
                                method = assembly.MainModule.Import(method);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    foreach (System.Reflection.MethodInfo methodInfo in type.GetMethods())
                    {
                        if (methodInfo.Name == "FromNative" && methodInfo.IsPublic && methodInfo.GetParameters().Length == paramCount &&
                            (methodInfo.IsStatic || ManagedUnrealTypeInfo.IsCollectionType(typeCode)))
                        {
                            method = assembly.MainModule.ImportEx(methodInfo);
                            break;
                        }
                    }
                }
            }

            if (method != null)
            {
                fromNativeMarshalers.Add(marshalerInfo, method);
            }

            if (method == null)
            {
                throw new RewriteException(typePath, "Failed to get FromNative marshaler");
            }

            return method;
        }

        private MethodReference GetToNativeMarshaler(ManagedUnrealMarshalerType marshalerType, ManagedUnrealPropertyInfo propertyInfo)
        {
            ManagedUnrealTypeInfoReference arg1 = propertyInfo.GenericArgs.Count >= 1 ? propertyInfo.GenericArgs[0] : null;
            ManagedUnrealTypeInfoReference arg2 = propertyInfo.GenericArgs.Count >= 2 ? propertyInfo.GenericArgs[1] : null;

            return GetToNativeMarshaler(marshalerType,
                propertyInfo.Type.TypeCode, propertyInfo.Type.Path,
                arg1 != null ? arg1.TypeCode : EPropertyType.Unknown, arg1 != null ? arg1.Path : null,
                arg2 != null ? arg2.TypeCode : EPropertyType.Unknown, arg2 != null ? arg2.Path : null);
        }

        private MethodReference GetToNativeMarshaler(ManagedUnrealMarshalerType marshalerType, ManagedUnrealTypeInfo typeInfo)
        {
            return GetToNativeMarshaler(marshalerType, typeInfo.TypeCode, typeInfo.Path);
        }

        private MethodReference GetToNativeMarshaler(ManagedUnrealMarshalerType marshalerType, ManagedUnrealTypeInfoReference typeInfo)
        {
            return GetToNativeMarshaler(marshalerType, typeInfo.TypeCode, typeInfo.Path);
        }

        private MethodReference GetToNativeMarshaler(ManagedUnrealMarshalerType marshalerType,
            EPropertyType typeCode, string typePath,
            EPropertyType arg1TypeCode = EPropertyType.Unknown, string arg1TypePath = null,
            EPropertyType arg2TypeCode = EPropertyType.Unknown, string arg2TypePath = null)
        {
            ManagedUnrealMarshalerInfo marshalerInfo = new ManagedUnrealMarshalerInfo(
                typeCode, typePath, arg1TypeCode, arg1TypePath, arg2TypeCode, arg2TypePath, marshalerType);

            MethodReference method;
            if (toNativeMarshalers.TryGetValue(marshalerInfo, out method))
            {
                return method;
            }

            int paramCount = marshalerToNativeParamCountMin;
            if (!codeSettings.MinimalMarshalingParams || ManagedUnrealTypeInfo.MarshalerRequiresNativePropertyField(typeCode))
            {
                paramCount = marshalerToNativeParamCount;
            }

            Type type = ManagedUnrealTypeInfo.GetTypeFromMarshalerInfo(marshalerInfo);
            if (type != null)
            {
                if (typeCode == EPropertyType.Struct && !type.IsGenericType)
                {
                    // Struct marshaling methods are generated, find the existing type by path
                    TypeDefinition typeDef = assembly.MainModule.GetType(type.FullName);

                    bool requiresImport = false;
                    if (typeDef == null)
                    {
                        typeDef = assembly.MainModule.ImportEx(type).Resolve();
                        requiresImport = true;
                    }

                    foreach (MethodDefinition methodDef in typeDef.Methods)
                    {
                        if (methodDef.Name == "ToNative" && methodDef.IsPublic && methodDef.IsStatic && methodDef.Parameters.Count == paramCount)
                        {
                            method = methodDef;
                            if (requiresImport)
                            {
                                method = assembly.MainModule.Import(method);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    foreach (System.Reflection.MethodInfo methodInfo in type.GetMethods())
                    {
                        if (methodInfo.Name == "ToNative" && methodInfo.IsPublic && methodInfo.GetParameters().Length == paramCount &&
                            (methodInfo.IsStatic || ManagedUnrealTypeInfo.IsCollectionType(typeCode)))
                        {
                            method = assembly.MainModule.ImportEx(methodInfo);
                            break;
                        }
                    }
                }
            }

            if (method != null)
            {
                toNativeMarshalers.Add(marshalerInfo, method);
            }

            if (method == null)
            {
                throw new RewriteException(typePath, "Failed to get ToNative marshaler");
            }

            return method;
        }

        /// <summary>
        /// Gets the marshaler methods for a marshaler which has to be instantiated be used (the collection marshalers)
        /// </summary>
        private bool GetInstancedMarshalerMethods(ManagedUnrealPropertyInfo propertyInfo, ManagedUnrealMarshalerType marshalerType,
            out System.Reflection.ConstructorInfo ctor,
            out System.Reflection.MethodInfo fromNativeMethod,
            out System.Reflection.MethodInfo toNativeMethod)
        {
            Type collectionMarshalerType = ManagedUnrealTypeInfo.GetMarshalerType(marshalerType, propertyInfo);

            // Find the constructor for the marshaler
            ctor = null;
            foreach (System.Reflection.ConstructorInfo ctorInfo in collectionMarshalerType.GetConstructors())
            {
                System.Reflection.ParameterInfo[] ctorParams = ctorInfo.GetParameters();
                if (ctorParams.Length > 2 && ctorParams[0].ParameterType == typeof(int) && ctorParams[1].ParameterType == typeof(UFieldAddress))
                {
                    ctor = ctorInfo;
                    break;
                }
            }

            fromNativeMethod = GetInstancedMarshalerMethod(collectionMarshalerType, true);
            toNativeMethod = GetInstancedMarshalerMethod(collectionMarshalerType, false);

            bool success = ctor != null && fromNativeMethod != null && toNativeMethod != null;
            if (!success)
            {
                throw new RewriteException(propertyInfo.Path, "Failed to get marshaler methods");
            }
            return success;
        }

        private System.Reflection.MethodInfo GetInstancedMarshalerMethod(Type type, bool fromNative)
        {
            Dictionary<Type, System.Reflection.MethodInfo> cachedMarshalerMethods = null;
            if (fromNative)
            {
                cachedMarshalerMethods = codeSettings.MinimalMarshalingParams ? cachedInstancedMarshalerMethodsFromNativeMin :
                    cachedInstancedMarshalerMethodsFromNative;
            }
            else
            {
                cachedMarshalerMethods = codeSettings.MinimalMarshalingParams ? cachedInstancedMarshalerMethodsToNativeMin :
                    cachedInstancedMarshalerMethodsToNative;
            }

            System.Reflection.MethodInfo result;
            if (!cachedMarshalerMethods.TryGetValue(type, out result))
            {
                foreach (System.Reflection.MethodInfo method in type.GetMethods())
                {
                    if (fromNative)
                    {
                        if (method.Name == "FromNative" && method.IsPublic && !method.IsStatic)
                        {
                            int paramCount = codeSettings.MinimalMarshalingParams ?
                                marshalerFromNativeParamCountMin : marshalerFromNativeParamCount;
                            if (method.GetParameters().Length == paramCount)
                            {
                                result = method;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (method.Name == "ToNative" && method.IsPublic && !method.IsStatic)
                        {
                            int paramCount = codeSettings.MinimalMarshalingParams ?
                                marshalerToNativeParamCountMin : marshalerToNativeParamCount;
                            if (method.GetParameters().Length == paramCount)
                            {
                                result = method;
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
