using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Functions for accessing the reflection system without creating managed objects
    /// (This isn't a complete API for the reflection system, just a sub-set to be used by generated code)
    /// </summary>
    public static partial class NativeReflection
    {
        /// <summary>
        /// Gets the UClass address from the given path
        /// </summary>
        /// <param name="path">The path of the class</param>
        /// <returns>The UClass address</returns>
        public static IntPtr GetClass(string path)
        {
            return GetStructure(Classes.UClass, path);
        }

        /// <summary>
        /// Gets the UScriptStruct address from the given path
        /// </summary>
        /// <param name="path">The path of the struct</param>
        /// <returns>The UScriptStruct address</returns>
        public static IntPtr GetStruct(string path)
        {
            return GetStructure(Classes.UScriptStruct, path);
        }

        /// <summary>
        /// Gets the UStruct address from the given path (use GetStruct() if you want an actual struct)
        /// </summary>
        /// <param name="path">The path of the structure</param>
        /// <returns>The UStruct address</returns>
        public static IntPtr GetStructure(string path)
        {
            return GetStructure(Classes.UStruct, path);
        }

        /// <summary>
        /// Gets the UFunction address from the given path
        /// </summary>
        /// <param name="path">The path of the function</param>
        /// <returns>The UFunction address</returns>
        public static IntPtr GetFunction(string path)
        {
            return GetStructure(Classes.UFunction, path);
        }

        /// <summary>
        /// Returns the address of the UStruct for the given type of UStruct at the given path
        /// </summary>
        /// <param name="structureClass">The UClass address defining the type of this structure (UClass, UFunction, UScriptStruct, etc)</param>
        /// <param name="path">The path of the structure</param>
        /// <param name="load">If true attempt to load the structure if it failed to find the existing structure</param>
        /// <returns></returns>
        public static IntPtr GetStructure(IntPtr structureClass, string path, bool load = true)
        {
            if (structureClass == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            IntPtr foundClass = FindObject(structureClass, IntPtr.Zero, path);//AnyPackage, path);

            if (foundClass == IntPtr.Zero)
            {
                // Look for class redirectors
                FName newPath = FLinkerLoad.FindNewNameForClass(new FName(path), false);

                if (newPath != FName.None)
                {
                    foundClass = FindObject(structureClass, IntPtr.Zero, path);//AnyPackage, path);
                }
            }

            if (foundClass == IntPtr.Zero)
            {
                // Fallback if this class isn't loaded yet. TODO: Check if this is the correct method to call.
                foundClass = LoadObject(structureClass, IntPtr.Zero, path);
            }

            return foundClass;
        }

        /// <summary>
        /// Returns the size of the structure (use GetStructSize() for getting the size of an actual struct)
        /// </summary>
        public static int GetStructureSize(IntPtr unrealStruct)
        {
            if (unrealStruct == IntPtr.Zero)
            {
                return 0;
            }
            return Native_UStruct.GetStructureSize(unrealStruct);
        }

        /// <summary>
        /// Returns the size of the struct using ICppStructOps, otherwise falls back to GetStructureSize().
        /// </summary>
        public static int GetStructSize(IntPtr unrealStruct)
        {
            if (unrealStruct == IntPtr.Zero)
            {
                return 0;
            }
            if (Native_UObjectBaseUtility.IsA(unrealStruct, Classes.UScriptStruct))
            {
                IntPtr cppStructOps = Native_UScriptStruct.GetCppStructOps(unrealStruct);
                if (cppStructOps != IntPtr.Zero)
                {
                    return Native_ICppStructOps.GetSize(cppStructOps);
                }
            }
            return GetStructureSize(unrealStruct);
        }

        public static IntPtr GetFunction(IntPtr unrealClass, string functionName)
        {
            return FindField(Classes.UFunction, unrealClass, functionName);
        }

        public static IntPtr GetFunctionFromInstance(IntPtr obj, string functionName)
        {
            if (obj == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            FName functionFName = new FName(functionName);
            return Native_UObject.FindFunctionChecked(obj, ref functionFName);
        }

        public static int GetFunctionParamsSize(IntPtr function)
        {
            if (function == IntPtr.Zero)
            {
                return 0;
            }
            return Native_UFunction.Get_ParmsSize(function);
        }

        public static void InvokeStaticFunction(IntPtr obj, IntPtr function, IntPtr args, int argsSize)
        {
            // obj will always be IntPtr.Zero but requiring it for now so InvokeFunction / InvokeStaticFunction have the same signature

            Debug.Assert(argsSize == Native_UFunction.Get_ParmsSize(function), "Function was passed with bad args");

            // Is the above comment valid anymore? Should we still just get the class manually?
            IntPtr unrealClass = Native_UField.GetOwnerClass(function);
            Debug.Assert(unrealClass != IntPtr.Zero && unrealClass == obj);

            InvokeFunction(unrealClass, function, args, argsSize);
        }

        public static void InvokeFunction(IntPtr obj, IntPtr function, IntPtr args, int argsSize)
        {
            Debug.Assert(argsSize == Native_UFunction.Get_ParmsSize(function), "Function was passed with bad args");

            if (obj == IntPtr.Zero)
            {
                throw new Exception("Trying to call function " + GetPathName(function) + " on destroyed unreal object");
            }

            Native_UObject.ProcessEvent(obj, function, args);
        }

        public static void InvokeFunction_InitAll(IntPtr function, IntPtr args)
        {
            foreach (IntPtr paramProperty in new NativeFieldIterator(Classes.UProperty, function))
            {
                Native_UProperty.InitializeValue_InContainer(paramProperty, args);
            }
        }

        public static void InvokeFunction_DestroyAll(IntPtr function, IntPtr args)
        {
            foreach (IntPtr paramProperty in new NativeFieldIterator(Classes.UProperty, function))
            {
                Native_UProperty.DestroyValue_InContainer(paramProperty, args);
            }
        }

        public static void InitializeValue_InContainer(IntPtr property, IntPtr container)
        {
            Native_UProperty.InitializeValue_InContainer(property, container);
        }

        public static void DestroyValue_InContainer(IntPtr property, IntPtr container)
        {
            Native_UProperty.DestroyValue_InContainer(property, container);
        }

        public static string GetPathName(IntPtr obj)
        {
            if (obj == IntPtr.Zero)
            {
                return null;
            }
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UObjectBaseUtility.GetPathName(obj, IntPtr.Zero, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public static string GetName(IntPtr obj)
        {
            if (obj == IntPtr.Zero)
            {
                return string.Empty;
            }
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UObjectBaseUtility.GetNameOut(obj, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public static FName GetFName(IntPtr obj)
        {
            if (obj == IntPtr.Zero)
            {
                return FName.None;
            }
            FName result;
            Native_UObjectBase.GetFName(obj, out result);
            return result;
        }

        public static IntPtr GetProperty(IntPtr unrealStruct, string propertyName)
        {
            if (unrealStruct == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            return FindField(unrealStruct, propertyName);
        }

        public static bool GetPropertyRef(ref UFieldAddress property, IntPtr unrealStruct, string propertyName)
        {
            if (property == null)
            {
                property = new UFieldAddress();
            }
            return property.Update(GetProperty(unrealStruct, propertyName));
        }

        public static bool HasProperty(IntPtr unrealStruct, string propertyName)
        {
            if (unrealStruct == IntPtr.Zero)
            {
                return false;
            }
            return GetProperty(unrealStruct, propertyName) != IntPtr.Zero;
        }

        public static int GetPropertyOffset(IntPtr unrealStruct, string propertyName)
        {
            if (unrealStruct == IntPtr.Zero)
            {
                return 0;
            }

            IntPtr property = FindField(unrealStruct, propertyName);
            if (property == IntPtr.Zero)
            {
                return 0;
            }

            unsafe
            {
                int dummyContainer = 0;
                IntPtr dummyPointer = (IntPtr)(&dummyContainer);
                IntPtr offsetPointer = Native_UProperty.ContainerVoidPtrToValuePtr(property, dummyPointer, 0);
                return (int)(offsetPointer.ToInt64() - dummyPointer.ToInt64());
            }
        }

        public static ushort GetPropertyRepIndex(IntPtr unrealStruct, string propertyName)
        {
            IntPtr property = FindField(unrealStruct, propertyName);
            return property == IntPtr.Zero ? (ushort)0 : Native_UProperty.Get_RepIndex(property);
        }

        public static int GetPropertyArrayElementSize(IntPtr unrealStruct, string propertyName)
        {
            if (unrealStruct == IntPtr.Zero)
            {
                return 0;
            }
            IntPtr arrayProperty = FindField(Classes.UArrayProperty, unrealStruct, propertyName);
            if (arrayProperty == IntPtr.Zero)
            {
                return 0;
            }
            IntPtr innerProperty = Native_UArrayProperty.Get_Inner(arrayProperty);
            return Native_UProperty.GetSize(innerProperty);
        }

        public static int GetPropertyArrayDim(IntPtr unrealStruct, string propertyName)
        {
            IntPtr property = FindField(unrealStruct, propertyName);
            return property == IntPtr.Zero ? -1 : Native_UProperty.Get_ArrayDim(property);
        }
        
        public static List<string> GetPropertyNames(IntPtr unrealStruct)
        {
            List<string> result = new List<string>();
            if (unrealStruct != IntPtr.Zero)
            {
                foreach (IntPtr field in new NativeFieldIterator(Classes.UProperty, unrealStruct))
                {
                    result.Add(NativeReflection.GetFName(field).PlainName);
                }
            }
            return result;
        }

        public static Dictionary<string, EPropertyType> GetPropertyTypes(IntPtr unrealStruct)
        {
            Dictionary<string, EPropertyType> result = new Dictionary<string, EPropertyType>();
            if (unrealStruct != IntPtr.Zero)
            {
                foreach (IntPtr field in new NativeFieldIterator(Classes.UProperty, unrealStruct))
                {
                    string propertyName = NativeReflection.GetFName(field).PlainName;
                    EPropertyType propertyType = GetPropertyType(field);
                    result[propertyName] = propertyType;
                }
            }
            return result;
        }

        public static IntPtr FindField(IntPtr unrealStruct, FName fieldName)
        {
            return FindField(Classes.UProperty, unrealStruct, fieldName);
        }

        public static IntPtr FindField(IntPtr unrealStruct, string fieldName)
        {
            return FindField(unrealStruct, new FName(fieldName));
        }

        public static IntPtr FindField(IntPtr typeClass, IntPtr unrealStruct, FName fieldName)
        {
            if (typeClass == IntPtr.Zero || unrealStruct == IntPtr.Zero || fieldName == FName.None)
            {
                return IntPtr.Zero;
            }
            
            foreach (IntPtr field in new NativeFieldIterator(typeClass, unrealStruct))
            {
                if (NativeReflection.GetFName(field) == fieldName)
                {
                    return field;
                }
            }
            return IntPtr.Zero;
        }

        public static IntPtr FindField(IntPtr typeClass, IntPtr unrealStruct, string fieldName)
        {
            return FindField(typeClass, unrealStruct, new FName(fieldName));
        }

        /// <summary>
        /// Validates that the native size of a blittable struct matches the managed Type size
        /// </summary>
        /// <param name="unrealStruct">The address of the UScriptStruct</param>
        /// <param name="type">The type of the struct</param>
        public static void ValidateBlittableStructSize(IntPtr unrealStruct, Type type)
        {
            int nativeSize = GetStructSize(unrealStruct);
            FMessage.Assert(nativeSize == System.Runtime.InteropServices.Marshal.SizeOf(type),
                "Blittable struct size mismatch on '" + GetPathName(unrealStruct) + "'");
        }

        /// <summary>
        /// Validates that the given property exists and matches the given UProperty class (e.g. UBoolProperty::StaticClass())
        /// </summary>
        /// <param name="unrealStruct">The address of the structure which owns the property</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyClass">The expected UProperty class of the property</param>
        /// <returns></returns>
        public static bool ValidatePropertyClass(IntPtr unrealStruct, string propertyName, IntPtr propertyClass)
        {
            IntPtr field = FindField(unrealStruct, propertyName);
            if (field == IntPtr.Zero || !Native_UObjectBaseUtility.IsA(field, Classes.UProperty))
            {
                return false;
            }
            IntPtr actualClass = Native_UObjectBase.GetClass(field);
            if (actualClass == propertyClass)
            {
                return true;
            }
            if (actualClass != IntPtr.Zero && propertyClass == Classes.UEnumProperty &&
                Native_UStruct.IsChildOf(actualClass, Classes.UNumericProperty))
            {
                return Native_UNumericProperty.IsEnum(field);
            }
            return false;
        }

        /// <summary>
        /// Logs the validity of a function when loading underlying native type info
        /// (an invalid function is a function that either doesn't exist or doesn't match the native function signature)
        /// </summary>
        /// <param name="functionPath">The path of the function</param>
        /// <param name="isValid">Is the function valid</param>
        public static void LogFunctionIsValid(string functionPath, bool isValid)
        {
            if (!isValid)
            {
                FMessage.Log(ELogVerbosity.Warning, "Function is invalid '" + functionPath + "'");
            }
        }

        /// <summary>
        /// Logs the validity of a struct when loading underlying native type info
        /// (an invalid struct is a struct that has missing/unmatched properties)
        /// </summary>
        /// <param name="structPath">The path of the struct</param>
        /// <param name="isValid">Is the struct valid</param>
        public static void LogStructIsValid(string structPath, bool isValid)
        {
            if (!isValid)
            {
                FMessage.Log(ELogVerbosity.Warning, "Struct is invalid '" + structPath + "'");
            }
        }

        /// <summary>
        /// Logs information about an invalid property when accessed from the getter/setter
        /// (an invalid property is a property that either doesn't exist or doesn't match the native property)
        /// </summary>
        /// <param name="propertyPath">The path of the property</param>
        public static void LogInvalidPropertyAccessed(string propertyPath)
        {
            FMessage.Log(ELogVerbosity.Warning, "Invalid property accessed '" + propertyPath + "'");
        }

        /// <summary>
        /// Logs information about an invalid function when called
        /// (an invalid function is a function that either doesn't exist or doesn't match the native function signature)
        /// </summary>
        /// <param name="functionPath"></param>
        public static void LogInvalidFunctionAccessed(string functionPath)
        {
            FMessage.Log(ELogVerbosity.Warning, "Invalid function accessed '" + functionPath + "'");
        }

        /// <summary>
        /// Logs information about an invalid struct when marshaled
        /// (an invalid struct is a struct that has missing/unmatched properties)
        /// </summary>
        /// <param name="structPath"></param>
        public static void LogInvalidStructAccessed(string structPath)
        {
            FMessage.Log(ELogVerbosity.Warning, "Invalid struct accessed '" + structPath + "'");
        }
    }

    // UObjectBaseUtility functions which with IntPtr rather than UObject instances
    public partial class NativeReflection
    {
        public static bool IsA(IntPtr obj, IntPtr unrealClass)
        {
            return Native_UObjectBaseUtility.IsA(obj, unrealClass);
        }
    }

    // UObjectGlobals functions which work with IntPtr rather than UObject instances
    public partial class NativeReflection
    {
        public static readonly IntPtr AnyPackage = new IntPtr(-1);

        public static FName MakeUniqueObjectName(ObjectOuter outer, IntPtr unrealClass, FName baseName = default(FName))
        {
            FName result;
            Native_UObjectGlobals.MakeUniqueObjectName(outer.Address, unrealClass, ref baseName, out result);
            return result;
        }

        public static FName MakeUniqueObjectName(IntPtr outer, IntPtr unrealClass, FName baseName = default(FName))
        {
            FName result;
            Native_UObjectGlobals.MakeUniqueObjectName(outer, unrealClass, ref baseName, out result);
            return result;
        }

        public static bool IsReferenced(IntPtr res, EObjectFlags keepFlags, EInternalObjectFlags internalKeepFlags, bool checkSubObjects = false)
        {
            return Native_UObjectGlobals.IsReferenced(res, keepFlags, internalKeepFlags, checkSubObjects, IntPtr.Zero);
        }

        public static IntPtr GetTransientPackage()
        {
            return Native_UObjectGlobals.GetTransientPackage();
        }

        public static IntPtr NewObject(
            IntPtr outer,
            IntPtr unrealClass,
            FName name = default(FName),
            EObjectFlags flags = EObjectFlags.NoFlags,
            IntPtr template = default(IntPtr),
            bool copyTransientsFromClassDefaults = false,
            IntPtr instanceGraph = default(IntPtr))
        {
            if (unrealClass == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            if (outer == IntPtr.Zero)
            {
                outer = GetTransientPackage();
            }

            if (name == FName.None)
            {
                FObjectInitializer.AssertIfInConstructor(outer);
            }

            return Native_UObjectGlobals.StaticConstructObject_Internal(
                unrealClass, outer, ref name, flags, EInternalObjectFlags.None, template, copyTransientsFromClassDefaults, instanceGraph);
        }

        public static IntPtr DuplicateObject(
            IntPtr sourceObject,
            IntPtr outer,
            FName name = default(FName),
            EObjectFlags flagMask = EObjectFlags.AllFlags,
            IntPtr destClass = default(IntPtr),
            EDuplicateMode duplicateMode = EDuplicateMode.Normal,
            EInternalObjectFlags internalFlagsMask = EInternalObjectFlags.AllFlags)
        {
            if (sourceObject == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            if (outer == IntPtr.Zero)
            {
                outer = GetTransientPackage();
            }
            return Native_UObjectGlobals.StaticDuplicateObject(
                sourceObject, outer, ref name, flagMask, destClass, duplicateMode, internalFlagsMask);
        }

        public static IntPtr FindObjectFast(
            IntPtr unrealClass,
            IntPtr outer,
            FName name,
            bool exactClass = false,
            bool anyPackage = false,
            EObjectFlags exclusiveFlags = EObjectFlags.NoFlags,
            EInternalObjectFlags exclusiveInternalFlags = EInternalObjectFlags.None)
        {
            return Native_UObjectGlobals.StaticFindObjectFast(
                unrealClass, outer, ref name, exactClass, anyPackage, exclusiveFlags, exclusiveInternalFlags);
        }

        public static IntPtr FindObject(IntPtr unrealClass, IntPtr outer, string name, bool exactClass = false)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                return Native_UObjectGlobals.StaticFindObject(unrealClass, outer, ref nameUnsafe.Array, exactClass);
            }
        }

        public static IntPtr FindObjectChecked(IntPtr unrealClass, IntPtr outer, string name, bool exactClass = false)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                return Native_UObjectGlobals.StaticFindObjectChecked(
                    unrealClass, outer, ref nameUnsafe.Array, exactClass);
            }
        }

        public static IntPtr FindObjectSafe(IntPtr unrealClass, IntPtr outer, string name, bool exactClass = false)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                return Native_UObjectGlobals.StaticFindObjectSafe(
                    unrealClass, outer, ref nameUnsafe.Array, exactClass);
            }
        }

        public static IntPtr LoadObject(IntPtr unrealClass, IntPtr outer, string name, string filename = null, ELoadFlags loadFlags = ELoadFlags.None)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            using (FStringUnsafe filenameUnsafe = new FStringUnsafe(filename))
            {
                return Native_UObjectGlobals.StaticLoadObject(
                    unrealClass, outer, ref nameUnsafe.Array, ref filenameUnsafe.Array, loadFlags, IntPtr.Zero, true);
            }
        }

        public static IntPtr LoadClass(IntPtr baseClass, IntPtr outer, string name, string filename, ELoadFlags loadFlags = ELoadFlags.None)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            using (FStringUnsafe filenameUnsafe = new FStringUnsafe(filename))
            {
                return Native_UObjectGlobals.StaticLoadClass(
                    baseClass, outer, ref nameUnsafe.Array, ref filenameUnsafe.Array, loadFlags, IntPtr.Zero);
            }
        }

        public static IntPtr GetDefault(IntPtr unrealClass, bool createIfNeeded = true)
        {
            return Native_UClass.GetDefaultObject(unrealClass, createIfNeeded);
        }

        public static IntPtr LoadPackage(IntPtr outer, string longPackageName, ELoadFlags loadFlags)
        {
            using (FStringUnsafe longPackageNameUnsafe = new FStringUnsafe(longPackageName))
            {
                return Native_UObjectGlobals.LoadPackage(outer, ref longPackageNameUnsafe.Array, loadFlags);
            }
        }

        public static IntPtr FindPackage(IntPtr outer, string packageName)
        {
            using (FStringUnsafe packageNameUnsafe = new FStringUnsafe(packageName))
            {
                return Native_UObjectGlobals.FindPackage(outer, ref packageNameUnsafe.Array);
            }
        }

        public static IntPtr CreatePackage(IntPtr outer, string packageName)
        {
            using (FStringUnsafe packageNameUnsafe = new FStringUnsafe(packageName))
            {
                return Native_UObjectGlobals.CreatePackage(outer, ref packageNameUnsafe.Array);
            }
        }

        public static IntPtr StaticAllocateObject(IntPtr unrealClass, IntPtr outer, FName name, out bool outReusedSubobject,
            EObjectFlags setFlags, EInternalObjectFlags internalSetFlags, bool canReuseSubobjects = false)
        {
            csbool outReusedSubobjectTemp;
            IntPtr result = Native_UObjectGlobals.StaticAllocateObject(
                unrealClass, outer, ref name, setFlags, internalSetFlags, canReuseSubobjects, out outReusedSubobjectTemp);
            outReusedSubobject = outReusedSubobjectTemp;
            return result;
        }
    }

    // UObjectHash functions which work with IntPtr rather than UObject instances
    public partial class NativeReflection
    {
        public static IntPtr[] GetObjectsWithOuter(IntPtr outer, bool includeNestedObjects = true, EObjectFlags exclusionFlags = EObjectFlags.NoFlags, EInternalObjectFlags exclusionInternalFlags = EInternalObjectFlags.None)
        {
            using (TArrayUnsafe<IntPtr> result = new TArrayUnsafe<IntPtr>())
            {
                Native_UObjectHash.GetObjectsWithOuter(outer, result.Address, includeNestedObjects, exclusionFlags, exclusionInternalFlags);
                return result.ToArray();
            }
        }
    }

    public partial class NativeReflection
    {
        // IntPtr copy of TFieldIterator
        public struct NativeFieldIterator : IEnumerable<IntPtr>
        {
            /// <summary>
            /// The T UClass
            /// </summary>
            private IntPtr fieldTypeClass;

            /// <summary>
            /// The T UClass cast flags
            /// </summary>
            private EClassCastFlags fieldTypeClassCastFlags;

            private bool allFieldTypeClassCastFlags;

            /// <summary>
            /// The target struct which is held onto for .Reset() which resets unrealStruct to targetStruct
            /// </summary>
            private IntPtr targetStruct;

            /// <summary>
            /// Whether to include the super class or not
            /// </summary>
            private bool includeSuper;

            /// <summary>
            /// Whether to include deprecated fields or not
            /// </summary>
            private bool includeDeprecated;

            /// <summary>
            /// Whether to include interface fields or not
            /// </summary>
            private bool includeInterface;

            private EFieldIteratorType iteratorType;            

            public NativeFieldIterator(IntPtr fieldTypeClass,
                IntPtr unrealStruct,
                bool includeSuper = true,
                bool includeDeprecated = true,
                bool includeInterface = false)
                : this(fieldTypeClass, unrealStruct, EFieldIteratorType.Children, includeSuper, includeDeprecated, includeInterface)
            {
            }

            public NativeFieldIterator(IntPtr fieldTypeClass,
                IntPtr unrealStruct,
                EFieldIteratorType iteratorType,
                bool includeSuper = true,
                bool includeDeprecated = true,
                bool includeInterface = false)
            {
                this.fieldTypeClass = fieldTypeClass;
                fieldTypeClassCastFlags = Native_UClass.Get_ClassCastFlags(fieldTypeClass);
                allFieldTypeClassCastFlags = true;
                targetStruct = unrealStruct;
                this.iteratorType = iteratorType;
                this.includeSuper = includeSuper;
                this.includeDeprecated = includeDeprecated;
                this.includeInterface = includeInterface;
            }

            public NativeFieldIterator(EClassCastFlags fieldTypeClassCastFlags,
                IntPtr unrealStruct,
                bool allFieldTypeClassCastFlags = true,
                bool includeSuper = true,
                bool includeDeprecated = true,
                bool includeInterface = false)
                : this(fieldTypeClassCastFlags, unrealStruct, EFieldIteratorType.Children, allFieldTypeClassCastFlags, includeSuper, includeDeprecated, includeInterface)
            {
            }

            public NativeFieldIterator(EClassCastFlags fieldTypeClassCastFlags,                
                IntPtr unrealStruct,
                EFieldIteratorType iteratorType,
                bool allFieldTypeClassCastFlags = true,
                bool includeSuper = true,
                bool includeDeprecated = true,
                bool includeInterface = false)
            {
                this.fieldTypeClass = IntPtr.Zero;
                this.fieldTypeClassCastFlags = fieldTypeClassCastFlags;
                this.allFieldTypeClassCastFlags = allFieldTypeClassCastFlags;
                targetStruct = unrealStruct;
                this.iteratorType = iteratorType;
                this.includeSuper = includeSuper;
                this.includeDeprecated = includeDeprecated;
                this.includeInterface = includeInterface;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(ref this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator<IntPtr> IEnumerable<IntPtr>.GetEnumerator()
            {
                return GetEnumerator();
            }

            public struct Enumerator : IEnumerator<IntPtr>
            {
                private NativeFieldIterator iterator;

                /// <summary>
                /// The object being searched for the specified field
                /// </summary>
                private IntPtr unrealStruct;

                /// <summary>
                /// The current location in the list of fields being iterated
                /// </summary>
                private IntPtr field;

                /// <summary>
                /// The index of the current interface being iterated
                /// </summary>
                private int interfaceIndex;

                private bool first;

                public Enumerator(ref NativeFieldIterator iterator)
                {
                    this.iterator = iterator;
                    unrealStruct = iterator.targetStruct;
                    field = IntPtr.Zero;
                    interfaceIndex = -1;
                    first = true;
                    field = GetField(unrealStruct);
                }

                private IntPtr GetField(IntPtr unrealStruct)
                {
                    if (unrealStruct == IntPtr.Zero)
                    {
                        return IntPtr.Zero;
                    }

                    switch (iterator.iteratorType)
                    {
                        case EFieldIteratorType.Children:
                            return Native_UStruct.Get_Children(unrealStruct);

                        case EFieldIteratorType.Property:
                            return Native_UStruct.Get_PropertyLink(unrealStruct);

                        case EFieldIteratorType.Ref:
                            return Native_UStruct.Get_RefLink(unrealStruct);

                        case EFieldIteratorType.Destructor:
                            return Native_UStruct.Get_DestructorLink(unrealStruct);

                        case EFieldIteratorType.PostConstruct:
                            return Native_UStruct.Get_PostConstructLink(unrealStruct);
                    }
                    return IntPtr.Zero;
                }

                public IntPtr Current
                {
                    get { return field; }
                }

                object IEnumerator.Current
                {
                    get { return Current; }
                }

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        field = Native_UField.Get_Next(field);
                    }

                    IntPtr currentField = field;
                    IntPtr currentStruct = unrealStruct;

                    while (currentStruct != IntPtr.Zero)
                    {
                        while (currentField != IntPtr.Zero)
                        {
                            IntPtr fieldClass = Native_UObjectBase.GetClass(currentField);

                            bool hasClassCastFlags = iterator.allFieldTypeClassCastFlags ? 
                                Native_UClass.HasAllCastFlags(fieldClass, iterator.fieldTypeClassCastFlags) :
                                Native_UClass.HasAnyCastFlag(fieldClass, iterator.fieldTypeClassCastFlags);
                            bool hasUPropertyFlag = Native_UClass.HasAllCastFlags(fieldClass, EClassCastFlags.UProperty);
                            bool hasDeprecatedFlag = Native_UProperty.HasAllPropertyFlags(currentField, EPropertyFlags.Deprecated);

                            if (hasClassCastFlags && (iterator.includeDeprecated || !hasUPropertyFlag || !hasDeprecatedFlag))
                            {
                                unrealStruct = currentStruct;
                                field = currentField;
                                return true;
                            }

                            currentField = Native_UField.Get_Next(currentField);
                        }

                        if (iterator.includeInterface)
                        {
                            // We shouldn't be able to get here for non-classes
                            IntPtr currentClass = currentStruct;
                            ++interfaceIndex;
                            using (TArrayUnsafe<FImplementedInterface> interfaces = new TArrayUnsafe<FImplementedInterface>())
                            {
                                if (interfaceIndex < interfaces.Count)
                                {
                                    IntPtr interfaceClass = interfaces[interfaceIndex].InterfaceClassAddress;
                                    currentClass = GetField(interfaceClass);
                                    continue;
                                }
                            }
                        }

                        if (iterator.includeSuper)
                        {
                            currentStruct = Native_UStruct.GetInheritanceSuper(currentStruct);
                            if (currentStruct != IntPtr.Zero)
                            {
                                currentField = GetField(currentStruct);
                                interfaceIndex = -1;
                                continue;
                            }
                        }

                        break;
                    }

                    unrealStruct = currentStruct;
                    field = currentField;

                    return field != IntPtr.Zero;
                }

                public void Reset()
                {
                    unrealStruct = iterator.targetStruct;
                    field = GetField(unrealStruct);
                    interfaceIndex = -1;
                    first = true;
                }
            }
        }
    }

    public partial class NativeReflection
    {
        // IntPtr copy of TObjectIterator

        /// <summary>
        /// Class for iterating through all objects which inherit from a
        /// specified base class.  Does not include any class default objects.
        /// Note that when Playing In Editor, this will find objects in the
        /// editor as well as the PIE world, in an indeterminate order.
        /// </summary>
        public struct NativeObjectIterator : IEnumerator<IntPtr>
        {
            /// <summary>
            /// Results from the GetObjectsOfClass query
            /// </summary>
            private IntPtr[] objectArray;

            /// <summary>
            /// index of the current element in the object array
            /// </summary>
            private int index;

            public NativeObjectIterator(IntPtr unrealClass,
                EObjectFlags additionalExclusionFlags = EObjectFlags.ClassDefaultObject,
                bool includeDerivedClasses = true,
                EInternalObjectFlags internalExclusionFlags = EInternalObjectFlags.None)
            {
                index = -1;

                using (TArrayUnsafe<IntPtr> result = new TArrayUnsafe<IntPtr>())
                {
                    Native_UObjectHash.GetObjectsOfClass(unrealClass, result.Address, includeDerivedClasses,
                        additionalExclusionFlags, internalExclusionFlags);
                    objectArray = result.ToArray();
                }
            }

            public IntPtr Current
            {
                get { return objectArray == null || index < 0 || index >= objectArray.Length ? IntPtr.Zero : objectArray[index]; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (objectArray == null)
                {
                    return false;
                }

                //@todo UE4 check this for LHS on Index on consoles
                while (++index < objectArray.Length)
                {
                    if (Current != null)
                    {
                        return true;
                    }
                }
                return false;
            }

            public void Reset()
            {
                index = -1;
            }

            public IEnumerator GetEnumerator()
            {
                return this;
            }
        }
    }

    public partial class NativeReflection
    {
        // Helpers for getting the type of a given UProperty address

        static Dictionary<IntPtr, EPropertyType> propertyTypesByClass = new Dictionary<IntPtr, EPropertyType>();

        public static EPropertyType GetPropertyType(IntPtr propertyAddress)
        {
            if (propertyAddress != IntPtr.Zero)
            {
                return GetPropertyClassType(Native_UObjectBase.GetClass(propertyAddress));
            }
            return EPropertyType.Unknown;
        }

        public static EPropertyType GetPropertyClassType(IntPtr propertyClassAddress)
        {
            EPropertyType propertyType;
            propertyTypesByClass.TryGetValue(propertyClassAddress, out propertyType);
            return propertyType;
        }

        public static Type GetPropertyType(EPropertyType propertyType)
        {
            switch (propertyType)
            {
                case EPropertyType.Bool: return typeof(UBoolProperty);

                case EPropertyType.Int8: return typeof(UInt8Property);
                case EPropertyType.Int16: return typeof(UInt16Property);
                case EPropertyType.Int: return typeof(UIntProperty);
                case EPropertyType.Int64: return typeof(UInt64Property);

                case EPropertyType.Byte: return typeof(UByteProperty);
                case EPropertyType.UInt16: return typeof(UUInt16Property);
                case EPropertyType.UInt32: return typeof(UUInt32Property);
                case EPropertyType.UInt64: return typeof(UUInt64Property);

                case EPropertyType.Double: return typeof(UDoubleProperty);
                case EPropertyType.Float: return typeof(UFloatProperty);

                case EPropertyType.Enum: return typeof(UEnumProperty);

                case EPropertyType.Interface: return typeof(UInterfaceProperty);
                case EPropertyType.Struct: return typeof(UStructProperty);
                case EPropertyType.Class: return typeof(UClassProperty);

                case EPropertyType.Object: return typeof(UObjectProperty);
                case EPropertyType.LazyObject: return typeof(ULazyObjectProperty);
                case EPropertyType.WeakObject: return typeof(UWeakObjectProperty);

                case EPropertyType.SoftClass: return typeof(USoftClassProperty);
                case EPropertyType.SoftObject: return typeof(USoftObjectProperty);

                case EPropertyType.Delegate: return typeof(UDelegateProperty);
                case EPropertyType.MulticastDelegate: return typeof(UMulticastDelegateProperty);

                case EPropertyType.Array: return typeof(UArrayProperty);
                case EPropertyType.Map: return typeof(UMapProperty);
                case EPropertyType.Set: return typeof(USetProperty);

                case EPropertyType.Str: return typeof(UStrProperty);
                case EPropertyType.Name: return typeof(UNameProperty);
                case EPropertyType.Text: return typeof(UTextProperty);

                default: return null;
            }
        }

        public static IntPtr GetPropertyClass(EPropertyType propertyType)
        {
            switch (propertyType)
            {
                case EPropertyType.Bool: return Classes.UBoolProperty;

                case EPropertyType.Int8: return Classes.UInt8Property;
                case EPropertyType.Int16: return Classes.UInt16Property;
                case EPropertyType.Int: return Classes.UIntProperty;
                case EPropertyType.Int64: return Classes.UInt64Property;

                case EPropertyType.Byte: return Classes.UByteProperty;
                case EPropertyType.UInt16: return Classes.UUInt16Property;
                case EPropertyType.UInt32: return Classes.UUInt32Property;
                case EPropertyType.UInt64: return Classes.UUInt64Property;

                case EPropertyType.Double: return Classes.UDoubleProperty;
                case EPropertyType.Float: return Classes.UFloatProperty;

                case EPropertyType.Enum: return Classes.UEnumProperty;

                case EPropertyType.Interface: return Classes.UInterfaceProperty;
                case EPropertyType.Struct: return Classes.UStructProperty;
                case EPropertyType.Class: return Classes.UClassProperty;

                case EPropertyType.Object: return Classes.UObjectProperty;
                case EPropertyType.LazyObject: return Classes.ULazyObjectProperty;
                case EPropertyType.WeakObject: return Classes.UWeakObjectProperty;

                case EPropertyType.SoftClass: return Classes.USoftClassProperty;
                case EPropertyType.SoftObject: return Classes.USoftObjectProperty;

                case EPropertyType.Delegate: return Classes.UDelegateProperty;
                case EPropertyType.MulticastDelegate: return Classes.UMulticastDelegateProperty;

                case EPropertyType.Array: return Classes.UArrayProperty;
                case EPropertyType.Map: return Classes.UMapProperty;
                case EPropertyType.Set: return Classes.USetProperty;

                case EPropertyType.Str: return Classes.UStrProperty;
                case EPropertyType.Name: return Classes.UNameProperty;
                case EPropertyType.Text: return Classes.UTextProperty;

                default: return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Gets the property class name (including the prefix e.g. UBoolProperty)
        /// </summary>
        public static string GetPropertyClassName(EPropertyType propertyType)
        {
            Type type = GetPropertyType(propertyType);
            return type != null ? type.Name : null;
        }

        /// <summary>
        /// Gets the property class name (including the prefix e.g. UBoolProperty)
        /// </summary>
        public static bool TryGetPropertyClassName(EPropertyType propertyType, out string propertyClassName)
        {
            propertyClassName = GetPropertyClassName(propertyType);
            return !string.IsNullOrEmpty(propertyClassName);
        }

        internal static void OnNativeFunctionsRegistered()
        {
            propertyTypesByClass.Clear();
            propertyTypesByClass[Classes.UBoolProperty] = EPropertyType.Bool;
            propertyTypesByClass[Classes.UInt8Property] = EPropertyType.Int8;
            propertyTypesByClass[Classes.UInt16Property] = EPropertyType.Int16;
            propertyTypesByClass[Classes.UIntProperty] = EPropertyType.Int;
            propertyTypesByClass[Classes.UInt64Property] = EPropertyType.Int64;
            propertyTypesByClass[Classes.UByteProperty] = EPropertyType.Byte;
            propertyTypesByClass[Classes.UUInt16Property] = EPropertyType.UInt16;
            propertyTypesByClass[Classes.UUInt32Property] = EPropertyType.UInt32;
            propertyTypesByClass[Classes.UUInt64Property] = EPropertyType.UInt64;
            propertyTypesByClass[Classes.UDoubleProperty] = EPropertyType.Double;
            propertyTypesByClass[Classes.UFloatProperty] = EPropertyType.Float;
            propertyTypesByClass[Classes.UEnumProperty] = EPropertyType.Enum;
            propertyTypesByClass[Classes.UInterfaceProperty] = EPropertyType.Interface;
            propertyTypesByClass[Classes.UStructProperty] = EPropertyType.Struct;
            propertyTypesByClass[Classes.UClassProperty] = EPropertyType.Class;
            propertyTypesByClass[Classes.UObjectProperty] = EPropertyType.Object;
            propertyTypesByClass[Classes.ULazyObjectProperty] = EPropertyType.LazyObject;
            propertyTypesByClass[Classes.UWeakObjectProperty] = EPropertyType.WeakObject;
            propertyTypesByClass[Classes.USoftClassProperty] = EPropertyType.SoftClass;
            propertyTypesByClass[Classes.USoftObjectProperty] = EPropertyType.SoftObject;
            propertyTypesByClass[Classes.UDelegateProperty] = EPropertyType.Delegate;
            propertyTypesByClass[Classes.UMulticastDelegateProperty] = EPropertyType.MulticastDelegate;
            propertyTypesByClass[Classes.UArrayProperty] = EPropertyType.Array;
            propertyTypesByClass[Classes.UMapProperty] = EPropertyType.Map;
            propertyTypesByClass[Classes.USetProperty] = EPropertyType.Set;
            propertyTypesByClass[Classes.UStrProperty] = EPropertyType.Str;
            propertyTypesByClass[Classes.UNameProperty] = EPropertyType.Name;
            propertyTypesByClass[Classes.UTextProperty] = EPropertyType.Text;
        }
    }

    public partial class NativeReflection
    {
        public static class LookupTable
        {
            public static Dictionary<MethodInfo, IntPtr> Functions { get; private set; }
            public static Dictionary<IntPtr, MethodInfo> FunctionsByAddress { get; private set; }
            public static Dictionary<IntPtr, Dictionary<MethodInfo, IntPtr>> FunctionsByClass { get; private set; }
            public static Dictionary<IntPtr, Dictionary<IntPtr, MethodInfo>> FunctionsAddressByClass { get; private set; }

            static LookupTable()
            {
                Functions = new Dictionary<MethodInfo, IntPtr>();
                FunctionsByAddress = new Dictionary<IntPtr, MethodInfo>();
                FunctionsByClass = new Dictionary<IntPtr, Dictionary<MethodInfo, IntPtr>>();
                FunctionsAddressByClass = new Dictionary<IntPtr, Dictionary<IntPtr, MethodInfo>>();
            }

            public static void ResetClass(IntPtr unrealClass)
            {
                Dictionary<MethodInfo, IntPtr> functions;
                if (FunctionsByClass.TryGetValue(unrealClass, out functions))
                {
                    foreach (KeyValuePair<MethodInfo, IntPtr> function in functions)
                    {
                        Functions.Remove(function.Key);
                        FunctionsByAddress.Remove(function.Value);
                    }
                    FunctionsByClass.Remove(unrealClass);
                    FunctionsAddressByClass.Remove(unrealClass);
                }
            }

            public static IntPtr FindFunctionFromClass(IntPtr unrealClass, MethodInfo methodInfo, bool searchHierarchy = true)
            {
                if (methodInfo == null)
                {
                    return IntPtr.Zero;
                }

                IntPtr targetClass = unrealClass;
                while (targetClass != IntPtr.Zero)
                {
                    if (!FunctionsByClass.ContainsKey(targetClass))
                    {
                        Load(targetClass, methodInfo.DeclaringType);
                    }

                    Dictionary<MethodInfo, IntPtr> functions;
                    IntPtr functionAddress;
                    if (FunctionsByClass.TryGetValue(targetClass, out functions) &&
                        functions.TryGetValue(methodInfo, out functionAddress))
                    {
                        return functionAddress;
                    }

                    if (!searchHierarchy)
                    {
                        break;
                    }

                    targetClass = Native_UClass.GetSuperClass(targetClass);
                }

                return IntPtr.Zero;
            }

            public static IntPtr FindFunction(IntPtr owner, MethodInfo methodInfo, bool searchHierarchy = true)
            {
                return FindFunctionFromClass(Native_UObjectBase.GetClass(owner), methodInfo, searchHierarchy);
            }

            public static IntPtr FindFunction(UObject owner, MethodInfo methodInfo, bool searchHierarchy = true)
            {
                return FindFunctionFromClass(Native_UObjectBase.GetClass(owner.Address), methodInfo, searchHierarchy);
            }

            private static void Load(IntPtr unrealClass, Type type)
            {
                Dictionary<MethodInfo, IntPtr> classFunctions = new Dictionary<MethodInfo, IntPtr>();
                Dictionary<IntPtr, MethodInfo> classFunctionsByAddress = new Dictionary<IntPtr, MethodInfo>();
                FunctionsByClass.Add(unrealClass, classFunctions);
                FunctionsAddressByClass.Add(unrealClass, classFunctionsByAddress);

                if (!type.IsSubclassOf(typeof(UObject)))
                {                    
                    return;
                }

                BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                foreach (MethodInfo method in type.GetMethods(bindingFlags))
                {
                    UUnrealTypePathAttribute pathAttribute = method.GetCustomAttribute<UUnrealTypePathAttribute>(false);
               
                    if (pathAttribute != null && !string.IsNullOrEmpty(pathAttribute.Path))
                    {
                        IntPtr functionAddress = GetFunction(pathAttribute.Path);
                        if (functionAddress != IntPtr.Zero)
                        {
                            Functions[method] = functionAddress;
                            FunctionsByAddress[functionAddress] = method;

                            classFunctions[method] = functionAddress;
                            classFunctionsByAddress[functionAddress] = method;
                        }
                    }
                }
            }
            
            public static IntPtr GetFunctionAddress(Delegate del, bool searchHierarchy = true)
            {
                IntPtr functionAddress;
                UObject target;
                GetFunctionAddress(del, out functionAddress, out target, searchHierarchy);
                return functionAddress;
            }

            public static bool GetFunctionAddress(Delegate del, out IntPtr functionAddress, out UObject target, bool searchHierarchy = true)
            {
                if (del != null)
                {
                    target = del.Target as UObject;
                    if (target != null)
                    {
                        functionAddress = NativeReflection.LookupTable.FindFunction(target, del.Method, searchHierarchy);
                        if (functionAddress != IntPtr.Zero)
                        {
                            return true;
                        }
                    }
                }
                target = null;
                functionAddress = IntPtr.Zero;
                return false;
            }
        }
    }
}
