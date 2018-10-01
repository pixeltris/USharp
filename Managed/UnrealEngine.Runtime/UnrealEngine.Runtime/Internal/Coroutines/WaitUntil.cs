using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class WaitUntil : YieldInstruction
    {
        public WaitUntilCallback Callback { get; set; }

        public WaitUntil(WaitUntilCallback callback)
        {
            Callback = callback;
        }

        public override bool KeepWaiting
        {
            get { return Callback != null && !Callback(); }
        }

        internal WaitUntil PoolNew(WaitUntilCallback callback)
        {
            Callback = callback;
            return this;
        }
    }

    public delegate bool WaitUntilCallback();
}
