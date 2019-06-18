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
    // Engine\Source\Runtime\Engine\Classes\Engine\LatentActionManager.h

    /// <summary>
    /// The latent action manager handles all pending latent actions for a single world
    /// </summary>
    //[UStruct(Flags = 0x00001201), UMetaPath("/Script/Engine.LatentActionManager", "Engine", UnrealModuleType.Engine)]
    public struct FLatentActionManager
    {
        public IntPtr Address;

        public FLatentActionManager(IntPtr address)
        {
            Address = address;
        }

        // TODO: Provide access to OnLatentActionsChanged?

        /// <summary>
        /// Advance pending latent actions by <paramref name="deltaTime"/>.
        /// If no object is specified it will process any outstanding actions for objects that have not been processed for this frame.
        /// </summary>
        /// <param name="obj">Specific object pending action list to advance.</param>
        /// <param name="deltaTime">Delta time.</param>
        public void ProcessLatentActions(UObject obj, float deltaTime)
        {
            Native_FLatentActionManager.ProcessLatentActions(Address, obj.Address, deltaTime);
        }

        /// <summary>
        /// Finds the action instance for the supplied UUID, or will return NULL if one does not already exist.
        /// </summary>
        /// <typeparam name="T">The type of the action</typeparam>
        /// <param name="actionObject">Object to check for pending actions.</param>
        /// <param name="uuid">UUID of the action we are looking for.</param>
        /// <param name="result">The found action (may be null even if this function returns true - in the case that the UUID exists under a different type).</param>
        /// <returns>True if an action was found fo the given UUID.</returns>
        public bool FindExistingAction<T>(UObject actionObject, int uuid, out T result) where T : FUSharpLatentAction
        {
            IntPtr address = Native_FLatentActionManager.FindExistingActionUSharp(Address, actionObject.Address, uuid);
            if (address != IntPtr.Zero)
            {
                GCHandle handle = GCHandle.FromIntPtr(address);
                result = handle.Target as T;
                return true;
            }
            result = default(T);
            return false;
        }

        /// <summary>
        /// Finds the action instance for the supplied UUID, or will return NULL if one does not already exist.
        /// </summary>
        /// <param name="actionObject">Object to check for pending actions.</param>
        /// <param name="uuid">UUID of the action we are looking for.</param>
        /// <returns>The found action (or <see cref="IntPtr.Zero"/> if not found).</returns>
        public IntPtr FindExistingActionPtr(UObject actionObject, int uuid)
        {
            return Native_FLatentActionManager.FindExistingAction(Address, actionObject.Address, uuid);
        }

        /// <summary>
        /// Removes all actions for given object. 
        /// It the latent actions are being currently handled (so the function is called inside a ProcessLatentActions functions scope) 
        /// there is no guarantee, that the action will be removed before its execution.
        /// </summary>
        /// <param name="obj">Specific object</param>
        public void RemoveActionsForObject(TWeakObject<UObject> obj)
        {
            Native_FLatentActionManager.RemoveActionsForObject(Address, ref obj.weakObjectPtr);
        }

        /// <summary>
        /// Adds a new action to the action list under a given UUID 
        /// </summary>
        /// <param name="actionObject">The target object for the action.</param>
        /// <param name="uuid">UUID of the action.</param>
        /// <param name="newAction">The action to add.</param>
        public void AddNewAction(UObject actionObject, int uuid, FUSharpLatentAction newAction)
        {
            GCHandle handle = GCHandle.Alloc(newAction, GCHandleType.Normal);
            IntPtr address = Native_FLatentActionManager.AddNewAction(Address, actionObject.Address, uuid, GCHandle.ToIntPtr(handle),
                newAction.UpdateOperationFunc, newAction.NotifyObjectDestroyedFunc, newAction.NotifyActionAbortedFunc,
                newAction.GetDescriptionFunc, newAction.DestructorFunc);
            Debug.Assert(address != IntPtr.Zero);
            newAction.Address = address;
            newAction.Handle = handle;
        }

        /// <summary>
        /// Resets the list of objects we have processed the latent action list for.
        /// </summary>
        public void BeginFrame()
        {
            Native_FLatentActionManager.BeginFrame(Address);
        }

        /// <summary>
        /// Returns the number of actions for a given object
        /// </summary>
        /// <param name="obj">Object to check for pending actions.</param>
        /// <returns>The number of actions for a given object.</returns>
        public int GetNumActionsForObject(TWeakObject<UObject> obj)
        {
            return Native_FLatentActionManager.GetNumActionsForObject(Address, ref obj.weakObjectPtr);
        }

        /// <summary>
        /// Builds a set of the UUIDs of pending latent actions on a specific object.
        /// </summary>
        /// <param name="obj">Object to query for latent actions.</param>
        /// <returns>An array UUIDs of the pending latent actions.</returns>
        public int[] GetActiveUUIDs(UObject obj)
        {
#if WITH_EDITOR
            using (TArrayUnsafe<int> resultUnsafe = new TArrayUnsafe<int>())
            {
                Native_FLatentActionManager.GetActiveUUIDs(Address, obj.Address, resultUnsafe.Address);
                return resultUnsafe.ToArray();
            }
#else
            return null;
#endif
        }

        /// <summary>
        /// Gets the description string of a pending latent action with the specified UUID for a given object.
        /// </summary>
        /// <param name="obj">Object to check.</param>
        /// <param name="uuid">The UUID of the latent action.</param>
        /// <returns>The description of the found pending latent action.</returns>
        public string GetDescription(UObject obj, int uuid)
        {
#if WITH_EDITOR
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FLatentActionManager.GetDescription(Address, obj.Address, uuid, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
#else
            return null;
#endif
        }
    }
}
