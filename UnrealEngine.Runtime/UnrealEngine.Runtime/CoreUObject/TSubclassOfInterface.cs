using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // NOTE: C++ only has TSubclassOf<> without the need for a specialization like we are using here. We require
    // a specialization so that we can work with UObject for regular classes and IInterface on interfaces.

    /// <summary>
    /// Template to allow UClass's to be passed around with type safety.
    /// TSubclassOfInterface is a C# specialization for working with interfaces which are compatible with Unreal.
    /// </summary>
    public struct TSubclassOfInterface<T> : IEquatable<TSubclassOfInterface<T>> where T : class, IInterface
    {
        internal FSubclassOf subclassOf;

        public static TSubclassOfInterface<T> Null
        {
            get { return default(TSubclassOfInterface<T>); }
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

        public TSubclassOfInterface(UClass unrealClass)
        {
            subclassOf = default(FSubclassOf);
            value = null;
            SetClass(unrealClass);
        }

        public void SetClass(UClass unrealClass)
        {
            if (unrealClass != null)
            {
                if (!unrealClass.ImplementsInterface<T>())
                {
                    throw new Exception("TSubclassOfInterface - the given class doesn't implement the interface: '" +
                        typeof(T) + "' class:" + UClass.GetType(unrealClass.Address));
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

        public static bool operator ==(TSubclassOfInterface<T> a, TSubclassOfInterface<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TSubclassOfInterface<T> a, TSubclassOfInterface<T> b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is TSubclassOfInterface<T>)
            {
                return Equals((TSubclassOfInterface<T>)obj);
            }
            return false;
        }

        public bool Equals(TSubclassOfInterface<T> other)
        {
            return subclassOf == other.subclassOf;
        }

        public override int GetHashCode()
        {
            return subclassOf.GetHashCode();
        }
    }
}
