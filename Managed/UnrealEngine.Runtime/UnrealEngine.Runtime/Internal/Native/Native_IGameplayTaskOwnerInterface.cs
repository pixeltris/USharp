using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_IGameplayTaskOwnerInterface
    {
        public delegate IntPtr Del_Internal_GetGameplayTasksComponent(IntPtr instance, IntPtr task);
        public delegate IntPtr Del_Internal_GetGameplayTaskOwner(IntPtr instance, IntPtr task);
        public delegate IntPtr Del_Internal_GetGameplayTaskAvatar(IntPtr instance, IntPtr task);
        public delegate byte Del_Internal_GetGameplayTaskDefaultPriority(IntPtr instance);
        public delegate void Del_Internal_OnGameplayTaskInitialized(IntPtr instance, IntPtr task);
        public delegate void Del_Internal_OnGameplayTaskActivated(IntPtr instance, IntPtr task);
        public delegate void Del_Internal_OnGameplayTaskDeactivated(IntPtr instance, IntPtr task);

        public static Del_Internal_GetGameplayTasksComponent Internal_GetGameplayTasksComponent;
        public static Del_Internal_GetGameplayTaskOwner Internal_GetGameplayTaskOwner;
        public static Del_Internal_GetGameplayTaskAvatar Internal_GetGameplayTaskAvatar;
        public static Del_Internal_GetGameplayTaskDefaultPriority Internal_GetGameplayTaskDefaultPriority;
        public static Del_Internal_OnGameplayTaskInitialized Internal_OnGameplayTaskInitialized;
        public static Del_Internal_OnGameplayTaskActivated Internal_OnGameplayTaskActivated;
        public static Del_Internal_OnGameplayTaskDeactivated Internal_OnGameplayTaskDeactivated;
    }
}
