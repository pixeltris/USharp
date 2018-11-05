using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class WaitForExactTick : ComparableYieldInstruction<WaitForExactTick>
    {
        private bool isRelative;
        private ulong relativeTick;
        private ulong startTick;
        
        private ulong targetTick;
        public ulong TargetTick
        {
            get { return targetTick; }
            set
            {
                if (targetTick != value)
                {
                    targetTick = value;
                    ComparableValueChanged();
                }
            }
        }

        public WaitForExactTick(ulong tick)
            : this(tick, false)
        {
        }

        internal WaitForExactTick(ulong tick, bool relative)
        {
            isRelative = relative;
            if (isRelative)
            {
                relativeTick = tick;
            }
            else
            {
                TargetTick = tick;
            }
        }

        public override bool KeepWaiting
        {
            get { return TargetTick > EngineLoop.WorldTickCounter; }
        }

        public override void OnBegin()
        {
            if (isRelative)
            {
                startTick = EngineLoop.WorldTickCounter;
                UpdateRelativeTick();
            }
        }

        protected void UpdateRelativeTick(ulong tick)
        {
            relativeTick = tick;
            UpdateRelativeTick();
        }

        private void UpdateRelativeTick()
        {
            if (isRelative)
            {
                TargetTick = startTick + relativeTick;
            }
        }

        public override int CompareTo(WaitForExactTick other)
        {
            return TargetTick.CompareTo(other.TargetTick);
        }

        internal WaitForExactTick PoolNew(ulong tick, bool relative)
        {
            isRelative = relative;
            if (relative)
            {
                relativeTick = tick;
            }
            else
            {
                TargetTick = tick;
            }
            return this;
        }
    }

    public class WaitForTicks : WaitForExactTick
    {
        private ulong ticks;
        public ulong Ticks
        {
            get { return ticks; }
            set
            {
                ticks = value;
                UpdateRelativeTick(value);
            }
        }

        public WaitForTicks(ulong ticks) : base(ticks, true)
        {
            this.ticks = ticks;
        }

        internal WaitForTicks PoolNew(ulong ticks)
        {
            Ticks = ticks;
            return this;
        }
    }
}
