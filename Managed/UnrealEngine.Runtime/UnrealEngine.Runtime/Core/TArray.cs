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
    [DebuggerTypeProxy(typeof(TArrayDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public unsafe class TArrayBase<T> : IEnumerable<T>
    {
        protected readonly UObject Owner;
        protected MarshalingDelegates<T>.FromNative FromNative;
        protected MarshalingDelegates<T>.ToNative ToNative;

        protected readonly UFieldAddress property;
        readonly FScriptArray* array;

        public int Count
        {
            get
            {
                CheckOwner();
                return array->Count;
            }
        }

        /// <summary>
        /// Address of the TArray/FScriptArray
        /// </summary>
        protected IntPtr Address
        {
            get { return (IntPtr)array; }
        }

        /// <summary>
        /// Address of the data inside the TArray/FScriptArray
        /// </summary>
        protected IntPtr Data
        {
            get { return array->Data; }
        }

        protected FScriptArrayHelper ArrayHelper;

        public TArrayBase(UObject owner, UFieldAddress arrayProperty, IntPtr address,
            MarshalingDelegates<T>.FromNative fromNative, MarshalingDelegates<T>.ToNative toNative)
        {
            property = arrayProperty;
            array = (FScriptArray*)address;

            ArrayHelper = new FScriptArrayHelper(property.Address, address);

            Owner = owner;
            FromNative = fromNative;
            ToNative = toNative;
        }

        protected void CheckOwner()
        {
            if (Owner != null && Owner.IsDestroyed)
            {
                throw new UObjectDestroyedException("Trying to access a TArray which points to memory of a destroyed UObject (" + property.PathName + ")");
            }
        }

        protected void ClearInternal()
        {
            CheckOwner();
            ArrayHelper.Update(property);
            ArrayHelper.EmptyValues();
        }

        protected void AddInternal()
        {
            CheckOwner();
            ArrayHelper.Update(property);
            ArrayHelper.AddValue();
        }

        protected void InsertInternal(int index)
        {
            CheckOwner();
            ArrayHelper.Update(property);
            ArrayHelper.InsertValues(index);
        }

        protected void RemoveAtInternal(int index)
        {
            CheckOwner();
            ArrayHelper.Update(property);
            ArrayHelper.RemoveValues(index);
        }

        public T Get(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException(string.Format("Index {0} out of bounds. Array is size {1}", index, Count));
            }
            ArrayHelper.Update(property);
            return FromNative(Data, index, ArrayHelper.InnerPropertyAddress);
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public int IndexOf(T item)
        {
            ArrayHelper.Update(property);

            int count = Count;
            if ((object)item == null)
            {
                // Can a TArray contain a nullptr?
                for (int i = 0; i < count; ++i)
                {
                    if (FromNative(Data, i, ArrayHelper.InnerPropertyAddress) == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
            else
            {
                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                for (int i = 0; i < count; ++i)
                {
                    if (comparer.Equals(FromNative(Data, i, ArrayHelper.InnerPropertyAddress), item))
                    {
                        return i;
                    }
                }
                return -1;
            }
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
            private TArrayBase<T> array;

            public Enumerator(TArrayBase<T> array)
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
                return index < array.Count;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }

    public class TArrayReadOnly<T> : TArrayBase<T>, IReadOnlyList<T>
    {
        public TArrayReadOnly(UObject owner, UFieldAddress arrayProperty, IntPtr address, MarshalingDelegates<T>.FromNative fromNative)
            : base(owner, arrayProperty, address, fromNative, null)
        {
        }

        public T this[int index]
        {
            get { return Get(index); }
        }
    }

    public class TArrayReadWrite<T> : TArrayBase<T>, IList<T>
    {
        public TArrayReadWrite(UObject owner, UFieldAddress arrayProperty, IntPtr address,
            MarshalingDelegates<T>.FromNative fromNative, MarshalingDelegates<T>.ToNative toNative)
            : base(owner, arrayProperty, address, fromNative, toNative)
        {
        }

        public T this[int index]
        {
            get { return Get(index); }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new IndexOutOfRangeException(string.Format("Index {0} out of bounds. Array is size {1}", index, Count));
                }
                ToNative(Data, index, ArrayHelper.InnerPropertyAddress, value);
            }
        }

        public void SetValues(IList<T> values)
        {
            CheckOwner();
            ArrayHelper.Update(property);
            ArrayHelper.EmptyAndAddZeroedValues(values.Count);
            for (int i = 0; i < values.Count; ++i)
            {
                ToNative(Data, i, ArrayHelper.InnerPropertyAddress, values[i]);
            }
        }

        public void Add(T item)
        {
            int newIndex = Count;
            AddInternal();
            this[newIndex] = item;
        }

        public void Clear()
        {
            ClearInternal();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            // TODO: probably a faster way to do this
            int numElements = Count;
            for (int i = 0; i < numElements; ++i)
            {
                array[i + arrayIndex] = this[i];
            }
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
                return true;
            }
            return false;
        }

        public void Insert(int index, T item)
        {
            InsertInternal(index);
            this[index] = item;
        }

        public void RemoveAt(int index)
        {
            RemoveAtInternal(index);
        }
    }

    // Used for members only
    public class TArrayReadWriteMarshaler<T>
    {
        UFieldAddress property;
        FScriptArrayHelper helper;
        TArrayReadWrite<T>[] wrappers;
        MarshalingDelegates<T>.FromNative innerFromNative;
        MarshalingDelegates<T>.ToNative innerToNative;

        public TArrayReadWriteMarshaler(int length, UFieldAddress arrayProperty,
            MarshalingDelegates<T>.FromNative fromNative, MarshalingDelegates<T>.ToNative toNative)
        {
            property = arrayProperty;
            helper = new FScriptArrayHelper(property.Address);
            wrappers = new TArrayReadWrite<T>[length];
            innerFromNative = fromNative;
            innerToNative = toNative;
        }

        public TArrayReadWrite<T> FromNative(IntPtr nativeBuffer)
        {
            return FromNative(nativeBuffer, 0, IntPtr.Zero);
        }

        public TArrayReadWrite<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            if (wrappers[arrayIndex] == null)
            {
                wrappers[arrayIndex] = new TArrayReadWrite<T>(null, property, nativeBuffer +
                    (arrayIndex * Marshal.SizeOf(typeof(FScriptArray))), innerFromNative, innerToNative);
            }
            return wrappers[arrayIndex];
        }

        public void ToNative(IntPtr nativeBuffer, IList<T> value)
        {
            helper.Update(property);
            ToNativeInternal(nativeBuffer, 0, value, ref helper, innerToNative);
        }

        public void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, IList<T> value)
        {
            helper.Update(property);
            ToNativeInternal(nativeBuffer, arrayIndex, value, ref helper, innerToNative);
        }

        internal static void ToNativeInternal(IntPtr nativeBuffer, int arrayIndex, IList<T> value,
            ref FScriptArrayHelper helper, MarshalingDelegates<T>.ToNative innerToNative)
        {
            IntPtr scriptArrayAddress = nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptArray)));
            helper.Array = scriptArrayAddress;

            // Make sure any existing elements are properly destroyed
            helper.EmptyAndAddZeroedValues(value == null ? 0 : value.Count);

            if (value == null)
            {
                return;
            }

            unsafe
            {
                FScriptArray* array = (FScriptArray*)scriptArrayAddress;
                for (int i = 0; i < value.Count; ++i)
                {
                    innerToNative(array->Data, i, helper.InnerPropertyAddress, value[i]);
                }
            }
        }
    }

    // Used for members only where they are exposed as readonly
    public class TArrayReadOnlyMarshaler<T>
    {
        UFieldAddress property;
        TArrayReadOnly<T>[] wrappers;
        MarshalingDelegates<T>.FromNative innerFromNative;

        public TArrayReadOnlyMarshaler(int length, UFieldAddress arrayProperty,
            MarshalingDelegates<T>.FromNative fromNative, MarshalingDelegates<T>.ToNative toNative)
        {
            property = arrayProperty;
            wrappers = new TArrayReadOnly<T>[length];
            innerFromNative = fromNative;
        }

        public TArrayReadOnly<T> FromNative(IntPtr nativeBuffer)
        {
            return FromNative(nativeBuffer, 0, IntPtr.Zero);
        }

        public TArrayReadOnly<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            if (wrappers[arrayIndex] == null)
            {
                wrappers[arrayIndex] = new TArrayReadOnly<T>(null, property, nativeBuffer +
                    (arrayIndex * Marshal.SizeOf(typeof(FScriptArray))), innerFromNative);
            }
            return wrappers[arrayIndex];
        }

        public void ToNative(IntPtr nativeBuffer, IReadOnlyList<T> value)
        {
            ToNative(nativeBuffer, 0, IntPtr.Zero, value);
        }

        public void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, IReadOnlyList<T> value)
        {
            throw new NotImplementedException("Read-only TArray cannot write to native memory.");
        }
    }

    /// <summary>
    /// Array marshaler used for function parameters / return results to copy to/from native memory
    /// </summary>
    public struct TArrayCopyMarshaler<T>
    {
        UFieldAddress property;
        FScriptArrayHelper helper;
        MarshalingDelegates<T>.FromNative innerFromNative;
        MarshalingDelegates<T>.ToNative innerToNative;

        public TArrayCopyMarshaler(int length, UFieldAddress arrayProperty,
            MarshalingDelegates<T>.FromNative fromNative, MarshalingDelegates<T>.ToNative toNative)
        {
            property = arrayProperty;
            helper = new FScriptArrayHelper(property.Address);
            innerFromNative = fromNative;
            innerToNative = toNative;
        }

        public List<T> FromNative(IntPtr nativeBuffer)
        {
            return FromNative(nativeBuffer, 0, IntPtr.Zero);
        }

        public List<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            IntPtr scriptArrayAddress = nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptArray)));

            helper.Update(property);
            helper.Array = scriptArrayAddress;

            unsafe
            {
                FScriptArray* array = (FScriptArray*)scriptArrayAddress;
                List<T> result = new List<T>(array->ArrayNum);
                for (int i = 0; i < array->ArrayNum; ++i)
                {
                    result.Add(innerFromNative(array->Data, i, helper.InnerPropertyAddress));
                }
                return result;
            }
        }

        public void ToNative(IntPtr nativeBuffer, IList<T> value)
        {
            helper.Update(property);
            TArrayReadWriteMarshaler<T>.ToNativeInternal(nativeBuffer, 0, value, ref helper, innerToNative);
        }

        public void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, IList<T> value)
        {
            helper.Update(property);
            TArrayReadWriteMarshaler<T>.ToNativeInternal(nativeBuffer, arrayIndex, value, ref helper, innerToNative);
        }
    }

    /// <summary>
    /// Used for UObject.DynamicInvoke
    /// </summary>
    public static class TArrayStaticCopyMarshaler<T>
    {
        static MarshalingDelegates<T>.FromNative innerFromNative = MarshalingDelegateResolver<T>.FromNative;
        static MarshalingDelegates<T>.ToNative innerToNative = MarshalingDelegateResolver<T>.ToNative;

        public static IList<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            IntPtr scriptArrayAddress = nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptArray)));
            FScriptArrayHelper helper = new FScriptArrayHelper(prop, scriptArrayAddress);

            unsafe
            {
                FScriptArray* array = (FScriptArray*)scriptArrayAddress;
                List<T> result = new List<T>(array->ArrayNum);
                for (int i = 0; i < array->ArrayNum; ++i)
                {
                    result.Add(innerFromNative(array->Data, i, helper.InnerPropertyAddress));
                }
                return result;            
            }
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, IList<T> value)
        {
            FScriptArrayHelper helper = new FScriptArrayHelper(prop);
            TArrayReadWriteMarshaler<T>.ToNativeInternal(nativeBuffer, arrayIndex, value, ref helper, innerToNative);
        }
    }
}
