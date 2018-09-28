using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // This doesn't update to the time dilation each tick.
    // If this is on an actor and the actor changes the time dilation
    // over a period of time then that time dilation currently wont
    // be reflected on this instance. It will only be captured OnBegin.
    //
    // Probably need want a specialization or override IsComparable and add
    // AutoUpdateDilation which sets IsComparable to false (this needs to be
    // done early on and possibly getter only, set in constructor) - make
    // "dilated" an enum? DilationMode { None, Dilated, AutoDilated/UpdateDilationOnTick }
    // 
    //
    // We also need to allow changes of Time to be reflected properly
    // currently it will have no impact.
    //
    public class WaitFor : ComparableYieldInstruction<WaitFor>
    {
        public override bool KeepWaiting
        {
            get { return endTime > EngineLoop.Time; }
        }

        private bool dilated;
        public bool Dilated
        {
            get { return dilated; }
            set
            {
                if (dilated != value)
                {
                    dilated = value;
                    if (dilated)
                    {
                        UpdateDilatedTime();
                    }
                }
            }
        }


        private TimeSpan timeDilation = TimeSpan.FromSeconds(1);
        public TimeSpan TimeDilation
        {
            get { return timeDilation; }
            private set
            {
                timeDilation = value;
                UpdateTotalTime();
            }
        }

        private TimeSpan time;
        public TimeSpan Time
        {
            get { return time; }
            set
            {
                time = value;
                UpdateTotalTime();
            }
        }

        public TimeSpan TotalTime { get; private set; }

        private TimeSpan endTime;
        private TimeSpan startTime;

        public WaitFor(TimeSpan time, bool dilated = true)
        {
            Time = time;
            this.dilated = dilated;
        }

        public override void OnBegin()
        {
            UpdateDilatedTime();
            startTime = EngineLoop.Time;
            endTime = startTime + TotalTime;
        }

        private void UpdateDilatedTime()
        {
            if (Dilated && Owner.Owner != null)
            {
                UObject ownerObj = Owner.Owner as UObject;
                if (ownerObj != null)
                {
                    TimeDilation = TimeSpan.FromSeconds(Native_AActor.GetActorTimeDilationOrDefault(
                        ownerObj == null ? IntPtr.Zero : ownerObj.Address));
                }
                else
                {
                    TimeDilation = TimeSpan.FromSeconds(1);
                }
            }
            UpdateTotalTime();
        }

        private void UpdateTotalTime()
        {
            TotalTime = TimeSpan.FromSeconds(time.TotalSeconds * (dilated ? TimeDilation.TotalSeconds : 1));
        }        
        
        public override int CompareTo(WaitFor other)
        {
            return endTime.CompareTo(other.endTime);
        }

        internal void SetTime(TimeSpan time, bool dilated)
        {
            Time = time;
            this.dilated = dilated;
        }
    }

    public class WaitForSeconds : WaitFor
    {
        public double Seconds
        {
            get { return Time.TotalSeconds; }
            set { Time = TimeSpan.FromSeconds(value); }
        }

        public WaitForSeconds(uint seconds, bool dilated = true) : base(TimeSpan.FromSeconds(seconds), dilated)
        {
        }

        public WaitForSeconds(double seconds, bool dilated = true) : base(TimeSpan.FromSeconds(seconds), dilated)
        {
        }
    }

    public class WaitForMilliseconds : WaitFor
    {
        public uint Milliseconds
        {
            get { return (uint)Time.TotalMilliseconds; }
            set { Time = TimeSpan.FromMilliseconds(value); }
        }

        public WaitForMilliseconds(double milliseconds, bool dilated = true) : base(TimeSpan.FromMilliseconds(milliseconds), dilated)
        {
        }

        public WaitForMilliseconds(uint milliseconds, bool dilated = true) : base(TimeSpan.FromMilliseconds(milliseconds), dilated)
        {
        }
    }

    public class WaitForRealtime : WaitFor
    {
        public WaitForRealtime(TimeSpan time) : base(time, false)
        {
        }
    }

    public class WaitForSecondsRealtime : WaitForSeconds
    {
        public WaitForSecondsRealtime(uint seconds) : base(seconds, false)
        {
        }

        public WaitForSecondsRealtime(double seconds) : base(seconds, false)
        {
        }
    }

    public class WaitForMillisecondsRealtime : WaitForMilliseconds
    {
        public WaitForMillisecondsRealtime(uint milliseconds) : base(milliseconds, false)
        {
        }

        public WaitForMillisecondsRealtime(double milliseconds) : base(milliseconds, false)
        {
        }
    }
}
