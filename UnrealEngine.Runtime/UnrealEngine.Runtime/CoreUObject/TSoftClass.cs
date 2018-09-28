using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // TSoftClassPtr<TClass>
    /// <summary>
    /// TSoftClassPtr is a templatized wrapper around FSoftObjectPtr that works like a TSubclassOf, it can be used in UProperties for blueprint subclasses.
    /// 
    /// (Also see TSoftObject which provides access to the object instead of the class)
    /// </summary>
    public struct TSoftClass<T> : IEquatable<TSoftClass<T>> where T : UObject
    {
        private FSoftObjectPtr softObject;

        public static TSoftClass<T> Null
        {
            get { return default(TSoftClass<T>); }
        }

        public FSoftObjectPath ObjectPath
        {
            get { return softObject.ObjectPath; }
        }

        public string Path
        {
            get { return softObject.Path; }
        }

        public UClass Value
        {
            get
            {
                UClass unrealClass = softObject.Value as UClass;
                if (unrealClass == null || !unrealClass.IsChildOf<T>())
                {
                    return null;
                }
                return unrealClass;
            }
            set { SetClass(value); }
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

        public TSoftClass(UClass unrealClass)
        {
            softObject = default(FSoftObjectPtr);
            Value = unrealClass;
        }

        public TSoftClass(FSoftObjectPath objectPath)
        {
            softObject = new FSoftObjectPtr(objectPath);
        }

        public TSoftClass(string objectPath)
        {
            softObject = new FSoftObjectPtr(new FSoftObjectPath(new FName(objectPath), null));
        }

        public void SetClass(UClass unrealClass)
        {
            if (unrealClass != null && !unrealClass.IsA<T>())
            {
                throw new Exception("TAssetClass - tried to set class with the wrong target class type. Expected:" +
                    typeof(T) + " Actual:" + UClass.GetType(unrealClass.Address));
            }
            softObject.Value = unrealClass;
        }

        public void SetClass<TClass>() where TClass : T
        {
            SetClass(UClass.GetClass<TClass>());
        }

        public void Reset()
        {
            softObject.Reset();
        }

        /// <summary>
        /// Synchronously load (if necessary) and return the asset object represented by this asset ptr
        /// </summary>
        public UClass LoadSynchronous()
        {
            UObject asset = softObject.Value;
            if (asset == null || IsPending)
            {
                asset = softObject.LoadSynchronous();
            }
            UClass unrealClass = asset as UClass;
            if (unrealClass == null || !unrealClass.IsChildOf<T>())
            {
                return null;
            }
            return unrealClass;
        }

        public static bool operator ==(TSoftClass<T> a, TSoftClass<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TSoftClass<T> a, TSoftClass<T> b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is TSoftClass<T>)
            {
                return Equals((TSoftClass<T>)obj);
            }
            return false;
        }

        public bool Equals(TSoftClass<T> other)
        {
            return softObject == other.softObject;
        }

        public override int GetHashCode()
        {
            return softObject.GetHashCode();
        }
    }

    // This represents the layout of the native TSoftClassPtr
    [StructLayout(LayoutKind.Sequential)]
    public struct FSoftClassPtr : IDisposable
    {
        /// <summary>
        /// Wrapper of a UObject*
        /// </summary>
        public FSoftObjectPtrUnsafe SoftObjectPtr;

        public void Dispose()
        {
            SoftObjectPtr.Dispose();
        }
    }
}
