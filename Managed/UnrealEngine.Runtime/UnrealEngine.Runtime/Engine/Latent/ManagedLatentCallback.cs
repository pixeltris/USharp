using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    internal enum ManagedLatentCallbackType : int
    {
        None,

        FUSharpLatentAction_UpdateOperation,
        FUSharpLatentAction_NotifyObjectDestroyed,
        FUSharpLatentAction_NotifyActionAborted,
        FUSharpLatentAction_GetDescription,
        FUSharpLatentAction_Destructor,

        UUSharpAsyncActionBase_Activate,
        UUSharpAsyncActionBase_RegisterWithGameInstanceWorldContext,
        UUSharpAsyncActionBase_RegisterWithGameInstance,
        UUSharpAsyncActionBase_SetReadyToDestroy,
        UUSharpAsyncActionBase_BeginDestroy,

        UUSharpOnlineBlueprintCallProxyBase_Activate,
        UUSharpOnlineBlueprintCallProxyBase_BeginDestroy,

        UUSharpGameplayTask_Activate,
        UUSharpGameplayTask_InitSimulatedTask,
        UUSharpGameplayTask_TickTask,
        UUSharpGameplayTask_ExternalConfirm,
        UUSharpGameplayTask_ExternalCancel,
        UUSharpGameplayTask_GetDebugString,
        UUSharpGameplayTask_OnDestroy,
        UUSharpGameplayTask_Pause,
        UUSharpGameplayTask_Resume,
        UUSharpGameplayTask_GenerateDebugDescription
    }

    internal delegate void ManagedLatentCallbackDel(ManagedLatentCallbackType callbackType, IntPtr thisPtr, IntPtr data);
    internal delegate void ManagedLatentCallbackRegisterDel(ManagedLatentCallbackDel callback);

    internal static class ManagedLatentCallbackHelper
    {
        internal static List<ManagedLatentCallbackDel> callbacks = new List<ManagedLatentCallbackDel>();
        internal static List<ManagedLatentCallbackRegisterDel> registerFuncs = new List<ManagedLatentCallbackRegisterDel>();

        /// <summary>
        /// Registers latent callbacks with C++
        /// 
        /// <para/>
        /// Make sure this is called after C# classes are registered with Unreal as the callbacks often rely on
        /// the C# classes being loaded in order to function correctly!
        /// </summary>
        internal static void RegisterCallbacks()
        {
            RegisterCallback("/Script/USharp.USharpGameplayTask", Native_UUSharpGameplayTask.Set_Callback);
            RegisterCallback("/Script/USharp.USharpOnlineBlueprintCallProxyBase", Native_UUSharpOnlineBlueprintCallProxyBase.Set_Callback);
            RegisterCallback("/Script/USharp.USharpAsyncActionBase", Native_UUSharpAsyncActionBase.Set_Callback);
        }

        internal static void UnregisterCallbacks()
        {
            foreach (ManagedLatentCallbackRegisterDel registerFunc in registerFuncs)
            {
                registerFunc(null);
            }
        }

        private static void RegisterCallback(string classPath, ManagedLatentCallbackRegisterDel registerCallbackFunc)
        {
            Runtime.UClass unrealClass = Runtime.UClass.GetClass(classPath);
            Debug.Assert(unrealClass != null);
            Type type = Runtime.UClass.GetType(unrealClass);
            Debug.Assert(type != null);

            BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic;
            MethodInfo callbackMethod = type.GetMethod("Callback", bindingFlags);
            Debug.Assert(callbackMethod != null);


            ManagedLatentCallbackDel callback = (ManagedLatentCallbackDel)callbackMethod.CreateDelegate(typeof(ManagedLatentCallbackDel));
            callbacks.Add(callback);
            registerCallbackFunc(callback);
            registerFuncs.Add(registerCallbackFunc);
        }
    }
}
