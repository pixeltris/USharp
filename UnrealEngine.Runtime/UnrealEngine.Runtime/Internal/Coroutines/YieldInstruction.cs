using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public abstract class YieldInstruction : BinaryHeapItem
    {
        // Pool data
        public bool IsPooled
        {
            get { return poolId > 0; }
        }
        internal uint poolId;
        internal bool keepAlive;
        internal IYieldInstructionPool pool;

        internal IComparableYieldInstructionCollection comparableCollection;
        internal bool IsInsideComparableCollection
        {
            get { return comparableCollection != null; }
        }

        internal virtual bool IsComparable
        {
            get { return false; }
        }

        internal virtual bool AddToComparableCollection()
        {
            return false;
        }

        internal virtual void RemoveFromComparableCollection()
        {
        }

        private Coroutine owner;
        public Coroutine Owner
        {
            get { return owner; }
            set
            {
                bool changed = owner != value;

                owner = value;

                if (changed && value != null)
                {
                    OnOwnerSet();
                }
            }
        }

        public abstract bool KeepWaiting { get; }

        internal bool running;

        internal void Begin()
        {
            running = true;
            OnBegin();
            if (IsComparable && Owner.CurrentInstruction == this && KeepWaiting && AddToComparableCollection())
            {
                Coroutine.ComparableBegin(Owner);
            }
        }

        internal void End()
        {
            OnEnd();
            if (IsInsideComparableCollection)
            {
                RemoveFromComparableCollection();
                Coroutine.ComparableEnd(Owner);
            }
            running = false;
        }        

        public virtual void OnBegin()
        {
        }

        public virtual void OnEnd()
        {
        }

        public virtual void OnOwnerSet()
        {
        }
    }

    public static class YieldInstructionExtensions
    {
        public static T KeepAlive<T>(this T instruction) where T : YieldInstruction
        {
            if (!instruction.IsPooled)
            {
                throw new InvalidOperationException("Cannot call KeepAlive on a non pooled instruction");
            }
            instruction.keepAlive = true;
            return instruction;
        }
    }
}
