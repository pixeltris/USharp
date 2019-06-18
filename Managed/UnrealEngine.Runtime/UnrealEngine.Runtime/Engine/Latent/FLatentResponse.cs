using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    // Engine\Source\Runtime\Engine\Public\LatentActions.h

    /// <summary>
    /// The response to updating a latent action
    /// </summary>
    public struct FLatentResponse
    {
        public IntPtr Address;

        public FLatentResponse DoneIf(bool condition)
        {
            Native_FLatentResponse.DoneIf(Address, condition);
            return this;
        }

        public FLatentResponse TriggerLink(FName executionFunction, int linkID, FWeakObjectPtr callbackTarget)
        {
            Native_FLatentResponse.TriggerLink(Address, ref executionFunction, linkID, ref callbackTarget);
            return this;
        }

        public FLatentResponse FinishAndTriggerIf(bool condition, FName executionFunction, int linkID, FWeakObjectPtr callbackTarget)
        {
            Native_FLatentResponse.FinishAndTriggerIf(Address, condition, ref executionFunction, linkID, ref callbackTarget);
            return this;
        }

        public float ElapsedTime()
        {
            return Native_FLatentResponse.ElapsedTime(Address);
        }
    }
}
