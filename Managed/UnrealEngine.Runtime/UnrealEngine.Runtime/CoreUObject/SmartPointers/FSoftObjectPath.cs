using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FSoftObjectPathUnsafe : IDisposable
    {        
        private FName assetPathName;
        private FScriptArray subPathString;

        /// <summary>
        /// Asset path, patch top level object in a package. This is /package/path.assetname
        /// </summary>
        public FName AssetPathName
        {
            get { return assetPathName; }
            internal set { assetPathName = value; }
        }

        /// <summary>
        /// Optional FString for subobject within an asset. This is the sub path after the :
        /// </summary>
        public string SubPathString
        {
            get { return FStringMarshaler.FromArray(subPathString, false); }
            internal set { FStringMarshaler.ToArray(ref subPathString, value); }
        }
        
        /// <summary>
        /// Returns the /package/path, leaving off the asset name and sub object
        /// </summary>
        public string LongPackageName
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FSoftObjectPath.GetLongPackageName(ref this, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns assetname string, leaving off the /package/path part and sub object
        /// </summary>
        public string AssetName
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FSoftObjectPath.GetAssetName(ref this, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns string representation reference, in form /package/path.assetname[:subpath]
        /// </summary>
        public string Path
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FSoftObjectPath.ToString(ref this, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Check if this could possibly refer to a real object, or was initialized to NULL
        /// </summary>
        public bool IsValid
        {
            get { return Native_FSoftObjectPath.IsValid(ref this); }
        }

        /// <summary>
        /// Checks to see if this is initialized to null
        /// </summary>
        public bool IsNull
        {
            get { return Native_FSoftObjectPath.IsNull(ref this); }
        }

        /// <summary>
        /// Check if this represents an asset, meaning it is not null but does not have a sub path
        /// </summary>
        public bool IsAsset
        {
            get { return Native_FSoftObjectPath.IsAsset(ref this); }
        }

        public FSoftObjectPathUnsafe(FSoftObjectPath softObjectPath)
            : this(softObjectPath.AssetPathName, softObjectPath.SubPathString)
        {
        }

        public FSoftObjectPathUnsafe(FName assetPathName, string subPathString)
        {
            this.assetPathName = assetPathName;
            this.subPathString = default(FScriptArray);
            FStringMarshaler.ToArray(ref this.subPathString, subPathString);
        }

        /// <summary>
        /// Sets asset path of this reference.
        /// </summary>
        /// <param name="path">The path to the asset.</param>
        public void SetPath(string path)
        {
            using (FStringUnsafe pathUnsafe = new FStringUnsafe(path))
            {
                Native_FSoftObjectPath.SetPath(ref this, ref pathUnsafe.Array);
            }
        }

        /// <summary>
        /// Returns string version of asset path, including both package and asset but not subobject
        /// </summary>
        /// <returns></returns>
        public string GetAssetPathString()
        {
            if (AssetPathName == FName.None)
            {
                return string.Empty;
            }

            return assetPathName.ToString();
        }

        /// <summary>
        /// Attempts to load the asset.
        /// </summary>
        /// <returns>Loaded UObject, or null if the asset fails to load, or if the reference is not valid.</returns>
        public UObject TryLoad()
        {
            return GCHelper.Find(Native_FSoftObjectPath.TryLoad(ref this));
        }

        /// <summary>
        /// Attempts to find a currently loaded object that matches this object ID
        /// </summary>
        /// <returns>Found UObject, or NULL if not currently loaded</returns>
        public UObject ResolveObject()
        {
            return GCHelper.Find(Native_FSoftObjectPath.ResolveObject(ref this));
        }

        /// <summary>
        /// Resets reference to point to NULL
        /// </summary>
        public void Reset()
        {
            Native_FSoftObjectPath.Reset(ref this);
        }

        public FSoftObjectPath ToSafe()
        {
            return new FSoftObjectPath(assetPathName, SubPathString);
        }

        public override string ToString()
        {
            return AssetPathName.ToString();
        }

        public void Dispose()
        {
            assetPathName = FName.None;
            subPathString.Destroy();
        }
    }

    /// <summary>
    /// A struct that contains a string reference to an object, either a top level asset or a subobject
    /// This can be used to make soft references to assets that are loaded on demand.
    /// This is stored internally as an FName pointing to the top level asset (/package/path.assetname) and an option a string subobject path.
    /// If the MetaClass metadata is applied to a UProperty with this the UI will restrict to that type of asset.
    /// </summary>
    [UStruct(Flags = 0x000B980A), BlueprintType, UMetaPath("/Script/CoreUObject.SoftObjectPath", "CoreUObject", UnrealModuleType.Engine)]
    public struct FSoftObjectPath : IEquatable<FSoftObjectPath>
    {
        /// <summary>
        /// Asset path, patch top level object in a package. This is /package/path.assetname
        /// </summary>
        public FName AssetPathName { get; private set; }

        /// <summary>
        /// Optional FString for subobject within an asset. This is the sub path after the :
        /// </summary>
        public string SubPathString { get; private set; }

        /// <summary>
        /// Returns the /package/path, leaving off the asset name and sub object
        /// </summary>
        public string LongPackageName
        {
            get
            {
                using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
                {
                    return objectPathUnsafe.LongPackageName;
                }
            }
        }

        /// <summary>
        /// Returns assetname string, leaving off the /package/path part and sub object
        /// </summary>
        public string AssetName
        {
            get
            {
                using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
                {
                    return objectPathUnsafe.AssetName;
                }
            }
        }

        /// <summary>
        /// Returns string representation reference, in form /package/path.assetname[:subpath]
        /// </summary>
        public string Path
        {
            get
            {
                using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
                {
                    return objectPathUnsafe.Path;
                }
            }
        }

        /// <summary>
        /// Check if this could possibly refer to a real object, or was initialized to NULL
        /// </summary>
        public bool IsValid
        {
            get
            {
                using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
                {
                    return objectPathUnsafe.IsValid;
                }
            }
        }

        /// <summary>
        /// Checks to see if this is initialized to null
        /// </summary>
        public bool IsNull
        {
            get
            {
                using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
                {
                    return objectPathUnsafe.IsNull;
                }
            }
        }

        /// <summary>
        /// Check if this represents an asset, meaning it is not null but does not have a sub path
        /// </summary>
        public bool IsAsset
        {
            get
            {
                using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
                {
                    return objectPathUnsafe.IsAsset;
                }
            }
        }

        public FSoftObjectPath(FName assetPathName, string subPathString)
        {
            AssetPathName = assetPathName;
            SubPathString = subPathString;
        }

        public FSoftObjectPath(FSoftObjectPathUnsafe softObjectPath)
        {
            AssetPathName = softObjectPath.AssetPathName;
            SubPathString = softObjectPath.SubPathString;
        }

        public FSoftObjectPath(IntPtr softObjectPathPtr)
            : this(Marshal.PtrToStructure<FSoftObjectPathUnsafe>(softObjectPathPtr))
        {
        }

        public FSoftObjectPath(UObject obj)
        {
            this = default(FSoftObjectPath);
            if (obj != null)
            {
                SetPath(obj.GetPathName());
            }
        }

        /// <summary>
        /// Sets asset path of this reference.
        /// </summary>
        /// <param name="path">The path to the asset.</param>
        public void SetPath(string path)
        {
            using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
            {
                objectPathUnsafe.SetPath(path);
                AssetPathName = objectPathUnsafe.AssetPathName;
                SubPathString = objectPathUnsafe.SubPathString;
            }
        }

        /// <summary>
        /// Attempts to load the asset.
        /// </summary>
        /// <returns>Loaded UObject, or null if the asset fails to load, or if the reference is not valid.</returns>
        public UObject TryLoad()
        {
            using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
            {
                return objectPathUnsafe.TryLoad();
            }
        }

        /// <summary>
        /// Attempts to find a currently loaded object that matches this object ID
        /// </summary>
        /// <returns>Found UObject, or NULL if not currently loaded</returns>
        public UObject ResolveObject()
        {
            using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
            {
                return objectPathUnsafe.ResolveObject();
            }
        }

        public void Reset()
        {
            using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
            {
                objectPathUnsafe.Reset();
                AssetPathName = objectPathUnsafe.AssetPathName;
                SubPathString = objectPathUnsafe.SubPathString;
            }
        }

        public static bool operator ==(FSoftObjectPath a, FSoftObjectPath b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FSoftObjectPath a, FSoftObjectPath b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FSoftObjectPath)
            {
                return Equals((FSoftObjectPath)obj);
            }
            return false;
        }

        public bool Equals(FSoftObjectPath other)
        {
            if (AssetPathName == other.AssetPathName)
            {
                string path = SubPathString != null ? SubPathString : string.Empty;
                string otherPath = other.SubPathString != null ? other.SubPathString : string.Empty;
                return path == otherPath;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unsafe
            {
                int hash = 17;
                hash = hash * 23 + AssetPathName.GetHashCode();
                hash = hash * 23 + (SubPathString != null ? SubPathString : string.Empty).GetHashCode();
                return hash;
            }
        }

        // Marshalers

        public FSoftObjectPath Copy()
        {
            FSoftObjectPath result = this;
            return result;
        }

        public static FSoftObjectPath FromNative(IntPtr nativeBuffer)
        {
            return FSoftObjectPathMarshaler.FromNative(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, FSoftObjectPath value)
        {
            FSoftObjectPathMarshaler.ToNative(nativeBuffer, value);
        }

        public static FSoftObjectPath FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            return FSoftObjectPathMarshaler.FromNative(nativeBuffer, arrayIndex, prop);
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, FSoftObjectPath value)
        {
            FSoftObjectPathMarshaler.ToNative(nativeBuffer, arrayIndex, prop, value);
        }
    }

    /// <summary>
    /// A struct that contains a string reference to a class, can be used to make soft references to classes
    /// </summary>
    [UStruct(Flags = 0x000B980A), BlueprintType, UMetaPath("/Script/CoreUObject.SoftClassPath", "CoreUObject", UnrealModuleType.Engine)]
    public struct FSoftClassPath : IEquatable<FSoftClassPath>
    {
        public FSoftObjectPath ObjectPath;

        public FSoftClassPath(FSoftObjectPath objectPath)
        {
            ObjectPath = objectPath;
        }

        public FSoftClassPath(FName assetPathName, string subPathString)
        {
            ObjectPath = new FSoftObjectPath(assetPathName, subPathString);
        }

        public FSoftClassPath(FSoftObjectPathUnsafe softObjectPath)
        {
            ObjectPath = new FSoftObjectPath(softObjectPath);
        }

        public FSoftClassPath(IntPtr softObjectPathPtr)
        {
            ObjectPath = new FSoftObjectPath(softObjectPathPtr);
        }

        public FSoftClassPath(UObject obj)
        {
            ObjectPath = new FSoftObjectPath(obj);
        }

        /// <summary>
        /// Attempts to load the class.
        /// </summary>
        /// <typeparam name="T">The type of class to load.</typeparam>
        /// <returns>Loaded UObject, or null if the class fails to load, or if the reference is not valid.</returns>
        public UClass TryLoadClass<T>() where T : UObject
        {
            if (ObjectPath.IsValid)
            {
                return UObject.LoadClass<T>(null,  ObjectPath.Path, null, ELoadFlags.None);
            }
            return null;
        }

        /// <summary>
        /// Attempts to find a currently loaded object that matches this object ID
        /// </summary>
        /// <returns>Found UClass, or NULL if not currently loaded</returns>
        public UClass ResolveClass()
        {
            return ObjectPath.ResolveObject() as UClass;
        }

        public static bool operator ==(FSoftClassPath a, FSoftClassPath b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FSoftClassPath a, FSoftClassPath b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FSoftClassPath)
            {
                return Equals((FSoftClassPath)obj);
            }
            return false;
        }

        public bool Equals(FSoftClassPath other)
        {
            return ObjectPath == other.ObjectPath;
        }

        public override int GetHashCode()
        {
            return ObjectPath.GetHashCode();
        }

        // Marshalers

        public FSoftClassPath Copy()
        {
            FSoftClassPath result = this;
            return result;
        }

        public static FSoftClassPath FromNative(IntPtr nativeBuffer)
        {
            return new FSoftClassPath(FSoftObjectPathMarshaler.FromNative(nativeBuffer));
        }

        public static void ToNative(IntPtr nativeBuffer, FSoftClassPath value)
        {
            FSoftObjectPathMarshaler.ToNative(nativeBuffer, value.ObjectPath);
        }

        public static FSoftClassPath FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            return new FSoftClassPath(FSoftObjectPathMarshaler.FromNative(nativeBuffer, arrayIndex, prop));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, FSoftClassPath value)
        {
            FSoftObjectPathMarshaler.ToNative(nativeBuffer, arrayIndex, prop, value.ObjectPath);
        }
    }
}
