using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [UMetaPath("/Script/CoreUObject.NumericProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class UNumericProperty : UProperty
    {
        public override bool IsBlittableType
        {
            get { return true; }
        }

        /// <summary>
        /// Return true if this property is for a floating point number
        /// </summary>
        public bool IsFloatingPoint
        {
            get { return Native_UNumericProperty.IsFloatingPoint(Address); }
        }

        /// <summary>
        /// Return true if this property is for a integral or enum type
        /// </summary>
        public bool IsInteger
        {
            get { return Native_UNumericProperty.IsInteger(Address); }
        }

        /// <summary>
        /// Return true if this property is a UByteProperty with a non-null Enum
        /// </summary>
        public bool IsEnum
        {            
            get { return Native_UNumericProperty.IsEnum(Address); }
        }

        /// <summary>
        /// Return the UEnum if this property is a UByteProperty with a non-null Enum
        /// </summary>
        /// <returns></returns>
        public UEnum GetIntPropertyEnum()
        {
            return GCHelper.Find<UEnum>(Native_UNumericProperty.GetIntPropertyEnum(Address));
        }

        /// <summary>
        /// Set the value of an unsigned integral property type
        /// </summary>
        /// <param name="data">pointer to property data to set</param>
        /// <param name="value">Value to set data to</param>
        public void SetIntPropertyValue(IntPtr data, ulong value)
        {
            Native_UNumericProperty.SetIntPropertyValueUnsigned(Address, data, value);
        }

        /// <summary>
        /// Set the value of a signed integral property type
        /// </summary>
        /// <param name="data">pointer to property data to set</param>
        /// <param name="value">Value to set data to</param>
        public void SetIntPropertyValue(IntPtr data, long value)
        {
            Native_UNumericProperty.SetIntPropertyValueSigned(Address, data, value);
        }

        /// <summary>
        /// Set the value of a floating point property type
        /// </summary>
        /// <param name="data">pointer to property data to set</param>
        /// <param name="value">Value to set data to</param>
        public void SetFloatingPointPropertyValue(IntPtr data, double value)
        {
            Native_UNumericProperty.SetFloatingPointPropertyValue(Address, data, value);
        }

        /// <summary>
        /// Set the value of any numeric type from a string point
        /// CAUTION: This routine does not do enum name conversion
        /// </summary>
        /// <param name="data">pointer to property data to set</param>
        /// <param name="value">Value (as a string) to set</param>
        public void SetNumericPropertyValueFromString(IntPtr data, string value)
        {
            using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
            {
                Native_UNumericProperty.SetNumericPropertyValueFromString(Address, data, ref valueUnsafe.Array);
            }
        }

        /// <summary>
        /// Gets the value of a signed integral property type
        /// </summary>
        /// <param name="data">pointer to property data to get</param>
        /// <returns>Data as a signed int</returns>
        public long GetSignedIntPropertyValue(IntPtr data)
        {
            return Native_UNumericProperty.GetSignedIntPropertyValue(Address, data);
        }

        /// <summary>
        /// Gets the value of an unsigned integral property type
        /// </summary>
        /// <param name="data">pointer to property data to get</param>
        /// <returns>Data as an unsigned int</returns>
        public ulong GetUnsignedIntPropertyValue(IntPtr data)
        {
            return Native_UNumericProperty.GetUnsignedIntPropertyValue(Address, data);
        }

        /// <summary>
        /// Gets the value of an floating point property type
        /// </summary>
        /// <param name="data">pointer to property data to get</param>
        /// <returns>Data as a double</returns>
        public double GetFloatingPointPropertyValue(IntPtr data)
        {
            return Native_UNumericProperty.GetFloatingPointPropertyValue(Address, data);
        }

        /// <summary>
        /// Get the value of any numeric type and return it as a string
        /// CAUTION: This routine does not do enum name conversion
        /// </summary>
        /// <param name="data">pointer to property data to get</param>
        /// <returns>Data as a string</returns>
        public string GetNumericPropertyValueToString(IntPtr data)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UNumericProperty.GetNumericPropertyValueToString(Address, data, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }
    }
}
