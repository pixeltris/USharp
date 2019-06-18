using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// An object that holds a map of key/value pairs.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x104000A0), UMetaPath("/Script/CoreUObject.MetaData")]
    public class UMetaData : UObject
    {
        /// <summary>
        /// Return the value for the given key in the given property
        /// </summary>
        /// <param name="obj">the object to lookup the metadata for</param>
        /// <param name="key">The key to lookup</param>
        /// <returns>The value if found, otherwise an empty string</returns>
        public string GetValue(UObject obj, string key)
        {
            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UMetaData.GetValue(Address, obj == null ? IntPtr.Zero : obj.Address, ref keyUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Return the value for the given key in the given property
        /// </summary>
        /// <param name="obj">the object to lookup the metadata for</param>
        /// <param name="key">The key to lookup</param>
        /// <returns>The value if found, otherwise an empty string</returns>
        public string GetValue(UObject obj, FName key)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UMetaData.GetValueFName(Address, obj == null ? IntPtr.Zero : obj.Address, ref key, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Return whether or not the Key is in the meta data
        /// </summary>
        /// <param name="obj">the object to lookup the metadata for</param>
        /// <param name="key">The key to query for existence</param>
        /// <returns>true if found</returns>
        public bool HasValue(UObject obj, string key)
        {
            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            {
                return Native_UMetaData.HasValue(Address, obj == null ? IntPtr.Zero : obj.Address, ref keyUnsafe.Array);
            }
        }

        /// <summary>
        /// Return whether or not the Key is in the meta data
        /// </summary>
        /// <param name="obj">the object to lookup the metadata for</param>
        /// <param name="key">The key to query for existence</param>
        /// <returns>true if found</returns>
        public bool HasValue(UObject obj, FName key)
        {
            return Native_UMetaData.HasValueFName(Address, obj == null ? IntPtr.Zero : obj.Address, ref key);
        }

        /// <summary>
        /// Is there any metadata for this property?
        /// </summary>
        /// <param name="obj">the object to lookup the metadata for</param>
        /// <returns>True if the object has any metadata at all</returns>
        public bool HasObjectValues(UObject obj)
        {
            return Native_UMetaData.HasObjectValues(Address, obj == null ? IntPtr.Zero : obj.Address);
        }

        /// <summary>
        /// Set the key/value pair in the Property's metadata
        /// </summary>
        /// <param name="obj">the object to set the metadata for</param>
        /// <param name="value">The metadata key/value pairs</param>
        public void SetObjectValues(UObject obj, Dictionary<FName, string> value)
        {
            using (TArrayUnsafe<FName> keysUnsafe = new TArrayUnsafe<FName>())
            using (TArrayUnsafe<string> valuesUnsafe = new TArrayUnsafe<string>())
            {
                keysUnsafe.AddRange(value.Keys.ToArray());
                valuesUnsafe.AddRange(value.Values.ToArray());
                Native_UMetaData.SetObjectValues(Address, obj == null ? IntPtr.Zero : obj.Address, keysUnsafe.Address, valuesUnsafe.Address);
            }
        }

        /// <summary>
        /// Set the key/value pair in the Object's metadata
        /// </summary>
        /// <param name="obj">the object to set the metadata for</param>
        /// <param name="key">A key to set the data for</param>
        /// <param name="value">The value to set for the key</param>
        public void SetValue(UObject obj, string key, string value)
        {
            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
            {
                Native_UMetaData.SetValue(Address, obj == null ? IntPtr.Zero : obj.Address, ref keyUnsafe.Array, ref valueUnsafe.Array);
            }
        }

        /// <summary>
        /// Set the key/value pair in the Property's metadata
        /// </summary>
        /// <param name="obj">the object to set the metadata for</param>
        /// <param name="key">A key to set the data for</param>
        /// <param name="value">The value to set for the key</param>
        public void SetValue(UObject obj, FName key, string value)
        {
            using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
            {
                Native_UMetaData.SetValueFName(Address, obj == null ? IntPtr.Zero : obj.Address, ref key, ref valueUnsafe.Array);
            }
        }

        /// <summary>
        /// Remove any entry with the supplied Key form the Property's metadata
        /// </summary>
        /// <param name="obj">the object to clear the metadata for</param>
        /// <param name="key">A key to clear the data for</param>
        public void RemoveValue(UObject obj, string key)
        {
            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            {
                Native_UMetaData.RemoveValue(Address, obj == null ? IntPtr.Zero : obj.Address, ref keyUnsafe.Array);
            }
        }

        /// <summary>
        /// Remove any entry with the supplied Key form the Property's metadata
        /// </summary>
        /// <param name="obj">the object to clear the metadata for</param>
        /// <param name="key">A key to clear the data for</param>
        public void RemoveValue(UObject obj, FName key)
        {
            Native_UMetaData.RemoveValueFName(Address, obj == null ? IntPtr.Zero : obj.Address, ref key);
        }

        /// <summary>
        /// Find the name/value map for metadata for a specific object
        /// </summary>
        public static Dictionary<FName, string> GetMapForObject(UObject obj)
        {
            Dictionary<FName, string> result = new Dictionary<FName, string>();
            using (TArrayUnsafe<FName> keysUnsafe = new TArrayUnsafe<FName>())
            using (TArrayUnsafe<string> valuesUnsafe = new TArrayUnsafe<string>())
            {
                Native_UMetaData.GetMapForObject(obj == null ? IntPtr.Zero : obj.Address, keysUnsafe.Address, valuesUnsafe.Address);
                if (keysUnsafe.Count == valuesUnsafe.Count)
                {
                    int count = keysUnsafe.Count;
                    for (int i = 0; i < count; i++)
                    {
                        result[keysUnsafe[i]] = valuesUnsafe[i];
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Copy all metadata from the source object to the destination object. This will add to any existing metadata entries for SourceObject.
        /// </summary>
        public static void CopyMetadata(UObject sourceObject, UObject destObject)
        {
            Native_UMetaData.CopyMetadata(sourceObject == null ? IntPtr.Zero : sourceObject.Address,
                destObject == null ? IntPtr.Zero : destObject.Address);
        }

        /// <summary>
        /// Removes any metadata entries that are to objects not inside the same package as this UMetaData object.
        /// </summary>
        public void RemoveMetaDataOutsidePackage()
        {
            Native_UMetaData.RemoveMetaDataOutsidePackage(Address);
        }
    }
}
