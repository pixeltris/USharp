using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Note: Keep this as a struct so the implicit operator can convert null parameters to an ObjectOuter

    /// <summary>
    /// Wrapper class to allow an outer UObject to be specified as ANY_PACKAGE which is defined as -1 in native code
    /// </summary>
    public struct ObjectOuter
    {
        public UObject Object;
        public bool IsAnyPackage;

        public IntPtr Address
        {
            get
            {
                if (IsAnyPackage)
                {
                    return new IntPtr(-1);
                }
                return Object == null ? IntPtr.Zero : Object.Address;
            }
        }

        public static ObjectOuter AnyPackage
        {
            get
            {
                return new ObjectOuter()
                {
                    IsAnyPackage = true
                };
            }
        }

        public static ObjectOuter Null
        {
            get
            {
                return default(ObjectOuter);
            }
        }

        public static implicit operator ObjectOuter(UObject obj)
        {
            return new ObjectOuter()
            {
                Object = obj
            };
        }
    }
}
