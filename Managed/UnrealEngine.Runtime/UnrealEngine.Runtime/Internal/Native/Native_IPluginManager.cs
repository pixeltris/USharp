using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_IPluginManager
    {
        public delegate IntPtr Del_Get();
        public delegate void Del_RefreshPluginsList(IntPtr instance);
        public delegate csbool Del_LoadModulesForEnabledPlugins(IntPtr instance, int loadingPhase);
        public delegate void Del_GetLocalizationPathsForEnabledPlugins(IntPtr instance, IntPtr result);
        public delegate csbool Del_AreRequiredPluginsAvailable(IntPtr instance);
        public delegate csbool Del_CheckModuleCompatibility(IntPtr instance, IntPtr outIncompatibleModules);
        public delegate void Del_FindPlugin(IntPtr instance, ref FScriptArray name, out FSharedPtr result);
        public delegate void Del_GetEnabledPlugins(IntPtr instance, IntPtr result);
        public delegate void Del_GetEnabledPluginsWithContent(IntPtr instance, IntPtr result);
        public delegate void Del_GetDiscoveredPlugins(IntPtr instance, IntPtr result);
        public delegate void Del_AddPluginSearchPath(IntPtr instance, ref FScriptArray extraDiscoveryPath, csbool refresh);
        public delegate void Del_GetPluginsWithPakFile(IntPtr instance, IntPtr result);
        public delegate void Del_MountNewlyCreatedPlugin(IntPtr instance, ref FScriptArray pluginName);

        public static Del_Get Get;
        public static Del_RefreshPluginsList RefreshPluginsList;
        public static Del_LoadModulesForEnabledPlugins LoadModulesForEnabledPlugins;
        public static Del_GetLocalizationPathsForEnabledPlugins GetLocalizationPathsForEnabledPlugins;
        public static Del_AreRequiredPluginsAvailable AreRequiredPluginsAvailable;
        public static Del_CheckModuleCompatibility CheckModuleCompatibility;
        public static Del_FindPlugin FindPlugin;
        public static Del_GetEnabledPlugins GetEnabledPlugins;
        public static Del_GetEnabledPluginsWithContent GetEnabledPluginsWithContent;
        public static Del_GetDiscoveredPlugins GetDiscoveredPlugins;
        public static Del_AddPluginSearchPath AddPluginSearchPath;
        public static Del_GetPluginsWithPakFile GetPluginsWithPakFile;
        public static Del_MountNewlyCreatedPlugin MountNewlyCreatedPlugin;
    }
}
