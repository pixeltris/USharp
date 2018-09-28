using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // This version might not be enough if we use UObject pooling as both objRef and Value could be reused.
    // If we don't use UObject pooling this should be fine to use.
    public struct CachedUObject<T> where T : UObject
    {
        private UObjectRef objRef;
        public T Value;
        public IntPtr Address
        {
            get { return Value == null ? IntPtr.Zero : Value.Address; }
        }

        public T Update(IntPtr address)
        {
            if (objRef != null)
            {
                if (address == objRef.Native && Value.objRef == objRef)
                {
                    return Value;
                }
                Value = null;
            }
            objRef = GCHelper.FindRef(address);
            if (objRef != null)
            {
                Value = objRef.Managed as T;
                if (Value == null || Value.objRef != objRef)
                {
                    objRef = null;
                    Value = null;
                }
            }
            return Value;
        }
        
        public IntPtr Set(T value)
        {
            UObjectRef newObjRef = value == null ? null : value.objRef;
            if (newObjRef != objRef || Value.objRef != objRef)
            {
                if (newObjRef != null)
                {
                    objRef = newObjRef;
                    Value = objRef.Managed as T;
                    if (Value == null || Value.objRef != objRef)
                    {
                        objRef = null;
                        Value = null;
                        return IntPtr.Zero;
                    }
                    return objRef.Native;
                }
                else
                {
                    objRef = null;
                    Value = null;
                    return IntPtr.Zero;
                }
            }
            else
            {
                return objRef == null ? IntPtr.Zero : objRef.Native;
            }
        }
    }

    public struct CachedUObject
    {
        private UObjectRef objRef;
        public UObject Value;
        public IntPtr Address
        {
            get { return Value == null ? IntPtr.Zero : Value.Address; }
        }

        public UObject Update(IntPtr address)
        {
            if (objRef != null)
            {
                if (objRef.Native == address && Value.objRef == objRef)
                {
                    return Value;
                }
                Value = null;
            }
            objRef = GCHelper.FindRef(address);
            if (objRef != null)
            {
                Value = objRef.Managed;
                if (Value == null || Value.objRef != objRef)
                {
                    objRef = null;
                    Value = null;
                }
            }
            return Value;
        }

        public IntPtr Set(UObject value)
        {
            return Set(value == null ? null : value.objRef);
        }

        public IntPtr Set(UObjectRef value)
        {
            if (value != objRef || Value.objRef != objRef)
            {
                if (value != null)
                {
                    objRef = value;
                    Value = objRef.Managed;
                    if (Value == null || Value.objRef != objRef)
                    {
                        objRef = null;
                        Value = null;
                        return IntPtr.Zero;
                    }
                    return objRef.Native;
                }
                else
                {
                    objRef = null;
                    Value = null;
                    return IntPtr.Zero;
                }
            }
            else
            {
                return objRef == null ? IntPtr.Zero : objRef.Native;
            }
        }
    }

    /*// It would be better if we didn't have to construct a delegate here but due to UObjectRef pooling
    // we need to know when the object is destroyed to safetly clear the state. UObject might eventually
    // use pooling too which makes the delegate especially required.
    public struct CachedUObject<T> where T : UObject
    {
        // Could do away with needing the objRef here but as we clear objRef inside UObject
        // we can't do the required unbinding of the OnDestroyed delegate.
        private UObjectRefDestroyedHandler onObjRefDestroyed;
        private UObjectRef objRef;
        public T Value;

        private UObjectRefDestroyedHandler GetOnDestroyedHandler()
        {
            if (onObjRefDestroyed == null)
            {
                onObjRefDestroyed = OnObjRefDestroyed;
            }
            return onObjRefDestroyed;
        }

        public T Update(IntPtr address)
        {
            if (objRef != null)
            {
                if (objRef.Native == address)
                {
                    return Value;
                }

                if (onObjRefDestroyed != null)
                {
                    objRef.OnDestroyed -= onObjRefDestroyed;
                    onObjRefDestroyed = null;
                }

                Value = null;
            }
            objRef = GCHelper.FindRef(address);
            if (objRef != null)
            {
                Value = objRef.Managed as T;
                if (Value == null || Value.objRef != objRef)
                {
                    objRef = null;
                    Value = null;
                }
                else
                {
                    objRef.OnDestroyed += GetOnDestroyedHandler();
                }
            }
            return Value;
        }

        public IntPtr Set(T value)
        {
            UObjectRef newObjRef = value == null ? null : value.objRef;
            if (newObjRef != objRef)
            {
                if (onObjRefDestroyed != null)
                {
                    objRef.OnDestroyed -= onObjRefDestroyed;
                    onObjRefDestroyed = null;
                }
                
                if (newObjRef != null)
                {
                    objRef = newObjRef;
                    Value = objRef.Managed as T;
                    if (Value == null || Value.objRef != objRef)
                    {
                        objRef = null;
                        Value = null;
                        return IntPtr.Zero;
                    }
                    objRef.OnDestroyed += GetOnDestroyedHandler();
                    return objRef.Native;
                }
                else
                {
                    objRef = null;
                    Value = null;
                    return IntPtr.Zero;
                }
            }
            else
            {
                return objRef == null ? IntPtr.Zero : objRef.Native;
            }
        }

        private void OnObjRefDestroyed(UObjectRef objRef)
        {
            Set(null);
        }
    }

    public struct CachedUObject
    {
        private UObjectRefDestroyedHandler onObjRefDestroyed;
        private UObjectRef objRef;
        public UObject Value
        {
            get { return objRef == null ? null : objRef.Managed; }
        }
        public IntPtr Address
        {
            get { return objRef == null ? IntPtr.Zero : objRef.Native; }
        }

        private UObjectRefDestroyedHandler GetOnDestroyedHandler()
        {
            if (onObjRefDestroyed == null)
            {
                onObjRefDestroyed = OnObjRefDestroyed;
            }
            return onObjRefDestroyed;
        }

        public UObject Update(IntPtr address)
        {
            if (objRef != null)
            {
                if (objRef.Native == address)
                {
                    return Value;
                }

                if (onObjRefDestroyed != null)
                {
                    objRef.OnDestroyed -= onObjRefDestroyed;
                    onObjRefDestroyed = null;
                }
                
                objRef = GCHelper.FindRef(address);
                if (objRef != null)
                {
                    objRef.OnDestroyed += GetOnDestroyedHandler();
                }
            }
            else
            {
                objRef = GCHelper.FindRef(address);
                if (objRef != null)
                {
                    objRef.OnDestroyed += GetOnDestroyedHandler();
                }
            }
            return objRef == null ? null : objRef.Managed;
        }

        public IntPtr Set(UObject value)
        {
            return Set(value == null ? null : value.objRef);
        }

        public IntPtr Set(UObjectRef value)
        {
            if (objRef != value)
            {
                if (onObjRefDestroyed != null)
                {
                    objRef.OnDestroyed -= onObjRefDestroyed;
                    onObjRefDestroyed = null;
                }

                objRef = value;
                if (objRef != null)
                {
                    objRef.OnDestroyed += GetOnDestroyedHandler();
                    return objRef.Native;
                }
                else
                {
                    return IntPtr.Zero;
                }
            }
            else
            {
                return objRef == null ? IntPtr.Zero : objRef.Native;
            }
        }

        private void OnObjRefDestroyed(UObjectRef objRef)
        {
            Set((UObjectRef)null);
        }
    }*/
}
