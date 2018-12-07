using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FAssetData
    {
        public delegate IntPtr Del_Get_TagsAndValues(ref FAssetDataNative instance, IntPtr outTags, IntPtr outValues);
        public delegate csbool Del_IsValid(ref FAssetDataNative instance);
        public delegate csbool Del_IsUAsset(ref FAssetDataNative instance);
        public delegate csbool Del_IsRedirector(ref FAssetDataNative instance);
        public delegate void Del_GetFullName(ref FAssetDataNative instance, ref FScriptArray result);
        public delegate void Del_GetExportTextName(ref FAssetDataNative instance, ref FScriptArray result);

        public static Del_Get_TagsAndValues Get_TagsAndValues;
        public static Del_IsValid IsValid;
        public static Del_IsUAsset IsUAsset;
        public static Del_IsRedirector IsRedirector;
        public static Del_GetFullName GetFullName;
        public static Del_GetExportTextName GetExportTextName;
    }
}
