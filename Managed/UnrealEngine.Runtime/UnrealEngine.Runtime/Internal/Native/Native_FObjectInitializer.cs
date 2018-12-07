using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FObjectInitializer
    {
        public delegate IntPtr Del_GetArchetype(IntPtr instance);
        public delegate IntPtr Del_GetObj(IntPtr instance);
        public delegate IntPtr Del_GetClass(IntPtr instance);
        public delegate IntPtr Del_CreateEditorOnlyDefaultSubobject(IntPtr instance, IntPtr outer, ref FName subobjectName, IntPtr returnType, csbool transient);
        public delegate IntPtr Del_CreateDefaultSubobject(IntPtr instance, IntPtr outer, ref FName subobjectFName, IntPtr returnType,  IntPtr classToCreateByDefault, csbool isRequired, csbool isAbstract, csbool isTransient);
        public delegate IntPtr Del_DoNotCreateDefaultSubobject(IntPtr instance, ref FName subobjectName);
        public delegate IntPtr Del_DoNotCreateDefaultSubobjectStr(IntPtr instance, ref FScriptArray subobjectName);
        public delegate csbool Del_IslegalOverride(IntPtr instance, ref FName componentName, IntPtr derivedComponentClass, IntPtr baseComponentClass);
        public delegate void Del_FinalizeSubobjectClassInitialization(IntPtr instance);
        public delegate void Del_AssertIfInConstructor(IntPtr outer, ref FScriptArray errorMessage);
        public delegate IntPtr Del_Get();

        public static Del_GetArchetype GetArchetype;
        public static Del_GetObj GetObj;
        public static Del_GetClass GetClass;
        public static Del_CreateEditorOnlyDefaultSubobject CreateEditorOnlyDefaultSubobject;
        public static Del_CreateDefaultSubobject CreateDefaultSubobject;
        public static Del_DoNotCreateDefaultSubobject DoNotCreateDefaultSubobject;
        public static Del_DoNotCreateDefaultSubobjectStr DoNotCreateDefaultSubobjectStr;
        public static Del_IslegalOverride IslegalOverride;
        public static Del_FinalizeSubobjectClassInitialization FinalizeSubobjectClassInitialization;        
        public static Del_AssertIfInConstructor AssertIfInConstructor;
        public static Del_Get Get;        
    }
}
