using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class WaitForRealtime : ComparableYieldInstruction<WaitForRealtime>
    {
        public override bool KeepWaiting
        {
            get { return endTime > EngineLoop.Time; }
        }

        public TimeSpan Time { get; internal set; }

        private TimeSpan startTime;
        private TimeSpan endTime;

        public WaitForRealtime(TimeSpan time)
        {
            Time = time;
        }

        public override void OnBegin()
        {
            startTime = EngineLoop.Time;
            endTime = startTime + Time;
        }

        public override int CompareTo(WaitForRealtime other)
        {
            return endTime.CompareTo(other.endTime);
        }
    }

    public class WaitForSecondsRealtime : WaitForRealtime
    {
        public double Seconds
        {
            get { return Time.TotalSeconds; }
        }

        public WaitForSecondsRealtime(uint seconds) : base(TimeSpan.FromSeconds(seconds))
        {
        }

        public WaitForSecondsRealtime(double seconds) : base(TimeSpan.FromSeconds(seconds))
        {
        }
    }

    public class WaitForMillisecondsRealtime : WaitForRealtime
    {
        public uint Milliseconds
        {
            get { return (uint)Time.TotalMilliseconds; }
        }

        public WaitForMillisecondsRealtime(uint milliseconds) : base(TimeSpan.FromMilliseconds(milliseconds))
        {
        }

        public WaitForMillisecondsRealtime(double milliseconds) : base(TimeSpan.FromMilliseconds(milliseconds))
        {
        }
    }
}
