using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// TSoftObjectPtr is templatized wrapper of the generic FSoftObjectPtr, it can be used in UProperties
    /// 
    /// (Also see TSoftClass which provides access to the class instead of the object)
    /// </summary>
    public struct TSoftObject<T> : IEquatable<TSoftObject<T>> where T : UObject
    {
        private FSoftObjectPtr softObject;

        public static TSoftObject<T> Null
        {
            get { return default(TSoftObject<T>); }
        }

        public FSoftObjectPath ObjectPath
        {
            get { return softObject.ObjectPath; }
        }

        /// <summary>
        /// Returns string representation reference, in form /package/path.assetname[:subpath]
        /// </summary>
        public string Path
        {
            get { return softObject.Path; }
        }

        /// <summary>
        /// Asset path, patch top level object in a package. This is /package/path.assetname
        /// </summary>
        public string AssetPathName
        {
            get { return softObject.AssetPathName; }
        }

        /// <summary>
        /// Optional FString for subobject within an asset. This is the sub path after the :
        /// </summary>
        public string SubPathString
        {
            get { return softObject.SubPathString; }
        }

        /// <summary>
        /// Returns /package/path string, leaving off the asset name
        /// </summary>
        public string LongPackageName
        {
            get { return softObject.LongPackageName; }
        }

        /// <summary>
        /// Returns assetname string, leaving off the /package/path. path
        /// </summary>
        public string AssetName
        {
            get { return softObject.AssetName; }
        }

        public T Value
        {
            get { return softObject.Value as T; }
            set { softObject.Value = value; }
        }

        public bool IsPending
        {
            get { return softObject.IsPending; }
        }

        public bool IsValid
        {
            get { return softObject.IsValid; }
        }

        public bool IsStale
        {
            get { return softObject.IsStale; }
        }

        public bool IsNull
        {
            get { return softObject.IsNull; }
        }

        public TSoftObject(T obj)
        {
            softObject = default(FSoftObjectPtr);
            Value = obj;
        }

        public TSoftObject(FSoftObjectPath softObject)
        {
            this.softObject = new FSoftObjectPtr(softObject);
        }

        public void Reset()
        {
            softObject.Reset();
        }

        /// <summary>
        /// Synchronously load (if necessary) and return the asset object represented by this asset ptr
        /// </summary>
        public T LoadSynchronous()
        {
            UObject obj = softObject.Value;
            if (obj == null && IsPending)
            {
                obj = softObject.LoadSynchronous();
            }
            return obj as T;
        }

        public static bool operator ==(TSoftObject<T> a, TSoftObject<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TSoftObject<T> a, TSoftObject<T> b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is TSoftObject<T>)
            {
                return Equals((TSoftObject<T>)obj);
            }
            return false;
        }

        public bool Equals(TSoftObject<T> other)
        {
            return softObject == other.softObject;
        }

        public override int GetHashCode()
        {
            return softObject.GetHashCode();
        }
    }

    /// <summary>
    /// This is a wrapper for FSoftObjectPtrUnsafe, allowing for safe usage without having to deal with the FString which needs
    /// to be appropriately copied / destroyed depending on usage. This is slower as it needs to be resolved every time.
    /// </summary>
    public struct FSoftObjectPtr : IEquatable<FSoftObjectPtr>
    {
        private FSoftObjectPath softObjectPath;

        public FSoftObjectPath ObjectPath
        {
            get { return softObjectPath; }
        }

        /// <summary>
        /// Returns string representation reference, in form /package/path.assetname[:subpath]
        /// </summary>
        public string Path
        {
            get { return softObjectPath.Path; }
        }

        /// <summary>
        /// Asset path, patch top level object in a package. This is /package/path.assetname
        /// </summary>
        public string AssetPathName
        {
            get { return softObjectPath.AssetPathName.ToString(); }
        }

        /// <summary>
        /// Optional FString for subobject within an asset. This is the sub path after the :
        /// </summary>
        public string SubPathString
        {
            get { return softObjectPath.SubPathString; }
        }

        /// <summary>
        /// Returns /package/path string, leaving off the asset name
        /// </summary>
        public string LongPackageName
        {
            get { return softObjectPath.LongPackageName; }
        }

        /// <summary>
        /// Returns assetname string, leaving off the /package/path. path
        /// </summary>
        public string AssetName
        {
            get { return softObjectPath.AssetName; }
        }

        public UObject Value
        {
            get
            {
                using (FSoftObjectPtrUnsafe objectPtr = new FSoftObjectPtrUnsafe(softObjectPath))
                {
                    return objectPtr.Get();
                }
            }
            set
            {
                using (FSoftObjectPtrUnsafe objectPtr = new FSoftObjectPtrUnsafe(value))
                {
                    softObjectPath = new FSoftObjectPath(objectPtr.ObjectPath.AssetPathName, objectPtr.ObjectPath.SubPathString);
                }
            }
        }

        public bool IsPending
        {
            get
            {
                using (FSoftObjectPtrUnsafe objectPtr = new FSoftObjectPtrUnsafe(softObjectPath))
                {
                    return objectPtr.IsPending;
                }
            }
        }

        public bool IsValid
        {
            get
            {
                using (FSoftObjectPtrUnsafe objectPtr = new FSoftObjectPtrUnsafe(softObjectPath))
                {
                    return objectPtr.IsValid;
                }
            }
        }

        public bool IsStale
        {
            get
            {
                using (FSoftObjectPtrUnsafe objectPtr = new FSoftObjectPtrUnsafe(softObjectPath))
                {
                    return objectPtr.IsStale;
                }
            }
        }

        public bool IsNull
        {
            get
            {
                using (FSoftObjectPtrUnsafe objectPtr = new FSoftObjectPtrUnsafe(softObjectPath))
                {
                    return objectPtr.IsNull;
                }
            }
        }

        public FSoftObjectPtr(FSoftObjectPath softObjectPath)
        {
            this.softObjectPath = softObjectPath;
        }

        public void Reset()
        {
            softObjectPath.Reset();
        }

        public UObject LoadSynchronous()
        {
            using (FSoftObjectPtrUnsafe objectPtr = new FSoftObjectPtrUnsafe(softObjectPath))
            {
                return objectPtr.LoadSynchronous();
            }
        }

        public static bool operator ==(FSoftObjectPtr a, FSoftObjectPtr b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FSoftObjectPtr a, FSoftObjectPtr b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FSoftObjectPtr)
            {
                return Equals((FSoftObjectPtr)obj);
            }
            return false;
        }

        public bool Equals(FSoftObjectPtr other)
        {
            return Path == other.Path;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 17;
                string assetPathName = AssetPathName;
                if (assetPathName == null)
                {
                    assetPathName = string.Empty;
                }
                string subPathString = SubPathString;
                if (subPathString == null)
                {
                    subPathString = string.Empty;
                }
                hashCode = hashCode * 23 + assetPathName.GetHashCode();
                hashCode = hashCode * 23 + subPathString.GetHashCode();
                return hashCode;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FSoftObjectPtrUnsafe : IDisposable, IEquatable<FSoftObjectPtrUnsafe>
    {
        private TPersistentObjectPtr<FSoftObjectPathUnsafe> objectPath;

        public FSoftObjectPath ObjectPath
        {
            get { return objectPath.ObjectID.ToSafe(); }
            set
            {
                // Assign the members directly to avoid a having a shallow copy
                objectPath.ObjectID.AssetPathName = value.AssetPathName;
                objectPath.ObjectID.SubPathString = value.SubPathString;
            }
        }

        public bool IsPending
        {
            get { return Native_FSoftObjectPtr.IsPending(ref this); }
        }

        public bool IsValid
        {
            get { return Native_FSoftObjectPtr.IsValid(ref this); }
        }

        public bool IsStale
        {
            get { return Native_FSoftObjectPtr.IsStale(ref this); }
        }

        public bool IsNull
        {
            get { return Native_FSoftObjectPtr.IsNull(ref this); }
        }

        public FSoftObjectPtrUnsafe(FName assetPathName, string subPathString)
        {
            objectPath = new TPersistentObjectPtr<FSoftObjectPathUnsafe>();
            objectPath.WeakPtr.Reset();
            objectPath.TagAtLastTest = 0;

            objectPath.ObjectID.AssetPathName = assetPathName;
            objectPath.ObjectID.SubPathString = subPathString;
        }

        public FSoftObjectPtrUnsafe(FSoftObjectPath assetRef)
        {
            objectPath = new TPersistentObjectPtr<FSoftObjectPathUnsafe>();
            objectPath.WeakPtr.Reset();
            objectPath.TagAtLastTest = 0;

            ObjectPath = assetRef;
        }

        public FSoftObjectPtrUnsafe(FSoftObjectPathUnsafe assetRefUnsafe)
            : this(assetRefUnsafe.ToSafe())
        {
        }

        public FSoftObjectPtrUnsafe(FSoftObjectPtrUnsafe other)
        {
            objectPath = new TPersistentObjectPtr<FSoftObjectPathUnsafe>();
            objectPath.WeakPtr.Reset();
            objectPath.TagAtLastTest = 0;

            ObjectPath = other.objectPath.ObjectID.ToSafe();
        }

        public FSoftObjectPtrUnsafe(UObject obj)
        {
            objectPath = new TPersistentObjectPtr<FSoftObjectPathUnsafe>();
            objectPath.WeakPtr.Reset();
            objectPath.TagAtLastTest = 0;

            Set(obj);
        }

        /// <summary>
        ///  Deep copies the given value into the current structure
        /// </summary>
        /// <param name="other"></param>
        public void Copy(FSoftObjectPtrUnsafe other)
        {
            ObjectPath = other.ObjectPath;
        }

        public FSoftObjectPath GetObjectPath()
        {
            return objectPath.ObjectID.ToSafe();
        }

        public TPersistentObjectPtr<FSoftObjectPathUnsafe> GetObjectPathUnsafe()
        {
            return objectPath;
        }

        public UObject Get()
        {
            return GCHelper.Find(Native_FSoftObjectPtr.Get(ref this));
        }

        public void Set(UObject value)
        {
            Native_FSoftObjectPtr.SetUObject(ref this, value == null ? IntPtr.Zero : value.Address);
        }

        public void Set(FWeakObjectPtr value)
        {
            Native_FSoftObjectPtr.SetFWeakObjectPtr(ref this, ref value);
        }

        public void Reset()
        {
            Native_FSoftObjectPtr.Reset(ref this);
        }

        public UObject LoadSynchronous()
        {
            return GCHelper.Find(Native_FSoftObjectPtr.LoadSynchronous(ref this));
        }

        public static bool operator ==(FSoftObjectPtrUnsafe a, FSoftObjectPtrUnsafe b)
        {
            return Native_FSoftObjectPtr.Equals(ref a, ref b);
        }

        public static bool operator !=(FSoftObjectPtrUnsafe a, FSoftObjectPtrUnsafe b)
        {
            return !Native_FSoftObjectPtr.Equals(ref a, ref b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FSoftObjectPtrUnsafe)
            {
                return Equals((FSoftObjectPtrUnsafe)obj);
            }
            return false;
        }

        public bool Equals(FSoftObjectPtrUnsafe other)
        {
            return Native_FSoftObjectPtr.Equals(ref this, ref other);
        }

        public override int GetHashCode()
        {
            return (int)Native_FSoftObjectPtr.GetTypeHash(ref this);
        }

        public void Dispose()
        {
            objectPath.ObjectID.Dispose();
        }
    }
}
