using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // A copy of the native FScriptMapHelper but using without using the VM functions
    // (using CopySingleValue instead of CopySingleValueToScriptVM as we aren't working with VM memory layout)
    // Engine\Source\Runtime\CoreUObject\Public\UObject\UnrealType.h

    /// <summary>
    /// Pseudo dynamic map. Used to work with map properties in a sensible way.
    /// </summary>
    public unsafe struct FScriptMapHelper
    {                
        private IntPtr mapProperty;
        private FScriptMap* map;
        private FScriptMapLayout mapLayout;

        private IntPtr keyProp;
        private int keySize;
        private int keyArrayDim;

        private IntPtr valueProp;
        private int valueSize;
        private int valueArrayDim;

        public int Count
        {
            get { return map->Num(); }
        }

        public IntPtr PropertyAddress
        {
            get { return mapProperty; }
        }
        public IntPtr KeyPropertyAddress
        {
            get { return keyProp; }
        }
        public IntPtr ValuePropertyAddress
        {
            get { return valueProp; }
        }

        public IntPtr Map
        {
            get { return (IntPtr)map; }
            set { map = (FScriptMap*)value; }
        }

        public FScriptMapHelper(IntPtr mapProperty, IntPtr map)
        {                        
            this.mapProperty = mapProperty;
            this.map = (FScriptMap*)map;
            mapLayout = Native_UMapProperty.Get_MapLayout(mapProperty);

            keyProp = Native_UMapProperty.Get_KeyProp(mapProperty);
            keySize = Native_UProperty.Get_ElementSize(keyProp);
            keyArrayDim = Native_UProperty.Get_ArrayDim(keyProp);

            valueProp = Native_UMapProperty.Get_ValueProp(mapProperty);
            valueSize = Native_UProperty.Get_ElementSize(valueProp);
            valueArrayDim = Native_UProperty.Get_ArrayDim(valueProp);
        }

        public FScriptMapHelper(UMapProperty property, IntPtr map)
            : this(property.Address, map)
        {
        }

        public FScriptMapHelper(IntPtr mapProperty)
            : this(mapProperty, IntPtr.Zero)
        {
        }

        public FScriptMapHelper(UMapProperty property)
            : this(property.Address, IntPtr.Zero)
        {
        }

        public void Update(UFieldAddress property)
        {
            if (mapProperty != property.Address)
            {
                mapProperty = property.Address;
                keyProp = property.GenericArg1Address;
                keySize = property.GenericArg1Size;
                keyArrayDim = property.GenericArg1ArrayDim;
                valueProp = property.GenericArg2Address;
                valueSize = property.GenericArg2Size;
                valueArrayDim = property.GenericArg2ArrayDim;
            }
        }

        /// <summary>
        /// Index range check
        /// </summary>
        /// <param name="index">Index to check</param>
        /// <returns>true if accessing this element is legal.</returns>
        public bool IsValidIndex(int index)
        {
            return map->IsValidIndex(index);
        }

            /// <summary>
            /// Returns the number of elements in the map.
            /// </summary>
            /// <returns>The number of elements in the map.</returns>
            public int Num()
            {
                return map->Num();
            }

        /// <summary>
        /// Returns the (non-inclusive) maximum index of elements in the map.
        /// </summary>
        /// <returns>The (non-inclusive) maximum index of elements in the map.</returns>
        public int GetMaxIndex()
        {
            return map->GetMaxIndex();
        }

        /// <summary>
        /// Static version of Num() used when you don't need to bother to construct a FScriptMapHelper. Returns the number of elements in the map.
        /// </summary>
        /// <param name="target">Pointer to the raw memory associated with a FScriptMap</param>
        /// <returns>The number of elements in the map.</returns>
        public static int Num(IntPtr target)
        {
            return target == null ? 0 : ((FScriptMap*)target)->Num();
        }

        /// <summary>
        /// Returns a uint8 pointer to the pair in the array
        /// </summary>
        /// <param name="index">index of the item to return a pointer to.</param>
        /// <returns>Pointer to the pair, or nullptr if the array is empty.</returns>
        public IntPtr GetPairPtr(int index)
        {
            if (Num() == 0)
            {
                return IntPtr.Zero;
            }
            
            return map->GetData(index, ref mapLayout);
        }

        public bool GetPairPtr(int index, out IntPtr keyPtr, out IntPtr valuePtr)
        {
            if (Num() == 0)
            {
                keyPtr = IntPtr.Zero;
                valuePtr = IntPtr.Zero;
                return false;
            }

            IntPtr pairPtr = map->GetData(index, ref mapLayout);
            keyPtr = pairPtr;// + mapLayout.KeyOffset;
            valuePtr = pairPtr + mapLayout.ValueOffset;
            return true;
        }

        /// <summary>
        /// Returns a uint8 pointer to the Key (first element) in the map. Currently 
        /// identical to GetPairPtr, but provides clarity of purpose and avoids exposing
        /// implementation details of TMap.
        /// </summary>
        /// <param name="index">index of the item to return a pointer to.</param>
        /// <returns>Pointer to the key, or nullptr if the map is empty.</returns>
        public IntPtr GetKeyPtr(int index)
        {
            if (Num() == 0)
            {
                return IntPtr.Zero;
            }

            return map->GetData(index, ref mapLayout);// + mapLayout.KeyOffset;
        }

        /// <summary>
        /// Returns a uint8 pointer to the Value (second element) in the map.
        /// </summary>
        /// <param name="index">index of the item to return a pointer to.</param>
        /// <returns>Pointer to the value, or nullptr if the map is empty.</returns>
        public IntPtr GetValuePtr(int index)
        {
            if (Num() == 0)
            {
                return IntPtr.Zero;
            }

            return map->GetData(index, ref mapLayout) + mapLayout.ValueOffset;
        }

        /// <summary>
        /// Add an uninitialized value to the end of the map.
        /// </summary>
        /// <returns>The index of the added element.</returns>
        public int AddUninitializedValue()
        {
            return map->AddUninitialized(ref mapLayout);
        }

        /// <summary>
        /// Remove all values from the map, calling destructors, etc as appropriate.
        /// </summary>
        /// <param name="slack">used to presize the array for a subsequent add, to avoid reallocation.</param>
        public void EmptyValues(int slack = 0)
        {
            int oldNum = Num();
            if (oldNum != 0)
            {
                DestructItems(0, oldNum);
            }
            if (oldNum != 0 || slack != 0)
            {
                map->Empty(slack, ref mapLayout);
            }
        }

        /// <summary>
        /// Adds a blank, constructed value to a given size.
        /// Note that this will create an invalid map because all the keys will be default constructed, and the map needs rehashing.
        /// </summary>
        /// <returns>The index of the first element added.</returns>
        public int AddDefaultValue_Invalid_NeedsRehash()
        {
            int result = AddUninitializedValue();
            ConstructItem(result);
            return result;
        }

        /// <summary>
        /// Returns the property representing the key of the map pair.
        /// </summary>
        /// <returns>The property representing the key of the map pair.</returns>
        public IntPtr GetKeyPropertyPtr()
        {
            return keyProp;
        }

        /// <summary>
        /// Returns the property representing the key of the map pair.
        /// </summary>
        /// <returns>The property representing the key of the map pair.</returns>
        public UProperty GetKeyProperty()
        {
            return GCHelper.Find<UProperty>(keyProp);
        }

        /// <summary>
        /// Returns the property representing the value of the map pair.
        /// </summary>
        /// <returns>The property representing the value of the map pair.</returns>
        public IntPtr GetValuePropertyPtr()
        {
            return valueProp;
        }

        /// <summary>
        /// Returns the property representing the value of the map pair.
        /// </summary>
        /// <returns>The property representing the value of the map pair.</returns>
        public UProperty GetValueProperty()
        {
            return GCHelper.Find<UProperty>(valueProp);
        }

        /// <summary>
        /// Removes an element at the specified index, destroying it.
        /// The map will be invalid until the next Rehash() call.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        /// <param name="count"></param>
        public void RemoveAt(int index, int count = 1)
        {
            DestructItems(index, count);
            for (; count != 0; ++index)
            {
                if (IsValidIndex(index))
                {
                    map->RemoveAt(index, ref mapLayout);
                    --count;
                }
            }
        }

        /// <summary>
        /// Rehashes the keys in the map.
        /// This function must be called to create a valid map.
        /// </summary>
        public void Rehash()
        {
            IntPtr tempKeyProp = keyProp;
            HashDelegates.GetKeyHash callback = delegate (IntPtr src)
            {
                return Native_UProperty.GetValueTypeHash(tempKeyProp, src);
            };
            map->Rehash(ref mapLayout, callback);
        }

        /// <summary>
        /// Finds the index of an element in a map which matches the key in another pair.
        /// </summary>
        /// <param name="pairWithKeyToFind">The address of a map pair which contains the key to search for.</param>
        /// <param name="indexHint">The index to start searching from.</param>
        /// <returns>The index of an element found in MapHelper, or -1 if none was found.</returns>
        public int FindMapIndexWithKey(IntPtr pairWithKeyToFind, int indexHint = 0)
        {
            int mapMax = GetMaxIndex();
            if (mapMax == 0)
            {
                return -1;
            }

            IntPtr localKeyProp = keyProp;// prevent aliasing in loop below

            int index = indexHint;
            for (;;)
            {
                if (IsValidIndex(index))
                {
                    IntPtr pairToSearch = GetPairPtrWithoutCheck(index);
                    if (Native_UProperty.Identical(localKeyProp, pairWithKeyToFind, pairToSearch, 0))
                    {
                        return index;
                    }
                }

                ++index;
                if (index == mapMax)
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
        /// Finds the pair in a map which matches the key in another pair.
        /// </summary>
        /// <param name="pairWithKeyToFind">The address of a map pair which contains the key to search for.</param>
        /// <param name="indexHint">The index to start searching from.</param>
        /// <returns>A pointer to the found pair, or nullptr if none was found.</returns>
        public IntPtr FindMapPairPtrWithKey(IntPtr pairWithKeyToFind, int indexHint = 0)
        {
            int index = FindMapIndexWithKey(pairWithKeyToFind, indexHint);
            IntPtr result = (index >= 0) ? GetPairPtr(index) : IntPtr.Zero;
            return result;
        }

        /// <summary>
        /// Finds the associated value from hash, rather than linearly searching
        /// </summary>
        public IntPtr FindValueFromHash(IntPtr keyPtr)
        {
            IntPtr localKeyPropForCapture = keyProp;
            HashDelegates.GetKeyHash keyHash = delegate(IntPtr elementKey)
            {
                return Native_UProperty.GetValueTypeHash(localKeyPropForCapture, elementKey);
            };
            HashDelegates.Equality keyEquality = delegate(IntPtr a, IntPtr b)
            {
                return Native_UProperty.Identical(localKeyPropForCapture, a, b, 0);
            };
            return map->FindValue(keyPtr, ref mapLayout, keyHash, keyEquality);
        }

        /// <summary>
        /// Finds key index from hash, rather than linearly searching
        /// </summary>
        public int FindPairIndexFromHash(IntPtr keyToFind)
        {
            IntPtr localKeyPropForCapture = keyProp;
            HashDelegates.GetKeyHash keyHash = delegate (IntPtr elementKey)
            {
                return Native_UProperty.GetValueTypeHash(localKeyPropForCapture, elementKey);
            };
            HashDelegates.Equality keyEquality = delegate (IntPtr a, IntPtr b)
            {
                return Native_UProperty.Identical(localKeyPropForCapture, a, b, 0);
            };
            return map->FindPairIndex(keyToFind, ref mapLayout, keyHash, keyEquality);
        }

        public int FindPairIndex<TKey>(TKey key, MarshalingDelegates<TKey>.ToNative keyToNative, UObject owner)
        {
            unsafe
            {
                byte* tempKey = stackalloc byte[keySize * keyArrayDim];
                IntPtr tempKeyPtr = (IntPtr)tempKey;
                Native_UProperty.InitializeValue(keyProp, tempKeyPtr);
                keyToNative(tempKeyPtr, 0, keyProp, key);

                int index = FindPairIndexFromHash(tempKeyPtr);

                Native_UProperty.DestroyValue(keyProp, tempKeyPtr);

                return index;
            }
        }

        public void AddPair<TKey, TValue>(TKey key, TValue value, MarshalingDelegates<TKey>.ToNative keyToNative,
            MarshalingDelegates<TValue>.ToNative valueToNative)
        {
            unsafe
            {
                byte* tempKey = stackalloc byte[keySize * keyArrayDim];
                IntPtr tempKeyPtr = (IntPtr)tempKey;
                Native_UProperty.InitializeValue(keyProp, tempKeyPtr);
                keyToNative(tempKeyPtr, 0, keyProp, key);

                byte* tempValue = stackalloc byte[valueSize * valueArrayDim];                
                IntPtr tempValuePtr = (IntPtr)tempValue;
                Native_UProperty.InitializeValue(valueProp, tempValuePtr);
                valueToNative(tempValuePtr, 0, valueProp, value);

                AddPair(tempKeyPtr, tempValuePtr);

                Native_UProperty.DestroyValue(keyProp, tempKeyPtr);
                Native_UProperty.DestroyValue(valueProp, tempValuePtr);
            }
        }

        public void AddPair(IntPtr keyPtr, IntPtr valuePtr)
        {
            IntPtr localKeyPropForCapture = keyProp;
            IntPtr localValuePropForCapture = valueProp;
            HashDelegates.GetKeyHash keyHash = delegate(IntPtr elementKey)
            {
                return Native_UProperty.GetValueTypeHash(localKeyPropForCapture, elementKey);
            };
            HashDelegates.Equality keyEquality = delegate (IntPtr a, IntPtr b)
            {
                return Native_UProperty.Identical(localKeyPropForCapture, a, b, 0);
            };
            HashDelegates.ConstructAndAssign keyConstructAndAssign = delegate(IntPtr newElementKey)
            {
                if (Native_UProperty.HasAnyPropertyFlags(localKeyPropForCapture, EPropertyFlags.ZeroConstructor))
                {
                    FMemory.Memzero(newElementKey, Native_UProperty.GetSize(localKeyPropForCapture));
                }
                else
                {
                    Native_UProperty.InitializeValue(localKeyPropForCapture, newElementKey);
                }

                Native_UProperty.CopySingleValue(localKeyPropForCapture, newElementKey, keyPtr);
            };
            HashDelegates.ConstructAndAssign valueConstructAndAssign = delegate (IntPtr newElementValue)
            {
                if (Native_UProperty.HasAnyPropertyFlags(localValuePropForCapture, EPropertyFlags.ZeroConstructor))
                {
                    FMemory.Memzero(newElementValue, Native_UProperty.GetSize(localValuePropForCapture));
                }
                else
                {
                    Native_UProperty.InitializeValue(localValuePropForCapture, newElementValue);
                }

                Native_UProperty.CopySingleValue(localValuePropForCapture, newElementValue, valuePtr);
            };
            HashDelegates.Assign valueAssign = delegate(IntPtr existingElementValue)
            {
                Native_UProperty.CopySingleValue(localValuePropForCapture, existingElementValue, valuePtr);
            };
            HashDelegates.Destruct keyDestruct = delegate(IntPtr elementKey)
            {
                if (!Native_UProperty.HasAnyPropertyFlags(localKeyPropForCapture, EPropertyFlags.IsPlainOldData | EPropertyFlags.NoDestructor))
                {
                    Native_UProperty.DestroyValue(localKeyPropForCapture, elementKey);
                }
            };
            HashDelegates.Destruct valueDestruct = delegate(IntPtr elementValue)
            {
                if (!Native_UProperty.HasAnyPropertyFlags(localValuePropForCapture, EPropertyFlags.IsPlainOldData | EPropertyFlags.NoDestructor))
                {
                    Native_UProperty.DestroyValue(localValuePropForCapture, elementValue);
                }
            };
            map->Add(keyPtr, valuePtr, ref mapLayout, keyHash, keyEquality, keyConstructAndAssign, valueConstructAndAssign,
                valueAssign, keyDestruct, valueDestruct);
        }

        /// <summary>
        /// Removes the key and its associated value from the map
        /// </summary>
        public bool RemovePair(IntPtr keyPtr)
        {
            IntPtr localKeyPropForCapture = keyProp;
            HashDelegates.GetKeyHash keyHash = delegate (IntPtr elementKey)
            {
                return Native_UProperty.GetValueTypeHash(localKeyPropForCapture, elementKey);
            };
            HashDelegates.Equality keyEquality = delegate (IntPtr a, IntPtr b)
            {
                return Native_UProperty.Identical(localKeyPropForCapture, a, b, 0);
            };
            IntPtr entry = map->FindValue(keyPtr, ref mapLayout, keyHash, keyEquality);
            if (entry != IntPtr.Zero)
            {
                int idx = (int)((entry.ToInt64() - map->GetData(0, ref mapLayout).ToInt64()) / mapLayout.SetLayout.Size);
                RemoveAt(idx);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a key in the map matches the specified key
        /// </summary>
        /// <param name="inBaseAddress">The base address of the map</param>
        /// <param name="inKeyValue">The key to find within the map</param>
        /// <returns>True if the key is found, false otherwise</returns>
        public bool HasKey(IntPtr inBaseAddress, string inKeyValue)
        {
            for (int index = 0, itemsLeft = Num(); itemsLeft > 0; ++index)
            {
                if (IsValidIndex(index))
                {
                    --itemsLeft;

                    IntPtr pairPtr = GetPairPtr(index);
                    IntPtr keyPtr = Native_UProperty.ContainerVoidPtrToValuePtr(keyProp, pairPtr, 0);

                    using (FStringUnsafe keyValueUnsafe = new FStringUnsafe())
                    {
                        if (keyPtr != inBaseAddress && Native_UProperty.ExportText_Direct(keyProp, ref keyValueUnsafe.Array,
                                keyPtr, keyPtr, IntPtr.Zero, 0, IntPtr.Zero))
                        {
                            // Should this be case insensitive? (FString by default is case insensitive)
                            if ((Native_UObjectBaseUtility.IsA(keyProp, Classes.UObjectProperty) &&
                                keyValueUnsafe.Value.Contains(inKeyValue)) || inKeyValue == keyValueUnsafe.Value)
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
        /// <param name="index">First item to construct.</param>
        private void ConstructItem(int index)
        {
            bool zeroKey = Native_UProperty.HasAnyPropertyFlags(keyProp, EPropertyFlags.ZeroConstructor);
            bool zeroValue = Native_UProperty.HasAnyPropertyFlags(valueProp, EPropertyFlags.ZeroConstructor);

            IntPtr dest = GetPairPtrWithoutCheck(index);

            if (zeroKey || zeroValue)
            {
                // If any nested property needs zeroing, just pre-zero the whole space
                FMemory.Memzero(dest, mapLayout.SetLayout.Size);
            }

            if (!zeroKey)
            {
                Native_UProperty.InitializeValue_InContainer(keyProp, dest);
            }

            if (!zeroValue)
            {
                Native_UProperty.InitializeValue_InContainer(valueProp, dest);
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

            bool destroyKeys = !Native_UProperty.HasAnyPropertyFlags(keyProp, EPropertyFlags.IsPlainOldData | EPropertyFlags.NoDestructor);
            bool destroyValues = !Native_UProperty.HasAnyPropertyFlags(valueProp, EPropertyFlags.IsPlainOldData | EPropertyFlags.NoDestructor);

            if (destroyKeys || destroyValues)
            {
                int stride = mapLayout.SetLayout.Size;
                IntPtr pairPtr = GetPairPtr(index);
                if (destroyKeys)
                {
                    if (destroyValues)
                    {
                        for (; count != 0; ++index)
                        {
                            if (IsValidIndex(index))
                            {
                                Native_UProperty.DestroyValue_InContainer(keyProp, pairPtr);
                                Native_UProperty.DestroyValue_InContainer(valueProp, pairPtr);
                                --count;
                            }
                            pairPtr += stride;
                        }
                    }
                    else
                    {
                        for (; count != 0; ++index)
                        {
                            if (IsValidIndex(index))
                            {
                                Native_UProperty.DestroyValue_InContainer(keyProp, pairPtr);
                                --count;
                            }
                            pairPtr += stride;
                        }
                    }
                }
                else
                {
                    for (; count != 0; ++index)
                    {
                        if (IsValidIndex(index))
                        {
                            Native_UProperty.DestroyValue_InContainer(valueProp, pairPtr);
                            --count;
                        }
                        pairPtr += stride;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a uint8 pointer to the pair in the array without checking the index.
        /// </summary>
        /// <param name="index">index of the item to return a pointer to.</param>
        /// <returns>Pointer to the pair, or nullptr if the array is empty.</returns>
        private IntPtr GetPairPtrWithoutCheck(int index)
        {
            return map->GetData(index, ref mapLayout);
        }

        public static FScriptMapHelper CreateHelperFormInnerProperty(IntPtr keyProperty, IntPtr valProperty, IntPtr map)
        {
            FScriptMapHelper scriptMapHelper = new FScriptMapHelper();
            scriptMapHelper.keyProp = keyProperty;
            scriptMapHelper.valueProp = valProperty;
            scriptMapHelper.map = (FScriptMap*)map;
            scriptMapHelper.mapLayout = FScriptMap.GetScriptLayout(
                Native_UProperty.GetSize(keyProperty), Native_UProperty.GetMinAlignment(keyProperty),
                Native_UProperty.GetSize(valProperty), Native_UProperty.GetMinAlignment(valProperty));
            return scriptMapHelper;
        }

        public static FScriptMapHelper CreateHelperFormInnerProperty(UProperty keyProperty, UProperty valProperty, IntPtr array)
        {
            return CreateHelperFormInnerProperty(keyProperty.Address, valProperty.Address, array);
        }
    }
}
