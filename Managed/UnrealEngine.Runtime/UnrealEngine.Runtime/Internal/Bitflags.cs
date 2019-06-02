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
        public static bool HasBit<TEnum>(TEnum value, TEnum flag) where TEnum : struct, Enum, IConvertible
        {
            return (value.ToInt32(null) & (1 << flag.ToInt32(null))) > 0;
        }

        public static TEnum SetBit<TEnum>(TEnum value, TEnum flag) where TEnum : struct, Enum, IConvertible
        {
            return (TEnum)(value.ToInt32(null) | (1 << flag.ToInt32(null)));
        }
    }
}
