using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_IPlugin
    {
        public delegate void Del_GetName(IntPtr instance, ref FScriptArray result);
        public delegate void Del_GetDescriptorFileName(IntPtr instance, ref FScriptArray result);
        public delegate void Del_GetBaseDir(IntPtr instance, ref FScriptArray result);
        public delegate void Del_GetContentDir(IntPtr instance, ref FScriptArray result);
        public delegate void Del_GetMountedAssetPath(IntPtr instance, ref FScriptArray result);
        public delegate int Del_GetPluginType(IntPtr instance);
        public delegate csbool Del_IsEnabled(IntPtr instance);
        public delegate csbool Del_IsEnabledByDefault(IntPtr instance);
        public delegate csbool Del_IsHidden(IntPtr instance);
        public delegate csbool Del_CanContainContent(IntPtr instance);
        public delegate int Del_GetLoadedFrom(IntPtr instance);

        public static Del_GetName GetName;
        public static Del_GetDescriptorFileName GetDescriptorFileName;
        public static Del_GetBaseDir GetBaseDir;
        public static Del_GetContentDir GetContentDir;
        public static Del_GetMountedAssetPath GetMountedAssetPath;
        public static Del_GetPluginType GetPluginType;
        public static Del_IsEnabled IsEnabled;
        public static Del_IsEnabledByDefault IsEnabledByDefault;
        public static Del_IsHidden IsHidden;
        public static Del_CanContainContent CanContainContent;
        public static Del_GetLoadedFrom GetLoadedFrom;
    }
}
