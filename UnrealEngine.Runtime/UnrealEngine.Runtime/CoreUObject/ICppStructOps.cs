using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Interface to template to manage dynamic access to C++ struct construction and destruction
    /// </summary>
    public struct ICppStructOps
    {
        public IntPtr Address;

        public bool IsValid
        {
            get { return Address != IntPtr.Zero; }
        }

        public ICppStructOps(IntPtr address)
        {
            Address = address;
        }

        /// <summary>
        /// return true if this class has a no-op constructor and takes EForceInit to init
        /// </summary>
        public bool HasNoopConstructor
        {
            get { return Native_ICppStructOps.HasNoopConstructor(Address); }
        }

        /// <summary>
        /// return true if memset can be used instead of the constructor
        /// </summary>
        public bool HasZeroConstructor
        {
            get { return Native_ICppStructOps.HasZeroConstructor(Address); }
        }

        /// <summary>
        /// Call the C++ constructor
        /// </summary>
        public void Construct(IntPtr dest)
        {
            Native_ICppStructOps.Construct(Address, dest);
        }

        /// <summary>
        /// return false if this destructor can be skipped
        /// </summary>
        public bool HasDestructor
        {
            get { return Native_ICppStructOps.HasDestructor(Address); }
        }

        /// <summary>
        /// Call the C++ destructor
        /// </summary>
        public void Destruct(IntPtr dest)
        {
            Native_ICppStructOps.Destruct(Address, dest);
        }

        /// <summary>
        /// return the sizeof() of this structure
        /// </summary>
        public int Size
        {
            get { return Native_ICppStructOps.GetSize(Address); }
        }

        /// <summary>
        /// return the ALIGNOF() of this structure
        /// </summary>
        public int Alignment
        {
            get { return Native_ICppStructOps.GetAlignment(Address); }
        }

        /// <summary>
        /// return true if this struct should be memcopied
        /// </summary>
        public bool IsPlainOldData
        {
            get { return Native_ICppStructOps.IsPlainOldData(Address); }
        }

        /// <summary>
        /// return true if this struct can copy
        /// </summary>
        public bool HasCopy
        {
            get { return Native_ICppStructOps.HasCopy(Address); }
        }

        /// <summary>
        /// Copy this structure
        /// </summary>
        /// <returns>true if the copy was handled, otherwise it will fall back to CopySingleValue</returns>
        public bool Copy(IntPtr dest, IntPtr src, int arrayDim)
        {
            return Native_ICppStructOps.Copy(Address, dest, src, arrayDim);
        }

        /// <summary>
        /// return true if this struct can compare
        /// </summary>
        public bool HasIdentical
        {
            get { return Native_ICppStructOps.HasIdentical(Address); }
        }

        /// <summary>
        /// Compare this structure
        /// </summary>
        /// <returns>true if the copy was handled, otherwise it will fall back to UStructProperty::Identical</returns>
        public bool Identical(IntPtr a, IntPtr b, uint portFlags, out bool outResult)
        {
            csbool outResultTemp;
            bool result = Native_ICppStructOps.Identical(Address, a, b, portFlags, out outResultTemp);
            outResult = outResultTemp;
            return result;
        }

        /// <summary>
        /// return true if this struct has a GetTypeHash
        /// </summary>
        public bool HasGetTypeHash
        {
            get { return Native_ICppStructOps.HasGetTypeHash(Address); }
        }

        /// <summary>
        /// Calls GetTypeHash if enabled
        /// </summary>
        public uint GetTypeHash(IntPtr src)
        {
            return Native_ICppStructOps.GetTypeHash(Address, src);
        }

        /// <summary>
        /// Returns property flag values that can be computed at compile time
        /// </summary>
        public EPropertyFlags ComputedPropertyFlags
        {
            get { return (EPropertyFlags)Native_ICppStructOps.GetComputedPropertyFlags(Address); }
        }

        /// <summary>
        /// return true if this struct is abstract
        /// </summary>
        public bool IsAbstract
        {
            get { return Native_ICppStructOps.IsAbstract(Address); }
        }
    }
}
