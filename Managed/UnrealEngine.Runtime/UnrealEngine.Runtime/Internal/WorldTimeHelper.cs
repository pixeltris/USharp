using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public unsafe struct WorldTimeHelper
    {
        public IntPtr WorldAddress;
        public bool IsValid
        {
            get { return WorldAddress != IntPtr.Zero; }
        }

        public WorldTimeHelper(IntPtr worldAddress)
        {
            WorldAddress = worldAddress;
        }

        /// <summary>
        /// Time since level began play, but IS paused when the game is paused, and IS dilated/clamped.
        /// </summary>
        public TimeSpan Time
        {
            get { return TimeSpan.FromSeconds(TimeSeconds); }
        }

        /// <summary>
        /// Time since level began play, but IS NOT paused when the game is paused, and IS dilated/clamped.
        /// </summary>
        public TimeSpan UnpausedTime
        {
            get { return TimeSpan.FromSeconds(UnpausedTimeSeconds); }
        }

        /// <summary>
        /// Frame delta time adjusted by e.g. time dilation.
        /// </summary>
        public TimeSpan DeltaTime
        {
            get { return TimeSpan.FromSeconds(DeltaTimeSeconds); }
        }

        /// <summary>
        /// time at which to start pause
        /// </summary>
        public TimeSpan PauseDelayTime
        {
            get { return TimeSpan.FromSeconds(PauseDelay); }
        }

        private static int timeSecondsOffset;
        /// <summary>
        /// Time in seconds since level began play, but IS paused when the game is paused, and IS dilated/clamped.
        /// </summary>
        public float TimeSeconds
        {
            get
            {
                if (IsValid)
                {
                    return *(float*)(WorldAddress + timeSecondsOffset);
                }
                else
                {
                    return 0;
                }
            }
        }

        private static int unpausedTimeSecondsOffset;
        /// <summary>
        /// Time in seconds since level began play, but IS NOT paused when the game is paused, and IS dilated/clamped.
        /// </summary>
        public float UnpausedTimeSeconds
        {
            get
            {
                if (IsValid)
                {
                    return *(float*)(WorldAddress + unpausedTimeSecondsOffset);
                }
                else
                {
                    return 0;
                }
            }
        }

        private static int realTimeSecondsOffset;
        /// <summary>
        /// Time in seconds since level began play, but IS NOT paused when the game is paused, and IS NOT dilated/clamped.
        /// </summary>
        public float RealTimeSeconds
        {
            get
            {
                if (IsValid)
                {
                    return *(float*)(WorldAddress + realTimeSecondsOffset);
                }
                else
                {
                    return 0;
                }
            }
        }

        private static int deltaTimeSecondsOffset;
        /// <summary>
        /// Frame delta time in seconds adjusted by e.g. time dilation.
        /// </summary>
        public float DeltaTimeSeconds
        {
            get
            {
                if (IsValid)
                {
                    return *(float*)(WorldAddress + deltaTimeSecondsOffset);
                }
                else
                {
                    return 0;
                }
            }
        }

        private static int pauseDelayOffset;
        /// <summary>
        /// time at which to start pause
        /// </summary>
        public float PauseDelay
        {
            get
            {
                if (IsValid)
                {
                    return *(float*)(WorldAddress + pauseDelayOffset);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Kismet debugging flags - they can be only editor only, but they're uint32, so it doens't make much difference
        /// </summary>
        public bool DebugPauseExecution
        {
            get
            {
                if (IsValid)
                {
                    return Native_UWorld.Get_bDebugPauseExecution(WorldAddress);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns true if the world is in the paused state
        /// </summary>
        public bool IsPaused
        {
            get
            {
                if (IsValid)
                {
                    return Native_UWorld.IsPaused(WorldAddress);
                }
                else
                {
                    // What's best to return in this case?
                    return true;
                }
            }
        }

        internal static void OnNativeFunctionsRegistered()
        {
            timeSecondsOffset = Native_UWorld.Offset_TimeSeconds();
            unpausedTimeSecondsOffset = Native_UWorld.Offset_UnpausedTimeSeconds();
            realTimeSecondsOffset = Native_UWorld.Offset_RealTimeSeconds();
            deltaTimeSecondsOffset = Native_UWorld.Offset_DeltaTimeSeconds();
            pauseDelayOffset = Native_UWorld.Offset_PauseDelay();
        }

        public static TimeSpan GetTimeChecked(IntPtr world)
        {
            return GetTime(GetWorldChecked(world));
        }

        public static TimeSpan GetTime(IntPtr world)
        {
            return new WorldTimeHelper(world).Time;
        }

        public static TimeSpan GetUnpausedTimeChecked(IntPtr world)
        {
            return GetUnpausedTime(GetWorldChecked(world));
        }

        public static TimeSpan GetUnpausedTime(IntPtr world)
        {
            return new WorldTimeHelper(world).UnpausedTime;
        }

        public static TimeSpan GetDeltaTimeChecked(IntPtr world)
        {
            return GetDeltaTime(GetWorldChecked(world));
        }

        public static TimeSpan GetDeltaTime(IntPtr world)
        {
            return new WorldTimeHelper(world).DeltaTime;
        }

        // Seconds

        public static float GetTimeSecondsChecked(IntPtr world)
        {
            return GetTimeSeconds(GetWorldChecked(world));
        }

        public static float GetTimeSeconds(IntPtr world)
        {
            return new WorldTimeHelper(world).TimeSeconds;
        }

        public static float GetUnpausedTimeSecondsChecked(IntPtr world)
        {
            return GetUnpausedTimeSeconds(GetWorldChecked(world));
        }

        public static float GetUnpausedTimeSeconds(IntPtr world)
        {
            return new WorldTimeHelper(world).UnpausedTimeSeconds;
        }

        public static float GetDeltaTimeSecondsChecked(IntPtr world)
        {
            return GetDeltaTimeSeconds(GetWorldChecked(world));
        }

        public static float GetDeltaTimeSeconds(IntPtr world)
        {
            return new WorldTimeHelper(world).DeltaTimeSeconds;
        }

        private static IntPtr GetWorldChecked(IntPtr world)
        {
            if (world == IntPtr.Zero)
            {
                world = EngineLoop.WorldTime.WorldAddress;
                Debug.Assert(world != null, "WorldTimeHelper failed to find a world instance");
            }
            return world;
        }
    }
}
