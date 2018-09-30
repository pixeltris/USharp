using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public static class ManagedUnrealTypes
    {
        public static Dictionary<Type, ManagedUnrealClass> Classes { get; private set; }
        public static Dictionary<IntPtr, ManagedUnrealClass> ClassesByAddress { get; private set; }

        private static int HotReloadClassCount = 0;

        static ManagedUnrealTypes()
        {
            Classes = new Dictionary<Type, ManagedUnrealClass>();
            ClassesByAddress = new Dictionary<IntPtr, ManagedUnrealClass>();
        }

        public static void ReinstanceHotReloadedClasses()
        {
            if (HotReloadClassCount > 0)
            {
                foreach (KeyValuePair<Type, ManagedUnrealClass> managedClass in Classes)
                {
                    if (managedClass.Value.OldClass != IntPtr.Zero)
                    {
                        Native_USharpClass.RegisterClassForHotReloadReinstance(managedClass.Value.OldClass, managedClass.Value.StaticClass);
                        managedClass.Value.OldClass = IntPtr.Zero;
                    }
                }
                Native_USharpClass.ReinstanceHotReloadedClasses();
            }
            HotReloadClassCount = 0;
        }

        private static bool IsUnrealType(Type type)
        {
            return IsManagedUnrealType(type) || IsNativeUnrealType(type);
        }

        public static bool IsManagedUnrealType(Type type)
        {
            return type.GetCustomAttribute<USharpPathAttribute>() != null;
        }

        public static bool IsNativeUnrealType(Type type)
        {
            return type.GetCustomAttribute<UMetaPathAttribute>() != null;
        }        

        public static ManagedUnrealClass FindClass(Type type)
        {
            ManagedUnrealClass result;
            Classes.TryGetValue(type, out result);
            if (result == null)
            {

            }
            return result;
        }

        internal static IntPtr GetEnum(Type type)
        {
            if (IsManagedUnrealType(type))
            {
            }
            else
            {
                UMetaPathAttribute pathAttribute = type.GetCustomAttribute<UMetaPathAttribute>();
                if (pathAttribute != null)
                {
                    return NativeReflection.FindObject(Native_UEnum.StaticClass(), IntPtr.Zero, pathAttribute.Path, true);
                }
            }
            return IntPtr.Zero;
        }

        internal static IntPtr GetStaticClass(Type type)
        {
            if (IsManagedUnrealType(type))
            {
                ManagedUnrealClass baseClass = FindClass(type);
                if (baseClass != null)
                {
                    return baseClass.StaticClass;                    
                }
                baseClass = CreateClass(type);
                if (baseClass != null)
                {
                    return baseClass.StaticClass;
                }
            }
            else
            {
                UMetaPathAttribute pathAttribute = type.GetCustomAttribute<UMetaPathAttribute>();
                if (pathAttribute != null)
                {
                    return UClass.FindClassAddressByPath(pathAttribute.Path);
                }
            }
            return IntPtr.Zero;
        }

        public static ManagedUnrealClass CreateClass(Type type)
        {
            ManagedUnrealClass existingClass = FindClass(type);
            if (existingClass != null)
            {
                if (!FBuild.WithHotReload)
                {
                    // TODO: Add support for hotreloading C# classes when WITH_HOT_RELOAD isn't available
                    // - WITH_HOT_RELOAD will be false on shipping, monolithic and server builds
                    // - Would need to make a copy of FHotReloadClassReinstancer (or just use it directly if
                    //   it doesn't depend on WITH_HOT_RELOAD and gets compiled into builds)
                    // - Would likely break blueprint classes which depend on any C# classes reinstanced in this way
                    return existingClass;
                }

                existingClass.Clear();
                HotReloadClassCount++;
            }

            if (!type.IsSubclassOf(typeof(UObject)))
            {
                return null;
            }

            USharpPathAttribute pathAttribute = type.GetCustomAttribute<USharpPathAttribute>();
            if (pathAttribute == null || string.IsNullOrEmpty(pathAttribute.Path))
            {
                return null;
            }

            IntPtr parentClass = GetStaticClass(type.BaseType);
            if (parentClass == IntPtr.Zero)
            {
                return null;
            }

            string root, directory, moduleName, className, memberName;
            FPackageName.GetPathInfo(pathAttribute.Path, out root, out directory, out moduleName, out className, out memberName);

            string packageName = "/" + root + "/" + directory;

            if (string.IsNullOrEmpty(moduleName) || string.IsNullOrEmpty(className))
            {
                return null;
            }

            IntPtr package = NativeReflection.FindObject(Native_UPackage.StaticClass(), IntPtr.Zero, packageName, true);
            if (package == IntPtr.Zero)
            {
                package = NativeReflection.CreatePackage(IntPtr.Zero, packageName);
                Native_UPackage.SetPackageFlags(package, EPackageFlags.CompiledIn);

                // TODO: Find how to create a proper guid for a package (UHT CodeGenerator.cpp seems to use a crc of generated code)
                using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(packageName));

                    // Truncate the hash
                    byte[] buffer = new byte[16];
                    Buffer.BlockCopy(hash, 0, buffer, 0, buffer.Length);

                    Native_UPackage.SetGuid(package, new Guid(buffer));
                }
            }

            ManagedUnrealClass managedUnrealClass = null;
            if (existingClass != null)
            {
                managedUnrealClass = existingClass;
            }
            else
            {
                managedUnrealClass = new ManagedUnrealClass(type, packageName, className, parentClass);
            }
            
            managedUnrealClass.StaticClass = USharpClass.CreateClassPtr(
                    managedUnrealClass.PackageName,
                    managedUnrealClass.ClassName,
                    (uint)Native_UStruct.GetPropertiesSize(managedUnrealClass.ParentClass),
                    EClassFlags.None,
                    EClassCastFlags.None,
                    managedUnrealClass.ConfigName,
                    managedUnrealClass.ParentClass,
                    managedUnrealClass.WithinClass,
                    managedUnrealClass.ClassConstructor,
                    managedUnrealClass.ClassVTableHelperCtorCaller,
                    managedUnrealClass.ClassAddReferencedObjects);

            Native_UObjectBase.UObjectForceRegistration(managedUnrealClass.StaticClass);

            if (existingClass == null)
            {
                Classes.Add(type, managedUnrealClass);
                ClassesByAddress.Add(managedUnrealClass.StaticClass, managedUnrealClass);
            }

            managedUnrealClass.Initialize();

            return managedUnrealClass;
        }

        public static IntPtr GetPropertyClass(EPropertyType propertyType)
        {
            switch (propertyType)
            {
                case EPropertyType.Bool: return Native_UBoolProperty.StaticClass();

                case EPropertyType.Int8: return Native_UInt8Property.StaticClass();
                case EPropertyType.Int16: return Native_UInt16Property.StaticClass();
                case EPropertyType.Int: return Native_UIntProperty.StaticClass();
                case EPropertyType.Int64: return Native_UInt64Property.StaticClass();

                case EPropertyType.Byte: return Native_UByteProperty.StaticClass();
                case EPropertyType.UInt16: return Native_UUInt16Property.StaticClass();
                case EPropertyType.UInt32: return Native_UUInt32Property.StaticClass();
                case EPropertyType.UInt64: return Native_UUInt64Property.StaticClass();

                case EPropertyType.Double: return Native_UDoubleProperty.StaticClass();
                case EPropertyType.Float: return Native_UFloatProperty.StaticClass();

                case EPropertyType.Enum: return Native_UEnumProperty.StaticClass();

                case EPropertyType.Interface: return Native_UInterfaceProperty.StaticClass();
                case EPropertyType.Struct: return Native_UStructProperty.StaticClass();
                case EPropertyType.Class: return Native_UClassProperty.StaticClass();

                case EPropertyType.Object: return Native_UObjectProperty.StaticClass();
                case EPropertyType.LazyObject: return Native_ULazyObjectProperty.StaticClass();
                case EPropertyType.WeakObject: return Native_UWeakObjectProperty.StaticClass();

                case EPropertyType.SoftClass: return Native_USoftClassProperty.StaticClass();
                case EPropertyType.SoftObject: return Native_USoftObjectProperty.StaticClass();

                case EPropertyType.Delegate: return Native_UDelegateProperty.StaticClass();
                case EPropertyType.MulticastDelegate: return Native_UMulticastDelegateProperty.StaticClass();

                case EPropertyType.Array: return Native_UArrayProperty.StaticClass();
                case EPropertyType.Map: return Native_UMapProperty.StaticClass();
                case EPropertyType.Set: return Native_USetProperty.StaticClass();

                case EPropertyType.Str: return Native_UStrProperty.StaticClass();
                case EPropertyType.Name: return Native_UNameProperty.StaticClass();
                case EPropertyType.Text: return Native_UTextProperty.StaticClass();

                default: return IntPtr.Zero;
            }
        }

        public static EPropertyType GetPropertyType(Type type, EPropertyType predefinedType)
        {
            return predefinedType != EPropertyType.Unknown ? predefinedType : GetPropertyType(type);
        }

        public static EPropertyType GetPropertyType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean: return EPropertyType.Bool;

                case TypeCode.SByte: return EPropertyType.Int8;
                case TypeCode.Int16: return EPropertyType.Int16;
                case TypeCode.Int32: return EPropertyType.Int;
                case TypeCode.Int64: return EPropertyType.Int64;

                case TypeCode.Byte: return EPropertyType.Byte;
                case TypeCode.UInt16: return EPropertyType.UInt16;
                case TypeCode.UInt32: return EPropertyType.UInt32;
                case TypeCode.UInt64: return EPropertyType.UInt64;

                case TypeCode.Double: return EPropertyType.Double;
                case TypeCode.Single: return EPropertyType.Float;

                case TypeCode.String: return EPropertyType.Str;
            }

            if (type.IsEnum)
            {
                return EPropertyType.Enum;
            }

            if (type == typeof(FName))
            {
                return EPropertyType.Name;
            }
            //if (type == typeof(FText))
            //{
            //    return EPropertyType.Text;
            //}

            if (type.IsSameOrSubclassOf(typeof(UObject)))
            {
                return EPropertyType.Object;
            }

            // TODO: Interface

            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();
                
                if (genericType.IsSameOrSubclassOf(typeof(TLazyObject<>)))
                {
                    return EPropertyType.LazyObject;
                }
                if (genericType.IsSameOrSubclassOf(typeof(TWeakObject<>)))
                {
                    return EPropertyType.WeakObject;
                }

                if (genericType.IsSameOrSubclassOf(typeof(TSubclassOf<>)))
                {
                    if (!IsUnrealType(type.GenericTypeArguments[0]))
                    {
                        return EPropertyType.Unknown;
                    }
                    return EPropertyType.Class;
                }

                if (genericType.IsSameOrSubclassOf(typeof(TSoftClass<>)))
                {
                    return EPropertyType.SoftClass;
                }
                if (genericType.IsSameOrSubclassOf(typeof(TSoftObject<>)))
                {
                    return EPropertyType.SoftObject;
                }

                if (genericType.IsSameOrSubclassOf(typeof(FDelegate<>)))
                {
                    return EPropertyType.Delegate;
                }
                if (genericType.IsSameOrSubclassOf(typeof(FMulticastDelegate<>)))
                {
                    return EPropertyType.MulticastDelegate;
                }

                Type[] interfaces = genericType.GetInterfaces();
                for (int i = 0; i < interfaces.Length; i++)
                {
                    if (interfaces[i].IsGenericType)
                    {
                        Type genericInterfaceType = interfaces[i].GetGenericTypeDefinition();

                        if (genericInterfaceType.IsSameOrSubclassOf(typeof(ISet<>)))
                        {
                            return EPropertyType.Set;
                        }
                        if (genericInterfaceType.IsSameOrSubclassOf(typeof(IDictionary<,>)))
                        {
                            return EPropertyType.Map;
                        }
                        if (genericInterfaceType.IsSameOrSubclassOf(typeof(IList<>)) ||
                            genericInterfaceType.IsSameOrSubclassOf(typeof(IReadOnlyList<>)))
                        {
                            return EPropertyType.Array;
                        }
                    }
                }
            }

            if (type.IsValueType && !type.IsPrimitive && IsUnrealType(type))
            {
                return EPropertyType.Struct;
            }

            return EPropertyType.Unknown;
        }
    }

    /// <summary>
    /// Holds onto a USharpClass static class defined in managed code
    /// </summary>
    public class ManagedUnrealClass
    {
        public IntPtr StaticClass { get; set; }
        
        /// <summary>
        /// The old StaticClass, used for hotreload
        /// </summary>
        public IntPtr OldClass { get; set; }

        /// <summary>
        /// The managed type for this class
        /// </summary>
        public Type Type { get; private set; }

        public string PackageName { get; private set; }
        public string ClassName { get; private set; }
        public string ConfigName { get; private set; }

        public string PathName
        {
            get { return PackageName + "." + ClassName; }
        }

        public IntPtr WithinClass { get; private set; }

        /// <summary>
        /// The parent class
        /// </summary>
        public IntPtr ParentClass { get; private set; }

        /// <summary>
        /// The first non-USharpClass in the class hierarchy
        /// </summary>
        public IntPtr NonUSharpClassParentClass { get; private set; }

        /// <summary>
        /// The first native class in the class hierarchy
        /// </summary>
        public IntPtr NativeParentClass { get; private set; }        

        public USharpClass.ClassConstructorType ClassConstructor { get; private set; }
        public USharpClass.ClassVTableHelperCtorCallerType ClassVTableHelperCtorCaller { get; private set; }
        public USharpClass.ClassAddReferencedObjectsType ClassAddReferencedObjects { get; private set; }

        private CachedUObject<USharpClass> staticClass;
        private CachedUObject<UClass> parentClass;

        static bool firstRun = true;

        public ManagedUnrealClass(Type type, string packageName, string className, IntPtr parentClass)
        {
            Type = type;
            PackageName = packageName;
            ClassName = className;
            ParentClass = parentClass;
            ClassConstructor = Constructor;
            ClassVTableHelperCtorCaller = VTableHelperCtorCaller;
            ClassAddReferencedObjects = AddReferencedObjects;

            // This is what FKismetCompilerContext::CleanAndSanitizeClass uses
            IntPtr parentClassWithin = Native_UClass.Get_ClassWithin(ParentClass);
            WithinClass = parentClassWithin != IntPtr.Zero ? parentClassWithin : Native_UObject.StaticClass();

            NonUSharpClassParentClass = FindFirstNonUSharpClassParentClass(ParentClass);
            NativeParentClass = FindFirstNativeParentClass(ParentClass);
        }

        public USharpClass GetStaticClass()
        {
            return staticClass.Update(StaticClass);
        }

        public UClass GetParentClass()
        {
            return parentClass.Update(ParentClass);
        }

        private IntPtr FindFirstNonUSharpClassParentClass(IntPtr unrealClass)
        {
            IntPtr sharpStaticClass = Native_USharpClass.StaticClass();
            while (unrealClass != IntPtr.Zero && !Native_UObjectBaseUtility.IsA(unrealClass, sharpStaticClass))
            {
                unrealClass = Native_UClass.GetSuperClass(unrealClass);
            }
            return unrealClass;
        }

        private IntPtr FindFirstNativeParentClass(IntPtr unrealClass)
        {
            IntPtr sharpStaticClass = Native_USharpClass.StaticClass();
            while (unrealClass != IntPtr.Zero && (!Native_UClass.HasAnyClassFlags(unrealClass, EClassFlags.Native) ||
                Native_UObjectBaseUtility.IsA(unrealClass, sharpStaticClass)))
            {
                unrealClass = Native_UClass.GetSuperClass(unrealClass);
            }
            return unrealClass;
        }

        private void Constructor(IntPtr objectInitializerPtr)
        {
            Native_UClass.Call_ClassConstructor(ParentClass, objectInitializerPtr);
            FObjectInitializer objectInitializer = new FObjectInitializer(objectInitializerPtr);

            IntPtr sharpStaticClass = Native_USharpClass.StaticClass();
            IntPtr unrealClass = Native_FObjectInitializer.GetClass(objectInitializerPtr);
            IntPtr sharpClass = unrealClass;
            while (sharpClass != IntPtr.Zero && !Native_UObjectBaseUtility.IsA(sharpClass, sharpStaticClass))
            {
                sharpClass = Native_UClass.GetSuperClass(sharpClass);
            }

            System.Diagnostics.Debug.Assert(sharpClass != IntPtr.Zero);
        }

        private IntPtr VTableHelperCtorCaller(IntPtr vtableHelper)
        {
            return IntPtr.Zero;
        }

        private void AddReferencedObjects(IntPtr inThis, IntPtr collector)
        {
        }

        private void MoveToTransientPackage(IntPtr obj)
        {
            // Copy of UObjectBase.cpp UClassCompiledInDefer

            // Check if rooted?
            Native_UObjectBaseUtility.RemoveFromRoot(obj);
            Native_UObjectBaseUtility.ClearFlags(obj, EObjectFlags.Standalone | EObjectFlags.Public);

            IntPtr defaultObject = Native_UClass.GetDefaultObject(obj, false);
            if (defaultObject != IntPtr.Zero)
            {
                // Check if rooted?
                Native_UObjectBaseUtility.RemoveFromRoot(defaultObject);
                Native_UObjectBaseUtility.ClearFlags(defaultObject, EObjectFlags.Standalone | EObjectFlags.Public);
            }

            FName oldClassRename = NativeReflection.MakeUniqueObjectName(NativeReflection.GetTransientPackage(),
                Native_UObjectBase.GetClass(obj), new FName("USharpHotReload_" + Native_UObjectBase.GetFName(obj)));
            using (FStringUnsafe oldClassRenameUnsafe = new FStringUnsafe(oldClassRename.ToString()))
            {
                Native_UObject.Rename(obj, ref oldClassRenameUnsafe.Array, NativeReflection.GetTransientPackage(), ERenameFlags.None);
            }

            Native_UObjectBaseUtility.SetFlags(obj, EObjectFlags.Transient);
            Native_UObjectBaseUtility.AddToRoot(obj);
        }

        public void Clear()
        {
            System.Diagnostics.Debug.Assert(OldClass == IntPtr.Zero, "This class is already set to hot-reload");
            MoveToTransientPackage(StaticClass);
            OldClass = StaticClass;
        }

        /// <summary>
        /// Create properties / functions and link the class
        /// </summary>
        public void Initialize()
        {
            Native_UStruct.Set_Children(StaticClass, IntPtr.Zero);

            BindingFlags propertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (PropertyInfo property in Type.GetProperties(propertyBindingFlags))
            {
                if (property.GetCustomAttribute<USharpPathAttribute>() != null)
                {
                    CreateProperty(StaticClass, property);
                }
            }

            Native_UField.Bind(StaticClass);
            Native_UStruct.StaticLink(StaticClass, true);
            firstRun = false;
        }

        private IntPtr CreateProperty(IntPtr outer, PropertyInfo propertyInfo)
        {
            // Note that HeaderParser.cpp and UObjectGlobals.cpp use "new" instead of NewObject for creating properties
            // KismetCompilerMisc.cpp uses NewObject
            // The "new" initialization sets the offset and adds the property to the owner which in the case of UStruct
            // does the following:
            // void UStruct::AddCppProperty(UProperty* Property) { Property->Next = Children; Children = Property; }

            USharpPathAttribute pathAttribute = propertyInfo.GetCustomAttribute<USharpPathAttribute>();
            if (pathAttribute == null)
            {
                return IntPtr.Zero;
            }

            string root, directory, moduleName, typeName, propertyName;
            FPackageName.GetPathInfo(pathAttribute.Path, out root, out directory, out moduleName, out typeName, out propertyName);
            if (string.IsNullOrEmpty(propertyName))
            {
                return IntPtr.Zero;
            }

            IntPtr property = CreateProperty(outer, propertyInfo.PropertyType, propertyName, pathAttribute.PropertyType,
                pathAttribute.InnerPropertyType1, pathAttribute.InnerPropertyType2);

            if (property == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            if (FBuild.WithMetaData)
            {
                IntPtr outermost = Native_UObjectBaseUtility.GetOutermost(property);
                IntPtr metadata = outermost == IntPtr.Zero ? IntPtr.Zero : Native_UPackage.GetMetaData(outermost);

                if (metadata != IntPtr.Zero)
                {
                    string categoryName = null;
                    //propertyInfo.GetCustomAttribute

                    if (string.IsNullOrEmpty(categoryName))
                    {
                        categoryName = "Default";
                    }

                    SetMetaData(metadata, property, "Category", categoryName);
                }
            }

            return property;
        }

        private IntPtr CreateProperty(IntPtr outer, Type type, string propertyName, EPropertyType propertyType)
        {
            return CreateProperty(outer, type, propertyName, propertyType, EPropertyType.Unknown, EPropertyType.Unknown);
        }

        private IntPtr CreateProperty(IntPtr outer, Type type, string propertyName, EPropertyType propertyType,
            EPropertyType innerPropertyType1, EPropertyType innerPropertyType2)
        {
            propertyType = ManagedUnrealTypes.GetPropertyType(type, propertyType);

            IntPtr propertyClass = ManagedUnrealTypes.GetPropertyClass(propertyType);
            if (propertyClass == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            EObjectFlags objectFlags = EObjectFlags.Public | EObjectFlags.Transient | EObjectFlags.MarkAsNative;
            IntPtr property = NativeReflection.NewObject(outer, propertyClass, new FName(propertyName), objectFlags);

            Native_UProperty.SetPropertyFlags(property, EPropertyFlags.BlueprintVisible | EPropertyFlags.BlueprintAssignable | EPropertyFlags.Edit);

            // Set type specific information
            switch (propertyType)
            {
                case EPropertyType.Array:
                    if (!firstRun)
                    {
                        Native_UArrayProperty.Set_Inner(property,
                            CreateProperty(property, typeof(int), propertyName, innerPropertyType1));
                    }
                    else
                    {
                        Native_UArrayProperty.Set_Inner(property,
                            CreateProperty(property, type.GenericTypeArguments[0], propertyName, innerPropertyType1));
                    }
                    break;

                case EPropertyType.Set:
                    Native_USetProperty.Set_ElementProp(property,
                        CreateProperty(property, type.GenericTypeArguments[0], propertyName, innerPropertyType1));
                    break;

                case EPropertyType.Map:
                    Native_UMapProperty.Set_KeyProp(property,
                        CreateProperty(property, type.GenericTypeArguments[0], propertyName, innerPropertyType1));
                    Native_UMapProperty.Set_ValueProp(property,
                        CreateProperty(property, type.GenericTypeArguments[1], propertyName, innerPropertyType2));
                    break;

                case EPropertyType.Class:
                    Native_UClassProperty.SetMetaClass(property, UClass.GetClass(type.GenericTypeArguments[0]).Address);
                    break;

                case EPropertyType.Object:
                    var v1 = ManagedUnrealTypes.GetStaticClass(type);
                    var v2 = ManagedUnrealTypes.GetStaticClass(typeof(UObject));
                    Native_UObjectPropertyBase.SetPropertyClass(property, v1 == IntPtr.Zero ? v2 : v1);
                    break;

                case EPropertyType.LazyObject:
                case EPropertyType.WeakObject:
                case EPropertyType.SoftObject:
                    Native_UObjectPropertyBase.SetPropertyClass(property, UClass.GetClass(type.GenericTypeArguments[0]).Address);
                    break;

                case EPropertyType.SoftClass:
                    Native_USoftClassProperty.SetMetaClass(property, UClass.GetClass(type.GenericTypeArguments[0]).Address);
                    break;

                case EPropertyType.Enum:
                    Native_UEnumProperty.SetEnum(property, ManagedUnrealTypes.GetEnum(type));
                    break;

                case EPropertyType.Delegate:
                    //Native_UDelegateProperty.Set_SignatureFunction(property, ManagedUnrealTypes.GetSignatureFunction(type));
                    break;

                case EPropertyType.MulticastDelegate:
                    //Native_UMulticastDelegateProperty.Set_SignatureFunction(property, ManagedUnrealTypes.GetSignatureFunction(type));
                    break;
            }

            Native_UField.AddCppProperty(outer, property);

            return property;
        }

        private void SetMetaData(IntPtr metadata, IntPtr obj, string key, string value)
        {
            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
            {
                Native_UMetaData.SetValue(metadata, obj, ref keyUnsafe.Array, ref valueUnsafe.Array);
            }
        }
    }

    [USharpPath("/Script/SomeModule.HelloWorld")]
    public class AHelloWorld : UnrealEngine.Engine.AActor
    {
        [USharpPath("/Script/SomeModule.HelloWorld:TestMe", EPropertyType.Int8)]
        public sbyte TestMe { get; set; }

        [USharpPath("/Script/SomeModule.HelloWorld:TestClass")]
        public TSubclassOf<UnrealEngine.Engine.AActor> TestClass { get; set; }

        [USharpPath("/Script/SomeModule.HelloWorld:MyArray")]
        public List<string> MyArray { get; set; }

        [USharpPath("/Script/SomeModule.HelloWorld:MyObj")]
        public BlueprintTest.Pong.ABPUtil_C MyObj { get; set; }

        [USharpPath("/Script/SomeModule.HelloWorld:MyS")]
        public AMyStruct S { get; set; }

        [USharpPath("/Script/SomeModule.HelloWorld:MyTestDispatcher")]
        public BlueprintTest.Pong.ABPUtil_C.FMyTestDispatcher MyTestDispatcher { get; set; }

        protected AHelloWorld(IntPtr address)
            : base(address)
        {
        }

        protected AHelloWorld(FObjectInitializer initializer)
            : base(initializer)
        {
        }

        [UFunction(EFunctionFlags.Net | EFunctionFlags.NetValidate)]
        public int TestFunc3(string some1, ref long some2, out string some3)
        {
            some3 = null;
            return 0;
        }

        public bool TestFunc3_Validate(string some1, ref long some2, out string some3)
        {
            some3 = null;
            return true;
        }

        [UFunction(EFunctionFlags.BlueprintEvent)]
        public int TestFunc(out string some1, ref long some2, List<int> incorrect, out BlueprintTest.Pong.ABPUtil_C.FMyTestDispatcher despatch)
        {
            some1 = null;
            despatch = null;
            return 0;
        }

        public bool TestFunc_Validate(out string some1, ref long some2, List<int> incorrect, out BlueprintTest.Pong.ABPUtil_C.FMyTestDispatcher despatch)
        {
            some1 = null;
            despatch = null;
            return false;
        }

        public BlueprintTest.Pong.ABPUtil_C.FMyTestDispatcher TestFunc2(string some1, long some2, List<int> incorrect)
        {
            return null;
        }

        static AHelloWorld()
        {
            /*IntPtr @class = NativeReflection.GetClass("/Script/UnrealEngine_Runtime/HelloWorld");
            AHelloWorld.TestFunc_FunctionAddress = NativeReflection.GetFunction(@class, "TestFunc");
            AHelloWorld.TestFunc_ParamsSize = NativeReflection.GetFunctionParamsSize(AHelloWorld.TestFunc_FunctionAddress);
            AHelloWorld.TestFunc_some1_Offset = NativeReflection.GetPropertyOffset(AHelloWorld.TestFunc_FunctionAddress, "some1");
            AHelloWorld.TestFunc_some2_Offset = NativeReflection.GetPropertyOffset(AHelloWorld.TestFunc_FunctionAddress, "some2");
            AHelloWorld.TestFunc_incorrect_PropertyAddress = NativeReflection.GetProperty(AHelloWorld.TestFunc_FunctionAddress, "incorrect");
            AHelloWorld.TestFunc_incorrect_Offset = NativeReflection.GetPropertyOffset(AHelloWorld.TestFunc_FunctionAddress, "incorrect");
            AHelloWorld.TestFunc_despatch_Offset = NativeReflection.GetPropertyOffset(AHelloWorld.TestFunc_FunctionAddress, "despatch");
            AHelloWorld.TestFunc___return_Offset = NativeReflection.GetPropertyOffset(AHelloWorld.TestFunc_FunctionAddress, "__return");*/
        }

        /*// UnrealEngine.Runtime.AHelloWorld
        public unsafe int TestFunc3(out string some1, ref long some2, List<int> incorrect, out BlueprintTest.Pong.ABPUtil_C.FMyTestDispatcher despatch)
        {
            if (this.TestFunc_FunctionAddressInstance == IntPtr.Zero)
            {
                this.TestFunc_FunctionAddressInstance = NativeReflection.GetFunctionFromInstance(base.Address, "TestFunc");
            }
            byte* value = stackalloc byte[AHelloWorld.TestFunc_ParamsSize];
            IntPtr intPtr = new IntPtr((void*)value);
            BlittableTypeMarshaler<long>.ToNative(IntPtr.Add(intPtr, AHelloWorld.TestFunc_some2_Offset), 0, null, some2);
            TArrayCopyMarshaler<int> tArrayCopyMarshaler = new TArrayCopyMarshaler<int>(1, AHelloWorld.TestFunc_incorrect_PropertyAddress, new MarshalingDelegates<int>.FromNative(BlittableTypeMarshaler<int>.FromNative), new MarshalingDelegates<int>.ToNative(BlittableTypeMarshaler<int>.ToNative));
            tArrayCopyMarshaler.ToNative(IntPtr.Add(intPtr, AHelloWorld.TestFunc_incorrect_Offset), 0, null, incorrect);
            NativeReflection.InvokeFunction(base.Address, this.TestFunc_FunctionAddressInstance, intPtr, AHelloWorld.TestFunc_ParamsSize);
            int result = BlittableTypeMarshaler<int>.FromNative(IntPtr.Add(intPtr, AHelloWorld.TestFunc___return_Offset), 0, null);
            some1 = FStringMarshaler.FromNative(IntPtr.Add(intPtr, AHelloWorld.TestFunc_some1_Offset), 0, null);
            some2 = BlittableTypeMarshaler<long>.FromNative(IntPtr.Add(intPtr, AHelloWorld.TestFunc_some2_Offset), 0, null);
            incorrect = tArrayCopyMarshaler.FromNative(IntPtr.Add(intPtr, AHelloWorld.TestFunc_incorrect_Offset), 0, null);
            despatch = FMulticastDelegateMarshaler<BlueprintTest.Pong.ABPUtil_C.FMyTestDispatcher>.FromNative(IntPtr.Add(intPtr, AHelloWorld.TestFunc_despatch_Offset), 0, null);
            NativeReflection.InvokeFunction_DestroyAll(this.TestFunc_FunctionAddressInstance, intPtr);
            return result;
        }

        // UnrealEngine.Runtime.AHelloWorld
        private static readonly int TestFunc___return_Offset;

        // UnrealEngine.Runtime.AHelloWorld
        private static readonly int TestFunc_despatch_Offset;

        // UnrealEngine.Runtime.AHelloWorld
        private static IntPtr TestFunc_FunctionAddress;

        // UnrealEngine.Runtime.AHelloWorld
        private IntPtr TestFunc_FunctionAddressInstance;

        // UnrealEngine.Runtime.AHelloWorld
        private static readonly int TestFunc_incorrect_Offset;

        // UnrealEngine.Runtime.AHelloWorld
        private static readonly IntPtr TestFunc_incorrect_PropertyAddress;

        // UnrealEngine.Runtime.AHelloWorld
        private static readonly int TestFunc_ParamsSize;

        // UnrealEngine.Runtime.AHelloWorld
        private static readonly int TestFunc_some1_Offset;

        // UnrealEngine.Runtime.AHelloWorld
        private static readonly int TestFunc_some2_Offset;*/

        /*private static IntPtr TestFunc_FunctionAddress;
        private static readonly int TestFunc_ParamsSize;
        private static readonly int TestFunc___return_Offset;
        private static readonly int TestFunc_some1_Offset;
        private static readonly int TestFunc_some2_Offset;

        private static void TestFunc__Invoker(IntPtr buffer, IntPtr objPtr)
        {
            AHelloWorld aHelloWorld = GCHelper.Find<AHelloWorld>(objPtr);
            aHelloWorld.CheckDestroyed();
            string some = FStringMarshaler.FromNative(IntPtr.Add(buffer, AHelloWorld.TestFunc_some1_Offset), 0, null);
            long some2 = BlittableTypeMarshaler<long>.FromNative(IntPtr.Add(buffer, AHelloWorld.TestFunc_some2_Offset), 0, null);
            int value = aHelloWorld.TestFunc(some, some2);
            BlittableTypeMarshaler<int>.ToNative(IntPtr.Add(buffer, AHelloWorld.TestFunc___return_Offset), 0, null, value);
        }*/
    }

    public class FMyCustomDispatcher : FMulticastDelegate<FMyCustomDispatcher.Signature>
    {
        public delegate int Signature(int param1, out string param2);
    }

    [UStruct]
    public struct AMyStruct
    {
        public List<int> MyList1;

        public List<string> MyList2;

        public string MyStr;

        public int MyInt;
    }

    public class USharpPathAttribute : Attribute
    {
        public string Path { get; set; }

        /// <summary>
        /// Optional property type to avoid runtime lookup of the property type
        /// </summary>
        public EPropertyType PropertyType { get; set; }

        public EPropertyType InnerPropertyType1 { get; set; }
        public EPropertyType InnerPropertyType2 { get; set; }

        public USharpPathAttribute(string path)
        {
            Path = path;
        }

        public USharpPathAttribute(string path, EPropertyType propertyType)
        {
            Path = path;
            PropertyType = propertyType;
        }
    }
}
