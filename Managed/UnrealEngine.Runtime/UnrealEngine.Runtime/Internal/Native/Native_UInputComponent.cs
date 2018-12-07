using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.InputCore;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_UInputComponent
    {
        public delegate int Del_Get_Priority(IntPtr instance);
        public delegate void Del_Set_Priority(IntPtr instance, int priority);
        public delegate csbool Del_Get_bBlockInput(IntPtr instance);
        public delegate void Del_Set_bBlockInput(IntPtr instance, csbool blockInput);
        public delegate void Del_ConditionalBuildKeyMap(IntPtr instance, IntPtr playerInput);
        public delegate float Del_GetAxisValue(IntPtr instance, ref FName axisName);
        public delegate float Del_GetAxisKeyValue(IntPtr instance, ref FKey axisKey);
        public delegate void Del_GetVectorAxisValue(IntPtr instance, ref FKey axisKey, out FVector result);
        public delegate csbool Del_HasBindings(IntPtr instance);
        public delegate IntPtr Del_AddActionBinding(IntPtr instance, IntPtr binding);
        public delegate void Del_ClearActionBindings(IntPtr instance);
        public delegate IntPtr Del_GetActionBinding(IntPtr instance, int bindingIndex);
        public delegate int Del_GetNumActionBindings(IntPtr instance);
        public delegate void Del_RemoveActionBinding(IntPtr instance, int bindingIndex);
        public delegate void Del_RemoveActionBindingByName(IntPtr instance, ref FName name);
        public delegate void Del_RemoveActionBindingByHandle(IntPtr instance, IntPtr value);
        public delegate void Del_ClearBindingValues(IntPtr instance);
        public delegate IntPtr Del_BindAction(IntPtr instance, ref FName actionName, byte keyEvent, IntPtr obj, IntPtr func);
        public delegate IntPtr Del_BindAxis(IntPtr instance, ref FName axisName, IntPtr obj, IntPtr func);
        public delegate IntPtr Del_BindAxisName(IntPtr instance, ref FName axisName);
        public delegate IntPtr Del_BindVectorAxis(IntPtr instance, ref FKey axisKey, IntPtr obj, IntPtr func);
        public delegate IntPtr Del_BindVectorAxisKey(IntPtr instance, ref FKey axisKey);
        public delegate IntPtr Del_BindKey(IntPtr instance, ref FKey key, byte keyEvent, IntPtr obj, IntPtr func);
        public delegate IntPtr Del_BindKeyChord(IntPtr instance, ref FKey key, csbool shift, csbool ctrl, csbool alt, csbool cmd, byte keyEvent, IntPtr obj, IntPtr func);
        public delegate IntPtr Del_BindTouch(IntPtr instance, byte keyEvent, IntPtr obj, IntPtr func);
        public delegate IntPtr Del_BindGesture(IntPtr instance, ref FKey gestureKey, IntPtr obj, IntPtr func);

        public static Del_Get_Priority Get_Priority;
        public static Del_Set_Priority Set_Priority;
        public static Del_Get_bBlockInput Get_bBlockInput;
        public static Del_Set_bBlockInput Set_bBlockInput;
        public static Del_ConditionalBuildKeyMap ConditionalBuildKeyMap;
        public static Del_GetAxisValue GetAxisValue;
        public static Del_GetAxisKeyValue GetAxisKeyValue;
        public static Del_GetVectorAxisValue GetVectorAxisValue;
        public static Del_HasBindings HasBindings;
        public static Del_AddActionBinding AddActionBinding;
        public static Del_ClearActionBindings ClearActionBindings;
        public static Del_GetActionBinding GetActionBinding;
        public static Del_GetNumActionBindings GetNumActionBindings;
        public static Del_RemoveActionBinding RemoveActionBinding;
        public static Del_RemoveActionBindingByName RemoveActionBindingByName;
        public static Del_RemoveActionBindingByHandle RemoveActionBindingByHandle;
        public static Del_ClearBindingValues ClearBindingValues;
        public static Del_BindAction BindAction;
        public static Del_BindAxis BindAxis;
        public static Del_BindAxisName BindAxisName;
        public static Del_BindVectorAxis BindVectorAxis;
        public static Del_BindVectorAxisKey BindVectorAxisKey;
        public static Del_BindKey BindKey;
        public static Del_BindKeyChord BindKeyChord;
        public static Del_BindTouch BindTouch;
        public static Del_BindGesture BindGesture;
    }
}
