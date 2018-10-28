using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\UnrealMathUtility.h

    // Templated methods which we have expanded
    public static partial class FMath
    {
        ///////////////////////////////////////////
        // IsWithin
        ///////////////////////////////////////////

        /// <summary>
        /// Checks if value is within a range, exclusive on MaxValue)
        /// </summary>
        public static bool IsWithin(sbyte testValue, sbyte minValue, sbyte maxValue)
        {
            return ((testValue >= minValue) && (testValue < maxValue));
        }

        /// <summary>
        /// Checks if value is within a range, exclusive on MaxValue)
        /// </summary>
        public static bool IsWithin(byte testValue, byte minValue, byte maxValue)
        {
            return ((testValue >= minValue) && (testValue < maxValue));
        }

        /// <summary>
        /// Checks if value is within a range, exclusive on MaxValue)
        /// </summary>
        public static bool IsWithin(short testValue, short minValue, short maxValue)
        {
            return ((testValue >= minValue) && (testValue < maxValue));
        }

        /// <summary>
        /// Checks if value is within a range, exclusive on MaxValue)
        /// </summary>
        public static bool IsWithin(ushort testValue, ushort minValue, ushort maxValue)
        {
            return ((testValue >= minValue) && (testValue < maxValue));
        }

        /// <summary>
        /// Checks if value is within a range, exclusive on MaxValue)
        /// </summary>
        public static bool IsWithin(int testValue, int minValue, int maxValue)
        {
            return ((testValue >= minValue) && (testValue < maxValue));
        }

        /// <summary>
        /// Checks if value is within a range, exclusive on MaxValue)
        /// </summary>
        public static bool IsWithin(uint testValue, uint minValue, uint maxValue)
        {
            return ((testValue >= minValue) && (testValue < maxValue));
        }

        /// <summary>
        /// Checks if value is within a range, exclusive on MaxValue)
        /// </summary>
        public static bool IsWithin(long testValue, long minValue, long maxValue)
        {
            return ((testValue >= minValue) && (testValue < maxValue));
        }

        /// <summary>
        /// Checks if value is within a range, exclusive on MaxValue)
        /// </summary>
        public static bool IsWithin(ulong testValue, ulong minValue, ulong maxValue)
        {
            return ((testValue >= minValue) && (testValue < maxValue));
        }

        /// <summary>
        /// Checks if value is within a range, exclusive on MaxValue)
        /// </summary>
        public static bool IsWithin(float testValue, float minValue, float maxValue)
        {
            return ((testValue >= minValue) && (testValue < maxValue));
        }

        /// <summary>
        /// Checks if value is within a range, exclusive on MaxValue)
        /// </summary>
        public static bool IsWithin(double testValue, double minValue, double maxValue)
        {
            return ((testValue >= minValue) && (testValue < maxValue));
        }

        /// <summary>
        /// Checks if value is within a range, exclusive on MaxValue)
        /// </summary>
        public static bool IsWithin(decimal testValue, decimal minValue, decimal maxValue)
        {
            return ((testValue >= minValue) && (testValue < maxValue));
        }

        ///////////////////////////////////////////
        // IsWithinInclusive
        ///////////////////////////////////////////

        public static bool IsWithinInclusive(sbyte testValue, sbyte minValue, sbyte maxValue)
        {
            return ((testValue >= minValue) && (testValue <= maxValue));
        }

        public static bool IsWithinInclusive(byte testValue, byte minValue, byte maxValue)
        {
            return ((testValue >= minValue) && (testValue <= maxValue));
        }

        public static bool IsWithinInclusive(short testValue, short minValue, short maxValue)
        {
            return ((testValue >= minValue) && (testValue <= maxValue));
        }

        public static bool IsWithinInclusive(ushort testValue, ushort minValue, ushort maxValue)
        {
            return ((testValue >= minValue) && (testValue <= maxValue));
        }

        public static bool IsWithinInclusive(int testValue, int minValue, int maxValue)
        {
            return ((testValue >= minValue) && (testValue <= maxValue));
        }

        public static bool IsWithinInclusive(uint testValue, uint minValue, uint maxValue)
        {
            return ((testValue >= minValue) && (testValue <= maxValue));
        }

        public static bool IsWithinInclusive(long testValue, long minValue, long maxValue)
        {
            return ((testValue >= minValue) && (testValue <= maxValue));
        }

        public static bool IsWithinInclusive(ulong testValue, ulong minValue, ulong maxValue)
        {
            return ((testValue >= minValue) && (testValue <= maxValue));
        }

        public static bool IsWithinInclusive(float testValue, float minValue, float maxValue)
        {
            return ((testValue >= minValue) && (testValue <= maxValue));
        }

        public static bool IsWithinInclusive(double testValue, double minValue, double maxValue)
        {
            return ((testValue >= minValue) && (testValue <= maxValue));
        }

        public static bool IsWithinInclusive(decimal testValue, decimal minValue, decimal maxValue)
        {
            return ((testValue >= minValue) && (testValue <= maxValue));
        }

        ///////////////////////////////////////////
        // IsPowerOfTwo
        ///////////////////////////////////////////

        /// <summary>
        /// Checks whether a number is a power of two.
        /// </summary>
        /// <param name="value">Number to check</param>
        /// <returns>true if Value is a power of two</returns>
        public static bool IsPowerOfTwo(byte value)
        {
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Checks whether a number is a power of two.
        /// </summary>
        /// <param name="value">Number to check</param>
        /// <returns>true if Value is a power of two</returns>
        public static bool IsPowerOfTwo(short value)
        {
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Checks whether a number is a power of two.
        /// </summary>
        /// <param name="value">Number to check</param>
        /// <returns>true if Value is a power of two</returns>
        public static bool IsPowerOfTwo(ushort value)
        {
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Checks whether a number is a power of two.
        /// </summary>
        /// <param name="value">Number to check</param>
        /// <returns>true if Value is a power of two</returns>
        public static bool IsPowerOfTwo(int value)
        {
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Checks whether a number is a power of two.
        /// </summary>
        /// <param name="value">Number to check</param>
        /// <returns>true if Value is a power of two</returns>
        public static bool IsPowerOfTwo(uint value)
        {
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Checks whether a number is a power of two.
        /// </summary>
        /// <param name="value">Number to check</param>
        /// <returns>true if Value is a power of two</returns>
        public static bool IsPowerOfTwo(long value)
        {
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Checks whether a number is a power of two.
        /// </summary>
        /// <param name="value">Number to check</param>
        /// <returns>true if Value is a power of two</returns>
        public static bool IsPowerOfTwo(ulong value)
        {
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Checks whether a number is a power of two.
        /// </summary>
        /// <param name="value">Number to check</param>
        /// <returns>true if Value is a power of two</returns>
        public static bool IsPowerOfTwo(float value)
        {
            return IsPowerOfTwo((int)value);
        }

        /// <summary>
        /// Checks whether a number is a power of two.
        /// </summary>
        /// <param name="value">Number to check</param>
        /// <returns>true if Value is a power of two</returns>
        public static bool IsPowerOfTwo(double value)
        {
            return IsPowerOfTwo((long)value);
        }

        /// <summary>
        /// Checks whether a number is a power of two.
        /// </summary>
        /// <param name="value">Number to check</param>
        /// <returns>true if Value is a power of two</returns>
        public static bool IsPowerOfTwo(decimal value)
        {
            return IsPowerOfTwo((long)value);
        }

        ///////////////////////////////////////////
        // Max3
        ///////////////////////////////////////////

        /// <summary>
        /// Returns highest of 3 values
        /// </summary>
        public static sbyte Max3(sbyte val1, sbyte val2, sbyte val3)
        {
            return Math.Max(Math.Max(val1, val2), val3);
        }

        /// <summary>
        /// Returns highest of 3 values
        /// </summary>
        public static byte Max3(byte val1, byte val2, byte val3)
        {
            return Math.Max(Math.Max(val1, val2), val3);
        }

        /// <summary>
        /// Returns highest of 3 values
        /// </summary>
        public static short Max3(short val1, short val2, short val3)
        {
            return Math.Max(Math.Max(val1, val2), val3);
        }

        /// <summary>
        /// Returns highest of 3 values
        /// </summary>
        public static ushort Max3(ushort val1, ushort val2, ushort val3)
        {
            return Math.Max(Math.Max(val1, val2), val3);
        }

        /// <summary>
        /// Returns highest of 3 values
        /// </summary>
        public static int Max3(int val1, int val2, int val3)
        {
            return Math.Max(Math.Max(val1, val2), val3);
        }

        /// <summary>
        /// Returns highest of 3 values
        /// </summary>
        public static uint Max3(uint val1, uint val2, uint val3)
        {
            return Math.Max(Math.Max(val1, val2), val3);
        }

        /// <summary>
        /// Returns highest of 3 values
        /// </summary>
        public static long Max3(long val1, long val2, long val3)
        {
            return Math.Max(Math.Max(val1, val2), val3);
        }

        /// <summary>
        /// Returns highest of 3 values
        /// </summary>
        public static ulong Max3(ulong val1, ulong val2, ulong val3)
        {
            return Math.Max(Math.Max(val1, val2), val3);
        }

        /// <summary>
        /// Returns highest of 3 values
        /// </summary>
        public static float Max3(float val1, float val2, float val3)
        {
            return Math.Max(Math.Max(val1, val2), val3);
        }

        /// <summary>
        /// Returns highest of 3 values
        /// </summary>
        public static double Max3(double val1, double val2, double val3)
        {
            return Math.Max(Math.Max(val1, val2), val3);
        }

        /// <summary>
        /// Returns highest of 3 values
        /// </summary>
        public static decimal Max3(decimal val1, decimal val2, decimal val3)
        {
            return Math.Max(Math.Max(val1, val2), val3);
        }

        ///////////////////////////////////////////
        // Min3
        ///////////////////////////////////////////

        /// <summary>
        /// Returns lowest of 3 values
        /// </summary>
        public static sbyte Min3(sbyte val1, sbyte val2, sbyte val3)
        {
            return Math.Min(Math.Min(val1, val2), val3);
        }

        /// <summary>
        /// Returns lowest of 3 values
        /// </summary>
        public static byte Min3(byte val1, byte val2, byte val3)
        {
            return Math.Min(Math.Min(val1, val2), val3);
        }

        /// <summary>
        /// Returns lowest of 3 values
        /// </summary>
        public static short Min3(short val1, short val2, short val3)
        {
            return Math.Min(Math.Min(val1, val2), val3);
        }

        /// <summary>
        /// Returns lowest of 3 values
        /// </summary>
        public static ushort Min3(ushort val1, ushort val2, ushort val3)
        {
            return Math.Min(Math.Min(val1, val2), val3);
        }

        /// <summary>
        /// Returns lowest of 3 values
        /// </summary>
        public static int Min3(int val1, int val2, int val3)
        {
            return Math.Min(Math.Min(val1, val2), val3);
        }

        /// <summary>
        /// Returns lowest of 3 values
        /// </summary>
        public static uint Min3(uint val1, uint val2, uint val3)
        {
            return Math.Min(Math.Min(val1, val2), val3);
        }

        /// <summary>
        /// Returns lowest of 3 values
        /// </summary>
        public static long Min3(long val1, long val2, long val3)
        {
            return Math.Min(Math.Min(val1, val2), val3);
        }

        /// <summary>
        /// Returns lowest of 3 values
        /// </summary>
        public static ulong Min3(ulong val1, ulong val2, ulong val3)
        {
            return Math.Min(Math.Min(val1, val2), val3);
        }

        /// <summary>
        /// Returns lowest of 3 values
        /// </summary>
        public static float Min3(float val1, float val2, float val3)
        {
            return Math.Min(Math.Min(val1, val2), val3);
        }

        /// <summary>
        /// Returns lowest of 3 values
        /// </summary>
        public static double Min3(double val1, double val2, double val3)
        {
            return Math.Min(Math.Min(val1, val2), val3);
        }

        /// <summary>
        /// Returns lowest of 3 values
        /// </summary>
        public static decimal Min3(decimal val1, decimal val2, decimal val3)
        {
            return Math.Min(Math.Min(val1, val2), val3);
        }

        ///////////////////////////////////////////
        // Square
        ///////////////////////////////////////////

        /// <summary>
        /// Multiples value by itself
        /// </summary>
        public static int Square(sbyte value)
        {
            return value * value;
        }

        /// <summary>
        /// Multiples value by itself
        /// </summary>
        public static int Square(byte value)
        {
            return value * value;
        }

        /// <summary>
        /// Multiples value by itself
        /// </summary>
        public static int Square(short value)
        {
            return value * value;
        }

        /// <summary>
        /// Multiples value by itself
        /// </summary>
        public static int Square(ushort value)
        {
            return value * value;
        }

        /// <summary>
        /// Multiples value by itself
        /// </summary>
        public static int Square(int value)
        {
            return value * value;
        }

        /// <summary>
        /// Multiples value by itself
        /// </summary>
        public static uint Square(uint value)
        {
            return value * value;
        }

        /// <summary>
        /// Multiples value by itself
        /// </summary>
        public static long Square(long value)
        {
            return value * value;
        }

        /// <summary>
        /// Multiples value by itself
        /// </summary>
        public static ulong Square(ulong value)
        {
            return value * value;
        }

        /// <summary>
        /// Multiples value by itself
        /// </summary>
        public static float Square(float value)
        {
            return value * value;
        }

        /// <summary>
        /// Multiples value by itself
        /// </summary>
        public static double Square(double value)
        {
            return value * value;
        }

        /// <summary>
        /// Multiples value by itself
        /// </summary>
        public static decimal Square(decimal value)
        {
            return value * value;
        }

        ///////////////////////////////////////////
        // Clamp
        ///////////////////////////////////////////

        /// <summary>
        /// Clamps X to be between Min and Max, inclusive
        /// </summary>
        public static sbyte Clamp(sbyte x, sbyte min, sbyte max)
        {
            return x < min ? min : x < max ? x : max;
        }

        /// <summary>
        /// Clamps X to be between Min and Max, inclusive
        /// </summary>
        public static byte Clamp(byte x, byte min, byte max)
        {
            return x < min ? min : x < max ? x : max;
        }

        /// <summary>
        /// Clamps X to be between Min and Max, inclusive
        /// </summary>
        public static short Clamp(short x, short min, short max)
        {
            return x < min ? min : x < max ? x : max;
        }

        /// <summary>
        /// Clamps X to be between Min and Max, inclusive
        /// </summary>
        public static ushort Clamp(ushort x, ushort min, ushort max)
        {
            return x < min ? min : x < max ? x : max;
        }

        /// <summary>
        /// Clamps X to be between Min and Max, inclusive
        /// </summary>
        public static int Clamp(int x, int min, int max)
        {
            return x < min ? min : x < max ? x : max;
        }

        /// <summary>
        /// Clamps X to be between Min and Max, inclusive
        /// </summary>
        public static uint Clamp(uint x, uint min, uint max)
        {
            return x < min ? min : x < max ? x : max;
        }

        /// <summary>
        /// Clamps X to be between Min and Max, inclusive
        /// </summary>
        public static long Clamp(long x, long min, long max)
        {
            return x < min ? min : x < max ? x : max;
        }

        /// <summary>
        /// Clamps X to be between Min and Max, inclusive
        /// </summary>
        public static ulong Clamp(ulong x, ulong min, ulong max)
        {
            return x < min ? min : x < max ? x : max;
        }

        /// <summary>
        /// Clamps X to be between Min and Max, inclusive
        /// </summary>
        public static float Clamp(float x, float min, float max)
        {
            return x < min ? min : x < max ? x : max;
        }

        /// <summary>
        /// Clamps X to be between Min and Max, inclusive
        /// </summary>
        public static double Clamp(double x, double min, double max)
        {
            return x < min ? min : x < max ? x : max;
        }

        /// <summary>
        /// Clamps X to be between Min and Max, inclusive
        /// </summary>
        public static decimal Clamp(decimal x, decimal min, decimal max)
        {
            return x < min ? min : x < max ? x : max;
        }

        ///////////////////////////////////////////
        // DivideAndRoundUp
        ///////////////////////////////////////////

        /// <summary>
        /// Divides two integers and rounds up
        /// </summary>
        public static int DivideAndRoundUp(int dividend, int divisor)
        {
            return (dividend + divisor - 1) / divisor;
        }

        /// <summary>
        /// Divides two integers and rounds up
        /// </summary>
        public static uint DivideAndRoundUp(uint dividend, uint divisor)
        {
            return (dividend + divisor - 1) / divisor;
        }

        /// <summary>
        /// Divides two integers and rounds up
        /// </summary>
        public static long DivideAndRoundUp(long dividend, long divisor)
        {
            return (dividend + divisor - 1) / divisor;
        }

        /// <summary>
        /// Divides two integers and rounds up
        /// </summary>
        public static ulong DivideAndRoundUp(ulong dividend, ulong divisor)
        {
            return (dividend + divisor - 1) / divisor;
        }

        ///////////////////////////////////////////
        // DivideAndRoundDown
        ///////////////////////////////////////////

        /// <summary>
        /// Divides two integers and rounds down
        /// </summary>
        public static int DivideAndRoundDown(int dividend, int divisor)
        {
            return dividend / divisor;
        }

        /// <summary>
        /// Divides two integers and rounds down
        /// </summary>
        public static uint DivideAndRoundDown(uint dividend, uint divisor)
        {
            return dividend / divisor;
        }

        /// <summary>
        /// Divides two integers and rounds down
        /// </summary>
        public static long DivideAndRoundDown(long dividend, long divisor)
        {
            return dividend / divisor;
        }

        /// <summary>
        /// Divides two integers and rounds down
        /// </summary>
        public static ulong DivideAndRoundDown(ulong dividend, ulong divisor)
        {
            return dividend / divisor;
        }

        ///////////////////////////////////////////
        // DivideAndRoundNearest
        ///////////////////////////////////////////

        /// <summary>
        /// Divides two integers and rounds to nearest
        /// </summary>
        public static int DivideAndRoundNearest(int dividend, int divisor)
        {
            return (dividend >= 0) ? 
                (dividend + divisor / 2) / divisor : 
                (dividend - divisor / 2 + 1) / divisor;
        }

        /// <summary>
        /// Divides two integers and rounds to nearest
        /// </summary>
        public static uint DivideAndRoundNearest(uint dividend, uint divisor)
        {
            return (dividend >= 0) ?
                (dividend + divisor / 2) / divisor :
                (dividend - divisor / 2 + 1) / divisor;
        }

        /// <summary>
        /// Divides two integers and rounds to nearest
        /// </summary>
        public static long DivideAndRoundNearest(long dividend, long divisor)
        {
            return (dividend >= 0) ?
                (dividend + divisor / 2) / divisor :
                (dividend - divisor / 2 + 1) / divisor;
        }

        /// <summary>
        /// Divides two integers and rounds to nearest
        /// </summary>
        public static ulong DivideAndRoundNearest(ulong dividend, ulong divisor)
        {
            return (dividend >= 0) ?
                (dividend + divisor / 2) / divisor :
                (dividend - divisor / 2 + 1) / divisor;
        }

        ///////////////////////////////////////////
        // RadiansToDegrees
        ///////////////////////////////////////////

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radVal">Value in radians.</param>
        /// <returns>Value in degrees.</returns>
        public static float RadiansToDegrees(float radVal)
        {
            return radVal * (180.0f / PI);
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radVal">Value in radians.</param>
        /// <returns>Value in degrees.</returns>
        public static double RadiansToDegrees(double radVal)
        {
            return radVal * (180.0f / PI);
        }

        ///////////////////////////////////////////
        // DegreesToRadians
        ///////////////////////////////////////////

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degVal">Value in degrees.</param>
        /// <returns>Value in radians.</returns>
        public static float DegreesToRadians(float degVal)
        {
            return degVal * (PI / 180.0f);
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degVal">Value in degrees.</param>
        /// <returns>Value in radians.</returns>
        public static double DegreesToRadians(double degVal)
        {
            return degVal * (PI / 180.0f);
        }
    }
}
