using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TLazyObject<T> : IEquatable<TLazyObject<T>> where T : UObject
    {
        internal FLazyObjectPtr lazyObjectPtr;

        public static TLazyObject<T> Null
        {
            get { return default(TLazyObject<T>); }
        }

        public T Value
        {
            get { return Get() as T; }
            set { Set(value); }
        }

        public FUniqueObjectGuid Guid
        {
            get { return lazyObjectPtr.Guid; }
            set { lazyObjectPtr.Guid = value; }
        }

        public bool IsPending
        {
            get { return lazyObjectPtr.IsPending; }
        }

        public bool IsValid
        {
            get { return lazyObjectPtr.IsValid; }
        }

        public bool IsStale
        {
            get { return lazyObjectPtr.IsStale; }
        }

        public bool IsNull
        {
            get { return lazyObjectPtr.IsNull; }
        }

        public TLazyObject(FLazyObjectPtr lazyObjectPtr)
        {
            this.lazyObjectPtr = new FLazyObjectPtr(lazyObjectPtr);
        }

        public TLazyObject(IntPtr native)
            : this(Marshal.PtrToStructure<FLazyObjectPtr>(native))
        {            
        }

        public TLazyObject(T obj)
        {
            lazyObjectPtr = new FLazyObjectPtr(obj);
        }

        public T Get()
        {
            return lazyObjectPtr.Get() as T;
        }

        public void Set(T value)
        {
            lazyObjectPtr.Set(value);
        }

        public void Set(FLazyObjectPtr value)
        {
            lazyObjectPtr.Set(value);
        }

        public void Reset()
        {
            lazyObjectPtr.Reset();
        }

        public static bool operator ==(TLazyObject<T> a, TLazyObject<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TLazyObject<T> a, TLazyObject<T> b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is TLazyObject<T>)
            {
                return Equals((TLazyObject<T>)obj);
            }
            return false;
        }

        public bool Equals(TLazyObject<T> other)
        {
            return lazyObjectPtr == other.lazyObjectPtr;
        }

        public override int GetHashCode()
        {
            return lazyObjectPtr.GetHashCode();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FLazyObjectPtr : IEquatable<FLazyObjectPtr>
    {
        private TPersistentObjectPtr<FUniqueObjectGuid> uniqueObjectGuid;

        public FUniqueObjectGuid Guid
        {
            get { return uniqueObjectGuid.ObjectID; }
            set
            {
                // Assign the members directly to avoid a having a shallow copy
                uniqueObjectGuid.ObjectID.Guid = value.Guid;
            }
        }

        public bool IsPending
        {
            get { return Native_FLazyObjectPtr.IsPending(ref this); }
        }

        public bool IsValid
        {
            get { return Native_FLazyObjectPtr.IsValid(ref this); }
        }

        public bool IsStale
        {
            get { return Native_FLazyObjectPtr.IsStale(ref this); }
        }

        public bool IsNull
        {
            get { return Native_FLazyObjectPtr.IsNull(ref this); }
        }

        public FLazyObjectPtr(FUniqueObjectGuid guid)
        {
            uniqueObjectGuid = new TPersistentObjectPtr<FUniqueObjectGuid>();
            uniqueObjectGuid.WeakPtr.Reset();
            uniqueObjectGuid.TagAtLastTest = 0;

            Guid = guid;
        }

        public FLazyObjectPtr(FLazyObjectPtr other)
        {
            uniqueObjectGuid = new TPersistentObjectPtr<FUniqueObjectGuid>();
            uniqueObjectGuid.WeakPtr.Reset();
            uniqueObjectGuid.TagAtLastTest = 0;

            Guid = other.uniqueObjectGuid.ObjectID;
        }

        public FLazyObjectPtr(UObject obj)
        {
            uniqueObjectGuid = new TPersistentObjectPtr<FUniqueObjectGuid>();
            uniqueObjectGuid.WeakPtr.Reset();
            uniqueObjectGuid.TagAtLastTest = 0;

            Set(obj);
        }

        /// <summary>
        ///  Deep copies the given value into the current structure
        /// </summary>
        /// <param name="other"></param>
        public void Copy(FLazyObjectPtr other)
        {
            Guid = other.Guid;
        }

        public TPersistentObjectPtr<FUniqueObjectGuid> GetUniqueObjectGuid()
        {
            return uniqueObjectGuid;
        }

        public UObject Get()
        {
            return GCHelper.Find(Native_FLazyObjectPtr.Get(ref this));
        }

        public void Set(UObject value)
        {
            Native_FLazyObjectPtr.SetUObject(ref this, value == null ? IntPtr.Zero : value.Address);
        }

        public void Set(FLazyObjectPtr value)
        {
            Native_FLazyObjectPtr.SetFLazyObjectPtr(ref this, ref value);
        }

        public void Reset()
        {
            Native_FLazyObjectPtr.Reset(ref this);
        }

        public static bool operator ==(FLazyObjectPtr a, FLazyObjectPtr b)
        {
            return Native_FLazyObjectPtr.Equals(ref a, ref b);
        }

        public static bool operator !=(FLazyObjectPtr a, FLazyObjectPtr b)
        {
            return !Native_FLazyObjectPtr.Equals(ref a, ref b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FLazyObjectPtr)
            {
                return Equals((FLazyObjectPtr)obj);
            }
            return false;
        }

        public bool Equals(FLazyObjectPtr other)
        {
            return Native_FLazyObjectPtr.Equals(ref this, ref other);
        }

        public override int GetHashCode()
        {
            return (int)Native_FLazyObjectPtr.GetTypeHash(ref this);
        }
    }
}
