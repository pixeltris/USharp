using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FARFilter
    {
        public delegate IntPtr Del_New();
        public delegate void Del_Delete(IntPtr instance);
        public delegate void Del_Set_PackageNames(IntPtr instance, IntPtr value);
        public delegate void Del_Set_PackagePaths(IntPtr instance, IntPtr value);
        public delegate void Del_Set_ObjectPaths(IntPtr instance, IntPtr value);
        public delegate void Del_Set_ClassNames(IntPtr instance, IntPtr value);
        public delegate void Del_Set_TagsAndValues(IntPtr instance, IntPtr keys, IntPtr values);
        public delegate void Del_Set_RecursiveClassesExclusionSet(IntPtr instance, IntPtr value);
        public delegate void Del_Set_bRecursivePaths(IntPtr instance, csbool value);
        public delegate void Del_Set_bRecursiveClasses(IntPtr instance, csbool value);
        public delegate void Del_Set_bIncludeOnlyOnDiskAssets(IntPtr instance, csbool value);

        public static Del_New New;
        public static Del_Delete Delete;
        public static Del_Set_PackageNames Set_PackageNames;
        public static Del_Set_PackagePaths Set_PackagePaths;
        public static Del_Set_ObjectPaths Set_ObjectPaths;
        public static Del_Set_ClassNames Set_ClassNames;
        public static Del_Set_TagsAndValues Set_TagsAndValues;
        public static Del_Set_RecursiveClassesExclusionSet Set_RecursiveClassesExclusionSet;
        public static Del_Set_bRecursivePaths Set_bRecursivePaths;
        public static Del_Set_bRecursiveClasses Set_bRecursiveClasses;
        public static Del_Set_bIncludeOnlyOnDiskAssets Set_bIncludeOnlyOnDiskAssets;
    }
}
