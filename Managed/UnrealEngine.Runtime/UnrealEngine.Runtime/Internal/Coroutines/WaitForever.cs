using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class WaitForever : YieldInstruction
    {
        private bool complete;

        public override bool KeepWaiting
        {
            get { return !complete && Owner.IsPaused; }
        }

        public override void OnBegin()
        {            
            Owner.IsPaused = true;
        }

        public void Continue()
        {
            complete = true;
        }

        public void Reset()
        {
            complete = false;
        }

        internal WaitForever PoolNew()
        {
            complete = false;
            return this;
        }
    }
}
