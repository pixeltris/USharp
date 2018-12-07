using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UNumericProperty
    {
        public delegate csbool Del_IsFloatingPoint(IntPtr instance);
        public delegate csbool Del_IsInteger(IntPtr instance);
        public delegate csbool Del_IsEnum(IntPtr instance);
        public delegate IntPtr Del_GetIntPropertyEnum(IntPtr instance);
        public delegate void Del_SetIntPropertyValueUnsigned(IntPtr instance, IntPtr data, ulong value);
        public delegate void Del_SetIntPropertyValueSigned(IntPtr instance, IntPtr data, long value);
        public delegate void Del_SetFloatingPointPropertyValue(IntPtr instance, IntPtr data, double value);
        public delegate void Del_SetNumericPropertyValueFromString(IntPtr instance, IntPtr data, ref FScriptArray value);
        public delegate long Del_GetSignedIntPropertyValue(IntPtr instance, IntPtr data);
        public delegate ulong Del_GetUnsignedIntPropertyValue(IntPtr instance, IntPtr data);
        public delegate double Del_GetFloatingPointPropertyValue(IntPtr instance, IntPtr data);
        public delegate void Del_GetNumericPropertyValueToString(IntPtr instance, IntPtr data, ref FScriptArray result);

        public static Del_IsFloatingPoint IsFloatingPoint;
        public static Del_IsInteger IsInteger;
        public static Del_IsEnum IsEnum;
        public static Del_GetIntPropertyEnum GetIntPropertyEnum;
        public static Del_SetIntPropertyValueUnsigned SetIntPropertyValueUnsigned;
        public static Del_SetIntPropertyValueSigned SetIntPropertyValueSigned;
        public static Del_SetFloatingPointPropertyValue SetFloatingPointPropertyValue;
        public static Del_SetNumericPropertyValueFromString SetNumericPropertyValueFromString;
        public static Del_GetSignedIntPropertyValue GetSignedIntPropertyValue;
        public static Del_GetUnsignedIntPropertyValue GetUnsignedIntPropertyValue;
        public static Del_GetFloatingPointPropertyValue GetFloatingPointPropertyValue;
        public static Del_GetNumericPropertyValueToString GetNumericPropertyValueToString;
    }
}
