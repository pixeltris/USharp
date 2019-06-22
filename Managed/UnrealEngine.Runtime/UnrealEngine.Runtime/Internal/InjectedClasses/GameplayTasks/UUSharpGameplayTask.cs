using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;
using UnrealEngine.Engine;

namespace UnrealEngine.GameplayTasks
{
    [UClass(Flags = (ClassFlags)0x0), UMetaPath("/Script/USharp.USharpGameplayTask")]
    public class UUSharpGameplayTask : UGameplayTask
    {
        private static unsafe void Callback(ManagedLatentCallbackType callbackType, IntPtr thisPtr, IntPtr data)
        {
            try
            {
                UUSharpGameplayTask obj = GCHelper.Find<UUSharpGameplayTask>(thisPtr);
                switch (callbackType)
                {
                    case ManagedLatentCallbackType.UUSharpGameplayTask_Activate:
                        obj.OnActivate();
                        break;
                    case ManagedLatentCallbackType.UUSharpGameplayTask_InitSimulatedTask:
                        obj.OnInitSimulatedTask(GCHelper.Find<UGameplayTasksComponent>(data));
                        break;
                    case ManagedLatentCallbackType.UUSharpGameplayTask_TickTask:
                        obj.OnTickTask(*(float*)data);
                        break;
                    case ManagedLatentCallbackType.UUSharpGameplayTask_ExternalConfirm:
                        obj.OnExternalConfirm(*(csbool*)data);
                        break;
                    case ManagedLatentCallbackType.UUSharpGameplayTask_ExternalCancel:
                        obj.OnExternalCancel();
                        break;
                    case ManagedLatentCallbackType.UUSharpGameplayTask_GetDebugString:
                        FStringMarshaler.ToNative(data, obj.OnGetDebugString());
                        break;
                    case ManagedLatentCallbackType.UUSharpGameplayTask_OnDestroy:
                        obj.OnDestroy(*(csbool*)data);
                        break;
                    case ManagedLatentCallbackType.UUSharpGameplayTask_Pause:
                        obj.OnPause();
                        break;
                    case ManagedLatentCallbackType.UUSharpGameplayTask_Resume:
                        obj.OnResume();
                        break;
                    case ManagedLatentCallbackType.UUSharpGameplayTask_GenerateDebugDescription:
                        FStringMarshaler.ToNative(data, obj.OnGenerateDebugDescription());
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                FMessage.LogException(e);
            }
        }

        /// <summary>
        /// Called to trigger the actual task once the delegates have been set up
        /// Note that the default implementation does nothing and you don't have to call it
        /// </summary>
        protected virtual void OnActivate()
        {
            Native_UUSharpGameplayTask.Base_Activate(Address);
        }

        protected virtual void OnInitSimulatedTask(UGameplayTasksComponent gameplayTasksComponent)
        {
            Native_UUSharpGameplayTask.Base_InitSimulatedTask(Address,
                gameplayTasksComponent == null ? IntPtr.Zero : gameplayTasksComponent.Address);
        }

        protected virtual void OnTickTask(float deltaTime)
        {
            // Base is function body is empty.
        }

        /// <summary>
        /// Called when the task is asked to confirm from an outside node. What this means depends on the individual task. By default, this does nothing other than ending if bEndTask is true.
        /// </summary>
        protected virtual void OnExternalConfirm(bool endTask)
        {
            Native_UUSharpGameplayTask.Base_ExternalConfirm(Address, endTask);
        }

        /// <summary>
        /// Called when the task is asked to cancel from an outside node. What this means depends on the individual task. By default, this does nothing other than ending the task.
        /// </summary>
        protected virtual void OnExternalCancel()
        {
            Native_UUSharpGameplayTask.Base_ExternalCancel(Address);
        }

        /// <summary>
        /// Return debug string describing task
        /// </summary>
        protected virtual string OnGetDebugString()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UUSharpGameplayTask.Base_GetDebugString(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// End and CleanUp the task - may be called by the task itself or by the task owner if the owner is ending.
        /// IMPORTANT! Do NOT call directly! Call EndTask() or TaskOwnerEnded() 
        /// IMPORTANT! When overriding this function make sure to call Super::OnDestroy(bOwnerFinished) as the last thing,
        ///            since the function internally marks the task as "Pending Kill", and this may interfere with internal BP mechanics
        /// </summary>
        protected virtual void OnDestroy(bool ownerFinished)
        {
            Native_UUSharpGameplayTask.Base_OnDestroy(Address, ownerFinished);
        }

        protected virtual void OnPause()
        {
            Native_UUSharpGameplayTask.Base_Pause(Address);
        }

        protected virtual void OnResume()
        {
            Native_UUSharpGameplayTask.Base_Resume(Address);
        }

        protected virtual string OnGenerateDebugDescription()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UUSharpGameplayTask.Base_GenerateDebugDescription(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }
    }
}
