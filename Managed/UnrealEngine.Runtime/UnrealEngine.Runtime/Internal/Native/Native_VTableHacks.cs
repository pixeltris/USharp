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
        public delegate void Del_CallOriginal_ActorBeginPlay(IntPtr originalFunc, IntPtr obj);
        public delegate void Del_CallOriginal_ActorEndPlay(IntPtr originalFunc, IntPtr obj, byte endPlayReason);
        public delegate void Del_CallOriginal_ActorComponentBeginPlay(IntPtr originalFunc, IntPtr obj);
        public delegate void Del_CallOriginal_ActorComponentEndPlay(IntPtr originalFunc, IntPtr obj, byte endPlayReason);
        public delegate void Del_CallOriginal_PlayerControllerSetupInputComponent(IntPtr originalFunc, IntPtr obj);
        public delegate void Del_CallOriginal_PlayerControllerUpdateRotation(IntPtr originalFunc, IntPtr obj, float deltaTime);

        public static Del_Set_VTableCallback Set_VTableCallback;
        public static Del_CallOriginal_GetLifetimeReplicatedProps CallOriginal_GetLifetimeReplicatedProps;
        public static Del_CallOriginal_SetupPlayerInputComponent CallOriginal_SetupPlayerInputComponent;
        public static Del_CallOriginal_ActorBeginPlay CallOriginal_ActorBeginPlay;
        public static Del_CallOriginal_ActorEndPlay CallOriginal_ActorEndPlay;
        public static Del_CallOriginal_ActorComponentBeginPlay CallOriginal_ActorComponentBeginPlay;
        public static Del_CallOriginal_ActorComponentEndPlay CallOriginal_ActorComponentEndPlay;
        public static Del_CallOriginal_PlayerControllerSetupInputComponent CallOriginal_PlayerControllerSetupInputComponent;
    }
}
