using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public class FPaths
    {
        /// <summary>
        /// Should the "saved" directory structures be rooted in the user dir or relative to the "engine/game" 
        /// </summary>
        public static bool ShouldSaveToUserDir
        {
            get { return Native_FPaths.ShouldSaveToUserDir(); }
        }

        /// <summary>
        /// Returns the directory the application was launched from (useful for commandline utilities)
        /// </summary>
        public static string LaunchDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.LaunchDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the base directory of the "core" engine that can be shared across
        /// several games or across games &amp; mods. Shaders and base localization files
        /// e.g. reside in the engine directory.
        /// </summary>
        public static string EngineDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.EngineDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the root directory for user-specific engine files. Always writable.
        /// </summary>
        public static string EngineUserDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.EngineUserDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the root directory for user-specific engine files which can be shared between versions. Always writable.
        /// </summary>
        public static string EngineVersionAgnosticUserDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.EngineVersionAgnosticUserDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the content directory of the "core" engine that can be shared across
        /// several games or across games &amp; mods.
        /// </summary>
        public static string EngineContentDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.EngineContentDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the directory the root configuration files are located.
        /// </summary>
        public static string EngineConfigDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.EngineConfigDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the intermediate directory of the engine
        /// </summary>
        public static string EngineIntermediateDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.EngineIntermediateDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the saved directory of the engine
        /// </summary>
        public static string EngineSavedDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.EngineSavedDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the plugins directory of the engine
        /// </summary>
        public static string EnginePluginsDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.EnginePluginsDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the root directory of the engine directory tree
        /// </summary>
        public static string RootDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.RootDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the base directory of the current game by looking at FApp::GetProjectName().
        /// This is usually a subdirectory of the installation
        /// root directory and can be overridden on the command line to allow self
        /// contained mod support.
        /// </summary>
        public static string ProjectDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.ProjectDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the root directory for user-specific game files.
        /// </summary>
        public static string ProjectUserDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.ProjectUserDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the content directory of the current game by looking at FApp::GetProjectName().
        /// </summary>
        public static string ProjectContentDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.ProjectContentDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the directory the root configuration files are located.
        /// </summary>
        public static string ProjectConfigDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.ProjectConfigDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the saved directory of the current game by looking at FApp::GetProjectName().
        /// </summary>
        public static string ProjectSavedDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.ProjectSavedDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the intermediate directory of the current game by looking at FApp::GetProjectName().
        /// </summary>
        public static string ProjectIntermediateDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.ProjectIntermediateDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the plugins directory of the current game by looking at FApp::GetProjectName().
        /// </summary>
        public static string ProjectPluginsDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.ProjectPluginsDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the writable directory for downloaded data that persists across play sessions.
        /// </summary>
        public static string ProjectPersistentDownloadDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.ProjectPersistentDownloadDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the directory the engine uses to look for the source leaf ini files. This
        /// can't be an .ini variable for obvious reasons.
        /// </summary>
        public static string SourceConfigDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.SourceConfigDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the directory the engine saves generated config files.
        /// </summary>
        public static string GeneratedConfigDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.GeneratedConfigDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the directory the engine stores sandbox output
        /// </summary>
        public static string SandboxesDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.SandboxesDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the directory the engine uses to output profiling files.
        /// </summary>
        public static string ProfilingDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.ProfilingDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the directory the engine uses to output screenshot files.
        /// </summary>
        public static string ScreenShotDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.ScreenShotDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the directory the engine uses to output BugIt files.
        /// </summary>
        public static string BugItDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.BugItDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the directory the engine uses to output user requested video capture files.
        /// </summary>
        public static string VideoCaptureDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.VideoCaptureDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns the directory the engine uses to output logs. This currently can't 
        /// be an .ini setting as the game starts logging before it can read from .ini
        /// files.
        /// </summary>
        public static string ProjectLogDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.ProjectLogDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The directory for automation save files
        /// </summary>
        public static string AutomationDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.AutomationDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The directory for automation save files that are meant to be deleted every run
        /// </summary>
        public static string AutomationTransientDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.AutomationTransientDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The directory for automation log files.
        /// </summary>
        public static string AutomationLogDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.AutomationLogDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The directory for local files used in cloud emulation or support
        /// </summary>
        public static string CloudDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.CloudDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The directory that contains subfolders for developer-specific content
        /// </summary>
        public static string GameDevelopersDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.GameDevelopersDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The directory that contains developer-specific content for the current user
        /// </summary>
        public static string GameUserDeveloperDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.GameUserDeveloperDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The directory for temp files used for diff'ing
        /// </summary>
        public static string DiffDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.DiffDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns a list of engine-specific localization paths
        /// </summary>
        /// <returns></returns>
        public static string[] GetEngineLocalizationPaths()
        {
            return GetStringArray(Native_FPaths.GetEngineLocalizationPaths());
        }

        /// <summary>
        /// Returns a list of editor-specific localization paths
        /// </summary>
        /// <returns></returns>
        public static string[] GetEditorLocalizationPaths()
        {
            return GetStringArray(Native_FPaths.GetEditorLocalizationPaths());
        }

        /// <summary>
        /// Returns a list of property name localization paths
        /// </summary>
        /// <returns></returns>
        public static string[] GetPropertyNameLocalizationPaths()
        {
            return GetStringArray(Native_FPaths.GetPropertyNameLocalizationPaths());
        }

        /// <summary>
        /// Returns a list of tool tip localization paths
        /// </summary>
        /// <returns></returns>
        public static string[] GetToolTipLocalizationPaths()
        {
            return GetStringArray(Native_FPaths.GetToolTipLocalizationPaths());
        }

        /// <summary>
        /// Returns a list of game-specific localization paths
        /// </summary>
        /// <returns></returns>
        public static string[] GetGameLocalizationPaths()
        {
            return GetStringArray(Native_FPaths.GetGameLocalizationPaths());
        }

        /// <summary>
        /// Returns the saved directory that is not game specific. This is usually the same as
        /// EngineSavedDir().
        /// </summary>
        /// <returns></returns>
        public static string GameAgnosticSavedDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.GameAgnosticSavedDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The directory where engine source code files are kept
        /// </summary>
        public static string EngineSourceDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.EngineSourceDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The directory where game source code files are kept
        /// </summary>
        public static string GameSourceDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.GameSourceDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The directory where feature packs are kept
        /// </summary>
        public static string FeaturePackDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.FeaturePackDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Checks whether the path to the project file, if any, is set.
        /// </summary>
        public static bool IsProjectFilePathSet
        {
            get { return Native_FPaths.IsProjectFilePathSet(); }
        }

        /// <summary>
        /// Gets the path to the project file.
        /// </summary>
        /// <returns></returns>
        public static string ProjectFilePath
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FPaths.GetProjectFilePath(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Sets the path to the project file.
        /// </summary>
        /// <param name="newGameProjectFilePath">The project file path to set.</param>
        public static void SetProjectFilePath(string newGameProjectFilePath)
        {
            using (FStringUnsafe newGameProjectFilePathUnsafe = new FStringUnsafe(newGameProjectFilePath))
            {
                // Make sure this doesn't hold onto the address of this string
                Native_FPaths.SetProjectFilePath(ref newGameProjectFilePathUnsafe.Array);
            }
        }

        /// <summary>
        /// Gets the extension for this filename.
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="includeDot">if true, includes the leading dot in the result</param>
        /// <returns></returns>
        public static string GetExtension(string inPath, bool includeDot = false)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPaths.GetExtension(ref inPathUnsafe.Array, includeDot, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the filename (with extension), minus any path information.
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns></returns>
        public static string GetCleanFilename(string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPaths.GetCleanFilename(ref inPathUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the same thing as GetCleanFilename, but without the extension
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="removePath"></param>
        /// <returns></returns>
        public static string GetBaseFilename(string inPath, bool removePath = true)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPaths.GetBaseFilename(ref inPathUnsafe.Array, removePath, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the path in front of the filename
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns></returns>
        public static string GetPath(string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPaths.GetPath(ref inPathUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Changes the extension of the given filename
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="inNewExtension"></param>
        /// <returns></returns>
        public static string ChangeExtension(string inPath, string inNewExtension)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            using (FStringUnsafe inNewExtensionUnsafe = new FStringUnsafe(inNewExtension))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPaths.ChangeExtension(ref inPathUnsafe.Array, ref inNewExtensionUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Checks if the file path exists
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns>True if this file was found, false otherwise</returns>
        public static bool FileExists(string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            {
                return Native_FPaths.FileExists(ref inPathUnsafe.Array);
            }
        }

        /// <summary>
        /// Checks if the directory exists
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns>True if this directory was found, false otherwise</returns>
        public static bool DirectoryExists(string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            {
                return Native_FPaths.DirectoryExists(ref inPathUnsafe.Array);
            }
        }

        public static void MakeDirectory(string inPath)
        {
            // MakeDirectory is actually under IFileManager, just emulate it here
            Directory.CreateDirectory(inPath);
        }

        /// <summary>
        /// Checks if drive exists
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns>True if this path represents a drive</returns>
        public static bool IsDrive(string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            {
                return Native_FPaths.IsDrive(ref inPathUnsafe.Array);
            }
        }

        /// <summary>
        /// Checks if the path is valid
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns>True if this path is relative</returns>
        public static bool IsRelative(string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            {
                return Native_FPaths.IsRelative(ref inPathUnsafe.Array);
            }
        }

        /// <summary>
        /// Convert all / and \ to TEXT("/")
        /// </summary>
        /// <param name="inPath"></param>
        public static string NormalizeFilename(string inPath)
        {
            NormalizeFilename(ref inPath);
            return inPath;
        }

        /// <summary>
        /// Convert all / and \ to TEXT("/")
        /// </summary>
        /// <param name="inPath"></param>
        public static void NormalizeFilename(ref string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            {
                Native_FPaths.NormalizeFilename(ref inPathUnsafe.Array);
                inPath = inPathUnsafe.Value;
            }
        }

        /// <summary>
        /// Checks if two paths are the same.
        /// </summary>
        /// <param name="pathA">PathA First path to check.</param>
        /// <param name="pathB">PathB Second path to check.</param>
        /// <returns>True if both paths are the same. False otherwise.</returns>
        public static bool IsSamePath(string pathA, string pathB)
        {
            using (FStringUnsafe pathAUnsafe = new FStringUnsafe(pathA))
            using (FStringUnsafe pathBUnsafe = new FStringUnsafe(pathB))
            {
                return Native_FPaths.IsSamePath(ref pathAUnsafe.Array, ref pathBUnsafe.Array);
            }
        }

        /// <summary>
        /// Normalize all / and \ to TEXT("/") and remove any trailing TEXT("/") if the character before that is not a TEXT("/") or a colon
        /// </summary>
        /// <param name="inPath"></param>
        public static void NormalizeDirectoryName(ref string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            {
                Native_FPaths.NormalizeDirectoryName(ref inPathUnsafe.Array);
                inPathUnsafe.Value = inPath;
            }
        }

        /// <summary>
        /// Takes a fully pathed string and eliminates relative pathing (eg: annihilates ".." with the adjacent directory).
        /// Assumes all slashes have been converted to TEXT('/').
        /// For example, takes the string:
        ///  BaseDirectory/SomeDirectory/../SomeOtherDirectory/Filename.ext
        /// and converts it to:
        ///  BaseDirectory/SomeOtherDirectory/Filename.ext
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns></returns>
        public static bool CollapseRelativeDirectories(ref string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            {
                bool result = Native_FPaths.CollapseRelativeDirectories(ref inPathUnsafe.Array);
                inPathUnsafe.Value = inPath;
                return result;
            }
        }

        /// <summary>
        /// Removes duplicate slashes in paths.
        /// Assumes all slashes have been converted to TEXT('/').
        /// For example, takes the string:
        ///  BaseDirectory/SomeDirectory//SomeOtherDirectory////Filename.ext
        /// and converts it to:
        ///  BaseDirectory/SomeDirectory/SomeOtherDirectory/Filename.ext
        /// </summary>
        /// <param name="inPath"></param>
        public static void RemoveDuplicateSlashes(ref string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            {
                Native_FPaths.RemoveDuplicateSlashes(ref inPathUnsafe.Array);
                inPathUnsafe.Value = inPath;
            }
        }

        /// <summary>
        /// Make fully standard "Unreal" pathname:
        ///    - Normalizes path separators [NormalizeFilename]
        ///    - Removes extraneous separators  [NormalizeDirectoryName, as well removing adjacent separators]
        ///    - Collapses internal ..'s
        ///    - Makes relative to Engine\Binaries\[PLATFORM]\ (will ALWAYS start with ..\..\..)
        /// </summary>
        /// <param name="inPath"></param>
        public static void MakeStandardFilename(ref string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            {
                Native_FPaths.MakeStandardFilename(ref inPathUnsafe.Array);
                inPathUnsafe.Value = inPath;
            }
        }

        /// <summary>
        /// Takes an "Unreal" pathname and converts it to a platform filename.
        /// </summary>
        /// <param name="inPath"></param>
        public static void MakePlatformFilename(ref string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            {
                Native_FPaths.MakePlatformFilename(ref inPathUnsafe.Array);
                inPathUnsafe.Value = inPath;
            }
        }

        /// <summary>
        /// Assuming both paths (or filenames) are relative to the base dir, find the relative path to the InPath.
        /// </summary>
        /// <param name="inPath">Path to make this path relative to.</param>
        /// <param name="inRelativeTo">Path relative to InPath</param>
        /// <returns></returns>
        public static string MakePathRelativeTo(string inPath, string inRelativeTo)
        {
            string result = inPath;
            if (MakePathRelativeTo(ref result, inRelativeTo))
            {
                return result;
            }
            return inPath;
        }

        /// <summary>
        /// Assuming both paths (or filenames) are relative to the base dir, find the relative path to the InPath.
        /// </summary>
        /// <param name="inPath">Path to make this path relative to.</param>
        /// <param name="inRelativeTo">Path relative to InPath</param>
        /// <returns></returns>
        public static bool MakePathRelativeTo(ref string inPath, string inRelativeTo)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            using (FStringUnsafe inRelativeToUnsafe = new FStringUnsafe(inRelativeTo))
            {
                bool result = Native_FPaths.MakePathRelativeTo(ref inPathUnsafe.Array, ref inRelativeToUnsafe.Array);
                inPath = inPathUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Converts a relative path name to a fully qualified name relative to the process BaseDir().
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns></returns>
        public static string ConvertRelativePathToFull(string inPath)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPaths.ConvertRelativePathToFull(ref inPathUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Converts a relative path name to a fully qualified name relative to the specified BasePath.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="inPath"></param>
        /// <returns></returns>
        public static string ConvertRelativePathToFull(string basePath, string inPath)
        {
            using (FStringUnsafe basePathUnsafe = new FStringUnsafe(basePath))
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPaths.ConvertRelativePathToFullBase(ref basePathUnsafe.Array, ref inPathUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Converts a normal path to a sandbox path (in Saved/Sandboxes).
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="inSandboxName">The name of the sandbox.</param>
        /// <returns></returns>
        public static string ConvertToSandboxPath(string inPath, string inSandboxName)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            using (FStringUnsafe inSandboxNameUnsafe = new FStringUnsafe(inSandboxName))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPaths.ConvertFromSandboxPath(ref inPathUnsafe.Array, ref inSandboxNameUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Converts a sandbox (in Saved/Sandboxes) path to a normal path.
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="inSandboxName">The name of the sandbox.</param>
        /// <returns></returns>
        public static string ConvertFromSandboxPath(string inPath, string inSandboxName)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            using (FStringUnsafe inSandboxNameUnsafe = new FStringUnsafe(inSandboxName))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPaths.ConvertFromSandboxPath(ref inPathUnsafe.Array, ref inSandboxNameUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Creates a temporary filename with the specified prefix.
        /// </summary>
        /// <param name="path">The file pathname.</param>
        /// <param name="prefix">The file prefix.</param>
        /// <param name="extension">File extension ('.' required).</param>
        /// <returns></returns>
        public static string CreateTempFilename(string path, string prefix = "", string extension = ".tmp")
        {
            using (FStringUnsafe pathUnsafe = new FStringUnsafe(path))
            using (FStringUnsafe prefixUnsafe = new FStringUnsafe(prefix))
            using (FStringUnsafe extensionUnsafe = new FStringUnsafe(extension))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPaths.CreateTempFilename(ref pathUnsafe.Array, ref prefixUnsafe.Array, ref extensionUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Validates that the parts that make up the path contain no invalid characters as dictated by the operating system
        /// Note that this is a different set of restrictions to those imposed by FPackageName
        /// </summary>
        /// <param name="inPath">path to validate</param>
        /// <param name="reason">optional parameter to fill with the failure reason</param>
        /// <returns></returns>
        public static bool ValidatePath(string inPath, out string reason)
        {
            using (FStringUnsafe reasonUnsafe = new FStringUnsafe())
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            {
                bool result = Native_FPaths.ValidatePath(ref inPathUnsafe.Array, ref reasonUnsafe.Array);
                reason = reasonUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Parses a fully qualified or relative filename into its components (filename, path, extension).
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="pathPart">[out] receives the value of the path portion of the input string</param>
        /// <param name="filenamePart">[out] receives the value of the filename portion of the input string</param>
        /// <param name="extensionPart">[out] receives the value of the extension portion of the input string</param>
        public static void Split(string inPath, ref string pathPart, ref string filenamePart, ref string extensionPart)
        {
            using (FStringUnsafe inPathUnsafe = new FStringUnsafe(inPath))
            using (FStringUnsafe pathPartUnsafe = new FStringUnsafe(pathPart))
            using (FStringUnsafe filenamePartUnsafe = new FStringUnsafe(filenamePart))
            using (FStringUnsafe extensionPartUnsafe = new FStringUnsafe(extensionPart))
            {
                Native_FPaths.Split(ref inPathUnsafe.Array, ref pathPartUnsafe.Array, ref filenamePartUnsafe.Array, ref extensionPartUnsafe.Array);
            }
        }

        /// <summary>
        /// Gets thhe relative path to get from BaseDir to RootDirectory
        /// </summary>
        /// <returns></returns>
        public static string GetRelativePathToRoot()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPaths.GetRelativePathToRoot(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public static string Combine(params string[] paths)
        {
            if (paths != null && paths.Length > 0)
            {
                string result = paths[0];
                using (FStringUnsafe pathAUnsafe = new FStringUnsafe())
                using (FStringUnsafe pathBUnsafe = new FStringUnsafe())
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    for (int i = 1; i < paths.Length; i++)
                    {
                        pathAUnsafe.Value = result;
                        pathBUnsafe.Value = paths[i];
                        Native_FPaths.Combine(ref pathAUnsafe.Array, ref pathBUnsafe.Array, ref resultUnsafe.Array);
                        result = resultUnsafe.Value;
                    }
                }
                return result;
            }
            return null;
        }

        private static string[] GetStringArray(IntPtr stringArray)
        {
            return new TArrayUnsafeRef<string>(stringArray).ToArray();
        }

        // IsSameOrSubDirectory is a copy from Loader/EntryPoint.cs

        public static bool IsFileInDirectoryOrSubDirectory(string filePath, string directory)
        {
            return IsSameOrSubDirectory(directory, Path.GetDirectoryName(filePath));
        }

        public static bool IsSameOrSubDirectory(string basePath, string path)
        {
            string subDirectory;
            return IsSameOrSubDirectory(basePath, path, out subDirectory);
        }

        public static bool IsSameOrSubDirectory(string basePath, string path, out string subDirectory)
        {
            DirectoryInfo di = new DirectoryInfo(Path.GetFullPath(path).TrimEnd('\\', '/'));
            DirectoryInfo diBase = new DirectoryInfo(Path.GetFullPath(basePath).TrimEnd('\\', '/'));

            subDirectory = null;
            while (di != null)
            {
                if (di.FullName.Equals(diBase.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else
                {
                    if (string.IsNullOrEmpty(subDirectory))
                    {
                        subDirectory = di.Name;
                    }
                    else
                    {
                        subDirectory = Path.Combine(di.Name, subDirectory);
                    }
                    di = di.Parent;
                }
            }
            return false;
        }
    }
}
