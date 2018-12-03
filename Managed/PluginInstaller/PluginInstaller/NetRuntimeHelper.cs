using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// TODO:
// Support for copying absolute paths on disk as well as paths relative to PluginInstaller

namespace PluginInstaller
{
    static class NetRuntimeHelper
    {
        static Dictionary<Profile, List<RuntimeConfigEntry>> config = new Dictionary<Profile, List<RuntimeConfigEntry>>();

        public static void CopyAll(bool minimal)
        {
            if (!LoadConfig(minimal))
            {
                return;
            }
            CopyMono(minimal);
            CopyCoreCLR(minimal);
        }

        public static void CopyMono(bool minimal)
        {
            if (!LoadConfig(minimal))
            {
                return;
            }
            CopyFiles(Profile.Mono_Win32, minimal);
            CopyFiles(Profile.Mono_Win64, minimal);
            CopyFiles(Profile.Mono_Mac, minimal);
            CopyFiles(Profile.Mono_Linux, minimal);
        }

        public static void CopyCoreCLR(bool minimal)
        {
            if (!LoadConfig(minimal))
            {
                return;
            }
            CopyFiles(Profile.CoreCLR_Win32, minimal);
            CopyFiles(Profile.CoreCLR_Win64, minimal);
            CopyFiles(Profile.CoreCLR_Mac, minimal);
            CopyFiles(Profile.CoreCLR_Linux, minimal);
        }

