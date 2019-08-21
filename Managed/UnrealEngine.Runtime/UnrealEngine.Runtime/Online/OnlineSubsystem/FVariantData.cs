using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Plugins.OnlineSubsystem
{
    /// <summary>
    /// Container for storing data of variable type
    /// </summary>
    public struct FVariantData : IEquatable<FVariantData>
    {
        /// <summary>
        /// Current data type
        /// </summary>
        public EOnlineKeyValuePairDataType Type;
        public byte[] Data;

        /// <summary>
        /// Cleans up the existing data and sets the type to Empty
        /// </summary>
        public void Empty()
        {
            Type = EOnlineKeyValuePairDataType.Empty;
            Data = null;
        }

        /// <summary>
        /// Copies the data and sets the type
        /// </summary>
        /// <param name="data">The new data to assign</param>
        public void SetValue(string data)
        {
            Type = EOnlineKeyValuePairDataType.String;
            Data = Encoding.Unicode.GetBytes(data);
        }

        /// <summary>
        /// Copies the data and sets the type
        /// </summary>
        /// <param name="data">The new data to assign</param>
        public void SetValue(int data)
        {
            Type = EOnlineKeyValuePairDataType.Int32;
            Data = BitConverter.GetBytes(data);
        }

        /// <summary>
        /// Copies the data and sets the type
        /// </summary>
        /// <param name="data">The new data to assign</param>
        public void SetValue(uint data)
        {
            Type = EOnlineKeyValuePairDataType.UInt32;
            Data = BitConverter.GetBytes(data);
        }

        /// <summary>
        /// Copies the data and sets the type
        /// </summary>
        /// <param name="data">The new data to assign</param>
        public void SetValue(bool data)
        {
            Type = EOnlineKeyValuePairDataType.Bool;
            Data = BitConverter.GetBytes(data);
        }

        /// <summary>
        /// Copies the data and sets the type
        /// </summary>
        /// <param name="data">The new data to assign</param>
        public void SetValue(double data)
        {
            Type = EOnlineKeyValuePairDataType.Double;
            Data = BitConverter.GetBytes(data);
        }

        /// <summary>
        /// Copies the data and sets the type
        /// </summary>
        /// <param name="data">The new data to assign</param>
        public void SetValue(float data)
        {
            Type = EOnlineKeyValuePairDataType.Float;
            Data = BitConverter.GetBytes(data);
        }

        /// <summary>
        /// Copies the data and sets the type
        /// </summary>
        /// <param name="data">The new data to assign</param>
        public unsafe void SetValue(byte[] data)
        {
            Type = EOnlineKeyValuePairDataType.Blob;
            Data = new byte[data.Length];
            Buffer.BlockCopy(data, 0, Data, 0, data.Length);
        }

        /// <summary>
        /// Copies the data and sets the type
        /// </summary>
        /// <param name="data">The new data to assign</param>
        public void SetValue(long data)
        {
            Type = EOnlineKeyValuePairDataType.Int64;
            Data = BitConverter.GetBytes(data);
        }

        /// <summary>
        /// Copies the data and sets the type
        /// </summary>
        /// <param name="data">The new data to assign</param>
        public void SetValue(ulong data)
        {
            Type = EOnlineKeyValuePairDataType.UInt64;
            Data = BitConverter.GetBytes(data);
        }

        /// <summary>
        /// Copies the data and sets the type
        /// </summary>
        /// <param name="data">The new data to assign</param>
        public void SetJsonValueFromString(string data)
        {
            Type = EOnlineKeyValuePairDataType.Json;
            Data = Encoding.Unicode.GetBytes(data);
        }

        /// <summary>
        /// Copies the data after verifying the type
        /// </summary>
        /// <returns>The data</returns>
        public string GetString()
        {
            if (Type == EOnlineKeyValuePairDataType.String || Type == EOnlineKeyValuePairDataType.Json)
            {
                return Encoding.Unicode.GetString(Data);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Copies the data after verifying the type
        /// </summary>
        /// <returns>The data</returns>
        public int GetInt32()
        {
            if (Type == EOnlineKeyValuePairDataType.Int32)
            {
                return BitConverter.ToInt32(Data, 0);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Copies the data after verifying the type
        /// </summary>
        /// <returns>The data</returns>
        public uint GetUInt32()
        {
            if (Type == EOnlineKeyValuePairDataType.UInt32)
            {
                return BitConverter.ToUInt32(Data, 0);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Copies the data after verifying the type
        /// </summary>
        /// <returns>The data</returns>
        public bool GetBool()
        {
            if (Type == EOnlineKeyValuePairDataType.Bool)
            {
                return BitConverter.ToBoolean(Data, 0);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Copies the data after verifying the type
        /// </summary>
        /// <returns>The data</returns>
        public long GetInt64()
        {
            if (Type == EOnlineKeyValuePairDataType.Int64)
            {
                return BitConverter.ToInt64(Data, 0);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Copies the data after verifying the type
        /// </summary>
        /// <returns>The data</returns>
        public ulong GetUInt64()
        {
            if (Type == EOnlineKeyValuePairDataType.UInt64)
            {
                return BitConverter.ToUInt64(Data, 0);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Copies the data after verifying the type
        /// </summary>
        /// <returns>The data</returns>
        public float GetSingle()
        {
            if (Type == EOnlineKeyValuePairDataType.Float)
            {
                return BitConverter.ToSingle(Data, 0);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Copies the data after verifying the type
        /// </summary>
        /// <returns>The data</returns>
        public double GetDouble()
        {
            if (Type == EOnlineKeyValuePairDataType.Double)
            {
                return BitConverter.ToDouble(Data, 0);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Copies the data after verifying the type
        /// </summary>
        /// <returns>The data</returns>
        public byte[] GetBytes()
        {
            if (Type == EOnlineKeyValuePairDataType.Blob)
            {
                byte[] result = new byte[Data.Length];
                Buffer.BlockCopy(Data, 0, result, 0, result.Length);
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns true if Type is numeric
        /// </summary>
        public bool IsNumeric()
        {
            switch (Type)
            {
                case EOnlineKeyValuePairDataType.Int32:
                case EOnlineKeyValuePairDataType.Int64:
                case EOnlineKeyValuePairDataType.UInt32:
                case EOnlineKeyValuePairDataType.UInt64:
                case EOnlineKeyValuePairDataType.Float:
                case EOnlineKeyValuePairDataType.Double:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Converts the data into a string representation
        /// </summary>
        public override string ToString()
        {
            switch (Type)
            {
                case EOnlineKeyValuePairDataType.Bool: return GetBool() ? "true" : "false";
                case EOnlineKeyValuePairDataType.Float: return GetSingle().ToString();
                case EOnlineKeyValuePairDataType.Int32: return GetInt32().ToString();
                case EOnlineKeyValuePairDataType.UInt32: return GetUInt32().ToString();
                case EOnlineKeyValuePairDataType.Int64: return GetInt64().ToString();
                case EOnlineKeyValuePairDataType.UInt64: return GetUInt64().ToString();
                case EOnlineKeyValuePairDataType.Double: return GetDouble().ToString();
                case EOnlineKeyValuePairDataType.Json: return GetString();
                case EOnlineKeyValuePairDataType.String: return GetString();
                case EOnlineKeyValuePairDataType.Blob: return Data.Length + " byte blob";
                default: return null;
            }
        }

        /// <summary>
        /// Converts the string to the specified type of data for this setting
        /// </summary>
        /// <param name="newValue">The string value to convert</param>
        /// <returns>True if it was converted, false otherwise</returns>
        public bool FromString(string newValue)
        {
            switch (Type)
            {
                case EOnlineKeyValuePairDataType.Float:
                    {
                        float value;
                        if (float.TryParse(newValue, out value))
                        {
                            SetValue(value);
                            return true;
                        }
                    }
                    break;
                case EOnlineKeyValuePairDataType.Int32:
                    {
                        int value;
                        if (int.TryParse(newValue, out value))
                        {
                            SetValue(value);
                            return true;
                        }
                    }
                    break;
                case EOnlineKeyValuePairDataType.UInt32:
                    {
                        uint value;
                        if (uint.TryParse(newValue, out value))
                        {
                            SetValue(value);
                            return true;
                        }
                    }
                    break;
                case EOnlineKeyValuePairDataType.Double:
                    {
                        double value;
                        if (double.TryParse(newValue, out value))
                        {
                            SetValue(value);
                            return true;
                        }
                    }
                    break;
                case EOnlineKeyValuePairDataType.Int64:
                    {
                        long value;
                        if (long.TryParse(newValue, out value))
                        {
                            SetValue(value);
                            return true;
                        }
                    }
                    break;
                case EOnlineKeyValuePairDataType.UInt64:
                    {
                        ulong value;
                        if (ulong.TryParse(newValue, out value))
                        {
                            SetValue(value);
                            return true;
                        }
                    }
                    break;
                case EOnlineKeyValuePairDataType.String:
                case EOnlineKeyValuePairDataType.Json:
                    {
                        SetValue(newValue);
                        return true;
                    }
                case EOnlineKeyValuePairDataType.Bool:
                    {
                        bool value;
                        if (bool.TryParse(newValue, out value))
                        {
                            SetValue(value);
                            return true;
                        }
                    }
                    break;
                case EOnlineKeyValuePairDataType.Blob:
                case EOnlineKeyValuePairDataType.Empty:
                    break;
            }
            return false;
        }

        public static bool operator ==(FVariantData a, FVariantData b)
        {
            if (Object.ReferenceEquals(a, null))
            {
                if (Object.ReferenceEquals(b, null))
                {
                    return true;
                }
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(FVariantData a, FVariantData b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FVariantData)
            {
                return Equals((FVariantData)obj);
            }
            return false;
        }

        public bool Equals(FVariantData other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }
            if (Type == other.Type)
            {
                if (Data == null && other.Data == null)
                {
                    return true;
                }
                if (Data == null || other.Data == null)
                {
                    return false;
                }
                return Enumerable.SequenceEqual(Data, other.Data);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void FromStringEx(string str)
        {
            int index = str.IndexOf('|');
            Type = (EOnlineKeyValuePairDataType)int.Parse(str.Substring(0, index));
            str = str.Substring(index + 1);

            switch (Type)
            {
                case EOnlineKeyValuePairDataType.Blob:
                    string[] splitted = str.Split(',');
                    byte[] buffer = new byte[splitted.Length];
                    for (int j = 0; j < splitted.Length; j++)
                    {
                        buffer[j] = byte.Parse(splitted[j]);
                    }
                    SetValue(buffer);
                    break;
                default:
                    FromString(str);
                    break;
            }
        }

        public string ToStringEx()
        {
            string valueStr = (int)Type + "|";
            switch (Type)
            {
                case EOnlineKeyValuePairDataType.Blob:
                    StringBuilder blobStr = new StringBuilder();
                    for (int j = 0; j < Data.Length; j++)
                    {
                        blobStr.Append(Data[j].ToString());
                        if (j < Data.Length - 1)
                        {
                            blobStr.Append(",");
                        }
                    }
                    valueStr += blobStr.ToString();
                    break;
                default:
                    valueStr += ToString();
                    break;
            }
            return valueStr;
        }

        public void FromNative(IntPtr address)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FVariantData.ToStringEx(address, ref resultUnsafe.Array);
                FromStringEx(resultUnsafe.Value);
            }
        }

        public void ToNative(IntPtr address)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(ToStringEx()))
            {
                Native_FVariantData.FromStringEx(address, ref strUnsafe.Array);
            }
        }
    }
}
