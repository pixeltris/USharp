using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\GenericPlatform\GenericPlatformMath.h

    // Templated methods which we have expanded
    // These are for coverage of Unreal's FMath class and are otherwise mostly useless as they just call into System.Math
    public static partial class FMath
    {
        ///////////////////////////////////////////
        // Abs
        ///////////////////////////////////////////

        /// <summary>
        /// Returns the absolute value of an 8-bit signed integer.
        /// </summary>
        public static sbyte Abs(sbyte value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Returns the absolute value of a 16-bit signed integer.
        /// </summary>
        public static short Abs(short value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Returns the absolute value of a 32-bit signed integer.
        /// </summary>
        public static int Abs(int value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Returns the absolute value of a 64-bit signed integer.
        /// </summary>
        public static long Abs(long value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Returns the absolute value of a single-precision floating-point number.
        /// </summary>
        public static float Abs(float value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Returns the absolute value of a double-precision floating-point number.
        /// </summary>
        public static double Abs(double value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Returns the absolute value of a System.Decimal number.
        /// </summary>
        public static decimal Abs(decimal value)
        {
            return Math.Abs(value);
        }

        ///////////////////////////////////////////
        // Sign
        ///////////////////////////////////////////

        /// <summary>
        ///  Returns a value indicating the sign of an 8-bit signed integer.
        /// </summary>
        public static int Sign(sbyte value)
        {
            return Math.Sign(value);
        }

        /// <summary>
        /// Returns a value indicating the sign of a 16-bit signed integer.
        /// </summary>
        public static int Sign(short value)
        {
            return Math.Sign(value);
        }

        /// <summary>
        /// Returns a value indicating the sign of a 32-bit signed integer.
        /// </summary>
        public static int Sign(int value)
        {
            return Math.Sign(value);
        }

        /// <summary>
        /// Returns a value indicating the sign of a 64-bit signed integer.
        /// </summary>
        public static int Sign(long value)
        {
            return Math.Sign(value);
        }

        /// <summary>
        /// Returns a value indicating the sign of a single-precision floating-point number.
        /// </summary>
        public static int Sign(float value)
        {
            return Math.Sign(value);
        }

        /// <summary>
        /// Returns a value indicating the sign of a double-precision floating-point number.
        /// </summary>
        public static int Sign(double value)
        {
            return Math.Sign(value);
        }

        /// <summary>
        /// Returns a value indicating the sign of a decimal number.
        /// </summary>
        public static int Sign(decimal value)
        {
            return Math.Sign(value);
        }

        ///////////////////////////////////////////
        // Max
        ///////////////////////////////////////////

        /// <summary>
        /// Returns the larger of two 8-bit signed integers.
        /// </summary>
        public static sbyte Max(sbyte val1, sbyte val2)
        {
            return Math.Max(val1, val2);
        }

        /// <summary>
        /// Returns the larger of two 8-bit unsigned integers.
        /// </summary>
        public static byte Max(byte val1, byte val2)
        {
            return Math.Max(val1, val2);
        }

        /// <summary>
        /// Returns the larger of two 16-bit signed integers.
        /// </summary>
        public static short Max(short val1, short val2)
        {
            return Math.Max(val1, val2);
        }

        /// <summary>
        /// Returns the larger of two 16-bit unsigned integers.
        /// </summary>
        public static ushort Max(ushort val1, ushort val2)
        {
            return Math.Max(val1, val2);
        }

        /// <summary>
        /// Returns the larger of two 32-bit signed integers.
        /// </summary>
        public static int Max(int val1, int val2)
        {
            return Math.Max(val1, val2);
        }

        /// <summary>
        /// Returns the larger of two 32-bit unsigned integers.
        /// </summary>
        public static uint Max(uint val1, uint val2)
        {
            return Math.Max(val1, val2);
        }

        /// <summary>
        /// Returns the larger of two 64-bit signed integers.
        /// </summary>
        public static long Max(long val1, long val2)
        {
            return Math.Max(val1, val2);
        }

        /// <summary>
        /// Returns the larger of two 64-bit unsigned integers.
        /// </summary>
        public static ulong Max(ulong val1, ulong val2)
        {
            return Math.Max(val1, val2);
        }

        /// <summary>
        /// Returns the larger of two single-precision floating-point numbers.
        /// </summary>
        public static float Max(float val1, float val2)
        {
            return Math.Max(val1, val2);
        }

        /// <summary>
        /// Returns the larger of two double-precision floating-point numbers.
        /// </summary>
        public static double Max(double val1, double val2)
        {
            return Math.Max(val1, val2);
        }

        /// <summary>
        /// Returns the larger of two decimal numbers.
        /// </summary>
        public static decimal Max(decimal val1, decimal val2)
        {
            return Math.Max(val1, val2);
        }

        ///////////////////////////////////////////
        // Min
        ///////////////////////////////////////////

        /// <summary>
        /// Returns the smaller of two 8-bit signed integers.
        /// </summary>
        public static sbyte Min(sbyte val1, sbyte val2)
        {
            return Math.Min(val1, val2);
        }

        /// <summary>
        /// Returns the smaller of two 8-bit unsigned integers.
        /// </summary>
        public static byte Min(byte val1, byte val2)
        {
            return Math.Min(val1, val2);
        }

        /// <summary>
        /// Returns the smaller of two 16-bit signed integers.
        /// </summary>
        public static short Min(short val1, short val2)
        {
            return Math.Min(val1, val2);
        }

        /// <summary>
        /// Returns the smaller of two 16-bit unsigned integers.
        /// </summary>
        public static ushort Min(ushort val1, ushort val2)
        {
            return Math.Min(val1, val2);
        }

        /// <summary>
        /// Returns the smaller of two 32-bit signed integers.
        /// </summary>
        public static int Min(int val1, int val2)
        {
            return Math.Min(val1, val2);
        }

        /// <summary>
        /// Returns the smaller of two 64-bit signed integers.
        /// </summary>
        public static long Min(long val1, long val2)
        {
            return Math.Min(val1, val2);
        }

        /// <summary>
        /// Returns the smaller of two 64-bit unsigned integers.
        /// </summary>
        public static ulong Min(ulong val1, ulong val2)
        {
            return Math.Min(val1, val2);
        }

        /// <summary>
        /// Returns the smaller of two single-precision floating-point numbers.
        /// </summary>
        public static float Min(float val1, float val2)
        {
            return Math.Min(val1, val2);
        }

        /// <summary>
        /// Returns the smaller of two double-precision floating-point numbers.
        /// </summary>
        public static double Min(double val1, double val2)
        {
            return Math.Min(val1, val2);
        }

        /// <summary>
        /// Returns the smaller of two decimal numbers.
        /// </summary>
        public static decimal Min(decimal val1, decimal val2)
        {
            return Math.Min(val1, val2);
        }

        ///////////////////////////////////////////
        // Min (array)
        ///////////////////////////////////////////

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static sbyte Min(params sbyte[] values)
        {
            int minIndex;
            return Min(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static sbyte Min(sbyte[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            sbyte curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                sbyte value = values[i];
                if (value < curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static byte Min(params byte[] values)
        {
            int minIndex;
            return Min(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static byte Min(byte[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            byte curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                byte value = values[i];
                if (value < curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static short Min(params short[] values)
        {
            int minIndex;
            return Min(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static short Min(short[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            short curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                short value = values[i];
                if (value < curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static ushort Min(params ushort[] values)
        {
            int minIndex;
            return Min(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static ushort Min(ushort[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            ushort curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                ushort value = values[i];
                if (value < curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static int Min(params int[] values)
        {
            int minIndex;
            return Min(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static int Min(int[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            int curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                int value = values[i];
                if (value < curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static uint Min(params uint[] values)
        {
            int minIndex;
            return Min(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static uint Min(uint[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            uint curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                uint value = values[i];
                if (value < curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static long Min(params long[] values)
        {
            int minIndex;
            return Min(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static long Min(long[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            long curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                long value = values[i];
                if (value < curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static ulong Min(params ulong[] values)
        {
            int minIndex;
            return Min(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static ulong Min(ulong[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            ulong curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                ulong value = values[i];
                if (value < curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static float Min(params float[] values)
        {
            int minIndex;
            return Min(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static float Min(float[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            float curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                float value = values[i];
                if (value < curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static double Min(params double[] values)
        {
            int minIndex;
            return Min(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static double Min(double[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            double curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                double value = values[i];
                if (value < curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static decimal Min(params decimal[] values)
        {
            int minIndex;
            return Min(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static decimal Min(decimal[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            decimal curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                decimal value = values[i];
                if (value < curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        ///////////////////////////////////////////
        // Max (array)
        ///////////////////////////////////////////

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static sbyte Max(params sbyte[] values)
        {
            int minIndex;
            return Max(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static sbyte Max(sbyte[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            sbyte curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                sbyte value = values[i];
                if (value > curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static byte Max(params byte[] values)
        {
            int minIndex;
            return Max(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static byte Max(byte[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            byte curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                byte value = values[i];
                if (value > curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static short Max(params short[] values)
        {
            int minIndex;
            return Max(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static short Max(short[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            short curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                short value = values[i];
                if (value > curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static ushort Max(params ushort[] values)
        {
            int minIndex;
            return Max(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static ushort Max(ushort[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            ushort curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                ushort value = values[i];
                if (value > curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static int Max(params int[] values)
        {
            int minIndex;
            return Max(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static int Max(int[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            int curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                int value = values[i];
                if (value > curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static uint Max(params uint[] values)
        {
            int minIndex;
            return Max(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static uint Max(uint[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            uint curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                uint value = values[i];
                if (value > curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static long Max(params long[] values)
        {
            int minIndex;
            return Max(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static long Max(long[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            long curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                long value = values[i];
                if (value > curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static ulong Max(params ulong[] values)
        {
            int minIndex;
            return Max(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static ulong Max(ulong[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            ulong curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                ulong value = values[i];
                if (value > curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static float Max(params float[] values)
        {
            int minIndex;
            return Max(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static float Max(float[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            float curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                float value = values[i];
                if (value > curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static double Max(params double[] values)
        {
            int minIndex;
            return Max(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static double Max(double[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            double curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                double value = values[i];
                if (value > curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static decimal Max(params decimal[] values)
        {
            int minIndex;
            return Max(values, out minIndex);
        }

        /// <summary>
        /// Min of Array
        /// </summary>
        /// <param name="values">Array of values</param>
        /// <param name="minIndex">The the index of the minimum element, if multiple minimum elements the first index is returned</param>
        /// <returns>The min value found in the array or default value if the array was empty</returns>
        public static decimal Max(decimal[] values, out int minIndex)
        {
            minIndex = 0;

            int length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            decimal curMin = values[0];
            for (int i = 1; i < length; ++i)
            {
                decimal value = values[i];
                if (value > curMin)
                {
                    curMin = value;
                    minIndex = i;
                }
            }

            return curMin;
        }
    }
}
