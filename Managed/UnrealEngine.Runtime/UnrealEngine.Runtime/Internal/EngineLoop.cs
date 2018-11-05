using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public static class EngineLoop
    {
        private static Stopwatch stopwatch;

        /// <summary>
        /// This will start at zero and be saved/loaded on hotreload
        /// </summary>
        private static TimeSpan startTime;

        /// <summary>
        /// A snapshot of the total elapsed time taken in functions: OnBeginFrame / OnEndFrame / Tick
        /// </summary>
        public static TimeSpan TickTime { get; private set; }

        /// <summary>
        /// The total elapsed delta time which is incremented by delta time on each tick
        /// </summary>
        public static double TimeByDeltaTime { get; private set; }

        /// <summary>
        /// The total elapsed time starting from when EngineLoop was first initialized
        /// </summary>
        public static TimeSpan Time
        {
            get { return startTime + stopwatch.Elapsed; }
        }

        private static ulong lastTickCounter;
        public static ulong LastTickCounter { get; private set; }
        public static ulong TickCounter { get; private set; }        

        private static uint lastFrameNumber;
        public static ulong LastFrameNumber { get; private set; }
        public static ulong FrameNumber { get; private set; }

        private static uint lastFrameNumberRenderThread;
        public static ulong LastFrameNumberRenderThread { get; private set; }
        public static ulong FrameNumberRenderThread { get; private set; }

        public static WorldTimeHelper WorldTime;

        /// <summary>
        /// The same as TickCounter but only updated when the active world isn't paused
        /// </summary>
        public static ulong WorldTickCounter { get; private set; }
        /// <summary>
        /// The same as FrameNumber but only updated when the active world isn't paused
        /// </summary>
        public static ulong WorldFrameNumber { get; private set; }

        static EngineLoop()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        internal static void OnNativeFunctionsRegistered()
        {
            FTicker.AddTicker(Tick);

            FCoreDelegates.OnBeginFrame.Bind(OnBeginFrame);
            FCoreDelegates.OnEndFrame.Bind(OnEndFrame);
        }

        private static void OnBeginFrame()
        {
            TickTime = Time;
            UpdateCounters();
            Coroutine.ProcessCoroutines(CoroutineGroup.BeginFrame);
            Invoker.ProcessInvokers(CoroutineGroup.BeginFrame);
        }

        private static void OnEndFrame()
        {
            TickTime = Time;
            UpdateCounters();
            Coroutine.ProcessCoroutines(CoroutineGroup.EndFrame);
            Invoker.ProcessInvokers(CoroutineGroup.EndFrame);
        }

        private static bool Tick(float deltaTime)
        {
            TickTime = Time;
            TimeByDeltaTime += deltaTime;
            UpdateCounters();
            Coroutine.ProcessCoroutines(CoroutineGroup.Tick);
            Invoker.ProcessInvokers(CoroutineGroup.Tick);
            return true;
        }

        private static void UpdateCounters()
        {
            LastTickCounter = TickCounter;
            ulong tickCounter = FGlobals.FrameCounter;
            ulong tickCounterDifference = tickCounter - lastTickCounter;
            Debug.Assert(tickCounterDifference >= 0, "GFrameCounter decreased");
            TickCounter = lastTickCounter = tickCounter;
            
            LastFrameNumber = FrameNumber;
            uint frameNumber = FGlobals.FrameNumber;
            uint frameNumberDifference = frameNumber - lastFrameNumber;
            Debug.Assert(frameNumberDifference >= 0, "GFrameNumber decreased");
            FrameNumber = lastFrameNumber = frameNumber;

            LastFrameNumberRenderThread = FrameNumberRenderThread;
            uint frameNumberRenderThread = FGlobals.FrameNumberRenderThread;
            uint frameNumberRenderThreadDifference = frameNumberRenderThread - lastFrameNumberRenderThread;
            Debug.Assert(frameNumberDifference >= 0, "GFrameNumberRenderThread decreased");
            FrameNumberRenderThread = lastFrameNumberRenderThread = frameNumberRenderThread;

            WorldTime.WorldAddress = FGlobals.GWorld;
            if (!WorldTime.IsPaused)
            {
                WorldTickCounter += tickCounterDifference;
                WorldFrameNumber += frameNumberDifference;
            }
        }

        internal static void OnUnload()
        {
            EngineLoopHotReloadData hotReloadData = HotReload.Data.Create<EngineLoopHotReloadData>();
            hotReloadData.ReloadDateTime = DateTime.Now;
            hotReloadData.Time = Time;
            hotReloadData.TimeByDeltaTime = TimeByDeltaTime;
        }

        internal static void OnReload()
        {
            EngineLoopHotReloadData hotReloadData = HotReload.Data.Get<EngineLoopHotReloadData>();
            if (hotReloadData != null)
            {
                startTime = hotReloadData.Time + (DateTime.Now - hotReloadData.ReloadDateTime);
                TimeByDeltaTime = hotReloadData.TimeByDeltaTime;
            }
        }

        class EngineLoopHotReloadData : HotReload.DataItem
        {
            public DateTime ReloadDateTime;
            public TimeSpan Time;
            public double TimeByDeltaTime;

            public override void Load()
            {
                ReloadDateTime = ReadDateTime();
                Time = ReadTimeSpan();
                TimeByDeltaTime = ReadDouble();
            }

            public override void Save()
            {
                WriteDateTime(ReloadDateTime);
                WriteTimeSpan(Time);
                WriteDouble(TimeByDeltaTime);
            }
        }
    }
}