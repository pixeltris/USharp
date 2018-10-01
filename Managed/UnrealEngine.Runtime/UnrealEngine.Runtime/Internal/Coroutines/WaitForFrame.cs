using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class WaitForExactFrame : ComparableYieldInstruction<WaitForExactFrame>
    {
        private bool isRelative;
        private ulong relativeFrame;
        private ulong startFrame;

        private ulong targetFrame;
        public ulong TargetFrame
        {
            get { return targetFrame; }
            set
            {
                if (targetFrame != value)
                {
                    targetFrame = value;
                    ComparableValueChanged();
                }
            }
        }

        public WaitForExactFrame(ulong frame)
            : this(frame, false)
        {
        }

        internal WaitForExactFrame(ulong frame, bool relative)
        {
            isRelative = relative;
            if (relative)
            {
                relativeFrame = frame;
            }
            else
            {
                TargetFrame = frame;
            }
        }

        public override bool KeepWaiting
        {
            get { return TargetFrame > EngineLoop.FrameNumber; }
        }

        public override void OnBegin()
        {
            if (isRelative)
            {
                startFrame = EngineLoop.FrameNumber;
                UpdateRelativeFrame();
            }
        }

        protected void UpdateRelativeFrame(ulong frame)
        {
            relativeFrame = frame;
            UpdateRelativeFrame();
        }

        private void UpdateRelativeFrame()
        {
            if (isRelative)
            {
                TargetFrame = startFrame + relativeFrame;
            }
        }

        public override int CompareTo(WaitForExactFrame other)
        {
            return TargetFrame.CompareTo(other.TargetFrame);
        }

        internal WaitForExactFrame PoolNew(ulong frame, bool relative)
        {
            isRelative = relative;
            if (relative)
            {
                relativeFrame = frame;
            }
            else
            {
                TargetFrame = frame;
            }
            return this;
        }
    }

    public class WaitForFrames : WaitForExactFrame
    {
        private ulong frames;
        public ulong Frames
        {
            get { return frames; }
            set
            {
                frames = value;
                UpdateRelativeFrame(value);
            }
        }

        public WaitForFrames(ulong frames) : base(frames, true)
        {
            this.frames = frames;
        }

        internal WaitForFrames PoolNew(ulong frames)
        {
            Frames = frames;
            return this;
        }
    }
}
