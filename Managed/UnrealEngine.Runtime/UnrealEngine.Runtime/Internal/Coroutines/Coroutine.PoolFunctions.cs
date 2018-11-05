using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public partial class Coroutine
    {
        public const bool PoolByDefault = true;

        public static SetCoroutineGroup SetGroup(CoroutineGroup group)
        {
            return YieldPools.SetCoroutineGroup.PoolNew(group);
        }

        public static WaitForExactFrame WaitForExactFrame(ulong frame)
        {
            return YieldPools.WaitForExactFrame.PoolNew(frame);
        }

        public static WaitForExactTick WaitForExactTick(ulong tick)
        {
            return YieldPools.WaitForExactTick.PoolNew(tick);
        }

        public static WaitForTicks WaitForTicks(ulong ticks)
        {
            return YieldPools.WaitForTicks.PoolNew(ticks);
        }

        public static WaitForFrames WaitForFrames(ulong frames)
        {
            return YieldPools.WaitForFrames.PoolNew(frames);
        }

        public static WaitForTicks WaitOneTick()
        {
            return WaitForTicks(1);
        }

        public static WaitForFrames WaitOneFrame()
        {
            return WaitForFrames(1);
        }

        public static WaitForGroup WaitForGroup(CoroutineGroup group, ulong skipTicks = 0, uint skipFrames = 0)
        {
            return YieldPools.WaitForGroup.PoolNew(group, skipTicks, skipFrames);
        }

        public static WaitUntil WaitUntil(WaitUntilCallback callback)
        {
            return YieldPools.WaitUntil.PoolNew(callback);
        }

        public static WaitWhile WaitWhile(WaitWhileCallback callback)
        {
            return YieldPools.WaitWhile.PoolNew(callback);
        }

        public static WaitAll WaitAll(params YieldInstruction[] instructions)
        {
            return YieldPools.WaitAll.PoolNew(instructions);
        }

        public static WaitAny WaitAny(params YieldInstruction[] instructions)
        {
            return YieldPools.WaitAny.PoolNew(instructions);
        }

        public static WaitForever WaitForever()
        {
            return YieldPools.WaitForever.PoolNew();
        }

        public static WaitForCoroutine WaitForCoroutine(Coroutine coroutine)
        {
            return YieldPools.WaitForCoroutine.PoolNew(coroutine);
        }

        // WaitFor methods

        public static WaitFor WaitFor(TimeSpan time)
        {
            return YieldPools.WaitFor.PoolNew(time);
        }

        public static WaitForMilliseconds WaitForMilliseconds(uint milliseconds)
        {
            return YieldPools.WaitForMilliseconds.PoolNew(milliseconds);
        }

        public static WaitForMilliseconds WaitForMilliseconds(double milliseconds)
        {
            return YieldPools.WaitForMilliseconds.PoolNew(milliseconds);
        }

        public static WaitForSeconds WaitForSeconds(uint seconds)
        {
            return YieldPools.WaitForSeconds.PoolNew(seconds);
        }

        public static WaitForSeconds WaitForSeconds(double seconds)
        {
            return YieldPools.WaitForSeconds.PoolNew(seconds);
        }

        // WaitForReltime methods

        public static WaitForRealtime WaitForRealtime(TimeSpan time)
        {
            return YieldPools.WaitForRealtime.PoolNew(time);
        }

        public static WaitForMillisecondsRealtime WaitForMillisecondsRealtime(uint milliseconds)
        {
            return YieldPools.WaitForMillisecondsRealtime.PoolNew(milliseconds);
        }

        public static WaitForMillisecondsRealtime WaitForMillisecondsRealtime(double milliseconds)
        {
            return YieldPools.WaitForMillisecondsRealtime.PoolNew(milliseconds);
        }

        public static WaitForSecondsRealtime WaitForSecondsRealtime(uint seconds)
        {
            return YieldPools.WaitForSecondsRealtime.PoolNew(seconds);
        }

        public static WaitForSecondsRealtime WaitForSecondsRealtime(double seconds)
        {
            return YieldPools.WaitForSecondsRealtime.PoolNew(seconds);
        }
    }
}
