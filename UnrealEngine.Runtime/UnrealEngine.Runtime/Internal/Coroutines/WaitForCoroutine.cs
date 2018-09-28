using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class WaitForCoroutine : YieldInstruction
    {
        public Coroutine Coroutine { get; set; }

        public override bool KeepWaiting
        {
            get { return Coroutine != null && !Coroutine.Complete; }
        }

        public WaitForCoroutine(Coroutine coroutine)
        {
            Coroutine = coroutine;
        }

        internal WaitForCoroutine PoolNew(Coroutine coroutine)
        {
            Coroutine = coroutine;
            return this;
        }
    }
}
