using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Holds a UField address. This is used so that references to hot reloaded UProperties are updated.
    /// </summary>
    public class UFieldAddress
    {
        // Don't give these getters/setters as generated IL depends on them being a field (or at least for Address)

        public IntPtr Address;
        public int Size;
        public int ArrayDim;
        // Useful fields for checking readonly values before writing to data
        public bool IsEditConst;
        public bool IsBlueprintReadOnly;

        public IntPtr GenericArg1Address;
        public int GenericArg1Size;
        public int GenericArg1ArrayDim;

        public IntPtr GenericArg2Address;
        public int GenericArg2Size;
        public int GenericArg2ArrayDim;

        public EPropertyType PropertyType;

        public string PathName
        {
            get { return NativeReflection.GetPathName(Address); }
        }

        public UFieldAddress()
        {
        }

        public UFieldAddress(IntPtr address)
        {
            Update(address);
        }

        public bool Update(IntPtr address)
        {
            if (Address != address)
            {
                Address = address;
                Size = 0;
                ArrayDim = 0;
                IsEditConst = false;
                IsBlueprintReadOnly = false;
                GenericArg1Address = IntPtr.Zero;
                GenericArg1Size = 0;
                GenericArg1ArrayDim = 0;
                GenericArg2Address = IntPtr.Zero;
                GenericArg2Size = 0;
                GenericArg2ArrayDim = 0;
                PropertyType = EPropertyType.Unknown;

                if (address == IntPtr.Zero)
                {
                    return true;
                }

                EPropertyType propertyType = NativeReflection.GetPropertyType(address);
                if (propertyType != EPropertyType.Unknown)
                {
                    PropertyType = propertyType;
                    
                    Size = Native_UProperty.Get_ElementSize(address);
                    ArrayDim = Native_UProperty.Get_ArrayDim(address);

                    IsEditConst = Native_UProperty.HasAnyPropertyFlags(address, EPropertyFlags.EditConst);
                    IsBlueprintReadOnly = Native_UProperty.HasAnyPropertyFlags(address, EPropertyFlags.BlueprintReadOnly);

                    switch (propertyType)
                    {
                        case EPropertyType.Array:
                            GenericArg1Address = Native_UArrayProperty.Get_Inner(address);
                            if (GenericArg1Address != IntPtr.Zero)
                            {
                                GenericArg1Size = Native_UProperty.Get_ElementSize(GenericArg1Address);
                                GenericArg1ArrayDim = Native_UProperty.Get_ArrayDim(GenericArg1Address);
                            }
                            break;
                        case EPropertyType.Map:
                            GenericArg1Address = Native_UMapProperty.Get_KeyProp(address);
                            if (GenericArg1Address != IntPtr.Zero)
                            {
                                GenericArg1Size = Native_UProperty.Get_ElementSize(GenericArg1Address);
                                GenericArg1ArrayDim = Native_UProperty.Get_ArrayDim(GenericArg1Address);
                            }
                            GenericArg2Address = Native_UMapProperty.Get_ValueProp(address);
                            if (GenericArg2Address != IntPtr.Zero)
                            {
                                GenericArg2Size = Native_UProperty.Get_ElementSize(GenericArg2Address);
                                GenericArg2ArrayDim = Native_UProperty.Get_ArrayDim(GenericArg2Address);
                            }
                            break;
                        case EPropertyType.Set:
                            GenericArg1Address = Native_USetProperty.Get_ElementProp(address);
                            if (GenericArg1Address != IntPtr.Zero)
                            {
                                GenericArg1Size = Native_UProperty.Get_ElementSize(GenericArg1Address);
                                GenericArg1ArrayDim = Native_UProperty.Get_ArrayDim(GenericArg1Address);
                            }
                            break;
                    }
                }

                return true;
            }
            return false;
        }
    }
}
