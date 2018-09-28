using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public static class FPackageName
    {
        /// <summary>
        /// Helper function for converting short to long script package name (InputCore -> /Script/InputCore)
        /// </summary>
        /// <param name="shortName">Short package name.</param>
        /// <returns>Long package name.</returns>
        public static string ConvertToLongScriptPackageName(string shortName)
        {
            using (FStringUnsafe shortNameUnsafe = new FStringUnsafe(shortName))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.ConvertToLongScriptPackageName(ref shortNameUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Registers all short package names found in ini files.
        /// </summary>
        public static void RegisterShortPackageNamesForUObjectModules()
        {
            Native_FPackageName.RegisterShortPackageNamesForUObjectModules();
        }

        /// <summary>
        /// Finds long script package name associated with a short package name.
        /// </summary>
        /// <param name="shortName">Short script package name.</param>
        /// <param name="scriptPackageName">Long script package name (/Script/Package) associated with short name or NULL.</param>
        /// <returns>True if found</returns>
        public static bool FindScriptPackageName(FName shortName, out FName scriptPackageName)
        {
            return Native_FPackageName.FindScriptPackageName(ref shortName, out scriptPackageName);
        }

        /// <summary>
        /// Tries to convert the supplied filename to long package name. Will attempt to find the package on disk (very slow).
        /// </summary>
        /// <param name="filename">Filename to convert.</param>
        /// <param name="packageName">The resulting long package name if the conversion was successful.</param>
        /// <returns>Returns true if the supplied filename properly maps to one of the long package roots.</returns>
        public static bool TryConvertFilenameToLongPackageName(string filename, out string packageName)
        {
            string failureReason;
            return TryConvertFilenameToLongPackageName(filename, out packageName, out failureReason);
        }

        /// <summary>
        /// Tries to convert the supplied filename to long package name. Will attempt to find the package on disk (very slow).
        /// </summary>
        /// <param name="filename">Filename to convert.</param>
        /// <param name="packageName">The resulting long package name if the conversion was successful.</param>
        /// <param name="failureReason">Description of an error if the conversion failed.</param>
        /// <returns>Returns true if the supplied filename properly maps to one of the long package roots.</returns>
        public static bool TryConvertFilenameToLongPackageName(string filename, out string packageName, out string failureReason)
        {
            using (FStringUnsafe filenameUnsafe = new FStringUnsafe(filename))
            using (FStringUnsafe packageNameUnsafe = new FStringUnsafe())
            using (FStringUnsafe failureReasonUnsafe = new FStringUnsafe())
            {
                bool result = Native_FPackageName.TryConvertFilenameToLongPackageName(
                    ref filenameUnsafe.Array,
                    ref packageNameUnsafe.Array,
                    ref failureReasonUnsafe.Array);
                packageName = packageNameUnsafe.Value;
                failureReason = failureReasonUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Converts the supplied filename to long package name.
        ///  Throws a fatal error if the conversion is not successfull.
        /// </summary>
        /// <param name="filename">Filename to convert.</param>
        /// <returns>Long package name.</returns>
        public static string FilenameToLongPackageName(string filename)
        {
            using (FStringUnsafe filenameUnsafe = new FStringUnsafe(filename))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.FilenameToLongPackageName(ref filenameUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Tries to convert a long package name to a file name with the supplied extension.
        /// </summary>
        /// <param name="longPackageName">Long Package Name</param>
        /// <param name="filename">Package filename.</param>
        /// <param name="extension">Package extension.</param>
        /// <returns></returns>
        public static bool TryConvertLongPackageNameToFilename(string longPackageName, out string filename, string extension = "")
        {
            using (FStringUnsafe longPackageNameUnsafe = new FStringUnsafe(longPackageName))
            using (FStringUnsafe filenameUnsafe = new FStringUnsafe())
            using (FStringUnsafe extensionUnsafe = new FStringUnsafe(extension))
            {
                bool result = Native_FPackageName.TryConvertLongPackageNameToFilename(
                    ref longPackageNameUnsafe.Array,
                    ref filenameUnsafe.Array,
                    ref extensionUnsafe.Array);
                filename = filenameUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Converts a long package name to a file name with the supplied extension.
        /// </summary>
        /// <param name="longPackageName">Long Package Name</param>
        /// <param name="extension">Package extension.</param>
        /// <returns>Package filename.</returns>
        public static string LongPackageNameToFilename(string longPackageName, string extension)
        {
            using (FStringUnsafe longPackageNameUnsafe = new FStringUnsafe(longPackageName))
            using (FStringUnsafe extensionUnsafe = new FStringUnsafe(extension))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.LongPackageNameToFilename(ref longPackageNameUnsafe.Array, ref extensionUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the path to the specified package, excluding the short package name
        /// </summary>
        /// <param name="longPackageName">Package Name.</param>
        /// <returns>The path to the specified package.</returns>
        public static string GetLongPackagePath(string longPackageName)
        {
            using (FStringUnsafe longPackageNameUnsafe = new FStringUnsafe(longPackageName))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.GetLongPackagePath(ref longPackageNameUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Convert a long package name into root, path, and name components
        /// </summary>
        /// <param name="longPackageName">Package Name.</param>
        /// <param name="packageRoot">The package root path, eg "/Game/"</param>
        /// <param name="packagePath">The path from the mount point to the package, eg "Maps/TestMaps/</param>
        /// <param name="packageName">The name of the package, including its extension, eg "MyMap.umap"</param>
        /// <param name="stripRootLeadingSlash">String any leading / character from the returned root</param>
        /// <returns>True if the conversion was possible, false otherwise</returns>
        public static bool SplitLongPackageName(string longPackageName, out string packageRoot, out string packagePath, out string packageName, bool stripRootLeadingSlash)
        {
            using (FStringUnsafe longPackageNameUnsafe = new FStringUnsafe(longPackageName))
            using (FStringUnsafe packageRootUnsafe = new FStringUnsafe())
            using (FStringUnsafe packagePathUnsafe = new FStringUnsafe())
            using (FStringUnsafe packageNameUnsafe = new FStringUnsafe())
            {
                bool result = Native_FPackageName.SplitLongPackageName(
                    ref longPackageNameUnsafe.Array,
                    ref packageRootUnsafe.Array,
                    ref packagePathUnsafe.Array,
                    ref packageNameUnsafe.Array,
                    stripRootLeadingSlash);
                packageRoot = packageRootUnsafe.Value;
                packagePath = packagePathUnsafe.Value;
                packageName = packageNameUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Returns the clean asset name for the specified package
        /// </summary>
        /// <param name="longPackageName">Long Package Name</param>
        /// <returns>Clean asset name.</returns>
        public static string GetLongPackageAssetName(string longPackageName)
        {
            using (FStringUnsafe longPackageNameUnsafe = new FStringUnsafe(longPackageName))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.GetLongPackageAssetName(ref longPackageNameUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns true if the path starts with a valid root (i.e. /Game/, /Engine/, etc) and contains no illegal characters.
        /// </summary>
        /// <param name="longPackageName">The package name to test</param>
        /// <param name="includeReadOnlyRoots">If true, will include roots that you should not save to. (/Temp/, /Script/)</param>
        /// <returns>true if a valid long package name</returns>
        public static bool IsValidLongPackageName(string longPackageName, bool includeReadOnlyRoots)
        {
            string reason;
            return IsValidLongPackageName(longPackageName, includeReadOnlyRoots, out reason);
        }

        /// <summary>
        /// Returns true if the path starts with a valid root (i.e. /Game/, /Engine/, etc) and contains no illegal characters.
        /// </summary>
        /// <param name="longPackageName">The package name to test</param>
        /// <param name="includeReadOnlyRoots">If true, will include roots that you should not save to. (/Temp/, /Script/)</param>
        /// <param name="reason">When returning false, this will provide a description of what was wrong with the name.</param>
        /// <returns>true if a valid long package name</returns>
        public static bool IsValidLongPackageName(string longPackageName, bool includeReadOnlyRoots, out string reason)
        {
            using (FStringUnsafe longPackageNameUnsafe = new FStringUnsafe(longPackageName))
            using (FStringUnsafe reasonUnsafe = new FStringUnsafe())
            {
                bool result = Native_FPackageName.IsValidLongPackageName(ref longPackageNameUnsafe.Array, includeReadOnlyRoots, ref reasonUnsafe.Array);
                reason = reasonUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Checks if the given string is a long package name or not.
        /// </summary>
        /// <param name="possiblyLongName">Package name.</param>
        /// <returns>true if the given name is a long package name, false otherwise.</returns>
        public static bool IsShortPackageName(string possiblyLongName)
        {
            using (FStringUnsafe possiblyLongNameUnsafe = new FStringUnsafe(possiblyLongName))
            {
                return Native_FPackageName.IsShortPackageName(ref possiblyLongNameUnsafe.Array);
            }
        }

        /// <summary>
        /// Checks if the given name is a long package name or not.
        /// </summary>
        /// <param name="possiblyLongName">Package name.</param>
        /// <returns>true if the given name is a long package name, false otherwise.</returns>
        public static bool IsShortPackageName(FName possiblyLongName)
        {
            return Native_FPackageName.IsShortPackageFName(ref possiblyLongName);
        }

        /// <summary>
        /// Converts package name to short name.
        /// </summary>
        /// <param name="package">Package which name to convert.</param>
        /// <returns>Short package name.</returns>
        public static string GetShortName(UPackage package)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.GetShortNameFromPackage(package == null ? IntPtr.Zero : package.Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Converts package name to short name.
        /// </summary>
        /// <param name="longName">Package name to convert.</param>
        /// <returns>Short package name.</returns>
        public static string GetShortName(string longName)
        {
            using (FStringUnsafe longNameUnsafe = new FStringUnsafe(longName))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.GetShortNameFromFString(ref longNameUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Converts package name to short name.
        /// </summary>
        /// <param name="longName">Package name to convert.</param>
        /// <returns>Short package name.</returns>
        public static string GetShortName(FName longName)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.GetShortNameFromFName(ref longName, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Converts package name to short name.
        /// </summary>
        /// <param name="longName">Package name to convert.</param>
        /// <returns>Short package name.</returns>
        public static FName GetShortFName(string longName)
        {
            using (FStringUnsafe longNameUnsafe = new FStringUnsafe(longName))
            {
                FName result;
                Native_FPackageName.GetShortFNameFromFString(ref longNameUnsafe.Array, out result);
                return result;
            }
        }

        /// <summary>
        /// Converts package name to short name.
        /// </summary>
        /// <param name="longName">Package name to convert.</param>
        /// <returns>Short package name.</returns>
        public static FName GetShortFName(FName longName)
        {
            FName result;
            Native_FPackageName.GetShortFNameFromFName(ref longName, out result);
            return result;
        }

        /// <summary>
        /// This will insert a mount point at the head of the search chain (so it can overlap an existing mount point and win).
        /// </summary>
        /// <param name="rootPath">Root Path.</param>
        /// <param name="contentPath">Content Path.</param>
        public static void RegisterMountPoint(string rootPath, string contentPath)
        {
            using (FStringUnsafe rootPathUnsafe = new FStringUnsafe(rootPath))
            using (FStringUnsafe contentPathUnsafe = new FStringUnsafe(contentPath))
            {
                Native_FPackageName.RegisterMountPoint(ref rootPathUnsafe.Array, ref contentPathUnsafe.Array);
            }
        }

        /// <summary>
        /// This will remove a previously inserted mount point.
        /// </summary>
        /// <param name="rootPath">Root Path.</param>
        /// <param name="contentPath">Content Path.</param>
        public static void UnRegisterMountPoint(string rootPath, string contentPath)
        {
            using (FStringUnsafe rootPathUnsafe = new FStringUnsafe(rootPath))
            using (FStringUnsafe contentPathUnsafe = new FStringUnsafe(contentPath))
            {
                Native_FPackageName.UnRegisterMountPoint(ref rootPathUnsafe.Array, ref contentPathUnsafe.Array);
            }
        }

        /// <summary>
        /// Get the mount point for a given package path
        /// </summary>
        /// <param name="packagePath">The package path to get the mount point for</param>
        /// <returns>FName corresponding to the mount point, or Empty if invalid</returns>
        public static FName GetPackageMountPoint(string packagePath)
        {
            using (FStringUnsafe packagePathUnsafe = new FStringUnsafe(packagePath))
            {
                FName result;
                Native_FPackageName.GetPackageMountPoint(ref packagePathUnsafe.Array, out result);
                return result;
            }
        }

        /// <summary>
        /// Checks if the package exists on disk.
        /// </summary>
        /// <param name="longPackageName">Package name.</param>
        /// <param name="guid"></param>
        /// <param name="filename">Package filename on disk.</param>
        /// <returns>true if the specified package name points to an existing package, false otherwise.</returns>
        public static bool DoesPackageExist(string longPackageName, Guid guid, out string filename)
        {
            using (FStringUnsafe longPackageNameUnsafe = new FStringUnsafe(longPackageName))
            using (FStringUnsafe filenameUnsafe = new FStringUnsafe())
            {
                bool result = Native_FPackageName.DoesPackageExist(ref longPackageNameUnsafe.Array, ref guid, ref filenameUnsafe.Array);
                filename = filenameUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Attempts to find a package given its short name on disk (very slow).
        /// </summary>
        /// <param name="packageName">Package to find.</param>
        /// <param name="longPackageName">Long package name corresponding to the found file (if any).</param>
        /// <param name="filename"></param>
        /// <returns>true if the specified package name points to an existing package, false otherwise.</returns>
        public static bool SearchForPackageOnDisk(string packageName, out string longPackageName, out string filename)
        {
            using (FStringUnsafe packageNameUnsafe = new FStringUnsafe(packageName))
            using (FStringUnsafe longPackageNameUnsafe = new FStringUnsafe())
            using (FStringUnsafe filenameUnsafe = new FStringUnsafe())
            {
                bool result = Native_FPackageName.SearchForPackageOnDisk(
                    ref packageNameUnsafe.Array,
                    ref longPackageNameUnsafe.Array,
                    ref filenameUnsafe.Array);
                longPackageName = longPackageNameUnsafe.Value;
                filename = filenameUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Tries to convert object path with short package name to object path with long package name found on disk (very slow)
        /// </summary>
        /// <param name="objectPath">Path to the object.</param>
        /// <param name="convertedObjectPath">Converted object path.</param>
        /// <returns>True if succeeded. False otherwise.</returns>
        public static bool TryConvertShortPackagePathToLongInObjectPath(string objectPath, out string convertedObjectPath)
        {
            using (FStringUnsafe objectPathUnsafe = new FStringUnsafe(objectPath))
            using (FStringUnsafe convertedObjectPathUnsafe = new FStringUnsafe())
            {
                bool result = Native_FPackageName.TryConvertShortPackagePathToLongInObjectPath(
                    ref objectPathUnsafe.Array,
                    ref convertedObjectPathUnsafe.Array);
                convertedObjectPath = convertedObjectPathUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Gets normalized object path i.e. with long package format.
        /// </summary>
        /// <param name="objectPath">Path to the object.</param>
        /// <returns>Normalized path (or empty path, if short object path was given and it wasn't found on the disk).</returns>
        public static string GetNormalizedObjectPath(string objectPath)
        {
            using (FStringUnsafe objectPathUnsafe = new FStringUnsafe(objectPath))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.GetNormalizedObjectPath(ref objectPathUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets the localized version of a long package path for the current culture, or returns the source package if there is no suitable localized package.
        /// </summary>
        /// <param name="sourcePackagePath">Path to the source package.</param>
        /// <returns>Localized package path, or the source package path if there is no suitable localized package.</returns>
        public static string GetLocalizedPackagePath(string sourcePackagePath)
        {
            using (FStringUnsafe sourcePackagePathUnsafe = new FStringUnsafe(sourcePackagePath))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.GetLocalizedPackagePath(ref sourcePackagePathUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets the localized version of a long package path for the given culture, or returns the source package if there is no suitable localized package.
        /// </summary>
        /// <param name="sourcePackagePath">Path to the source package.</param>
        /// <param name="cultureName">Culture name to get the localized package for.</param>
        /// <returns>Localized package path, or the source package path if there is no suitable localized package.</returns>
        public static string GetLocalizedPackagePath(string sourcePackagePath, string cultureName)
        {
            using (FStringUnsafe sourcePackagePathUnsafe = new FStringUnsafe(sourcePackagePath))
            using (FStringUnsafe cultureNameUnsafe = new FStringUnsafe(cultureName))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.GetLocalizedPackagePathWithCulture(
                    ref sourcePackagePathUnsafe.Array,
                    ref cultureNameUnsafe.Array,
                    ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the file extension for packages containing assets.
        /// </summary>
        /// <returns>file extension for asset pacakges ( dot included )</returns>
        public static string GetAssetPackageExtension()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.GetAssetPackageExtension(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the file extension for packages containing assets.
        /// </summary>
        /// <returns>file extension for asset pacakges ( dot included )</returns>
        public static string GetMapPackageExtension()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.GetMapPackageExtension(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns whether the passed in extension is a valid package
        /// extension. Extensions with and without trailing dots are supported.
        /// </summary>
        /// <param name="extension">Extension to test.</param>
        /// <returns>True if Ext is either an asset or a map extension, otherwise false</returns>
        public static bool IsPackageExtension(string extension)
        {
            using (FStringUnsafe extensionUnsafe = new FStringUnsafe(extension))
            {
                return Native_FPackageName.IsPackageExtension(ref extensionUnsafe.Array);
            }
        }

        /// <summary>
        /// Returns whether the passed in filename ends with any of the known
        /// package extensions.
        /// </summary>
        /// <param name="filename">Filename to test.</param>
        /// <returns>True if the filename ends with a package extension.</returns>
        public static bool IsPackageFilename(string filename)
        {
            using (FStringUnsafe filenameUnsafe = new FStringUnsafe(filename))
            {
                return Native_FPackageName.IsPackageFilename(ref filenameUnsafe.Array);
            }
        }

        /// <summary>
        /// This will recurse over a directory structure looking for packages.
        /// </summary>
        /// <param name="rootDir">The root of the directory structure to recurse through</param>
        /// <param name="packages">The output array that is filled out with a file paths</param>
        /// <returns>Returns true if any packages have been found, otherwise false</returns>
        public static bool FindPackagesInDirectory(string rootDir, out string[] packages)
        {
            using (FStringUnsafe rootDirUnsafe = new FStringUnsafe(rootDir))
            using (TArrayUnsafe<string> packagesUnsafe = new TArrayUnsafe<string>())
            {
                bool result = Native_FPackageName.FindPackagesInDirectory(packagesUnsafe.Address, ref rootDirUnsafe.Array);
                packages = packagesUnsafe.ToArray();
                return result;
            }
        }

        /// <summary>
        /// Queries all of the root content paths, like "/Game/", "/Engine/", and any dynamically added paths
        /// </summary>
        /// <returns>List of content paths</returns>
        public static string[] QueryRootContentPaths()
        {
            using (TArrayUnsafe<string> resultUnsafe = new TArrayUnsafe<string>())
            {
                Native_FPackageName.QueryRootContentPaths(resultUnsafe.Address);
                return resultUnsafe.ToArray();
            }
        }

        /// <summary>
        /// If the FLongPackagePathsSingleton is not created yet, this function will create it and thus allow mount points to be added
        /// </summary>
        public static void EnsureContentPathsAreRegistered()
        {
            Native_FPackageName.EnsureContentPathsAreRegistered();
        }

        /// <summary>
        /// Converts the supplied export text path to an object path and class name.
        /// </summary>
        /// <param name="exportTextPath">The export text path for an object. Takes on the form: ClassName'ObjectPath'</param>
        /// <param name="className">The name of the class at the start of the path.</param>
        /// <param name="objectPath">The path to the object.</param>
        /// <returns>True if the supplied export text path could be parsed</returns>
        public static bool ParseExportTextPath(string exportTextPath, out string className, out string objectPath)
        {
            using (FStringUnsafe exportTextPathUnsafe = new FStringUnsafe(exportTextPath))
            using (FStringUnsafe classNameUnsafe = new FStringUnsafe())
            using (FStringUnsafe objectPathUnsafe = new FStringUnsafe())
            {
                bool result = Native_FPackageName.ParseExportTextPath(
                    ref exportTextPathUnsafe.Array,
                    ref classNameUnsafe.Array,
                    ref objectPathUnsafe.Array);
                className = classNameUnsafe.Value;
                objectPath = objectPathUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Returns the path to the object referred to by the supplied export text path, excluding the class name.
        /// </summary>
        /// <param name="exportTextPath">The export text path for an object. Takes on the form: ClassName'ObjectPath'</param>
        /// <returns>The path to the object referred to by the supplied export path.</returns>
        public static string ExportTextPathToObjectPath(string exportTextPath)
        {
            using (FStringUnsafe exportTextPathUnsafe = new FStringUnsafe(exportTextPath))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.ExportTextPathToObjectPath(ref exportTextPathUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the name of the package referred to by the specified object path
        /// </summary>
        public static string ObjectPathToPackageName(string objectPath)
        {
            using (FStringUnsafe objectPathUnsafe = new FStringUnsafe(objectPath))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.ObjectPathToPackageName(ref objectPathUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the name of the object referred to by the specified object path
        /// </summary>
        public static string ObjectPathToObjectName(string objectPath)
        {
            using (FStringUnsafe objectPathUnsafe = new FStringUnsafe(objectPath))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FPackageName.ObjectPathToObjectName(ref objectPathUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Checks the root of the package's path to see if it is a script package
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns>true if the root of the path matches the script path</returns>
        public static bool IsScriptPackage(string packageName)
        {
            using (FStringUnsafe packageNameUnsafe = new FStringUnsafe(packageName))
            {
                return Native_FPackageName.IsScriptPackage(ref packageNameUnsafe.Array);
            }
        }

        /// <summary>
        /// Checks the root of the package's path to see if it is a localized package
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns>true if the root of the path matches any localized root path</returns>
        public static bool IsLocalizedPackage(string packageName)
        {
            using (FStringUnsafe packageNameUnsafe = new FStringUnsafe(packageName))
            {
                return Native_FPackageName.IsLocalizedPackage(ref packageNameUnsafe.Array);
            }
        }

        /// <summary>
        /// Checks if a package name contains characters that are invalid for package names.
        /// </summary>
        public static bool DoesPackageNameContainInvalidCharacters(string longPackageName)
        {
            string reason;
            return DoesPackageNameContainInvalidCharacters(longPackageName, out reason);
        }

        /// <summary>
        /// Checks if a package name contains characters that are invalid for package names.
        /// </summary>
        public static bool DoesPackageNameContainInvalidCharacters(string longPackageName, out string reason)
        {
            using (FStringUnsafe longPackageNameUnsafe = new FStringUnsafe(longPackageName))
            using (FStringUnsafe reasonUnsafe = new FStringUnsafe())
            {
                bool result = Native_FPackageName.DoesPackageNameContainInvalidCharacters(ref longPackageNameUnsafe.Array, ref reasonUnsafe.Array);
                reason = reasonUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Checks if a package can be found using known package extensions.
        /// </summary>
        /// <param name="packageFilename">Package filename without the extension.</param>
        /// <param name="filename">If the package could be found, filename with the extension.</param>
        /// <returns>true if the package could be found on disk.</returns>
        public static bool FindPackageFileWithoutExtension(string packageFilename, out string filename)
        {
            using (FStringUnsafe packageFilenameUnsafe = new FStringUnsafe(packageFilename))
            using (FStringUnsafe filenameUnsafe = new FStringUnsafe())
            {
                bool result = Native_FPackageName.FindPackageFileWithoutExtension(ref packageFilenameUnsafe.Array, ref filenameUnsafe.Array);
                filename = filenameUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Replaces all invalid package name characters with _
        /// </summary>
        public static string SanitizePackageName(string packageName)
        {
            // Copy of PackageTools::SanitizePackageName as it is inside an editor module
            // Engine\Source\Editor\UnrealEd\Public\PackageTools.h

            StringBuilder sanitizedName = new StringBuilder();
            char[] invalidChars = InvalidLongPackageCharacters;

            // See if the name contains invalid characters.
            char c;
            for (int i = 0; i < packageName.Length; ++i)
            {
                c = packageName[i];

                if (invalidChars.Contains(c))
                {
                    sanitizedName.Append('_');
                }
                else
                {
                    sanitizedName.Append(c);
                }
            }

            // Remove double-slashes
            sanitizedName.Replace("//", "/");

            return sanitizedName.ToString();
        }

        /// <summary>
        /// Gets information for a given path string
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="pathRoot">The path root ("Game", "Script")</param>
        /// <param name="directory">The path without the path root and without the file / asset name information (includes the module name if it exists)</param>
        /// <param name="moduleName">The module name ("CoreUObject", "Engine")</param>
        /// <param name="objectName">The name of the object / asset</param>
        /// <param name="subobjectName">The name after the subobject delimiter ':'</param>
        public static void GetPathInfo(string path, out string pathRoot, out string directory, out string moduleName, out string objectName, out string subobjectName)
        {
            pathRoot = string.Empty;
            directory = string.Empty;
            moduleName = string.Empty;
            objectName = string.Empty;
            subobjectName = string.Empty;

            string filename = string.Empty;

            if (!string.IsNullOrEmpty(path))
            {
                // Get and remove the path root e.g. "/Game/"
                if (path.Length > 1 && path[0] == '/')
                {
                    pathRoot = path.Substring(1, path.IndexOf('/', 1) - 1);
                    path = path.Substring(pathRoot.Length + 2);
                }

                int index = path.LastIndexOf('/');
                if (index >= 0)
                {
                    directory = path.Substring(0, index);
                    filename = path.Substring(index + 1);
                }
                else
                {
                    filename = path;
                }

                index = filename.LastIndexOf('.');
                if (index >= 0)
                {
                    if (pathRoot == "Script")
                    {
                        // "Script" paths have an asset name of /Path1/Path2/Path3/ModuleName.AssetName
                        moduleName = filename.Substring(0, index);
                        directory += moduleName;
                        objectName = filename.Substring(index + 1);

                        index = objectName.IndexOf(':');
                        if (index >= 0)
                        {
                            subobjectName = objectName.Substring(index + 1);
                            objectName = objectName.Substring(0, index);
                        }
                    }
                    else
                    {
                        // Non "Script" paths have an asset name of /Path1/Path2/Path3/AssetName.AssetName
                        objectName = filename.Substring(0, index);

                        index = filename.IndexOf(':');
                        if (index >= 0)
                        {
                            subobjectName = filename.Substring(index + 1);
                        }
                    }
                }
            }
        }

        // Copy of string constants related to path / package names
        // Engine\Source\Runtime\Core\Public\UObject\NameTypes.h

        /// <summary>
        /// this is the character used to separate a subobject root from its subobjects in a path name.
        /// </summary>
        public static char SubobjectDelimiter = ':';

        /// <summary>
        /// These are the characters that cannot be used in general FNames
        /// </summary>
        public static char[] InvalidNameChars = "\"' ,\n\r\t".ToCharArray();

        /// <summary>
        /// These characters cannot be used in object names
        /// </summary>
        public static char[] InvalidObjectNameCharacters = "\"' ,/.:|&!~\n\r\t@#(){}[]=;^%$`".ToCharArray();

        /// <summary>
        /// These characters cannot be used in ObjectPaths, which includes both the package path and part after the first.
        /// </summary>
        public static char[] InvalidObjectPathCharacters = "\"' ,|&!~\n\r\t@#(){}[]=;^%$`".ToCharArray();

        /// <summary>
        /// These characters cannot be used in long package names
        /// </summary>
        public static char[] InvalidLongPackageCharacters = "\\:*?\"<>|' ,.&!~\n\r\t@#".ToCharArray();

        /// <summary>
        /// These characters can be used in relative directory names (lowercase versions as well)
        /// </summary>
        public static char[] ValidSavedDirSuffixCharacters = "_0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
    }
}
