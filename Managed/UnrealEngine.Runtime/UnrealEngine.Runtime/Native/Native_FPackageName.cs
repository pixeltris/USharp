using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FPackageName
    {
        public delegate void Del_ConvertToLongScriptPackageName(ref FScriptArray inShortName, ref FScriptArray result);
        public delegate void Del_RegisterShortPackageNamesForUObjectModules();
        public delegate csbool Del_FindScriptPackageName(ref FName shortName, out FName outScriptPackageName);
        public delegate csbool Del_TryConvertFilenameToLongPackageName(ref FScriptArray inFilename, ref FScriptArray outPackageName, ref FScriptArray outFailureReason);
        public delegate void Del_FilenameToLongPackageName(ref FScriptArray inFilename, ref FScriptArray result);
        public delegate csbool Del_TryConvertLongPackageNameToFilename(ref FScriptArray inLongPackageName, ref FScriptArray outFilename, ref FScriptArray inExtension);
        public delegate void Del_LongPackageNameToFilename(ref FScriptArray inLongPackageName, ref FScriptArray inExtension, ref FScriptArray result);
        public delegate void Del_GetLongPackagePath(ref FScriptArray inLongPackageName, ref FScriptArray result);
        public delegate csbool Del_SplitLongPackageName(ref FScriptArray inLongPackageName, ref FScriptArray outPackageRoot, ref FScriptArray outPackagePath, ref FScriptArray outPackageName, csbool stripRootLeadingSlash);
        public delegate void Del_GetLongPackageAssetName(ref FScriptArray inLongPackageName, ref FScriptArray result);
        public delegate csbool Del_IsValidLongPackageName(ref FScriptArray inLongPackageName, csbool includeReadOnlyRoots, ref FScriptArray outReason);
        public delegate csbool Del_IsShortPackageName(ref FScriptArray possiblyLongName);
        public delegate csbool Del_IsShortPackageFName(ref FName possiblyLongName);
        public delegate void Del_GetShortNameFromPackage(IntPtr package, ref FScriptArray result);
        public delegate void Del_GetShortNameFromFString(ref FScriptArray longName, ref FScriptArray result);
        public delegate void Del_GetShortNameFromFName(ref FName longName, ref FScriptArray result);
        public delegate void Del_GetShortFNameFromFString(ref FScriptArray longName, out FName result);
        public delegate void Del_GetShortFNameFromFName(ref FName longName, out FName result);
        public delegate void Del_RegisterMountPoint(ref FScriptArray rootPath, ref FScriptArray contentPath);
        public delegate void Del_UnRegisterMountPoint(ref FScriptArray rootPath, ref FScriptArray contentPath);
        public delegate void Del_GetPackageMountPoint(ref FScriptArray inPackagePath, out FName result);

        public delegate csbool Del_DoesPackageExist(ref FScriptArray longPackageName, ref Guid guid, ref FScriptArray outFilename);
        public delegate csbool Del_SearchForPackageOnDisk(ref FScriptArray packageName, ref FScriptArray outLongPackageName, ref FScriptArray outFilename);
        public delegate csbool Del_TryConvertShortPackagePathToLongInObjectPath(ref FScriptArray objectPath, ref FScriptArray convertedObjectPath);
        public delegate void Del_GetNormalizedObjectPath(ref FScriptArray objectPath, ref FScriptArray result);
        public delegate void Del_GetLocalizedPackagePath(ref FScriptArray inSourcePackagePath, ref FScriptArray result);
        public delegate void Del_GetLocalizedPackagePathWithCulture(ref FScriptArray inSourcePackagePath, ref FScriptArray inCultureName, ref FScriptArray result);
        public delegate void Del_GetAssetPackageExtension(ref FScriptArray result);
        public delegate void Del_GetMapPackageExtension(ref FScriptArray result);
        public delegate csbool Del_IsPackageExtension(ref FScriptArray ext);
        public delegate csbool Del_IsPackageFilename(ref FScriptArray filename);
        public delegate csbool Del_FindPackagesInDirectory(IntPtr outPackages, ref FScriptArray rootDir);
        public delegate void Del_QueryRootContentPaths(IntPtr outRootContentPaths);
        public delegate void Del_EnsureContentPathsAreRegistered();
        public delegate csbool Del_ParseExportTextPath(ref FScriptArray inExportTextPath, ref FScriptArray outClassName, ref FScriptArray outObjectPath);
        public delegate void Del_ExportTextPathToObjectPath(ref FScriptArray inExportTextPath, ref FScriptArray result);
        public delegate void Del_ObjectPathToPackageName(ref FScriptArray inObjectPath, ref FScriptArray result);
        public delegate void Del_ObjectPathToObjectName(ref FScriptArray inObjectPath, ref FScriptArray result);
        public delegate csbool Del_IsScriptPackage(ref FScriptArray inPackageName);
        public delegate csbool Del_IsLocalizedPackage(ref FScriptArray inPackageName);
        public delegate csbool Del_DoesPackageNameContainInvalidCharacters(ref FScriptArray inLongPackageName, ref FScriptArray outReasonText);
        public delegate csbool Del_FindPackageFileWithoutExtension(ref FScriptArray inPackageFilename, ref FScriptArray outFilename);

        public static Del_ConvertToLongScriptPackageName ConvertToLongScriptPackageName;
        public static Del_RegisterShortPackageNamesForUObjectModules RegisterShortPackageNamesForUObjectModules;
        public static Del_FindScriptPackageName FindScriptPackageName;
        public static Del_TryConvertFilenameToLongPackageName TryConvertFilenameToLongPackageName;
        public static Del_FilenameToLongPackageName FilenameToLongPackageName;
        public static Del_TryConvertLongPackageNameToFilename TryConvertLongPackageNameToFilename;
        public static Del_LongPackageNameToFilename LongPackageNameToFilename;
        public static Del_GetLongPackagePath GetLongPackagePath;
        public static Del_SplitLongPackageName SplitLongPackageName;
        public static Del_GetLongPackageAssetName GetLongPackageAssetName;
        public static Del_IsValidLongPackageName IsValidLongPackageName;
        public static Del_IsShortPackageName IsShortPackageName;
        public static Del_IsShortPackageFName IsShortPackageFName;
        public static Del_GetShortNameFromPackage GetShortNameFromPackage;
        public static Del_GetShortNameFromFString GetShortNameFromFString;
        public static Del_GetShortNameFromFName GetShortNameFromFName;
        public static Del_GetShortFNameFromFString GetShortFNameFromFString;
        public static Del_GetShortFNameFromFName GetShortFNameFromFName;
        public static Del_RegisterMountPoint RegisterMountPoint;
        public static Del_UnRegisterMountPoint UnRegisterMountPoint;
        public static Del_GetPackageMountPoint GetPackageMountPoint;

        public static Del_DoesPackageExist DoesPackageExist;
        public static Del_SearchForPackageOnDisk SearchForPackageOnDisk;
        public static Del_TryConvertShortPackagePathToLongInObjectPath TryConvertShortPackagePathToLongInObjectPath;
        public static Del_GetNormalizedObjectPath GetNormalizedObjectPath;
        public static Del_GetLocalizedPackagePath GetLocalizedPackagePath;
        public static Del_GetLocalizedPackagePathWithCulture GetLocalizedPackagePathWithCulture;
        public static Del_GetAssetPackageExtension GetAssetPackageExtension;
        public static Del_GetMapPackageExtension GetMapPackageExtension;
        public static Del_IsPackageExtension IsPackageExtension;
        public static Del_IsPackageFilename IsPackageFilename;
        public static Del_FindPackagesInDirectory FindPackagesInDirectory;
        public static Del_QueryRootContentPaths QueryRootContentPaths;
        public static Del_EnsureContentPathsAreRegistered EnsureContentPathsAreRegistered;
        public static Del_ParseExportTextPath ParseExportTextPath;
        public static Del_ExportTextPathToObjectPath ExportTextPathToObjectPath;
        public static Del_ObjectPathToPackageName ObjectPathToPackageName;
        public static Del_ObjectPathToObjectName ObjectPathToObjectName;
        public static Del_IsScriptPackage IsScriptPackage;
        public static Del_IsLocalizedPackage IsLocalizedPackage;
        public static Del_DoesPackageNameContainInvalidCharacters DoesPackageNameContainInvalidCharacters;
        public static Del_FindPackageFileWithoutExtension FindPackageFileWithoutExtension;
    }
}
