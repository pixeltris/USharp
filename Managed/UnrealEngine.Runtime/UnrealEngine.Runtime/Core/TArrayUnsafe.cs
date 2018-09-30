using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Be careful when using this array with structs which have a copyCtor/ctor/dtor.
    // Implement IDisposable where appropriate to ensure memory is properly cleaned up.

    [DebuggerTypeProxy(typeof(TArrayDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public unsafe class TArrayUnsafeRef<T> : TArrayUnsafe<T>
    {
        public TArrayUnsafeRef(IntPtr native)
            : base(native)
        {
            isRef = true;
        }
    }

    [DebuggerTypeProxy(typeof(TArrayDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public unsafe class TArrayUnsafe<T> : IList<T>, IDisposable
    {
        private IntPtr address;// manually allocated memory (will be null if this array points to existing address)
        private FScriptArray* nativeArray;
        private bool isUObject;
        private bool isString;
        private int numBytesPerElement;

        // ref points to existing memory which we shouldn't dispose / destroy
        protected bool isRef = false;

        internal IntPtr Address
        {
            get { return address; }
        }

        internal FScriptArray ScriptArray
        {
            get { return *nativeArray; }
        }

        public bool IsValid
        {
            get { return nativeArray != null; }
        }

        public TArrayUnsafe(IntPtr native)
        {
            nativeArray = (FScriptArray*)native;
            ValidateType();
        }

        public TArrayUnsafe()
        {
            address = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FScriptArray)));
            nativeArray = (FScriptArray*)address;
            nativeArray->Data = IntPtr.Zero;
            nativeArray->ArrayNum = 0;
            nativeArray->ArrayMax = 0;
            ValidateType();
        }

        public TArrayUnsafe(FScriptArray array)
            : this()
        {
            CopyFrom(array);
        }

        public void Dispose()
        {
            if (isRef)
            {
                return;
            }
                        
            if (nativeArray != null)
            {
                Clear();
                nativeArray->Destroy();
                nativeArray = null;
            }

            if (address != IntPtr.Zero)
            {                
                Marshal.FreeHGlobal(address);
                address = IntPtr.Zero;
            }
        }

        private void ValidateType()
        {
            if (typeof(T).IsSameOrSubclassOf(typeof(UObject)))
            {
                isUObject = true;
                numBytesPerElement = IntPtr.Size;
            }
            else if (typeof(T) == typeof(string))
            {
                isString = true;
                numBytesPerElement = Marshal.SizeOf(typeof(FScriptArray));
            }
            else if (typeof(T).IsValueType)
            {
                numBytesPerElement = Marshal.SizeOf(typeof(T));
            }
            else
            {
                throw new InvalidOperationException("TArray can only work with UnrealObject types and value types.");
            }
        }

        public void CopyFrom(FScriptArray array)
        {
            Clear();
            nativeArray->AddZeroed(numBytesPerElement, array.Count);
            for (int i = 0; i < array.Count; i++)
            {
                IntPtr address = IntPtr.Add(nativeArray->Data, numBytesPerElement * i);
                if (isUObject)
                {
                    Add((T)(object)GCHelper.Find(address));
                }
                else if (isString)
                {
                    Add((T)(object)FStringMarshaler.FromPtr(address));
                }
                else
                {
                    Add((T)Marshal.PtrToStructure(address, typeof(T)));
                }
            }
        }

        public int IndexOf(T item)
        {
            int count = Count;
            for (int i = 0; i < count; i++)
            {
                if (this[i].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            // Probably only really need zeroing for FString
            nativeArray->InsertZeroed(index, numBytesPerElement);
            this[index] = item;
        }

        public void RemoveAt(int index)
        {
            if (isString)
            {
                this[index] = (T)(object)null;
            }
            nativeArray->RemoveAt(index, numBytesPerElement);
        }

        public T this[int index]
        {
            get
            {
                if (!nativeArray->IsValidIndex(index))
                {
                    throw new IndexOutOfRangeException();
                }

                IntPtr address = IntPtr.Add(nativeArray->Data, numBytesPerElement * index);
                if (isUObject)
                {
                    return (T)(object)GCHelper.Find(Marshal.ReadIntPtr(address));
                }
                else if (isString)
                {
                    return (T)(object)FStringMarshaler.FromPtr(address);
                }
                else
                {
                    return (T)Marshal.PtrToStructure(address, typeof(T));
                }
            }
            set
            {
                if (!nativeArray->IsValidIndex(index))
                {
                    throw new IndexOutOfRangeException();
                }

                IntPtr address = IntPtr.Add(nativeArray->Data, numBytesPerElement * index);
                if (isUObject)
                {
                    if (value == null)
                    {
                        Marshal.WriteIntPtr(address, IntPtr.Zero);
                    }
                    else
                    {
                        UObject obj = (UObject)(object)value;
                        Marshal.WriteIntPtr(address, obj.Address);
                    }
                }
                else if (isString)
                {
                    unsafe
                    {
                        // Get the current value, clear the current value and write the new value
                        FScriptArray* current = (FScriptArray*)address;
                        current->Destroy();

                        string valueStr = value == null ? null : (string)(object)value;
                        if (!string.IsNullOrEmpty(valueStr))
                        {
                            FStringMarshaler.ToArray(address, valueStr);
                        }
                    }
                }
                else
                {
                    Marshal.StructureToPtr(value, address, false);
                }
            }
        }

        public void Add(T item)
        {
            // Probably only really need zeroing for FString
            int index = nativeArray->AddZeroed(numBytesPerElement);
            this[index] = item;
        }

        public void AddRange(T[] items)
        {
            if (items != null)
            {
                int index = nativeArray->AddZeroed(numBytesPerElement, items.Length);
                for (int i = 0; i < items.Length; i++)
                {
                    this[index + i] = items[i];
                }
            }
        }

        public void AddRange(List<T> items)
        {
            if (items != null)
            {
                int index = nativeArray->AddZeroed(numBytesPerElement, items.Count);
                for (int i = 0; i < items.Count; i++)
                {
                    this[index + i] = items[i];
                }
            }
        }

        public void Clear()
        {
            if (isString)
            {
                for (int i = 0; i < Count; i++)
                {
                    this[i] = (T)(object)null;
                }
            }
            else if (typeof(T).IsAssignableFrom(typeof(IDisposable)))
            {
                for (int i = 0; i < Count; i++)
                {
                    IDisposable value = this[i] as IDisposable;
                    value.Dispose();
                }
            }
            nativeArray->Empty(0, numBytesPerElement);
        }

        public bool Contains(T item)
        {
            if ((object)item == null)
            {
                foreach (T element in this)
                {
                    if ((object)element == null)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                foreach (T element in this)
                {
                    if (comparer.Equals(element, item))
                    {
                        return true;
                    }
                }
                return false;
            }            
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int count = Count;
            for (int i = 0; i < count; i++)
            {
                array[i + arrayIndex] = this[i];
            }
        }

        public int Count
        {
            get { return nativeArray->Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
            }
            return index != -1;
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

        public unsafe class Enumerator : IEnumerator<T>
        {
            private int index;
            private T current;
            private TArrayUnsafe<T> array;

            public Enumerator(TArrayUnsafe<T> array)
            {
                index = 0;
                this.array = array;
                current = default(T);
            }

            public void Dispose()
            {
            }

            public T Current
            {
                get { return current; }
            }

            object IEnumerator.Current
            {
                get { return current; }
            }

            public bool MoveNext()
            {
                if (array.nativeArray == null)
                {
                    return false;
                }

                if (index < array.nativeArray->ArrayNum)
                {
                    current = array[index];
                    index++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                index = 0;
                current = default(T);
            }
        }
    }
}
