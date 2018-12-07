using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FWorldDelegates
    {
        public delegate void Del_WorldEvent(IntPtr world);
        public delegate void Del_WorldInitializationEvent(IntPtr world, IntPtr ivs);
        public delegate void Del_WorldCleanupEvent(IntPtr world, csbool sessionEnded, csbool cleanupResources);
        public delegate void Del_WorldPostDuplicateEvent(IntPtr world, csbool duplicateForPIE, IntPtr replacementMap, IntPtr objectsToFixReferences);

        public delegate void Del_Reg_OnPostWorldCreation(IntPtr instance, Del_WorldEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnPreWorldInitialization(IntPtr instance, Del_WorldInitializationEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnPostWorldInitialization(IntPtr instance, Del_WorldInitializationEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnPostDuplicate(IntPtr instance, Del_WorldPostDuplicateEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnWorldCleanup(IntPtr instance, Del_WorldCleanupEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnPostWorldCleanup(IntPtr instance, Del_WorldCleanupEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnPreWorldFinishDestroy(IntPtr instance, Del_WorldEvent handler, ref FDelegateHandle handle, csbool enable);

        public static Del_Reg_OnPostWorldCreation Reg_OnPostWorldCreation;
        public static Del_Reg_OnPreWorldInitialization Reg_OnPreWorldInitialization;
        public static Del_Reg_OnPostWorldInitialization Reg_OnPostWorldInitialization;
        public static Del_Reg_OnPostDuplicate Reg_OnPostDuplicate;
        public static Del_Reg_OnWorldCleanup Reg_OnWorldCleanup;
        public static Del_Reg_OnPostWorldCleanup Reg_OnPostWorldCleanup;
        public static Del_Reg_OnPreWorldFinishDestroy Reg_OnPreWorldFinishDestroy;
    }
}
