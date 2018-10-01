using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    public interface IPropertyAccessor<T>
    {
        T GetValuePtr(IntPtr address);
        void SetValuePtr(IntPtr address, T value);
    }

    public interface IPropertyDefaultValueAccessor<T>
    {
        T GetDefaultValue();
    }

    public class PropertyAccessor<T>
    {
        protected UProperty property;
        protected IPropertyAccessor<T> customAccessor;
        protected IPropertyDefaultValueAccessor<T> defaultValueAccessor;

        public PropertyAccessor(UProperty property)
        {
            this.property = property;
            this.customAccessor = property as IPropertyAccessor<T>;
            this.defaultValueAccessor = property as IPropertyDefaultValueAccessor<T>;
        }

        /// <summary>
        /// Gets the value for the given object instance
        /// </summary>
        /// <param name="instance">The object instance</param>
        /// <param name="arrayIndex">The array index if this is an array type</param>
        /// <returns>The value</returns>
        public T GetValue(UObject instance, int arrayIndex = 0)
        {
            return GetValuePtr(property.ContainerPtrToValuePtr(instance == null ? IntPtr.Zero : instance.Address, arrayIndex));
        }

        /// <summary>
        /// Gets the value for the given object instance
        /// </summary>
        /// <param name="instance">The address of the object instance</param>
        /// <param name="arrayIndex">The array index if this is an array type</param>
        /// <returns>The value</returns>
        public T GetValue(IntPtr instance, int arrayIndex = 0)
        {
            return GetValuePtr(property.ContainerPtrToValuePtr(instance, arrayIndex));
        }

        /// <summary>
        /// Gets the value for the given object instance or a default value if null
        /// </summary>
        /// <param name="instance">The object instance</param>
        /// <param name="arrayIndex">The array index if this is an array type</param>
        /// <returns>The value</returns>
        public T GetValueOrDefault(UObject instance, int arrayIndex = 0)
        {
            return instance == null ? GetDefaultValue() : GetValue(instance, arrayIndex);
        }

        /// <summary>
        /// Gets the value for the given object instance or a default value if null
        /// </summary>
        /// <param name="instance">The address of the object instance</param>
        /// <param name="arrayIndex">The array index if this is an array type</param>
        /// <returns>The value</returns>
        public T GetValueOrDefault(IntPtr instance, int arrayIndex = 0)
        {
            return instance == IntPtr.Zero ? GetDefaultValue() : GetValue(instance, arrayIndex);
        }

        /// <summary>
        /// Gets the value at given address (address is of the value (float, int, byte, etc) rather than an object instance)
        /// </summary>
        /// <param name="address">The value address</param>
        /// <returns>The value</returns>
        public virtual T GetValuePtr(IntPtr address)
        {
            if (customAccessor != null)
            {
                return customAccessor.GetValuePtr(address);
            }
            else
            {
                return Marshal.PtrToStructure<T>(address);
            }
        }

        public void SetValue(UObject obj, T value, int arrayIndex = 0)
        {
            SetValuePtr(property.ContainerPtrToValuePtr(obj, arrayIndex), value);
        }

        public void SetValue(IntPtr instance, T value, int arrayIndex = 0)
        {
            SetValuePtr(property.ContainerPtrToValuePtr(instance, arrayIndex), value);
        }

        public virtual void SetValuePtr(IntPtr address, T value)
        {
            if (customAccessor != null)
            {
                customAccessor.SetValuePtr(address, value);
            }
            else
            {
                Marshal.StructureToPtr(value, address, false);
            }
        }

        public T GetDefaultValue()
        {
            if (defaultValueAccessor != null)
            {
                return defaultValueAccessor.GetDefaultValue();
            }
            else
            {
                return default(T);
            }
        }
    }    
}
