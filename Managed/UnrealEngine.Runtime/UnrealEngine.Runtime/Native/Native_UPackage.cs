using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UPackage
    {
        public delegate void Del_Get_FileName(IntPtr instance, out FName result);
        public delegate void Del_Set_FileName(IntPtr instance, ref FName value);
        public delegate IntPtr Del_Get_MetaData(IntPtr instance);
        public delegate float Del_GetLoadTime(IntPtr instance);
        public delegate void Del_GetFolderName(IntPtr instance, out FName result);
        public delegate void Del_SetDirtyFlag(IntPtr instance, csbool isDirty);
        public delegate csbool Del_IsDirty(IntPtr instance);
        public delegate void Del_MarkAsFullyLoaded(IntPtr instance);
        public delegate csbool Del_IsFullyLoaded(IntPtr instance);
        public delegate void Del_FullyLoad(IntPtr instance);
        public delegate csbool Del_ContainsMap(IntPtr instance);
        public delegate void Del_SetPackageFlags(IntPtr instance, EPackageFlags newFlags);
        public delegate void Del_ClearPackageFlags(IntPtr instance, EPackageFlags newFlags);
        public delegate csbool Del_HasAnyPackageFlags(IntPtr instance, EPackageFlags flagsToCheck);
        public delegate csbool Del_HasAllPackagesFlags(IntPtr instance, EPackageFlags flagsToCheck);
        public delegate EPackageFlags Del_GetPackageFlags(IntPtr instance);
        public delegate void Del_GetGuid(IntPtr instance, out Guid result);
        public delegate void Del_MakeNewGuid(IntPtr instance);
        public delegate void Del_SetGuid(IntPtr instance, ref Guid guid);
        public delegate long Del_GetFileSize(IntPtr instance);
        public delegate IntPtr Del_GetMetaData(IntPtr instance);
        public delegate void Del_WaitForAsyncFileWrites();
        
        public static Del_Get_FileName Get_FileName;
        public static Del_Set_FileName Set_FileName;
        public static Del_Get_MetaData Get_MetaData;
        public static Del_GetLoadTime GetLoadTime;
        public static Del_GetFolderName GetFolderName;
        public static Del_SetDirtyFlag SetDirtyFlag;
        public static Del_IsDirty IsDirty;
        public static Del_MarkAsFullyLoaded MarkAsFullyLoaded;
        public static Del_IsFullyLoaded IsFullyLoaded;
        public static Del_FullyLoad FullyLoad;
        public static Del_ContainsMap ContainsMap;
        public static Del_SetPackageFlags SetPackageFlags;
        public static Del_ClearPackageFlags ClearPackageFlags;
        public static Del_HasAnyPackageFlags HasAnyPackageFlags;
        public static Del_HasAllPackagesFlags HasAllPackagesFlags;
        public static Del_GetPackageFlags GetPackageFlags;
        public static Del_GetGuid GetGuid;
        public static Del_MakeNewGuid MakeNewGuid;
        public static Del_SetGuid SetGuid;
        public static Del_GetFileSize GetFileSize;
        public static Del_GetMetaData GetMetaData;
        public static Del_WaitForAsyncFileWrites WaitForAsyncFileWrites;
    }
}
