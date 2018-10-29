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
    // TODO: Ensure correct IL usage on ref/out params. Ensure these functions are valid: 
    //       - EmitLoad, StoreLoad (taking ParameterDefinition) - structs possibly aren't writing things properly (stdind/ldind/ldobj)
    //       - CreateSetDefaultValue (taking ParameterDefinition)

    // Make sure we aren't sharing instructions between functions. We had an instruction shared when we were creating
    // the struct marshaling methods (Ctor/FromNative/ToNative).
    // Instruction loadBufferInstruction = ctorProcessor.Create(OpCodes.Ldarg_1);

    // TODO: Ensure correct usage of stfld / stsfld / ldfld / ldsfld / possibly other opcodes
    // TODO: Lots of work to do with name conflicts

    partial class AssemblyRewriter
    {
        public const bool UpdatePdb = true;

        // These collections are only the available types which need updating, additional lookups required for other types
        Dictionary<TypeDefinition, ManagedUnrealTypeInfo> classesByType = new Dictionary<TypeDefinition, ManagedUnrealTypeInfo>();
        Dictionary<ManagedUnrealTypeInfo, TypeDefinition> classesByInfo = new Dictionary<ManagedUnrealTypeInfo, TypeDefinition>();

        Dictionary<TypeDefinition, ManagedUnrealTypeInfo> structsByType = new Dictionary<TypeDefinition, ManagedUnrealTypeInfo>();
        Dictionary<ManagedUnrealTypeInfo, TypeDefinition> structsByInfo = new Dictionary<ManagedUnrealTypeInfo, TypeDefinition>();

        Dictionary<TypeDefinition, ManagedUnrealEnumInfo> enumsByType = new Dictionary<TypeDefinition, ManagedUnrealEnumInfo>();
        Dictionary<ManagedUnrealEnumInfo, TypeDefinition> enumsByInfo = new Dictionary<ManagedUnrealEnumInfo, TypeDefinition>();

        Dictionary<TypeDefinition, ManagedUnrealTypeInfo> interfacesByType = new Dictionary<TypeDefinition, ManagedUnrealTypeInfo>();
        Dictionary<ManagedUnrealTypeInfo, TypeDefinition> interfacesByInfo = new Dictionary<ManagedUnrealTypeInfo, TypeDefinition>();

        Dictionary<TypeDefinition, ManagedUnrealTypeInfo> delegatesByType = new Dictionary<TypeDefinition, ManagedUnrealTypeInfo>();
        Dictionary<ManagedUnrealTypeInfo, TypeDefinition> delegatesByInfo = new Dictionary<ManagedUnrealTypeInfo, TypeDefinition>();

        // Commonly used types / misc methods
        TypeReference typeTypeRef;
        TypeReference voidTypeRef;
        TypeReference boolTypeRef;
        TypeReference int32TypeRef;
        TypeReference intPtrTypeRef;
        TypeReference byteTypeRef;
        TypeReference bytePtrTypeRef;
        TypeReference uobjectTypeRef;
        TypeReference stringTypeRef;
        FieldReference intPtrZeroFieldRef;
        MethodReference intPtrAddMethod;
        MethodReference intPtrConversionMethod;
        MethodReference intPtrConstructorMethod;
        MethodReference intPtrEqualsMethod;
        MethodReference intPtrNotEqualsMethod;
        MethodReference typeGetTypeFromHandleMethod;        
        MethodReference checkUObjectDestroyedMethod;
        MethodReference uobjectAddressGetter;
        TypeReference sharpPathAttributeTypeRef;
        MethodReference sharpPathAttributeTypeCtor;
        TypeReference functionInvokerAttributeTypeRef;
        MethodReference functionInvokerAttributeTypeCtor;
        MethodReference rpcValidateFailedMethod;
        MethodReference fmemoryMemzero;

        // FText
        TypeReference ftextTypeRef;
        MethodReference ftextCtor;
        MethodReference ftextCopyFrom;

        // StructAsClass related
        MethodReference structAsClassAddressGetter;
        MethodReference checkStructAsClassDestroyedMethod;

        // IInterface related
        TypeReference iinterfaceImplTypeRef;
        MethodReference iinterfaceImplAddressGetter;
        MethodReference checkInterfaceObjDestroyedMethod;

        // NativeReflection methods
        MethodReference reflectionGetClassMethod;
        MethodReference reflectionGetStructMethod;
        MethodReference reflectionGetStructSizeMethod;
        //MethodReference reflectionGetPropertyMethod;
        //MethodReference reflectionGetPropertyRefMethod;
        //MethodReference reflectionGetPropertyOffsetMethod;
        //MethodReference reflectionGetPropertyArrayElementSizeMethod;
        //MethodReference reflectionGetPropertyRepIndexMethod;
        MethodReference reflectionGetFunctionParamsSizeMethod;
        //MethodReference reflectionGetFunctionMethod;
        MethodReference reflectionGetFunctionFromPathMethod;
        MethodReference reflectionGetFunctionFromInstance;
        MethodReference reflectionInvokeFunctionMethod;
        MethodReference reflectionInitAllMethod;
        MethodReference reflectionDestroyAllMethod;
        MethodReference reflectionInitializeValue_InContainer;
        MethodReference reflectionDestroyValue_InContainer;
        MethodReference reflectionValidateBlittableStructSizeMethod;
        //MethodReference reflectionValidatePropertyClassMethod;
        MethodReference reflectionLogFunctionIsValidMethod;
        MethodReference reflectionLogStructIsValidMethod;
        MethodReference reflectionLogInvalidPropertyAccessedMethod;
        MethodReference reflectionLogInvalidFunctionAccessedMethod;
        MethodReference reflectionLogInvalidStructAccessedMethod;

        // NativeReflectionCached methods
        MethodReference reflectionCachedGetPropertyMethod;
        MethodReference reflectionCachedGetPropertyRefMethod;
        MethodReference reflectionCachedGetPropertyOffsetMethod;
        MethodReference reflectionCachedGetPropertyArrayElementSizeMethod;
        MethodReference reflectionCachedGetPropertyRepIndexMethod;
        MethodReference reflectionCachedGetFunctionMethod;
        MethodReference reflectionCachedValidatePropertyClassMethod;

        // GCHelper methods
        MethodReference gchelperFindMethodGeneric;

        // UnrealTypes methods
        MethodReference unrealTypesCanLazyLoadManagedTypeMethod;
        MethodReference unrealTypesOnCCtorCalledMethod;

        // UFieldAddress related
        MethodReference ufieldAddressCtor;
        MethodReference ufieldAddressUpdateMethod;
        TypeReference ufieldAddressTypeRef;
        FieldReference ufieldAddressAddressFieldRef;

        // UProperty class fields in "Classes"
        Dictionary<EPropertyType, FieldReference> classesUPropertyFieldRefs = new Dictionary<EPropertyType, FieldReference>();

        Dictionary<ManagedUnrealMarshalerInfo, MethodReference> fromNativeMarshalers = new Dictionary<ManagedUnrealMarshalerInfo, MethodReference>();
        Dictionary<ManagedUnrealMarshalerInfo, MethodReference> toNativeMarshalers = new Dictionary<ManagedUnrealMarshalerInfo, MethodReference>();

        // Cached instanced marshaler methods (used for collection marshalers which have a non-static marshaler)
        Dictionary<Type, System.Reflection.MethodInfo> cachedInstancedMarshalerMethodsFromNative = new Dictionary<Type, System.Reflection.MethodInfo>();
        Dictionary<Type, System.Reflection.MethodInfo> cachedInstancedMarshalerMethodsFromNativeMin = new Dictionary<Type, System.Reflection.MethodInfo>();
        Dictionary<Type, System.Reflection.MethodInfo> cachedInstancedMarshalerMethodsToNative = new Dictionary<Type, System.Reflection.MethodInfo>();
        Dictionary<Type, System.Reflection.MethodInfo> cachedInstancedMarshalerMethodsToNativeMin = new Dictionary<Type, System.Reflection.MethodInfo>();

        const int marshalerFromNativeParamCount = 3;// (IntPtr nativeBuffer, int arrayIndex, IntPtr property)
        const int marshalerFromNativeParamCountMin = 1;// (IntPtr nativeBuffer)
        const int marshalerToNativeParamCount = 4;// (IntPtr nativeBuffer, int arrayIndex, IntPtr property, XXXX value)
        const int marshalerToNativeParamCountMin = 2;// (IntPtr nativeBuffer, XXXX value)

        CodeGeneratorSettings codeSettings;

        AssemblyDefinition assembly;

        public void RewriteModule(ManagedUnrealModuleInfo moduleInfo, string assemblyPath)
        {
            System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();
            sw2.Start();
            string pdbFile = Path.ChangeExtension(assemblyPath, ".pdb");

            codeSettings = new CodeGeneratorSettings();
            codeSettings.Load();

            ReaderParameters readerParams = new ReaderParameters();
            WriterParameters writerParams = new WriterParameters();

            var resolver = readerParams.AssemblyResolver as DefaultAssemblyResolver ?? new DefaultAssemblyResolver();
            readerParams.AssemblyResolver = resolver;

            resolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));

            if (File.Exists(pdbFile) && UpdatePdb)
            {
                readerParams.SymbolReaderProvider = new PdbReaderProvider();
                readerParams.ReadSymbols = true;
                writerParams.WriteSymbols = true;
            };

            assembly = AssemblyDefinition.ReadAssembly(assemblyPath, readerParams);

            typeTypeRef = assembly.MainModule.ImportEx(typeof(Type));
            voidTypeRef = assembly.MainModule.ImportEx(typeof(void));
            boolTypeRef = assembly.MainModule.ImportEx(typeof(bool));
            int32TypeRef = assembly.MainModule.ImportEx(typeof(Int32));
            intPtrTypeRef = assembly.MainModule.ImportEx(typeof(IntPtr));
            byteTypeRef = assembly.MainModule.ImportEx(typeof(byte));
            bytePtrTypeRef = assembly.MainModule.ImportEx(typeof(byte*));
            uobjectTypeRef = assembly.MainModule.ImportEx(typeof(UObject));
            stringTypeRef = assembly.MainModule.ImportEx(typeof(string));
            intPtrZeroFieldRef = assembly.MainModule.ImportEx(typeof(IntPtr).GetField("Zero"));
            intPtrAddMethod = assembly.MainModule.ImportEx(typeof(IntPtr).GetMethod("Add"));
            intPtrConstructorMethod = assembly.MainModule.ImportEx(typeof(IntPtr).GetConstructor(new Type[] { typeof(void*) }));
            foreach (System.Reflection.MethodInfo method in typeof(IntPtr).GetMethods())
            {
                if (method.Name == "op_Explicit")
                {
                    if (method.ReturnType == typeof(void*))
                    {
                        intPtrConversionMethod = assembly.MainModule.ImportEx(method);
                    }
                }

                bool isEqualsOp = method.Name == "op_Equality";
                bool isNotEqualsOp = method.Name == "op_Inequality";
                if (isEqualsOp || isNotEqualsOp)
                {
                    System.Reflection.ParameterInfo[] parameters = method.GetParameters();
                    if (method.ReturnType == typeof(bool) && parameters.Length == 2 &&
                        parameters[0].ParameterType == typeof(IntPtr) && parameters[1].ParameterType == typeof(IntPtr))
                    {
                        if (isEqualsOp)
                        {
                            intPtrEqualsMethod = assembly.MainModule.ImportEx(method);
                        }
                        else if (isNotEqualsOp)
                        {
                            intPtrNotEqualsMethod = assembly.MainModule.ImportEx(method);
                        }
                    }
                }
            }
            typeGetTypeFromHandleMethod = assembly.MainModule.ImportEx(typeof(Type).GetMethod("GetTypeFromHandle"));            
            checkUObjectDestroyedMethod = assembly.MainModule.ImportEx(FindMethodByName(uobjectTypeRef.Resolve(), "CheckDestroyed"));
            uobjectAddressGetter = assembly.MainModule.ImportEx(FindPropertyByName(uobjectTypeRef.Resolve(), "Address").GetMethod);

            Type sharpPathAttributeType = typeof(USharpPathAttribute);
            sharpPathAttributeTypeRef = assembly.MainModule.ImportEx(sharpPathAttributeType);
            sharpPathAttributeTypeCtor = assembly.MainModule.ImportEx(
                sharpPathAttributeType.GetConstructor(new Type[] { typeof(string) }));

            Type functionInvokerAttributeType = typeof(UFunctionInvokerAttribute);
            functionInvokerAttributeTypeRef = assembly.MainModule.ImportEx(functionInvokerAttributeType);
            functionInvokerAttributeTypeCtor = assembly.MainModule.ImportEx(
                functionInvokerAttributeType.GetConstructor(new Type[] { typeof(string) }));

            rpcValidateFailedMethod = assembly.MainModule.ImportEx(typeof(FCoreNet).GetMethod("RPC_ValidateFailed"));
            fmemoryMemzero = assembly.MainModule.ImportEx(typeof(FMemory).GetMethod("Memzero", new Type[] { typeof(IntPtr), typeof(int) }));

            Type ftextType = typeof(FText);
            ftextTypeRef = assembly.MainModule.ImportEx(ftextType);
            ftextCtor = assembly.MainModule.ImportEx(ftextType.GetConstructor(new Type[] { typeof(IntPtr), typeof(bool) }));
            ftextCopyFrom = assembly.MainModule.ImportEx(ftextType.GetMethod("CopyFrom"));

            structAsClassAddressGetter = assembly.MainModule.ImportEx(typeof(StructAsClass).GetProperty("Address").GetMethod);
            checkStructAsClassDestroyedMethod = assembly.MainModule.ImportEx(typeof(StructAsClass).GetMethod("CheckDestroyed",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly));

            Type iinterfaceImplType = typeof(IInterfaceImpl);
            iinterfaceImplTypeRef = assembly.MainModule.ImportEx(iinterfaceImplType);
            iinterfaceImplAddressGetter = assembly.MainModule.ImportEx(iinterfaceImplType.GetProperty("Address").GetMethod);
            checkInterfaceObjDestroyedMethod = assembly.MainModule.ImportEx(iinterfaceImplType.GetMethod("CheckDestroyed"));

            // TODO: Make sure these there is only a single result for these function names
            Type nativeReflectionType = typeof(NativeReflection);
            reflectionGetClassMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetClass"));
            reflectionGetStructMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetStruct"));
            reflectionGetStructSizeMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetStructSize"));
            //reflectionGetPropertyMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetProperty"));
            //reflectionGetPropertyRefMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetPropertyRef"));
            //reflectionGetPropertyOffsetMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetPropertyOffset"));
            //reflectionGetPropertyArrayElementSizeMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetPropertyArrayElementSize"));
            //reflectionGetPropertyRepIndexMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetPropertyRepIndex"));
            reflectionGetFunctionParamsSizeMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetFunctionParamsSize"));
            reflectionInvokeFunctionMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("InvokeFunction"));
            reflectionInitAllMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("InvokeFunction_InitAll"));
            reflectionDestroyAllMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("InvokeFunction_DestroyAll"));
            reflectionInitializeValue_InContainer = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("InitializeValue_InContainer"));
            reflectionDestroyValue_InContainer = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("DestroyValue_InContainer"));
            reflectionGetFunctionFromInstance = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetFunctionFromInstance"));
            reflectionValidateBlittableStructSizeMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("ValidateBlittableStructSize"));
            //reflectionValidatePropertyClassMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("ValidatePropertyClass"));
            reflectionLogFunctionIsValidMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("LogFunctionIsValid"));
            reflectionLogStructIsValidMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("LogStructIsValid"));
            reflectionLogInvalidPropertyAccessedMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("LogInvalidPropertyAccessed"));
            reflectionLogInvalidFunctionAccessedMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("LogInvalidFunctionAccessed"));
            reflectionLogInvalidStructAccessedMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("LogInvalidStructAccessed"));

            foreach (System.Reflection.MethodInfo method in nativeReflectionType.GetMethods())
            {
                System.Reflection.ParameterInfo[] methodParams = method.GetParameters();
                switch (method.Name)
                {
                    case "GetFunction":
                        if (methodParams.Length == 2)
                        {
                            //reflectionGetFunctionMethod = assembly.MainModule.ImportEx(method);
                        }
                        else if (methodParams.Length == 1)
                        {
                            reflectionGetFunctionFromPathMethod = assembly.MainModule.ImportEx(method);
                        }
                        break;
                }
            }

            Type nativeReflectionCachedType = typeof(NativeReflectionCached);

            reflectionCachedGetPropertyMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetProperty"));
            reflectionCachedGetPropertyRefMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetPropertyRef"));
            reflectionCachedGetPropertyOffsetMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetPropertyOffset"));
            reflectionCachedGetPropertyArrayElementSizeMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetPropertyArrayElementSize"));
            reflectionCachedGetPropertyRepIndexMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("GetPropertyRepIndex"));
            reflectionCachedValidatePropertyClassMethod = assembly.MainModule.ImportEx(nativeReflectionType.GetMethod("ValidatePropertyClass"));

            foreach (System.Reflection.MethodInfo method in nativeReflectionCachedType.GetMethods())
            {
                System.Reflection.ParameterInfo[] methodParams = method.GetParameters();
                switch (method.Name)
                {
                    case "GetFunction":
                        if (methodParams.Length == 2)
                        {
                            reflectionCachedGetFunctionMethod = assembly.MainModule.ImportEx(method);
                        }
                        break;
                }
            }

            foreach (System.Reflection.MethodInfo method in typeof(GCHelper).GetMethods())
            {
                if (method.Name == "Find" && method.IsGenericMethod)
                {
                    System.Reflection.ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(IntPtr))
                    {
                        gchelperFindMethodGeneric = assembly.MainModule.ImportEx(method);
                        break;
                    }
                }
            }

            unrealTypesCanLazyLoadManagedTypeMethod = assembly.MainModule.ImportEx(typeof(UnrealTypes).GetMethod("CanLazyLoadManagedType"));
            unrealTypesOnCCtorCalledMethod = assembly.MainModule.ImportEx(typeof(UnrealTypes).GetMethod("OnCCtorCalled"));

            ufieldAddressCtor = assembly.MainModule.ImportEx(typeof(UFieldAddress).GetConstructor(Type.EmptyTypes));
            ufieldAddressUpdateMethod = assembly.MainModule.ImportEx(typeof(UFieldAddress).GetMethod("Update"));
            ufieldAddressTypeRef = assembly.MainModule.ImportEx(typeof(UFieldAddress));
            ufieldAddressAddressFieldRef = assembly.MainModule.ImportEx(typeof(UFieldAddress).GetField("Address"));

            classesUPropertyFieldRefs.Clear();
            foreach (EPropertyType propertyType in Enum.GetValues(typeof(EPropertyType)))
            {
                string propertyClassName;
                if (NativeReflection.TryGetPropertyClassName(propertyType, out propertyClassName))
                {
                    var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static;
                    FieldReference fieldRef = assembly.MainModule.ImportEx(typeof(Classes).GetField(propertyClassName, flags));
                    VerifyNonNull(fieldRef, null, "Failed to find the property type " + propertyClassName + " in Classes");
                    classesUPropertyFieldRefs.Add(propertyType, fieldRef);
                }
            }

            classesByType.Clear();
            classesByInfo.Clear();
            structsByType.Clear();
            structsByInfo.Clear();
            enumsByType.Clear();
            enumsByInfo.Clear();
            interfacesByType.Clear();
            interfacesByInfo.Clear();
            delegatesByType.Clear();
            delegatesByInfo.Clear();

            fromNativeMarshalers.Clear();
            toNativeMarshalers.Clear();

            cachedInstancedMarshalerMethodsFromNative.Clear();
            cachedInstancedMarshalerMethodsFromNativeMin.Clear();
            cachedInstancedMarshalerMethodsToNative.Clear();
            cachedInstancedMarshalerMethodsToNativeMin.Clear();

            sw2.Stop();
            Console.WriteLine("assembly.load time: " + sw2.Elapsed);
            System.Diagnostics.Stopwatch sw3 = new System.Diagnostics.Stopwatch();
            sw3.Start();

            foreach (ModuleDefinition module in assembly.Modules)
            {
                Dictionary<string, ManagedUnrealTypeInfo> typesByFullName = new Dictionary<string, ManagedUnrealTypeInfo>();
                foreach (ManagedUnrealTypeInfo typeInfo in moduleInfo.TypeInfosByPath.Values)
                {
                    typesByFullName.Add(typeInfo.FullName, typeInfo);
                }

                foreach (TypeDefinition type in module.Types)
                {
                    AddTypesNested(type, typesByFullName);
                }
            }

            // Make sure structs are all pre-processed as they may reference each others yet-to-exist marshalers
            foreach (KeyValuePair<TypeDefinition, ManagedUnrealTypeInfo> structInfo in structsByType)
            {
                if (!structInfo.Value.IsStructAsClass)
                {
                    PreRewriteStruct(structInfo.Key, structInfo.Value);
                }
            }

            foreach (KeyValuePair<TypeDefinition, ManagedUnrealTypeInfo> structInfo in structsByType)
            {
                if (structInfo.Value.IsStructAsClass)
                {
                    RewriteClass(structInfo.Key, structInfo.Value);
                }
                else
                {
                    RewriteStruct(structInfo.Key, structInfo.Value);
                }
            }

            foreach (KeyValuePair<TypeDefinition, ManagedUnrealTypeInfo> classInfo in classesByType)
            {
                RewriteClass(classInfo.Key, classInfo.Value);
            }

            foreach (KeyValuePair<TypeDefinition, ManagedUnrealTypeInfo> interfaceInfo in interfacesByType)
            {
                RewriteInterface(interfaceInfo.Key, interfaceInfo.Value);
            }

            foreach (KeyValuePair<TypeDefinition, ManagedUnrealTypeInfo> delegateInfo in delegatesByType)
            {
                RewriteDelegate(delegateInfo.Key, delegateInfo.Value);
            }

            foreach (KeyValuePair<TypeDefinition, ManagedUnrealEnumInfo> enumInfo in enumsByType)
            {
                RewriteEnum(enumInfo.Key, enumInfo.Value);
            }

            foreach (ManagedUnrealTypeInfo typeInfo in moduleInfo.TypesByTypeInfo.Keys)
            {
                typeInfo.CreateHash();
            }

            sw3.Stop();
            Console.WriteLine("assembly process time: " + sw3.Elapsed);

            System.Diagnostics.Stopwatch sw4 = new System.Diagnostics.Stopwatch();
            sw4.Start();

            AddSerializedModuleInfoType(moduleInfo);

            sw4.Stop();
            Console.WriteLine("serialize time: " + sw4.Elapsed);

            try
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                // TODO: Find how to do this in a single stream rather than two streams.
                // - Using assembly.Write(stream, writerParams) doesn't update the pdb.

                assembly.Write(assemblyPath, writerParams);

                // Attempt a few times in case something is holding onto the file
                Exception exception = null;
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        using (FileStream stream = File.OpenWrite(assemblyPath))
                        {
                            stream.Position = stream.Length;
                            stream.Write(Encoding.ASCII.GetBytes("+UEsRW++"), 0, 8);
                        }
                        exception = null;
                    }
                    catch (Exception e)
                    {
                        exception = e;
                        System.Threading.Thread.Sleep(20);
                    }
                }
                if (exception != null)
                {
                    throw exception;
                }

                sw.Stop();
                Console.WriteLine("assembly.Write time: " + sw.Elapsed);
            }
            catch (Exception e)
            {
                // HACK! Improve this!
                // We are stripping setters on collections and a few other things so any code which used setters
                // will cause an error when rewriting the assembly
                if (e.ToString().Contains("System.Void set_"))
                {
                    throw new RewriteException("Using a setter on a property that had a setter stripped! Collections and a few " +
                        "other types have their setters stripped. " + Environment.NewLine + e.ToString());
                }
                else
                {
                    throw;
                }
            }
        }

        private bool AddTypeCollection<T>(TypeDefinition type, List<T> typeInfos,
            Dictionary<TypeDefinition, T> collectionByType,
            Dictionary<T, TypeDefinition> collectionByInfo) where T : ManagedUnrealTypeInfo
        {
            foreach (T typeInfo in typeInfos)
            {
                if (type.FullName == typeInfo.FullName)
                {
                    collectionByType.Add(type, typeInfo);
                    collectionByInfo.Add(typeInfo, type);
                    return true;
                }
            }
            return false;
        }

        private void AddTypesNested(TypeDefinition type, Dictionary<string, ManagedUnrealTypeInfo> typesByFullName)
        {
            foreach (TypeDefinition nestedType in type.NestedTypes)
            {
                AddTypesNested(nestedType, typesByFullName);
            }

            ManagedUnrealTypeInfo typeInfo;
            if (typesByFullName.TryGetValue(type.FullName.Replace("/", "+"), out typeInfo))
            {
                switch (typeInfo.TypeCode)
                {
                    case EPropertyType.Object:
                        classesByType.Add(type, typeInfo);
                        classesByInfo.Add(typeInfo, type);
                        break;
                    case EPropertyType.Struct:
                        structsByType.Add(type, typeInfo);
                        structsByInfo.Add(typeInfo, type);
                        break;
                    case EPropertyType.Enum:
                        enumsByType.Add(type, (ManagedUnrealEnumInfo)typeInfo);
                        enumsByInfo.Add((ManagedUnrealEnumInfo)typeInfo, type);
                        break;
                    case EPropertyType.Interface:
                        interfacesByType.Add(type, typeInfo);
                        interfacesByInfo.Add(typeInfo, type);
                        break;
                    case EPropertyType.Delegate:
                    case EPropertyType.MulticastDelegate:
                        delegatesByType.Add(type, typeInfo);
                        delegatesByInfo.Add(typeInfo, type);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void AddSerializedModuleInfoType(ManagedUnrealModuleInfo moduleInfo)
        {
            string serialized = ManagedUnrealReflectionBase.Serialize(moduleInfo);
            if (string.IsNullOrEmpty(serialized))
            {
                throw new RewriteException("Failed to serialize module " + moduleInfo.ModuleName);
            }

            string typeName = "SerializedManagedUnrealModuleInfo";

            List<TypeDefinition> serializeModuleInfoTypes = new List<TypeDefinition>();
            foreach (TypeDefinition type in assembly.MainModule.Types)
            {
                if (type.Name == typeName)
                {
                    serializeModuleInfoTypes.Add(type);
                }
            }
            VerifyNoResults(serializeModuleInfoTypes, null, typeName);

            TypeDefinition serializedModuleInfoType = new TypeDefinition(null, typeName, TypeAttributes.Public | TypeAttributes.Class);
            serializedModuleInfoType.BaseType = assembly.MainModule.ImportEx(typeof(object));
            serializedModuleInfoType.Interfaces.Add(assembly.MainModule.ImportEx(typeof(ISerializedManagedUnrealModuleInfo)));

            MethodDefinition defaultCtor = new MethodDefinition(".ctor",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                assembly.MainModule.ImportEx(typeof(void)));
            defaultCtor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            serializedModuleInfoType.Methods.Add(defaultCtor);

            MethodAttributes methodAttribute = MethodAttributes.Public | MethodAttributes.Virtual;
            MethodDefinition getStringMethod = new MethodDefinition("GetString", methodAttribute, assembly.MainModule.ImportEx(typeof(string)));
            serializedModuleInfoType.Methods.Add(getStringMethod);

            ILProcessor processor = getStringMethod.Body.GetILProcessor();
            processor.Emit(OpCodes.Ldstr, serialized);
            processor.Emit(OpCodes.Ret);
            FinalizeMethod(getStringMethod);

            assembly.MainModule.Types.Add(serializedModuleInfoType);
        }

        private void RewriteEnum(TypeDefinition type, ManagedUnrealTypeInfo enumInfo)
        {
            AddPathAttribute(type, enumInfo);
        }

        /// <summary>
        /// Returns the UClass for a UProperty which is stored in the Classes class.
        /// </summary>
        private FieldReference GetUPropertyClass(ManagedUnrealPropertyInfo propertyInfo)
        {
            EPropertyType propertyType = propertyInfo.Type.TypeCode;
            if (propertyInfo.IsFixedSizeArray)
            {
                propertyType = propertyInfo.GenericArgs[0].TypeCode;
            }
            return classesUPropertyFieldRefs[propertyType];
        }

        /// <summary>
        /// Creates LoadNativeType() which is used to load the native type info (class address/properties/functions/offsets)
        /// </summary>
        private void CreateLoadNativeTypeMethod(TypeDefinition type, ManagedUnrealTypeInfo typeInfo, InjectedMembers injectedMembers)
        {
            string methodName = codeSettings.VarNames.LoadNativeType;

            List<MethodDefinition> methodDefinitions = new List<MethodDefinition>();
            foreach (MethodDefinition methodDef in type.Methods)
            {
                if (methodDef.Name == methodName)
                {
                    methodDefinitions.Add(methodDef);
                }
            }
            VerifyNoResults(methodDefinitions, type, "method " + methodName);

            MethodAttributes methodAttributes = MethodAttributes.Private | MethodAttributes.Static;
            MethodDefinition method = new MethodDefinition(methodName, methodAttributes, assembly.MainModule.ImportEx(typeof(void)));
            type.Methods.Add(method);

            ILProcessor processor = method.Body.GetILProcessor();

            // This was originally a hook of the cctor which inserts all instructions before the last existing instruction.
            // Emit a "Ret" so that the code below still functions as before.
            processor.Emit(OpCodes.Ret);

            Instruction target = method.Body.Instructions[0];

            Instruction loadNativeClassPtr = null;

            bool isDelegate = typeInfo.IsDelegate;
            if (!isDelegate)
            {
                // Get the class / struct address
                // IntPtr classAddress = NativeReflection.GetClass("XXXX");
                processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, typeInfo.Path));
                processor.InsertBefore(target, processor.Create(OpCodes.Call,
                    typeInfo.TypeCode == EPropertyType.Struct ? reflectionGetStructMethod : reflectionGetClassMethod));

                if (injectedMembers.ClassAddress != null)
                {
                    processor.InsertBefore(target, processor.Create(OpCodes.Stsfld, injectedMembers.ClassAddress));
                    loadNativeClassPtr = processor.Create(OpCodes.Ldsfld, injectedMembers.ClassAddress);
                }
                else
                {
                    VariableDefinition nativeClassVar = new VariableDefinition(intPtrTypeRef);
                    method.Body.Variables.Add(nativeClassVar);
                    processor.InsertBefore(target, processor.Create(OpCodes.Stloc, nativeClassVar));
                    loadNativeClassPtr = processor.Create(OpCodes.Ldloc, nativeClassVar);
                }
            }

            // Get the struct size
            if (injectedMembers.StructSize != null)
            {
                // StructSize = NativeReflection.GetStructSize(classAddress);
                processor.InsertBefore(target, loadNativeClassPtr);
                processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionGetStructSizeMethod));
                processor.InsertBefore(target, processor.Create(OpCodes.Stsfld, injectedMembers.StructSize));
            }

            // Initialize properties
            foreach (InjectedMembers.InjectedPropertyInfo injectedPropertyInfo in injectedMembers.Properties.Values)
            {
                if (injectedPropertyInfo.PropertyAddress != null)
                {
                    // NativeReflection.GetPropertyRef(ref XXXX_PropertyAddress, classAddress, "XXXX"));
                    processor.InsertBefore(target, processor.Create(OpCodes.Ldsflda, injectedPropertyInfo.PropertyAddress));
                    processor.InsertBefore(target, loadNativeClassPtr);
                    processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, injectedPropertyInfo.PropertyInfo.Name));
                    processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionCachedGetPropertyRefMethod));
                    processor.InsertBefore(target, processor.Create(OpCodes.Pop));// Pop the result from GetPropertyRef
                }

                if (injectedPropertyInfo.Offset != null)
                {
                    // XXXX_Offset = NativeReflection.GetPropertyOffset(classAddress, "XXXX");
                    processor.InsertBefore(target, loadNativeClassPtr);
                    processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, injectedPropertyInfo.PropertyInfo.Name));
                    processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionCachedGetPropertyOffsetMethod));
                    processor.InsertBefore(target, processor.Create(OpCodes.Stsfld, injectedPropertyInfo.Offset));
                }

                if (injectedPropertyInfo.RepIndex != null)
                {
                    // XXXX_RepIndex = NativeReflection.GetPropertyRepIndex(classAddress, "XXXX");
                    processor.InsertBefore(target, loadNativeClassPtr);
                    processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, injectedPropertyInfo.PropertyInfo.Name));
                    processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionCachedGetPropertyRepIndexMethod));
                    processor.InsertBefore(target, processor.Create(OpCodes.Stsfld, injectedPropertyInfo.RepIndex));
                }

                if (injectedPropertyInfo.IsValid != null)
                {
                    // XXXX_IsValid = NativeReflection.ValidatePropertyClass(classAddress, "XXXX", Classes.YYYY);
                    FieldReference propertyClassField = GetUPropertyClass(injectedPropertyInfo.PropertyInfo);
                    processor.InsertBefore(target, loadNativeClassPtr);
                    processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, injectedPropertyInfo.PropertyInfo.Name));
                    processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, propertyClassField));
                    processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionCachedValidatePropertyClassMethod));
                    processor.InsertBefore(target, processor.Create(OpCodes.Stsfld, injectedPropertyInfo.IsValid));
                }
            }

            // Initialize functions
            foreach (InjectedMembers.InjectedFunctionInfo injectedFunctionInfo in injectedMembers.Functions.Values)
            {
                if (isDelegate)
                {
                    // If this is a delegate type get the function from the path as it doesn't have an owner (other than package)
                    if (injectedMembers.Functions.Values.Count > 1)
                    {
                        throw new RewriteException("Delegate type has more than 1 function '" + typeInfo.Path + "'");
                    }

                    // XXXX_FunctionAddress = NativeReflection.GetFunction("XXXX");
                    processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, typeInfo.Path));
                    processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionGetFunctionFromPathMethod));
                    processor.InsertBefore(target, processor.Create(OpCodes.Stsfld, injectedFunctionInfo.FunctionAddress));
                }
                else if (injectedFunctionInfo.FunctionAddress != null)
                {
                    // XXXX_FunctionAddress = NativeReflection.GetFunction(classAddress, "XXXX");
                    processor.InsertBefore(target, loadNativeClassPtr);
                    processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, injectedFunctionInfo.FunctionInfo.GetName()));
                    processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionCachedGetFunctionMethod));
                    processor.InsertBefore(target, processor.Create(OpCodes.Stsfld, injectedFunctionInfo.FunctionAddress));
                }

                if (injectedFunctionInfo.ParamsSize != null)
                {
                    // XXXX_ParamsSize = NativeReflection.GetFunctionParamsSize(XXXX_FunctionAddress);
                    processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, injectedFunctionInfo.FunctionAddress));
                    processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionGetFunctionParamsSizeMethod));
                    processor.InsertBefore(target, processor.Create(OpCodes.Stsfld, injectedFunctionInfo.ParamsSize));
                }

                foreach (InjectedMembers.InjectedFunctionParamInfo paramInfo in injectedFunctionInfo.Params.Values)
                {
                    if (paramInfo.Address != null)
                    {
                        // NativeReflection.GetPropertyRef(ref XXXX_YYYY_PropertyAddress, XXXX_FunctionAddress, "YYYY");
                        processor.InsertBefore(target, processor.Create(OpCodes.Ldsflda, paramInfo.Address));
                        processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, injectedFunctionInfo.FunctionAddress));
                        processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, paramInfo.PropertyInfo.Name));
                        processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionCachedGetPropertyRefMethod));
                        processor.InsertBefore(target, processor.Create(OpCodes.Pop));// Pop the result from GetPropertyRef
                    }

                    if (paramInfo.Offset != null)
                    {
                        // XXXX_YYYY_Offset = NativeReflection.GetPropertyOffset(XXXX_FunctionAddress, "YYYY");
                        processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, injectedFunctionInfo.FunctionAddress));
                        processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, paramInfo.PropertyInfo.Name));
                        processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionCachedGetPropertyOffsetMethod));
                        processor.InsertBefore(target, processor.Create(OpCodes.Stsfld, paramInfo.Offset));
                    }

                    if (paramInfo.ElementSize != null)
                    {
                        // XXXX_YYYY_ElementSize = NativeReflection.GetFunctionParamsSize(XXXX_FunctionAddress);
                        processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, injectedFunctionInfo.FunctionAddress));
                        processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, paramInfo.PropertyInfo.Name));
                        processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionCachedGetPropertyArrayElementSizeMethod));
                        processor.InsertBefore(target, processor.Create(OpCodes.Stsfld, paramInfo.ElementSize));
                    }

                    if (paramInfo.IsValid != null)
                    {
                        // XXXX_YYYY_IsValid = NativeReflection.ValidatePropertyClass(XXXX_FunctionAddress, "YYYY", Classes.ZZZZ);
                        FieldReference propertyClassField = GetUPropertyClass(paramInfo.PropertyInfo);
                        processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, injectedFunctionInfo.FunctionAddress));
                        processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, paramInfo.PropertyInfo.Name));
                        processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, propertyClassField));
                        processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionCachedValidatePropertyClassMethod));
                        processor.InsertBefore(target, processor.Create(OpCodes.Stsfld, paramInfo.IsValid));
                    }
                }

                // XXXX_IsValid = XXXX_FunctionAddress != IntPtr.Zero && Param1_IsValid && Param1_IsValid...;
                // NativeReflection.LogFunctionIsValid("XXXX", XXXX_IsValid);
                if (injectedFunctionInfo.IsValid != null)
                {
                    Instruction loadFalse = processor.Create(OpCodes.Ldc_I4_0);
                    Instruction setIsValid = processor.Create(OpCodes.Stsfld, injectedFunctionInfo.IsValid);

                    processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, injectedFunctionInfo.FunctionAddress));
                    processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, intPtrZeroFieldRef));
                    processor.InsertBefore(target, processor.Create(OpCodes.Call, intPtrNotEqualsMethod));

                    foreach (InjectedMembers.InjectedFunctionParamInfo paramInfo in injectedFunctionInfo.Params.Values)
                    {
                        if (paramInfo.IsValid != null)
                        {
                            // Brfalse for the previous statement (including classAddress != IntPtr.Zero)
                            // - This is done so that the last statement has either a 0/1 on the stack for the Br/Stslfd
                            processor.InsertBefore(target, processor.Create(OpCodes.Brfalse, loadFalse));

                            processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, paramInfo.IsValid));
                        }
                    }

                    // If all conditions are met (true on should be on the stack) branch to the store the XXXX_IsValid,
                    // otherwise push 0 onto the stack and then store
                    processor.InsertBefore(target, processor.Create(OpCodes.Br, setIsValid));
                    processor.InsertBefore(target, loadFalse);
                    processor.InsertBefore(target, setIsValid);

                    processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, isDelegate ? typeInfo.Path : injectedFunctionInfo.FunctionInfo.Path));
                    processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, injectedFunctionInfo.IsValid));
                    processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionLogFunctionIsValidMethod));
                }
            }

            // NativeReflection.ValidateBlittableStructSize(classAddress, typeof(XXXX));
            if (injectedMembers.StructSize != null && typeInfo.IsBlittable)
            {
                processor.InsertBefore(target, loadNativeClassPtr);
                processor.InsertBefore(target, processor.Create(OpCodes.Ldtoken, type));
                processor.InsertBefore(target, processor.Create(OpCodes.Call, typeGetTypeFromHandleMethod));
                processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionValidateBlittableStructSizeMethod));
            }

            // XXXX_IsValid = classAddress != IntPtr.Zero && Prop1_IsValid && Prop2_IsValid...;
            // NativeReflection.LogStructIsValid("XXXX", XXXX_IsValid);
            if (injectedMembers.StructIsValid != null)
            {
                Instruction loadFalse = processor.Create(OpCodes.Ldc_I4_0);
                Instruction setIsValid = processor.Create(OpCodes.Stsfld, injectedMembers.StructIsValid);

                processor.InsertBefore(target, loadNativeClassPtr);
                processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, intPtrZeroFieldRef));
                processor.InsertBefore(target, processor.Create(OpCodes.Call, intPtrNotEqualsMethod));

                foreach (InjectedMembers.InjectedPropertyInfo injectedPropertyInfo in injectedMembers.Properties.Values)
                {
                    if (injectedPropertyInfo.IsValid != null)
                    {
                        // Brfalse for the previous statement (including classAddress != IntPtr.Zero)
                        // - This is done so that the last statement has either a 0/1 on the stack for the Br/Stslfd
                        processor.InsertBefore(target, processor.Create(OpCodes.Brfalse, loadFalse));

                        processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, injectedPropertyInfo.IsValid));
                    }
                }

                // If all conditions are met (true on should be on the stack) branch to the store the XXXX_IsValid,
                // otherwise push 0 onto the stack and then store
                processor.InsertBefore(target, processor.Create(OpCodes.Br, setIsValid));
                processor.InsertBefore(target, loadFalse);
                processor.InsertBefore(target, setIsValid);

                processor.InsertBefore(target, processor.Create(OpCodes.Ldstr, typeInfo.Path));
                processor.InsertBefore(target, processor.Create(OpCodes.Ldsfld, injectedMembers.StructIsValid));
                processor.InsertBefore(target, processor.Create(OpCodes.Call, reflectionLogStructIsValidMethod));
            }

            FinalizeMethod(method);

            HookStaticConstructor(type, method);
        }

        /// <summary>
        /// Modifies or creates a cctor to call to LoadNativeType() and adds a call to notify UnrealTypes that the cctor has been called
        /// </summary>
        private void HookStaticConstructor(TypeDefinition type, MethodDefinition loadNativeTypeMethod)
        {
            MethodDefinition cctor = TypeDefinitionRocks.GetStaticConstructor(type);
            ILProcessor processor;

            if (cctor == null)
            {
                MethodAttributes cctorAttributes = MethodAttributes.Private | MethodAttributes.HideBySig |
                    MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Static;
                cctor = new MethodDefinition(".cctor", cctorAttributes, assembly.MainModule.ImportEx(typeof(void)));
                type.Methods.Add(cctor);

                processor = cctor.Body.GetILProcessor();
                processor.Emit(OpCodes.Ret);
            }
            else
            {
                processor = cctor.Body.GetILProcessor();
            }

            Instruction target = cctor.Body.Instructions[0];

            Instruction branchTargetInstruction = processor.Create(OpCodes.Ldtoken, type);

            // if (UnrealTypes.CanLazyLoadNativeType(typeof(XXXX))) { LoadNativeType(); }
            processor.InsertBefore(target, processor.Create(OpCodes.Ldtoken, type));
            processor.InsertBefore(target, processor.Create(OpCodes.Call, typeGetTypeFromHandleMethod));
            processor.InsertBefore(target, processor.Create(OpCodes.Call, unrealTypesCanLazyLoadManagedTypeMethod));
            processor.InsertBefore(target, processor.Create(OpCodes.Brfalse, branchTargetInstruction));
            processor.InsertBefore(target, processor.Create(OpCodes.Call, loadNativeTypeMethod));

            // UnrealTypes.OnCCtorCalled(typeof(XXXX));
            processor.InsertBefore(target, branchTargetInstruction);
            processor.InsertBefore(target, processor.Create(OpCodes.Call, typeGetTypeFromHandleMethod));
            processor.InsertBefore(target, processor.Create(OpCodes.Call, unrealTypesOnCCtorCalledMethod));

            FinalizeMethod(cctor);
        }

        private void AddPathAttribute(TypeDefinition type, ManagedUnrealTypeInfo typeInfo, TypeReference interfaceImplType)
        {
            AddSharpPathAttribute(type.CustomAttributes, typeInfo.Path, interfaceImplType);
        }

        private void AddPathAttribute(TypeDefinition type, ManagedUnrealTypeInfo typeInfo)
        {
            AddSharpPathAttribute(type.CustomAttributes, typeInfo.Path, null);
        }

        private void AddPathAttribute(PropertyDefinition prop, ManagedUnrealPropertyInfo propInfo)
        {
            AddSharpPathAttribute(prop.CustomAttributes, propInfo.Path, null);
        }

        private void AddPathAttribute(FieldDefinition prop, ManagedUnrealPropertyInfo propInfo)
        {
            AddSharpPathAttribute(prop.CustomAttributes, propInfo.Path, null);
        }

        private void AddPathAttribute(MethodDefinition method, ManagedUnrealFunctionInfo functionInfo)
        {
            AddSharpPathAttribute(method.CustomAttributes, functionInfo.Path, null);
        }

        private void AddSharpPathAttribute(Mono.Collections.Generic.Collection<CustomAttribute> attributes, string path,
            TypeReference interfaceImplType)
        {
            for (int i = attributes.Count - 1; i >= 0; i--)
            {
                // Type comparison doesn't seem to work. TODO: look into
                if (attributes[i].AttributeType.FullName == sharpPathAttributeTypeRef.FullName)
                {
                    attributes.RemoveAt(i);
                    break;
                }
            }

            CustomAttribute sharpPathAttribute = new CustomAttribute(sharpPathAttributeTypeCtor);
            attributes.Add(sharpPathAttribute);
            sharpPathAttribute.ConstructorArguments.Clear();
            sharpPathAttribute.ConstructorArguments.Add(new CustomAttributeArgument(stringTypeRef, path));
            if (interfaceImplType != null)
            {
                sharpPathAttribute.Fields.Add(new CustomAttributeNamedArgument("InterfaceImpl",
                    new CustomAttributeArgument(typeTypeRef, interfaceImplType)));
            }
        }

        /// <summary>
        /// Inserts an object destroyed check at the start of the given ILProcessor (if object destroyed checks are enabled)
        /// </summary>
        private void InsertObjectDestroyedCheck(ManagedUnrealTypeInfo typeInfo, ILProcessor processor)
        {
            if (codeSettings.CheckObjectDestroyed)
            {
                processor.Body.Instructions.Insert(0, processor.Create(OpCodes.Ldarg_0));
                switch (typeInfo.TypeCode)
                {
                    case EPropertyType.Object:
                        processor.Body.Instructions.Insert(1, processor.Create(OpCodes.Call, checkUObjectDestroyedMethod));
                        break;
                    case EPropertyType.Interface:
                        processor.Body.Instructions.Insert(1, processor.Create(OpCodes.Call, checkInterfaceObjDestroyedMethod));
                        break;
                    case EPropertyType.Struct:
                        Debug.Assert(typeInfo.IsStructAsClass);
                        processor.Body.Instructions.Insert(1, processor.Create(OpCodes.Call, checkStructAsClassDestroyedMethod));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private Instruction[] GetLoadNativeBufferInstructions(ManagedUnrealTypeInfo typeInfo, ILProcessor processor,
            Instruction loadBufferInstruction, FieldDefinition offsetField)
        {
            // "IntPtr.Add(Address, offset);"

            List<Instruction> instructionBuffer = new List<Instruction>();
            if (loadBufferInstruction != null)
            {
                instructionBuffer.Add(loadBufferInstruction);
            }
            else
            {
                instructionBuffer.Add(processor.Create(OpCodes.Ldarg_0));
                switch (typeInfo.TypeCode)
                {
                    case EPropertyType.Object:
                        instructionBuffer.Add(processor.Create(OpCodes.Call, uobjectAddressGetter));
                        break;
                    case EPropertyType.Interface:
                        instructionBuffer.Add(processor.Create(OpCodes.Call, iinterfaceImplAddressGetter));
                        break;
                    case EPropertyType.Struct:
                        Debug.Assert(typeInfo.IsStructAsClass);
                        instructionBuffer.Add(processor.Create(OpCodes.Call, structAsClassAddressGetter));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            instructionBuffer.Add(processor.Create(OpCodes.Ldsfld, offsetField));
            instructionBuffer.Add(processor.Create(OpCodes.Call, intPtrAddMethod));

            return instructionBuffer.ToArray();
        }

        private void StripBackingField(TypeDefinition type, ManagedUnrealPropertyInfo propertyInfo)
        {
            string backingFieldName = string.Format("<{0}>k__BackingField", propertyInfo.Name);

            List<FieldDefinition> matchingFields = new List<FieldDefinition>();
            foreach (FieldDefinition field in type.Fields)
            {
                if (field.Name == backingFieldName)
                {
                    matchingFields.Add(field);
                }
            }

            VerifySingleResult(matchingFields, type, "backing field for property " + propertyInfo.Name);

            type.Fields.Remove(matchingFields[0]);
        }

        private MethodDefinition FindMethodByName(TypeDefinition type, string methodName)
        {
            List<MethodDefinition> matchingMethods = new List<MethodDefinition>();
            foreach (MethodDefinition method in type.Methods)
            {
                if (method.Name == methodName)
                {
                    matchingMethods.Add(method);
                }
            }

            VerifySingleResult(matchingMethods, type, "method " + methodName);

            return matchingMethods[0];
        }

        private MethodDefinition FindBaseMethodByName(TypeDefinition type, string methodName)
        {
            TypeDefinition baseType = type.BaseType.Resolve();
            MethodDefinition baseMethod = null;
            while (baseMethod == null)
            {
                foreach (MethodDefinition method in baseType.Methods)
                {
                    if (method.Name == methodName && method.IsVirtual)
                    {
                        baseMethod = method;
                        break;
                    }
                }
                if (baseType.BaseType == null)
                {
                    break;
                }
                baseType = baseType.BaseType.Resolve();
            }
            return baseMethod;
        }

        private PropertyDefinition FindPropertyByName(TypeDefinition type, string propertyName)
        {
            List<PropertyDefinition> matchingProperties = new List<PropertyDefinition>();
            foreach (PropertyDefinition property in type.Properties)
            {
                if (property.Name == propertyName)
                {
                    matchingProperties.Add(property);
                }
            }

            VerifySingleResult(matchingProperties, type, "property " + propertyName);

            return matchingProperties[0];
        }

        // <Assembly.FullName, <Type.FullName, Type>>
        static Dictionary<string, Dictionary<string, Type>> assemblyTypes = new Dictionary<string, Dictionary<string, Type>>();
        private Type GetTypeFromTypeDefinition(TypeDefinition type)
        {
            // TODO: Remove this method and use cecil equivalents instead of Type

            Dictionary<string, Type> types;
            if (!assemblyTypes.TryGetValue(type.Module.Assembly.FullName, out types))
            {
                assemblyTypes.Add(type.Module.Assembly.FullName, types = new Dictionary<string, Type>());

                foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.FullName == type.Module.Assembly.FullName)
                    {
                        foreach (Type assemblyType in assembly.GetTypes())
                        {
                            types[assemblyType.FullName] = assemblyType;
                        }
                        break;
                    }
                }
            }

            Type result;
            types.TryGetValue(type.FullName.Replace("/", "+"), out result);
            return result;

            //return Type.GetType(System.Reflection.Assembly.CreateQualifiedName(type.Module.Assembly.FullName, type.FullName.Replace("/", "+")));
        }

        private MethodReference MakeGenericMethod(MethodReference method, params TypeReference[] args)
        {
            if (args.Length == 0)
            {
                return method;
            }

            if (method.GenericParameters.Count != args.Length)
            {
                throw new ArgumentException("Invalid number of generic type arguments supplied");
            }

            GenericInstanceMethod genericMethodRef = new GenericInstanceMethod(method);
            foreach (TypeReference arg in args)
            {
                genericMethodRef.GenericArguments.Add(arg);
            }

            return genericMethodRef;
        }

        private MethodReference GetConstrainedGenericTypeCtor(TypeReference genericType, MethodReference ctor)
        {
            MethodReference reference = new MethodReference(ctor.Name, ctor.ReturnType, genericType);
            reference.HasThis = ctor.HasThis;
            reference.ExplicitThis = ctor.ExplicitThis;
            reference.CallingConvention = ctor.CallingConvention;
            foreach (ParameterReference parameter in ctor.Parameters)
            {
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
            }
            return reference;
        }

        private MethodDefinition CopyMethod(MethodDefinition method, bool overrideMethod)
        {
            return CopyMethod(method, overrideMethod, assembly.MainModule.ImportEx(method.ReturnType));
        }

        private MethodDefinition CopyMethod(MethodDefinition method, bool overrideMethod, TypeReference returnType)
        {
            MethodDefinition newMethod = new MethodDefinition(method.Name, method.Attributes, returnType);

            if (overrideMethod)
            {
                newMethod.Attributes &= ~MethodAttributes.VtableLayoutMask;
                newMethod.Attributes &= ~MethodAttributes.NewSlot;
                newMethod.Attributes |= MethodAttributes.ReuseSlot;
            }
            newMethod.HasThis = true;
            newMethod.ExplicitThis = method.ExplicitThis;
            newMethod.CallingConvention = method.CallingConvention;

            foreach (ParameterDefinition parameter in method.Parameters)
            {
                newMethod.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes,
                    assembly.MainModule.ImportEx(parameter.ParameterType)));
            }

            return newMethod;
        }

        private void FinalizeMethod(MethodDefinition method)
        {
            if (method.Body.Variables.Count > 0)
            {
                method.Body.InitLocals = true;
            }

            method.Body.SimplifyMacros();
            method.Body.OptimizeMacros();
        }

        private void AppendInstructions(ILProcessor processor, Instruction[] instructions)
        {
            foreach (Instruction instruction in instructions)
            {
                processor.Append(instruction);
            }
        }

        private void InsertInstructionsAfter(ILProcessor processor, Instruction insertAfter, Instruction[] instructions)
        {
            foreach (Instruction instruction in instructions)
            {
                processor.InsertAfter(insertAfter, instruction);
            }
        }

        private void InsertInstructionsBefore(ILProcessor processor, Instruction insertBefore, Instruction[] instructions)
        {
            foreach (Instruction instruction in instructions)
            {
                processor.InsertBefore(insertBefore, instruction);
            }
        }

        private void InsertInstructionsAt(ILProcessor processor, int index, Instruction[] instructions)
        {
            for (int i = instructions.Length - 1; i >= 0; --i)
            {
                processor.Body.Instructions.Insert(index, instructions[i]);
            }
        }

        private void VerifyFieldsNotExist(TypeDefinition type, params string[] fieldNames)
        {
            foreach (FieldDefinition field in type.Fields)
            {
                if (fieldNames.Contains(field.Name))
                {
                    throw new RewriteException(type, "Found existing field " + field.Name);
                }
            }
        }

        private void VerifySingleResult<T>(List<T> results, TypeDefinition type, string message)
        {
            if (results == null || results.Count == 0)
            {
                throw new RewriteException(type, "Could not find " + message);
            }

            if (results.Count > 1)
            {
                throw new RewriteException(type, "Found more than one " + message);
            }
        }

        private void VerifyNoResults<T>(List<T> results, TypeDefinition type, string message)
        {
            if (results != null && results.Count > 0)
            {
                throw new RewriteException(type, "Found existing " + message);
            }
        }

        private void VerifyNonNull<T>(T value, TypeDefinition type, string message)
        {
            if (value == null)
            {
                throw new RewriteException(type, "Unexpected null " + message);
            }
        }
    }

    class RewriteException : Exception
    {
        public RewriteException(string message)
            : base(message)
        {
        }

        public RewriteException(string typePath, string message)
            : base(typePath == null ? message : typePath + ": " + message)
        {
        }

        public RewriteException(TypeDefinition type, string message)
            : base(type == null ? message : type.FullName + ": " + message)
        {
        }
    }
}
