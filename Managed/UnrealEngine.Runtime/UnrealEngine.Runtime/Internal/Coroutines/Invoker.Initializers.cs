using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public partial class Invoker
    {
        public const bool PoolByDefault = true;

        private static Invoker StartInvoker(object obj, InvokerHandlerType handlerType, Delegate handler, InvokerType type,
            ulong value, ulong repeatValue, CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            Invoker invoker = pool ? InvokerPool.GetObject() : new Invoker();
            invoker.SetHandler(handlerType, handler);
            switch (type)
            {
                case InvokerType.Delay:
                    TimeSpan time = TimeSpan.FromTicks((long)value);
                    TimeSpan repeatTime = TimeSpan.FromTicks((long)repeatValue);
                    if (repeatTime != default(TimeSpan))
                    {
                        invoker.SetTime(time, repeatTime);
                    }
                    else
                    {
                        invoker.SetTime(time);
                    }
                    break;

                case InvokerType.Ticks:
                    if (repeatValue != default(ulong))
                    {
                        invoker.SetTicks(value, repeatValue);
                    }
                    else
                    {
                        invoker.SetTicks(value);
                    }
                    break;

                case InvokerType.Frames:
                    if (repeatValue != default(ulong))
                    {
                        invoker.SetFrames(value, repeatValue);
                    }
                    else
                    {
                        invoker.SetFrames(value);
                    }
                    break;
            }
            invoker.SetGroup(group);
            invoker.Start();
            return invoker;
        }

        public static Invoker StartInvoker(object obj, InvokerHandler handler, TimeSpan time, TimeSpan repeatedTime = default(TimeSpan),
            CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            return StartInvoker(obj, InvokerHandlerType.Default, handler, InvokerType.Delay, (ulong)time.Ticks, (ulong)repeatedTime.Ticks, group, pool);
        }

        public static Invoker StartInvoker(object obj, InvokerHandlerWithInvoker handler, TimeSpan time, TimeSpan repeatedTime = default(TimeSpan),
            CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            return StartInvoker(obj, InvokerHandlerType.WithInvoker, handler, InvokerType.Delay, (ulong)time.Ticks, (ulong)repeatedTime.Ticks, group, pool);
        }

        public static Invoker StartInvoker(object obj, InvokerHandlerWithObject handler, TimeSpan time, TimeSpan repeatedTime = default(TimeSpan),
            CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            return StartInvoker(obj, InvokerHandlerType.WithObject, handler, InvokerType.Delay, (ulong)time.Ticks, (ulong)repeatedTime.Ticks, group, pool);
        }

        public static Invoker StartInvoker(object obj, InvokerHandlerWithObjectInvoker handler, TimeSpan time, TimeSpan repeatedTime = default(TimeSpan),
            CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            return StartInvoker(obj, InvokerHandlerType.WithObjectInvoker, handler, InvokerType.Delay, (ulong)time.Ticks, (ulong)repeatedTime.Ticks, group, pool);
        }

        public static Invoker StartInvokerTicks(object obj, InvokerHandler handler, ulong ticks, ulong repeatedTicks = default(ulong),
            CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            return StartInvoker(obj, InvokerHandlerType.Default, handler, InvokerType.Ticks, ticks, repeatedTicks, group, pool);
        }

        public static Invoker StartInvokerTicks(object obj, InvokerHandlerWithInvoker handler, ulong ticks, ulong repeatedTicks = default(ulong),
            CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            return StartInvoker(obj, InvokerHandlerType.WithInvoker, handler, InvokerType.Ticks, ticks, repeatedTicks, group, pool);
        }

        public static Invoker StartInvokerTicks(object obj, InvokerHandlerWithObject handler, ulong ticks, ulong repeatedTicks = default(ulong),
            CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            return StartInvoker(obj, InvokerHandlerType.WithObject, handler, InvokerType.Ticks, ticks, repeatedTicks, group, pool);
        }

        public static Invoker StartInvokerTicks(object obj, InvokerHandlerWithObjectInvoker handler, ulong ticks, ulong repeatedTicks = default(ulong),
            CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            return StartInvoker(obj, InvokerHandlerType.WithObjectInvoker, handler, InvokerType.Ticks, ticks, repeatedTicks, group, pool);
        }

        public static Invoker StartInvokerFrames(object obj, InvokerHandler handler, ulong frames, ulong repeatedFrames = default(ulong),
            CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            return StartInvoker(obj, InvokerHandlerType.Default, handler, InvokerType.Frames, frames, repeatedFrames, group, pool);
        }

        public static Invoker StartInvokerFrames(object obj, InvokerHandlerWithInvoker handler, ulong frames, ulong repeatedFrames = default(ulong),
            CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            return StartInvoker(obj, InvokerHandlerType.WithInvoker, handler, InvokerType.Frames, frames, repeatedFrames, group, pool);
        }

        public static Invoker StartInvokerFrames(object obj, InvokerHandlerWithObject handler, ulong frames, ulong repeatedFrames = default(ulong),
            CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            return StartInvoker(obj, InvokerHandlerType.WithObject, handler, InvokerType.Frames, frames, repeatedFrames, group, pool);
        }

        public static Invoker StartInvokerFrames(object obj, InvokerHandlerWithObjectInvoker handler, ulong frames, ulong repeatedFrames = default(ulong),
            CoroutineGroup group = CoroutineGroup.Tick, bool pool = PoolByDefault)
        {
            return StartInvoker(obj, InvokerHandlerType.WithObjectInvoker, handler, InvokerType.Frames, frames, repeatedFrames, group, pool);
        }

        public static void StopInvoker(Invoker invoker)
        {
            invoker.Stop();
        }

        public static void StopAllInvokers(UObject owner)
        {
            List<Invoker> invokers;
            if (owner != null && invokersByUObject.TryGetValue(owner, out invokers))
            {
                for (int i = invokers.Count - 1; i >= 0; --i)
                {
                    Invoker invoker = invokers[i];
                    invoker.Stop();
                }
                if (invokers.Count == 0)
                {
                    invokersByUObject.Remove(owner);
                }
            }
        }

        // TODO: Also allow stopping invokers by handler? Would need an additional collection invokersByHandler

        /// <summary>
        /// This stops the invokers which have the underlying method (not the actual handler itself)
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="method"></param>
        public static void StopInvokerByMethod(UObject owner, Delegate method)
        {
            List<Invoker> invokers;
            if (method.Method != null && invokersByMethod.TryGetValue(method.Method, out invokers))
            {
                for (int i = invokers.Count; i >= 0; --i)
                {
                    Invoker invoker = invokers[i];
                    if ((invoker.Owner as UObject) == owner)
                    {
                        invoker.Stop();
                    }
                }
            }
        }

        public static void StopInvokerByMethod(Delegate method)
        {
            List<Invoker> invokers;
            if (method.Method != null && invokersByMethod.TryGetValue(method.Method, out invokers))
            {
                for (int i = invokers.Count; i >= 0; --i)
                {
                    invokers[i].Stop();
                }
            }
        }

        public static List<Invoker> FindInvokers(UObject owner)
        {
            List<Invoker> result = new List<Invoker>();
            List<Invoker> invokers;
            if (owner != null && invokersByUObject.TryGetValue(owner, out invokers))
            {
                result.AddRange(invokers);
            }
            return result;
        }

        public static List<Invoker> FindInvokers(UObject owner, string tag)
        {
            List<Invoker> result = new List<Invoker>();
            List<Invoker> invokers;
            if (invokersByTag.TryGetValue(tag, out invokers))
            {
                foreach (Invoker invoker in invokers)
                {
                    if (owner == (invoker.Owner as UObject))
                    {
                        result.Add(invoker);
                    }
                }
            }
            return result;
        }

        public static List<Invoker> FindInvokers(UObject owner, int tagId)
        {
            List<Invoker> result = new List<Invoker>();
            List<Invoker> invokers;
            if (invokersByTagId.TryGetValue(tagId, out invokers))
            {
                foreach (Invoker invoker in invokers)
                {
                    if (owner == (invoker.Owner as UObject))
                    {
                        result.Add(invoker);
                    }
                }
            }
            return result;
        }
    }
}
