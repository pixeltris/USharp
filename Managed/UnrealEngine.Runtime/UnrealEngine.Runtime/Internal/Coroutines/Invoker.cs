using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Engine;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public partial class Invoker : BinaryHeapItem, IComparable<Invoker>
    {
        // We are basically just providing our own timing manager. See unreals:
        // See FTimerManager::Tick Engine\Source\Runtime\Engine\Private\TimerManager.cpp

        private static InvokerCollectionGroup tickInvokers = new InvokerCollectionGroup(CoroutineGroup.Tick);
        private static InvokerCollectionGroup beginFrameInvokers = new InvokerCollectionGroup(CoroutineGroup.BeginFrame);
        private static InvokerCollectionGroup endFrameInvokers = new InvokerCollectionGroup(CoroutineGroup.EndFrame);

        private static Dictionary<string, List<Invoker>> invokersByTag = new Dictionary<string, List<Invoker>>();
        private static Dictionary<int, List<Invoker>> invokersByTagId = new Dictionary<int, List<Invoker>>();
        
        private static Dictionary<UObject, List<Invoker>> invokersByUObject = new Dictionary<UObject, List<Invoker>>();
        private static Dictionary<MethodInfo, List<Invoker>> invokersByMethod = new Dictionary<MethodInfo, List<Invoker>>();

        public event InvokerEventHandler OnStopped;

        /// <summary>
        /// A pooled invoker will be reused once it has stopped
        /// </summary>
        public bool IsPooled { get; internal set; }

        internal IntPtr OwnerWorld;
        public object Owner { get; internal set; }
        public bool IsRepeated { get; private set; }
        public InvokerType Type { get; private set; }
        public CoroutineGroup Group { get; private set; }

        internal ulong Value { get; private set; }
        internal ulong RepeatedValue { get; private set; }

        public bool IsFirstRun { get; private set; }
        public bool Running { get; private set; }

        internal ulong CurrentTargetValue
        {
            get { return IsFirstRun ? Value : RepeatedValue; }
        }

        public bool HasStopAfterValue { get; private set; }
        private ulong stopAfterValue;
        private ulong stopAfterEndValue;

        // The value when Start() was called
        private ulong startingValue;

        // Current cycle starting value
        private ulong beginValue;
        internal ulong BeginValue
        {
            get { return beginValue; }
            private set
            {
                beginValue = value;
                EndValue = beginValue + CurrentTargetValue;
            }
        }

        // Current cycle end value
        internal ulong EndValue { get; private set; }

        // If true repeat time is constant relative to the previous call time rather than relative to the time overlap.
        // Example: Invoke function takes 3000 MS to complete. RepeatedValue is 10000 MS. The caller wants exactly 10000 MS
        // to elapse BETWEEN calls. That will be 13000 total MS if this is value is true. If false then 10000 total MS.
        // - This value doesn't take into account call count per cycle. E.g. If the engine hangs in other code for 50000 MS
        //   the function will be called 5 times. Therefore this value is constant time between calls rather than between cycles.
        //   - It is likely that you want to set MaxCallCount to 1 if using this value to limit cycle calls to 1.
        // TODO: Possibly add another member which can take into account cycle time (RepeatConstantTimeIncludingCycleTime?); If more
        //       than 1 cycle then calculate how long each cycle takes and stop calling if the total time is greater then the expected
        //       total time between each call.
        public bool RepeatConstantTime { get; set; }        

        // How many times invoke is going to be called within the current cycle.
        public int CallCount { get; private set; }

        // Maximum number of times this invoker should be called in a single cycle
        public int MaxCallCount { get; set; }

        // Maximum number of times this invoker should be called in total
        public uint MaxTotalCallCount { get; set; }

        // Total number of times this invoker has been called
        private ulong totalCallCount;

        // Used to cancel futher calls during this cycle
        private bool cancelInvoke;

        // The collection group this invoker is added to
        private InvokerCollectionGroup collectionGroup;

        // The collection this invoker is added to
        private UnrealBinaryHeapEx<Invoker> collection;

        private int invokersByUObjectIndex = -1;
        private int invokersByMethodIndex = -1;

        private int tagIdIndex = -1;
        private int tagId;
        public int TagId
        {
            get { return tagId; }
            set
            {
                if (tagId != value)
                {
                    RemoveIdTagFromCollection();
                    tagId = value;
                    AddIdTagToCollection();
                }
            }
        }

        private int tagIndex = -1;
        private string tag;
        public string Tag
        {
            get { return tag; }
            set
            {
                if (tag != value)
                {
                    RemoveStringTagFromCollection();
                    tag = value;
                    AddStringTagToCollection();
                }
            }
        }

        private InvokerHandlerType handlerType;
        private InvokerHandler handler;
        private InvokerHandlerWithObject handlerWithObject;
        private InvokerHandlerWithInvoker handlerWithInvoker;
        private InvokerHandlerWithObjectInvoker handlerWithObjectInvoker;

        internal void SetHandler(InvokerHandlerType handlerType, Delegate handler)
        {
            switch (handlerType)
            {
                case InvokerHandlerType.Default: SetHandler((InvokerHandler)handler); break;
                case InvokerHandlerType.WithInvoker: SetHandler((InvokerHandlerWithInvoker)handler); break;
                case InvokerHandlerType.WithObject: SetHandler((InvokerHandlerWithObject)handler); break;
                case InvokerHandlerType.WithObjectInvoker: SetHandler((InvokerHandlerWithObjectInvoker)handler); break;
            }
        }

        public void SetHandler(InvokerHandler handler)
        {
            SetHandler(InvokerHandlerType.Default, handler, null, null, null);
        }

        public void SetHandler(InvokerHandlerWithInvoker handler)
        {
            SetHandler(InvokerHandlerType.WithInvoker, null, handler, null, null);
        }

        public void SetHandler(InvokerHandlerWithObject handler)
        {
            SetHandler(InvokerHandlerType.WithObject, null, null, handler, null);
        }        

        public void SetHandler(InvokerHandlerWithObjectInvoker handler)
        {
            SetHandler(InvokerHandlerType.WithInvoker, null, null, null, handler);
        }

        private void SetHandler(InvokerHandlerType handlerType,
            InvokerHandler handler,            
            InvokerHandlerWithInvoker handlerWithInvoker,
            InvokerHandlerWithObject handlerWithObject,
            InvokerHandlerWithObjectInvoker handlerWithObjectInvoker)
        {
            RemoveInvokerFromInvokersByMethod();

            this.handlerType = handlerType;
            this.handler = handler;
            this.handlerWithInvoker = handlerWithInvoker;
            this.handlerWithObject = handlerWithObject;
            this.handlerWithObjectInvoker = handlerWithObjectInvoker;

            AddInvokerToInvokersByMethod();
        }

        private Delegate GetHandler()
        {
            switch (handlerType)
            {
                case InvokerHandlerType.Default: return handler;
                case InvokerHandlerType.WithInvoker: return handlerWithInvoker;
                case InvokerHandlerType.WithObject: return handlerWithObject;
                case InvokerHandlerType.WithObjectInvoker: return handlerWithObjectInvoker;
                default: return null;
            }
        }

        public Invoker SetTime(TimeSpan value)
        {
            InvokerType oldType = Type;
            Type = InvokerType.Delay;
            Value = (ulong)value.Ticks;
            UpdateValues(oldType != Type);
            return this;
        }

        public Invoker SetTime(TimeSpan value, TimeSpan repeatedValue)
        {
            InvokerType oldType = Type;
            Type = InvokerType.Delay;
            Value = (ulong)value.Ticks;
            RepeatedValue = (ulong)repeatedValue.Ticks;
            IsRepeated = true;
            UpdateValues(oldType != Type);
            return this;
        }

        public Invoker SetFrames(ulong value)
        {
            InvokerType oldType = Type;
            Type = InvokerType.Frames;
            Value = value;
            UpdateValues(oldType != Type);
            return this;
        }

        public Invoker SetFrames(ulong value, ulong repeatedValue)
        {
            InvokerType oldType = Type;
            Type = InvokerType.Frames;
            Value = value;
            RepeatedValue = repeatedValue;
            IsRepeated = true;
            UpdateValues(oldType != Type);
            return this;
        }

        public Invoker SetTicks(ulong value)
        {
            InvokerType oldType = Type;
            Type = InvokerType.Ticks;
            Value = value;
            UpdateValues(Type != oldType);
            return this;
        }

        public Invoker SetTicks(ulong value, ulong repeatedValue)
        {
            InvokerType oldType = Type;
            Type = InvokerType.Ticks;
            Value = value;
            RepeatedValue = repeatedValue;
            IsRepeated = true;
            UpdateValues(oldType != Type);
            return this;
        }

        public Invoker ClearRepeatedValue()
        {
            IsRepeated = false;
            RepeatedValue = 0;
            UpdateValues(false);
            return this;
        }

        public Invoker SetGroup(CoroutineGroup group)
        {
            Group = group;
            UpdateValues(false);
            return this;
        }

        public TimeSpan GetTime()
        {
            return TimeSpan.FromTicks((long)Value);
        }

        public TimeSpan GetRepeatedTime()
        {
            return TimeSpan.FromTicks((long)RepeatedValue);
        }

        public ulong GetTicks()
        {
            return Value;
        }

        public ulong GetRepeatedTicks()
        {
            return RepeatedValue;
        }

        public ulong GetFrames()
        {
            return Value;
        }

        public ulong GetRepeatedFrames()
        {
            return Value;
        }

        public Invoker SetStopAfterTime(TimeSpan time)
        {
            return SetStopAfterValue((ulong)time.Ticks);
        }

        public Invoker SetStopAfterTicks(ulong ticks)
        {
            return SetStopAfterValue(ticks);
        }

        public Invoker SetStopAfterFrames(ulong frames)
        {
            return SetStopAfterValue(frames);
        }

        private Invoker SetStopAfterValue(ulong value)
        {
            stopAfterValue = value;
            stopAfterEndValue = startingValue + stopAfterValue;
            HasStopAfterValue = true;
            return this;
        }

        public TimeSpan GetStopAfterTime()
        {
            return TimeSpan.FromTicks((long)stopAfterValue);
        }

        public ulong GetStopAfterTicks()
        {
            return Value;
        }

        public ulong GetStopAfterFrames()
        {
            return Value;
        }

        public void ClearStopAfterValue()
        {
            HasStopAfterValue = false;
            stopAfterValue = 0;
        }

        public int CompareTo(Invoker other)
        {
            if (Type != other.Type)
            {
                throw new InvalidOperationException("Unexpected comparison of two invokers with different invoker types");
            }
            return EndValue.CompareTo(other.EndValue);
        }

        private void UpdateValues(bool setStartValue)
        {
            if (!Running)
            {
                return;
            }

            if (setStartValue)
            {
                switch (Type)
                {
                    case InvokerType.Delay:
                        BeginValue = startingValue = (ulong)WorldTimeHelper.GetTimeChecked(OwnerWorld).Ticks;
                        break;

                    case InvokerType.Ticks:
                        BeginValue = startingValue = EngineLoop.WorldTickCounter;
                        break;

                    case InvokerType.Frames:
                        BeginValue = startingValue = EngineLoop.WorldFrameNumber;
                        break;
                }
            }

            InvokerCollectionGroup newCollectionGroup = GetInvokerGroup(Group);
            UnrealBinaryHeapEx<Invoker> newCollection = newCollectionGroup == null ? null : newCollectionGroup.GetCollection(Type);

            if (collectionGroup != null && collection != null)
            {
                if (collectionGroup.Group != Group || newCollection != collection)
                {
                    collection.HeapRemove(this);
                    collection = null;
                    collectionGroup = null;
                }
            }

            if (newCollection != null)
            {
                collectionGroup = newCollectionGroup;
                collection = newCollection;
                newCollection.HeapPush(this);
            }
        }

        public void Start()
        {
            if (Running)
            {
                return;
            }            

            IsFirstRun = true;
            Running = true;
            UpdateValues(true);
            UpdateTags();

            AddInvokerToInvokersByUObject();
            AddInvokerToInvokersByMethod();
        }

        public void Stop()
        {
            if (!Running)
            {
                return;
            }

            if (collection != null)
            {
                collection.HeapRemove(this);
                collection = null;
                collectionGroup = null;
            }
            IsFirstRun = false;
            Running = false;
            UpdateTags();

            RemoveInvokerFromInvokersByUObject();
            RemoveInvokerFromInvokersByMethod();

            if (OnStopped != null)
            {
                OnStopped(this);
            }

            if (IsPooled)
            {
                InvokerPool.ReturnObject(this);
            }
        }

        internal void Reset()
        {
            // There is a lot going on here. We probably don't need to reset everything.

            OnStopped = null;

            Running = false;
            IsFirstRun = false;
            Tag = null;
            TagId = 0;

            OwnerWorld = IntPtr.Zero;
            Owner = null;
            IsRepeated = false;
            Type = default(InvokerType);
            Group = default(CoroutineGroup);

            Value = 0;
            RepeatedValue = 0;

            HasStopAfterValue = false;
            stopAfterValue = 0;
            stopAfterEndValue = 0;

            startingValue = 0;
            beginValue = 0;
            EndValue = 0;

            RepeatConstantTime = false;
            MaxCallCount = 0;
            MaxTotalCallCount = 0;
            totalCallCount = 0;
            cancelInvoke = false;

            if (collection != null)
            {
                collection.HeapRemove(this);
                collection = null;
            }
            collectionGroup = null;

            handlerType = InvokerHandlerType.Default;
            handler = null;
            handlerWithObject = null;
            handlerWithInvoker = null;
            handlerWithObjectInvoker = null;

            invokersByUObjectIndex = -1;
            invokersByMethodIndex = -1;
        }

        public void CancelInvoke()
        {
            cancelInvoke = true;
        }

        public bool Invoke()
        {
            if (!Running || (MaxTotalCallCount > 0 && totalCallCount >= MaxTotalCallCount))
            {
                return false;
            }
            Invoke();
            return true;
        }

        private bool InvokeInternal()
        {
            switch (handlerType)
            {
                case InvokerHandlerType.Default: handler(); break;
                case InvokerHandlerType.WithInvoker: handlerWithInvoker(this); break;
                case InvokerHandlerType.WithObject: handlerWithObject(Owner); break;
                case InvokerHandlerType.WithObjectInvoker: handlerWithObjectInvoker(Owner, this); break;
            }
            IsFirstRun = false;
            totalCallCount++;
            if (cancelInvoke || (MaxTotalCallCount > 0 && totalCallCount >= MaxTotalCallCount))
            {
                Stop();
                return false;
            }
            return true;
        }

        internal void Process(ulong value)
        {
            cancelInvoke = false;

            int expectedAdditionalCallCount = 0;
            if (IsRepeated)
            {
                expectedAdditionalCallCount = (int)((value - EndValue) / RepeatedValue);
            }
            CallCount = expectedAdditionalCallCount + 1;

            // Clamp the call count to MaxCallCount
            int clampedAdditionalCallCount = expectedAdditionalCallCount;
            if (MaxCallCount > 0)
            {
                clampedAdditionalCallCount = Math.Min(MaxCallCount - 1, clampedAdditionalCallCount);
            }

            // target value may change if IsFirstRun is true
            ulong oldTargetValue = CurrentTargetValue;

            if (!InvokeInternal() || (HasStopAfterValue && HasStopAfterValueCompleted(oldTargetValue, 0)))
            {
                return;
            }

            uint additionalCalls = 0;
            for (; additionalCalls < clampedAdditionalCallCount && !cancelInvoke && Running; ++additionalCalls)
            {
                if (!InvokeInternal() || (HasStopAfterValue && HasStopAfterValueCompleted(oldTargetValue, additionalCalls + 1)))
                {
                    return;
                }
            }

            if (IsRepeated)
            {
                if (RepeatConstantTime && Type == InvokerType.Delay)
                {
                    ulong realTime = (ulong)WorldTimeHelper.GetTimeChecked(OwnerWorld).Ticks;
                    BeginValue = value + (realTime - EndValue);
                }
                else
                {
                    BeginValue += oldTargetValue + (additionalCalls * RepeatedValue);
                }

                collection.HeapPush(this);
            }
            else
            {
                collection = null;
                collectionGroup = null;
                Stop();
            }
        }

        private bool HasStopAfterValueCompleted(ulong oldTargetValue, uint callIndex)
        {
            ulong beginValue = BeginValue + oldTargetValue + (callIndex * RepeatedValue);
            ulong endValue = BeginValue + CurrentTargetValue;
            return stopAfterEndValue <= endValue;
        }

        internal static void ProcessInvokers(CoroutineGroup group)
        {
            GetInvokerGroup(group).Process();
        }

        private static InvokerCollectionGroup GetInvokerGroup(CoroutineGroup group)
        {
            switch (group)
            {
                case CoroutineGroup.Tick: return tickInvokers;
                case CoroutineGroup.BeginFrame: return beginFrameInvokers;
                case CoroutineGroup.EndFrame: return endFrameInvokers;
            }
            return null;
        }

        private void AddInvokerToInvokersByUObject()
        {
            UObject obj = Owner as UObject;
            if (obj != null)
            {
                List<Invoker> invokers;
                if (!invokersByUObject.TryGetValue(obj, out invokers))
                {
                    invokersByUObject.Add(obj, invokers = new List<Invoker>());
                }
                invokersByUObjectIndex = invokers.Count;
                invokers.Add(this);
            }
        }

        private void RemoveInvokerFromInvokersByUObject()
        {
            if (invokersByUObjectIndex >= 0)
            {
                UObject obj = Owner as UObject;
                if (obj != null)
                {
                    List<Invoker> invokers;
                    if (invokersByUObject.TryGetValue(obj, out invokers) && invokers.Count > 0)
                    {
                        Invoker lastInvoker = invokers[invokers.Count - 1];
                        invokers.RemoveAtSwapEx(ref invokersByUObjectIndex, ref lastInvoker.invokersByUObjectIndex);
                        if (invokers.Count == 0)
                        {
                            invokersByUObject.Remove(obj);
                        }
                    }
                }
                invokersByUObjectIndex = -1;
            }
        }

        private void AddInvokerToInvokersByMethod()
        {
            Delegate handler = GetHandler();
            if (Running && handler != null && handler.Method != null)
            {
                List<Invoker> invokers;
                if (!invokersByMethod.TryGetValue(handler.Method, out invokers))
                {
                    invokersByMethod.Add(handler.Method, invokers = new List<Invoker>());
                }
                invokersByMethodIndex = invokers.Count;
                invokers.Add(this);
            }
        }

        private void RemoveInvokerFromInvokersByMethod()
        {
            if (invokersByMethodIndex >= 0)
            {
                Delegate handler = GetHandler();
                if (handler != null && handler.Method != null)
                {
                    List<Invoker> invokers;
                    if (invokersByMethod.TryGetValue(handler.Method, out invokers) && invokers.Count > 0)
                    {
                        Invoker lastInvoker = invokers[invokers.Count - 1];
                        invokers.RemoveAtSwapEx(ref invokersByMethodIndex, ref lastInvoker.invokersByMethodIndex);
                    }
                }
                invokersByMethodIndex = -1;
            }
        }

        private void AddIdTagToCollection()
        {
            if (Running && tagId != 0 && tagIdIndex < 0)
            {
                List<Invoker> invokers;
                if (!invokersByTagId.TryGetValue(tagId, out invokers))
                {
                    invokersByTagId.Add(tagId, invokers = new List<Invoker>());
                }
                tagIdIndex = invokers.Count;
                invokers.Add(this);
            }
        }

        private void RemoveIdTagFromCollection()
        {
            if (tagIdIndex >= 0)
            {
                List<Invoker> invokers;
                if (invokersByTagId.TryGetValue(tagId, out invokers))
                {
                    Invoker endInvoker = invokers[invokers.Count - 1];
                    invokers.RemoveAtSwapEx(ref tagIdIndex, ref endInvoker.tagIdIndex);
                }
            }
        }

        private void AddStringTagToCollection()
        {
            if (Running && tag != null && tagIdIndex < 0)
            {
                List<Invoker> invokers;
                if (!invokersByTag.TryGetValue(tag, out invokers))
                {
                    invokersByTag.Add(tag, invokers = new List<Invoker>());
                }
                tagIndex = invokers.Count;
                invokers.Add(this);
            }
        }

        private void RemoveStringTagFromCollection()
        {
            if (tagIndex >= 0)
            {
                List<Invoker> invokers;
                if (invokersByTag.TryGetValue(tag, out invokers))
                {
                    Invoker endInvoker = invokers[invokers.Count - 1];
                    invokers.RemoveAtSwapEx(ref tagIndex, ref endInvoker.tagIndex);
                }
            }
        }        

        private void UpdateTags()
        {
            if (Running)
            {
                AddIdTagToCollection();
                AddStringTagToCollection();
            }
            else
            {
                RemoveIdTagFromCollection();
                RemoveStringTagFromCollection();
            }
        }

        internal static void OnNativeFunctionsRegistered()
        {
            FWorldDelegates.OnPostWorldCleanup.Bind(OnPostWorldCleanup);
        }

        private static void OnPostWorldCleanup(IntPtr world, bool sessionEnded, bool cleanupResources)
        {
            List<UObject> objectsToRemove = new List<UObject>();
            foreach (KeyValuePair<UObject, List<Invoker>> obj in invokersByUObject)
            {
                if (obj.Key.Address != IntPtr.Zero)
                {
                    IntPtr objWorld = Native_UObject.GetWorld(obj.Key.Address);
                    if (objWorld == world)
                    {
                        objectsToRemove.Add(obj.Key);
                    }
                }
                else
                {
                    objectsToRemove.Add(obj.Key);
                }
            }
            foreach (UObject obj in objectsToRemove)
            {
                StopAllInvokers(obj, true);
            }
        }

        internal static void RemoveObjectByGC(UObject owner)
        {
            StopAllInvokers(owner, true);
        }
    }    

    class InvokerCollectionGroup
    {
        private UnrealBinaryHeapEx<Invoker> delayInvokers = new UnrealBinaryHeapEx<Invoker>();
        private UnrealBinaryHeapEx<Invoker> ticksInvokers = new UnrealBinaryHeapEx<Invoker>();
        private UnrealBinaryHeapEx<Invoker> framesInvokers = new UnrealBinaryHeapEx<Invoker>();

        public CoroutineGroup Group { get; private set; }

        public InvokerCollectionGroup(CoroutineGroup group)
        {
            Group = group;
        }

        public UnrealBinaryHeapEx<Invoker> GetCollection(InvokerType type)
        {
            switch (type)
            {
                case InvokerType.Delay: return delayInvokers;
                case InvokerType.Ticks: return ticksInvokers;
                case InvokerType.Frames: return framesInvokers;
            }
            return null;
        }

        public void Process()
        {
            if (delayInvokers.Count > 0)
            {
                // Poll time for each invoker to ensure as much as possible is processed in the current tick
                ProcessTime(delayInvokers);

                //Process(delayInvokers, (ulong)EngineLoop.InternalTime.Ticks);
            }

            if (ticksInvokers.Count > 0)
            {
                Process(ticksInvokers, EngineLoop.WorldTickCounter);
            }

            if (framesInvokers.Count > 0)
            {
                Process(framesInvokers, EngineLoop.WorldFrameNumber);
            }
        }

        private void Process(UnrealBinaryHeapEx<Invoker> invokers, ulong value)
        {
            while (invokers.Count > 0)
            {
                Invoker invoker = invokers.HeapTop();
                if (invoker.EndValue <= value)
                {
                    invokers.HeapPopDiscard();
                    invoker.Process(value);
                }
                else
                {
                    break;
                }
            }
        }

        private void ProcessTime(UnrealBinaryHeapEx<Invoker> invokers)
        {
            // Note: If the the delay is low enough this may loop forever (the timer would need a repeat
            // value lower than the time it takes to get from the end of Process() back to the EndValue check
            // - If this becomes a problem you could stop this from happening by holding onto the last invoker
            //   processed and checking invoker!=lastInvoker
            // - Though if this is a problem it will make the game crawl anyway due to CallCount. A frame will
            //   occasionally get through between the invoker spam if doing the lastInvoker check.

            //Invoker lastInvoker = null;

            while (invokers.Count > 0)
            {
                Invoker invoker = invokers.HeapTop();
                ulong value = (ulong)WorldTimeHelper.GetTimeChecked(invoker.OwnerWorld).Ticks;
                if (invoker.EndValue <= value /*&& invoker != lastInvoker*/)
                {
                    invokers.HeapPopDiscard();
                    invoker.Process(value);
                    //lastInvoker = invoker;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public enum InvokerHandlerType
    {
        Default,
        WithInvoker,
        WithObject,
        WithObjectInvoker
    }

    public delegate void InvokerHandler();
    public delegate void InvokerHandlerWithObject(object obj);
    public delegate void InvokerHandlerWithInvoker(Invoker invoker);
    public delegate void InvokerHandlerWithObjectInvoker(object obj, Invoker invoker);

    public delegate void InvokerEventHandler(Invoker invoker);

    public enum InvokerType
    {
        Delay,
        Ticks,
        Frames,
    }
}
