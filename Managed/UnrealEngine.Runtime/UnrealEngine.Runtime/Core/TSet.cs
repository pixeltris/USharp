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
    [DebuggerTypeProxy(typeof(TSetDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public unsafe class TSetBase<T> : IEnumerable<T>
    {
        protected readonly UObject Owner;
        protected MarshalingDelegates<T>.FromNative FromNative;
        protected MarshalingDelegates<T>.ToNative ToNative;

        readonly UFieldAddress property;
        readonly FScriptSet* set;

        public int Count
        {
            get
            {
                CheckOwner();
                return set->Count;
            }
        }

        /// <summary>
        /// Address of the TSet/FScriptSet
        /// </summary>
        protected IntPtr Address
        {
            get { return (IntPtr)set; }
        }

        protected FScriptSetHelper SetHelper;

        public TSetBase(UObject owner, UFieldAddress setProperty, IntPtr address,
            MarshalingDelegates<T>.FromNative fromNative, MarshalingDelegates<T>.ToNative toNative)
        {
            property = setProperty;
            set = (FScriptSet*)address;

            SetHelper = new FScriptSetHelper(property.Address, address);

            Owner = owner;
            FromNative = fromNative;
            ToNative = toNative;
            
            ContainerHashValidator.Validate(Native_USetProperty.Get_ElementProp(setProperty.Address));
        }

        [Conditional("DEBUG")]
        protected void CheckOwner()
        {
            if (Owner != null && Owner.IsDestroyed)
            {
                throw new UObjectDestroyedException("Trying to access a TSet which points to memory of a destroyed UObject (" + property.PathName + ")");
            }
        }

        protected void ClearInternal()
        {
            CheckOwner();
            SetHelper.Update(property);
            SetHelper.EmptyValues();
        }

        protected void AddInternal(T item)
        {
            CheckOwner();
            SetHelper.Update(property);
            SetHelper.AddElement(item, ToNative);
        }

        protected void RemoveAtInternal(int index)
        {
            CheckOwner();
            if (SetHelper.IsValidIndex(index))
            {
                SetHelper.Update(property);
                SetHelper.RemoveAt(index);
            }
        }

        public T Get(int index)
        {
            if (!SetHelper.IsValidIndex(index))
            {
                throw new IndexOutOfRangeException(string.Format("Index {0} is invalid. Indicies aren't necessarily sequential.", index));
            }
            SetHelper.Update(property);
            return FromNative(SetHelper.GetElementPtr(index), 0, SetHelper.ElementPropertyAddress);
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public int IndexOf(T item)
        {
            SetHelper.Update(property);
            return SetHelper.IndexOf(item, ToNative, Owner);
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
            private TSetBase<T> set;

            public Enumerator(TSetBase<T> set)
            {
                this.set = set;
                index = -1;
            }

            public T Current
            {
                get { return set.Get(index); }
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
                int maxIndex = set.SetHelper.GetMaxIndex();
                while (++index < maxIndex && !set.SetHelper.IsValidIndex(index)) { }
                return index < maxIndex;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }

    public class TSetReadOnly<T> : TSetReadWrite<T>
    {
        public new bool IsReadOnly
        {
            get { return true; }
        }

        public TSetReadOnly(UObject owner, UFieldAddress setProperty, IntPtr address, MarshalingDelegates<T>.FromNative fromNative)
            : base(owner, setProperty, address, fromNative, null)
        {
        }
    }

    public class TSetReadWrite<T> : TSetBase<T>, ISet<T>
    {
        // Expect some of the ISet functions to be broken. They are a quick dirty job and some allocate a HashSet
        // to avoid unwanted duplicates on IEnumerable<T>

        public TSetReadWrite(UObject owner, UFieldAddress setProperty, IntPtr address,
            MarshalingDelegates<T>.FromNative fromNative, MarshalingDelegates<T>.ToNative toNative)
            : base(owner, setProperty, address, fromNative, toNative)
        {
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Add(T item)
        {
            // It would be better if AddInternal told us if the collection was modified when we add...
            // It's best to make sure the ISet interface behaves correctly, so look for the element before adding it.
            if (!Contains(item))
            {
                AddInternal(item);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            ClearInternal();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int maxIndex = SetHelper.GetMaxIndex();
            int index = arrayIndex;
            for (int i = 0; i < maxIndex; ++i)
            {
                if (SetHelper.IsValidIndex(i))
                {
                    array[index++] = Get(i);
                }
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (Count == 0)
            {
                return;
            }

            TSetBase<T> otherAsSet = other as TSetBase<T>;
            if (otherAsSet != null)
            {
                foreach (T element in otherAsSet)
                {
                    Remove(element);
                }
            }
            else
            {
                foreach (T element in other)
                {
                    Remove(element);
                }
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            TSetBase<T> otherAsSet = other as TSetBase<T>;
            if (otherAsSet != null)
            {
                if (otherAsSet.Count == 0)
                {
                    Clear();
                    return;
                }

                int maxIndex = SetHelper.GetMaxIndex();
                for (int i = maxIndex - 1; i >= 0; --i)
                {
                    if (SetHelper.IsValidIndex(i))
                    {
                        T item = Get(i);
                        if (!otherAsSet.Contains(item))
                        {
                            RemoveAtInternal(i);
                        }
                    }
                }
            }
            else
            {
                // HashSet to avoid duplicates
                HashSet<T> set = new HashSet<T>(other);
                if (set.Count == 0)
                {
                    Clear();
                    return;
                }

                int maxIndex = SetHelper.GetMaxIndex();
                for (int i = maxIndex - 1; i >= 0; --i)
                {
                    if (SetHelper.IsValidIndex(i))
                    {
                        T item = Get(i);
                        if (!set.Contains(item))
                        {
                            RemoveAtInternal(i);
                        }
                    }
                }
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            TSetBase<T> otherAsSet = other as TSetBase<T>;
            if (otherAsSet != null)
            {
                if (Count == 0)
                {
                    return otherAsSet.Count > 0;
                }
                if (Count >= otherAsSet.Count)
                {
                    return false;
                }
                foreach (T item in this)
                {
                    if (!otherAsSet.Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                // HashSet to avoid duplicates
                HashSet<T> set = new HashSet<T>(other);
                if (Count == 0)
                {
                    return set.Count > 0;
                }
                if (Count >= set.Count)
                {
                    return false;
                }
                foreach (T item in this)
                {
                    if (!set.Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (Count == 0)
            {
                return false;
            }

            TSetBase<T> otherAsSet = other as TSetBase<T>;
            if (otherAsSet != null)
            {
                foreach (T item in otherAsSet)
                {
                    if (!Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                foreach (T item in other)
                {
                    if (!Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (Count == 0)
            {
                return true;
            }

            TSetBase<T> otherAsSet = other as TSetBase<T>;
            if (otherAsSet != null)
            {
                if (Count > otherAsSet.Count)
                {
                    return false;
                }

                foreach (T item in this)
                {
                    if (!otherAsSet.Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                // HashSet to avoid duplicates
                HashSet<T> set = new HashSet<T>(other);
                if (Count > set.Count)
                {
                    return false;
                }

                foreach (T item in this)
                {
                    if (!set.Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            TSetBase<T> otherAsSet = other as TSetBase<T>;
            if (otherAsSet != null)
            {
                if (otherAsSet.Count == 0)
                {
                    return true;
                }
                if (otherAsSet.Count > Count)
                {
                    return false;
                }
                foreach (T item in otherAsSet)
                {
                    if (!Contains(item))
                    {
                        return false;
                    }
                }
            }
            else
            {
                foreach (T item in other)
                {
                    if (!Contains(item))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (Count == 0)
            {
                return false;
            }

            TSetBase<T> otherAsSet = other as TSetBase<T>;
            if (otherAsSet != null)
            {
                foreach (T item in otherAsSet)
                {
                    if (Contains(item))
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (T item in other)
                {
                    if (Contains(item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAtInternal(index);
                return true;
            }
            return false;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            TSetBase<T> otherAsSet = other as TSetBase<T>;
            if (otherAsSet != null)
            {
                if (Count != otherAsSet.Count)
                {
                    return false;
                }

                foreach (T item in otherAsSet)
                {
                    if (!Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                foreach (T item in other)
                {
                    if (!Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (Count == 0)
            {
                UnionWith(other);
                return;
            }

            TSetBase<T> otherAsSet = other as TSetBase<T>;
            if (otherAsSet != null)
            {
                foreach (T item in otherAsSet)
                {
                    if (!Remove(item))
                    {
                        Add(item);
                    }
                }
            }
            else
            {
                // HashSet to avoid duplicates
                HashSet<T> set = new HashSet<T>(other);
                foreach (T item in set)
                {
                    if (!Remove(item))
                    {
                        Add(item);
                    }
                }
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            TSetBase<T> otherAsSet = other as TSetBase<T>;
            if (otherAsSet != null)
            {
                foreach (T item in otherAsSet)
                {
                    if (!Contains(item))
                    {
                        Add(item);
                    }
                }
            }
            else
            {
                foreach (T item in other)
                {
                    if (!Contains(item))
                    {
                        Add(item);
                    }
                }
            }
        }

        void ICollection<T>.Add(T item)
        {
            AddInternal(item);
        }
    }

    // Used for members only
    public class TSetReadWriteMarshaler<T>
    {
        UFieldAddress property;
        FScriptSetHelper helper;
        TSetReadWrite<T>[] wrappers;
        MarshalingDelegates<T>.FromNative elementFromNative;
        MarshalingDelegates<T>.ToNative elementToNative;

        public TSetReadWriteMarshaler(int length, UFieldAddress setProperty,
            MarshalingDelegates<T>.FromNative fromNative, MarshalingDelegates<T>.ToNative toNative)
        {
            property = setProperty;
            helper = new FScriptSetHelper(property.Address);
            wrappers = new TSetReadWrite<T>[length];
            elementFromNative = fromNative;
            elementToNative = toNative;
        }

        public TSetReadWrite<T> FromNative(IntPtr nativeBuffer)
        {
            return FromNative(nativeBuffer, 0, IntPtr.Zero);
        }

        public TSetReadWrite<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            if (wrappers[arrayIndex] == null)
            {
                wrappers[arrayIndex] = new TSetReadWrite<T>(null, property, nativeBuffer +
                    (arrayIndex * Marshal.SizeOf(typeof(FScriptSet))), elementFromNative, elementToNative);
            }
            return wrappers[arrayIndex];
        }

        public void ToNative(IntPtr nativeBuffer, IEnumerable<T> value)
        {
            helper.Update(property);
            ToNativeInternal(nativeBuffer, 0, value, ref helper, elementToNative);
        }

        public void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, IEnumerable<T> value)
        {
            helper.Update(property);
            ToNativeInternal(nativeBuffer, arrayIndex, value, ref helper, elementToNative);
        }

        internal static void ToNativeInternal(IntPtr nativeBuffer, int arrayIndex, IEnumerable<T> value,
            ref FScriptSetHelper helper, MarshalingDelegates<T>.ToNative elementToNative)
        {
            IntPtr scriptSetAddress = nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptSet)));
            helper.Set = scriptSetAddress;

            // Make sure any existing elements are properly destroyed
            helper.EmptyValues();

            if (value == null)
            {
                return;
            }

            unsafe
            {
                FScriptSet* set = (FScriptSet*)scriptSetAddress;

                IList<T> list = value as IList<T>;
                if (list != null)
                {
                    for (int i = 0; i < list.Count; ++i)
                    {
                        helper.AddElement(list[i], elementToNative);
                    }
                    return;
                }

                HashSet<T> hashSet = value as HashSet<T>;
                if (hashSet != null)
                {
                    foreach (T item in hashSet)
                    {
                        helper.AddElement(item, elementToNative);
                    }
                    return;
                }

                TSetBase<T> setBase = value as TSetBase<T>;
                if (setBase != null)
                {
                    foreach (T item in setBase)
                    {
                        helper.AddElement(item, elementToNative);
                    }
                    return;
                }

                foreach (T item in value)
                {
                    helper.AddElement(item, elementToNative);
                }
            }
        }
    }

    // Used for members only where they are exposed as readonly
    public class TSetReadOnlyMarshaler<T>
    {
        UFieldAddress property;
        TSetReadOnly<T>[] wrappers;
        MarshalingDelegates<T>.FromNative elementFromNative;

        public TSetReadOnlyMarshaler(int length, UFieldAddress setProperty,
            MarshalingDelegates<T>.FromNative fromNative, MarshalingDelegates<T>.ToNative toNative)
        {
            property = setProperty;
            wrappers = new TSetReadOnly<T>[length];
            elementFromNative = fromNative;
        }

        public TSetReadOnly<T> FromNative(IntPtr nativeBuffer)
        {
            return FromNative(nativeBuffer, 0, IntPtr.Zero);
        }

        public TSetReadOnly<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            if (wrappers[arrayIndex] == null)
            {
                wrappers[arrayIndex] = new TSetReadOnly<T>(null, property, nativeBuffer +
                    (arrayIndex * Marshal.SizeOf(typeof(FScriptSet))), elementFromNative);
            }
            return wrappers[arrayIndex];
        }

        public void ToNative(IntPtr nativeBuffer, IReadOnlyCollection<T> value)
        {
            ToNative(nativeBuffer, 0, IntPtr.Zero, value);
        }

        public void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, IReadOnlyCollection<T> value)
        {
            throw new NotImplementedException("Read-only TSet cannot write to native memory.");
        }
    }

    // Used for function parameters / return results to copy to/from native memory
    public struct TSetCopyMarshaler<T>
    {
        UFieldAddress property;
        FScriptSetHelper helper;
        MarshalingDelegates<T>.FromNative elementFromNative;
        MarshalingDelegates<T>.ToNative elementToNative;

        public TSetCopyMarshaler(int length, UFieldAddress setProperty,
            MarshalingDelegates<T>.FromNative fromNative, MarshalingDelegates<T>.ToNative toNative)
        {
            property = setProperty;
            helper = new FScriptSetHelper(property.Address);
            elementFromNative = fromNative;
            elementToNative = toNative;
        }

        public HashSet<T> FromNative(IntPtr nativeBuffer)
        {
            return FromNative(nativeBuffer, 0, IntPtr.Zero);
        }

        public HashSet<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            IntPtr scriptSetAddress = nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptSet)));

            helper.Update(property);
            helper.Set = scriptSetAddress;

            unsafe
            {
                FScriptSet* set = (FScriptSet*)scriptSetAddress;
                HashSet<T> result = new HashSet<T>();
                int maxIndex = set->GetMaxIndex();
                for (int i = 0; i < maxIndex; ++i)
                {
                    if (set->IsValidIndex(i))
                    {
                        result.Add(elementFromNative(helper.GetElementPtr(i), 0, helper.ElementPropertyAddress));
                    }
                }
                return result;
            }
        }

        public void ToNative(IntPtr nativeBuffer, IEnumerable<T> value)
        {
            helper.Update(property);
            TSetReadWriteMarshaler<T>.ToNativeInternal(nativeBuffer, 0, value, ref helper, elementToNative);
        }

        public void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, IEnumerable<T> value)
        {
            helper.Update(property);
            TSetReadWriteMarshaler<T>.ToNativeInternal(nativeBuffer, arrayIndex, value, ref helper, elementToNative);
        }
    }

    /// <summary>
    /// Used for UObject.DynamicInvoke
    /// </summary>
    public static class TSetStaticCopyMarshaler<T>
    {
        static MarshalingDelegates<T>.FromNative elementFromNative = MarshalingDelegateResolver<T>.FromNative;
        static MarshalingDelegates<T>.ToNative elementToNative = MarshalingDelegateResolver<T>.ToNative;

        public static ISet<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            IntPtr scriptSetAddress = nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptSet)));
            FScriptSetHelper helper = new FScriptSetHelper(prop, scriptSetAddress);

            unsafe
            {
                FScriptSet* set = (FScriptSet*)scriptSetAddress;
                HashSet<T> result = new HashSet<T>();
                int maxIndex = set->GetMaxIndex();
                for (int i = 0; i < maxIndex; ++i)
                {
                    if (set->IsValidIndex(i))
                    {
                        result.Add(elementFromNative(helper.GetElementPtr(i), 0, helper.ElementPropertyAddress));
                    }
                }
                return result;
            }
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, ISet<T> value)
        {
            FScriptSetHelper helper = new FScriptSetHelper(prop);
            TSetReadWriteMarshaler<T>.ToNativeInternal(nativeBuffer, arrayIndex, value, ref helper, elementToNative);
        }
    }
}
