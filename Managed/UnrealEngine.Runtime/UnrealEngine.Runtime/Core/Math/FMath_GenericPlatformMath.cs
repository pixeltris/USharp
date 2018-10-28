using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\GenericPlatform\GenericPlatformMath.h

    public static partial class FMath
    {
        /// <summary>
        /// Converts a float to an integer with truncation towards zero.
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>Truncated integer.</returns>
        public static int TruncToInt(float f)
        {
            return (int)f;
        }

        /// <summary>
        /// Converts a float to an integer value with truncation towards zero.
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>Truncated integer value.</returns>
        public static float TruncToFloat(float f)
        {
            return TruncToInt(f);
        }

        /// <summary>
        /// Converts a float to a nearest less or equal integer.
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>An integer less or equal to 'F'.</returns>
        public static int FloorToInt(float f)
        {
            return (int)Math.Floor(f);
        }

        /// <summary>
        /// Converts a float to the nearest less or equal integer.
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>An integer less or equal to 'F'.</returns>
        public static float FloorToFloat(float f)
        {
            return (float)Math.Floor(f);
        }

        /// <summary>
        /// Converts a double to a less or equal integer.
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The nearest integer value to 'F'.</returns>
        public static double FloorToDouble(double f)
        {
            return Math.Floor(f);
        }

        /// <summary>
        /// Converts a float to the nearest integer. Rounds up when the fraction is .5
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The nearest integer to 'F'.</returns>
        public static int RoundToInt(float f)
        {
            return (int)Math.Round(f);
            //return FloorToInt(f + 0.5f);
        }

        /// <summary>
        /// Converts a float to the nearest integer. Rounds up when the fraction is .5
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The nearest integer to 'F'.</returns>
        public static float RoundToFloat(float f)
        {
            return (float)Math.Round(f);
            //return FloorToFloat(f + 0.5f);
        }

        /// <summary>
        /// Converts a double to the nearest integer. Rounds up when the fraction is .5
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The nearest integer to 'F'.</returns>
        public static double RoundToDouble(double f)
        {
            return Math.Round(f);
            //return FloorToDouble(f + 0.5);
        }

        /// <summary>
        /// Converts a float to the nearest greater or equal integer.
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>An integer greater or equal to 'F'.</returns>
        public static int CeilToInt(float f)
        {
            return (int)Math.Ceiling(f);
            //return TruncToInt((float)Math.Ceiling(f));
        }

        /// <summary>
        /// Converts a float to the nearest greater or equal integer.
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>An integer greater or equal to 'F'.</returns>
        public static float CeilToFloat(float f)
        {
            return (float)Math.Ceiling(f);
        }

        /// <summary>
        /// Converts a double to the nearest greater or equal integer.
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>An integer greater or equal to 'F'.</returns>
        public static double CeilToDouble(double f)
        {
            return Math.Ceiling(f);
        }

        /// <summary>
        /// Returns signed fractional part of a float.
        /// </summary>
        /// <param name="value">Floating point value to convert</param>
        /// <returns>A float between >=0 and &lt; 1 for nonnegative input. A float between >= -1 and &lt; 0 for negative input.</returns>
        public static float Fractional(float value)
        {
            return value - TruncToFloat(value);
        }

        /// <summary>
        /// Returns the fractional part of a float.
        /// </summary>
        /// <param name="value">Floating point value to convert</param>
        /// <returns>A float between >=0 and &lt; 1.</returns>
        public static float Frac(float value)
        {
            return value - FloorToFloat(value);
        }

        /// <summary>
        /// Breaks the given value into an integral and a fractional part.
        /// </summary>
        /// <param name="value">Floating point value to convert</param>
        /// <param name="intPart">Floating point value that receives the integral part of the number.</param>
        /// <returns>The fractional part of the number.</returns>
        public static float Modf(float value, out float intPart)
        {
            intPart = (float)Math.Truncate(value);
            return value - intPart;
        }

        /// <summary>
        /// Breaks the given value into an integral and a fractional part.
        /// </summary>
        /// <param name="value">Floating point value to convert</param>
        /// <param name="intPart">Floating point value that receives the integral part of the number.</param>
        /// <returns></returns>
        public static double Modf(double value, out double intPart)
        {
            intPart = Math.Truncate(value);
            return value - intPart;
        }

        /// <summary>
        /// Returns e^Value
        /// </summary>
        public static float Exp(float value)
        {
            return (float)Math.Exp(value);
        }

        /// <summary>
        /// Returns 2^Value
        /// </summary>
        public static float Exp2(float value)
        {
            return (float)Math.Pow(2.0, value);
        }

        public static float Loge(float value)
        {
            return (float)Math.Log(value);
        }

        public static float LogX(float baseValue, float value)
        {
            return (float)(Math.Log(value) / Math.Log(baseValue));
        }

        /// <summary>
        /// 1.0 / Loge(2) = 1.4426950f
        /// </summary>
        public static float Log2(float value)
        {
            return (float)Math.Log(value) * 1.4426950f;
        }

        /// <summary>
        /// Returns the floating-point remainder of X / Y
        /// Warning: Always returns remainder toward 0, not toward the smaller multiple of Y.
        ///          So for example Fmod(2.8f, 2) gives .8f as you would expect, however, Fmod(-2.8f, 2) gives -.8f, NOT 1.2f 
        /// Use Floor instead when snapping positions that can be negative to a grid
        /// </summary>
        public static float Fmod(float x, float y)
        {
            return x % y;
        }

        public static float Sin(float value)
        {
            return (float)Math.Sin(value);
        }

        public static float Asin(float value)
        {
            return (float)Math.Asin((value < -1f) ? -1f : ((value < 1f) ? value : 1f));
        }

        public static float Sinh(float value)
        {
            return (float)Math.Sinh(value);
        }

        public static float Cos(float value)
        {
            return (float)Math.Cos(value);
        }

        public static float Acos(float value)
        {
            return (float)Math.Acos((value < -1f) ? -1f : ((value < 1f) ? value : 1f));
        }

        public static float Tan(float value)
        {
            return (float)Math.Tan(value);
        }

        public static float Atan(float value)
        {
            return (float)Math.Atan(value);
        }

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        public static float Sqrt(float value)
        {
            return (float)Math.Sqrt(value);
        }

        public static float Pow(float a, float b)
        {
            return (float)Math.Pow(a, b);
        }

        /// <summary>
        /// Computes a fully accurate inverse square root
        /// </summary>
        public static float InvSqrt(float value)
        {
            return 1.0f / (float)Math.Sqrt(value);
        }

        /// <summary>
        /// Computes a faster but less accurate inverse square root
        /// </summary>
        public static unsafe float InvSqrtEst(float value)
        {
            // OpenTK MathHelper.InverseSqrtFast
            float xhalf = 0.5f * value;
            int i = *(int*)&value;// Read bits as integer.
            i = 0x5f375a86 - (i >> 1);// Make an initial guess for Newton-Raphson approximation
            value = *(float*)&i;// Convert bits back to float
            value = value * (1.5f - xhalf * value * value);// Perform left single Newton-Raphson step.
            return value;
        }

        /// <summary>
        /// Return true if value is NaN (not a number).
        /// </summary>
        public static unsafe bool IsNaN(float value)
        {
            return ((*(uint*)&value) & 0x7FFFFFFF) > 0x7F800000;
        }

        /// <summary>
        /// Return true if value is finite (not NaN and not Infinity).
        /// </summary>
        public static unsafe bool IsFinite(float value)
        {
            return ((*(uint*)&value) & 0x7F800000) != 0x7F800000;
        }

        public static unsafe bool IsNegativeFloat(float value)
        {
            return ((*(uint*)&value) >= 0x80000000U); // Detects sign bit.
        }

        public static unsafe bool IsNegativeDouble(double value)
        {
            return ((*(ulong*)&value) >= 0x8000000000000000UL); // Detects sign bit.
        }

        /// <summary>
        /// Returns a random integer between 0 and RAND_MAX, inclusive
        /// </summary>
        public static int Rand()
        {
            return Native_FMath.Rand();
        }

        /// <summary>
        /// Seeds global random number functions Rand() and FRand()
        /// </summary>
        public static void RandInit(int seed)
        {
            Native_FMath.RandInit(seed);
        }

        /// <summary>
        /// Returns a random float between 0 and 1, inclusive.
        /// </summary>
        public static float FRand()
        {
            return Native_FMath.FRand();
        }

        /// <summary>
        /// Seeds future calls to SRand()
        /// </summary>
        public static void SRandInit(int seed)
        {
            Native_FMath.SRandInit(seed);
        }

        /// <summary>
        /// Returns the current seed for SRand().
        /// </summary>
        public static int GetRandSeed()
        {
            return Native_FMath.GetRandSeed();
        }

        /// <summary>
        /// Returns a seeded random float in the range [0,1), using the seed from SRandInit().
        /// </summary>
        public static float SRand()
        {
            return Native_FMath.SRand();
        }

        /// <summary>
        /// Computes the base 2 logarithm for an integer value that is greater than 0.
        /// The result is rounded down to the nearest integer.
        /// </summary>
        /// <param name="value">The value to compute the log of</param>
        /// <returns>Log2 of Value. 0 if Value is 0.</returns>
        public static uint FloorLog2(uint value)
        {
            uint pos = 0;
            if (value >= 1 << 16) { value >>= 16; pos += 16; }
            if (value >= 1 << 8) { value >>= 8; pos += 8; }
            if (value >= 1 << 4) { value >>= 4; pos += 4; }
            if (value >= 1 << 2) { value >>= 2; pos += 2; }
            if (value >= 1 << 1) { pos += 1; }
            return (value == 0) ? 0 : pos;
        }

        /// <summary>
        /// Computes the base 2 logarithm for a 64-bit value that is greater than 0.
        /// The result is rounded down to the nearest integer.
        /// </summary>
        /// <param name="value">The value to compute the log of</param>
        /// <returns>Log2 of Value. 0 if Value is 0.</returns>
        public static ulong FloorLog2_64(ulong value)
        {
            ulong pos = 0;
            if (value >= 1UL << 32) { value >>= 32; pos += 32; }
            if (value >= 1UL << 16) { value >>= 16; pos += 16; }
            if (value >= 1UL << 8) { value >>= 8; pos += 8; }
            if (value >= 1UL << 4) { value >>= 4; pos += 4; }
            if (value >= 1UL << 2) { value >>= 2; pos += 2; }
            if (value >= 1UL << 1) { pos += 1; }
            return (value == 0) ? 0 : pos;
        }

        /// <summary>
        /// Counts the number of leading zeros in the bit representation of the value
        /// </summary>
        /// <param name="value">the value to determine the number of leading zeros for</param>
        /// <returns>the number of zeros before the first "on" bit</returns>
        public static uint CountLeadingZeros(uint value)
        {
            if (value == 0)
            {
                return 32;
            }
            return 31 - FloorLog2(value);
        }

        /// <summary>
        /// Counts the number of leading zeros in the bit representation of the 64-bit value
        /// </summary>
        /// <param name="value">the value to determine the number of leading zeros for</param>
        /// <returns>the number of zeros before the first "on" bit</returns>
        public static ulong CountLeadingZeros64(ulong value)
        {
            if (value == 0)
            {
                return 64;
            }
            return 63 - FloorLog2_64(value);
        }

        /// <summary>
        /// Counts the number of trailing zeros in the bit representation of the value
        /// </summary>
        /// <param name="value">the value to determine the number of trailing zeros for</param>
        /// <returns>the number of zeros after the last "on" bit</returns>
        public static uint CountTrailingZeros(uint value)
        {
            if (value == 0)
            {
                return 32;
            }
            uint result = 0;
            while ((value & 1) == 0)
            {
                value >>= 1;
                ++result;
            }
            return result;
        }

        /// <summary>
        /// Returns smallest N such that (1&lt;&lt;N)>=Arg.
        /// Note: CeilLogTwo(0)=0 because (1&lt;&lt;0)=1 >= 0.
        /// </summary>
        public static uint CeilLogTwo(uint arg)
        {
            // Casts might be wrong here
            uint bitmask = (uint)(((int)(CountLeadingZeros(arg) << 26)) >> 31);
            return (32 - CountLeadingZeros(arg - 1)) & (~bitmask);
        }

        public static ulong CeilLogTwo64(ulong arg)
        {
            // Casts might be wrong here
            ulong bitmask = (ulong)(((long)(CountLeadingZeros64(arg) << 57)) >> 63);
            return (64 - CountLeadingZeros64(arg - 1)) & (~bitmask);
        }

        /// <summary>
        /// Rounds the given number up to the next highest power of two.
        /// </summary>
        public static uint RoundUpToPowerOfTwo(uint arg)
        {
            // Casts might be wrong here
            return (uint)(1 << (int)CeilLogTwo(arg));
        }

        public static ulong RoundUpToPowerOfTwo64(ulong arg)
        {
            // Casts might be wrong here
            return (ulong)(1L << (int)CeilLogTwo64(arg));
        }

        /// <summary>
        /// Spreads bits to every other.
        /// </summary>
        public static uint MortonCode2(uint x)
        {
            x &= 0x0000ffff;
            x = (x ^ (x << 8)) & 0x00ff00ff;
            x = (x ^ (x << 4)) & 0x0f0f0f0f;
            x = (x ^ (x << 2)) & 0x33333333;
            x = (x ^ (x << 1)) & 0x55555555;
            return x;
        }

        /// <summary>
        /// Reverses MortonCode2. Compacts every other bit to the right.
        /// </summary>
        public static uint ReverseMortonCode2(uint x)
        {
            x &= 0x55555555;
            x = (x ^ (x >> 1)) & 0x33333333;
            x = (x ^ (x >> 2)) & 0x0f0f0f0f;
            x = (x ^ (x >> 4)) & 0x00ff00ff;
            x = (x ^ (x >> 8)) & 0x0000ffff;
            return x;
        }

        /// <summary>
        /// Spreads bits to every 3rd.
        /// </summary>
        public static uint MortonCode3(uint x)
        {
            x &= 0x000003ff;
            x = (x ^ (x << 16)) & 0xff0000ff;
            x = (x ^ (x << 8)) & 0x0300f00f;
            x = (x ^ (x << 4)) & 0x030c30c3;
            x = (x ^ (x << 2)) & 0x09249249;
            return x;
        }

        /// <summary>
        /// Reverses MortonCode3. Compacts every 3rd bit to the right.
        /// </summary>
        public static uint ReverseMortonCode3(uint x)
        {
            x &= 0x09249249;
            x = (x ^ (x >> 2)) & 0x030c30c3;
            x = (x ^ (x >> 4)) & 0x0300f00f;
            x = (x ^ (x >> 8)) & 0xff0000ff;
            x = (x ^ (x >> 16)) & 0x000003ff;
            return x;
        }

        /// <summary>
        /// Returns value based on comparand. The main purpose of this function is to avoid
        /// branching based on floating point comparison which can be avoided via compiler
        /// intrinsics.
        /// 
        /// Please note that we don't define what happens in the case of NaNs as there might
        /// be platform specific differences.
        /// </summary>
        /// <param name="comparand">Comparand the results are based on</param>
        /// <param name="valueGEZero">Return value if Comparand >= 0</param>
        /// <param name="valueLTZero">Return value if Comparand &lt; 0</param>
        /// <returns>valueGEZero if comparand >= 0, valueLTZero otherwise</returns>
        public static float FloatSelect(float comparand, float valueGEZero, float valueLTZero)
        {
            return comparand >= 0.0f ? valueGEZero : valueLTZero;
        }

        /// <summary>
        /// Returns value based on comparand. The main purpose of this function is to avoid
        /// branching based on floating point comparison which can be avoided via compiler
        /// intrinsics.
        /// 
        /// Please note that we don't define what happens in the case of NaNs as there might
        /// be platform specific differences.
        /// </summary>
        /// <param name="comparand">Comparand the results are based on</param>
        /// <param name="valueGEZero">Return value if Comparand >= 0</param>
        /// <param name="valueLTZero">Return value if Comparand < 0</param>
        /// <returns>valueGEZero if comparand >= 0, valueLTZero otherwise</returns>
        public static double FloatSelect(double comparand, double valueGEZero, double valueLTZero)
        {
            return comparand >= 0.0 ? valueGEZero : valueLTZero;
        }

        public static int CountBits(ulong bits)
        {
            // https://en.wikipedia.org/wiki/Hamming_weight
            bits -= (bits >> 1) & 0x5555555555555555UL;
            bits = (bits & 0x3333333333333333UL) + ((bits >> 2) & 0x3333333333333333UL);
            bits = (bits + (bits >> 4)) & 0x0f0f0f0f0f0f0f0fUL;
            return (int)((bits * 0x0101010101010101) >> 56);
        }
    }
}
