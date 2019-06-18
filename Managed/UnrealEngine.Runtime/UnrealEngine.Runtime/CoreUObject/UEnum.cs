using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Reflection data for an enumeration.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.Enum")]
    public partial class UEnum : UField
    {
        /// <summary>
        /// This will be the true type of the enum as a string, e.g. "ENamespacedEnum::InnerType" or "ERegularEnum" or "EEnumClass"
        /// </summary>
        public string CppType
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_UEnum.Get_CppType(Address, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }                    
            set
            {
                using (FStringUnsafe cppTypeUnsafe = new FStringUnsafe(value))
                {
                    Native_UEnum.Set_CppType(Address, ref cppTypeUnsafe.Array);
                }
            }
        }

        /// <summary>
        /// Gets the internal index for an enum value. Returns INDEX_NONE if not valid
        /// </summary>
        public int GetIndexByValue(long value)
        {
            return Native_UEnum.GetIndexByValue(Address, value);
        }

        /// <summary>
        /// Gets enum value by index in Names array. Assets on invalid index
        /// </summary>
        public long GetValueByIndex(int index)
        {
            return Native_UEnum.GetValueByIndex(Address, index);
        }

        /// <summary>
        /// Gets enum name by index in Names array. Returns NAME_None if Index is not valid.
        /// </summary>
        public FName GetNameByIndex(int index)
        {
            FName result;
            Native_UEnum.GetNameByIndex(Address, index, out result);
            return result;
        }

        /// <summary>
        /// Gets index of name in enum, returns INDEX_NONE and optionally errors when name is not found. This is faster than ByNameString if the FName is exact, but will fall back if needed
        /// </summary>
        public int GetIndexByName(FName name, EGetByNameFlags flags = EGetByNameFlags.None)
        {
            return Native_UEnum.GetIndexByName(Address, ref name, flags);
        }

        /// <summary>
        /// Gets enum name by value. Returns NAME_None if value is not found.
        /// </summary>
        public FName GetNameByValue(long value)
        {
            FName result;
            Native_UEnum.GetNameByValue(Address, value, out result);
            return result;
        }

        /// <summary>
        /// Gets enum value by name. Returns INDEX_NONE when name is not found.
        /// </summary>
        public int GetValueByName(FName name, EGetByNameFlags flags = EGetByNameFlags.None)
        {
            return Native_UEnum.GetValueByName(Address, ref name, flags);
        }

        /// <summary>
        /// Returns the short name at the enum index, returns empty string if invalid
        /// </summary>
        public string GetNameStringByIndex(int index)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UEnum.GetNameStringByIndex(Address, index, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets index of name in enum, returns INDEX_NONE and optionally errors when name is not found. Handles full or short names.
        /// </summary>
        public int GetIndexByNameString(string searchString, EGetByNameFlags flags = EGetByNameFlags.None)
        {
            using (FStringUnsafe searchStringUnsafe = new FStringUnsafe(searchString))
            {
                return Native_UEnum.GetIndexByNameString(Address, ref searchStringUnsafe.Array, flags);
            }
        }

        /// <summary>
        /// Returns the short name matching the enum Value, returns empty string if invalid
        /// </summary>
        public string GetNameStringByValue(long value)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UEnum.GetNameStringByValue(Address, value, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets enum value by name, returns INDEX_NONE and optionally errors when name is not found. Handles full or short names
        /// </summary>
        public long GetValueByNameString(string searchString, EGetByNameFlags flags = EGetByNameFlags.None)
        {
            using (FStringUnsafe searchStringUnsafe = new FStringUnsafe(searchString))
            {
                return Native_UEnum.GetValueByNameString(Address, ref searchStringUnsafe.Array, flags);
            }
        }

        /// <summary>
        /// Finds the localized display name or native display name as a fallback.
        /// If called from a cooked build this will normally return the short name as Metadata is not available.
        /// </summary>
        /// <param name="index">Index of the enum value to get Display Name for</param>
        /// <returns>The display name for this object, or an empty text if Index is invalid</returns>
        public string GetDisplayNameTextStringByIndex(int index)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UEnum.GetDisplayNameTextStringByIndex(Address, index, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Version of GetDisplayNameTextByIndex that takes a value instead
        /// </summary>
        public string GetDisplayNameTextStringByValue(long value)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UEnum.GetDisplayNameTextStringByValue(Address, value, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets max value of Enum. Defaults to zero if there are no entries.
        /// </summary>
        public long GetMaxEnumValue()
        {
            return Native_UEnum.GetMaxEnumValue(Address);
        }

        /// <summary>
        /// Checks if enum has entry with given value. Includes autogenerated _MAX entry.
        /// </summary>
        public bool IsValidEnumValue(long value)
        {
            return Native_UEnum.IsValidEnumValue(Address, value);
        }

        /// <summary>
        /// Checks if enum has entry with given name. Includes autogenerated _MAX entry.
        /// </summary>
        public bool IsValidEnumName(FName name)
        {
            return Native_UEnum.IsValidEnumName(Address, ref name);
        }

        /// <summary>
        /// Removes the Names in this enum from the master AllEnumNames list
        /// </summary>
        public void RemoveNamesFromMasterList()
        {
            Native_UEnum.RemoveNamesFromMasterList(Address);
        }

        /// <summary>
        /// Returns the type of enum: whether it's a regular enum, namespaced enum or C++11 enum class.
        /// </summary>
        /// <returns>The enum type.</returns>
        public ECppForm GetCppForm()
        {
            return Native_UEnum.GetCppForm(Address);
        }

        /// <summary>
        /// Checks if a enum name is fully qualified name.
        /// </summary>
        /// <param name="inEnumName">Name to check.</param>
        /// <returns>true if the specified name is full enum name, false otherwise.</returns>
        public static bool IsFullEnumName(string inEnumName)
        {
            using (FStringUnsafe inEnumNameUnsafe = new FStringUnsafe(inEnumName))
            {
                return Native_UEnum.IsFullEnumName(ref inEnumNameUnsafe.Array);
            }
        }

        /// <summary>
        /// Generates full enum name give enum name.
        /// </summary>
        /// <param name="inEnumName">Enum name.</param>
        /// <returns>Full enum name.</returns>
        public string GenerateFullEnumName(string inEnumName)
        {
            using (FStringUnsafe inEnumNameUnsafe = new FStringUnsafe(inEnumName))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UEnum.GenerateFullEnumName(Address, ref inEnumNameUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Sets the array of enums.
        /// </summary>
        /// <param name="names">List of enum names.</param>
        /// <param name="inCppForm">The form of enum.</param>
        /// <param name="addMaxKeyIfMissing">Should a default Max item be added.</param>
        /// <returns>true unless the MAX enum already exists and isn't the last enum.</returns>
        public bool SetEnums(Dictionary<FName, long> names, UEnum.ECppForm inCppForm, bool addMaxKeyIfMissing = true)
        {
            using (TArrayUnsafe<FName> namesUnsafe = new TArrayUnsafe<FName>())
            using (TArrayUnsafe<long> valuesUnsafe = new TArrayUnsafe<long>())
            {
                namesUnsafe.AddRange(names.Keys.ToArray());
                valuesUnsafe.AddRange(names.Values.ToArray());
                return Native_UEnum.SetEnums(Address, namesUnsafe.Address, valuesUnsafe.Address, inCppForm, addMaxKeyIfMissing);
            }
        }

        /// <summary>
        /// Returns the number of enum names.
        /// </summary>
        /// <returns>The number of enum names.</returns>
        public int NumEnums()
        {
            return Native_UEnum.NumEnums(Address);
        }

        /// <summary>
        /// Find the longest common prefix of all items in the enumeration.
        /// </summary>
        /// <returns>the longest common prefix between all items in the enum.  If a common prefix
        /// cannot be found, returns the full name of the enum.</returns>
        public string GenerateEnumPrefix()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UEnum.GenerateEnumPrefix(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Finds the localized tooltip or native tooltip as a fallback.
        /// </summary>
        /// <param name="nameIndex">Index of the enum value to get tooltip for</param>
        /// <returns>The tooltip for this object.</returns>
        public string GetToolTipByIndex(int nameIndex)
        {
            // WITH_EDITOR
            if (Native_UEnum.GetToolTipByIndex == null)
            {
                return null;
            }

            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UEnum.GetToolTipByIndex(Address, nameIndex, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Wrapper method for easily determining whether this enum has metadata associated with it.
        /// </summary>
        /// <param name="key">the metadata tag to check for</param>
        /// <param name="nameIndex">if specified, will search for metadata linked to a specified value in this enum; otherwise, searches for metadata for the enum itself</param>
        /// <returns>true if the specified key exists in the list of metadata for this enum, even if the value of that key is empty</returns>
        public bool HasMetaData(string key, int nameIndex = -1)
        {
            // WITH_EDITOR
            if (Native_UEnum.HasMetaData == null)
            {
                return false;
            }

            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            {
                return Native_UEnum.HasMetaData(Address, ref keyUnsafe.Array, nameIndex);
            }
        }

        /// <summary>
        /// Return the metadata value associated with the specified key.
        /// </summary>
        /// <param name="key">the metadata tag to find the value for</param>
        /// <param name="nameIndex">if specified, will search the metadata linked for that enum value; otherwise, searches the metadata for the enum itself</param>
        /// <returns>the value for the key specified, or an empty string if the key wasn't found or had no value.</returns>
        public string GetMetaData(string key, int nameIndex = -1)
        {
            // WITH_EDITOR
            if (Native_UEnum.GetMetaData == null)
            {
                return null;
            }

            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UEnum.GetMetaData(Address, ref keyUnsafe.Array, nameIndex, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Set the metadata value associated with the specified key.
        /// </summary>
        /// <param name="key">the metadata tag to find the value for</param>
        /// <param name="value">if specified, will search the metadata linked for that enum value; otherwise, searches the metadata for the enum itself</param>
        /// <param name="nameIndex">Value of the metadata for the key</param>
        public void SetMetaData(string key, string value, int nameIndex = -1)
        {
            // WITH_EDITOR
            if (Native_UEnum.SetMetaData == null)
            {
                return;
            }

            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
            {
                Native_UEnum.SetMetaData(Address, ref keyUnsafe.Array, ref valueUnsafe.Array, nameIndex);
            }
        }

        /// <summary>
        /// Remove given key meta data
        /// </summary>
        /// <param name="key">the metadata tag to find the value for</param>
        /// <param name="nameIndex">if specified, will search the metadata linked for that enum value; otherwise, searches the metadata for the enum itself</param>
        public void RemoveMetaData(string key, int nameIndex = -1)
        {
            // WITH_EDITOR
            if (Native_UEnum.RemoveMetaData == null)
            {
                return;
            }

            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            {
                Native_UEnum.RemoveMetaData(Address, ref keyUnsafe.Array, nameIndex);
            }
        }

        /// <summary>
        /// Gets the enum names / values
        /// <para/>
        /// TODO: Call this GetEnums to be consistent with SetEnums / NumEnums?
        /// </summary>
        /// <returns>The enum names / values</returns>
        public Dictionary<FName, long> GetValues()
        {
            Dictionary<FName, long> values = new Dictionary<FName, long>();
            int numOldValues = NumEnums() - 1;
            for (int i = 0; i < numOldValues; i++)
            {
                values[GetNameByIndex(i)] = GetValueByIndex(i);
            }
            return values;
        }

        public enum ECppForm : int
        {
            Regular,
            Namespaced,
            EnumClass
        }

        public enum EGetByNameFlags : int
        {
            None = 0,
            ErrorIfNotFound = 0x1,
            CaseSensitive = 0x2
        }
    }
}
