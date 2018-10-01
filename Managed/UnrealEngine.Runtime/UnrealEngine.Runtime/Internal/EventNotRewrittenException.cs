using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Used to consume an event method (BlueprintEvent / RPC) which will be rewritten by AssemblyRewriter
    /// to redirect function calls to the correct class (BlueprintEvent) or correct network endpoint (RPC).
    /// </summary>
    public class EventNotRewrittenException : Exception
    {
        public EventNotRewrittenException()
            : base("BlueprintEvent / RPC function was not rewritten. Run this assembly through AssemblyRewriter.")
        {
        }
    }
}
