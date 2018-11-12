using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public interface INativeDelegate
    {
        /// <summary>
        /// Called on hotreload unload
        /// </summary>
        void OnUnload();
    }

    public class NativeDelegate<TNativeDelegate, TRegisterNativeDelegate, TManagedDelegate> : INativeDelegate
        where TNativeDelegate : class
        where TRegisterNativeDelegate : class
        where TManagedDelegate : class
    {        
        private Dictionary<TManagedDelegate, MethodInfo> boundEvents = new Dictionary<TManagedDelegate, MethodInfo>();
        private Dictionary<MethodInfo, TManagedDelegate> boundMethods = new Dictionary<MethodInfo, TManagedDelegate>();
        private bool registeredNativeCallback = false;
        private TNativeDelegate nativeCallback;
        protected NativeDelegateHandle<TManagedDelegate> managed;

        // The static native function delegate e.g. Native_FCoreUObjectDelegates.Reg_PostGarbageCollect
        private TRegisterNativeDelegate registerNativeDelegate;

        // Define a wrapper delegate which just calls registerNativeDelegate (we do this as we don't have access to a valid
        // delegate signature which we can call directly - so we define one here and make registerNativeDelegate the target)
        private delegate void RegisterNativeMulticastDelegateWrapper(IntPtr instance, TNativeDelegate handler, ref FDelegateHandle handle, csbool enable);
        private delegate void RegisterNativeDelegateWrapper(TNativeDelegate handler, csbool enable);
        private RegisterNativeMulticastDelegateWrapper registerNativeMulticastDelegateWrapper;
        private RegisterNativeDelegateWrapper registerNativeDelegateWrapper;

        public IntPtr TargetAddress { get; set; }

        public virtual bool IsMulticast
        {
            get { return false; }
        }

        public NativeDelegate(IntPtr targetObjAddress)
            : this()
        {
            TargetAddress = targetObjAddress;
        }

        public NativeDelegate()
        {
            UpdateNativeRegistrar();
            UpdateNativeCallback();
        }

        private void UpdateNativeRegistrar()
        {
            if (registerNativeDelegate != null)
            {
                return;
            }

            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public;
            FieldInfo[] fields = typeof(TRegisterNativeDelegate).DeclaringType.GetFields(bindingFlags);
            foreach (FieldInfo field in fields)
            {
                if (field.IsStatic && field.FieldType == typeof(TRegisterNativeDelegate))
                {
                    registerNativeDelegate = field.GetValue(null) as TRegisterNativeDelegate;
                    if (registerNativeDelegate != null)
                    {
                        Delegate del = registerNativeDelegate as Delegate;

                        // Mono / .NET Framework have slightly different requirements for what we are doing with CreateDelegate
                        // - Mono requires firstArg to be null to create a valid delegate
                        // - NET Framework requires the del as context otherwise when the delegate is called an exception is thrown
                        object firstArg = null;
                        if (!AssemblyContext.IsMono)
                        {
                            firstArg = del;
                        }

                        if (IsMulticast)
                        {
                            registerNativeMulticastDelegateWrapper = (RegisterNativeMulticastDelegateWrapper)
                                Delegate.CreateDelegate(typeof(RegisterNativeMulticastDelegateWrapper),
                                    firstArg, del.Method);
                        }
                        else
                        {
                            registerNativeDelegateWrapper = (RegisterNativeDelegateWrapper)
                                Delegate.CreateDelegate(typeof(RegisterNativeDelegateWrapper),
                                    firstArg, del.Method);
                        }
                    }
                    else
                    {
                        FMessage.Log(ELogVerbosity.Error, "Failed to find native delegate '" +
                            typeof(TRegisterNativeDelegate).FullName + "'. Did you call REGISTER_FUNC()?");
                    }
                    break;
                }
            }
        }

        private void UpdateNativeCallback()
        {
            if (nativeCallback != null)
            {
                return;
            }

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo method = null;
            Type type = GetType();
            while (type != null)
            {
                method = type.GetMethod("NativeCallback", bindingFlags);
                if (method != null)
                {
                    break;
                }
                type = type.BaseType;
            }
            
            if (method != null)
            {
                nativeCallback = Delegate.CreateDelegate(typeof(TNativeDelegate), this, method) as TNativeDelegate;
            }
        }

        public void Bind(TManagedDelegate evnt)
        {
            Delegate del = evnt as Delegate;
            if (del == null)
            {
                return;
            }

            MethodInfo method = del.Method;

            if (!registeredNativeCallback)
            {
                if (IsMulticast)
                {
                    if (registerNativeMulticastDelegateWrapper != null && nativeCallback != null)
                    {
                        registerNativeMulticastDelegateWrapper(TargetAddress, nativeCallback, ref managed.Handle, true);
                    }
                }
                else
                {
                    if (registerNativeDelegateWrapper != null && nativeCallback != null)
                    {
                        registerNativeDelegateWrapper(nativeCallback, true);
                    }
                }
                registeredNativeCallback = true;
            }

            if (IsBound(evnt))
            {
                return;
            }

            if (IsMulticast)
            {
                managed.Delegate = Delegate.Combine(managed.Delegate as Delegate, del) as TManagedDelegate;                
            }
            else
            {
                boundEvents.Clear();
                boundMethods.Clear();
                managed.Delegate = del as TManagedDelegate;
            }

            boundEvents[evnt] = method;
            boundMethods[method] = evnt;
        }

        public void Unbind(TManagedDelegate evnt)
        {
            Delegate del = evnt as Delegate;
            if (del == null || del.Method == null)
            {
                return;
            }

            MethodInfo method;
            if (!boundEvents.TryGetValue(evnt, out method))
            {
                TManagedDelegate existingEvnt;
                if (boundMethods.TryGetValue(del.Method, out existingEvnt))
                {
                    method = del.Method;
                }
            }

            if (method != null)
            {
                boundEvents.Remove(evnt);
                boundMethods.Remove(method);

                managed.Delegate = Delegate.Remove(managed.Delegate as Delegate, evnt as Delegate) as TManagedDelegate;
                if (managed.Delegate == null && registeredNativeCallback)
                {
                    if (IsMulticast)
                    {
                        if (registerNativeMulticastDelegateWrapper != null && nativeCallback != null)
                        {
                            registerNativeMulticastDelegateWrapper(TargetAddress, nativeCallback, ref managed.Handle, false);
                        }
                    }
                    else
                    {
                        if (registerNativeDelegateWrapper != null && nativeCallback != null)
                        {
                            registerNativeDelegateWrapper(nativeCallback, false);
                        }
                    }
                    registeredNativeCallback = false;
                }
            }
        }
        
        public void OnUnload()
        {
            // Flag this class as unavailable to create new binds?
            UnbindAll();
        }

        public void UnbindAll()
        {
            while (boundEvents.Count > 0)
            {
                using (var enumerator = boundEvents.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        Unbind(enumerator.Current.Key);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public bool IsBound(TManagedDelegate evnt)
        {
            Delegate del = evnt as Delegate;
            if (del == null)
            {
                return false;
            }
            MethodInfo method = del.Method;
            return boundEvents.ContainsKey(evnt) || method != null && boundMethods.ContainsKey(method);
        }
    }

    public class NativeMulticastDelegate<TNativeDelegate, TRegisterNativeDelegate, TManagedDelegate> :
        NativeDelegate<TNativeDelegate, TRegisterNativeDelegate, TManagedDelegate>
        where TNativeDelegate : class
        where TRegisterNativeDelegate : class
        where TManagedDelegate : class
    {
        public override bool IsMulticast
        {
            get { return true; }
        }
    }

    public class NativeSimpleDelegate<TRegisterNativeDelegate> :
        NativeDelegate<FSimpleDelegate, TRegisterNativeDelegate, FSimpleDelegate>
        where TRegisterNativeDelegate : class
    {
        private void NativeCallback()
        {
            var evnt = managed.Delegate;
            if (evnt != null)
            {
                evnt();
            }
        }
    }

    public class NativeSimpleMulticastDelegate<TRegisterNativeDelegate> :
        NativeMulticastDelegate<FSimpleMulticastDelegate, TRegisterNativeDelegate, FSimpleMulticastDelegate>
        where TRegisterNativeDelegate : class
    {
        private void NativeCallback()
        {
            var evnt = managed.Delegate;
            if (evnt != null)
            {
                evnt();
            }
        }
    }

    public struct NativeDelegateHandle<T>
    {
        public T Delegate;
        public FDelegateHandle Handle;
    }

    /// <summary>
    /// Struct representing an handle to a delegate.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FDelegateHandle : IEquatable<FDelegateHandle>
    {
        public ulong ID;

        public bool IsValid
        {
            get { return ID != 0; }
        }

        public void Reset()
        {
            ID = 0;
        }

        public static FDelegateHandle New()
        {
            FDelegateHandle result = default(FDelegateHandle);
            Native_FDelegateHandle.GenerateNewHandle(ref result);
            return result;
        }

        public static bool operator ==(FDelegateHandle a, FDelegateHandle b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FDelegateHandle a, FDelegateHandle b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FDelegateHandle)
            {
                return Equals((FDelegateHandle)obj);
            }
            return false;
        }

        public bool Equals(FDelegateHandle other)
        {
            return ID == other.ID;
        }

        // Use the C++ GetTypeHash instead to stay in sync with the ue4 hashcode?
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }

    public delegate void FSimpleDelegate();
    public delegate void FSimpleMulticastDelegate();
}
