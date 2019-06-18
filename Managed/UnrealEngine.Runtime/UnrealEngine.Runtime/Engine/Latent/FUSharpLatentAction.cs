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
    public class FUSharpLatentAction
    {
        public IntPtr Address { get; internal set; }
        public bool Destroyed { get; private set; }

        // Hold onto a handle (so that we can get the managed object from the native object)
        internal GCHandle Handle;

        private int index;
        internal static List<FUSharpLatentAction> latentActions = new List<FUSharpLatentAction>();

        public delegate void UpdateOperationDel(FLatentResponse response);
        public delegate void NotifyDel();
        public delegate void GetDescriptionDel(ref FScriptArray str);

        // Cache the delegate as otherwise they will get GCed
        internal UpdateOperationDel UpdateOperationFunc;
        internal NotifyDel NotifyObjectDestroyedFunc;
        internal NotifyDel NotifyActionAbortedFunc;
        internal GetDescriptionDel GetDescriptionFunc;
        internal NotifyDel DestructorFunc;

        public FUSharpLatentAction()
        {
            UpdateOperationFunc = UpdateOperation;
            NotifyObjectDestroyedFunc = NotifyObjectDestroyed;
            NotifyActionAbortedFunc = NotifyActionAborted;
            GetDescriptionFunc = GetDescriptionInternal;
            DestructorFunc = Destructor;

            latentActions.Add(this);
            index = latentActions.Count - 1;
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

        private void GetDescriptionInternal(ref FScriptArray str)
        {
            FStringUnsafe stringUnsafe = new FStringUnsafe(str);
            stringUnsafe.Value = GetDescription();
            str = stringUnsafe.Array;
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
