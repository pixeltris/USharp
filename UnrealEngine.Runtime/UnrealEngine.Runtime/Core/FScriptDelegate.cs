using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // TBaseDynamicDelegate inherits from TScriptDelegate (TBaseDynamicMulticastDelegate / TMulticastScriptDelegate)
    // These delegates are defined with DECLARE_DYNAMIC_DELEGATE / DECLARE_DYNAMIC_MULTICAST_DELEGATE

    /// <summary>
    /// Script delegate base class.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FScriptDelegate
    {
        /// <summary>
        /// The object bound to this delegate, or nullptr if no object is bound
        /// </summary>
        public FWeakObjectPtr Object;

        /// <summary>
        /// Name of the function to call on the bound object
        /// </summary>
        public FName FunctionName;

        public bool IsBound
        {
            get
            {
                if (FunctionName != FName.None)
                {
                    IntPtr obj = Object.GetPtr();
                    return obj != IntPtr.Zero && Native_UObject.FindFunction(obj, ref FunctionName) != IntPtr.Zero;
                }
                return false;
            }
        }

        public FScriptDelegate(UObject obj, FName functionName)
        {
            Object = new FWeakObjectPtr();
            FunctionName = functionName;
            Object.Set(obj);
        }

        public FScriptDelegate(IntPtr obj, FName functionName)
        {
            Object = new FWeakObjectPtr();
            FunctionName = functionName;
            Object.Set(obj);
        }

        public FScriptDelegate(FWeakObjectPtr obj, FName functionName)
        {
            Object = obj;
            FunctionName = functionName;
        }

        public void Bind(IntPtr obj, FName functionName)
        {
            Object.Set(obj);
            FunctionName = functionName;
        }

        public void Unbind(IntPtr obj, FName functionName)
        {
            if (Object.GetPtr() == obj && FunctionName == functionName)
            {
                Object.Set(IntPtr.Zero);
                FunctionName = FName.None;
            }
        }

        /// <summary>
        /// Checks to see if this delegate is bound to the given user object and function.
        /// </summary>
        /// <param name="obj">The UObject derived object to check</param>
        /// <param name="functionName">The name of the function to check</param>
        /// <returns>True if the delegate is bound to the given object and function name, otherwise false.</returns>
        public bool IsTargetBound(IntPtr obj, FName functionName)
        {
            if (FunctionName == FName.None)
            {
                return false;
            }
            return Object.GetPtrEvenIfUnreachable() == obj && FunctionName == functionName;
        }

        /// <summary>
        /// Checks to see if this delegate is bound to the given user object.
        /// </summary>
        /// <param name="obj">The UObject derived object to check</param>
        /// <returns>True if this delegate is bound to the given object, false otherwise.</returns>
        public bool IsBoundToObject(IntPtr obj)
        {
            if (obj == IntPtr.Zero)
            {
                return false;
            }
            return Object.GetPtrEvenIfUnreachable() == obj;
        }

        public void Clear()
        {
            Object.Set(IntPtr.Zero);
            FunctionName = FName.None;
        }

        public void ProcessDelegate(IntPtr parameters)
        {
            Native_FScriptDelegate.ProcessDelegate(ref this, parameters);
        }

        public IntPtr GetFunctionAddress()
        {
            if (FunctionName == FName.None)
            {
                return IntPtr.Zero;
            }
            IntPtr obj = Object.GetPtr();
            return obj == IntPtr.Zero ? IntPtr.Zero : Native_UObject.FindFunction(obj, ref FunctionName);
        }

        public UFunction GetFunction()
        {
            return GCHelper.Find<UFunction>(GetFunctionAddress());
        }
    }

    /// <summary>
    /// Script multi-cast delegate base class
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FMulticastScriptDelegate
    {        
        /// <summary>
        /// Ordered list functions to invoke when the Broadcast function is called
        /// </summary>
        public FScriptArray InvocationList;//TArray<TScriptDelegate>

        private int ElementSize
        {
            get { return Marshal.SizeOf(typeof(FScriptDelegate)); }
        }

        public FScriptDelegate this[int index]
        {
            get
            {
                return BlittableTypeMarshaler<FScriptDelegate>.FromNative(InvocationList.Data, index);
            }
            set
            {
                BlittableTypeMarshaler<FScriptDelegate>.ToNative(InvocationList.Data, index, value);
            }
        }

        public FScriptDelegate Get(int index)
        {
            return this[index];
        }

        public void Set(int index, FScriptDelegate value)
        {
            this[index] = value;
        }

        public int Count
        {
            get { return InvocationList.Count; }
        }

        public void Clear()
        {
            // Should be safe to just call empty. There shouldn't be any memory that needs to be freed per element.
            InvocationList.Empty(0, ElementSize);
        }

        public void Add(FScriptDelegate value)
        {
            this[InvocationList.AddZeroed(ElementSize)] = value;
        }

        public void Remove(FScriptDelegate value)
        {
            int index = IndexOf(value);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < Count)
            {
                InvocationList.RemoveAt(index, ElementSize);
            }
        }

        public int IndexOf(FScriptDelegate value)
        {
            EqualityComparer<FScriptDelegate> comparer = EqualityComparer<FScriptDelegate>.Default;
            int count = Count;
            for(int i = 0; i < count; ++i)
            {
                if (comparer.Equals(this[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool Contains(FScriptDelegate value)
        {
            return IndexOf(value) >= 0;
        }

        public int IndexOf(IntPtr obj, FName functionName)
        {
            if (obj == IntPtr.Zero)
            {
                // Can't be bound to a nullptr object
                return -1;
            }
            int count = Count;
            for (int i = 0; i < count; ++i)
            {
                FScriptDelegate scriptDelegate = this[i];
                if (scriptDelegate.Object.GetPtrEvenIfUnreachable() == obj && scriptDelegate.FunctionName == functionName)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool Contains(IntPtr obj, FName functionName)
        {
            return IndexOf(obj, functionName) >= 0;
        }

        /// <summary>
        /// Checks to see if any functions are bound to the given user object.
        /// </summary>
        /// <param name="obj">The UObject derived object to check</param>
        /// <returns>True if any functions are bound to the given object, false otherwise.</returns>
        public bool IsBoundToObject(IntPtr obj)
        {
            if (obj == IntPtr.Zero)
            {
                return false;
            }
            int count = Count;
            for (int i = 0; i < count; ++i)
            {
                FScriptDelegate scriptDelegate = this[i];
                if (scriptDelegate.Object.GetPtrEvenIfUnreachable() == obj)
                {
                    return true;
                }
            }
            return false;
        }

        public void ProcessMulticastDelegate(IntPtr parameters)
        {
            Native_FMulticastScriptDelegate.ProcessMulticastDelegate(ref this, parameters);
        }
    }

    /// <summary>
    /// Wrapper class for when working with delegates in managed code (this is to avoid having to manage
    /// the TArray memory which would require FMulticastScriptDelegate to implement IDisposable)
    /// </summary>
    struct FMulticastScriptDelegateWrapper
    {
        private List<FScriptDelegate> delegates;
        public List<FScriptDelegate> Delegates
        {
            get { return delegates != null ? delegates : delegates = new List<FScriptDelegate>(); }
        }

        public FScriptDelegate this[int index]
        {
            get
            {
                return Delegates[index];
            }
            set
            {
                Delegates[index] = value;
            }
        }

        public int Count
        {
            get { return Delegates.Count; }
        }

        public void Clear()
        {
            Delegates.Clear();
        }

        public void Add(FScriptDelegate value)
        {
            Delegates.Add(value);
        }

        public void Remove(FScriptDelegate value)
        {
            int index = IndexOf(value);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < Count)
            {
                Delegates.RemoveAt(index);
            }
        }

        public int IndexOf(FScriptDelegate value)
        {
            return Delegates.IndexOf(value);
        }

        public bool Contains(FScriptDelegate value)
        {
            return IndexOf(value) >= 0;
        }

        public int IndexOf(IntPtr obj, FName functionName)
        {
            int count = Count;
            for (int i = 0; i < count; ++i)
            {
                FScriptDelegate scriptDelegate = this[i];
                if (scriptDelegate.Object.GetPtrEvenIfUnreachable() == obj && scriptDelegate.FunctionName == functionName)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool Contains(IntPtr obj, FName functionName)
        {
            return IndexOf(obj, functionName) >= 0;
        }

        /// <summary>
        /// Checks to see if any functions are bound to the given user object.
        /// </summary>
        /// <param name="obj">The UObject derived object to check</param>
        /// <returns>True if any functions are bound to the given object, false otherwise.</returns>
        public bool IsBoundToObject(IntPtr obj)
        {
            if (obj == IntPtr.Zero)
            {
                return false;
            }
            int count = Count;
            for (int i = 0; i < count; ++i)
            {
                FScriptDelegate scriptDelegate = this[i];
                if (scriptDelegate.Object.GetPtrEvenIfUnreachable() == obj)
                {
                    return true;
                }
            }
            return false;
        }

        public void ProcessMulticastDelegate(IntPtr parameters)
        {
            // Create an unmanaged version of this delegate and then destroy the underlying TArray once complete
            FMulticastScriptDelegate scriptDelegate = new FMulticastScriptDelegate();
            scriptDelegate.InvocationList.AddZeroed(Marshal.SizeOf(typeof(FScriptDelegate)), Count);
            for (int i = 0; i < Count; ++i)
            {
                BlittableTypeMarshaler<FScriptDelegate>.ToNative(scriptDelegate.InvocationList.Data, i, this[i]);
            }
            Native_FMulticastScriptDelegate.ProcessMulticastDelegate(ref scriptDelegate, parameters);
            // Something may have modified the delegate? Copy it back from the native array.
            CopyFrom(scriptDelegate);
            scriptDelegate.InvocationList.Destroy();
        }

        public void CopyFrom(FMulticastScriptDelegate native)
        {
            Clear();
            int count = native.Count;
            for (int i = 0; i < count; ++i)
            {
                Add(native[i]);
            }
        }
    }
}
