using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [DebuggerTypeProxy(typeof(TFixedSizeArrayDebugView<>))]
    [DebuggerDisplay("Length = {Length}")]
    public abstract class TFixedSizeArrayBase<T> : IEnumerable<T>
    {
        private readonly IntPtr address;
        private readonly UFieldAddress property;
        private readonly UObject owner;
        private static readonly MarshalingDelegates<T>.FromNative fromNative = MarshalingDelegateResolver<T>.FromNative;
        private static readonly MarshalingDelegates<T>.ToNative toNative = MarshalingDelegateResolver<T>.ToNative;

        public int Length
        {
            get { return property.ArrayDim; }
        }

        public TFixedSizeArrayBase(IntPtr address, UFieldAddress property, UObject owner)
        {
            this.address = address;
            this.property = property;
            this.owner = owner;
        }

        private void CheckOwner()
        {
            if (owner != null && owner.IsDestroyed)
            {
                throw new UObjectDestroyedException("Trying to access a TFixedSizeArray which points to memory of a destroyed UObject (" + property.PathName + ")");
            }
        }

        protected T Get(int index)
        {
            CheckOwner();
            if (index < 0 || index >= Length)
            {
                throw new IndexOutOfRangeException(string.Format("Index {0} out of bounds. Array is size {1}.", index, Length));
            }
            return fromNative(address, index, property.Address);
        }

        protected void Set(int index, ref T value)
        {
            CheckOwner();
            if (index < 0 || index >= Length)
            {
                throw new IndexOutOfRangeException(string.Format("Index {0} out of bounds. Array is size {1}.", index, Length));
            }
            toNative(address, index, property.Address, value);
        }

        public T[] ToArray()
        {
            T[] result = new T[Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = fromNative(address, i, property.Address);
            }
            return result;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<T>
        {
            private int index;
            private TFixedSizeArrayBase<T> array;

            public Enumerator(TFixedSizeArrayBase<T> array)
            {
                this.array = array;
                index = -1;
            }

            public T Current
            {
                get { return array.Get(index); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                ++index;
                return index < array.Length;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }
    
    public sealed class TFixedSizeArray<T> : TFixedSizeArrayBase<T>
    {
        public TFixedSizeArray(IntPtr address, UFieldAddress property, UObject owner) 
            : base(address, property, owner)
        {
        }

        public T this[int index]
        {
            get { return Get(index); }
            set { Set(index, ref value); }
        }

        public void SetValues(T[] values)
        {
            if (values == null)
            {
                return;
            }

            int length = Math.Min(values.Length, Length);
            for (int i = 0; i < length; i++)
            {
                Set(i, ref values[i]);
            }
        }
    }
    
    public sealed class TFixedSizeArrayReadOnly<T> : TFixedSizeArrayBase<T>
    {
        public TFixedSizeArrayReadOnly(IntPtr address, UFieldAddress property, UObject owner) 
            : base(address, property, owner)
        {
        }

        public T this[int index]
        {
            get { return Get(index); }
        }
    }
}
