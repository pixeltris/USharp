using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public abstract class StructAsClass : IDisposable
    {
        private bool initialized;
        private bool selfAllocated;

        public UObject Owner { get; internal set; }
        public IntPtr Address { get; internal set; }

        private IntPtr structAddress;

        public StructAsClass()
        {
            structAddress = GetStructAddress();
        }

        /// <summary>
        /// Checks if the UObject which owns this struct is destroyed (if this struct is contained within a UObject)
        /// </summary>
        protected void CheckDestroyed()
        {
            if (Owner != null && Owner.IsDestroyed)
            {
                throw new UObjectDestroyedException("Trying to access a StructAsClass which points to memory of a destroyed UObject (" + 
                    NativeReflection.GetPathName(structAddress) + ")");
            }
            if (!initialized)
            {
                throw new Exception("Trying to access a StructAsClass which either wasn't initialized or was destroyed (" +
                    NativeReflection.GetPathName(structAddress) + ")");
            }
        }

        public void Dispose()
        {
            Destroy();
        }

        private void EnsureNotInitialized()
        {
            if (initialized)
            {
                throw new Exception("StructAsClass is already initialized '" + GetType().FullName + "'");
            }
            if (structAddress == IntPtr.Zero)
            {
                throw new Exception("StructAsClass is not loaded '" + GetType().FullName + "'");
            }
        }

        public void Initialize(IntPtr address)
        {
            EnsureNotInitialized();

            Address = address;
            Initialize();
        }

        internal void Initialize()
        {
            EnsureNotInitialized();

            if (Address == IntPtr.Zero)
            {
                Address = FMemory.Malloc(NativeReflection.GetStructSize(structAddress));
                Native_UStruct.InitializeStruct(structAddress, Address, 1);
            }

            initialized = true;
        }

        internal void Destroy()
        {
            if (Address != IntPtr.Zero)
            {
                Native_UStruct.DestroyStruct(structAddress, Address, 1);
                if (selfAllocated)
                {
                    FMemory.Free(Address);
                }
                Address = IntPtr.Zero;
                initialized = false;
                selfAllocated = false;
            }
        }

        private void EnsureInitialized()
        {
            if (!initialized || Address == IntPtr.Zero)
            {
                throw new Exception("StructAsClass is not initialized '" + GetType().FullName + "'");
            }
            if (structAddress == IntPtr.Zero)
            {
                throw new Exception("StructAsClass is not loaded '" + GetType().FullName + "'");
            }
        }

        internal void InternalCopyTo(IntPtr address)
        {
            EnsureInitialized();
            Native_UScriptStruct.CopyScriptStruct(structAddress, address, Address, 1);
        }

        internal void InternalCopyFrom(IntPtr address)
        {            
            if (address == IntPtr.Zero)
            {
                return;
            }
            EnsureInitialized();
            Native_UScriptStruct.CopyScriptStruct(structAddress, Address, address, 1);
        }

        internal void InternalCopyFromInstance(StructAsClass other)
        {
            if (other == null || !other.initialized || other.Address == IntPtr.Zero)
            {
                return;
            }
            InternalCopyFrom(other.Address);
        }

        protected virtual IntPtr GetStructAddress()
        {
            return IntPtr.Zero;
        }
    }

    static class StructAsClassExtensions
    {
        public static T Copy<T>(this T value) where T : StructAsClass, new()
        {
            T copy = new T();
            copy.Initialize();
            value.InternalCopyTo(copy.Address);
            return copy;
        }

        public static void CopyFrom<T>(this T value, T other) where T : StructAsClass
        {
            value.InternalCopyFromInstance(other);
        }
    }
}
