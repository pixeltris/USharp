using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TWeakObject<T> : IEquatable<TWeakObject<T>> where T : UObject
    {
        internal FWeakObjectPtr weakObjectPtr;

        public static TWeakObject<T> Null
        {
            get { return default(TWeakObject<T>); }
        }

        public T Value
        {
            get { return Get() as T; }
            set { Set(value); }
        }

        public bool IsStale
        {
            get { return weakObjectPtr.IsStale; }
        }

        public TWeakObject(FWeakObjectPtr weakObjectPtr)
        {
            this.weakObjectPtr = new FWeakObjectPtr(weakObjectPtr);
        }

        public TWeakObject(IntPtr native)
            : this(Marshal.PtrToStructure<FWeakObjectPtr>(native))
        {
        }

        public TWeakObject(T obj)
        {
            weakObjectPtr = new FWeakObjectPtr(obj);
        }

        public bool IsValid()
        {
            return weakObjectPtr.IsValid(false, false);
        }

        public bool IsValid(bool evenIfPendingKill, bool threadsafeTest = false)
        {
            return weakObjectPtr.IsValid(evenIfPendingKill, threadsafeTest);
        }

        public T Get()
        {
            return weakObjectPtr.Get() as T;
        }

        public T GetEvenIfUnreachable()
        {
            return weakObjectPtr.GetEvenIfUnreachable() as T;
        }

        public void Set(T value)
        {
            weakObjectPtr.Set(value);
        }

        public void Set(FWeakObjectPtr value)
        {
            weakObjectPtr.Set(value);
        }

        public void Reset()
        {
            weakObjectPtr.Reset();
        }

        public static bool operator ==(TWeakObject<T> a, TWeakObject<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TWeakObject<T> a, TWeakObject<T> b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is TWeakObject<T>)
            {
                return Equals((TWeakObject<T>)obj);
            }
            return false;
        }

        public bool Equals(TWeakObject<T> other)
        {
            return weakObjectPtr == other.weakObjectPtr;
        }

        public override int GetHashCode()
        {
            return weakObjectPtr.GetHashCode();
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct FWeakObjectPtr : IEquatable<FWeakObjectPtr>
    {
        public int ObjectIndex;
        public int ObjectSerialNumber;

        public bool IsStale
        {
            get { return Native_FWeakObjectPtr.IsStale(ref this); }
        }

        public static FWeakObjectPtr Default
        {
            get
            {
                FWeakObjectPtr result = new FWeakObjectPtr();
                result.Reset();
                return result;
            }
        }

        public FWeakObjectPtr(UObject obj)
        {
            ObjectIndex = -1;
            ObjectSerialNumber = 0;
            Set(obj);
        }

        public FWeakObjectPtr(IntPtr objAddress)
        {
            ObjectIndex = -1;
            ObjectSerialNumber = 0;
            Set(objAddress);
        }

        public FWeakObjectPtr(FWeakObjectPtr obj)
        {
            ObjectIndex = -1;
            ObjectSerialNumber = 0;
            Set(obj);
        }

        public bool IsValid()
        {
            return IsValid(false, false);
        }

        public bool IsValid(bool evenIfPendingKill, bool threadsafeTest = false)
        {
            return Native_FWeakObjectPtr.IsValid(ref this, evenIfPendingKill, threadsafeTest);
        }

        public UObject Get()
        {
            return GCHelper.Find(Native_FWeakObjectPtr.Get(ref this));
        }        

        public UObject GetEvenIfUnreachable()
        {
            return GCHelper.Find(Native_FWeakObjectPtr.GetEvenIfUnreachable(ref this));
        }

        /// <summary>
        /// Returns the UObject as an IntPtr
        /// </summary>
        public IntPtr GetPtr()
        {
            return Native_FWeakObjectPtr.Get(ref this);
        }

        /// <summary>
        /// Returns the UObject as an IntPtr
        /// </summary>
        public IntPtr GetPtrEvenIfUnreachable()
        {
            return Native_FWeakObjectPtr.GetEvenIfUnreachable(ref this);
        }

        /// <summary>
        /// Set UObject value as IntPtr
        /// </summary>
        public void Set(IntPtr value)
        {
            Native_FWeakObjectPtr.SetUObject(ref this, value);
        }

        public void Set(UObject value)
        {
            Native_FWeakObjectPtr.SetUObject(ref this, value == null ? IntPtr.Zero : value.Address);
        }

        public void Set(FWeakObjectPtr value)
        {
            Native_FWeakObjectPtr.SetFWeakObjectPtr(ref this, ref value);
        }

        public void Reset()
        {
            Native_FWeakObjectPtr.Reset(ref this);
        }        

        public static bool operator ==(FWeakObjectPtr a, FWeakObjectPtr b)
        {
            return Native_FWeakObjectPtr.Equals(ref a, ref b);
        }

        public static bool operator !=(FWeakObjectPtr a, FWeakObjectPtr b)
        {
            return !Native_FWeakObjectPtr.Equals(ref a, ref b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FWeakObjectPtr)
            {
                return Equals((FWeakObjectPtr)obj);
            }
            return false;
        }

        public bool Equals(FWeakObjectPtr other)
        {
            return Native_FWeakObjectPtr.Equals(ref this, ref other);
        }

        public override int GetHashCode()
        {
            return (int)Native_FWeakObjectPtr.GetTypeHash(ref this);
        }
    }
}
