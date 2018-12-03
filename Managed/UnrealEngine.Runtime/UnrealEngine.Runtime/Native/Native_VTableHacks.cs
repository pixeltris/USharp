using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_VTableHacks
    {
        public delegate void Del_Set_VTableCallback(ref FScriptArray dummyName, IntPtr callback);

        public delegate void Del_CallOriginal_GetLifetimeReplicatedProps(IntPtr originalFunc, IntPtr obj, IntPtr outLifetimeProps);
        public delegate void Del_CallOriginal_SetupPlayerInputComponent(IntPtr originalFunc, IntPtr obj, IntPtr inputComponent);

        public static Del_Set_VTableCallback Set_VTableCallback;
        public static Del_CallOriginal_GetLifetimeReplicatedProps CallOriginal_GetLifetimeReplicatedProps;
        public static Del_CallOriginal_SetupPlayerInputComponent CallOriginal_SetupPlayerInputComponent;
    }
}
