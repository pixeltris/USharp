using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// To be used in combination with <see cref="BitflagsAttribute"/> (but NOT <see cref="UseEnumValuesAsMaskValuesInEditorAttribute"/>).
    /// This works on bit indices (which is different to regular flags).
    /// </summary>
    public static class Bitflags
    {
        // TODO: Try to avoid boxing values

        public static bool HasBit<TEnum>(TEnum value, TEnum flag) where TEnum : struct, Enum, IConvertible
        {
            return HasBit(value.ToInt32(null), flag.ToInt32(null));
        }

        public static bool HasAllBits<TEnum>(TEnum value, params TEnum[] flags) where TEnum : struct, Enum, IConvertible
        {
            foreach (TEnum flag in flags)
            {
                if (!HasBit(value, flag))
                {
                    return false;
                }
            }
            return flags.Length > 0;
        }

        public static bool HasAnyBits<TEnum>(TEnum value, params TEnum[] flags) where TEnum : struct, Enum, IConvertible
        {
            foreach (TEnum flag in flags)
            {
                if (HasBit(value, flag))
                {
                    return true;
                }
            }
            return false;
        }

        public static TEnum SetBit<TEnum>(TEnum value, TEnum flag) where TEnum : struct, Enum, IConvertible
        {
            return (TEnum)(object)SetBit(value.ToInt32(null), flag.ToInt32(null));
        }

        public static TEnum SetBits<TEnum>(TEnum value, params TEnum[] flags) where TEnum : struct, Enum, IConvertible
        {
            int result = value.ToInt32(null);
            foreach (TEnum flag in flags)
            {
                result = SetBit(result, flag.ToInt32(null));
            }
            return (TEnum)(object)result;
        }

        public static TEnum ClearBit<TEnum>(TEnum value, TEnum flag) where TEnum : struct, Enum, IConvertible
        {
            return (TEnum)(object)ClearBit(value.ToInt32(null), flag.ToInt32(null));
        }

        public static TEnum ClearBits<TEnum>(TEnum value, params TEnum[] flags) where TEnum : struct, Enum, IConvertible
        {
            int result = value.ToInt32(null);
            foreach (TEnum flag in flags)
            {
                result = ClearBit(result, flag.ToInt32(null));
            }
            return (TEnum)(object)result;
        }

        public static bool HasBit(int value, int flag)
        {
            return (value & (1 << flag)) > 0;
        }

        public static bool HasAllBits(int value, params int[] flags)
        {
            foreach (int flag in flags)
            {
                if (!HasBit(value, flag))
                {
                    return false;
                }
            }
            return flags.Length > 0;
        }

        public static bool HasAnyBits(int value, params int[] flags)
        {
            foreach (int flag in flags)
            {
                if (HasBit(value, flag))
                {
                    return true;
                }
            }
            return false;
        }

        public static int SetBits(int value, params int[] flags)
        {
            int result = value;
            foreach (int flag in flags)
            {
                result = SetBit(result, flag);
            }
            return result;
        }

        public static int SetBit(int value, int flag)
        {
            return (value | (1 << flag));
        }

        public static int ClearBit(int value, int flag)
        {
            return (value & ~(1 << flag));
        }

        public static int ClearBits(int value, params int[] flags)
        {
            int result = value;
            foreach (int flag in flags)
            {
                result = ClearBit(result, flag);
            }
            return result;
        }
    }
}
