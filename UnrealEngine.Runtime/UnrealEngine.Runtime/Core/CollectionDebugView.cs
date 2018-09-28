using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    class TFixedSizeArrayDebugView<T>
    {
        private TFixedSizeArrayBase<T> fixedSizeArray;

        public TFixedSizeArrayDebugView(TFixedSizeArrayBase<T> fixedSizeArray)
        {
            if(fixedSizeArray == null)
                throw new ArgumentNullException("fixedSizeArray");
            this.fixedSizeArray = fixedSizeArray;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get { return fixedSizeArray.ToArray(); }
        }
    }

    class TArrayDebugView<T>
    {
        private ICollection<T> collection;

        public TArrayDebugView(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            this.collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[this.collection.Count];
                this.collection.CopyTo(array, 0);
                return array;
            }
        }
    }

    class TSetDebugView<T>
    {
        private ICollection<T> collection;

        public TSetDebugView(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            this.collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[this.collection.Count];
                this.collection.CopyTo(array, 0);
                return array;
            }
        }
    }

    class TMapDebugView<TKey, TValue>
    {
        private IDictionary<TKey, TValue> collection;

        public TMapDebugView(IDictionary<TKey, TValue> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            this.collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue>[] Items
        {
            get
            {
                KeyValuePair<TKey, TValue>[] items = new KeyValuePair<TKey, TValue>[collection.Count];
                collection.CopyTo(items, 0);
                return items;
            }
        }
    }
}
