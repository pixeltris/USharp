using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// The base interface for exposing interfaces to Unreal.
    /// </summary>
    public interface IInterface
    {
        IntPtr GetAddress();
        UObject GetObject();
    }

    /// <summary>
    /// The base interface implementation for exposing interfaces to Unreal.
    /// 
    /// This is used by generated code to provide a fallback implementation which allows for calling
    /// implementation methods without needing a concrete implementation defined in C#.<para/> 
    /// This is desirable for cases where we have an interface defined in C# but implemented in a Blueprint 
    /// for which we wouldn't have a type defined in C#. So instead we inject this IInterfaceImpl into the 
    /// UObject so we can use the desired interface.
    /// </summary>
    public abstract class IInterfaceImpl : IInterface
    {
        private CachedUObject cachedObj;
        public IntPtr Address
        {
            get { return cachedObj.Address; }
        }
        public IntPtr GetAddress()
        {
            return cachedObj.Address;
        }
        public UObject GetObject()
        {
            return cachedObj.Value;
        }
        public void CheckDestroyed()
        {            
            if (cachedObj.Value == null || cachedObj.Value.IsDestroyed)
            {
                Type type = cachedObj.Value != null ? cachedObj.Value.GetType() : GetType();
                throw new Exception("Attempting to access a destroyed unreal object of type " + type.ToString());
            }
        }

        internal void SetObj(UObjectRef objRef)
        {
            cachedObj.Set(objRef);
        }

        // OnReleased OnReleasedFromPool OnReturned Reset ResetState PoolReset PooledObjectReset ResetPooledObject
        // OnPoolReturn OnPoolReturned ResetInterface ResetInterfaceState
        /// <summary>
        /// Called when this pooled object is returned to the object pool and needs to be reset to a default state.
        /// </summary>
        public virtual void ResetInterface()
        {
        }
    }

    /// <summary>
    /// Pools IInterface instances have have been injected into UObject instances which dont't have a
    /// concrete implementation of the given interface defined in C# (this hold onto generated "Impl" interfaces).<para/>
    /// NOTE: This means that any given generated "Impl" IInterface could be reclaimed and reused at any time.
    /// </summary>
    public static class UnrealInterfacePool
    {
        // <IInterface type, IInterfaceImpl type>
        private static Dictionary<Type, Type> interfaceTypes = new Dictionary<Type, Type>();

        // <IInterfaceImpl, pool>
        private static Dictionary<Type, Stack<IInterfaceImpl>> pools = new Dictionary<Type, Stack<IInterfaceImpl>>();        

        internal static void LoadType(Type type)
        {
            UUnrealTypePathAttribute pathAttribute;
            if (type.IsInterface && UnrealTypes.All.TryGetValue(type, out pathAttribute) && pathAttribute.InterfaceImpl != null &&
                !pools.ContainsKey(pathAttribute.InterfaceImpl))
            {
                interfaceTypes[type] = pathAttribute.InterfaceImpl;
                pools[pathAttribute.InterfaceImpl] = new Stack<IInterfaceImpl>();
            }
        }

        public static IInterface New(Type type, UObjectRef objRef)
        {
            Type implType;
            Stack<IInterfaceImpl> pool;
            if (interfaceTypes.TryGetValue(type, out implType) &&
                pools.TryGetValue(implType, out pool))
            {
                if (pool.Count > 0)
                {
                    IInterfaceImpl instance = pool.Pop();
                    instance.SetObj(objRef);
                    return instance;
                }
                else
                {
                    // TODO: Use a faster way of constructing an instance
                    IInterfaceImpl instance = (IInterfaceImpl)Activator.CreateInstance(implType);
                    instance.SetObj(objRef);
                    return instance;
                }
            }
            return null;
        }

        public static void ReturnObject(IInterface obj)
        {
            if (obj != null)
            {
                // TODO: Put the pool it belongs to inside IInterfaceImpl
                Stack<IInterfaceImpl> pool;
                if (pools.TryGetValue(obj.GetType(), out pool))
                {
                    IInterfaceImpl objImpl = (IInterfaceImpl)obj;
                    objImpl.ResetInterface();
                    objImpl.SetObj((UObjectRef)null);

                    // TODO: Set a cap on how many objects should be available in the stack?
                    pool.Push(objImpl);
                }
            }
        }
    }
}
