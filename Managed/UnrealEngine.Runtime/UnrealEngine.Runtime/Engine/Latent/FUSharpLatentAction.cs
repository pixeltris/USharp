using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    // K2_NodeCallFunction handles latent functions
    // Engine\Source\Editor\BlueprintGraph\Private\K2Node_CallFunction.cpp

    // Engine\Source\Runtime\Engine\Classes\Engine\LatentActionManager.h

    public unsafe class FUSharpLatentAction
    {
        public IntPtr Address { get; internal set; }
        public bool Destroyed { get; private set; }

        // Hold onto a handle (so that we can get the managed object from the native object)
        internal GCHandle Handle;

        private int index;
        internal static List<FUSharpLatentAction> latentActions = new List<FUSharpLatentAction>();

        // Cache the delegate as otherwise it will get GCed
        internal ManagedLatentCallbackDel CallbackFunc;

        public FUSharpLatentAction()
        {
            CallbackFunc = Callback;

            latentActions.Add(this);
            index = latentActions.Count - 1;
        }

        private void Callback(ManagedLatentCallbackType callbackType, IntPtr thisPtr, IntPtr data)
        {
            try
            {
                switch (callbackType)
                {
                    case ManagedLatentCallbackType.FUSharpLatentAction_UpdateOperation:
                        UpdateOperation(new FLatentResponse(data));
                        break;
                    case ManagedLatentCallbackType.FUSharpLatentAction_NotifyObjectDestroyed:
                        NotifyObjectDestroyed();
                        break;
                    case ManagedLatentCallbackType.FUSharpLatentAction_NotifyActionAborted:
                        NotifyActionAborted();
                        break;
                    case ManagedLatentCallbackType.FUSharpLatentAction_GetDescription:
                        GetDescriptionInternal(data);
                        break;
                    case ManagedLatentCallbackType.FUSharpLatentAction_Destructor:
                        Destructor();
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

        internal static void OnUnload()
        {
            foreach (FUSharpLatentAction action in latentActions)
            {
                Native_FUSharpLatentAction.Set_bManagedObjectDestroyed(action.Address, true);
                action.Handle.Free();
                action.Handle = default(GCHandle);
                action.index = -1;
                action.Destroyed = true;
            }
            latentActions.Clear();
        }

        private void Destructor()
        {
            OnDestroyed();

            Debug.Assert(Handle != default(GCHandle));
            Handle.Free();
            Handle = default(GCHandle);

            int lastIndex = latentActions.Count - 1;
            Debug.Assert(lastIndex >= 0 && index >= 0);
            latentActions.RemoveAtSwap(lastIndex);
            if (lastIndex != index)
            {
                latentActions[index].index = index;
            }
            index = -1;
            Destroyed = true;
        }

        protected virtual void OnDestroyed()
        {
        }

        public virtual void UpdateOperation(FLatentResponse response)
        {
            response.DoneIf(true);
        }

        /// <summary>
        /// Lets the latent action know that the object which originated it has been garbage collected
        /// and the action is going to be destroyed (no more UpdateOperation calls will occur and
        /// CallbackTarget is already NULL)
        /// This is only called when the object goes away before the action is finished; perform normal
        /// cleanup when responding that the action is completed in UpdateOperation
        /// </summary>
        public virtual void NotifyObjectDestroyed()
        {
        }

        public virtual void NotifyActionAborted()
        {
        }

        private void GetDescriptionInternal(IntPtr str)
        {
            FStringMarshaler.ToNative(str, GetDescription());
        }

        public virtual string GetDescription()
        {
            return "Not implemented";
        }
    }

    public class FSimpleLatentAction : FUSharpLatentAction
    {
        public bool Complete { get; set; }

        public FLatentActionInfo LatentAction { get; private set; }
        public FWeakObjectPtr CallbackTarget { get; private set; }

        public FSimpleLatentAction(FLatentActionInfo latentAction)
        {
            LatentAction = latentAction;
            CallbackTarget = new FWeakObjectPtr(latentAction.CallbackTargetAddress);
        }

        public override void UpdateOperation(FLatentResponse response)
        {
            if (Complete)
            {
                response.FinishAndTriggerIf(true, LatentAction.ExecutionFunction, LatentAction.Linkage, CallbackTarget);
            }
        }
    }
}
