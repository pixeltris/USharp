using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Untyped set type for accessing TSet data, like FScriptArray for TArray.
    /// Must have the same memory representation as a TSet.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FScriptSet
    {
        public FScriptSparseArray Elements;
        public FHashAllocator Hash;
        public int HashSize;

        public int Count
        {
            get { return Native_FScriptSet.Num(ref this); }
        }

        public bool IsValidIndex(int index)
        {
            return Native_FScriptSet.IsValidIndex(ref this, index);
        }

        public int Num()
        {
            return Native_FScriptSet.Num(ref this);
        }

        public int GetMaxIndex()
        {
            return Native_FScriptSet.GetMaxIndex(ref this);
        }

        public IntPtr GetData(int index, ref FScriptSetLayout layout)
        {
            return Native_FScriptSet.GetData(ref this, index, ref layout);
        }

        public void Empty(int slack, ref FScriptSetLayout layout)
        {
            Native_FScriptSet.Empty(ref this, slack, ref layout);
        }

        public void RemoveAt(int index, ref FScriptSetLayout layout)
        {
            Native_FScriptSet.RemoveAt(ref this, index, ref layout);
        }

        public int AddUninitialized(ref FScriptSetLayout layout)
        {
            return Native_FScriptSet.AddUninitialized(ref this, ref layout);
        }

        public void Rehash(ref FScriptSetLayout layout, HashDelegates.GetKeyHash getKeyHash)
        {
            Native_FScriptSet.Rehash(ref this, ref layout, getKeyHash);
        }

        public int FindIndex(IntPtr element, ref FScriptSetLayout layout, HashDelegates.GetKeyHash getKeyHash, HashDelegates.Equality equalityFn)
        {
            return Native_FScriptSet.FindIndex(ref this, element, ref layout, getKeyHash, equalityFn);
        }

        public void Add(IntPtr element, ref FScriptSetLayout layout, HashDelegates.GetKeyHash getKeyHash, HashDelegates.Equality equalityFn,
            HashDelegates.Construct constructFn, HashDelegates.Destruct destructFn)
        {
            Native_FScriptSet.Add(ref this, element, ref layout, getKeyHash, equalityFn, constructFn, destructFn);
        }

        public void Destroy()
        {
            Native_FScriptSet.Destroy(ref this);
            ZeroMemory();
        }
        
        public void Destroy(IntPtr mapProperty)
        {
            unsafe
            {
                fixed (FScriptSet* set = &this)
                {
                    Native_UProperty.DestroyValue(mapProperty, (IntPtr)set);
                }
            }
            ZeroMemory();
        }

        public void Destroy(USetProperty property)
        {
            Destroy(property.Address);
        }

        private void ZeroMemory()
        {
            FMemory.Memzero(ref this);
        }

        public static FScriptSetLayout GetScriptLayout(int elementSize, int elementAlignment)
        {
            return Native_FScriptSet.GetScriptLayout(elementSize, elementAlignment);
        }
    }

    /// <summary>
    /// Either NULL or an identifier for an element of a set.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FSetElementId
    {
        /// <summary>
        /// The index of the element in the set's element array.
        /// </summary>
        public int Index;

        public bool IsValidId
        {
            get { return Index != -1; }
        }

        public static FSetElementId Default
        {
            get { return new FSetElementId(-1); }
        }

        public FSetElementId(int index)
        {
            Index = index;
        }

        public int AsInteger()
        {
            return Index;
        }

        public static FSetElementId FromInteger(int integer)
        {
            return new FSetElementId(integer);
        }
    }

    //Allocator::HashAllocator = FDefaultSetAllocator
    //FDefaultSetAllocator = TInlineAllocator<1>
    //FDefaultSetAllocator::ForElementType<FSetElementId> = TInlineAllocator<1>::ForElementType<FSetElementId>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FHashAllocator
    {
        // As NumInlineElements=1 we can just avoid using an array
        public FSetElementId InlineData;
        public IntPtr SecondaryData;

        // FSetElementId is size of int32 so int[1] works
    }

    /// <summary>
    /// Untyped sparse array type for accessing TSparseArray data, like FScriptArray for TArray.
    /// Must have the same memory representation as a TSet.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FScriptSparseArray
    {
        public FScriptArray Data;
        public FScriptBitArray AllocationFlags;
        public int FirstFreeIndex;
        public int NumFreeIndices;

        public int Count
        {
            get { return Native_FScriptSparseArray.Num(ref this); }
        }

        public bool IsValidIndex(int index)
        {
            return Native_FScriptSparseArray.IsValidIndex(ref this, index);
        }

        public int Num()
        {
            return Native_FScriptSparseArray.Num(ref this);
        }

        public int GetMaxIndex()
        {
            return Native_FScriptSparseArray.GetMaxIndex(ref this);
        }

        public IntPtr GetData(int index, ref FScriptSparseArrayLayout layout)
        {
            return Native_FScriptSparseArray.GetData(ref this, index, ref layout);
        }

        public void Empty(int slack, ref FScriptSparseArrayLayout layout)
        {
            Native_FScriptSparseArray.Empty(ref this, slack, ref layout);
        }

        public int AddUninitialized(ref FScriptSparseArrayLayout layout)
        {
            return Native_FScriptSparseArray.AddUninitialized(ref this, ref layout);
        }

        public void RemoveAtUninitialized(ref FScriptSparseArrayLayout layout, int index, int count = 1)
        {
            Native_FScriptSparseArray.RemoveAtUninitialized(ref this, ref layout, index, count);
        }

        public void Destroy()
        {
            Native_FScriptSparseArray.Destroy(ref this);
            ZeroMemory();
        }

        private void ZeroMemory()
        {
            FMemory.Memzero(ref this);
        }

        public static FScriptSparseArrayLayout GetScriptLayout(int elementSize, int elementAlignment)
        {
            return Native_FScriptSparseArray.GetScriptLayout(elementSize, elementAlignment);
        }
    }

    /// <summary>
    /// Untyped bit array type for accessing TBitArray data, like FScriptArray for TArray.
    /// Must have the same memory representation as a TBitArray.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FScriptBitArray
    {
        FDefaultBitArrayAllocator AllocatorInstance;
        public int NumBits;
        public int MaxBits;

        public FBitReference this[int index]
        {
            get { return Native_FScriptBitArray.Get(ref this, index); }
        }

        public bool IsValidIndex(int index)
        {
            return Native_FScriptBitArray.IsValidIndex(ref this, index);
        }

        public void Empty(int slack = 0)
        {
            Native_FScriptBitArray.Empty(ref this, slack);
        }

        public int Add(bool value)
        {
            return Native_FScriptBitArray.Add(ref this, value);
        }

        public void Destroy()
        {
            Native_FScriptBitArray.Destroy(ref this);
        }
    }

    //FDefaultBitArrayAllocator = TInlineAllocator<4>
    //FDefaultBitArrayAllocator::ForElementType<uint32> = TInlineAllocator<4>::ForElementType<uint32>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FDefaultBitArrayAllocator
    {
        public fixed int InlineData[4];
        public IntPtr SecondaryData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FScriptSetLayout
    {
        //public int ElementOffset;// always at zero offset from the TSetElement - not stored here
        public int HashNextIdOffset;
        public int HashIndexOffset;
        public int Size;

        public FScriptSparseArrayLayout SparseArrayLayout;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FScriptSparseArrayLayout
    {
        //public int ElementOffset;// ElementOffset is at zero offset from the TSparseArrayElementOrFreeListLink - not stored here
        public int Alignment;
        public int Size;
    }

    /// <summary>
    /// Used to read/write a bit in the array as a bool.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FBitReference
    {
        public IntPtr Data;// uint32&
        public uint Mask;

        public bool Value
        {
            get { return Native_FBitReference.Get(ref this); }
            set { Native_FBitReference.Set(ref this, value); }
        }

        public void AtomicSet(bool value)
        {
            Native_FBitReference.AtomicSet(ref this, value);
        }
    }
}
