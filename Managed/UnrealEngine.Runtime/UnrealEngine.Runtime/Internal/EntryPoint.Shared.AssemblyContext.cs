using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // This AssemblyContext stuff is kind of a limited fragile mess. TODO: Recode this.
    // - Move into its own project
    // - Remove unnecessary Proxy class
    // - Remove IL code gen, use #if directives instead
    // - Remove AssemblyContextRef, just work with AssemblyContext objects directly (though they are still useful for passing between appdomains)

    public partial class AssemblyContext
    {
        private static long nextContextId = 2;// 1 is reserved for the "main" context which we will only ever refer to by id
        private static Dictionary<long, AssemblyContext> contexts;

        /// <summary>
        /// Our runtime generated AssemblyLoadContext type
        /// </summary>
        private static Type generatedContextType;
        /// <summary>
        /// True if the currently executing code is under Mono runtime
        /// </summary>
        public static readonly bool IsMono;
        /// <summary>
        /// True if the currently executing code is under CoreCLR (.NET Core)
        /// </summary>
        public static readonly bool IsCoreCLR;
        /// <summary>
        /// True if the currently executing code is under CLR (.NET Framework)
        /// </summary>
        public static readonly bool IsCLR;

        /// <summary>
        /// Use AppDomain GetData/SetData to store the current AppDomain AssemblyContextRef on non-CoreCLR runtimes
        /// </summary>
        public const string CurrentAppDomainContextRefKey = "CurrentAppDomainContextRef";

        static AssemblyContext()
        {
            if (Type.GetType("Mono.Runtime") != null)
            {
                IsMono = true;
            }
            // AssemblyLoadContext is only in .NET Core
            else if (Type.GetType("System.Runtime.Loader.AssemblyLoadContext") != null)
            {
                IsCoreCLR = true;
            }
            else
            {
                IsCLR = true;
            }
        }

        public static void Initialize()
        {
            contexts = new Dictionary<long, AssemblyContext>();
            if (IsCoreCLR)
            {
                CreateAssemblyLoadContextType();
            }
            AssemblyContextProxy.Initialize(true);
        }

        public static void Initialize(AssemblyContextRef currentContext)
        {
            if (contexts == null)
            {
                AssemblyContextProxy.Initialize(false);
            }

            if (!IsCoreCLR)
            {
                AppDomain.CurrentDomain.SetData(CurrentAppDomainContextRefKey, currentContext.Format());
            }
        }

        internal static AssemblyContext InternalGetContext(AssemblyContextRef contextRef)
        {
            if (contexts == null)
            {
                throw new Exception("Attempting to get context on an AssemblyContext which doesn't maintain a list of contexts");
            }

            lock (contexts)
            {
                AssemblyContext context;
                if (contexts.TryGetValue(contextRef.Id, out context))
                {
                    return context;
                }
            }
            throw new Exception("Unknown AssemblyContextHelper contextId: " + contextRef.Id);
        }

        public static AssemblyContextRef GetContextRef(Assembly assembly)
        {
            if (IsCoreCLR)
            {
                if (contexts != null)
                {
                    IAssemblyLoadContext context = MethodRedirects.GetLoadContextInternal(assembly);
                    if (context != null)
                    {
                        AssemblyContext owner = context.GetOwner();
                        if (owner != null)
                        {
                            return owner.Reference;
                        }
                    }
                    return AssemblyContextRef.Invalid;
                }
                else
                {
                    return AssemblyContextProxy.GetContextRef(assembly);
                }
            }
            else
            {
                return AssemblyContextProxy.GetContextRef(assembly);
            }
        }

        public static AssemblyContextRef Create()
        {
            return Create(new AssemblyContextRef(1, 1));
        }

        public static AssemblyContextRef Create(AssemblyContextRef currentContext)
        {
            if (contexts == null)
            {
                Debug.Assert(false, "TODO: Make sure our logic allows for contexts to create contexts");
                return AssemblyContextProxy.Create(currentContext);
            }

            lock (contexts)
            {
                if (!IsCoreCLR)
                {
                    throw new NotImplementedException("Use the overload taking an AppDomain parameter on runtimes which support AppDomain loading/unloading");
                }

                AssemblyContextRef contextRef = new AssemblyContextRef(nextContextId, currentContext.Id);
                IAssemblyLoadContext loadContext = (IAssemblyLoadContext)Activator.CreateInstance(generatedContextType, contextRef.Id);
                AssemblyContext context = new AssemblyContext(loadContext, contextRef);
                contexts.Add(contextRef.Id, context);
                nextContextId++;
                return contextRef;
            }
        }

        public static AssemblyContextRef Create(AppDomain domain)
        {
            return Create(domain, new AssemblyContextRef(1, 1));
        }

        public static AssemblyContextRef Create(AppDomain domain, AssemblyContextRef currentContext)
        {
            if (contexts == null)
            {
                throw new NotImplementedException("Already within a context. TODO: Allow an AppDomain to have sub domains?");
            }

            lock (contexts)
            {
                if (IsCoreCLR)
                {
                    throw new NotImplementedException("Use the overload taking no parameters on runtimes which don't support AppDomain loading/unloading");
                }

                AssemblyContextRef contextRef = new AssemblyContextRef(nextContextId, currentContext.Id);
                AssemblyContext context = new AssemblyContext(domain, contextRef);
                contexts.Add(context.Id, context);
                nextContextId++;
                return contextRef;
            }
        }

        private static void RemoveContext(AssemblyContext context)
        {
            lock (contexts)
            {
                contexts.Remove(context.Id);
            }
        }
    }

    public partial class AssemblyContext
    {
        /// <summary>
        /// Our implementation of <see cref="System.Runtime.Loader.AssemblyLoadContext"/> which manages the assemblies.
        /// This should only be used on .NET Core, otherwise use <see cref="Domain"/>.
        /// </summary>
        public IAssemblyLoadContext LoadContext { get; private set; }

        /// <summary>
        /// <see cref="AppDomain"/> which manages the assemblies.
        /// On .NET Core this will be null and <see cref="LoadContext"/> should be used instead.
        /// </summary>
        public AppDomain Domain { get; private set; }

        /// <summary>
        /// A reference structure holding both the ID of this context and also the ID of the context which owns this context.
        /// </summary>
        public AssemblyContextRef Reference { get; private set; }

        /// <summary>
        /// The ID of this context used to track context lifetime
        /// </summary>
        public long Id
        {
            get { return Reference.Id; }
        }

        /// <summary>
        /// The ID of the owning context. If this context is the main app domain main context this will be the same as <see cref="Id"/>.
        /// </summary>
        public long OwnerId
        {
            get { return Reference.OwnerId; }
        }

        /// <summary>
        /// True if this context has been unloaded and no further operations should be executed
        /// </summary>
        public bool IsUnloaded
        {
            get { return State == AssemblyContextState.Unloaded; }
        }

        /// <summary>
        /// The state of this context (alive / unloading / unloaded)
        /// </summary>
        public AssemblyContextState State { get; private set; }

        private WeakReference weakRef;
        /// <summary>
        /// A weak reference pointing to the AssemblyLoadContext has been destroyed.
        /// The associated assemblies should have been fully unloaded (a call to GC.Collect may be required before this happens).
        /// </summary>
        public bool IsFullyUnloaded
        {
            get { return IsUnloaded && (weakRef == null || !weakRef.IsAlive); }
        }

        /// <summary>
        /// Used to handle custom unloading. This is only used under .NET Core.
        /// Subscribe to AppMain.DomainUnload for other use cases.<para/>
        /// </summary>
        public event Action<KeyValuePair<long, long>> Unloading;

        /// <summary>
        /// Used to handle custom resolving. This is only used under .NET Core.
        /// Subscribe to AppDomain.AssemblyResolve for other use cases.<para/>
        /// </summary>
        public event Func<KeyValuePair<long, long>, AssemblyName, Assembly> Resolving;

        public AssemblyContext(IAssemblyLoadContext loadContext, AssemblyContextRef reference)
        {
            LoadContext = loadContext;
            weakRef = new WeakReference(LoadContext);
            Reference = reference;
        }

        public AssemblyContext(AppDomain domain, AssemblyContextRef reference)
        {
            Domain = domain;
            weakRef = new WeakReference(Domain);
            Reference = reference;
        }

        /// <summary>
        /// Clear the events as they may keep an assembly alive
        /// </summary>
        private void ClearEvents()
        {
            Unloading = null;
            Resolving = null;
        }

        internal void OnUnloading()
        {
            Debug.Assert(IsCoreCLR, "This method is only used by under .NET Core. Subscribe to AppDomain.DomainUnload.");

            Debug.Assert(!IsUnloaded, "The AssemblyContext is already unloaded (should be be either Unloading or Alive)");
            bool isAlive = State == AssemblyContextState.Alive;

            if (Unloading != null)
            {
                Unloading(Reference);
            }

            if (IsCoreCLR)
            {
                EnsureUnsubscribed();
            }

            if (isAlive)
            {
                Debug.Assert(IsCoreCLR, "Something else called the unload of our context. This should only happen on CoreCLR.");
                Debug.Assert(LoadContext != null && Domain == null);

                LoadContext = null;
                RemoveContext(this);
                ClearEvents();
                State = AssemblyContextState.Unloaded;
            }
        }

        internal Assembly OnResolving(AssemblyName assemblyName)
        {
            Debug.Assert(IsCoreCLR, "This method is only used by under .NET Core. Subscribe to AppDomain.Resolving.");
            if (Resolving != null)
            {
                return Resolving(Reference, assemblyName);
            }
            return null;
        }

        /// <summary>
        /// .NET Core assemblies will not unload if there are events held which pull themselves into other assemblies which don't unload.
        /// Make sure the user unsubscribed from a few key event handlers which will definitely keep the assembly alive.
        /// </summary>
        [Conditional("DEBUG")]
        private void EnsureUnsubscribed()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // NOTE: Under .NET Core AppDomain.CurrentDomain.GetType() will point to System.Runtime.Extensions.AppDomain
            object currentDomain = Type.GetType("System.AppDomain").GetProperty("CurrentDomain").GetValue(null);
            EnsureUnsubscribed(currentDomain, "AssemblyLoad", "AssemblyLoad");
            EnsureUnsubscribed(currentDomain, "AssemblyResolve", "_AssemblyResolve");
            EnsureUnsubscribed(currentDomain, "DomainUnload", "_domainUnload");
            EnsureUnsubscribed(currentDomain, "FirstChanceException", "_firstChanceException");
            EnsureUnsubscribed(currentDomain, "ProcessExit", "_processExit");
            //EnsureUnsubscribed(currentDomain, "ReflectionOnlyAssemblyResolve", ???);
            EnsureUnsubscribed(currentDomain, "ResourceResolve", "_ResourceResolve");
            EnsureUnsubscribed(currentDomain, "TypeResolve", "_TypeResolve");
            EnsureUnsubscribed(currentDomain, "UnhandledException", "_unhandledException");

            stopwatch.Stop();
            SharedRuntimeState.Log("EnsureUnsubscribed took: " + stopwatch.Elapsed);
        }

        private void EnsureUnsubscribed<T>(string eventName, string fieldName)
        {
            EnsureUnsubscribed(null, typeof(T), eventName, fieldName);
        }

        private void EnsureUnsubscribed(object obj, string eventName, string fieldName)
        {
            if (obj != null)
            {
                EnsureUnsubscribed(obj, obj.GetType(), eventName, fieldName);
            }
        }

        private void EnsureUnsubscribed(object obj, Type type, string eventName, string fieldName)
        {
            FieldInfo fieldInfo;
            Delegate evnt = GetDelegate(obj, type, eventName, fieldName, out fieldInfo);
            if (evnt != null)
            {
                Delegate[] invocationList = evnt.GetInvocationList();

                foreach (Delegate del in invocationList)
                {
                    Assembly assembly = del.Method.DeclaringType.Assembly;
                    AssemblyContextRef contextRef = AssemblyContext.GetContextRef(assembly);

                    if (contextRef == this.Reference)
                    {
                        evnt = Delegate.Remove(evnt, del);
                        SharedRuntimeState.LogWarning("Event is still being subscribed to in an assembly which is unloading. " +
                            "Assembly: " + assembly.FullName + " event: " + type.Name + "." + eventName + " target: " +
                            del.Method.DeclaringType.FullName + "." + del.Method.Name);
                    }
                }

                fieldInfo.SetValue(obj, evnt);
            }
        }

        private static Delegate GetDelegate(object obj, Type type, string eventName, string fieldName)
        {
            FieldInfo fieldInfo;
            return GetDelegate(obj, type, eventName, fieldName, out fieldInfo);
        }

        private static Delegate GetDelegate(object obj, Type type, string eventName, string fieldName, out FieldInfo fieldInfo)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            EventInfo eventInfo = type.GetEvent(eventName, bindingFlags);
            fieldInfo = type.GetField(fieldName, bindingFlags);
            return fieldInfo.GetValue(obj) as Delegate;
        }

        public void Unload()
        {
            EnsureAlive();
            State = AssemblyContextState.Unloading;
            if (Domain != null)
            {
                try
                {
                    AppDomain.Unload(Domain);
                }
                catch
                {
                    // Allow Unload to be called multiple times until unload is successful
                    State = AssemblyContextState.Alive;
                    throw;
                }
                Domain = null;
            }
            if (LoadContext != null)
            {
                // It is important that we don't have any more references to the AssemblyLoadContext so that it
                // can be properly cleaned up
                LoadContext.Unload();
                LoadContext = null;
            }
            RemoveContext(this);
            ClearEvents();
            State = AssemblyContextState.Unloaded;
        }

        public Assembly[] GetAssemblies()
        {
            EnsureAlive();
            if (LoadContext != null)
            {
                return LoadContext.GetAssemblies();
            }
            else if (Domain != null)
            {
                return Domain.GetAssemblies();
            }
            return null;
        }

        public Assembly LoadFrom(string assemblyPath)
        {
            EnsureAlive();
            if (LoadContext != null)
            {
                return LoadContext.LoadFromAssemblyPath(assemblyPath);
            }
            else if (Domain != null)
            {
                return Assembly.LoadFrom(assemblyPath);
            }
            return null;
        }

        public Assembly LoadFromStream(Stream assembly, Stream assemblySymbols)
        {
            EnsureAlive();
            if (LoadContext != null)
            {
                return LoadContext.LoadFromStream(assembly, assemblySymbols);
            }
            else if (Domain != null)
            {
                throw new NotImplementedException("AppDomain doesn't have LoadFrom functions taking a stream / byte array. Use AppDomain.Load instead.");
            }
            return null;
        }

        private void EnsureAlive()
        {
            if (State != AssemblyContextState.Alive)
            {
                throw new InvalidOperationException("Trying to access unloaded AssemblyContext");
            }
        }
    }

    /// <summary>
    /// Used for working with contexts by id so that contexts can be passed around where there are
    /// duplicate versions of AssemblyContext classes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AssemblyContextRef : IEquatable<AssemblyContextRef>
    {
        /// <summary>
        /// The context ID
        /// </summary>
        public long Id;

        /// <summary>
        /// The context ID which owns this context ID
        /// </summary>
        public long OwnerId;

        public bool IsInvalid
        {
            get { return Id == 0 || OwnerId == 0; }
        }

        public static readonly AssemblyContextRef Invalid = new AssemblyContextRef(0, 0);

        public AssemblyContextRef(long id, long ownerId)
        {
            Id = id;
            OwnerId = ownerId;
        }

        public void Unload()
        {
            if (AssemblyContext.IsCoreCLR)
            {
                AssemblyContextProxy.Unload(this);
            }
            else
            {
                Debug.Assert(AppDomain.CurrentDomain.IsDefaultAppDomain(),
                    "Unload can only be called from the default AppDomain");
                AssemblyContext.InternalGetContext(this).Unload();
            }
        }

        /// <summary>
        /// Used for checking to see if the underlying context is still alive (AssemblyLoadContext / AppDomain).
        /// Only call this from within the assembly which loaded this
        /// </summary>
        public WeakReference GetWeakReference()
        {
            AssemblyContext context = AssemblyContext.InternalGetContext(this);
            if (context != null)
            {
                object target = null;
                if (AssemblyContext.IsCoreCLR)
                {
                    target = context.LoadContext;
                }
                else
                {
                    target = context.Domain;
                }
                if (target != null)
                {
                    return new WeakReference(target);
                }
            }
            return null;
        }

        public Assembly[] GetAssemblies()
        {
            if (AssemblyContext.IsCoreCLR)
            {
                return AssemblyContextProxy.GetAssemblies(this);
            }
            else
            {
                return AppDomain.CurrentDomain.GetAssemblies();
            }
        }

        public Assembly LoadFrom(string assemblyPath)
        {
            if (AssemblyContext.IsCoreCLR)
            {
                return AssemblyContextProxy.LoadFrom(this, assemblyPath);
            }
            else
            {
                return Assembly.LoadFrom(assemblyPath);
            }
        }

        public Assembly LoadFromStream(Stream assembly)
        {
            return LoadFromStream(assembly, null);
        }

        public Assembly LoadFromStream(Stream assembly, Stream assemblySymbols)
        {
            if (AssemblyContext.IsCoreCLR)
            {
                return AssemblyContextProxy.LoadFromStream(this, assembly, assemblySymbols);
            }
            else
            {
                throw new NotSupportedException("LoadFrom doesn't support byte arrays / streams. Use Assembly.Load instead.");
            }
        }

        public void DoCallBack(CrossAssemblyContextDelegate callBackDelegate)
        {
            if (AssemblyContext.IsCoreCLR)
            {
                // Just call the method directly as there are no app domains
                callBackDelegate();
            }
            else
            {
                // Seperate method to avoid issues with .NET Core
                DoCallBackAppDomain(callBackDelegate);
            }
        }

        private void DoCallBackAppDomain(CrossAssemblyContextDelegate callBackDelegate)
        {
            AssemblyContext.InternalGetContext(this).Domain.DoCallBack(new CrossAppDomainDelegate(callBackDelegate));
        }

        public string Format()
        {
            return "I:" + Id + " O:" + OwnerId;
        }

        public static implicit operator KeyValuePair<long, long>(AssemblyContextRef contextRef)
        {
            return new KeyValuePair<long, long>(contextRef.Id, contextRef.OwnerId);
        }

        public static implicit operator AssemblyContextRef(KeyValuePair<long, long> pair)
        {
            return new AssemblyContextRef(pair.Key, pair.Value);
        }

        public KeyValuePair<long, long> ToPair()
        {
            return new KeyValuePair<long, long>(Id, OwnerId);
        }

        public static AssemblyContextRef FromPair(KeyValuePair<long, long> pair)
        {
            return new AssemblyContextRef(pair.Key, pair.Value);
        }

        public static AssemblyContextRef Parse(string str)
        {
            if (str != null)
            {
                string[] splitted = str.Split(' ');
                long id, owner;
                if (splitted.Length >= 2 && splitted[0].StartsWith("I:") && splitted[1].StartsWith("O:") &&
                    long.TryParse(splitted[0].Substring(2), out id) && long.TryParse(splitted[1].Substring(2), out owner))
                {
                    return new AssemblyContextRef(id, owner);
                }
            }
            throw new FormatException("Badly formated AssemblyContextRef string '" + str + "'");
        }

        public static bool operator ==(AssemblyContextRef a, AssemblyContextRef b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(AssemblyContextRef a, AssemblyContextRef b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is AssemblyContextRef)
            {
                return Equals((AssemblyContextRef)obj);
            }
            return false;
        }

        public bool Equals(AssemblyContextRef other)
        {
            return Id == other.Id && OwnerId == other.OwnerId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ OwnerId.GetHashCode();
            }
        }
    }

    /// <summary>
    /// Helper for working with AssemblyContext functions where there are duplicate copies of the AssemblyContext class
    /// loaded into different assemblies. This wouldn't be needed if we were doing things a little less stupid (creating
    /// a seperate project which Loader/UnrealEngine.Runtime reference rather than having this in shared .cs file).
    /// </summary>
    static class AssemblyContextProxy
    {
        // These delegates are used on CoreCLR only!
        delegate bool IsFullyUnloadedDel(KeyValuePair<long, long> contextRef);
        private static IsFullyUnloadedDel internalIsFullyUnloaded;

        delegate int GetContextStateDel(KeyValuePair<long, long> contextRef);
        private static GetContextStateDel internalGetContextState;

        delegate void UnloadDel(KeyValuePair<long, long> contextRef);
        private static UnloadDel internalUnload;

        delegate Assembly[] GetAssembliesDel(KeyValuePair<long, long> contextRef);
        private static GetAssembliesDel internalGetAssemblies;

        delegate Assembly LoadFromDel(KeyValuePair<long, long> contextRef, string assemblyPath);
        private static LoadFromDel internalLoadFrom;

        delegate Assembly LoadFromStreamDel(KeyValuePair<long, long> contextRef, Stream assembly, Stream assemblySymbols);
        private static LoadFromStreamDel internalLoadFromStream;

        delegate KeyValuePair<long, long> GetContextRefDel(Assembly assembly);
        private static GetContextRefDel internalGetContextRef;

        delegate KeyValuePair<long, long> CreateDel(KeyValuePair<long, long> currentContextRef);
        private static CreateDel internalCreate;

        delegate void AddUnloadingEventDel(KeyValuePair<long, long> contextRef, Action<KeyValuePair<long, long>> callback);
        private static AddUnloadingEventDel internalAddUnloadingEvent;

        delegate void RemoveUnloadingEventDel(KeyValuePair<long, long> contextRef, Action<KeyValuePair<long, long>> callback);
        private static RemoveUnloadingEventDel internalRemoveUnloadingEvent;

        delegate void AddResolvingEventDel(KeyValuePair<long, long> contextRef, Func<KeyValuePair<long, long>, AssemblyName, Assembly> callback);
        private static AddResolvingEventDel internalAddResolvingEvent;

        delegate void RemoveResolvingEventDel(KeyValuePair<long, long> contextRef, Func<KeyValuePair<long, long>, AssemblyName, Assembly> callback);
        private static RemoveResolvingEventDel internalRemoveResolvingEvent;

        public static void Initialize(bool isContextMaintainer)
        {
            unsafe
            {
                if (AssemblyContext.IsCoreCLR)// AssemblyContextProxyCoreCLR
                {
                    if (isContextMaintainer)
                    {
                        object[] values =
                        {
                            internalIsFullyUnloaded = InternalIsFullyUnloaded,
                            internalGetContextState = InternalGetContextState,
                            internalUnload = InternalUnload,
                            internalGetAssemblies = InternalGetAssemblies,
                            internalLoadFrom = InternalLoadFrom,
                            internalLoadFromStream = InternalLoadFromStream,
                            internalGetContextRef = InternalGetContextRef,
                            internalCreate = InternalCreate,
                            internalAddUnloadingEvent = InternalAddUnloadingEvent,
                            internalRemoveUnloadingEvent = InternalRemoveUnloadingEvent,
                            internalAddResolvingEvent = InternalAddResolvingEvent,
                            internalRemoveResolvingEvent = InternalRemoveResolvingEvent
                        };
                        AppDomain.CurrentDomain.SetData("AssemblyContextProxyCoreCLR", values);
                    }
                    else
                    {
                        object[] values = AppDomain.CurrentDomain.GetData("AssemblyContextProxyCoreCLR") as object[];
                        internalIsFullyUnloaded = (IsFullyUnloadedDel)Delegate.CreateDelegate(typeof(IsFullyUnloadedDel), (values[0] as Delegate).Method);
                        internalGetContextState = (GetContextStateDel)Delegate.CreateDelegate(typeof(GetContextStateDel), (values[1] as Delegate).Method);
                        internalUnload = (UnloadDel)Delegate.CreateDelegate(typeof(UnloadDel), (values[2] as Delegate).Method);
                        internalGetAssemblies = (GetAssembliesDel)Delegate.CreateDelegate(typeof(GetAssembliesDel), (values[3] as Delegate).Method);
                        internalLoadFrom = (LoadFromDel)Delegate.CreateDelegate(typeof(LoadFromDel), (values[4] as Delegate).Method);
                        internalLoadFromStream = (LoadFromStreamDel)Delegate.CreateDelegate(typeof(LoadFromStreamDel), (values[5] as Delegate).Method);
                        internalGetContextRef = (GetContextRefDel)Delegate.CreateDelegate(typeof(GetContextRefDel), (values[6] as Delegate).Method);
                        internalCreate = (CreateDel)Delegate.CreateDelegate(typeof(CreateDel), (values[7] as Delegate).Method);
                        internalAddUnloadingEvent = (AddUnloadingEventDel)Delegate.CreateDelegate(typeof(AddUnloadingEventDel), (values[8] as Delegate).Method);
                        internalRemoveUnloadingEvent = (RemoveUnloadingEventDel)Delegate.CreateDelegate(typeof(RemoveUnloadingEventDel), (values[9] as Delegate).Method);
                        internalAddResolvingEvent = (AddResolvingEventDel)Delegate.CreateDelegate(typeof(AddResolvingEventDel), (values[10] as Delegate).Method);
                        internalRemoveResolvingEvent = (RemoveResolvingEventDel)Delegate.CreateDelegate(typeof(RemoveResolvingEventDel), (values[11] as Delegate).Method);
                    }
                }
            }
        }

        public static bool IsFullyUnloaded(AssemblyContextRef contextRef)
        {
            return internalIsFullyUnloaded(contextRef);
        }

        public static int GetContextState(AssemblyContextRef contextRef)
        {
            return internalGetContextState(contextRef);
        }

        public static void Unload(AssemblyContextRef contextRef)
        {
            Debug.Assert(AssemblyContext.IsCoreCLR);
            internalUnload(contextRef);
        }

        public static Assembly[] GetAssemblies(AssemblyContextRef contextRef)
        {
            Debug.Assert(AssemblyContext.IsCoreCLR);
            return internalGetAssemblies(contextRef);
        }

        public static Assembly LoadFrom(AssemblyContextRef contextRef, string assemblyPath)
        {
            Debug.Assert(AssemblyContext.IsCoreCLR);
            return internalLoadFrom(contextRef, assemblyPath);
        }

        public static Assembly LoadFromStream(AssemblyContextRef contextRef, Stream assembly, Stream assemblySymbols)
        {
            Debug.Assert(AssemblyContext.IsCoreCLR);
            return internalLoadFromStream(contextRef, assembly, assemblySymbols);
        }

        public static AssemblyContextRef GetContextRef(Assembly assembly)
        {
            if (AssemblyContext.IsCoreCLR)
            {
                return internalGetContextRef(assembly);
            }
            else
            {
                // NOTE: We use a string instead of the struct as we include the .cs file in each project seperately
                return AssemblyContextRef.Parse(AppDomain.CurrentDomain.GetData(AssemblyContext.CurrentAppDomainContextRefKey) as string);
            }
        }

        public static AssemblyContextRef Create(AssemblyContextRef currentContext)
        {
            Debug.Assert(AssemblyContext.IsCoreCLR);
            return internalCreate(currentContext);
        }

        public static void AddUnloadingEvent(KeyValuePair<long, long> contextRef, Action<KeyValuePair<long, long>> callback)
        {
            Debug.Assert(AssemblyContext.IsCoreCLR);
            internalAddUnloadingEvent(contextRef, callback);
        }

        public static void RemoveUnloadingEvent(KeyValuePair<long, long> contextRef, Action<KeyValuePair<long, long>> callback)
        {
            Debug.Assert(AssemblyContext.IsCoreCLR);
            internalRemoveUnloadingEvent(contextRef, callback);
        }

        public static void AddUnloadingResolving(KeyValuePair<long, long> contextRef, Func<KeyValuePair<long, long>, AssemblyName, Assembly> callback)
        {
            Debug.Assert(AssemblyContext.IsCoreCLR);
            internalAddResolvingEvent(contextRef, callback);
        }

        public static void RemoveUnloadingResolving(KeyValuePair<long, long> contextRef, Func<KeyValuePair<long, long>, AssemblyName, Assembly> callback)
        {
            Debug.Assert(AssemblyContext.IsCoreCLR);
            internalRemoveResolvingEvent(contextRef, callback);
        }

        // Internal functions, these are safe to access AssemblyContext directly

        private static bool InternalIsFullyUnloaded(KeyValuePair<long, long> contextRef)
        {
            return InternalGetContext(contextRef).IsFullyUnloaded;
        }

        private static int InternalGetContextState(KeyValuePair<long, long> contextRef)
        {
            return (int)InternalGetContext(contextRef).State;
        }

        private static void InternalUnload(KeyValuePair<long, long> contextRef)
        {
            InternalGetContext(contextRef).Unload();
        }

        private static Assembly[] InternalGetAssemblies(KeyValuePair<long, long> contextRef)
        {
            return InternalGetContext(contextRef).GetAssemblies();
        }

        private static Assembly InternalLoadFrom(KeyValuePair<long, long> contextRef, string assemblyPath)
        {
            return InternalGetContext(contextRef).LoadFrom(assemblyPath);
        }

        private static Assembly InternalLoadFromStream(KeyValuePair<long, long> contextRef, Stream assembly, Stream assemblySymbols)
        {
            return InternalGetContext(contextRef).LoadFromStream(assembly, assemblySymbols);
        }

        private static KeyValuePair<long, long> InternalGetContextRef(Assembly assembly)
        {
            return AssemblyContext.GetContextRef(assembly as Assembly);
        }

        private static KeyValuePair<long, long> InternalCreate(KeyValuePair<long, long> currentContextRef)
        {
            return AssemblyContext.Create(currentContextRef);
        }

        private static void InternalAddUnloadingEvent(KeyValuePair<long, long> contextRef, Action<KeyValuePair<long, long>> callback)
        {
            AssemblyContext context = InternalGetContext(contextRef);
            context.Unloading += callback;
        }

        private static void InternalRemoveUnloadingEvent(KeyValuePair<long, long> contextRef, Action<KeyValuePair<long, long>> callback)
        {
            AssemblyContext context = InternalGetContext(contextRef);
            context.Unloading -= callback;
        }

        private static void InternalAddResolvingEvent(KeyValuePair<long, long> contextRef, Func<KeyValuePair<long, long>, AssemblyName, Assembly> callback)
        {
            AssemblyContext context = InternalGetContext(contextRef);
            context.Resolving += callback;
        }

        private static void InternalRemoveResolvingEvent(KeyValuePair<long, long> contextRef, Func<KeyValuePair<long, long>, AssemblyName, Assembly> callback)
        {
            AssemblyContext context = InternalGetContext(contextRef);
            context.Resolving -= callback;
        }

        /// <summary>
        /// Only call this from within other InternalXXXX functions (as AssemblyContext can't move between context boundries)
        /// </summary>
        private static AssemblyContext InternalGetContext(AssemblyContextRef contextRef)
        {
            return AssemblyContext.InternalGetContext(contextRef);
        }
    }

    public enum AssemblyContextState
    {
        Alive,
        Unloading,
        Unloaded
    }

    /// <summary>
    /// Copy of CrossAppDomainDelegate
    /// </summary>
    public delegate void CrossAssemblyContextDelegate();

    /// <summary>
    /// An interface for working with AssemblyLoadContext without having to reference .NET Core assemblies
    /// </summary>
    public interface IAssemblyLoadContext
    {
        /// <summary>
        /// Loads an assembly from the given file path. This is the equivalent of Assembly.LoadFrom(path).
        /// </summary>
        /// <param name="assemblyPath">The file path of the assembly.</param>
        /// <returns>The loaded assembly.</returns>
        Assembly LoadFromAssemblyPath(string assemblyPath);

        /// <summary>
        /// Loads an assembly from the given stream containing the assembly file byte data.
        /// </summary>
        /// <param name="assembly">A stream containing the assembly file byte data.</param>
        /// <param name="assemblySymbols">A stream containing the assembly file symbols (PDB) byte data. This can be null.</param>
        /// <returns>The loaded assembly.</returns>
        Assembly LoadFromStream(Stream assembly, Stream assemblySymbols);

        /// <summary>
        /// Unloads the assembly context. One or more GC.Collect() is likely required to fully release all held the resources.
        /// </summary>
        void Unload();

        /// <summary>
        /// Checks if this context is still alive. Make sure to check this before calling Unload.
        /// </summary>
        bool IsAlive();

        /// <summary>
        /// Returns an array of assemblies which are loaded by this context.
        /// </summary>
        Assembly[] GetAssemblies();

        /// <summary>
        /// Returns the owning <see cref="AssemblyContext"/>.
        /// </summary>
        AssemblyContext GetOwner();
    }

    public partial class AssemblyContext
    {
        private static void CreateAssemblyLoadContextType()
        {
            //Create a class which roughly looks like this:
            //public class DynamicAssemblyLoadContext : AssemblyLoadContext, IAssemblyLoadContext
            //{
            //    private Action<AssemblyLoadContext> unloadingEvent;
            //    private Func<AssemblyLoadContext, AssemblyName, Assembly> resolvingEvent;
            //    private MethodRedirects.LoadFromAssemblyPathDel baseLoadFromAssemblyPath;
            //    private long contextId;// An id for finding our AssemblyContext
            //
            //    public DynamicAssemblyLoadContext(long contextId) : base(true)
            //    {
            //        unloadingEvent = new Action<AssemblyLoadContext>(OnUnloading);
            //        Unloading += unloadingEvent;
            //        resolvingEvent = new Func<AssemblyLoadContext, AssemblyName, Assembly>(OnResolving);
            //        Resolving += resolvingEvent;
            //        this.contextId = contextId;
            //        baseLoadFromAssemblyPath = new MethodRedirects.LoadFromAssemblyPathDel(base.LoadFromAssemblyPath);
            //    }
            //
            //    private void OnUnloading(AssemblyLoadContext context) { Unloading -= unloadingEvent; MethodRedirects.OnUnloading(context); }
            //    private Assembly OnResolving(AssemblyLoadContext context, AssemblyName assemblyName) { return MethodRedirects.OnResolving(context, assemblyName); }
            //    protected Assembly Load(AssemblyName assemblyName) { return MethodRedirects.Load(this, assemblyName); }
            //    public Assembly[] GetAssemblies() { return MethodRedirects.GetAssemblies(this); }
            //    public bool IsAlive() { return MethodRedirects.IsAlive(this); }
            //    public AssemblyContext GetOwner() { return MethodRedirects.GetOwner(this, contextId); }
            //    public Assembly LoadFromAssemblyPath(string assemblyPath)// This wont override the base as the base isn't declared virtual
            //    {
            //        AssemblyContext.MethodRedirects.LoadFromAssemblyPath(this, assemblyPath, baseLoadFromAssemblyPath);
            //    }
            //    public Assembly LoadFromStream(Stream assembly, Stream assemblySymbols { return base.LoadFromStream(assembly, assemblySymbols); }
            //}

            Type baseType = Type.GetType("System.Runtime.Loader.AssemblyLoadContext");

            // MethodRedirects
            Type methodRedirectsType = typeof(MethodRedirects);
            ConstructorInfo delLoadFromAssemblyPath = typeof(MethodRedirects.LoadFromAssemblyPathDel).GetConstructor(
                new Type[] { typeof(object), typeof(IntPtr) });
            MethodInfo redirectLoadFromAssemblyPathMethod = methodRedirectsType.GetMethod("LoadFromAssemblyPath");
            MethodInfo redirectIsAliveMethod = methodRedirectsType.GetMethod("IsAlive");
            MethodInfo redirectGetOwnerMethod = methodRedirectsType.GetMethod("GetOwner");
            MethodInfo redirectGetAssembliesMethod = methodRedirectsType.GetMethod("GetAssemblies");
            MethodInfo redirectOnUnloadingMethod = methodRedirectsType.GetMethod("OnUnloading");
            MethodInfo redirectOnResolvingMethod = methodRedirectsType.GetMethod("OnResolving");

            // Base type methods used by MethodRedirects
            MethodRedirects.Initialize(
                stateField: baseType.GetField("state", BindingFlags.NonPublic | BindingFlags.Instance),
                getLoadContextMethod: baseType.GetMethod("GetLoadContext", BindingFlags.Public | BindingFlags.Static));

            // Get base type info
            ConstructorInfo baseCtor = baseType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(bool) }, null);

            MethodInfo baseLoadMethod = baseType.GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo baseUnloadMethod = baseType.GetMethod("Unload", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo baseLoadFromAssemblyPath = baseType.GetMethod("LoadFromAssemblyPath");
            MethodInfo baseLoadFromStream = baseType.GetMethod("LoadFromStream", new Type[] { typeof(Stream), typeof(Stream) });
            EventInfo baseUnloadingEvent = baseType.GetEvent("Unloading");
            EventInfo baseResolvingEvent = baseType.GetEvent("Resolving");

            // Create the assembly / type
            AssemblyName assemblyName = new AssemblyName("AssemblyLoadContextHelper");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            // Create the runtime generated AssemblyLoadContext
            TypeBuilder typeBuilder = moduleBuilder.DefineType("GeneratedAssemblyLoadContext", TypeAttributes.Public, baseType,
                new Type[] { typeof(IAssemblyLoadContext) });

            // private Action<AssemblyLoadContext> unloadingEvent;
            FieldBuilder unloadingEventField = typeBuilder.DefineField("unloadingEvent", typeof(Action<object>), FieldAttributes.Private);
            ConstructorInfo unloadingEventCtor = typeof(Action<object>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) });

            // private Func<AssemblyLoadContext, AssemblyName, Assembly> resolvingEvent;
            Type resolvingEventType = typeof(Func<object, AssemblyName, Assembly>);
            FieldBuilder resolvingEventField = typeBuilder.DefineField("resolvingEvent", resolvingEventType, FieldAttributes.Private);
            ConstructorInfo resolvingEventCtor = resolvingEventType.GetConstructor(new Type[] { typeof(object), typeof(IntPtr) });

            // private LoadFromAssemblyPathDel baseLoadFromAssemblyPath;
            FieldBuilder baseLoadFromAssemblyPathField = typeBuilder.DefineField("baseLoadFromAssemblyPath",
                typeof(MethodRedirects.LoadFromAssemblyPathDel), FieldAttributes.Private);

            // private long contextId;
            FieldBuilder contextIdField = typeBuilder.DefineField("contextId", typeof(long), FieldAttributes.Private);

            // OnUnloading - private void OnUnloading(AssemblyLoadContext context) { Unloading -= unloadingEvent; MethodRedirects.OnUnloading(context); }
            MethodBuilder onUnloadingMethod = typeBuilder.DefineMethod("OnUnloading",
                MethodAttributes.Private | MethodAttributes.HideBySig,
                CallingConventions.HasThis, typeof(void), new Type[] { baseType });
            {
                ILGenerator il = onUnloadingMethod.GetILGenerator();

                // Unloading -= unloadingEvent;
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, unloadingEventField);
                il.Emit(OpCodes.Call, baseUnloadingEvent.RemoveMethod);

                // MethodRedirects.OnUnloading(context);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, redirectOnUnloadingMethod);

                // Resolving -= resolvingEvent; // (just in case this keeps something alive)
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, resolvingEventField);
                il.Emit(OpCodes.Call, baseResolvingEvent.RemoveMethod);

                il.Emit(OpCodes.Ret);
            }

            // OnResolving - private Assembly OnResolving(AssemblyLoadContext context, AssemblyName assemblyName) { return MethodRedirects.OnResolving(context, assemblyName); }
            MethodBuilder onResolvingMethod = typeBuilder.DefineMethod("OnResolving",
                MethodAttributes.Private | MethodAttributes.HideBySig,
                CallingConventions.HasThis, typeof(Assembly), new Type[] { baseType, typeof(AssemblyName) });
            {
                ILGenerator il = onResolvingMethod.GetILGenerator();

                // return MethodRedirects.OnResolving(context, assemblyName);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Call, redirectOnResolvingMethod);
                il.Emit(OpCodes.Ret);
            }

            // Constructor - public DynamicAssemblyLoadContext(long contextId) : base(false, true)
            ConstructorBuilder ctor = typeBuilder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.HasThis, new Type[] { typeof(long) });
            {
                ILGenerator il = ctor.GetILGenerator();

                // : base(isCollectible)
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4_1);// isCollectible: true
                il.Emit(OpCodes.Call, baseCtor);

                // unloadingEvent = new Action<AssemblyLoadContext>(OnUnloading);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldftn, onUnloadingMethod);
                il.Emit(OpCodes.Newobj, unloadingEventCtor);
                il.Emit(OpCodes.Stfld, unloadingEventField);

                // Unloading += unloadingEvent;
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, unloadingEventField);
                il.Emit(OpCodes.Call, baseUnloadingEvent.AddMethod);

                // resolvingEvent = new Func<AssemblyLoadContext, AssemblyName, Assembly>
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldftn, onResolvingMethod);
                il.Emit(OpCodes.Newobj, resolvingEventCtor);
                il.Emit(OpCodes.Stfld, resolvingEventField);

                // Resolving += unloadingEvent;
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, resolvingEventField);
                il.Emit(OpCodes.Call, baseResolvingEvent.AddMethod);

                // this.contextId = contextId;
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, contextIdField);

                // baseLoadFromAssemblyPath = new MethodRedirects.LoadFromAssemblyPathDel(base.LoadFromAssemblyPath);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldftn, baseLoadFromAssemblyPath);
                il.Emit(OpCodes.Newobj, delLoadFromAssemblyPath);
                il.Emit(OpCodes.Stfld, baseLoadFromAssemblyPathField);

                il.Emit(OpCodes.Ret);
            }

            // Load - protected Assembly Load(AssemblyName assemblyName) { return null; }
            MethodBuilder loadMethod = typeBuilder.DefineMethod("Load",
                MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                CallingConventions.HasThis, typeof(Assembly), new Type[] { typeof(AssemblyName) });
            {
                ILGenerator il = loadMethod.GetILGenerator();
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ret);
            }
            typeBuilder.DefineMethodOverride(loadMethod, baseLoadMethod);

            // Unload - public void Load() { base.Unload(); }
            MethodBuilder unloadMethod = typeBuilder.DefineMethod("Unload",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                CallingConventions.HasThis, typeof(void), Type.EmptyTypes);
            {
                ILGenerator il = unloadMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, baseUnloadMethod);
                il.Emit(OpCodes.Ret);
            }
            typeBuilder.DefineMethodOverride(loadMethod, baseLoadMethod);

            // GetAssemblies - public Assembly[] GetAssemblies() { return MethodRedirects.GetAssemblies(this); }
            MethodBuilder getAssembliesMethod = typeBuilder.DefineMethod("GetAssemblies",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                CallingConventions.HasThis, typeof(Assembly[]), Type.EmptyTypes);
            {
                ILGenerator il = getAssembliesMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, redirectGetAssembliesMethod);
                il.Emit(OpCodes.Ret);
            }

            // IsAlive - public bool IsAlive() { return MethodRedirects.IsAlive(this); }
            MethodBuilder getStateMethod = typeBuilder.DefineMethod("IsAlive",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                CallingConventions.HasThis, typeof(bool), Type.EmptyTypes);
            {
                ILGenerator il = getStateMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, redirectIsAliveMethod);
                il.Emit(OpCodes.Ret);
            }

            // GetOwner - public AssemblyContext GetOwner() { return MethodRedirects.GetOwner(this, contextId); }
            MethodBuilder getOwnerMethod = typeBuilder.DefineMethod("GetOwner",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                CallingConventions.HasThis, typeof(AssemblyContext), Type.EmptyTypes);
            {
                ILGenerator il = getOwnerMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, contextIdField);
                il.Emit(OpCodes.Call, redirectGetOwnerMethod);
                il.Emit(OpCodes.Ret);
            }

            // LoadFromAssemblyPath - public Assembly LoadFromAssemblyPath(string assemblyPath) { ... }
            MethodBuilder loadFromAssemblyPathMethod = typeBuilder.DefineMethod("LoadFromAssemblyPath",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                CallingConventions.HasThis, typeof(Assembly), new Type[] { typeof(string) });
            {
                // return MethodRedirects.LoadFromAssemblyPath(this, assemblyPath, baseLoadFromAssemblyPath);
                ILGenerator il = loadFromAssemblyPathMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, baseLoadFromAssemblyPathField);
                il.Emit(OpCodes.Call, redirectLoadFromAssemblyPathMethod);
                il.Emit(OpCodes.Ret);
            }

            // LoadFromStream - public Assembly LoadFromStream(Stream assembly, Stream assemblySymbols { return base.LoadFromStream(assembly, assemblySymbols); }
            MethodBuilder loadFromStreamMethod = typeBuilder.DefineMethod("LoadFromStream",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                CallingConventions.HasThis, typeof(Assembly), new Type[] { typeof(Stream), typeof(Stream) });
            {
                // return base.LoadFromStream(assembly, assemblySymbols);
                ILGenerator il = loadFromStreamMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Call, baseLoadFromStream);
                il.Emit(OpCodes.Ret);
            }

            generatedContextType = typeBuilder.CreateType();
        }

        /// <summary>
        /// Defines various methods redirected from our generated class to reduce the amount of IL we have to write
        /// </summary>
        public class MethodRedirects
        {
            private static FieldInfo stateField;

            /// <summary>
            /// Delegate pointing to <see cref="System.Runtime.Loader.AssemblyLoadContext.GetLoadContext"/>
            /// </summary>
            public static GetLoadContextDel GetLoadContext;
            public delegate object GetLoadContextDel(Assembly assembly);

            public static void Initialize(
                FieldInfo stateField,
                MethodInfo getLoadContextMethod)
            {
                MethodRedirects.stateField = stateField;
                GetLoadContext = (GetLoadContextDel)getLoadContextMethod.CreateDelegate(typeof(GetLoadContextDel));
            }

            public static bool IsAlive(IAssemblyLoadContext context)
            {
                return (int)Convert.ChangeType(stateField.GetValue(context), TypeCode.Int32) == 0;
            }

            public static AssemblyContext GetOwner(IAssemblyLoadContext context, long contextId)
            {
                AssemblyContext result;
                if (contexts.TryGetValue(contextId, out result))
                {
                    Debug.Assert(result.LoadContext == context);
                    return result;
                }
                return null;
            }

            public static IAssemblyLoadContext GetLoadContextInternal(Assembly assembly)
            {
                return GetLoadContext(assembly) as IAssemblyLoadContext;
            }

            public static Assembly[] GetAssemblies(IAssemblyLoadContext context)
            {
                List<Assembly> result = new List<Assembly>();
                Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in allAssemblies)
                {
                    if (GetLoadContextInternal(assembly) == context)
                    {
                        result.Add(assembly);
                    }
                }
                return result.ToArray();
            }

            public delegate Assembly LoadFromAssemblyPathDel(string assemblyPath);
            public static Assembly LoadFromAssemblyPath(IAssemblyLoadContext context, string assemblyPath, LoadFromAssemblyPathDel baseMethod)
            {
                // As this isn't overriding the base function this function is a bit pointless. We may as well do any needed checks elsewhere.

                // TODO: Validate the caller is within the correct domain by checking the callstack? (this would have to assume the caller isn't inlined?)
                return baseMethod(assemblyPath);
            }

            public static void OnUnloading(object contextObj)
            {
                IAssemblyLoadContext context = contextObj as IAssemblyLoadContext;
                if (context != null)
                {
                    AssemblyContext owner = context.GetOwner();
                    Debug.Assert(owner != null, "Owner shouldn't be null");
                    Debug.Assert(!owner.IsUnloaded, "Context shouldn't have already unloaded");
                    owner.OnUnloading();
                }
            }

            public static Assembly OnResolving(object contextObj, AssemblyName assemblyName)
            {
                IAssemblyLoadContext context = contextObj as IAssemblyLoadContext;
                if (context != null)
                {
                    AssemblyContext owner = context.GetOwner();
                    Debug.Assert(owner != null, "Owner shouldn't be null");
                    return owner.OnResolving(assemblyName);
                }
                return null;
            }
        }
    }
}
