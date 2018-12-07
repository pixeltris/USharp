using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UEnum
    {
        public delegate void Del_Get_CppType(IntPtr instance, ref FScriptArray result);
        public delegate void Del_Set_CppType(IntPtr instance, ref FScriptArray value);
        public delegate int Del_GetIndexByValue(IntPtr instance, long value);
        public delegate long Del_GetValueByIndex(IntPtr instance, int index);
        public delegate void Del_GetNameByIndex(IntPtr instance, int index, out FName result);
        public delegate int Del_GetIndexByName(IntPtr instance, ref FName inName, UEnum.EGetByNameFlags flags);
        public delegate void Del_GetNameByValue(IntPtr instance, long inValue, out FName result);
        public delegate int Del_GetValueByName(IntPtr instance, ref FName inName, UEnum.EGetByNameFlags flags);
        public delegate void Del_GetNameStringByIndex(IntPtr instance, int index, ref FScriptArray result);
        public delegate int Del_GetIndexByNameString(IntPtr instance, ref FScriptArray searchString, UEnum.EGetByNameFlags flags);
        public delegate void Del_GetNameStringByValue(IntPtr instance, long value, ref FScriptArray result);        
        public delegate long Del_GetValueByNameString(IntPtr instance, ref FScriptArray searchString, UEnum.EGetByNameFlags flags);
        public delegate void Del_GetDisplayNameTextStringByIndex(IntPtr instance, int index, ref FScriptArray result);
        public delegate void Del_GetDisplayNameTextStringByValue(IntPtr instance, long value, ref FScriptArray result);
        public delegate long Del_GetMaxEnumValue(IntPtr instance);
        public delegate csbool Del_IsValidEnumValue(IntPtr instance, long inValue);
        public delegate csbool Del_IsValidEnumName(IntPtr instance, ref FName inName);
        public delegate void Del_RemoveNamesFromMasterList(IntPtr instance);
        public delegate long Del_ResolveEnumerator(IntPtr instance, IntPtr ar, long enumeratorIndex);
        public delegate UEnum.ECppForm Del_GetCppForm(IntPtr instance);
        public delegate csbool Del_IsFullEnumName(ref FScriptArray inEnumName);
        public delegate void Del_GenerateFullEnumName(IntPtr instance, ref FScriptArray inEnumName, ref FScriptArray result);
        public delegate long Del_LookupEnumName(ref FName testName, IntPtr foundEnum);
        public delegate long Del_LookupEnumNameSlow(ref FScriptArray inTestShortName, IntPtr foundEnum);
        public delegate long Del_ParseEnum(ref FScriptArray str);
        public delegate csbool Del_SetEnums(IntPtr instance, IntPtr inNames, IntPtr inValues, UEnum.ECppForm inCppForm, csbool addMaxKeyIfMissing);
        public delegate int Del_NumEnums(IntPtr instance);
        public delegate void Del_GenerateEnumPrefix(IntPtr instance, ref FScriptArray result);
        public delegate void Del_GetToolTipByIndex(IntPtr instance, int nameIndex, ref FScriptArray result);
        public delegate csbool Del_HasMetaData(IntPtr instance, ref FScriptArray key, int nameIndex);
        public delegate void Del_GetMetaData(IntPtr instance, ref FScriptArray key, int nameIndex, ref FScriptArray result);
        public delegate void Del_SetMetaData(IntPtr instance, ref FScriptArray key, ref FScriptArray inValue, int nameIndex);
        public delegate void Del_RemoveMetaData(IntPtr instance, ref FScriptArray key, int nameIndex);
        
        public static Del_Get_CppType Get_CppType;
        public static Del_Set_CppType Set_CppType;
        public static Del_GetIndexByValue GetIndexByValue;
        public static Del_GetValueByIndex GetValueByIndex;
        public static Del_GetNameByIndex GetNameByIndex;
        public static Del_GetIndexByName GetIndexByName;
        public static Del_GetNameByValue GetNameByValue;
        public static Del_GetValueByName GetValueByName;
        public static Del_GetNameStringByIndex GetNameStringByIndex;
        public static Del_GetIndexByNameString GetIndexByNameString;
        public static Del_GetNameStringByValue GetNameStringByValue;
        public static Del_GetValueByNameString GetValueByNameString;        
        public static Del_GetDisplayNameTextStringByIndex GetDisplayNameTextStringByIndex;
        public static Del_GetDisplayNameTextStringByValue GetDisplayNameTextStringByValue;
        public static Del_GetMaxEnumValue GetMaxEnumValue;
        public static Del_IsValidEnumValue IsValidEnumValue;
        public static Del_IsValidEnumName IsValidEnumName;
        public static Del_RemoveNamesFromMasterList RemoveNamesFromMasterList;
        public static Del_ResolveEnumerator ResolveEnumerator;
        public static Del_GetCppForm GetCppForm;
        public static Del_IsFullEnumName IsFullEnumName;
        public static Del_GenerateFullEnumName GenerateFullEnumName;
        public static Del_LookupEnumName LookupEnumName;
        public static Del_LookupEnumNameSlow LookupEnumNameSlow;
        public static Del_ParseEnum ParseEnum;
        public static Del_SetEnums SetEnums;
        public static Del_NumEnums NumEnums;
        public static Del_GenerateEnumPrefix GenerateEnumPrefix;
        public static Del_GetToolTipByIndex GetToolTipByIndex;
        public static Del_HasMetaData HasMetaData;
        public static Del_GetMetaData GetMetaData;
        public static Del_SetMetaData SetMetaData;
        public static Del_RemoveMetaData RemoveMetaData;
    }
}
