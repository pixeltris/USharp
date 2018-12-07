using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UMetaData
    {
        public delegate void Del_GetValue(IntPtr instance, IntPtr obj, ref FScriptArray key, ref FScriptArray result);
        public delegate void Del_GetValueFName(IntPtr instance, IntPtr obj, ref FName key, ref FScriptArray result);
        public delegate csbool Del_HasValue(IntPtr instance, IntPtr obj, ref FScriptArray key);
        public delegate csbool Del_HasValueFName(IntPtr instance, IntPtr obj, ref FName key);
        public delegate csbool Del_HasObjectValues(IntPtr instance, IntPtr obj);
        public delegate void Del_SetObjectValues(IntPtr instance, IntPtr obj, IntPtr keys, IntPtr values);
        public delegate void Del_SetValue(IntPtr instance, IntPtr obj, ref FScriptArray key, ref FScriptArray value);
        public delegate void Del_SetValueFName(IntPtr instance, IntPtr obj, ref FName key, ref FScriptArray value);
        public delegate void Del_RemoveValue(IntPtr instance, IntPtr obj, ref FScriptArray key);
        public delegate void Del_RemoveValueFName(IntPtr instance, IntPtr obj, ref FName key);
        public delegate void Del_GetMapForObject(IntPtr obj, IntPtr keys, IntPtr values);
        public delegate void Del_CopyMetadata(IntPtr sourceObject, IntPtr destObject);
        public delegate void Del_RemoveMetaDataOutsidePackage(IntPtr instance);

        public static Del_GetValue GetValue;
        public static Del_GetValueFName GetValueFName;
        public static Del_HasValue HasValue;
        public static Del_HasValueFName HasValueFName;
        public static Del_HasObjectValues HasObjectValues;
        public static Del_SetObjectValues SetObjectValues;
        public static Del_SetValue SetValue;
        public static Del_SetValueFName SetValueFName;
        public static Del_RemoveValue RemoveValue;
        public static Del_RemoveValueFName RemoveValueFName;
        public static Del_GetMapForObject GetMapForObject;
        public static Del_CopyMetadata CopyMetadata;
        public static Del_RemoveMetaDataOutsidePackage RemoveMetaDataOutsidePackage;
    }
}