        private static void CopyFiles(Profile profile, bool minimal)
        {
            try
            {
                List<RuntimeConfigEntry> copyItems;
                if (!config.TryGetValue(profile, out copyItems))
                {
                    return;
                }

                bool isProfileMono = IsProfileMono(profile);
                bool isProfileCoreCLR = IsProfileCoreCLR(profile);
                Debug.Assert(!(isProfileMono && isProfileCoreCLR));
                if (!isProfileMono && !isProfileCoreCLR)
                {
                    return;
                }
                
                string basePath = FindLocalRuntimePath(profile, false);
                bool isLocalRuntimePath = !string.IsNullOrEmpty(basePath);

                string targetBasePath = null;

                switch (profile)
                {
                    case Profile.Mono_Mac:
                        targetBasePath = Path.Combine(Program.AppDirectory, "../Runtimes/Mono/Mac/");
                        break;

                    case Profile.Mono_Linux:
                        targetBasePath = Path.Combine(Program.AppDirectory, "../Runtimes/Mono/Linux/");
                        break;

                    case Profile.Mono_Win32:
                    case Profile.Mono_Win64:
                        targetBasePath = Path.Combine(Program.AppDirectory, "../Runtimes/Mono/" +
                                (profile == Profile.Mono_Win32 ? "Win32" : "Win64") + "/");
                        break;

                    case Profile.CoreCLR_Mac:
                        targetBasePath = Path.Combine(Program.AppDirectory, "../Runtimes/CoreCLR/Mac/");
                        break;

                    case Profile.CoreCLR_Linux:
                        targetBasePath = Path.Combine(Program.AppDirectory, "../Runtimes/CoreCLR/Linux/");
                        break;

                    case Profile.CoreCLR_Win32:
                    case Profile.CoreCLR_Win64:
                        targetBasePath = Path.Combine(Program.AppDirectory, "../Runtimes/CoreCLR/" +
                            (profile == Profile.CoreCLR_Win32 ? "Win32" : "Win64") + "/");
                        break;
                }

                if (string.IsNullOrEmpty(targetBasePath))
                {
                    return;
                }

                string monoBinPath = null, monoEtcPath = null, monoLibPath = null;
                string monoTargetBinPath = null, monoTargetEtcPath = null, monoTargetLibPath = null;

                if (isProfileMono)
                {
                    monoTargetBinPath = Path.Combine(targetBasePath, "bin");
                    monoTargetEtcPath = Path.Combine(targetBasePath, "etc");
                    monoTargetLibPath = Path.Combine(targetBasePath, "lib");

                    if (!string.IsNullOrEmpty(basePath))
                    {
                        monoBinPath = Path.Combine(basePath, "bin");
                        monoEtcPath = Path.Combine(basePath, "etc");
                        monoLibPath = Path.Combine(basePath, "lib");
                    }
                }

                string coreSdkDir = null, coreAspNetCoreDir = null, coreNetCoreDir = null, coreWindowsDesktopDir = null;

                if (string.IsNullOrEmpty(basePath))
                {                    
                    if (isProfileMono)
                    {
                        basePath = FindMonoPath(profile, out monoBinPath, out monoEtcPath, out monoLibPath);
                        if (monoBinPath == null || monoEtcPath == null || monoLibPath == null)
                        {
                            return;
                        }
                    }
                    else
                    {
                        basePath = FindCoreCLRPath(profile);
                    }
                }

                if (!string.IsNullOrEmpty(basePath))
                {
                    Console.WriteLine("Copying .NET runtime " + profile + "...");

                    if (isProfileCoreCLR)
                    {
                        GetCoreCLRPaths(basePath, out coreSdkDir, out coreAspNetCoreDir, out coreNetCoreDir, out coreWindowsDesktopDir);
                    }

                    if (minimal && Directory.Exists(targetBasePath))
                    {
                        // If this is a minimal build try deleting target the folder first
                        try
                        {
                            Directory.Delete(targetBasePath, true);
                        }
                        catch
                        {
                        }
                    }

                    foreach (RuntimeConfigEntry copyItem in copyItems)
                    {
                        string srcPath = null;
                        string dstPath = isProfileCoreCLR ? targetBasePath : null;
                        bool allowEmptyPath = true;
                        switch (copyItem.Folder)
                        {
                            case RuntimeConfigEntry.FolderType.Mono_Bin:
                                srcPath = monoBinPath;
                                dstPath = monoTargetBinPath;
                                break;
                            case RuntimeConfigEntry.FolderType.Mono_Etc:
                                srcPath = monoEtcPath;
                                dstPath = monoTargetEtcPath;

                                // Don't allow an empty path on non local paths as this will point to /etc/
                                allowEmptyPath = isLocalRuntimePath;
                                break;
                            case RuntimeConfigEntry.FolderType.Mono_Lib:
                                srcPath = monoLibPath;
                                dstPath = monoTargetLibPath;

                                // Don't allow an empty path on non local paths as this will point to /usr/lib/
                                allowEmptyPath = isLocalRuntimePath;
                                break;

                            case RuntimeConfigEntry.FolderType.CoreCLR_Dotnet:
                                srcPath = basePath;
                                break;
                            case RuntimeConfigEntry.FolderType.CoreCLR_Sdk:
                                srcPath = coreSdkDir;
                                break;
                            case RuntimeConfigEntry.FolderType.CoreCLR_AspNetCore:
                                srcPath = coreAspNetCoreDir;
                                break;
                            case RuntimeConfigEntry.FolderType.CoreCLR_NETCore:
                                srcPath = coreNetCoreDir;
                                break;
                            case RuntimeConfigEntry.FolderType.CoreCLR_WindowsDesktop:
                                srcPath = coreWindowsDesktopDir;
                                break;
                        }
                        if (!string.IsNullOrEmpty(srcPath) && !string.IsNullOrEmpty(dstPath))
                        {
                            if (!string.IsNullOrEmpty(copyItem.Path))
                            {
                                srcPath = Path.Combine(srcPath, copyItem.Path);
                                dstPath = Path.Combine(dstPath, copyItem.Path);
                            }
                            else if (!allowEmptyPath)
                            {
                                // Print a warning?
                                continue;
                            }

                            bool exists = false;
                            try
                            {
                                if (copyItem.Type == RuntimeConfigEntry.PathType.File)
                                {
                                    exists = File.Exists(srcPath);
                                }
                                else
                                {
                                    exists = Directory.Exists(srcPath);
                                }
                            }
                            catch
                            {
                                // Likely a badly formatted path
                            }
                            if (exists)
                            {
                                try
                                {
                                    dstPath = Path.GetFullPath(dstPath);

                                    string dstDir = Path.GetDirectoryName(dstPath);
                                    if (!Directory.Exists(dstDir))
                                    {
                                        Directory.CreateDirectory(dstDir);
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine("Failed to create directory for '" + dstPath + "'");
                                    continue;
                                }

                                switch (copyItem.Type)
                                {
                                    case RuntimeConfigEntry.PathType.File:
                                        Program.CopyFile(srcPath, dstPath, true);
                                        break;

                                    case RuntimeConfigEntry.PathType.Folder:
                                        Program.CopyFiles(new DirectoryInfo(srcPath), new DirectoryInfo(dstPath), true);
                                        break;

                                    case RuntimeConfigEntry.PathType.Recursive:
                                        if (isProfileCoreCLR)
                                        {
                                            // No recursive copying for now
                                            Program.CopyFiles(new DirectoryInfo(srcPath), new DirectoryInfo(dstPath), true);
                                        }
                                        else
                                        {
                                            Program.CopyFilesRecursive(new DirectoryInfo(srcPath), new DirectoryInfo(dstPath), true);
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    if (isProfileMono)
                    {
                        // Validate the /etc/config file, some entries are file redirects (e.g. libmono-system-native.dylib)
                        // Those redirects aren't handled in embedded mono so we need to make the config point to the real files
                        FixupMonoConfigRedirects(monoTargetEtcPath, monoTargetLibPath);
                    }

                    Console.WriteLine("Copied .NET runtime " + profile);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an exception when trying to copy the .NET runtime " + profile);
                Console.WriteLine(e);
            }
        }

        private static void FixupMonoConfigRedirects(string targetEtcPath, string targetLibPath)
        {
            string configFile = Path.Combine(targetEtcPath, "mono", "config");
            if (File.Exists(configFile))
            {
                bool modified = false;
                string[] lines = File.ReadAllLines(configFile);
                for (int i = 0; i < lines.Length; i++)
                {
                    try
                    {
                        string line = lines[i];
                        string monoLibDirPrefix = "mono_libdir/";
                        int libIndex = line.IndexOf(monoLibDirPrefix);
                        if (libIndex >= 0)
                        {
                            libIndex += monoLibDirPrefix.Length;
                            int libEndIndex = line.IndexOf("\"", libIndex);
                            string libName = line.Substring(libIndex, libEndIndex - libIndex);
                            string libPath = Path.Combine(targetLibPath, libName);
                            if (File.Exists(libPath))
                            {
                                bool isLibFile = false;

                                using (FileStream stream = File.OpenRead(libPath))
                                {
                                    if (stream.Length >= 4)
                                    {
                                        byte[] magicBytes = new byte[4];
                                        if (stream.Read(magicBytes, 0, magicBytes.Length) == magicBytes.Length)
                                        {
                                            uint magic = BitConverter.ToUInt32(magicBytes, 0);
                                            if (magic == 0xFEEDFACE || magic == 0xFEEDFACF ||
                                                magic == 0XBEBAFECA || magic == 0XCAFEBABE)
                                            {
                                                isLibFile = true;
                                            }
                                        }
                                    }
                                }

                                if (!isLibFile)
                                {
                                    string[] redirect = File.ReadAllLines(libPath);
                                    if (redirect.Length > 0 && !string.IsNullOrEmpty(redirect[0]))
                                    {
                                        string redirectLibName = redirect[0].Trim();
                                        string redirectFile = Path.Combine(targetLibPath, redirectLibName);
                                        if (File.Exists(redirectFile))
                                        {
                                            lines[i] = lines[i].Replace(libName, redirectLibName);
                                            modified = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                if (modified)
                {
                    File.WriteAllLines(configFile, lines);
                    Console.WriteLine("Fixed up Mono config file (/etc/mono/config)");
                }
            }
        }

        private static bool IsProfileMono(Profile profile)
        {
            switch (profile)
            {
                case Profile.Mono_Win32:
                case Profile.Mono_Win64:
                case Profile.Mono_Mac:
                case Profile.Mono_Linux:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsProfileCoreCLR(Profile profile)
        {
            switch (profile)
            {
                case Profile.CoreCLR_Win32:
                case Profile.CoreCLR_Win64:
                case Profile.CoreCLR_Mac:
                case Profile.CoreCLR_Linux:
                    return true;
                default:
                    return false;
            }
        }

        private static string FindLocalRuntimePath(Profile profile, bool checkBinDir)
        {
            string[] runtimeDirs =
            {                
                Path.Combine(Program.AppDirectory, "Runtimes"),// /Binaries/Managed/PluginInstaller/Runtimes/
                Path.Combine(Program.AppDirectory, "../Runtimes/")// /Binaries/Managed/Runtimes/ (used with checkBinDir)
            };

            for (int i = 0; i < 2; i++)
            {
                string runtimeDir = runtimeDirs[i];

                if (i == 1 && !checkBinDir)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(runtimeDir) && Directory.Exists(runtimeDir))
                {
                    string dir = null;
                    switch (profile)
                    {
                        case Profile.Mono_Win32:
                            dir = Path.Combine(runtimeDir, "Mono", "Win32");
                            break;
                        case Profile.Mono_Win64:
                            dir = Path.Combine(runtimeDir, "Mono", "Win64");
                            break;
                        case Profile.Mono_Mac:
                            dir = Path.Combine(runtimeDir, "Mono", "Mac");
                            break;
                        case Profile.Mono_Linux:
                            dir = Path.Combine(runtimeDir, "Mono", "Linux");
                            break;

                        case Profile.CoreCLR_Win32:
                            dir = Path.Combine(runtimeDir, "CoreCLR", "Win32");
                            break;
                        case Profile.CoreCLR_Win64:
                            dir = Path.Combine(runtimeDir, "CoreCLR", "Win64");
                            break;
                        case Profile.CoreCLR_Mac:
                            dir = Path.Combine(runtimeDir, "CoreCLR", "Mac");
                            break;
                        case Profile.CoreCLR_Linux:
                            dir = Path.Combine(runtimeDir, "CoreCLR", "Linux");
                            break;
                    }

                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                    {
                        return dir;
                    }
                }
            }
            return null;
        }

        public static string FindMsBuildPath()
        {
            if (Program.IsLinux || Program.IsMacOS)
            {
                // We might already be running under mono, get the path from the module file path
                try
                {
                    string processFilePath = Process.GetCurrentProcess().MainModule.FileName;
                    if (!string.IsNullOrEmpty(processFilePath) && File.Exists(processFilePath) &&
                        Path.GetFileName(processFilePath).ToLower().StartsWith("mono"))
                    {
                        string msbuildPath = Path.Combine(Path.GetDirectoryName(processFilePath), "msbuild");
                        if (File.Exists(msbuildPath))
                        {
                            return msbuildPath;
                        }
                    }
                }
                catch
                {
                }

                // Default linux install location
                string linuxInstallPath = "/usr/bin/msbuild";
                if (File.Exists(linuxInstallPath))
                {
                    return linuxInstallPath;
                }

                // Default mac install location
                string macMonoDir = FindMonoPathMac();
                if (!string.IsNullOrEmpty(macMonoDir))
                {
                    string macInstallPath = Path.Combine(macMonoDir, "bin", "msbuild");
                    if (File.Exists(macInstallPath))
                    {
                        return macInstallPath;
                    }
                }
                
                return null;
            }
            else
            {
                // We need to use the VS2017 MSBuild instead of the redistributable version as the redist version
                // seems to target the wrong toolset and we get compile errors. The redist version can be found at:
                // SOFTWARE\Microsoft\MSBUILD\ToolsVersions\4.0 (MSBuildToolsPath) then append msbuild.exe

                try
                {
                    string baseMicrosoftKeyPath = @"SOFTWARE\WOW6432Node\Microsoft";
                    string visualStudioRegistryKeyPath = baseMicrosoftKeyPath + @"\VisualStudio\SxS\VS7";

                    //Try Obtaining the VS version of MSBuild First
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(visualStudioRegistryKeyPath))
                    {
                        if (key != null)
                        {
                            string path = key.GetValue("15.0") as string;
                            if (!string.IsNullOrEmpty(path))
                            {
                                path = Path.Combine(path, "MSBuild", "15.0", "Bin", "msbuild.exe");
                                if (File.Exists(path))
                                {
                                    return path;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }

                return null;
            }
        }

        private static string FindMonoPath(Profile profile)
        {
            string binPath, etcPath, libPath;
            return FindMonoPath(profile, out binPath, out etcPath, out libPath);
        }

        private static string FindMonoPath(Profile profile, out string binPath, out string etcPath, out string libPath)
        {
            string basePath = null;
            switch (profile)
            {
                case Profile.Mono_Mac:
                    if (Program.IsMacOS)
                    {
                        basePath = FindMonoPathMac();
                    }
                    break;

                case Profile.Mono_Linux:
                    if (Program.IsLinux)
                    {
                        basePath = "/usr/bin/";
                        binPath = basePath;
                        etcPath = "/etc/";
                        libPath = "/usr/lib/";
                        return basePath;
                    }
                    break;

                case Profile.Mono_Win32:
                case Profile.Mono_Win64:
                    if (Program.IsWindows)
                    {
                        basePath = FindMonoPathWindows(profile);
                    }
                    break;
            }
            if (!string.IsNullOrEmpty(basePath) && Directory.Exists(basePath))
            {
                basePath = Path.GetFullPath(basePath);
                binPath = Path.Combine(basePath, "bin");
                etcPath = Path.Combine(basePath, "etc");
                libPath = Path.Combine(basePath, "lib");
                return basePath;
            }
            else
            {
                binPath = null;
                etcPath = null;
                libPath = null;
                return null;
            }
        }

        private static string FindMonoPathMac()
        {
            string macVersionsPath = "/Library/Frameworks/Mono.framework/Versions/";
            if (Directory.Exists(macVersionsPath))
            {
                string latestVersionDir = null;
                Version latestVersion = null;

                foreach (string dir in Directory.GetDirectories(macVersionsPath))
                {
                    Version version;
                    if (Version.TryParse(new DirectoryInfo(dir).Name, out version))
                    {
                        if (latestVersion == null || version > latestVersion)
                        {
                            latestVersionDir = dir;
                            latestVersion = version;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(latestVersionDir))
                {
                    string monoPath = Path.Combine(latestVersionDir, "bin", "mono-sgen64");
                    if (File.Exists(monoPath))
                    {
                        return latestVersionDir;
                    }
                }
            }
            return null;
        }

        private static string FindMonoPathWindows(Profile profile)
        {
            try
            {
                RegistryView view = profile == Profile.Mono_Win64 ? RegistryView.Registry64 : RegistryView.Registry32;
                using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (RegistryKey monoKey = localMachine.OpenSubKey(@"SOFTWARE\Mono"))
                    {
                        return monoKey.GetValue("SdkInstallRoot") as string;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private static string FindCoreCLRPath(Profile profile)
        {
            switch (profile)
            {
                case Profile.CoreCLR_Mac:
                    if (Program.IsMacOS)
                    {
                        return "/usr/local/share/dotnet/";
                    }
                    return null;

                case Profile.CoreCLR_Linux:
                    if (Program.IsLinux)
                    {
                        return "/usr/share/dotnet/";
                    }
                    return null;

                case Profile.CoreCLR_Win32:
                case Profile.CoreCLR_Win64:
                    if (Program.IsWindows)
                    {
                        return FindCoreCLRPathWindows(profile);
                    }
                    return null;

                default:
                    return null;
            }
        }

        private static string FindCoreCLRPathWindows(Profile profile)
        {
            // Grabbing the path of coreclr.dll from MiniDumpAuxiliaryDlls seems like the best way to get the path for now
            // https://github.com/dotnet/core-setup/blob/7c25ee34e49ff83324d3ec750f88356f05f28a68/src/pkg/packaging/windows/sharedframework/registrykeys.wxs

            try
            {
                string latestVersionRootDir = null;
                Version latestVersion = null;

                RegistryView view = profile == Profile.CoreCLR_Win64 ? RegistryView.Registry64 : RegistryView.Registry32;
                using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                {
                    using (RegistryKey dlls = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\MiniDumpAuxiliaryDlls"))
                    {
                        foreach (string dllPath in dlls.GetValueNames())
                        {
                            try
                            {
                                if (File.Exists(dllPath) && Path.GetFileName(dllPath).Equals("coreclr.dll", StringComparison.OrdinalIgnoreCase))
                                {
                                    string netCoreAppDir = Path.Combine(Path.GetDirectoryName(dllPath), "../");// Microsoft.NETCore.App dir
                                    string dotNetDir = Path.Combine(Path.GetDirectoryName(dllPath), "../../../");// Root dir
                                    if (Directory.Exists(dotNetDir) && Directory.Exists(Path.Combine(dotNetDir, "shared")))
                                    {
                                        netCoreAppDir = Path.GetFullPath(netCoreAppDir);
                                        dotNetDir = Path.GetFullPath(dotNetDir);

                                        Version version;
                                        FindLatestCoreCLRFolder(netCoreAppDir, out version);
                                        if (version != null && (latestVersion == null || version > latestVersion))
                                        {
                                            latestVersionRootDir = dotNetDir;
                                            latestVersion = version;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                return latestVersionRootDir;
            }
            catch
            {
                return null;
            }
        }

        private static string FindLatestCoreCLRFolder(string dir)
        {
            Version version;
            return FindLatestCoreCLRFolder(dir, out version);
        }

        private static string FindLatestCoreCLRFolder(string dir, out Version latestVersion)
        {
            string latestVersionDir = null;
            latestVersion = null;

            if (Directory.Exists(dir))
            {
                foreach (string versionDir in Directory.GetDirectories(dir))
                {
                    Version version;
                    if (TryParseCoreCLRVersion(new DirectoryInfo(versionDir).Name, out version) &&
                        (latestVersion == null || version > latestVersion))
                    {
                        latestVersionDir = versionDir;
                        latestVersion = version;
                    }
                }
            }

            return latestVersionDir;
        }

        private static bool TryParseCoreCLRVersion(string folderName, out Version version)
        {
            version = default(Version);
            for (int i = 0; i < folderName.Length; i++)
            {
                if (!char.IsNumber(folderName[i]) && folderName[i] != '.')
                {
                    folderName = folderName.Substring(0, i);
                    break;
                }
            }
            return Version.TryParse(folderName, out version);
        }

        private static bool GetCoreCLRPaths(string basePath, out string sdk, out string aspNetCore, out string netCore, out string windowsDesktop)
        {
            sdk = null;
            aspNetCore = null;
            netCore = null;
            windowsDesktop = null;

            if (!string.IsNullOrEmpty(basePath) && Directory.Exists(basePath))
            {
                string sdkBaseDir = Path.Combine(basePath, "sdk");
                string sharedDir = Path.Combine(basePath, "shared");

                sdk = FindLatestCoreCLRFolder(sdkBaseDir);
                aspNetCore = FindLatestCoreCLRFolder(Path.Combine(sharedDir, "Microsoft.AspNetCore.App"));
                netCore = FindLatestCoreCLRFolder(Path.Combine(sharedDir, "Microsoft.NETCore.App"));
                windowsDesktop = FindLatestCoreCLRFolder(Path.Combine(sharedDir, "Microsoft.WindowsDesktop.App"));

                return !string.IsNullOrEmpty(netCore);
            }
            return false;
        }

        private static string GetConfigName(bool minimal)
        {
            return minimal ? "RuntimeConfigMinimal.txt" : "RuntimeConfig.txt";
        }

        public static string GetConfigFile(bool minimal)
        {
            return Path.GetFullPath(Path.Combine(Program.AppDirectory, "../../../Settings/" + GetConfigName(minimal)));
        }

        private static bool LoadConfig(bool minimal)
        {
            config.Clear();
            
            Profile currentProfile = Profile.Unknown;
            List<RuntimeConfigEntry> currentEntries = null;

            string error = null;
            int index = 0;
            string[] lines = File.ReadAllLines(GetConfigFile(minimal));
            for (; index < lines.Length; index++)
            {
                string line = lines[index].Trim().Replace("\\", "/");

                if (line.StartsWith("["))
                {
                    int closeBraceIndex = line.IndexOf(']');
                    if (closeBraceIndex == -1)
                    {
                        error = "Couldn't find closing brace ']'";
                        break;
                    }

                    string profileStr = line.Substring(1, closeBraceIndex - 1);
                    if (Enum.TryParse(profileStr, out currentProfile))
                    {
                        if (!config.TryGetValue(currentProfile, out currentEntries))
                        {
                            config.Add(currentProfile, currentEntries = new List<RuntimeConfigEntry>());
                        }
                    }
                }
                else if (line.StartsWith("#"))
                {
                    // Comment line
                }
                else if (line.StartsWith("/"))
                {
                    int nextSlashIndex = line.IndexOf('/', 1);
                    if (nextSlashIndex > 1 && currentProfile != Profile.Unknown)
                    {
                        RuntimeConfigEntry entry = new RuntimeConfigEntry();
                        entry.Path = line.Substring(nextSlashIndex + 1).TrimEnd('*');

                        string rootFolderName = line.Substring(1, nextSlashIndex - 1).ToLower();
                        switch (rootFolderName)
                        {
                            case "bin":
                                entry.Folder = RuntimeConfigEntry.FolderType.Mono_Bin;
                                break;
                            case "etc":
                                entry.Folder = RuntimeConfigEntry.FolderType.Mono_Etc;
                                break;
                            case "lib":
                                entry.Folder = RuntimeConfigEntry.FolderType.Mono_Lib;
                                break;

                            case "dotnet":
                                entry.Folder = RuntimeConfigEntry.FolderType.CoreCLR_Dotnet;
                                break;
                            case "sdk":
                                entry.Folder = RuntimeConfigEntry.FolderType.CoreCLR_Sdk;
                                break;
                            case "aspnetcore":
                                entry.Folder = RuntimeConfigEntry.FolderType.CoreCLR_AspNetCore;
                                break;
                            case "netcore":
                                entry.Folder = RuntimeConfigEntry.FolderType.CoreCLR_NETCore;
                                break;
                            case "windowsdesktop":
                                entry.Folder = RuntimeConfigEntry.FolderType.CoreCLR_WindowsDesktop;
                                break;
                        }

                        if (entry.Folder == RuntimeConfigEntry.FolderType.Unknown)
                        {
                            error = "Unknown root folder '" + rootFolderName + "' (expected one of the following: bin, etc, lib)";
                            break;
                        }

                        bool isFolder = false;
                        bool isRecursive = false;
                        for (int j = line.Length - 1; j >= 0; j--)
                        {
                            char c = line[j];
                            if (c == '/')
                            {
                                isFolder = true;
                                break;
                            }
                            else if (c == '*')
                            {
                                isRecursive = true;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (isRecursive && !isFolder)
                        {
                            error = "Invalid format ('*' was provided without a folder path)";
                            break;
                        }

                        if (isRecursive)
                        {
                            entry.Type = RuntimeConfigEntry.PathType.Recursive;
                        }
                        else if (isFolder)
                        {
                            entry.Type = RuntimeConfigEntry.PathType.Folder;
                        }
                        else
                        {
                            entry.Type = RuntimeConfigEntry.PathType.File;
                        }

                        if ((entry.IsMonoFolder && IsProfileMono(currentProfile)) ||
                            (entry.IsCoreCLRFolder && IsProfileCoreCLR(currentProfile)))
                        {
                            currentEntries.Add(entry);
                        }
                    }                
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error + " on line " + (index + 1) + " (" + GetConfigName(minimal) + ")");
                return false;
            }

            return true;
        }

        class RuntimeConfigEntry
        {
            public FolderType Folder;
            public PathType Type;
            public string Path;

            public bool IsMonoFolder
            {
                get
                {
                    switch (Folder)
                    {
                        case FolderType.Mono_Bin:
                        case FolderType.Mono_Lib:
                        case FolderType.Mono_Etc:
                            return true;
                        default:
                            return false;
                    }
                }
            }

            public bool IsCoreCLRFolder
            {
                get
                {
                    switch (Folder)
                    {
                        case FolderType.CoreCLR_Dotnet:
                        case FolderType.CoreCLR_Sdk:
                        case FolderType.CoreCLR_AspNetCore:
                        case FolderType.CoreCLR_NETCore:
                        case FolderType.CoreCLR_WindowsDesktop:
                            return true;
                        default:
                            return false;
                    }
                }
            }

            public enum FolderType
            {
                Unknown,

                // Mono
                Mono_Bin,
                Mono_Lib,
                Mono_Etc,

                // .NET Core
                CoreCLR_Dotnet,
                CoreCLR_Sdk,
                CoreCLR_AspNetCore,
                CoreCLR_NETCore,
                CoreCLR_WindowsDesktop
            }

            public enum PathType
            {
                Unknown,
                File,
                Folder,
                Recursive
            }
        }

        enum Profile
        {
            Unknown,

            Mono_Win32,
            Mono_Win64,
            Mono_Mac,
            Mono_Linux,

            CoreCLR_Win32,
            CoreCLR_Win64,
            CoreCLR_Mac,
            CoreCLR_Linux,
        }
    }
}
