using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UField
    {
        public delegate IntPtr Del_Get_Next(IntPtr instance);
        public delegate void Del_Set_Next(IntPtr instance, IntPtr value);
        public delegate void Del_AddCppProperty(IntPtr instance, IntPtr property);
        public delegate void Del_Bind(IntPtr instance);
        public delegate IntPtr Del_GetOwnerClass(IntPtr instance);
        public delegate IntPtr Del_GetOwnerStruct(IntPtr instance);
        public delegate void Del_GetDisplayName(IntPtr instance, ref FScriptArray result);
        public delegate void Del_GetToolTip(IntPtr instance, ref FScriptArray result);
        public delegate csbool Del_HasMetaData(IntPtr instance, ref FScriptArray key);
        public delegate csbool Del_HasMetaDataF(IntPtr instance, ref FName key);
        public delegate void Del_GetMetaData(IntPtr instance, ref FScriptArray key, ref FScriptArray result);
        public delegate void Del_GetMetaDataF(IntPtr instance, ref FName key, ref FScriptArray result);
        public delegate void Del_SetMetaData(IntPtr instance, ref FScriptArray key, ref FScriptArray inValue);
        public delegate void Del_SetMetaDataF(IntPtr instance, ref FName key, ref FScriptArray inValue);
        public delegate csbool Del_GetBoolMetaData(IntPtr instance, ref FScriptArray key);
        public delegate csbool Del_GetBoolMetaDataF(IntPtr instance, ref FName key);
        public delegate int Del_GetINTMetaData(IntPtr instance, ref FScriptArray key);
        public delegate int Del_GetINTMetaDataF(IntPtr instance, ref FName key);
        public delegate float Del_GetFLOATMetaData(IntPtr instance, ref FScriptArray key);
        public delegate float Del_GetFLOATMetaDataF(IntPtr instance, ref FName key);
        public delegate IntPtr Del_GetClassMetaData(IntPtr instance, ref FScriptArray key);
        public delegate IntPtr Del_GetClassMetaDataF(IntPtr instance, ref FName key);
        public delegate void Del_RemoveMetaData(IntPtr instance, ref FScriptArray key);
        public delegate void Del_RemoveMetaDataF(IntPtr instance, ref FName key);
        
        public static Del_Get_Next Get_Next;
        public static Del_Set_Next Set_Next;
        public static Del_AddCppProperty AddCppProperty;
        public static Del_Bind Bind;
        public static Del_GetOwnerClass GetOwnerClass;
        public static Del_GetOwnerStruct GetOwnerStruct;
        public static Del_GetDisplayName GetDisplayName;
        public static Del_GetToolTip GetToolTip;
        public static Del_HasMetaData HasMetaData;
        public static Del_HasMetaDataF HasMetaDataF;
        public static Del_GetMetaData GetMetaData;
        public static Del_GetMetaDataF GetMetaDataF;
        public static Del_SetMetaData SetMetaData;
        public static Del_SetMetaDataF SetMetaDataF;
        public static Del_GetBoolMetaData GetBoolMetaData;
        public static Del_GetBoolMetaDataF GetBoolMetaDataF;
        public static Del_GetINTMetaData GetINTMetaData;
        public static Del_GetINTMetaDataF GetINTMetaDataF;
        public static Del_GetFLOATMetaData GetFLOATMetaData;
        public static Del_GetFLOATMetaDataF GetFLOATMetaDataF;
        public static Del_GetClassMetaData GetClassMetaData;
        public static Del_GetClassMetaDataF GetClassMetaDataF;
        public static Del_RemoveMetaData RemoveMetaData;
        public static Del_RemoveMetaDataF RemoveMetaDataF;
    }
}
