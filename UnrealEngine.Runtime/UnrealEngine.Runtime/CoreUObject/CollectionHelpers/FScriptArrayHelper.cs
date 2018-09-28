using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // A copy of the native FScriptMapHelper
    // Engine\Source\Runtime\CoreUObject\Public\UObject\UnrealType.h

    /// <summary>
    /// FScriptArrayHelper: Pseudo dynamic array. Used to work with array properties in a sensible way.
    /// </summary>
    public unsafe struct FScriptArrayHelper
    {
        private IntPtr innerProperty;
        private IntPtr arrayProperty;
        private FScriptArray* array;
        private int elementSize;

        public int Count
        {
            get { return array->ArrayNum; }
        }

        public IntPtr PropertyAddress
        {
            get { return arrayProperty; }
        }
        public IntPtr InnerPropertyAddress
        {
            get { return innerProperty; }
        }

        public IntPtr Array
        {
            get { return (IntPtr)array; }
            set { array = (FScriptArray*)value; }
        }

        public FScriptArrayHelper(IntPtr arrayProperty, IntPtr array)
        {
            innerProperty = Native_UArrayProperty.Get_Inner(arrayProperty);
            this.arrayProperty = arrayProperty;
            this.array = (FScriptArray*)array;
            elementSize = Native_UProperty.Get_ElementSize(innerProperty);
        }

        public FScriptArrayHelper(UArrayProperty property, IntPtr array)
            : this(property.Address, array)
        {
        }

        public FScriptArrayHelper(IntPtr arrayProperty)
            : this(arrayProperty, IntPtr.Zero)
        {
        }

        public FScriptArrayHelper(UArrayProperty property)
            : this(property, IntPtr.Zero)
        {
        }

        public void Update(UFieldAddress property)
        {
            if (arrayProperty != property.Address)
            {
                arrayProperty = property.Address;
                innerProperty = property.GenericArg1Address;
                elementSize = property.GenericArg1Size;
            }
        }

        /// <summary>
        /// Index range check
        /// </summary>
        /// <param name="index">Index to check</param>
        /// <returns>true if accessing this element is legal.</returns>
        public bool IsValidIndex(int index)
        {
            return index >= 0 && index <= Num();
        }

        /// <summary>
        /// Return the number of elements in the array.
        /// </summary>
        /// <returns>The number of elements in the array.</returns>
        public int Num()
        {
            return Count;
        }

        /// <summary>
        /// Static version of Num() used when you don't need to bother to construct a FScriptArrayHelper. Returns the number of elements in the array.
        /// </summary>
        /// <param name="target">pointer to the raw memory associated with a FScriptArray</param>
        /// <returns>The number of elements in the array.</returns>
        public static int Num(IntPtr target)
        {
            return target == null ? 0 : ((FScriptArray*)target)->ArrayNum;
        }

        /// <summary>
        /// Returns a uint8 pointer to an element in the array
        /// </summary>
        /// <param name="index">index of the item to return a pointer to.</param>
        /// <returns>Pointer to this element, or NULL if the array is empty</returns>
        public IntPtr GetRawPtr(int index = 0)
        {
            if (Count == 0)
            {
                return IntPtr.Zero;
            }
            return array->Data + (index * elementSize);
        }

        /// <summary>
        /// Empty the array, then add blank, constructed values to a given size.
        /// </summary>
        /// <param name="count">the number of items the array will have on completion.</param>
        public void EmptyAndAddValues(int count)
        {
            EmptyValues(count);
            if (count > 0)
            {
                AddValues(count);
            }
        }

        /// <summary>
        /// Empty the array, then add uninitialized values to a given size.
        /// </summary>
        /// <param name="count">the number of items the array will have on completion.</param>
        public void EmptyAndAddUninitializedValues(int count)
        {
            EmptyValues(count);
            if (count > 0)
            {
                AddUninitializedValues(count);
            }
        }

        /// <summary>
        /// Empty the array, then add zeroed values to a given size.
        /// </summary>
        /// <param name="count">the number of items the array will have on completion.</param>
        public void EmptyAndAddZeroedValues(int count)
        {
            EmptyValues(count);
            if (count > 0)
            {
                AddZeroedValues(count);
            }
        }

        /// <summary>
        /// Expand the array, if needed, so that the given index is valid.
        /// 
        /// NOTE: This is not a count, it is an INDEX, so the final count will be at least Index+1 this matches the usage.
        /// </summary>
        /// <param name="index">index for the item that we want to ensure is valid</param>
        /// <returns>true if expansion was necessary</returns>
        public bool ExpandForIndex(int index)
        {
            if (index >= Num())
            {
                AddValues(index - Num() + 1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add or remove elements to set the array to a given size.
        /// </summary>
        /// <param name="count">the number of items the array will have on completion.</param>
        public void Resize(int count)
        {
            int oldNum = Num();
            if (count > oldNum)
            {
                AddValues(count - oldNum);
            }
            else if (count < oldNum)
            {
                RemoveValues(count, oldNum - count);
            }
        }

        /// <summary>
        /// Add blank, constructed values to the end of the array.
        /// </summary>
        /// <param name="count">the number of items to insert.</param>
        /// <returns>the index of the first newly added item.</returns>
        public int AddValues(int count)
        {
            int oldNum = AddUninitializedValues(count);
            ConstructItems(oldNum, count);
            return oldNum;
        }

        /// <summary>
        /// Add a blank, constructed values to the end of the array.
        /// </summary>
        /// <returns>the index of the newly added item.</returns>
        public int AddValue()
        {
            return AddValues(1);
        }

        /// <summary>
        /// Add uninitialized values to the end of the array.
        /// </summary>
        /// <param name="count">the number of items to insert.</param>
        /// <returns>the index of the first newly added item.</returns>
        public int AddUninitializedValues(int count)
        {
            int oldNum = array->Add(elementSize, count);
            return oldNum;
        }

        /// <summary>
        /// Add an uninitialized value to the end of the array.
        /// </summary>
        /// <returns>the index of the newly added item.</returns>
        public int AddUninitializedValue()
        {
            return AddUninitializedValues(1);
        }

        /// <summary>
        /// Adds zeroed values to the end of the array (calls FMemory.Memzero on the memory range).
        /// </summary>
        /// <param name="count">The number of items to insert.</param>
        /// <returns>the indnex of the first newly added item</returns>
        public int AddZeroedValues(int count)
        {
            int oldNum = array->Add(elementSize, count);
            IntPtr dest = GetRawPtr();
            FMemory.Memzero(dest, count * elementSize);
            return oldNum;
        }

        /// <summary>
        /// Insert blank, constructed values into the array.
        /// </summary>
        /// <param name="index">index of the first inserted item after completion</param>
        /// <param name="count">the number of items to insert.</param>
        public void InsertValues(int index, int count = 1)
        {
            array->Insert(index, elementSize, count);
            ConstructItems(index, count);
        }

        /// <summary>
        /// Remove all values from the array, calling destructors, etc as appropriate.
        /// </summary>
        /// <param name="slack">used to presize the array for a subsequent add, to avoid reallocation.</param>
        public void EmptyValues(int slack = 0)
        {
            int oldNum = Num();
            if (oldNum > 0)
            {
                DestructItems(0, oldNum);
            }
            if (oldNum > 0 || slack > 0)
            {
                array->Empty(slack, elementSize);
            }
        }

        /// <summary>
        /// Remove values from the array, calling destructors, etc as appropriate.
        /// </summary>
        /// <param name="index">first item to remove.</param>
        /// <param name="count">number of items to remove.</param>
        public void RemoveValues(int index, int count = 1)
        {
            DestructItems(index, count);
            array->RemoveAt(index, elementSize, count);
        }

        /// <summary>
        /// Clear values in the array. The meaning of clear is defined by the property system.
        /// </summary>
        /// <param name="index">first item to clear.</param>
        /// <param name="count">number of items to clear.</param>
        public void ClearValues(int index, int count = 1)
        {
            ClearItems(index, count);
        }

        /// <summary>
        /// Swap two elements in the array, does not call constructors and destructors
        /// </summary>
        /// <param name="a">index of one item to swap.</param>
        /// <param name="b">index of the other item to swap.</param>
        public void SwapValues(int a, int b)
        {
            array->SwapMemory(a, b, elementSize);
        }

        /// <summary>
        /// Internal function to call into the property system to construct / initialize elements.
        /// </summary>
        /// <param name="index">first item to .</param>
        /// <param name="count">number of items to .</param>
        private void ConstructItems(int index, int count)
        {
            IntPtr dest = GetRawPtr(index);
            if (Native_UProperty.HasAnyPropertyFlags(innerProperty, EPropertyFlags.ZeroConstructor))
            {
                FMemory.Memzero(dest, count * elementSize);
            }
            else
            {
                for (int i = 0; i < count; i++, dest += elementSize)
                {
                    Native_UProperty.InitializeValue(innerProperty, dest);
                }
            }
        }

        /// <summary>
        /// Internal function to call into the property system to destruct elements.
        /// </summary>
        /// <param name="index">first item to .</param>
        /// <param name="count">number of items to .</param>
        private void DestructItems(int index, int count)
        {
            if (!Native_UProperty.HasAnyPropertyFlags(innerProperty, EPropertyFlags.IsPlainOldData | EPropertyFlags.NoDestructor))
            {
                IntPtr dest = GetRawPtr(index);
                for (int i = 0; i < count; i++, dest += elementSize)
                {
                    Native_UProperty.DestroyValue(innerProperty, dest);
                }
            }
        }

        private void ClearItems(int index, int count)
        {
            IntPtr dest = GetRawPtr(index);
            if (Native_UProperty.HasAnyPropertyFlags(innerProperty, EPropertyFlags.ZeroConstructor | EPropertyFlags.NoDestructor))
            {
                FMemory.Memzero(dest, count * elementSize);
            }
            else
            {
                for (int i = 0; i < count; i++, dest += elementSize)
                {
                    Native_UProperty.ClearValue(innerProperty, dest);
                }
            }
        }

        public static FScriptArrayHelper CreateHelperFormInnerProperty(IntPtr innerProperty, IntPtr array)
        {
            FScriptArrayHelper scriptArrayHelper = new FScriptArrayHelper();
            scriptArrayHelper.innerProperty = innerProperty;
            scriptArrayHelper.array = (FScriptArray*)array;
            scriptArrayHelper.elementSize = Native_UProperty.Get_ElementSize(innerProperty);
            return scriptArrayHelper;
        }

        public static FScriptArrayHelper CreateHelperFormInnerProperty(UProperty innerProperty, IntPtr array)
        {
            return CreateHelperFormInnerProperty(innerProperty.Address, array);
        }
    }
}
