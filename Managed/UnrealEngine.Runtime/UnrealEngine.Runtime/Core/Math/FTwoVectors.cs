using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\TwoVectors.h

    /// <summary>
    /// A pair of 3D vectors.
    /// </summary>
    [UStruct(Flags = 0x0000E838), BlueprintType, UMetaPath("/Script/CoreUObject.TwoVectors", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FTwoVectors : IEquatable<FTwoVectors>
    {
        static bool v1_IsValid;
        static int v1_Offset;
        /// <summary>
        /// Holds the first vector.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000005), UMetaPath("/Script/CoreUObject.TwoVectors:v1")]
        public FVector V1;

        static bool v2_IsValid;
        static int v2_Offset;
        /// <summary>
        /// Holds the second vector.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000005), UMetaPath("/Script/CoreUObject.TwoVectors:v2")]
        public FVector V2;

        static int FTwoVectors_StructSize;

        public FTwoVectors Copy()
        {
            FTwoVectors result = this;
            return result;
        }

        static FTwoVectors()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FTwoVectors)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FTwoVectors));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.TwoVectors");
            FTwoVectors_StructSize = NativeReflection.GetStructSize(classAddress);
            v1_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "v1");
            v1_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "v1", Classes.UStructProperty);
            v2_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "v2");
            v2_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "v2", Classes.UStructProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FTwoVectors));
        }

        /// <summary>
        /// Creates and initializes a new instance with the specified vectors.
        /// </summary>
        /// <param name="v1">The first vector pair.</param>
        /// <param name="v2">The second vector pair.</param>
        public FTwoVectors(FVector v1, FVector v2)
        {
            V1 = v1;
            V2 = v2;
        }

        /// <summary>
        /// Gets the result of adding two pairs of vectors.
        /// </summary>
        /// <param name="a">The first vector pair.</param>
        /// <param name="b">The second vector pair.</param>
        /// <returns>Result of addition.</returns>
        public static FTwoVectors operator +(FTwoVectors a, FTwoVectors b)
        {
            a.V1 = a.V1 + b.V1;
            a.V2 = a.V2 + b.V2;
            return a;
        }

        /// <summary>
        /// Gets the result of subtracting two pairs of vectors.
        /// </summary>
        /// <param name="a">The first vector pair.</param>
        /// <param name="b">The second vector pair.</param>
        /// <returns>Result of subtraction.</returns>
        public static FTwoVectors operator -(FTwoVectors a, FTwoVectors b)
        {
            a.V1 = a.V1 - b.V1;
            a.V2 = a.V2 - b.V2;
            return a;
        }

        /// <summary>
        /// Gets the result of scaling a pair of vectors.
        /// </summary>
        /// <param name="scale">The scaling factor.</param>
        /// <param name="a">The vector pair.</param>
        /// <returns>Result of scaling.</returns>
        public static FTwoVectors operator *(float scale, FTwoVectors a)
        {
            a.V1 = a.V1 * scale;
            a.V2 = a.V2 * scale;
            return a;
        }

        /// <summary>
        /// Gets the result of scaling a pair of vectors.
        /// </summary>        
        /// <param name="a">The vector pair.</param>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>Result of scaling.</returns>
        public static FTwoVectors operator *(FTwoVectors a, float scale)
        {
            a.V1 = a.V1 * scale;
            a.V2 = a.V2 * scale;
            return a;
        }

        /// <summary>
        /// Gets the result of dividing a pair of vectors.
        /// </summary>        
        /// <param name="a">The vector pair.</param>
        /// <param name="scale">The dividing factor.</param>
        /// <returns>Result of dividing.</returns>
        public static FTwoVectors operator /(FTwoVectors a, float scale)
        {
            float factor = 1.0f / scale;
            a.V1 *= factor;
            a.V2 *= factor;
            return a;
        }

        /// <summary>
        /// Gets the result of multiplying two pairs of vectors.
        /// </summary>
        /// <param name="a">The first vector pair.</param>
        /// <param name="b">The second vector pair.</param>
        /// <returns>Result of multiplication.</returns>
        public static FTwoVectors operator *(FTwoVectors a, FTwoVectors b)
        {
            a.V1 *= b.V1;
            a.V2 *= b.V2;
            return a;
        }

        /// <summary>
        /// Gets the result of division of two pairs of vectors.
        /// </summary>
        /// <param name="a">The first vector pair.</param>
        /// <param name="b">The second vector pair.</param>
        /// <returns>Result of division.</returns>
        public static FTwoVectors operator /(FTwoVectors a, FTwoVectors b)
        {
            a.V1 /= b.V1;
            a.V2 /= b.V2;
            return a;
        }

        /// <summary>
        /// Get a negated copy of the vector pair.
        /// </summary>
        /// <param name="a">The vector pair</param>
        /// <returns>A negated copy of the vector pair.</returns>
        public static FTwoVectors operator -(FTwoVectors a)
        {
            a.V1 = -a.V1;
            a.V2 = -a.V2;
            return a;
        }

        /// <summary>
        /// Compares two pairs of vectors for equality.
        /// </summary>
        /// <param name="a">The first vector pair.</param>
        /// <param name="b">The second vector pair.</param>
        /// <returns>true if the two pairs are equal, false otherwise.</returns>
        public static bool operator ==(FTwoVectors a, FTwoVectors b)
        {
            return a.V1 == b.V1 && a.V2 == b.V2;
        }

        /// <summary>
        /// Compare two pairs of vectors for inequality.
        /// </summary>
        /// <param name="a">The first vector pair.</param>
        /// <param name="b">The second vector pair.</param>
        /// <returns>true if the two pairs are different, false otherwise.</returns>
        public static bool operator !=(FTwoVectors a, FTwoVectors b)
        {
            return a.V1 != b.V1 || a.V2 != b.V2;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FTwoVectors))
            {
                return false;
            }

            return Equals((FTwoVectors)obj);
        }

        public bool Equals(FTwoVectors other)
        {
            return V1 == other.V1 && V2 == other.V2;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (V1.GetHashCode() * 397) ^ V2.GetHashCode();
            }
        }

        /// <summary>
        /// Error-tolerant comparison.
        /// </summary>
        /// <param name="v">The other pair.</param>
        /// <param name="tolerance">Error Tolerance.</param>
        /// <returns>true if two pairs are equal within specified tolerance, false otherwise..</returns>
        public bool Equals(FTwoVectors v, float tolerance)
        {
            return V1.Equals(v.V1, tolerance) && V2.Equals(v.V2, tolerance);
        }

        /// <summary>
        /// Get a specific component from the vector pair.
        /// </summary>
        /// <param name="index">The index of the component, even indices are for the first vector,
        /// odd ones are for the second. Returns index 5 if out of range.</param>
        /// <returns>The specified component value.</returns>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return V1.X;
                    case 1: return V2.X;
                    case 2: return V1.Y;
                    case 3: return V2.Y;
                    case 4: return V1.Z;
                    case 5: return V2.Z;
                    default:
                        throw new IndexOutOfRangeException("Invalid FTwoVectors index (" + index + ")");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: V1.X = value; break;
                    case 1: V2.X = value; break;
                    case 2: V1.Y = value; break;
                    case 3: V2.Y = value; break;
                    case 4: V1.Z = value; break;
                    case 5: V2.Z = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FTwoVectors index (" + index + ")");
                }
            }
        }

        /// <summary>
        /// Get the maximum value of all the vector coordinates.
        /// </summary>
        /// <returns>The maximum value of all the vector coordinates.</returns>
        public float GetMax()
        {
            float maxMax = FMath.Max(FMath.Max(V1.X, V1.Y), V1.Z);
            float maxMin = FMath.Max(FMath.Max(V2.X, V2.Y), V2.Z);

            return FMath.Max(maxMax, maxMin);
        }

        /// <summary>
        /// Get the minimum value of all the vector coordinates.
        /// </summary>
        /// <returns>The minimum value of all the vector coordinates.</returns>
        public float GetMin()
        {
            float MinMax = FMath.Min(FMath.Min(V1.X, V1.Y), V1.Z);
            float MinMin = FMath.Min(FMath.Min(V2.X, V2.Y), V2.Z);

            return FMath.Min(MinMax, MinMin);
        }

        public override string ToString()
        {
            return "V1=(" + V1 + ") V2=(" + V2 + ")";
        }
    }
}
