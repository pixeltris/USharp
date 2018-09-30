using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Untyped map type for accessing TMap data, like FScriptArray for TArray.
    /// Must have the same memory representation as a TMap.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FScriptMap
    {
        public FScriptSet Pairs;

        public int Count
        {
            get { return Pairs.Count; }
        }

        public bool IsValidIndex(int index)
        {
            return Pairs.IsValidIndex(index);
        }

        public int Num()
        {
            return Pairs.Num();
        }

        public int GetMaxIndex()
        {
            return Pairs.GetMaxIndex();
        }

        public IntPtr GetData(int index, ref FScriptMapLayout layout)
        {
            return Pairs.GetData(index, ref layout.SetLayout);
        }

        public void Empty(int slack, ref FScriptMapLayout layout)
        {
            Pairs.Empty(slack, ref layout.SetLayout);
        }

        public void RemoveAt(int index, ref FScriptMapLayout layout)
        {
            Pairs.RemoveAt(index, ref layout.SetLayout);
        }

        /// <summary>
        /// Adds an uninitialized object to the map.
        /// The map will need rehashing at some point after this call to make it valid.
        /// </summary>
        /// <returns>The index of the added element.</returns>
        public int AddUninitialized(ref FScriptMapLayout layout)
        {
            return Pairs.AddUninitialized(ref layout.SetLayout);
        }

        public void Rehash(ref FScriptMapLayout layout, HashDelegates.GetKeyHash getKeyHash)
        {
            Pairs.Rehash(ref layout.SetLayout, getKeyHash);
        }

        /// <summary>
        /// Finds the associated key, value from hash of Key, rather than linearly searching
        /// </summary>
        public int FindPairIndex(IntPtr key, ref FScriptMapLayout layout, HashDelegates.GetKeyHash getKeyHash, HashDelegates.Equality keyEqualityFn)
        {
            return Native_FScriptMap.FindPairIndex(ref this, key, ref layout, getKeyHash, keyEqualityFn);
        }

        /// <summary>
        /// Finds the associated value from hash of Key, rather than linearly searching
        /// </summary>
        public IntPtr FindValue(IntPtr key, ref FScriptMapLayout layout, HashDelegates.GetKeyHash getKeyHash, HashDelegates.Equality keyEqualityFn)
        {
            return Native_FScriptMap.FindValue(ref this, key, ref layout, getKeyHash, keyEqualityFn);
        }

        /// <summary>
        /// Adds the (key, value) pair to the map, returning true if the element was added, or false if the element was already present and has been overwritten
        /// </summary>
        public void Add(IntPtr key, IntPtr value, ref FScriptMapLayout layout, HashDelegates.GetKeyHash getKeyHash, HashDelegates.Equality keyEqualityFn,
            HashDelegates.ConstructAndAssign keyConstructAndAssignFn, HashDelegates.ConstructAndAssign valueConstructAndAssignFn,
            HashDelegates.Assign valueAssignFn, HashDelegates.Destruct destructKeyFn, HashDelegates.Destruct destructValueFn)
        {
            Native_FScriptMap.Add(ref this, key, value, ref layout, getKeyHash, keyEqualityFn, keyConstructAndAssignFn, valueConstructAndAssignFn,
                valueAssignFn, destructKeyFn, destructValueFn);
        }

        public void Destroy()
        {
            Native_FScriptMap.Destroy(ref this);
            ZeroMemory();
        }

        public void Destroy(IntPtr property)
        {
            unsafe
            {
                fixed (FScriptMap* map = &this)
                {
                    Native_UProperty.DestroyValue(property, (IntPtr)map);
                }
            }
            ZeroMemory();
        }

        public void Destroy(UMapProperty property)
        {
            Destroy(property.Address);
        }

        private void ZeroMemory()
        {
            FMemory.Memzero(ref this);
        }

        public static FScriptMapLayout GetScriptLayout(int keySize, int keyAlignment, int valueSize, int valueAlignment)
        {
            return Native_FScriptMap.GetScriptLayout(keySize, keyAlignment, valueSize, valueAlignment);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FScriptMapLayout
    {
        public int KeyOffset;
        public int ValueOffset;

        public FScriptSetLayout SetLayout;
    }

    public class HashDelegates
    {
        public delegate uint GetKeyHash(IntPtr element);
        public delegate csbool Equality(IntPtr a, IntPtr b);
        public delegate void Construct(IntPtr element);
        public delegate void Destruct(IntPtr element);
        public delegate void ConstructAndAssign(IntPtr element);
        public delegate void Assign(IntPtr element);
    }
}
