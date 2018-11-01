using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // A copy of the native FScriptSetHelper but using without using the VM functions
    // (using CopySingleValue instead of CopySingleValueToScriptVM as we aren't working with VM memory layout)
    // Engine\Source\Runtime\CoreUObject\Public\UObject\UnrealType.h

    /// <summary>
    /// Pseudo dynamic Set. Used to work with Set properties in a sensible way.
    /// </summary>
    public unsafe struct FScriptSetHelper
    {        
        private IntPtr setProperty;
        private FScriptSet* set;
        private FScriptSetLayout setLayout;

        private IntPtr elementProp;
        private int elementSize;
        private int elementArrayDim;

        public int Count
        {
            get { return set->Num(); }
        }

        public IntPtr PropertyAddress
        {
            get { return setProperty; }
        }
        public IntPtr ElementPropertyAddress
        {
            get { return elementProp; }
        }

        public IntPtr Set
        {
            get { return (IntPtr)set; }
            set { set = (FScriptSet*)value; }
        }

        public FScriptSetHelper(IntPtr setProperty, IntPtr set)
        {            
            this.setProperty = setProperty;
            this.set = (FScriptSet*)set;
            setLayout = Native_USetProperty.Get_SetLayout(setProperty);

            elementProp = Native_USetProperty.Get_ElementProp(setProperty);
            elementSize = Native_UProperty.Get_ElementSize(elementProp);
            elementArrayDim = Native_UProperty.Get_ArrayDim(elementProp);
        }

        public FScriptSetHelper(USetProperty property, IntPtr set)
            : this(property.Address, set)
        {
        }

        public FScriptSetHelper(IntPtr setProperty)
            : this(setProperty, IntPtr.Zero)
        {
        }

        public FScriptSetHelper(USetProperty property)
            : this(property.Address, IntPtr.Zero)
        {
        }

        public void Update(UFieldAddress property)
        {
            if (setProperty != property.Address)
            {
                setProperty = property.Address;
                elementProp = property.GenericArg1Address;
                elementSize = property.GenericArg1Size;
                elementArrayDim = property.GenericArg1ArrayDim;
            }
        }

        /// <summary>
        /// Index range check
        /// </summary>
        /// <param name="index">Index to check</param>
        /// <returns>true if accessing this element is legal.</returns>
        public bool IsValidIndex(int index)
        {
            return set->IsValidIndex(index);
        }

        /// <summary>
        /// Returns the number of elements in the set.
        /// </summary>
        /// <returns>The number of elements in the set.</returns>
        public int Num()
        {
            return set->Num();
        }

        /// <summary>
        /// Returns the (non-inclusive) maximum index of elements in the set.
        /// </summary>
        /// <returns>The (non-inclusive) maximum index of elements in the set.</returns>
        public int GetMaxIndex()
        {
            return set->GetMaxIndex();
        }

        /// <summary>
        /// Static version of Num() used when you don't need to bother to construct a FScriptSetHelper. Returns the number of elements in the set.
        /// </summary>
        /// <param name="target">Pointer to the raw memory associated with a FScriptSet</param>
        /// <returns>The number of elements in the set.</returns>
        public static int Num(IntPtr target)
        {
            return target == null ? 0 : ((FScriptSet*)target)->Num();
        }

        /// <summary>
        /// Returns a uint8 pointer to the element in the set.
        /// </summary>
        /// <param name="index">index of the item to return a pointer to.</param>
        /// <returns>Pointer to the element, or nullptr if the set is empty.</returns>
        public IntPtr GetElementPtr(int index)
        {
            if (Num() == 0)
            {
                return IntPtr.Zero;
            }

            return set->GetData(index, ref setLayout);
        }

        /// <summary>
        /// Add an uninitialized value to the end of the set.
        /// </summary>
        /// <returns>The index of the added element.</returns>
        public int AddUninitializedValue()
        {
            return set->AddUninitialized(ref setLayout);
        }

        /// <summary>
        /// Remove all values from the set, calling destructors, etc as appropriate.
        /// </summary>
        /// <param name="slack">used to presize the set for a subsequent add, to avoid reallocation.</param>
        public void EmptyValues(int slack = 0)
        {
            int oldNum = Num();
            if (oldNum != 0)
            {
                DestructItems(0, oldNum);
            }
            if (oldNum != 0 || slack != 0)
            {
                set->Empty(slack, ref setLayout);
            }
        }

        /// <summary>
        /// Adds a blank, constructed value to a given size.
        /// Note that this will create an invalid Set because all the keys will be default constructed, and the set needs rehashing.
        /// </summary>
        /// <returns>The index of the first element added.</returns>
        public int AddDefaultValue_Invalid_NeedsRehash()
        {
            int result = AddUninitializedValue();
            ConstructItem(result);
            return result;
        }

        /// <summary>
        /// Returns the property representing the element of the set
        /// </summary>
        public IntPtr GetElementPropertyPtr()
        {
            return elementProp;
        }

        /// <summary>
        /// Returns the property representing the element of the set
        /// </summary>
        public UProperty GetElementProperty()
        {
            return GCHelper.Find<UProperty>(elementProp);
        }

        /// <summary>
        /// Removes an element at the specified index, destroying it.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public void RemoveAt(int index, int count = 1)
        {
            DestructItems(index, count);
            for (; count != 0; ++index)
            {
                if (IsValidIndex(index))
                {
                    set->RemoveAt(index, ref setLayout);
                    --count;
                }
            }
        }

        /// <summary>
        /// Rehashes the keys in the set.
        /// This function must be called to create a valid set.
        /// </summary>
        public void Rehash()
        {
            IntPtr tempKeyProp = elementProp;
            HashDelegates.GetKeyHash callback = delegate (IntPtr src)
            {
                return Native_UProperty.GetValueTypeHash(tempKeyProp, src);
            };
            set->Rehash(ref setLayout, callback);
        }

        /// <summary>
        /// Finds the index of an element in a set
        /// </summary>
        /// <param name="elementToFind">The address of an element to search for.</param>
        /// <param name="indexHint">The index to start searching from.</param>
        /// <returns>The index of an element found in SetHelper, or -1 if none was found.The index of an element found in SetHelper, or -1 if none was found.</returns>
        public int FindElementIndex(IntPtr elementToFind, int indexHint = 0)
        {
            int setMax = GetMaxIndex();
            if (setMax == 0)
            {
                return -1;
            }

            IntPtr localKeyProp = elementProp;// prevent aliasing in loop below

            int index = indexHint;
            for (;;)
            {
                if (IsValidIndex(index))
                {
                    IntPtr elementToCheck = GetElementPtrWithoutCheck(index);
                    if (Native_UProperty.Identical(localKeyProp, elementToFind, elementToCheck, 0))
                    {
                        return index;
                    }
                }

                ++index;
                if (index == setMax)
                {
                    index = 0;
                }

                if (index == indexHint)
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Finds the pair in a set which matches the key in another pair.
        /// </summary>
        /// <param name="elementToFind">The address of an element to search for.</param>
        /// <param name="indexHint">The index to start searching from.</param>
        /// <returns>A pointer to the found pair, or nullptr if none was found.</returns>
        public IntPtr FindElementPtr(IntPtr elementToFind, int indexHint = 0)
        {
            int index = FindElementIndex(elementToFind, indexHint);
            IntPtr result = (index >= 0) ? GetElementPtr(index) : IntPtr.Zero;
            return result;
        }

        /// <summary>
        /// Finds element index from hash, rather than linearly searching
        /// </summary>
        public int FindElementIndexFromHash(IntPtr elementToFind)
        {
            IntPtr localElementPropForCapture = elementProp;
            HashDelegates.GetKeyHash elementHash = delegate (IntPtr elementKey)
            {
                return Native_UProperty.GetValueTypeHash(localElementPropForCapture, elementKey);
            };
            HashDelegates.Equality elementEquality = delegate (IntPtr a, IntPtr b)
            {
                return Native_UProperty.Identical(localElementPropForCapture, a, b, 0);
            };
            return set->FindIndex(elementToFind, ref setLayout, elementHash, elementEquality);
        }

        public int IndexOf<T>(T item, MarshalingDelegates<T>.ToNative toNative, UObject owner)
        {
            unsafe
            {
                byte* temp = stackalloc byte[elementSize * elementArrayDim];
                IntPtr tempPtr = (IntPtr)temp;
                Native_UProperty.InitializeValue(elementProp, tempPtr);
                toNative(tempPtr, 0, elementProp, item);

                int index = FindElementIndexFromHash(tempPtr);

                Native_UProperty.DestroyValue(elementProp, tempPtr);

                return index;
            }
        }

        public void AddElement<T>(T item, MarshalingDelegates<T>.ToNative toNative)
        {
            unsafe
            {
                byte* temp = stackalloc byte[elementSize * elementArrayDim];
                IntPtr tempPtr = (IntPtr)temp;
                Native_UProperty.InitializeValue(elementProp, tempPtr);
                toNative(tempPtr, 0, elementProp, item);

                AddElement(tempPtr);

                Native_UProperty.DestroyValue(elementProp, tempPtr);
            }
        }

        /// <summary>
        /// Adds the element to the set, returning true if the element was added, or false if the element was already present
        /// </summary>
        public void AddElement(IntPtr elementToAdd)
        {
            IntPtr localElementPropForCapture = elementProp;
            HashDelegates.GetKeyHash elementHash = delegate (IntPtr elementKey)
            {
                return Native_UProperty.GetValueTypeHash(localElementPropForCapture, elementKey);
            };
            HashDelegates.Equality elementEquality = delegate (IntPtr a, IntPtr b)
            {
                return Native_UProperty.Identical(localElementPropForCapture, a, b, 0);
            };
            HashDelegates.Construct elementConstruct = delegate (IntPtr newElement)
            {
                if (Native_UProperty.HasAnyPropertyFlags(localElementPropForCapture, EPropertyFlags.ZeroConstructor))
                {
                    FMemory.Memzero(newElement, Native_UProperty.GetSize(localElementPropForCapture));
                }
                else
                {
                    Native_UProperty.InitializeValue(localElementPropForCapture, newElement);
                }

                Native_UProperty.CopySingleValue(localElementPropForCapture, newElement, elementToAdd);
            };
            HashDelegates.Destruct elementDestruct = delegate (IntPtr element)
            {
                if (!Native_UProperty.HasAnyPropertyFlags(localElementPropForCapture, EPropertyFlags.IsPlainOldData | EPropertyFlags.NoDestructor))
                {
                    Native_UProperty.DestroyValue(localElementPropForCapture, element);
                }
            };
            set->Add(
                elementToAdd,
                ref setLayout,
                elementHash,
                elementEquality,
                elementConstruct,
                elementDestruct);
        }

        /// <summary>
        /// Removes the element from the set
        /// </summary>
        public bool RemoveElement(IntPtr elementToRemove)
        {
            IntPtr localElementPropForCapture = elementProp;
            HashDelegates.GetKeyHash elementHash = delegate (IntPtr elementKey)
            {
                return Native_UProperty.GetValueTypeHash(localElementPropForCapture, elementKey);
            };
            HashDelegates.Equality elementEquality = delegate (IntPtr a, IntPtr b)
            {
                return Native_UProperty.Identical(localElementPropForCapture, a, b, 0);
            };
            int foundIndex = set->FindIndex(elementToRemove, ref setLayout, elementHash, elementEquality);
            if (foundIndex != -1)
            {
                RemoveAt(foundIndex);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if an element has already been added to the set
        /// </summary>
        /// <param name="inBaseAddress">The base address of the set</param>
        /// <param name="inElementValue">The element value to check for</param>
        /// <returns>True if the element is found in the set, false otherwise</returns>
        public bool HasElement(IntPtr inBaseAddress, string inElementValue)
        {
            for (int index = 0, itemsLeft = Num(); itemsLeft > 0; ++index)
            {
                if (IsValidIndex(index))
                {
                    --itemsLeft;

                    IntPtr element = GetElementPtr(index);

                    using (FStringUnsafe keyValueUnsafe = new FStringUnsafe())
                    {
                        if (element != inBaseAddress && Native_UProperty.ExportText_Direct(elementProp, ref keyValueUnsafe.Array,
                                element, element, IntPtr.Zero, 0, IntPtr.Zero))
                        {
                            // Should this be case insensitive? (FString by default is case insensitive)
                            if ((Native_UObjectBaseUtility.IsA(elementProp, Classes.UObjectProperty) &&
                                keyValueUnsafe.Value.Contains(inElementValue)) || inElementValue == keyValueUnsafe.Value)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Internal function to call into the property system to construct / initialize elements.
        /// </summary>
        /// <param name="index">First item to construct.></param>
        private void ConstructItem(int index)
        {
            bool zeroElement = Native_UProperty.HasAnyPropertyFlags(elementProp, EPropertyFlags.ZeroConstructor);

            IntPtr dest = GetElementPtrWithoutCheck(index);

            if (zeroElement)
            {
                // If any nested property needs zeroing, just pre-zero the whole space
                FMemory.Memzero(dest, setLayout.Size);
            }

            if (!zeroElement)
            {
                Native_UProperty.InitializeValue_InContainer(elementProp, dest);
            }
        }

        /// <summary>
        /// Internal function to call into the property system to destruct elements.
        /// </summary>
        private void DestructItems(int index, int count)
        {
            if (count <= 0)
            {
                return;
            }

            bool destroyElements = !Native_UProperty.HasAnyPropertyFlags(elementProp, EPropertyFlags.IsPlainOldData | EPropertyFlags.NoDestructor);

            if (destroyElements)
            {
                int stride = setLayout.Size;
                IntPtr elementPtr = GetElementPtrWithoutCheck(index);

                for (; count != 0; ++index)
                {
                    if (IsValidIndex(index))
                    {
                        Native_UProperty.DestroyValue_InContainer(elementProp, elementPtr);
                        --count;
                    }
                    elementPtr += stride;
                }
            }
        }

        /// <summary>
        /// Returns a uint8 pointer to the element in the array without checking the index.
        /// </summary>
        /// <param name="index">index of the item to return a pointer to.</param>
        /// <returns>Pointer to the element, or nullptr if the array is empty.</returns>
        private IntPtr GetElementPtrWithoutCheck(int index)
        {
            return set->GetData(index, ref setLayout);
        }

        public static FScriptSetHelper CreateHelperFormInnerProperty(IntPtr elementProperty, IntPtr set)
        {
            FScriptSetHelper scriptSetHelper = new FScriptSetHelper();
            scriptSetHelper.elementProp = elementProperty;
            scriptSetHelper.set = (FScriptSet*)set;
            scriptSetHelper.setLayout = FScriptSet.GetScriptLayout(
                Native_UProperty.GetSize(elementProperty), Native_UProperty.GetMinAlignment(elementProperty));
            return scriptSetHelper;
        }

        public static FScriptSetHelper CreateHelperFormInnerProperty(UProperty elementProperty, IntPtr set)
        {
            return CreateHelperFormInnerProperty(elementProperty.Address, set);
        }
    }
}
