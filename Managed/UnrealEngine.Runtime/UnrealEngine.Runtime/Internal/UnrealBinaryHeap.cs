using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Emulates unreals TArray binary heap functions
    /// 
    /// Rename this to PriorityQueue and update functions to be Enqueue/Dequeue/Peek?
    /// If heavily used this could benefit from [MethodImpl(MethodImplOptions.AggressiveInlining)] in various places.
    /// </summary>
    internal class UnrealBinaryHeap<T> where T : IComparable<T>
    {
        private List<T> items;

        public int Count
        {
            get { return items.Count; }
        }

        public UnrealBinaryHeap()
        {
            items = new List<T>();
        }

        public UnrealBinaryHeap(int capacity)
        {
            items = new List<T>(capacity);
        }

        public List<T> ToList()
        {
            return new List<T>(items);
        }

        /// <summary>
        /// Returns the raw backing list for this binary heap
        /// </summary>
        public List<T> GetList()
        {
            return items;
        }

        /// <summary>
        /// Adds a new element to the heap.
        /// </summary>
        /// <param name="item">Item to be added.</param>
        /// <returns>The index of the new element.</returns>
        public int HeapPush(T item)
        {
            // Add at the end, then sift up
            items.Add(item);
            return SiftUp(0, Count - 1);
        }

        /// <summary>
        /// Removes the top element from the heap.
        /// </summary>
        /// <returns>The popped item.</returns>
        public T HeapPop()
        {
            T result = items[0];
            RemoveAtSwap(0);
            SiftDown(0, Count);
            return result;
        }

        /// <summary>
        /// Removes the top element from the heap.
        /// </summary>
        public void HeapPopDiscard()
        {
            RemoveAtSwap(0);
            SiftDown(0, Count);
        }

        /// <summary>
        /// Returns the top element from the heap (does not remove the element).
        /// </summary>
        /// <returns>The reference to the top element from the heap.</returns>
        public T HeapTop()
        {
            return items[0];
        }

        /// <summary>
        /// Removes an element from the heap.
        /// </summary>
        /// <param name="index">Position at which to remove item.</param>
        public void HeapRemoveAt(int index)
        {
            RemoveAtSwap(index);

            SiftDown(index, Count);
            SiftUp(0, Math.Min(index, Count - 1));
        }

        /// <summary>
        /// Verifies the heap.
        /// </summary>
        public bool VerifyHeap()
        {
            for (int i = 1; i < Count; i++)
            {
                int parentIndex = HeapGetParentIndex(i);
                if (Predicate(i, parentIndex))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Builds an implicit heap from the array. Assumes &lt; operator is defined
        /// for the template type.
        /// </summary>
        private void Heapify()
        {
            for (int i = HeapGetParentIndex(Count - 1); i >= 0; i--)
            {
                SiftDown(i, Count);
            }
        }

        /// <summary>
        /// Gets the index of the left child of node at Index.
        /// </summary>
        /// <param name="index">Node for which the left child index is to be returned.</param>
        /// <returns>Index of the left child.</returns>
        private int HeapGetLeftChildIndex(int index)
        {
            return index * 2 + 1;
        }

        /// <summary>
        /// Checks if node located at Index is a leaf or not.
        /// </summary>
        /// <param name="index">Node index.</param>
        /// <param name="count"></param>
        /// <returns>true if node is a leaf, false otherwise.</returns>
        private bool HeapIsLeaf(int index, int count)
        {
            return HeapGetLeftChildIndex(index) >= count;
        }

        /// <summary>
        /// Gets the parent index for node at Index.
        /// </summary>
        /// <param name="index">node index.</param>
        /// <returns>Parent index.</returns>
        private int HeapGetParentIndex(int index)
        {
            return (index - 1) / 2;
        }

        /// <summary>
        /// Removes an element (or elements) at given location
        /// </summary>
        /// <param name="index">Location in array of the element to remove.</param>
        private void RemoveAtSwap(int index)
        {
            items[index] = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
        }

        /// <summary>
        /// Performs heap sort on the array.
        /// </summary>
        private void HeapSort()
        {
            Heapify();

            for (int i = Count - 1; i > 0; i--)
            {
                Exchange(0, i);
                SiftDown(0, i);
            }
        }

        /// <summary>
        /// Fixes a possible violation of order property between node at Index and a child.
        /// </summary>
        /// <param name="index">Node index.</param>
        /// <param name="count">Size of the heap (to avoid using Num()).</param>
        private void SiftDown(int index, int count)
        {
            while (!HeapIsLeaf(index, count))
            {
                int leftChildIndex = HeapGetLeftChildIndex(index);
                int rightChildIndex = leftChildIndex + 1;

                int minChildIndex = leftChildIndex;
                if (rightChildIndex < count)
                {
                    minChildIndex = Predicate(leftChildIndex, rightChildIndex) ? leftChildIndex : rightChildIndex;
                }

                if (!Predicate(minChildIndex, index))
                {
                    break;
                }

                Exchange(index, minChildIndex);
                index = minChildIndex;
            }
        }

        /// <summary>
        /// Fixes a possible violation of order property between node at NodeIndex and a parent.
        /// </summary>
        /// <param name="rootIndex">How far to go up?</param>
        /// <param name="nodeIndex">Node index.</param>
        /// <returns>The new index of the node that was at NodeIndex</returns>
        private int SiftUp(int rootIndex, int nodeIndex)
        {
            while (nodeIndex > rootIndex)
            {
                int parentIndex = HeapGetParentIndex(nodeIndex);
                if (!Predicate(nodeIndex, parentIndex))
                {
                    break;
                }

                Exchange(nodeIndex, parentIndex);
                nodeIndex = parentIndex;
            }

            return nodeIndex;
        }

        // Exchange/Swap both same thing
        private void Exchange(int indexA, int indexB)
        {
            T temp = items[indexA];
            items[indexA] = items[indexB];
            items[indexB] = temp;
        }

        private bool Predicate(int indexA, int indexB)
        {
            return items[indexA].CompareTo(items[indexB]) < 0;
        }
    }
}
