using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Class for platform properties.
    /// 
    /// These are shared between:
    ///     the runtime platform - via FPlatformProperties
    ///     the target platforms - via ITargetPlatform
    /// </summary>
    public static class FPlatformProperties
    {
        /// <summary>
        /// Gets the platform's physics format.
        /// </summary>
        /// <returns>The physics format name.</returns>
        public static string GetPhysicsFormat()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPlatformProperties.GetPhysicsFormat(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets whether this platform has Editor-only data.
        /// </summary>
        /// <returns>true if the platform has Editor-only data, false otherwise.</returns>
        public static bool HasEditorOnlyData()
        {
            return Native_FPlatformProperties.HasEditorOnlyData();
        }

        /// <summary>
        /// Gets the name of this platform when loading INI files. Defaults to PlatformName.
        /// 
        /// Note: MUST be implemented per platform.
        /// </summary>
        /// <returns>Platform name.</returns>
        public static string IniPlatformName()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPlatformProperties.IniPlatformName(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets whether this is a game only platform.
        /// </summary>
        /// <returns>true if this is a game only platform, false otherwise.</returns>
        public static bool IsGameOnly()
        {
            return Native_FPlatformProperties.IsGameOnly();
        }

        /// <summary>
        /// Gets whether this is a server only platform.
        /// </summary>
        /// <returns>true if this is a server only platform, false otherwise.</returns>
        public static bool IsServerOnly()
        {
            return Native_FPlatformProperties.IsServerOnly();
        }

        /// <summary>
        /// Gets whether this is a client only (no capability to run the game without connecting to a server) platform.
        /// </summary>
        /// <returns>true if this is a client only platform, false otherwise.</returns>
        public static bool IsClientOnly()
        {
            return Native_FPlatformProperties.IsClientOnly();
        }

        /// <summary>
        /// Gets whether this was a monolithic build or not
        /// </summary>
        public static bool IsMonolithicBuild()
        {
            return Native_FPlatformProperties.IsMonolithicBuild();
        }

        /// <summary>
        /// Gets whether this was a program or not
        /// </summary>
        public static bool IsProgram()
        {
            return Native_FPlatformProperties.IsProgram();
        }

        /// <summary>
        /// Gets whether this is a Little Endian platform.
        /// </summary>
        public static bool IsLittleEndian()
        {
            return Native_FPlatformProperties.IsLittleEndian();
        }

        /// <summary>
        /// Gets the name of this platform
        /// 
        /// Note: MUST be implemented per platform.
        /// </summary>
        /// <returns>Platform Name.</returns>
        public static string PlatformName()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPlatformProperties.PlatformName(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Checks whether this platform requires cooked data.
        /// </summary>
        /// <returns>true if cooked data is required, false otherwise.</returns>
        public static bool RequiresCookedData()
        {
            return Native_FPlatformProperties.RequiresCookedData();
        }

        /// <summary>
        /// Checks whether the specified build target is supported.
        /// </summary>
        /// <param name="buildTarget">The build target to check.</param>
        /// <returns>true if the build target is supported, false otherwise.</returns>
        public static bool SupportsBuildTarget(EBuildTarget buildTarget)
        {
            return Native_FPlatformProperties.SupportsBuildTarget((int)buildTarget);
        }

        /// <summary>
        /// Returns true if platform supports the AutoSDK system
        /// </summary>
        public static bool SupportsAutoSDK()
        {
            return Native_FPlatformProperties.SupportsAutoSDK();
        }

        /// <summary>
        /// Gets whether this platform supports gray scale sRGB texture formats.
        /// </summary>
        /// <returns>true if gray scale sRGB texture formats are supported.</returns>
        public static bool SupportsGrayscaleSRGB()
        {
            return Native_FPlatformProperties.SupportsGrayscaleSRGB();
        }

        /// <summary>
        /// Checks whether this platforms supports running multiple game instances on a single device.
        /// </summary>
        /// <returns>true if multiple instances are supported, false otherwise.</returns>
        public static bool SupportsMultipleGameInstances()
        {
            return Native_FPlatformProperties.SupportsMultipleGameInstances();
        }

        /// <summary>
        /// Gets whether this platform supports tessellation.
        /// </summary>
        /// <returns>true if tessellation is supported, false otherwise.</returns>
        public static bool SupportsTessellation()
        {
            return Native_FPlatformProperties.SupportsTessellation();
        }

        /// <summary>
        /// Gets whether this platform supports windowed mode rendering.
        /// </summary>
        /// <returns>true if windowed mode is supported.</returns>
        public static bool SupportsWindowedMode()
        {
            return Native_FPlatformProperties.SupportsWindowedMode();
        }

        /// <summary>
        /// Whether this platform wants to allow framerate smoothing or not.
        /// </summary>
        public static bool AllowsFramerateSmoothing()
        {
            return Native_FPlatformProperties.AllowsFramerateSmoothing();
        }

        /// <summary>
        /// Whether this platform supports streaming audio
        /// </summary>
        public static bool SupportsAudioStreaming()
        {
            return Native_FPlatformProperties.SupportsAudioStreaming();
        }
        
        public static bool SupportsHighQualityLightmaps()
        {
            return Native_FPlatformProperties.SupportsHighQualityLightmaps();
        }

        public static bool SupportsLowQualityLightmaps()
        {
            return Native_FPlatformProperties.SupportsLowQualityLightmaps();
        }

        public static bool SupportsDistanceFieldShadows()
        {
            return Native_FPlatformProperties.SupportsDistanceFieldShadows();
        }

        public static bool SupportsTextureStreaming()
        {
            return Native_FPlatformProperties.SupportsTextureStreaming();
        }

        /// <summary>
        /// Gets whether user settings should override the resolution or not
        /// </summary>
        public static bool HasFixedResolution()
        {
            return Native_FPlatformProperties.HasFixedResolution();
        }

        public static bool SupportsMinimize()
        {
            return Native_FPlatformProperties.SupportsMinimize();
        }

        /// <summary>
        /// Whether the platform allows an application to quit to the OS
        /// </summary>
        public static bool SupportsQuit()
        {
            return Native_FPlatformProperties.SupportsQuit();
        }

        /// <summary>
        /// Whether the platform allows the call stack to be dumped during an assert
        /// </summary>
        public static bool AllowsCallStackDumpDuringAssert()
        {
            return Native_FPlatformProperties.AllowsCallStackDumpDuringAssert();
        }

        // The platform shouldn't change, cache it from the platform name.
        static bool hasPlatformValue;
        static EPlatform platform;
        public static EPlatform GetPlatform()
        {
            if (hasPlatformValue)
            {
                return platform;
            }

            string platformName = IniPlatformName();
            if (!string.IsNullOrEmpty(platformName))
            {
                switch (platformName.ToLower())
                {
                    case "windows": platform = EPlatform.Windows; break;// Engine\Source\Runtime\Core\Public\Windows\WindowsPlatformProperties.h
                    case "ps4": platform = EPlatform.PS4; break;// Engine\Source\Runtime\Core\Public\PS4\PS4Properties.h
                    case "xboxone": platform = EPlatform.XboxOne; break;// Engine\Source\Runtime\Core\Public\XboxOne\XboxOneProperties.h
                    case "mac": platform = EPlatform.Mac; break;// Engine\Source\Runtime\Core\Public\Mac\MacPlatformProperties.h
                    case "ios": platform = EPlatform.IOS; break;// Engine\Source\Runtime\Core\Public\iOS\IOSPlatformProperties.h
                    case "android": platform = EPlatform.Android; break;// Engine\Source\Runtime\Core\Public\Android\AndroidProperties.h
                    case "uwp": platform = EPlatform.UWP; break; // Engine\Source\Runtime\Core\Public\UWP\UWPProperties.h
                    case "html5": platform = EPlatform.HTML5; break;// Engine\Source\Runtime\Core\Public\HTML5\HTML5PlatformProperties.h                
                    case "linux": platform = EPlatform.Linux; break;// Engine\Source\Runtime\Core\Public\Linux\LinuxPlatformProperties.h
                    case "switch": platform = EPlatform.Switch; break;// Engine\Source\Runtime\Core\Public\Switch\SwitchPlatformProperties.h
                    default: platform = EPlatform.Unknown; break;
                }
            }

            hasPlatformValue = true;
            return platform;
        }
    }

    // Engine\Source\Runtime\Core\Public\HAL\PlatformProperties.h
    public enum EPlatform
    {
        Unknown,
        Windows,
        PS4,
        XboxOne,
        Mac,
        IOS,
        Android,
        UWP,
        HTML5,
        Linux,
        Switch
    }

    public enum EBuildTarget
    {
        /// <summary>
        /// Unknown build target.
        /// </summary>
        Unknown,

        /// <summary>
        /// Editor target.
        /// </summary>
        Editor,

        /// <summary>
        /// Game target.
        /// </summary>
        Game,

        /// <summary>
        /// Server target.
        /// </summary>
        Server
    }
}
