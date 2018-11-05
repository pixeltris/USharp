using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public class WaitFor : ComparableYieldInstruction<WaitFor>
    {
        public override bool KeepWaiting
        {
            get
            {
                return endTimeSeconds > WorldTimeHelper.GetTimeSecondsChecked(worldAddress);
            }
        }

        public TimeSpan Time { get; internal set; }
        
        private float startTimeSeconds;
        private float endTimeSeconds;

        private IntPtr worldAddress;

        public WaitFor(TimeSpan time)
        {
            Time = time;
        }

        public override void OnBegin()
        {
            UObject ownerObj = Owner.Owner as UObject;
            if (ownerObj != null && ownerObj.Address != IntPtr.Zero)
            {
                worldAddress = Native_UObject.GetWorld(ownerObj.Address);
            }

            startTimeSeconds = WorldTimeHelper.GetTimeSecondsChecked(worldAddress);
            endTimeSeconds = (float)(startTimeSeconds + Time.TotalSeconds);
        }
        
        public override int CompareTo(WaitFor other)
        {
            return endTimeSeconds.CompareTo(other.endTimeSeconds);
        }
    }

    public class WaitForSeconds : WaitFor
    {
        public double Seconds
        {
            get { return Time.TotalSeconds; }
        }

        public WaitForSeconds(uint seconds) : base(TimeSpan.FromSeconds(seconds))
        {
        }

        public WaitForSeconds(double seconds) : base(TimeSpan.FromSeconds(seconds))
        {
        }
    }

    public class WaitForMilliseconds : WaitFor
    {
        public uint Milliseconds
        {
            get { return (uint)Time.TotalMilliseconds; }
        }

        public WaitForMilliseconds(double milliseconds) : base(TimeSpan.FromMilliseconds(milliseconds))
        {
        }

        public WaitForMilliseconds(uint milliseconds) : base(TimeSpan.FromMilliseconds(milliseconds))
        {
        }
    }
}
