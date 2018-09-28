using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class WaitForGroup : YieldInstruction
    {        
        private bool hasWaitForTicks;
        private bool hasWaitForFrames;
        private WaitForTicks waitForTicks = null;
        private WaitForFrames waitForFrames = null;        

        public CoroutineGroup Group { get; set; }

        public WaitForGroup(CoroutineGroup group, ulong skipTicks = 0, uint skipFrames = 0)
        {
            Group = group;

            if (skipTicks > 0)
            {
                waitForTicks = new WaitForTicks(skipTicks);
                hasWaitForTicks = true;
            }
            if (skipFrames > 0)
            {
                waitForFrames = new WaitForFrames(skipFrames);
                hasWaitForFrames = true;
            }
        }

        public override bool KeepWaiting
        {
            get
            {
                if (hasWaitForTicks && waitForTicks.KeepWaiting)
                {
                    return true;
                }
                if (hasWaitForFrames && waitForFrames.KeepWaiting)
                {
                    return true;
                }
                return false;
            }
        }

        public override void OnBegin()
        {
            Owner.TargetGroup = Group;
            if (hasWaitForTicks)
            {
                waitForTicks.OnBegin();
            }
            if (hasWaitForFrames)
            {
                waitForFrames.OnBegin();
            }
        }

        public override void OnEnd()
        {
            Owner.TargetGroup = CoroutineGroup.None;
            if (hasWaitForTicks)
            {
                waitForTicks.OnEnd();
            }
            if (hasWaitForFrames)
            {
                waitForFrames.OnEnd();
            }
        }

        public override void OnOwnerSet()
        {
            if (hasWaitForTicks)
            {
                waitForTicks.Owner = Owner;
            }
            if (hasWaitForFrames)
            {
                waitForFrames.Owner = Owner;
            }
        }

        internal WaitForGroup PoolNew(CoroutineGroup group, ulong skipTicks, uint skipFrames)
        {
            hasWaitForTicks = false;
            hasWaitForFrames = false;

            Group = group;

            if (skipTicks > 0)
            {
                if (waitForTicks == null)
                {
                    waitForTicks = new WaitForTicks(skipTicks);
                }
                else
                {
                    waitForTicks.Ticks = skipTicks;
                }
                hasWaitForTicks = true;
            }
            if (skipFrames > 0)
            {
                if (waitForFrames == null)
                {
                    waitForFrames = new WaitForFrames(skipFrames);
                }
                else
                {
                    waitForFrames.Frames = skipFrames;
                }
                hasWaitForFrames = true;
            }

            return this;
        }
    }
}
