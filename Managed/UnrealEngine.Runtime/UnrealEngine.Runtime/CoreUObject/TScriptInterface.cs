using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Templated version of FScriptInterface, which provides accessors and operators for referencing the interface portion of a UObject that implements a native interface.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TScriptInterface<T> : IEquatable<TScriptInterface<T>> where T : class, IInterface
    {
        public FScriptInterface Base;

        public TScriptInterface(T value)
        {
            IntPtr interfaceClass = UClass.GetInterfaceClassAddress<T>();
            if (interfaceClass == IntPtr.Zero)
            {
                Base = default(FScriptInterface);
                return;
            }

            IntPtr objectPointer = value.GetAddress();
            if (objectPointer == IntPtr.Zero)
            {
                Base = default(FScriptInterface);
                return;
            }

            IntPtr interfacePointer = Native_UObjectBaseUtility.GetNativeInterfaceAddress(objectPointer, interfaceClass);
            if (interfacePointer == IntPtr.Zero)
            {
                Base = default(FScriptInterface);
                return;
            }

            Base = new FScriptInterface(objectPointer, interfacePointer);
        }

        public UObject GetObject()
        {
            return Base.GetObject();
        }

        public T GetInterface()
        {
            return Base.GetInterface<T>();
        }

        public static bool operator ==(TScriptInterface<T> a, TScriptInterface<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TScriptInterface<T> a, TScriptInterface<T> b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is TScriptInterface<T>)
            {
                return Equals((TScriptInterface<T>)obj);
            }
            return false;
        }

        public bool Equals(TScriptInterface<T> other)
        {
            return Base == other.Base;
        }

        public override int GetHashCode()
        {
            return Base.GetHashCode();
        }
    }

    /// <summary>
    /// This utility class stores the UProperty data for a native interface property.
    /// ObjectPointer and InterfacePointer point to different locations in the same UObject.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FScriptInterface : IEquatable<FScriptInterface>
    {
        /// <summary>
        /// A pointer to a UObject that implements a native interface.
        /// </summary>
        public IntPtr ObjectPointer;

        /// <summary>
        /// Pointer to the location of the interface object within the UObject referenced by ObjectPointer.
        /// </summary>
        public IntPtr InterfacePointer;

        public FScriptInterface(IntPtr objectPointer, IntPtr interfacePointer)
        {
            ObjectPointer = objectPointer;
            InterfacePointer = interfacePointer;
        }

        /// <summary>
        /// Returns the ObjectPointer contained by this FScriptInterface
        /// </summary>
        public UObject GetObject()
        {
            return GCHelper.Find(ObjectPointer);
        }

        /// <summary>
        /// Returns the interface
        /// </summary>
        public T GetInterface<T>() where T : class, IInterface
        {
            UObject obj = GetObject();
            if (obj != null)
            {
                return obj.GetInterface<T>();
            }
            return null;
        }

        public static bool operator ==(FScriptInterface a, FScriptInterface b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FScriptInterface a, FScriptInterface b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FScriptInterface)
            {
                return Equals((FScriptInterface)obj);
            }
            return false;
        }

        public bool Equals(FScriptInterface other)
        {
            return ObjectPointer == other.ObjectPointer && InterfacePointer == other.InterfacePointer;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + ObjectPointer.GetHashCode();
                hash = hash * 23 + InterfacePointer.GetHashCode();
                return hash;
            }
        }
    }
}
