using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Ticker class. Fires delegates after a delay.<para/>
    /// 
    /// Note: Do not try to add the same delegate instance twice, as there is no way to remove only a single instance (see member RemoveTicker).
    /// </summary>
    public class FTicker
    {
        private FTickerDelegate del;
        private FDelegateHandle handle;

        // Hold onto OnTick as a delegate to avoid it from being GCed
        private Native_FTicker.Del_RegisterTicker callback;

        private csbool OnTick(float deltaTime)
        {
            return del == null ? false : del(deltaTime);
        }

        public FTicker()
        {
            callback = OnTick;
        }

        private static Dictionary<FTicker, MethodInfo> handlers = new Dictionary<FTicker, MethodInfo>();
        private static Dictionary<MethodInfo, FTicker> handlersReverse = new Dictionary<MethodInfo, FTicker>();

        /// <summary>
        /// Add a new ticker with a given delay / interval
        /// </summary>
        /// <param name="del">Delegate to fire after the delay</param>
        /// <param name="delay">Delay until next fire; 0 means "next frame"</param>
        public static void AddTicker(FTickerDelegate del, float delay = 0)
        {            
            if (del == null || del.Method == null)
            {
                return;
            }

            if (!FThreading.IsInGameThread())
            {
                FThreading.RunOnGameThread(delegate { AddTicker(del, delay); });
                return;
            }

            FTicker existingTicker;
            if (handlersReverse.TryGetValue(del.Method, out existingTicker))
            {
                RemoveTicker(del);
            }

            FTicker ticker = new FTicker();
            ticker.del = del;
            Native_FTicker.Reg_CoreTicker(ticker.callback, ref ticker.handle, true, delay);

            handlers[ticker] = del.Method;
            handlersReverse[del.Method] = ticker;
        }

        /// <summary>
        /// Removes a previously added ticker delegate.<para/>
        /// 
        /// Note: will remove ALL tickers that use this handle, as there's no way to uniquely identify which one you are trying to remove.
        /// </summary>
        /// <param name="ticker">The ticker to remove.</param>
        public static void RemoveTicker(FTickerDelegate del)
        {
            if (del == null || del.Method == null)
            {
                return;
            }

            if (!FThreading.IsInGameThread())
            {
                FThreading.RunOnGameThread(delegate { RemoveTicker(del); });
                return;
            }

            FTicker ticker;
            if (handlersReverse.TryGetValue(del.Method, out ticker))
            {
                handlers.Remove(ticker);
                handlersReverse.Remove(del.Method);

                Native_FTicker.Reg_CoreTicker(ticker.callback, ref ticker.handle, false, 0);
            }
        }

        /// <summary>
        /// Fire all tickers who have passed their delay and reschedule the ones that return true
        /// 
        /// Note: This reschedule has timer skew, meaning we always wait a full Delay period after 
        /// each invocation even if we missed the last interval by a bit. For instance, setting a 
        /// delay of 0.1 seconds will not necessarily fire the delegate 10 times per second, depending 
        /// on the Tick() rate. What this DOES guarantee is that a delegate will never be called MORE
        /// FREQUENTLY than the Delay interval, regardless of how far we miss the scheduled interval.
        /// </summary>
        /// <param name="deltaTime">time that has passed since the last tick call</param>
        public static void Tick(float deltaTime)
        {
            if (!FThreading.IsInGameThread())
            {
                FThreading.RunOnGameThread(delegate { Tick(deltaTime); });
                return;
            }

            Native_FTicker.Tick(deltaTime);
        }

        public static void OnUnload()
        {
            foreach (KeyValuePair<FTicker, MethodInfo> handler in new Dictionary<FTicker, MethodInfo>(handlers))
            {
                Native_FTicker.Reg_CoreTicker(handler.Key.callback, ref handler.Key.handle, false, 0);
            }
            handlers.Clear();
            handlersReverse.Clear();
        }
    }

    public delegate bool FTickerDelegate(float deltaTime);
}
