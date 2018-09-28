using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FPlatformProperties
    {
        public delegate void Del_GetPhysicsFormat(ref FScriptArray result);
        public delegate csbool Del_HasEditorOnlyData();
        public delegate void Del_IniPlatformName(ref FScriptArray result);
        public delegate csbool Del_IsGameOnly();
        public delegate csbool Del_IsServerOnly();
        public delegate csbool Del_IsClientOnly();
        public delegate csbool Del_IsMonolithicBuild();
        public delegate csbool Del_IsProgram();
        public delegate csbool Del_IsLittleEndian();
        public delegate void Del_PlatformName(ref FScriptArray result);
        public delegate csbool Del_RequiresCookedData();
        public delegate csbool Del_SupportsBuildTarget(int buildTarget);
        public delegate csbool Del_SupportsAutoSDK();
        public delegate csbool Del_SupportsGrayscaleSRGB();
        public delegate csbool Del_SupportsMultipleGameInstances();
        public delegate csbool Del_SupportsTessellation();
        public delegate csbool Del_SupportsWindowedMode();
        public delegate csbool Del_AllowsFramerateSmoothing();
        public delegate csbool Del_SupportsAudioStreaming();
        public delegate csbool Del_SupportsHighQualityLightmaps();
        public delegate csbool Del_SupportsLowQualityLightmaps();
        public delegate csbool Del_SupportsDistanceFieldShadows();
        public delegate csbool Del_SupportsTextureStreaming();
        public delegate csbool Del_HasFixedResolution();
        public delegate csbool Del_SupportsMinimize();
        public delegate csbool Del_SupportsQuit();
        public delegate csbool Del_AllowsCallStackDumpDuringAssert();

        public static Del_GetPhysicsFormat GetPhysicsFormat;
        public static Del_HasEditorOnlyData HasEditorOnlyData;
        public static Del_IniPlatformName IniPlatformName;
        public static Del_IsGameOnly IsGameOnly;
        public static Del_IsServerOnly IsServerOnly;
        public static Del_IsClientOnly IsClientOnly;
        public static Del_IsMonolithicBuild IsMonolithicBuild;
        public static Del_IsProgram IsProgram;
        public static Del_IsLittleEndian IsLittleEndian;
        public static Del_PlatformName PlatformName;
        public static Del_RequiresCookedData RequiresCookedData;
        public static Del_SupportsBuildTarget SupportsBuildTarget;
        public static Del_SupportsAutoSDK SupportsAutoSDK;
        public static Del_SupportsGrayscaleSRGB SupportsGrayscaleSRGB;
        public static Del_SupportsMultipleGameInstances SupportsMultipleGameInstances;
        public static Del_SupportsTessellation SupportsTessellation;
        public static Del_SupportsWindowedMode SupportsWindowedMode;
        public static Del_AllowsFramerateSmoothing AllowsFramerateSmoothing;
        public static Del_SupportsAudioStreaming SupportsAudioStreaming;
        public static Del_SupportsHighQualityLightmaps SupportsHighQualityLightmaps;
        public static Del_SupportsLowQualityLightmaps SupportsLowQualityLightmaps;
        public static Del_SupportsDistanceFieldShadows SupportsDistanceFieldShadows;
        public static Del_SupportsTextureStreaming SupportsTextureStreaming;
        public static Del_HasFixedResolution HasFixedResolution;
        public static Del_SupportsMinimize SupportsMinimize;
        public static Del_SupportsQuit SupportsQuit;
        public static Del_AllowsCallStackDumpDuringAssert AllowsCallStackDumpDuringAssert;
    }
}
