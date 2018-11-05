using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    internal static class YieldPools
    {
        public class SetCoroutineGroupPool : YieldInstructionPool<SetCoroutineGroup>
        {
            protected override SetCoroutineGroup New()
            {
                return new SetCoroutineGroup(CoroutineGroup.None);
            }
            public SetCoroutineGroup PoolNew(CoroutineGroup group)
            {
                return GetObject().PoolNew(group);
            }
        }

        public class WaitForExactFramePool : YieldInstructionPool<WaitForExactFrame>
        {
            protected override WaitForExactFrame New()
            {
                return new WaitForExactFrame(0);
            }
            public WaitForExactFrame PoolNew(ulong frame)
            {
                return GetObject().PoolNew(frame, false);
            }
        }

        public class WaitForExactTickPool : YieldInstructionPool<WaitForExactTick>
        {
            protected override WaitForExactTick New()
            {
                return new WaitForExactTick(0);
            }
            public WaitForExactTick PoolNew(ulong tick)
            {
                return GetObject().PoolNew(tick, false);
            }
        }

        public class WaitForTicksPool : YieldInstructionPool<WaitForTicks>
        {
            protected override WaitForTicks New()
            {
                return new WaitForTicks(0);
            }
            public WaitForTicks PoolNew(ulong ticks)
            {
                return GetObject().PoolNew(ticks);
            }
        }

        public class WaitForFramesPool : YieldInstructionPool<WaitForFrames>
        {
            protected override WaitForFrames New()
            {
                return new WaitForFrames(0);
            }
            public WaitForFrames PoolNew(ulong frames)
            {
                return GetObject().PoolNew(frames);
            }
        }

        public class WaitForGroupPool : YieldInstructionPool<WaitForGroup>
        {
            protected override WaitForGroup New()
            {
                return new WaitForGroup(default(CoroutineGroup));
            }
            public WaitForGroup PoolNew(CoroutineGroup group, ulong skipTicks = 0, uint skipFrames = 0)
            {
                return GetObject().PoolNew(group, skipTicks, skipFrames);
            }
        }

        public class WaitUntilPool : YieldInstructionPool<WaitUntil>
        {
            protected override WaitUntil New()
            {
                return new WaitUntil(null);
            }
            public WaitUntil PoolNew(WaitUntilCallback callback)
            {
                return GetObject().PoolNew(callback);
            }
        }

        public class WaitWhilePool : YieldInstructionPool<WaitWhile>
        {
            protected override WaitWhile New()
            {
                return new WaitWhile(null);
            }
            public WaitWhile PoolNew(WaitWhileCallback callback)
            {
                return GetObject().PoolNew(callback);
            }
        }

        public class WaitAllPool : YieldInstructionPool<WaitAll>
        {
            protected override WaitAll New()
            {
                return new WaitAll(null);
            }
            public WaitAll PoolNew(params YieldInstruction[] instructions)
            {
                return GetObject().PoolNew(instructions);
            }
        }

        public class WaitAnyPool : YieldInstructionPool<WaitAny>
        {
            protected override WaitAny New()
            {
                return new WaitAny(null);
            }
            public WaitAny PoolNew(params YieldInstruction[] instructions)
            {
                return GetObject().PoolNew(instructions);
            }
        }

        public class WaitForeverPool : YieldInstructionPool<WaitForever>
        {
            protected override WaitForever New()
            {
                return new WaitForever();
            }
            public WaitForever PoolNew()
            {
                return GetObject().PoolNew();
            }
        }

        public class WaitForCoroutinePool : YieldInstructionPool<WaitForCoroutine>
        {
            protected override WaitForCoroutine New()
            {
                return new WaitForCoroutine(null);
            }
            public WaitForCoroutine PoolNew(Coroutine coroutine)
            {
                return GetObject().PoolNew(coroutine);
            }
        }

        // WaitFor

        public class WaitForPool : YieldInstructionPool<WaitFor>
        {
            protected override WaitFor New()
            {
                return new WaitFor(TimeSpan.Zero);
            }
            public WaitFor PoolNew(TimeSpan time)
            {
                return GetObject().PoolNew(time);
            }
        }

        public class WaitForSecondsPool : YieldInstructionPool<WaitForSeconds>
        {
            protected override WaitForSeconds New()
            {
                return new WaitForSeconds(0);
            }
            public WaitForSeconds PoolNew(uint seconds)
            {
                return GetObject().PoolNew(TimeSpan.FromSeconds(seconds));
            }
            public WaitForSeconds PoolNew(double seconds)
            {
                return GetObject().PoolNew(TimeSpan.FromSeconds(seconds));
            }
        }

        public class WaitForMillisecondsPool : YieldInstructionPool<WaitForMilliseconds>
        {
            protected override WaitForMilliseconds New()
            {
                return new WaitForMilliseconds(0);
            }
            public WaitForMilliseconds PoolNew(uint milliseconds)
            {
                return GetObject().PoolNew(TimeSpan.FromMilliseconds(milliseconds));
            }
            public WaitForMilliseconds PoolNew(double milliseconds)
            {
                return GetObject().PoolNew(TimeSpan.FromMilliseconds(milliseconds));
            }
        }

        // WaitForRealtime

        public class WaitForRealtimePool : YieldInstructionPool<WaitForRealtime>
        {
            protected override WaitForRealtime New()
            {
                return new WaitForRealtime(TimeSpan.Zero);
            }
            public WaitForRealtime PoolNew(TimeSpan time)
            {
                return GetObject().PoolNewTime(time);
            }
        }

        public class WaitForSecondsRealtimePool : YieldInstructionPool<WaitForSecondsRealtime>
        {
            protected override WaitForSecondsRealtime New()
            {
                return new WaitForSecondsRealtime(0);
            }
            public WaitForSecondsRealtime PoolNew(uint seconds)
            {
                return GetObject().PoolNewTime(TimeSpan.FromSeconds(seconds));
            }
            public WaitForSecondsRealtime PoolNew(double seconds)
            {
                return GetObject().PoolNewTime(TimeSpan.FromSeconds(seconds));
            }
        }

        public class WaitForMillisecondsRealtimePool : YieldInstructionPool<WaitForMillisecondsRealtime>
        {
            protected override WaitForMillisecondsRealtime New()
            {
                return new WaitForMillisecondsRealtime(0);
            }
            public WaitForMillisecondsRealtime PoolNew(uint milliseconds)
            {
                return GetObject().PoolNewTime(TimeSpan.FromMilliseconds(milliseconds));
            }
            public WaitForMillisecondsRealtime PoolNew(double milliseconds)
            {
                return GetObject().PoolNewTime(TimeSpan.FromMilliseconds(milliseconds));
            }
        }

        public static readonly SetCoroutineGroupPool SetCoroutineGroup = new SetCoroutineGroupPool();
        public static readonly WaitForExactFramePool WaitForExactFrame = new WaitForExactFramePool();
        public static readonly WaitForExactTickPool WaitForExactTick = new WaitForExactTickPool();
        public static readonly WaitForTicksPool WaitForTicks = new WaitForTicksPool();
        public static readonly WaitForFramesPool WaitForFrames = new WaitForFramesPool();
        public static readonly WaitForGroupPool WaitForGroup = new WaitForGroupPool();
        public static readonly WaitUntilPool WaitUntil = new WaitUntilPool();
        public static readonly WaitWhilePool WaitWhile = new WaitWhilePool();
        public static readonly WaitAllPool WaitAll = new WaitAllPool();
        public static readonly WaitAnyPool WaitAny = new WaitAnyPool();
        public static readonly WaitForeverPool WaitForever = new WaitForeverPool();
        public static readonly WaitForCoroutinePool WaitForCoroutine = new WaitForCoroutinePool();
        public static readonly WaitForPool WaitFor = new WaitForPool();
        public static readonly WaitForSecondsPool WaitForSeconds = new WaitForSecondsPool();
        public static readonly WaitForMillisecondsPool WaitForMilliseconds = new WaitForMillisecondsPool();
        public static readonly WaitForRealtimePool WaitForRealtime = new WaitForRealtimePool();
        public static readonly WaitForSecondsRealtimePool WaitForSecondsRealtime = new WaitForSecondsRealtimePool();
        public static readonly WaitForMillisecondsRealtimePool WaitForMillisecondsRealtime = new WaitForMillisecondsRealtimePool();        
    }

    internal static class YieldPoolExtensions
    {
        internal static T PoolNew<T>(this T instruction, TimeSpan time) where T : WaitFor
        {
            instruction.Time = time;
            return instruction;
        }

        internal static T PoolNewTime<T>(this T instruction, TimeSpan time) where T : WaitForRealtime
        {
            instruction.Time = time;
            return instruction;
        }
    }
}
