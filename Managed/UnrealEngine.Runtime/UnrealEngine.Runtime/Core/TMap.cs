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
    [DebuggerTypeProxy(typeof(TMapDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public unsafe class TMapBase<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        protected readonly UObject Owner;
        protected MarshalingDelegates<TKey>.FromNative KeyFromNative;
        protected MarshalingDelegates<TKey>.ToNative KeyToNative;
        protected MarshalingDelegates<TValue>.FromNative ValueFromNative;
        protected MarshalingDelegates<TValue>.ToNative ValueToNative;

        readonly UFieldAddress property;
        readonly FScriptMap* map;

        public int Count
        {
            get
            {
                CheckOwner();
                return map->Count;
            }
        }

        /// <summary>
        /// Address of the TMap/FScriptMap
        /// </summary>
        protected IntPtr Address
        {
            get { return (IntPtr)map; }
        }

        protected FScriptMapHelper MapHelper;

        public TMapBase(UObject owner, UFieldAddress mapProperty, IntPtr address,
            MarshalingDelegates<TKey>.FromNative keyFromNative, MarshalingDelegates<TKey>.ToNative keyToNative,
            MarshalingDelegates<TValue>.FromNative valueFromNative, MarshalingDelegates<TValue>.ToNative valueToNative)
        {
            property = mapProperty;
            map = (FScriptMap*)address;

            MapHelper = new FScriptMapHelper(property.Address, address);

            Owner = owner;
            KeyFromNative = keyFromNative;
            KeyToNative = keyToNative;
            ValueFromNative = valueFromNative;
            ValueToNative = valueToNative;

            ContainerHashValidator.Validate(Native_UMapProperty.Get_KeyProp(property.Address));
        }

        [Conditional("DEBUG")]
        protected void CheckOwner()
        {
            if (Owner != null && Owner.IsDestroyed)
            {
                throw new UObjectDestroyedException("Trying to access a TMap which points to memory of a destroyed UObject (" + property.PathName + ")");
            }
        }

        protected void ClearInternal()
        {
            CheckOwner();
            MapHelper.Update(property);
            MapHelper.EmptyValues();
        }

        protected void AddInternal(TKey key, TValue value)
        {
            CheckOwner();
            MapHelper.Update(property);
            MapHelper.AddPair(key, value, KeyToNative, ValueToNative);
        }

        protected bool RemoveInternal(TKey key)
        {
            CheckOwner();
            int index = IndexOf(key);
            if (index >= 0)
            {
                MapHelper.Update(property);
                MapHelper.RemoveAt(index);
                return true;
            }
            return false;
        }

        protected void RemoveAtInternal(int index)
        {   
            CheckOwner();
            if (MapHelper.IsValidIndex(index))
            {
                MapHelper.Update(property);
                MapHelper.RemoveAt(index);
            }
        }

        protected bool TryGetInternal(TKey key, out TValue value)
        {
            CheckOwner();
            int index = IndexOf(key);
            if (index >= 0)
            {
                value = GetAt(index).Value;
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        protected KeyValuePair<TKey, TValue> GetAt(int index)
        {
            if (!MapHelper.IsValidIndex(index))
            {
                throw new IndexOutOfRangeException(string.Format("Index {0} is invalid. Indicies aren't necessarily sequential.", index));
            }
            IntPtr keyPtr, valuePtr;
            MapHelper.Update(property);
            if (!MapHelper.GetPairPtr(index, out keyPtr, out valuePtr))
            {
                return default(KeyValuePair<TKey, TValue>);
            }
            return new KeyValuePair<TKey, TValue>(
                KeyFromNative(keyPtr, 0, MapHelper.KeyPropertyAddress),
                ValueFromNative(valuePtr, 0, MapHelper.ValuePropertyAddress));
        }

        public TValue Get(TKey key)
        {
            int index = IndexOf(key);
            if (index >= 0)
            {
                return GetAt(index).Value;
            }
            else
            {
                return default(TValue);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return IndexOf(key) >= 0;
        }

        public bool ContainsValue(TValue value)
        {
            EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;
            int maxIndex = MapHelper.GetMaxIndex();
            for (int i = 0; i < maxIndex; ++i)
            {
                if (MapHelper.IsValidIndex(i) && comparer.Equals(GetAt(i).Value, value))
                {
                    return true;
                }
            }
            return false;
        }
        
        protected int IndexOf(TKey key)
        {
            MapHelper.Update(property);
            return MapHelper.FindPairIndex(key, KeyToNative, Owner);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private int index;
            private TMapBase<TKey, TValue> map;

            public Enumerator(TMapBase<TKey, TValue> map)
            {
                this.map = map;
                index = -1;
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get { return map.GetAt(index); }
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
                int maxIndex = map.MapHelper.GetMaxIndex();
                while (++index < maxIndex && !map.MapHelper.IsValidIndex(index)) { }
                return index < maxIndex;
            }

            public void Reset()
            {
                index = -1;
            }
        }

        public struct KeyEnumerator : ICollection<TKey>
        {
            private TMapBase<TKey, TValue> map;

            public KeyEnumerator(TMapBase<TKey, TValue> map)
            {
                this.map = map;
            }

            public int Count
            {
                get { return map.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public void Add(TKey item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TKey item)
            {
                return map.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                int maxIndex = map.MapHelper.GetMaxIndex();
                int index = arrayIndex;
                for (int i = 0; i < maxIndex; ++i)
                {
                    if (map.MapHelper.IsValidIndex(i))
                    {
                        array[index++] = map.GetAt(i).Key;
                    }
                }
            }

            public bool Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(map);
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return new Enumerator(map);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(map);
            }

            public struct Enumerator : IEnumerator<TKey>
            {
                private int index;
                private TMapBase<TKey, TValue> map;

                public int Count
                {
                    get { return map.Count; }
                }

                public Enumerator(TMapBase<TKey, TValue> map)
                {
                    this.map = map;
                    index = -1;
                }

                public TKey Current
                {
                    get { return map.GetAt(index).Key; }
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
                    int maxIndex = map.MapHelper.GetMaxIndex();
                    while (++index < maxIndex && !map.MapHelper.IsValidIndex(index)) { }
                    return index < maxIndex;
                }

                public void Reset()
                {
                    index = -1;
                }
            }
        }

        public struct ValueCollection : ICollection<TValue>
        {
            private TMapBase<TKey, TValue> map;

            public ValueCollection(TMapBase<TKey, TValue> map)
            {
                this.map = map;
            }

            public int Count
            {
                get { return map.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public void Add(TValue item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TValue item)
            {
                return map.ContainsValue(item);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                int maxIndex = map.MapHelper.GetMaxIndex();
                int index = arrayIndex;
                for (int i = 0; i < maxIndex; ++i)
                {
                    if (map.MapHelper.IsValidIndex(i))
                    {
                        array[index++] = map.GetAt(i).Value;
                    }
                }
            }

            public bool Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(map);
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new Enumerator(map);
            }            

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(map);
            }

            public struct Enumerator : IEnumerator<TValue>
            {
                private int index;
                private TMapBase<TKey, TValue> map;

                public int Count
                {
                    get { return map.Count; }
                }

                public Enumerator(TMapBase<TKey, TValue> map)
                {
                    this.map = map;
                    index = -1;
                }

                public TValue Current
                {
                    get { return map.GetAt(index).Value; }
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
                    int maxIndex = map.MapHelper.GetMaxIndex();
                    while (++index < maxIndex && !map.MapHelper.IsValidIndex(index)) { }
                    return index < maxIndex;
                }

                public void Reset()
                {
                    index = -1;
                }
            }
        }
    }

    public class TMapReadOnly<TKey, TValue> : TMapBase<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        public TMapReadOnly(UObject owner, UFieldAddress mapProperty, IntPtr address,
            MarshalingDelegates<TKey>.FromNative keyFromNative, MarshalingDelegates<TValue>.FromNative valueFromNative)
            : base(owner, mapProperty, address, keyFromNative, null, valueFromNative, null)
        {
        }

        public TValue this[TKey key]
        {
            get { return Get(key); }
        }

        public KeyEnumerator Keys
        {
            get { return new KeyEnumerator(this); }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return new KeyEnumerator(this); }
        }

        public ValueCollection Values
        {
            get { return new ValueCollection(this); }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return new ValueCollection(this); }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = IndexOf(key);
            if (index >= 0)
            {
                value = GetAt(index).Value;
                return true;
            }
            value = default(TValue);
            return false;
        }
    }

    public class TMapReadWrite<TKey, TValue> : TMapBase<TKey, TValue>, IDictionary<TKey, TValue>
    {
        public TMapReadWrite(UObject owner, UFieldAddress mapProperty, IntPtr address,
            MarshalingDelegates<TKey>.FromNative keyFromNative, MarshalingDelegates<TKey>.ToNative keyToNative,
            MarshalingDelegates<TValue>.FromNative valueFromNative, MarshalingDelegates<TValue>.ToNative valueToNative)
            : base(owner, mapProperty, address, keyFromNative, keyToNative, valueFromNative, valueToNative)
        {
        }

        public TValue this[TKey key]
        {
            get { return Get(key); }
            set { AddInternal(key, value); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public KeyEnumerator Keys
        {
            get { return new KeyEnumerator(this); }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return new KeyEnumerator(this); }
        }

        public ValueCollection Values
        {
            get { return new ValueCollection(this); }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return new ValueCollection(this); }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            // An exception WONT be thrown if it already exists, it will set the key to the new value
            AddInternal(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            // An exception WONT be thrown if it already exists, it will set the key to the new value
            AddInternal(key, value);
        }

        public void Clear()
        {
            ClearInternal();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int maxIndex = MapHelper.GetMaxIndex();
            int index = arrayIndex;
            for (int i = 0; i < maxIndex; ++i)
            {
                if (MapHelper.IsValidIndex(i))
                {
                    array[index++] = GetAt(i);
                }
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return RemoveInternal(item.Key);
        }

        public bool Remove(TKey key)
        {
            return RemoveInternal(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return TryGetInternal(key, out value);
        }
    }

    // Used for members only
    public class TMapReadWriteMarshaler<TKey, TValue>
    {
        UFieldAddress property;
        FScriptMapHelper helper;
        TMapReadWrite<TKey, TValue>[] wrappers;
        MarshalingDelegates<TKey>.FromNative keyFromNative;
        MarshalingDelegates<TKey>.ToNative keyToNative;
        MarshalingDelegates<TValue>.FromNative valueFromNative;
        MarshalingDelegates<TValue>.ToNative valueToNative;

        public TMapReadWriteMarshaler(int length, UFieldAddress mapProperty,
            MarshalingDelegates<TKey>.FromNative keyFromNative, MarshalingDelegates<TKey>.ToNative keyToNative,
            MarshalingDelegates<TValue>.FromNative valueFromNative, MarshalingDelegates<TValue>.ToNative valueToNative)
        {
            property = mapProperty;
            helper = new FScriptMapHelper(property.Address);
            wrappers = new TMapReadWrite<TKey, TValue>[length];
            this.keyFromNative = keyFromNative;
            this.keyToNative = keyToNative;
            this.valueFromNative = valueFromNative;
            this.valueToNative = valueToNative;
        }

        public TMapReadWrite<TKey, TValue> FromNative(IntPtr nativeBuffer)
        {
            return FromNative(nativeBuffer, 0, IntPtr.Zero);
        }

        public TMapReadWrite<TKey, TValue> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            if (wrappers[arrayIndex] == null)
            {
                wrappers[arrayIndex] = new TMapReadWrite<TKey, TValue>(null, property, nativeBuffer +
                    (arrayIndex * Marshal.SizeOf(typeof(FScriptMap))), keyFromNative, keyToNative, valueFromNative, valueToNative);
            }
            return wrappers[arrayIndex];
        }

        public void ToNative(IntPtr nativeBuffer, IDictionary<TKey, TValue> value)
        {
            helper.Update(property);
            ToNativeInternal(nativeBuffer, 0, value, ref helper, keyToNative, valueToNative);
        }

        public void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, IDictionary<TKey, TValue> value)
        {
            helper.Update(property);
            ToNativeInternal(nativeBuffer, arrayIndex, value, ref helper, keyToNative, valueToNative);
        }

        internal static void ToNativeInternal(IntPtr nativeBuffer, int arrayIndex, IDictionary<TKey, TValue> value,
            ref FScriptMapHelper helper, MarshalingDelegates<TKey>.ToNative keyToNative, MarshalingDelegates<TValue>.ToNative valueToNative)
        {
            IntPtr scriptMapAddress = nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptMap)));            
            helper.Map = scriptMapAddress;

            // Make sure any existing elements are properly destroyed
            helper.EmptyValues();

            if (value == null)
            {
                return;
            }

            unsafe
            {
                FScriptMap* map = (FScriptMap*)scriptMapAddress;

                Dictionary<TKey, TValue> dictionary = value as Dictionary<TKey, TValue>;
                if (dictionary != null)
                {
                    foreach(KeyValuePair<TKey, TValue> pair in dictionary)
                    {
                        helper.AddPair(pair.Key, pair.Value, keyToNative, valueToNative);
                    }
                    return;
                }

                TMapBase<TKey, TValue> mapBase = value as TMapBase<TKey, TValue>;
                if (mapBase != null)
                {
                    foreach (KeyValuePair<TKey, TValue> pair in mapBase)
                    {
                        helper.AddPair(pair.Key, pair.Value, keyToNative, valueToNative);
                    }
                    return;
                }

                foreach (KeyValuePair<TKey, TValue> pair in value)
                {
                    helper.AddPair(pair.Key, pair.Value, keyToNative, valueToNative);
                }
            }
        }
    }

    // Used for members only where they are exposed as readonly
    public class TMapReadOnlyMarshaler<TKey, TValue>
    {
        UFieldAddress property;
        TMapReadOnly<TKey, TValue>[] wrappers;
        MarshalingDelegates<TKey>.FromNative keyFromNative;
        MarshalingDelegates<TValue>.FromNative valueFromNative;

        public TMapReadOnlyMarshaler(int length, UFieldAddress mapProperty,
            MarshalingDelegates<TKey>.FromNative keyFromNative, MarshalingDelegates<TKey>.ToNative keyToNative,
            MarshalingDelegates<TValue>.FromNative valueFromNative, MarshalingDelegates<TValue>.ToNative valueToNative)
        {
            property = mapProperty;
            wrappers = new TMapReadOnly<TKey, TValue>[length];
            this.keyFromNative = keyFromNative;
            this.valueFromNative = valueFromNative;
        }

        public TMapReadOnly<TKey, TValue> FromNative(IntPtr nativeBuffer)
        {
            return FromNative(nativeBuffer, 0, IntPtr.Zero);
        }

        public TMapReadOnly<TKey, TValue> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            if (wrappers[arrayIndex] == null)
            {
                wrappers[arrayIndex] = new TMapReadOnly<TKey, TValue>(null, property, nativeBuffer +
                    (arrayIndex * Marshal.SizeOf(typeof(FScriptMap))), keyFromNative, valueFromNative);
            }
            return wrappers[arrayIndex];
        }

        public void ToNative(IntPtr nativeBuffer, IReadOnlyDictionary<TKey, TValue> value)
        {
            ToNative(nativeBuffer, 0, IntPtr.Zero, value);
        }

        public void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, IReadOnlyDictionary<TKey, TValue> value)
        {
            throw new NotImplementedException("Read-only TMap cannot write to native memory.");
        }
    }

    // Used for function parameters / return results to copy to/from native memory
    public struct TMapCopyMarshaler<TKey, TValue>
    {
        UFieldAddress property;
        FScriptMapHelper helper;
        MarshalingDelegates<TKey>.FromNative keyFromNative;
        MarshalingDelegates<TKey>.ToNative keyToNative;
        MarshalingDelegates<TValue>.FromNative valueFromNative;
        MarshalingDelegates<TValue>.ToNative valueToNative;

        public TMapCopyMarshaler(int length, UFieldAddress mapProperty,
            MarshalingDelegates<TKey>.FromNative keyFromNative, MarshalingDelegates<TKey>.ToNative keyToNative,
            MarshalingDelegates<TValue>.FromNative valueFromNative, MarshalingDelegates<TValue>.ToNative valueToNative)
        {
            property = mapProperty;
            helper = new FScriptMapHelper(property.Address);
            this.keyFromNative = keyFromNative;
            this.keyToNative = keyToNative;
            this.valueFromNative = valueFromNative;
            this.valueToNative = valueToNative;
        }

        public Dictionary<TKey, TValue> FromNative(IntPtr nativeBuffer)
        {
            return FromNative(nativeBuffer, 0, IntPtr.Zero);
        }

        public Dictionary<TKey, TValue> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            IntPtr scriptMapAddress = nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptMap)));
            helper.Map = scriptMapAddress;

            unsafe
            {
                FScriptMap* map = (FScriptMap*)scriptMapAddress;
                Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
                int maxIndex = map->GetMaxIndex();
                for (int i = 0; i < maxIndex; ++i)
                {
                    if (map->IsValidIndex(i))
                    {
                        IntPtr keyPtr, valuePtr;
                        helper.GetPairPtr(i, out keyPtr, out valuePtr);
                        result.Add(
                            keyFromNative(keyPtr, 0, helper.KeyPropertyAddress),
                            valueFromNative(valuePtr, 0, helper.ValuePropertyAddress));
                    }
                }
                return result;
            }
        }

        public void ToNative(IntPtr nativeBuffer, IDictionary<TKey, TValue> value)
        {
            helper.Update(property);
            TMapReadWriteMarshaler<TKey, TValue>.ToNativeInternal(nativeBuffer, 0, value, ref helper, keyToNative, valueToNative);
        }

        public void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, IDictionary<TKey, TValue> value)
        {
            helper.Update(property);
            TMapReadWriteMarshaler<TKey, TValue>.ToNativeInternal(nativeBuffer, arrayIndex, value, ref helper, keyToNative, valueToNative);
        }
    }

    /// <summary>
    /// Used for UObject.DynamicInvoke
    /// </summary>
    public static class TMapStaticCopyMarshaler<TKey, TValue>
    {
        static MarshalingDelegates<TKey>.FromNative keyFromNative = MarshalingDelegateResolver<TKey>.FromNative;
        static MarshalingDelegates<TKey>.ToNative keyToNative = MarshalingDelegateResolver<TKey>.ToNative;
        static MarshalingDelegates<TValue>.FromNative valueFromNative = MarshalingDelegateResolver<TValue>.FromNative;
        static MarshalingDelegates<TValue>.ToNative valueToNative = MarshalingDelegateResolver<TValue>.ToNative;

        public static IDictionary<TKey, TValue> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            IntPtr scriptMapAddress = nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptMap)));
            FScriptMapHelper helper = new FScriptMapHelper(prop, scriptMapAddress);

            unsafe
            {
                FScriptMap* map = (FScriptMap*)scriptMapAddress;
                Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
                int maxIndex = map->GetMaxIndex();
                for (int i = 0; i < maxIndex; ++i)
                {
                    if (map->IsValidIndex(i))
                    {
                        IntPtr keyPtr, valuePtr;
                        helper.GetPairPtr(i, out keyPtr, out valuePtr);
                        result.Add(
                            keyFromNative(keyPtr, 0, helper.KeyPropertyAddress),
                            valueFromNative(valuePtr, 0, helper.ValuePropertyAddress));
                    }
                }
                return result;
            }
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, IDictionary<TKey, TValue> value)
        {
            FScriptMapHelper helper = new FScriptMapHelper(prop);
            TMapReadWriteMarshaler<TKey, TValue>.ToNativeInternal(
                nativeBuffer, arrayIndex, value, ref helper, keyToNative, valueToNative);
        }
    }
}
