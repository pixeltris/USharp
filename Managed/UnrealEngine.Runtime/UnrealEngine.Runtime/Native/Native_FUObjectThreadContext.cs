using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FUObjectThreadContext
    {
        public delegate void Del_PopInitializer();
        public delegate void Del_PushInitializer(IntPtr initializer);
        public delegate IntPtr Del_TopInitializer();
        public delegate int Del_Get_IsInConstructor();
        public delegate IntPtr Del_Get_ConstructedObject();
        public delegate IntPtr Del_Get_SerializedObject();

        public static Del_PopInitializer PopInitializer;
        public static Del_PushInitializer PushInitializer;
        public static Del_TopInitializer TopInitializer;
        public static Del_Get_IsInConstructor Get_IsInConstructor;
        public static Del_Get_ConstructedObject Get_ConstructedObject;
        public static Del_Get_SerializedObject Get_SerializedObject;
    }
}
