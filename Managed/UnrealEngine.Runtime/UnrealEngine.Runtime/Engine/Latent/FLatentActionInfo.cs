using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    // Engine\Source\Runtime\Engine\Classes\Engine\LatentActionManager.h

    /// <summary>
    /// Latent action info
    /// </summary>
    [UStruct(Flags = 0x00001201), BlueprintType, UMetaPath("/Script/Engine.LatentActionInfo", "Engine", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FLatentActionInfo
    {
        /// <summary>
        /// The resume point within the function to execute
        /// </summary>
        public int Linkage;

        /// <summary>
        /// The UUID for this action
        /// </summary>
        public int UUID;

        /// <summary>
        /// The function to execute.
        /// </summary>
        public FName ExecutionFunction;

        /// <summary>
        /// Object to execute the function on.
        /// </summary>
        public IntPtr CallbackTargetAddress;

        /// <summary>
        /// Object to execute the function on.
        /// </summary>
        public UObject CallbackTarget
        {
            get { return GCHelper.Find(CallbackTargetAddress); }
            set { CallbackTargetAddress = value == null ? IntPtr.Zero : value.Address; }
        }

        /// <summary>
        /// Represents the default constructor for the native struct <see cref="FLatentActionInfo"/>.
        /// </summary>
        public static FLatentActionInfo Default
        {
            get { return new FLatentActionInfo(-1, -1, null, IntPtr.Zero); }
        }

        public FLatentActionInfo(int linkage, int uuid, string functionName, UObject callbackTarget)
            : this(linkage, uuid, functionName, callbackTarget == null ? IntPtr.Zero : callbackTarget.Address)
        {
        }

        public FLatentActionInfo(int linkage, int uuid, string functionName, IntPtr callbackTarget)
        {
            Linkage = linkage;
            UUID = uuid;
            ExecutionFunction = (FName)functionName;
            CallbackTargetAddress = callbackTarget;
        }

        public FLatentActionInfo(string functionName, UObject callbackTarget, int linkage = 0)
            : this(functionName, callbackTarget == null ? IntPtr.Zero : callbackTarget.Address, linkage)
        {
        }

        public FLatentActionInfo(string functionName, IntPtr callbackTarget, int linkage = 0)
        {
            // 'Linkage' gets passed to the target function as a parameter. This lets use a single function with a multi-phase
            // latent action handler by using a switch/case block on linkage, and starting a new latent action depending on the linkage value.
            Linkage = linkage;
            UUID = Native_FLatentActionManager.GetNextUUID(callbackTarget);
            ExecutionFunction = (FName)functionName;
            CallbackTargetAddress = callbackTarget;

            // NOTE: Blueprint uses only a semi-unique UUID. See FCompilerResultsLog::CalculateStableIdentifierForLatentActionManager
        }
    }
}
