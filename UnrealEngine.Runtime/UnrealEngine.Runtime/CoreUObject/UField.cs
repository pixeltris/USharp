using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Base class of reflection data objects.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.Field", "CoreUObject", UnrealModuleType.Engine)]
    public class UField : UObject
    {
        private CachedUObject<UField> next;
        /// <summary>
        /// Variables
        /// </summary>
        public UField Next
        {
            get { return next.Update(Native_UField.Get_Next(Address)); }
            set { Native_UField.Set_Next(Address, next.Set(value)); }
        }

        private CachedUObject<UClass> ownerClass;
        public UClass OwnerClass
        {
            get { return ownerClass.Update(Native_UField.GetOwnerClass(Address)); }
        }

        private CachedUObject<UStruct> ownerStruct;
        public UStruct OwnerStruct
        {
            get { return ownerStruct.Update(Native_UField.GetOwnerStruct(Address)); }
        }

        public void AddCppProperty(UProperty property)
        {
            Native_UField.AddCppProperty(Address, property.Address);
        }

        public void Bind()
        {
            Native_UField.Bind(Address);
        }

        public UClass GetOwnerClass()
        {
            return OwnerClass;
        }

        public UStruct GetOwnerStruct()
        {
            return OwnerStruct;
        }

        /// <summary>
        /// Gets the display name or native display name as a fallback.
        /// </summary>
        /// <returns>The display name for this object.</returns>
        public string GetDisplayName()
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UField.GetDisplayName == null)
            {
                return null;
            }

            if (Native_UField.GetDisplayName != null)
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_UField.GetDisplayName(Address, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the localized tooltip or native tooltip as a fallback.
        /// </summary>
        /// <param name="shortTooltip">Look for a shorter version of the tooltip (falls back to the long tooltip if none was specified)</param>
        /// <returns>The tooltip for this object.</returns>
        public string GetToolTip(bool shortTooltip = false)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UField.GetToolTip == null)
            {
                return null;
            }

            if (Native_UField.GetToolTip != null)
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_UField.GetToolTip(Address, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Determines if the property has any metadata associated with the key
        /// </summary>
        /// <param name="key">The key to lookup in the metadata</param>
        /// <returns>true if there is a (possibly blank) value associated with this key</returns>
        public bool HasMetaData(string key)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UField.HasMetaData == null)
            {
                return false;
            }

            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            {
                return Native_UField.HasMetaData(Address, ref keyUnsafe.Array);
            }
        }

        /// <summary>
        /// Find the metadata value associated with the key
        /// </summary>
        /// <param name="key">The key to lookup in the metadata</param>
        /// <returns>The value associated with the key</returns>
        public string GetMetaData(string key)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UField.GetMetaData == null)
            {
                return null;
            }

            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UField.GetMetaData(Address, ref keyUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Find the metadata value associated with the key
        /// </summary>
        /// <param name="key">The key to lookup in the metadata</param>
        /// <returns>The value associated with the key</returns>
        public string GetMetaData(FName key)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UField.GetMetaDataF == null)
            {
                return null;
            }

            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UField.GetMetaDataF(Address, ref key, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Sets the metadata value associated with the key
        /// </summary>
        /// <param name="key">The key to lookup in the metadata</param>
        /// <param name="value">The value associated with the key</param>
        public void SetMetaData(string key, string value)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UField.SetMetaData == null)
            {
                return;
            }

            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
            {
                Native_UField.SetMetaData(Address, ref keyUnsafe.Array, ref valueUnsafe.Array);
            }
        }

        /// <summary>
        /// Find the metadata value associated with the key
        /// and return bool
        /// </summary>
        /// <param name="key">The key to lookup in the metadata</param>
        /// <returns>return true if the value was true (case insensitive)</returns>
        public bool GetBoolMetaData(string key)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UField.GetBoolMetaData == null)
            {
                return false;
            }

            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            {
                return Native_UField.GetBoolMetaData(Address, ref keyUnsafe.Array);
            }
        }

        /// <summary>
        /// Find the metadata value associated with the key
        /// and return int32
        /// </summary>
        /// <param name="key">The key to lookup in the metadata</param>
        /// <returns>the int value stored in the metadata.</returns>
        public int GetIntMetaData(string key)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UField.GetINTMetaData == null)
            {
                return 0;
            }

            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            {
                return Native_UField.GetINTMetaData(Address, ref keyUnsafe.Array);
            }
        }

        /// <summary>
        /// Find the metadata value associated with the key
        /// and return float
        /// </summary>
        /// <param name="key">The key to lookup in the metadata</param>
        /// <returns>the float value stored in the metadata.</returns>
        public float GetFloatMetaData(string key)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UField.GetFLOATMetaData == null)
            {
                return 0;
            }

            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            {
                return Native_UField.GetFLOATMetaData(Address, ref keyUnsafe.Array);
            }
        }
        
        public UClass GetClassMetaData(string key)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UField.GetClassMetaData == null)
            {
                return null;
            }

            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            {
                return GCHelper.Find<UClass>(Native_UField.GetClassMetaData(Address, ref keyUnsafe.Array));
            }
        }

        /// <summary>
        /// Clear any metadata associated with the key
        /// </summary>
        /// <param name="key"></param>
        public void RemoveMetaData(string key)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UField.RemoveMetaData == null)
            {
                return;
            }

            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            {
                Native_UField.RemoveMetaData(Address, ref keyUnsafe.Array);
            }
        }

        public string[] GetCommaSeperatedMetaData(string key)
        {
            List<string> result = new List<string>();
            string metaData = GetMetaData(key);
            if (!string.IsNullOrEmpty(metaData))
            {
                string[] splitted = metaData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string value in splitted)
                {
                    string trimmedValue = value.Trim();
                    if (!string.IsNullOrEmpty(trimmedValue))
                    {
                        result.Add(trimmedValue);
                    }
                }
            }
            return result.ToArray();
        }
    }
}
