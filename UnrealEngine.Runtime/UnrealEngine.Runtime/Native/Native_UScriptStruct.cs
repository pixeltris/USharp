using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UScriptStruct
    {
        public delegate EStructFlags Del_Get_StructFlags(IntPtr instance);
        public delegate void Del_Set_StructFlags(IntPtr instance, EStructFlags value);
        public delegate void Del_DeferCppStructOps(ref FName target, IntPtr inCppStructOps);
        public delegate void Del_PrepareCppStructOps(IntPtr instance);
        public delegate IntPtr Del_GetCppStructOps(IntPtr instance);
        public delegate void Del_ClearCppStructOps(IntPtr instance);
        public delegate csbool Del_HasDefaults(IntPtr instance);
        public delegate csbool Del_ShouldSerializeAtomically(IntPtr instance, IntPtr ar);
        public delegate csbool Del_CompareScriptStruct(IntPtr instance, IntPtr a, IntPtr b, uint portFlags);
        public delegate void Del_CopyScriptStruct(IntPtr instance, IntPtr dest, IntPtr src, int arrayDim);
        public delegate void Del_ClearScriptStruct(IntPtr instance, IntPtr dest, int arrayDim);
        public delegate void Del_RecursivelyPreload(IntPtr instance);
        public delegate void Del_GetCustomGuid(IntPtr instance, out Guid result);
        public delegate void Del_InitializeDefaultValue(IntPtr instance, byte[] inStructData);
        
        public static Del_Get_StructFlags Get_StructFlags;
        public static Del_Set_StructFlags Set_StructFlags;
        public static Del_DeferCppStructOps DeferCppStructOps;
        public static Del_PrepareCppStructOps PrepareCppStructOps;
        public static Del_GetCppStructOps GetCppStructOps;
        public static Del_ClearCppStructOps ClearCppStructOps;
        public static Del_HasDefaults HasDefaults;
        public static Del_ShouldSerializeAtomically ShouldSerializeAtomically;
        public static Del_CompareScriptStruct CompareScriptStruct;
        public static Del_CopyScriptStruct CopyScriptStruct;
        public static Del_ClearScriptStruct ClearScriptStruct;
        public static Del_RecursivelyPreload RecursivelyPreload;
        public static Del_GetCustomGuid GetCustomGuid;
        public static Del_InitializeDefaultValue InitializeDefaultValue;
    }
}
