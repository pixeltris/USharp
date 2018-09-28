using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    internal interface IComparableYieldInstructionCollection
    {
        void Process(CoroutineGroup group);
        void OnGroupChanged(YieldInstruction instruction, CoroutineGroup oldGroup, CoroutineGroup newGroup);
    }

    internal class ComparableYieldInstructions<T> : IComparableYieldInstructionCollection where T : YieldInstruction, IComparable<T>
    {
        private static ComparableYieldInstructions<T> instance = new ComparableYieldInstructions<T>();

        private UnrealBinaryHeapEx<T> tick = new UnrealBinaryHeapEx<T>();
        private UnrealBinaryHeapEx<T> beginFrame = new UnrealBinaryHeapEx<T>();
        private UnrealBinaryHeapEx<T> endFrame = new UnrealBinaryHeapEx<T>();

        public ComparableYieldInstructions()
        {
            Coroutine.comparableCollections.Add(this);
        }        

        public void Process(CoroutineGroup group)
        {
            UnrealBinaryHeapEx<T> collection = GetCollection(group);
            while (collection.Count > 0)
            {
                T instruction = collection.HeapTop();
                if (instruction.KeepWaiting)
                {
                    return;
                }
                collection.HeapPopDiscard();

                // Return the control over to the main coroutines collection            
                instruction.comparableCollection = null;
                Coroutine.ComparableEnd(instruction.Owner);
            }
        }

        public void OnGroupChanged(YieldInstruction instruction, CoroutineGroup oldGroup, CoroutineGroup newGroup)
        {
            OnGroupChanged((T)instruction, oldGroup, newGroup);
        }

        private void OnGroupChanged(T instruction, CoroutineGroup oldGroup, CoroutineGroup newGroup)
        {
            UnrealBinaryHeapEx<T> oldCollection = GetCollection(oldGroup);
            UnrealBinaryHeapEx<T> newCollection = GetCollection(newGroup);

            if (oldCollection != null && newCollection != null && oldCollection != newCollection)
            {
                oldCollection.HeapRemove(instruction);
                newCollection.HeapPush(instruction);
            }
        }

        private bool AddInternal(T value)
        {
            UnrealBinaryHeapEx<T> collection = GetCollection(value.Owner.Group);
            collection.HeapPush(value);
            value.comparableCollection = this;
            return true;
        }

        private void RemoveInternal(T value)
        {
            UnrealBinaryHeapEx<T> collection = GetCollection(value.Owner.Group);
            collection.HeapRemove(value);
            value.comparableCollection = null;
        }

        private void ValueChangedInternal(T value)
        {
            UnrealBinaryHeapEx<T> collection = GetCollection(value.Owner.Group);
            collection.HeapRemove(value);
            collection.HeapPush(value);
        }

        public static bool Add(T value)
        {
            return instance.AddInternal(value);
        }

        public static void Remove(T value)
        {
            instance.RemoveInternal(value);
        }

        public static void ValueChanged(T value)
        {
            instance.ValueChangedInternal(value);
        }

        private UnrealBinaryHeapEx<T> GetCollection(CoroutineGroup group)
        {
            switch (group)
            {
                case CoroutineGroup.Tick:
                    return tick;
                case CoroutineGroup.BeginFrame:
                    return beginFrame;
                case CoroutineGroup.EndFrame:
                    return endFrame;
                default: return null;
            }
        }
    }

    public abstract class ComparableYieldInstruction<T> : YieldInstruction, IComparable<ComparableYieldInstruction<T>> where T : ComparableYieldInstruction<T>
    {
        internal override bool IsComparable
        {
            get
            {
                return true;
            }
        }

        protected void ComparableValueChanged()
        {
            if (IsInsideComparableCollection)
            {
                ComparableYieldInstructions<ComparableYieldInstruction<T>>.ValueChanged(this);
            }
        }

        internal override bool AddToComparableCollection()
        {
            return ComparableYieldInstructions<ComparableYieldInstruction<T>>.Add(this);
        }

        internal override void RemoveFromComparableCollection()
        {
            ComparableYieldInstructions<ComparableYieldInstruction<T>>.Remove(this);
        }

        public int CompareTo(ComparableYieldInstruction<T> other)
        {
            // Can we avoid doing this cast?
            return CompareTo((T)other);
        }
        
        // Could do this to avoid cast... kind of ugly though.
        // I imagine there is a correct way of doing this to avoid this casting problem
        //public abstract T GetThis();

        public abstract int CompareTo(T other);
    }
}
