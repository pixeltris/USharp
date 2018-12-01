using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_USharpClass
    {
        public delegate void Del_ClearFuncMapEx(IntPtr instance);
        public delegate void Del_SetFallbackFunctionInvoker(IntPtr instance, IntPtr function);
        public delegate IntPtr Del_SetFunctionInvoker(IntPtr instance, ref FScriptArray functionName, IntPtr invoker);
        public delegate void Del_SetSharpClassConstructor(IntPtr instance, IntPtr managedConstructor);
        public delegate IntPtr Del_Get_ManagedConstructor(IntPtr instance);
        public delegate void Del_Set_ManagedConstructor(IntPtr instance, IntPtr value);
        public delegate void Del_UpdateNativeParentConstructor(IntPtr instance);
        public delegate void Del_GetDummyObjects(out IntPtr dummyObject1, out IntPtr dummyObject2, out IntPtr dummyObject3);
        public delegate void Del_Set_VTableCallback(int index, IntPtr callback);
        public delegate void Del_CallOriginalGetLifetimeReplicatedProps(IntPtr originalFunc, IntPtr obj, IntPtr outLifetimeProps);

        public static Del_ClearFuncMapEx ClearFuncMapEx;
        public static Del_SetFallbackFunctionInvoker SetFallbackFunctionInvoker;
        public static Del_SetFunctionInvoker SetFunctionInvoker;
        public static Del_SetSharpClassConstructor SetSharpClassConstructor;
        public static Del_Get_ManagedConstructor Get_ManagedConstructor;
        public static Del_Set_ManagedConstructor Set_ManagedConstructor;
        public static Del_UpdateNativeParentConstructor UpdateNativeParentConstructor;
        public static Del_GetDummyObjects GetDummyObjects;
        public static Del_Set_VTableCallback Set_VTableCallback;
        public static Del_CallOriginalGetLifetimeReplicatedProps CallOriginalGetLifetimeReplicatedProps;
    }
}
