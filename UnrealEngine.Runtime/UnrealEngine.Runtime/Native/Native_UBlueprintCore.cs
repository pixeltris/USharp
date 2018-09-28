using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UBlueprintCore
    {
        public delegate IntPtr Del_Get_SkeletonGeneratedClass(IntPtr instance);
        public delegate void Del_Set_SkeletonGeneratedClass(IntPtr instance, IntPtr value);
        public delegate IntPtr Del_Get_GeneratedClass(IntPtr instance);
        public delegate void Del_Set_GeneratedClass(IntPtr instance, IntPtr value);
        
        public static Del_Get_SkeletonGeneratedClass Get_SkeletonGeneratedClass;
        public static Del_Set_SkeletonGeneratedClass Set_SkeletonGeneratedClass;
        public static Del_Get_GeneratedClass Get_GeneratedClass;
        public static Del_Set_GeneratedClass Set_GeneratedClass;
    }
}
