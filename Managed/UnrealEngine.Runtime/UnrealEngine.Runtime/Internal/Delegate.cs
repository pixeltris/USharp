using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public abstract class IDelegateBase
    {
        public abstract void FromNative(IntPtr address);
        public abstract void ToNative(IntPtr address);
    }

    public abstract class FDelegateBase<TDelegate> : IDelegateBase where TDelegate : class
    {
        public IntPtr Address { get; private set; }
        public bool IsNative
        {
            get { return Address != IntPtr.Zero; }
        }

        /// <summary>
        /// Call this to invoke the delegate
        /// </summary>
        public TDelegate Invoke;

        public abstract bool IsBound { get; }

        public FDelegateBase()
        {
            Invoke = GetInvoker();
        }

        /// <summary>
        /// Set the address of the delegate. If this address isn't maintained by unreal then you are responsible 
        /// for cleaning up any unmanaged memory (FMulticastScriptDelegate)
        /// </summary>
        public void SetAddress(IntPtr address)
        {
            Address = address;
        }

        public virtual TDelegate GetInvoker()
        {
            return null;
        }

        private void OnInvalidDelegate(TDelegate evnt)
        {
            string objPath = "null";
            string funcName = "null";
            Delegate del = evnt as Delegate;
            if (del != null)
            {
                UObject obj = del.Target as UObject;
                if (obj != null)
                {
                    objPath = obj.GetPathName();
                }
                else
                {
                    objPath = "null";
                }
                if (del.Method != null)
                {
                    funcName = del.Method.DeclaringType != null ?
                        del.Method.DeclaringType.FullName + "." + del.Method.Name : del.Method.Name;
                }
                else
                {
                    funcName = evnt.ToString();
                }
            }
            else
            {
                funcName = evnt.ToString();
            }
            FMessage.Log(ELogVerbosity.Error, "Failed to find UFunction for delegate / dispatcher - UObject("
                + objPath + ") Func(" + funcName + ")");
        }

        public void Bind(UObject obj, FName functionName)
        {
            Bind(obj.Address, functionName);
        }

        public void Bind(TDelegate evnt)
        {
            IntPtr functionAddress;
            UObject target;
            if (GetFunctionAddress(evnt, out functionAddress, out target))
            {
                Bind(target, NativeReflection.GetFName(functionAddress));
            }
            else
            {
                OnInvalidDelegate(evnt);
            }
        }

        public void Unbind(UObject obj, FName functionName)
        {
            Unbind(obj.Address, functionName);
        }

        public void Unbind(TDelegate evnt)
        {
            IntPtr functionAddress;
            UObject target;
            if (GetFunctionAddress(evnt, out functionAddress, out target))
            {
                Unbind(target, NativeReflection.GetFName(functionAddress));
            }
            else
            {
                OnInvalidDelegate(evnt);
            }
        }

        public bool IsTargetBound(TDelegate evnt)
        {
            IntPtr functionAddress;
            UObject target;
            if (GetFunctionAddress(evnt, out functionAddress, out target))
            {
                return IsTargetBound(target, NativeReflection.GetFName(functionAddress));
            }
            else
            {
                OnInvalidDelegate(evnt);
            }
            return false;
        }

        public bool IsTargetBound(UObject obj, FName functionName)
        {
            return obj == null ? false : IsTargetBound(obj.Address, functionName);
        }

        public bool IsBoundToObject(UObject obj)
        {
            return obj == null ? false : IsBoundToObject(obj.Address);
        }

        public abstract void CopyFrom(FDelegateBase<TDelegate> other);        
        public abstract void Bind(IntPtr obj, FName functionName);
        public abstract void Unbind(IntPtr obj, FName functionName);
        public abstract void Clear();
        public abstract bool IsTargetBound(IntPtr obj, FName functionName);
        public abstract bool IsBoundToObject(IntPtr obj);
        public abstract void ProcessDelegate(IntPtr parameters);

        protected IntPtr GetFunctionAddress(TDelegate evnt)
        {
            return NativeReflection.LookupTable.GetFunctionAddress(evnt as Delegate);
        }

        protected bool GetFunctionAddress(TDelegate evnt, out IntPtr functionAddress, out UObject target)
        {
            return NativeReflection.LookupTable.GetFunctionAddress(evnt as Delegate, out functionAddress, out target);
        }

        public TDelegateClass Copy<TDelegateClass>() where TDelegateClass : FDelegateBase<TDelegate>, new()
        {
            TDelegateClass result = new TDelegateClass();
            result.CopyFrom(this);
            return result;
        }
    }

    public unsafe abstract class FDelegate<TDelegate> : FDelegateBase<TDelegate> where TDelegate : class
    {
        private FScriptDelegate* scriptDelegatePtr
        {
            get { return (FScriptDelegate*)Address; }
        }

        private FScriptDelegate managedScriptDelegate;

        public override bool IsBound
        {
            get { return IsNative ? scriptDelegatePtr->IsBound : managedScriptDelegate.IsBound; }
        }

        public override void FromNative(IntPtr address)
        {
            Clear();
            FScriptDelegate del = *((FScriptDelegate*)address);

            if (IsNative)
            {
                *scriptDelegatePtr = del;
            }
            else
            {
                managedScriptDelegate = del;
            }
        }

        public override void ToNative(IntPtr address)
        {
            FScriptDelegate* del = ((FScriptDelegate*)address);
            del->Clear();

            if (IsNative)
            {
                *del = *scriptDelegatePtr;
            }
            else
            {
                *del = managedScriptDelegate;
            }
        }

        public override void CopyFrom(FDelegateBase<TDelegate> other)
        {
            Clear();
            FDelegate<TDelegate> del = other as FDelegate<TDelegate>;
            if (del == null)
            {
                return;
            }

            FScriptDelegate otherScriptDelegate = del.IsNative ? *del.scriptDelegatePtr : managedScriptDelegate;

            if (IsNative)
            {
                scriptDelegatePtr->Bind(otherScriptDelegate.Object.GetPtr(), otherScriptDelegate.FunctionName);
            }
            else
            {
                managedScriptDelegate.Bind(otherScriptDelegate.Object.GetPtr(), otherScriptDelegate.FunctionName);
            }
        }

        public override void Bind(IntPtr obj, FName functionName)
        {
            if (IsNative)
            {
                scriptDelegatePtr->Bind(obj, functionName);
            }
            else
            {
                managedScriptDelegate.Bind(obj, functionName);
            }
        }

        public override void Unbind(IntPtr obj, FName functionName)
        {
            if (IsNative)
            {
                scriptDelegatePtr->Unbind(obj, functionName);
            }
            else
            {
                managedScriptDelegate.Unbind(obj, functionName);
            }
        }

        public override void Clear()
        {
            if (IsNative)
            {
                scriptDelegatePtr->Clear();
            }
            else
            {
                managedScriptDelegate.Clear();
            }            
        }

        public override bool IsTargetBound(IntPtr obj, FName functionName)
        {
            if (IsNative)
            {
                return scriptDelegatePtr->IsTargetBound(obj, functionName);
            }
            else
            {
                return managedScriptDelegate.IsTargetBound(obj, functionName);
            }
        }

        public override bool IsBoundToObject(IntPtr obj)
        {
            if (IsNative)
            {
                return scriptDelegatePtr->IsBoundToObject(obj);
            }
            else
            {
                return managedScriptDelegate.IsBoundToObject(obj);
            }
        }

        public override void ProcessDelegate(IntPtr parameters)
        {
            if (IsNative)
            {
                scriptDelegatePtr->ProcessDelegate(parameters);
            }
            else
            {
                managedScriptDelegate.ProcessDelegate(parameters);
            }
        }

        public FScriptDelegate GetFunctionInfo()
        {
            if (IsNative)
            {
                return *scriptDelegatePtr;
            }
            else
            {
                return managedScriptDelegate;
            }
        }

        public UObject GetObject()
        {
            return GetFunctionInfo().Object.Get();
        }

        public IntPtr GetObjectAddress()
        {
            return GetFunctionInfo().Object.GetPtr();
        }

        public UFunction GetFunction()
        {
            return GetFunctionInfo().GetFunction();
        }

        public IntPtr GetFunctionAddress()
        {
            return GetFunctionInfo().GetFunctionAddress();
        }
    }

    public unsafe abstract class FMulticastDelegate<TDelegate> : FDelegateBase<TDelegate> where TDelegate : class
    {
        private FMulticastScriptDelegate* scriptDelegatePtr
        {
            get { return (FMulticastScriptDelegate*)Address; }
        }

        private FMulticastScriptDelegateWrapper managedScriptDelegate;

        public int Count
        {
            get { return IsNative ? scriptDelegatePtr->Count : managedScriptDelegate.Count; }
        }

        public override bool IsBound
        {
            get { return Count > 0; }
        }

        public override void FromNative(IntPtr address)
        {
            Clear();
            FMulticastScriptDelegate del = *((FMulticastScriptDelegate*)address);
            
            for (int i = 0; i < del.Count; ++i)
            {
                if (IsNative)
                {
                    scriptDelegatePtr->Add(del[i]);
                }
                else
                {
                    managedScriptDelegate.Add(del[i]);
                }
            }
        }

        public override void ToNative(IntPtr address)
        {
            FMulticastScriptDelegate* del = ((FMulticastScriptDelegate*)address);
            del->Clear();

            for (int i = 0; i < Count; ++i)
            {
                if (IsNative)
                {
                    del->Add(scriptDelegatePtr->Get(i));
                }
                else
                {
                    del->Add(managedScriptDelegate[i]);
                }
            }
        }

        public override void CopyFrom(FDelegateBase<TDelegate> other)
        {
            Clear();
            FMulticastDelegate<TDelegate> del = other as FMulticastDelegate<TDelegate>;
            if (del == null)
            {
                return;
            }

            if (del.IsNative)
            {
                FMulticastScriptDelegate otherScriptDelegate = *del.scriptDelegatePtr;
                int count = otherScriptDelegate.Count;
                for (int i = 0; i < count; ++i)
                {
                    if (IsNative)
                    {
                        scriptDelegatePtr->Add(otherScriptDelegate[i]);
                    }
                    else
                    {
                        managedScriptDelegate.Add(otherScriptDelegate[i]);
                    }
                }
            }
            else
            {
                FMulticastScriptDelegateWrapper otherScriptDelegate = del.managedScriptDelegate;
                int count = otherScriptDelegate.Count;
                for (int i = 0; i < count; ++i)
                {
                    if (IsNative)
                    {
                        scriptDelegatePtr->Add(otherScriptDelegate[i]);
                    }
                    else
                    {
                        managedScriptDelegate.Add(otherScriptDelegate[i]);
                    }
                }
            }
        }

        public override void Bind(IntPtr obj, FName functionName)
        {
            if (IsNative)
            {
                if (!scriptDelegatePtr->Contains(obj, functionName))
                {
                    scriptDelegatePtr->Add(new FScriptDelegate(obj, functionName));
                }
            }
            else
            {
                if (!managedScriptDelegate.Contains(obj, functionName))
                {
                    managedScriptDelegate.Add(new FScriptDelegate(obj, functionName));
                }
            }
        }

        public override void Unbind(IntPtr obj, FName functionName)
        {
            if (IsNative)
            {
                int index = scriptDelegatePtr->IndexOf(obj, functionName);
                if (index >= 0)
                {
                    scriptDelegatePtr->RemoveAt(index);
                }
            }
            else
            {
                int index = managedScriptDelegate.IndexOf(obj, functionName);
                if (index >= 0)
                {
                    managedScriptDelegate.RemoveAt(index);
                }
            }
        }

        public override void Clear()
        {
            if (IsNative)
            {
                scriptDelegatePtr->Clear();
            }
            else
            {
                managedScriptDelegate.Clear();
            }
        }

        public override bool IsTargetBound(IntPtr obj, FName functionName)
        {
            if (IsNative)
            {
                return scriptDelegatePtr->Contains(obj, functionName);
            }
            else
            {
                return managedScriptDelegate.Contains(obj, functionName);
            }
        }

        public override bool IsBoundToObject(IntPtr obj)
        {
            if (IsNative)
            {
                return scriptDelegatePtr->IsBoundToObject(obj);
            }
            else
            {
                return managedScriptDelegate.IsBoundToObject(obj);
            }
        }

        public override void ProcessDelegate(IntPtr parameters)
        {
            if (IsNative)
            {
                scriptDelegatePtr->ProcessMulticastDelegate(parameters);
            }
            else
            {
                managedScriptDelegate.ProcessMulticastDelegate(parameters);
            }
        }
        
        public FScriptDelegate[] GetFunctionInfos()
        {
            FScriptDelegate[] result = new FScriptDelegate[Count];
            if (IsNative)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = (*scriptDelegatePtr)[i];
                }
            }
            else
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = managedScriptDelegate[i];
                }
            }
            return result;
        }

        public UObject[] GetObjects()
        {
            UObject[] result = new UObject[Count];
            if (IsNative)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = (*scriptDelegatePtr)[i].Object.Get();
                }
            }
            else
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = managedScriptDelegate[i].Object.Get();
                }
            }
            return result;
        }

        public IntPtr[] GetObjectAddresses()
        {
            IntPtr[] result = new IntPtr[Count];
            if (IsNative)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = (*scriptDelegatePtr)[i].Object.GetPtr();
                }
            }
            else
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = managedScriptDelegate[i].Object.GetPtr();
                }
            }
            return result;
        }

        public UFunction[] GetFunctions()
        {
            UFunction[] result = new UFunction[Count];
            if (IsNative)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = (*scriptDelegatePtr)[i].GetFunction();
                }
            }
            else
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = managedScriptDelegate[i].GetFunction();
                }
            }
            return result;
        }
        
        public IntPtr[] GetFunctionAddresses()
        {
            IntPtr[] result = new IntPtr[Count];
            if (IsNative)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = (*scriptDelegatePtr)[i].GetFunctionAddress();
                }
            }
            else
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = managedScriptDelegate[i].GetFunctionAddress();
                }
            }
            return result;
        }
    }
}
