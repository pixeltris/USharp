using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // This class has similar functionality to FTimerManager and can be used as a replacement for it.
    // See FTimerManager for an alternative to Coroutines (FTimerManager currently only supports UFunction methods)
    // FTimerManager is better in the following respects:
    // - We don't quite support the same features as FTimerManager with pausing. If you pause a coroutine and resume
    //   the time until expire isn't preserved. (IsPaused for Coroutine is mostly used for WaitForever). We could
    //   potentially add support for pausing in the WaitFor instruction?
    // - There are also time dilation changing issues with Coroutine (see notes in the WaitFor instruction)
    // - FTimerManager functions will stay alive / valid on HotReload due to working with UFunction.

    /// <summary>
    /// Unity-like coroutines.
    /// </summary>
    public partial class Coroutine
    {
        // Object that owns this coroutine (UObject, other instance object, or null)
        public object Owner { get; internal set; }

        public System.Collections.IEnumerator Enumerator { get; internal set; }

        private bool complete = false;
        public bool Complete
        {
            get { return complete; }
            set
            {
                if (complete != value)
                {
                    complete = value;
                    if (complete && OnComplete != null)
                    {
                        OnComplete(this);
                    }
                }
            }
        }

        // OnReset is called when this object is returned to a coroutine pool
        public event CoroutineEventHandler OnReset;
        public event CoroutineEventHandler OnComplete;
        public event CoroutineEventHandler OnStopped;

        // Used for pooled instructions
        private Dictionary<uint, YieldInstruction> keepAliveInstructions = null;

        // The user may want to inject instructions to a coroutine directly.
        private Queue<YieldInstruction> injectedInstructionQueue = null;

        public bool IsCurrentInstructionInjected { get; private set; }

        public YieldInstruction CurrentInstruction { get; private set; }        

        private CoroutineGroup group;
        /// <summary>
        /// The group for this coroutine
        /// </summary>
        public CoroutineGroup Group
        {
            get { return group; }
            set
            {
                if (group != value)
                {
                    CoroutineGroup oldGroup = group;
                    group = value;
                    if (CurrentInstruction != null && CurrentInstruction.IsInsideComparableCollection)
                    {
                        CurrentInstruction.comparableCollection.OnGroupChanged(CurrentInstruction, oldGroup, group);
                    }
                }
            }
        }
          
        /// <summary>
        /// The target group set by an instruction to temporarily override Group
        /// </summary>
        public CoroutineGroup TargetGroup { get; set; }

        /// <summary>
        /// The current group (uses TargetGroup if not "None", otherwise uses Group)
        /// </summary>
        public CoroutineGroup CurrentGroup
        {
            get { return TargetGroup != CoroutineGroup.None ? TargetGroup : Group; }
        }

        /// <summary>
        /// A tag which can be used to control multiple coroutines (currently pretty slow)
        /// </summary>        
        public string Tag
        {
            get { return tag; }
            set
            {
                if (mainCollectionIndex != -1 && tag != value)
                {
                    OnCoroutineTagChanged(this, tag, value);
                }
                tag = value;
            }
        }
        private string tag;

        // Index into the main "coroutines" collection
        internal int mainCollectionIndex = -1;

        // Index into the coroutinesByTag collection
        internal int tagsCollectionIndex = -1;

        // Index into the coroutinesByObject collection
        internal int objectsCollectionIndex = -1;

        public bool IsPaused { get; set; }

        /// <summary>
        /// A pooled coroutine will be reused once it has completed
        /// </summary>
        public bool IsPooled { get; internal set; }

        public Coroutine()
        {
        }

        public Coroutine(System.Collections.IEnumerator enumerator)
        {
            Enumerator = enumerator;
        }
        
        internal void Reset()
        {
            if (OnReset != null)
            {
                OnReset(this);
            }

            TargetGroup = CoroutineGroup.None;
            IsPaused = false;
            ReleaseInstructions();
            complete = false;
            mainCollectionIndex = -1;
            tagsCollectionIndex = -1;
            objectsCollectionIndex = -1;
            Owner = null;
            Tag = null;
            OnReset = null;
            OnComplete = null;
            OnStopped = null;
        }

        public void Stop()
        {
            if (!complete)
            {
                complete = true;
                if (OnStopped != null)
                {
                    OnStopped(this);
                }
            }

            ReleaseInstructions();
        }

        /// <summary>
        /// Returns pooled instructions which have keepAlive set to true
        /// </summary>
        private void ReleaseInstructions()
        {
            if (injectedInstructionQueue != null)
            {
                foreach (YieldInstruction instruction in injectedInstructionQueue)
                {
                    ReleaseInstruction(instruction);
                }
                injectedInstructionQueue.Clear();
            }

            if (CurrentInstruction != null)
            {
                ReleaseInstruction(CurrentInstruction);
            }
            CurrentInstruction = null;

            if (keepAliveInstructions != null)
            {
                foreach (KeyValuePair<uint, YieldInstruction> instruction in keepAliveInstructions)
                {
                    instruction.Value.pool.ReturnObject(instruction.Value);
                }
                keepAliveInstructions.Clear();
            }
        }

        private void ReleaseInstruction(YieldInstruction instruction)
        {            
            if (instruction.running)
            {
                // Make sure End is called as this instruction may be held in a comparable collection
                instruction.End();
            }

            if (instruction.IsPooled)
            {
                if (instruction.keepAlive)
                {
                    if (keepAliveInstructions == null)
                    {
                        keepAliveInstructions = new Dictionary<uint, YieldInstruction>();
                    }
                    if (!keepAliveInstructions.ContainsKey(instruction.poolId))
                    {
                        keepAliveInstructions.Add(instruction.poolId, instruction);
                    }
                }
                else
                {
                    instruction.pool.ReturnObject(instruction);
                }
            }
        }

        public void Process(CoroutineGroup group)
        {
            if (IsPaused)
            {
                return;
            }

            if (!ProcessCurrentInstruction(group))
            {
                return;
            }

            if (Enumerator == null)
            {
                FMessage.Log(ELogVerbosity.Error, "Coroutine enumerator is null");
                Stop();
                return;
            }

            try
            {
                while (Enumerator.MoveNext())
                {
                    object current = Enumerator.Current;
                    YieldInstruction instruction = current as YieldInstruction;
                    if (instruction != null)
                    {
                        CurrentInstruction = instruction;
                        CurrentInstruction.Owner = this;
                        CurrentInstruction.Begin();

                        if (!ProcessCurrentInstruction(group))
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                FMessage.Log(ELogVerbosity.Error, "Exception when running coroutine. " + Environment.NewLine + e.ToString());
                Stop();
                return;
            }

            Complete = true;
        }

        private bool ProcessCurrentInstruction(CoroutineGroup group)
        {
            if (CurrentInstruction != null)
            {
                if (!ProcessInstruction(CurrentInstruction, group))
                {
                    return false;
                }
                CurrentInstruction = null;
            }

            while (injectedInstructionQueue != null && injectedInstructionQueue.Count > 0)
            {
                CurrentInstruction = injectedInstructionQueue.Dequeue();
                CurrentInstruction.Begin();
                IsCurrentInstructionInjected = true;
                if (!ProcessInstruction(CurrentInstruction, group))
                {
                    return false;
                }
            }
            IsCurrentInstructionInjected = false;

            return true;
        }

        private bool ProcessInstruction(YieldInstruction instruction, CoroutineGroup group)
        {
            if (CurrentGroup != group || instruction.KeepWaiting)
            {
                return false;
            }
            instruction.End();
            ReleaseInstruction(instruction);
            return true;
        }        

        public void InjectInstruction(YieldInstruction instruction, YieldInstructionInjectType injectType = YieldInstructionInjectType.Queue)
        {
            instruction.Owner = this;

            if (instruction.running)
            {
                throw new InvalidOperationException("Cannot inject an already running instruction.");
            }

            if (CurrentInstruction != null)
            {
                if (injectType == YieldInstructionInjectType.RemoveCurrent)
                {
                    // ReleasePooledInstruction will call instruction.End() for us of already running
                    ReleaseInstruction(CurrentInstruction);
                    CurrentInstruction = instruction;
                    CurrentInstruction.Begin();
                }
                else if (injectType == YieldInstructionInjectType.SwapCurrent)
                {
                    if (CurrentInstruction.running)
                    {
                        CurrentInstruction.End();
                    }
                    CurrentInstruction = instruction;
                    CurrentInstruction.Begin();
                }
                else
                {
                    if (injectedInstructionQueue == null)
                    {
                        injectedInstructionQueue = new Queue<YieldInstruction>();
                    }
                    injectedInstructionQueue.Enqueue(instruction);
                }
            }
            else
            {
                CurrentInstruction = instruction;                
            }
        }
    }

    public delegate void CoroutineEventHandler(Coroutine coroutine);

    public enum CoroutineGroup
    {
        /// <summary>
        /// Called on every tick (after world/actor tick)
        /// </summary>
        Tick,

        /// <summary>
        /// Called at the start of each frame (before world/actor tick)
        /// </summary>
        BeginFrame,

        /// <summary>
        /// Called at the end of each frame (directly after CoroutineGroup.Tick)
        /// </summary>
        EndFrame,

        None
    }

    public enum YieldInstructionInjectType
    {
        Queue,
        RemoveCurrent,
        SwapCurrent
    }
}
