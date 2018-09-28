using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FScriptArray
    {
        public IntPtr Data;
        public int ArrayNum;
        public int ArrayMax;

        public int Count
        {
            get { return ArrayNum; }//return Native_FScriptArray.Num(ref this); }
        }

        public bool IsValidIndex(int index)
        {
            return Native_FScriptArray.IsValidIndex(ref this, index);
        }

        public void InsertZeroed(int index, int numBytesPerElement)
        {
            InsertZeroed(index, numBytesPerElement, 1);
        }

        public void InsertZeroed(int index, int numBytesPerElement, int count)
        {
            Native_FScriptArray.InsertZeroed(ref this, index, count, numBytesPerElement);
        }

        public void Insert(int index, int numBytesPerElement)
        {
            Insert(index, numBytesPerElement, 1);
        }

        public void Insert(int index, int numBytesPerElement, int count)
        {
            // Check if the index is beyond the count?
            Native_FScriptArray.Insert(ref this, index, count, numBytesPerElement);
        }

        public int Add(int numBytesPerElement)
        {
            return Add(numBytesPerElement, 1);
        }

        public int Add(int numBytesPerElement, int count)
        {
            return Native_FScriptArray.Add(ref this, count, numBytesPerElement);
        }

        public int AddZeroed(int numBytesPerElement)
        {
            return AddZeroed(numBytesPerElement, 1);
        }

        public int AddZeroed(int numBytesPerElement, int count)
        {
            return Native_FScriptArray.AddZeroed(ref this, count, numBytesPerElement);
        }

        public void Shrink(int numBytesPerElement)
        {
            Native_FScriptArray.Shrink(ref this, numBytesPerElement);
        }

        public void Empty(int slack, int numBytesPerElement)
        {
            Native_FScriptArray.Empty(ref this, slack, numBytesPerElement);
        }

        public void SwapMemory(int a, int b, int numBytesPerElement)
        {
            Native_FScriptArray.SwapMemory(ref this, a, b, numBytesPerElement);
        }

        public int GetSlack()
        {
            return Native_FScriptArray.GetSlack(ref this);
        }

        public void RemoveAt(int index, int numBytesPerElement)
        {
            RemoveAt(index, numBytesPerElement, 1);
        }

        public void RemoveAt(int index, int numBytesPerElement, int count)
        {
            Native_FScriptArray.Remove(ref this, index, count, numBytesPerElement);
        }

        public void Destroy()
        {
            Native_FScriptArray.Destroy(ref this);
            ZeroMemory();
        }

        public void Destroy(IntPtr property)
        {
            unsafe
            {
                fixed (FScriptArray* array = &this)
                {
                    Native_UProperty.DestroyValue(property, (IntPtr)array);
                }
            }
            ZeroMemory();
        }

        public void Destroy(UArrayProperty property)
        {
            Destroy(property.Address);
        }

        internal void ZeroMemory()
        {
            FMemory.Memzero(ref this);
        }
    }
}
