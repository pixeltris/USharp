using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Template to allow UClass's to be passed around with type safety
    /// </summary>
    public struct TSubclassOf<T> : IEquatable<TSubclassOf<T>> where T : UObject
    {
        internal FSubclassOf subclassOf;

        public static TSubclassOf<T> Null
        {
            get { return default(TSubclassOf<T>); }
        }

        private UClass value;
        public UClass Value
        {
            get
            {
                if (subclassOf.ClassAddress != IntPtr.Zero && (value == null || value.Address != subclassOf.ClassAddress))
                {
                    value = GCHelper.Find<UClass>(subclassOf.ClassAddress);
                }
                return value;
            }
            set { SetClass(value); }
        }

        public TSubclassOf(UClass unrealClass)
        {
            subclassOf = default(FSubclassOf);
            value = null;
            SetClass(unrealClass);
        }

        public void SetClass(UClass unrealClass)
        {
            if (unrealClass != null)
            {
                if (!unrealClass.IsChildOf<T>())
                {
                    throw new Exception("TSubclassOf - tried to set class with the wrong target class type. Expected:" +
                        typeof(T) + " Actual:" + UClass.GetType(unrealClass.Address));
                }
                subclassOf.ClassAddress = unrealClass.Address;
            }
            else
            {
                subclassOf.ClassAddress = IntPtr.Zero;
            }
        }

        public void SetClass<TClass>() where TClass : T
        {
            SetClass(UClass.GetClass<TClass>());
        }

        /// <summary>
        /// Get the CDO if we are referencing a valid class
        /// </summary>
        /// <returns>the CDO, or NULL</returns>
        public T GetDefaultObject()
        {
            UClass unrealClass = Value;
            if (unrealClass != null)
            {
                return unrealClass.GetDefaultObject() as T;
            }
            return null;
        }

        public static bool operator ==(TSubclassOf<T> a, TSubclassOf<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TSubclassOf<T> a, TSubclassOf<T> b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is TSubclassOf<T>)
            {
                return Equals((TSubclassOf<T>)obj);
            }
            return false;
        }

        public bool Equals(TSubclassOf<T> other)
        {
            return subclassOf == other.subclassOf;
        }

        public override int GetHashCode()
        {
            return subclassOf.GetHashCode();
        }
    }

    // This represents the layout of the native TSubclassOf
    [StructLayout(LayoutKind.Sequential)]
    public struct FSubclassOf : IEquatable<FSubclassOf>
    {
        public IntPtr ClassAddress;

        public UClass Class
        {
            get { return GCHelper.Find<UClass>(ClassAddress); }
        }

        public static bool operator ==(FSubclassOf a, FSubclassOf b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FSubclassOf a, FSubclassOf b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FSubclassOf)
            {
                return Equals((FSubclassOf)obj);
            }
            return false;
        }

        public bool Equals(FSubclassOf other)
        {
            return ClassAddress == other.ClassAddress;
        }

        public override int GetHashCode()
        {
            return ClassAddress.GetHashCode();
        }
    }
}
