using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class WaitWhile : YieldInstruction
    {
        public WaitWhileCallback Callback { get; set; }

        public WaitWhile(WaitWhileCallback callback)
        {
            Callback = callback;
        }

        public override bool KeepWaiting
        {
            get { return Callback != null && Callback(); }
        }

        internal WaitWhile PoolNew(WaitWhileCallback callback)
        {
            Callback = callback;
            return this;
        }
    }

    public delegate bool WaitWhileCallback();
}
