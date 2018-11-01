using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Matrix.h

    /// <summary>
    /// 4x4 matrix of floating point values.
    /// Matrix-matrix multiplication happens with a pre-multiple of the transpose --
    /// in other words, Res = Mat1.operator*(Mat2) means Res = Mat2^T * Mat1, as
    /// opposed to Res = Mat1 * Mat2.
    /// Matrix elements are accessed with M[RowIndex][ColumnIndex].
    /// </summary>
    [UStruct(Flags = 0x0000E838), BlueprintType, UMetaPath("/Script/CoreUObject.Matrix", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FMatrix : IEquatable<FMatrix>
    {
        /// <summary>
        /// A first row and first column value.
        /// </summary>
        public float M11;
        /// <summary>
        /// A first row and second column value.
        /// </summary>
        public float M12;
        /// <summary>
        /// A first row and third column value.
        /// </summary>
        public float M13;
        /// <summary>
        /// A first row and fourth column value.
        /// </summary>
        public float M14;

        /// <summary>
        /// A second row and first column value.
        /// </summary>
        public float M21;
        /// <summary>
        /// A second row and second column value.
        /// </summary>
        public float M22;
        /// <summary>
        /// A second row and third column value.
        /// </summary>
        public float M23;
        /// <summary>
        /// A second row and fourth column value.
        /// </summary>
        public float M24;

        /// <summary>
        /// A third row and first column value.
        /// </summary>
        public float M31;
        /// <summary>
        /// A third row and second column value.
        /// </summary>
        public float M32;
        /// <summary>
        /// A third row and third column value.
        /// </summary>
        public float M33;
        /// <summary>
        /// A third row and fourth column value.
        /// </summary>
        public float M34;

        /// <summary>
        /// A fourth row and first column value.
        /// </summary>
        public float M41;
        /// <summary>
        /// A fourth row and second column value.
        /// </summary>
        public float M42;
        /// <summary>
        /// A fourth row and third column value.
        /// </summary>
        public float M43;
        /// <summary>
        /// A fourth row and fourth column value.
        /// </summary>
        public float M44;

        static bool XPlane_IsValid;
        static int XPlane_Offset;
        //[UProperty(Flags = (PropFlags)0x0010001041000005), UMetaPath("/Script/CoreUObject.Matrix:XPlane")]
        //public FPlane XPlane;

        static bool YPlane_IsValid;
        static int YPlane_Offset;
        //[UProperty(Flags = (PropFlags)0x0010001041000005), UMetaPath("/Script/CoreUObject.Matrix:YPlane")]
        //public FPlane YPlane;

        static bool ZPlane_IsValid;
        static int ZPlane_Offset;
        //[UProperty(Flags = (PropFlags)0x0010001041000005), UMetaPath("/Script/CoreUObject.Matrix:ZPlane")]
        //public FPlane ZPlane;

        static bool WPlane_IsValid;
        static int WPlane_Offset;
        //[UProperty(Flags = (PropFlags)0x0010001041000005), UMetaPath("/Script/CoreUObject.Matrix:WPlane")]
        //public FPlane WPlane;

        static int FMatrix_StructSize;

        public FMatrix Copy()
        {
            FMatrix result = this;
            return result;
        }

        static FMatrix()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FMatrix)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FMatrix));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Matrix");
            FMatrix_StructSize = NativeReflection.GetStructSize(classAddress);
            XPlane_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "XPlane");
            XPlane_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "XPlane", Classes.UStructProperty);
            YPlane_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "YPlane");
            YPlane_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "YPlane", Classes.UStructProperty);
            ZPlane_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "ZPlane");
            ZPlane_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "ZPlane", Classes.UStructProperty);
            WPlane_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "WPlane");
            WPlane_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "WPlane", Classes.UStructProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FMatrix));
        }

        private static readonly FMatrix Identity = new FMatrix(
            1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, 1f, 0f,
            0f, 0f, 0f, 1f);

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return M11;
                    case 1: return M12;
                    case 2: return M13;
                    case 3: return M14;
                    case 4: return M21;
                    case 5: return M22;
                    case 6: return M23;
                    case 7: return M24;
                    case 8: return M31;
                    case 9: return M32;
                    case 10: return M33;
                    case 11: return M34;
                    case 12: return M41;
                    case 13: return M42;
                    case 14: return M43;
                    case 15: return M44;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: M11 = value; break;
                    case 1: M12 = value; break;
                    case 2: M13 = value; break;
                    case 3: M14 = value; break;
                    case 4: M21 = value; break;
                    case 5: M22 = value; break;
                    case 6: M23 = value; break;
                    case 7: M24 = value; break;
                    case 8: M31 = value; break;
                    case 9: M32 = value; break;
                    case 10: M33 = value; break;
                    case 11: M34 = value; break;
                    case 12: M41 = value; break;
                    case 13: M42 = value; break;
                    case 14: M43 = value; break;
                    case 15: M44 = value; break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public float this[int row, int column]
        {
            get
            {
                return this[(row * 4) + column];
            }
            set
            {
                this[(row * 4) + column] = value;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="m11">A first row and first column value.</param>
        /// <param name="m12">A first row and second column value.</param>
        /// <param name="m13">A first row and third column value.</param>
        /// <param name="m14">A first row and fourth column value.</param>
        /// <param name="m21">A second row and first column value.</param>
        /// <param name="m22">A second row and second column value.</param>
        /// <param name="m23">A second row and third column value.</param>
        /// <param name="m24">A second row and fourth column value.</param>
        /// <param name="m31">A third row and first column value.</param>
        /// <param name="m32">A third row and second column value.</param>
        /// <param name="m33">A third row and third column value.</param>
        /// <param name="m34">A third row and fourth column value.</param>
        /// <param name="m41">A fourth row and first column value.</param>
        /// <param name="m42">A fourth row and second column value.</param>
        /// <param name="m43">A fourth row and third column value.</param>
        /// <param name="m44">A fourth row and fourth column value.</param>
        public FMatrix(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;
            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;
            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X plane </param>
        /// <param name="y">Y plane</param>
        /// <param name="z">Z plane</param>
        /// <param name="w">W plane</param>
        public FMatrix(FPlane x, FPlane y, FPlane z, FPlane w)
        {
            M11 = x.X; M12 = x.Y; M13 = x.Z; M14 = x.W;
            M21 = y.X; M22 = y.Y; M23 = y.Z; M24 = y.W;
            M31 = z.X; M32 = z.Y; M33 = z.Z; M34 = z.W;
            M41 = w.X; M42 = w.Y; M43 = w.Z; M44 = w.W;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X plane </param>
        /// <param name="y">Y plane</param>
        /// <param name="z">Z plane</param>
        /// <param name="w">W plane</param>
        public FMatrix(FVector x, FVector y, FVector z, FVector w)
        {
            M11 = x.X; M12 = x.Y; M13 = x.Z; M14 = 0.0f;
            M21 = y.X; M22 = y.Y; M23 = y.Z; M24 = 0.0f;
            M31 = z.X; M32 = z.Y; M33 = z.Z; M34 = 0.0f;
            M41 = w.X; M42 = w.Y; M43 = w.Z; M44 = 1.0f;
        }

        /// <summary>
        /// Set this to the identity matrix
        /// </summary>
        public void SetIdentity()
        {
            this = Identity;
        }

        /// <summary>
        /// Gets the result of multiplying two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The result of multiplication.</returns>
        public static FMatrix operator *(FMatrix a, FMatrix b)
        {
            Multiply(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of multiplying two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The result of multiplication.</returns>
        public static FMatrix Multiply(FMatrix a, FMatrix b)
        {
            Multiply(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of multiplying two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <param name="result">The result of multiplication.</param>
        public static void Multiply(ref FMatrix a, ref FMatrix b, out FMatrix result)
        {
            float m11 = (((a.M11 * b.M11) + (a.M12 * b.M21)) + (a.M13 * b.M31)) + (a.M14 * b.M41);
            float m12 = (((a.M11 * b.M12) + (a.M12 * b.M22)) + (a.M13 * b.M32)) + (a.M14 * b.M42);
            float m13 = (((a.M11 * b.M13) + (a.M12 * b.M23)) + (a.M13 * b.M33)) + (a.M14 * b.M43);
            float m14 = (((a.M11 * b.M14) + (a.M12 * b.M24)) + (a.M13 * b.M34)) + (a.M14 * b.M44);
            float m21 = (((a.M21 * b.M11) + (a.M22 * b.M21)) + (a.M23 * b.M31)) + (a.M24 * b.M41);
            float m22 = (((a.M21 * b.M12) + (a.M22 * b.M22)) + (a.M23 * b.M32)) + (a.M24 * b.M42);
            float m23 = (((a.M21 * b.M13) + (a.M22 * b.M23)) + (a.M23 * b.M33)) + (a.M24 * b.M43);
            float m24 = (((a.M21 * b.M14) + (a.M22 * b.M24)) + (a.M23 * b.M34)) + (a.M24 * b.M44);
            float m31 = (((a.M31 * b.M11) + (a.M32 * b.M21)) + (a.M33 * b.M31)) + (a.M34 * b.M41);
            float m32 = (((a.M31 * b.M12) + (a.M32 * b.M22)) + (a.M33 * b.M32)) + (a.M34 * b.M42);
            float m33 = (((a.M31 * b.M13) + (a.M32 * b.M23)) + (a.M33 * b.M33)) + (a.M34 * b.M43);
            float m34 = (((a.M31 * b.M14) + (a.M32 * b.M24)) + (a.M33 * b.M34)) + (a.M34 * b.M44);
            float m41 = (((a.M41 * b.M11) + (a.M42 * b.M21)) + (a.M43 * b.M31)) + (a.M44 * b.M41);
            float m42 = (((a.M41 * b.M12) + (a.M42 * b.M22)) + (a.M43 * b.M32)) + (a.M44 * b.M42);
            float m43 = (((a.M41 * b.M13) + (a.M42 * b.M23)) + (a.M43 * b.M33)) + (a.M44 * b.M43);
            float m44 = (((a.M41 * b.M14) + (a.M42 * b.M24)) + (a.M43 * b.M34)) + (a.M44 * b.M44);
            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;
            result.M14 = m14;
            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;
            result.M24 = m24;
            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
            result.M34 = m34;
            result.M41 = m41;
            result.M42 = m42;
            result.M43 = m43;
            result.M44 = m44;
        }

        /// <summary>
        /// Gets the result of adding two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The result of the addition.</returns>
        public static FMatrix operator +(FMatrix a, FMatrix b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of adding two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The result of the addition.</returns>
        public static FMatrix Add(FMatrix a, FMatrix b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of adding two matrices.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <param name="result">The result of the addition.</param>
        public static void Add(ref FMatrix matrix1, ref FMatrix matrix2, out FMatrix result)
        {
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M14 = matrix1.M14 + matrix2.M14;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M24 = matrix1.M24 + matrix2.M24;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
            result.M34 = matrix1.M34 + matrix2.M34;
            result.M41 = matrix1.M41 + matrix2.M41;
            result.M42 = matrix1.M42 + matrix2.M42;
            result.M43 = matrix1.M43 + matrix2.M43;
            result.M44 = matrix1.M44 + matrix2.M44;
        }

        /// <summary>
        /// Multiply the matrix by a weighting factor (multiplying float to all members).
        /// </summary>
        /// <param name="m">The matrix.</param>
        /// <param name="weight">The weight.</param>
        /// <returns>The result of multiplication.</returns>
        public static FMatrix operator *(float weight, FMatrix m)
        {
            Multiply(ref m, weight, out m);
            return m;
        }

        /// <summary>
        /// Multiply the matrix by a weighting factor (multiplying float to all members).
        /// </summary>
        /// <param name="m">The matrix.</param>
        /// <param name="weight">The weight.</param>
        /// <returns>The result of multiplication.</returns>
        public static FMatrix operator *(FMatrix m, float weight)
        {
            Multiply(ref m, weight, out m);
            return m;
        }

        /// <summary>
        /// Multiply the matrix by a weighting factor (multiplying float to all members).
        /// </summary>
        /// <param name="m">The matrix.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="result">The result of multiplication.</param>
        public static void Multiply(ref FMatrix m, float weight, out FMatrix result)
        {
            result.M11 = m.M11 * weight;
            result.M12 = m.M12 * weight;
            result.M13 = m.M13 * weight;
            result.M14 = m.M14 * weight;
            result.M21 = m.M21 * weight;
            result.M22 = m.M22 * weight;
            result.M23 = m.M23 * weight;
            result.M24 = m.M24 * weight;
            result.M31 = m.M31 * weight;
            result.M32 = m.M32 * weight;
            result.M33 = m.M33 * weight;
            result.M34 = m.M34 * weight;
            result.M41 = m.M41 * weight;
            result.M42 = m.M42 * weight;
            result.M43 = m.M43 * weight;
            result.M44 = m.M44 * weight;
        }

        /// <summary>
        /// Checks two matrices for equality.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>true if the matrices are equal, false otherwise.</returns>
        public static bool operator ==(FMatrix a, FMatrix b)
        {
            return (
                a.M11 == b.M11 &&
                a.M12 == b.M12 &&
                a.M13 == b.M13 &&
                a.M14 == b.M14 &&
                a.M21 == b.M21 &&
                a.M22 == b.M22 &&
                a.M23 == b.M23 &&
                a.M24 == b.M24 &&
                a.M31 == b.M31 &&
                a.M32 == b.M32 &&
                a.M33 == b.M33 &&
                a.M34 == b.M34 &&
                a.M41 == b.M41 &&
                a.M42 == b.M42 &&
                a.M43 == b.M43 &&
                a.M44 == b.M44);
        }

        /// <summary>
        /// Checks two matrices for inequality.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>true if the matrices are not equal, false otherwise.</returns>
        public static bool operator !=(FMatrix a, FMatrix b)
        {
            return (
                a.M11 != b.M11 ||
                a.M12 != b.M12 ||
                a.M13 != b.M13 ||
                a.M14 != b.M14 ||
                a.M21 != b.M21 ||
                a.M22 != b.M22 ||
                a.M23 != b.M23 ||
                a.M24 != b.M24 ||
                a.M31 != b.M31 ||
                a.M32 != b.M32 ||
                a.M33 != b.M33 ||
                a.M34 != b.M34 ||
                a.M41 != b.M41 ||
                a.M42 != b.M42 ||
                a.M43 != b.M43 ||
                a.M44 != b.M44);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FMatrix))
            {
                return false;
            }

            return Equals((FMatrix)obj);
        }

        public bool Equals(FMatrix other)
        {
            return (
                M11 == other.M11 &&
                M12 == other.M12 &&
                M13 == other.M13 &&
                M14 == other.M14 &&
                M21 == other.M21 &&
                M22 == other.M22 &&
                M23 == other.M23 &&
                M24 == other.M24 &&
                M31 == other.M31 &&
                M32 == other.M32 &&
                M33 == other.M33 &&
                M34 == other.M34 &&
                M41 == other.M41 &&
                M42 == other.M42 &&
                M43 == other.M43 &&
                M44 == other.M44);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = M11.GetHashCode();
                hashcode = (hashcode * 397) ^ M12.GetHashCode();
                hashcode = (hashcode * 397) ^ M13.GetHashCode();
                hashcode = (hashcode * 397) ^ M14.GetHashCode();
                hashcode = (hashcode * 397) ^ M21.GetHashCode();
                hashcode = (hashcode * 397) ^ M22.GetHashCode();
                hashcode = (hashcode * 397) ^ M23.GetHashCode();
                hashcode = (hashcode * 397) ^ M24.GetHashCode();
                hashcode = (hashcode * 397) ^ M31.GetHashCode();
                hashcode = (hashcode * 397) ^ M32.GetHashCode();
                hashcode = (hashcode * 397) ^ M33.GetHashCode();
                hashcode = (hashcode * 397) ^ M34.GetHashCode();
                hashcode = (hashcode * 397) ^ M41.GetHashCode();
                hashcode = (hashcode * 397) ^ M42.GetHashCode();
                hashcode = (hashcode * 397) ^ M43.GetHashCode();
                hashcode = (hashcode * 397) ^ M44.GetHashCode();
                return hashcode;
            }
        }

        /// <summary>
        /// Checks whether another Matrix is equal to this, within specified tolerance.
        /// </summary>
        /// <param name="other">The other Matrix.</param>
        /// <param name="tolerance">Error Tolerance.</param>
        /// <returns>true if two Matrix are equal, within specified tolerance, otherwise false.</returns>
        public bool Equals(FMatrix other, float tolerance = FMath.KindaSmallNumber)
        {
            return (
                FMath.Abs(M11 - other.M11) > tolerance &&
                FMath.Abs(M12 - other.M12) > tolerance &&
                FMath.Abs(M13 - other.M13) > tolerance &&
                FMath.Abs(M14 - other.M14) > tolerance &&
                FMath.Abs(M21 - other.M21) > tolerance &&
                FMath.Abs(M22 - other.M22) > tolerance &&
                FMath.Abs(M23 - other.M23) > tolerance &&
                FMath.Abs(M24 - other.M24) > tolerance &&
                FMath.Abs(M31 - other.M31) > tolerance &&
                FMath.Abs(M32 - other.M32) > tolerance &&
                FMath.Abs(M33 - other.M33) > tolerance &&
                FMath.Abs(M34 - other.M34) > tolerance &&
                FMath.Abs(M41 - other.M41) > tolerance &&
                FMath.Abs(M42 - other.M42) > tolerance &&
                FMath.Abs(M43 - other.M43) > tolerance &&
                FMath.Abs(M44 - other.M44) > tolerance);
        }

        /// <summary>
        /// Transform a location - will take into account translation part of the FMatrix.
        /// </summary>
        public FVector4 TransformFVector4(FVector4 v)
        {
            float x = (v.X * M11) + (v.Y * M21) + (v.Z * M31) + (v.W * M41);
            float y = (v.X * M12) + (v.Y * M22) + (v.Z * M32) + (v.W * M42);
            float z = (v.X * M13) + (v.Y * M23) + (v.Z * M33) + (v.W * M43);
            float w = (v.X * M14) + (v.Y * M24) + (v.Z * M34) + (v.W * M44);
            v.X = x;
            v.Y = y;
            v.Z = z;
            v.W = w;
            return v;
        }

        /// <summary>
        /// Transform a location - will take into account translation part of the FMatrix.
        /// </summary>
        public FVector4 TransformPosition(FVector v)
        {
            return TransformFVector4(new FVector4(v.X, v.Y, v.Z, 1.0f));
        }

        /// <summary>
        /// Inverts the matrix and then transforms V - correctly handles scaling in this matrix.
        /// </summary>
        public FVector InverseTransformPosition(FVector v)
        {
            FMatrix invSelf = this.InverseFast();
            return (FVector)invSelf.TransformPosition(v);
        }

        /// <summary>
        /// Transform a direction vector - will not take into account translation part of the FMatrix.
        /// If you want to transform a surface normal (or plane) and correctly account for non-uniform scaling you should use TransformByUsingAdjointT.
        /// </summary>
        public FVector4 TransformVector(FVector v)
        {
            return TransformFVector4(new FVector4(v.X, v.Y, v.Z, 0.0f));
        }

        /// <summary>
        /// Transform a direction vector by the inverse of this matrix - will not take into account translation part.
        /// If you want to transform a surface normal (or plane) and correctly account for non-uniform scaling you should use TransformByUsingAdjointT with adjoint of matrix inverse.
        /// </summary>
        public FVector4 InverseTransformVector(FVector v)
        {
            FMatrix invSelf = this.InverseFast();
            return invSelf.TransformVector(v);
        }

        /// <summary>
        /// Transpose (swap the matrix rows and columns).
        /// </summary>
        public FMatrix GetTransposed()
        {
            FMatrix ret;

            ret.M11 = M11;
            ret.M12 = M21;
            ret.M13 = M31;
            ret.M14 = M41;

            ret.M21 = M12;
            ret.M22 = M22;
            ret.M23 = M32;
            ret.M24 = M42;

            ret.M31 = M13;
            ret.M32 = M23;
            ret.M33 = M33;
            ret.M34 = M43;

            ret.M41 = M14;
            ret.M42 = M24;
            ret.M43 = M34;
            ret.M44 = M44;

            return ret;
        }

        /// <summary>
        /// Returns the determinant of this matrix.
        /// </summary>
        public float Determinant()
        {
            return M11 * (
                M22 * (M33 * M44 - M34 * M43) -
                M32 * (M23 * M44 - M24 * M43) +
                M42 * (M23 * M34 - M24 * M33)
                ) -
                M21 * (
                M12 * (M33 * M44 - M34 * M43) -
                M32 * (M13 * M44 - M14 * M43) +
                M42 * (M13 * M34 - M14 * M33)
                ) +
                M31 * (
                M12 * (M23 * M44 - M24 * M43) -
                M22 * (M13 * M44 - M14 * M43) +
                M42 * (M13 * M24 - M14 * M23)
                ) -
                M41 * (
                M12 * (M23 * M34 - M24 * M33) -
                M22 * (M13 * M34 - M14 * M33) +
                M32 * (M13 * M24 - M14 * M23)
                );
        }

        /// <summary>
        /// Returns the determinant of rotation 3x3 matrix
        /// </summary>
        public float RotDeterminant()
        {
            return
                M11 * (M22 * M33 - M23 * M32) -
                M21 * (M12 * M33 - M13 * M32) +
                M31 * (M12 * M23 - M13 * M22);
        }

        /// <summary>
        /// Fast path, doesn't check for nil matrices in final release builds
        /// </summary>
        public FMatrix InverseFast()
        {
            //#if !(UE_BUILD_SHIPPING || UE_BUILD_TEST)
            FMessage.EnsureDebug(
                GetScaledAxis(EAxis.X).IsNearlyZero(FMath.SmallNumber) &&
                GetScaledAxis(EAxis.Y).IsNearlyZero(FMath.SmallNumber) &&
                GetScaledAxis(EAxis.Z).IsNearlyZero(FMath.SmallNumber),
                "FMatrix::InverseFast(), trying to invert a NIL matrix, this results in NaNs! Use Inverse() instead.");
            //#endif

            FMatrix result;
            VectorMatrixInverse(out result, ref this);
            return result;
        }

        private static void VectorMatrixInverse(out FMatrix result, ref FMatrix matrix)
        {
            // MonoGame implementation
            float num1 = matrix.M11;
            float num2 = matrix.M12;
            float num3 = matrix.M13;
            float num4 = matrix.M14;
            float num5 = matrix.M21;
            float num6 = matrix.M22;
            float num7 = matrix.M23;
            float num8 = matrix.M24;
            float num9 = matrix.M31;
            float num10 = matrix.M32;
            float num11 = matrix.M33;
            float num12 = matrix.M34;
            float num13 = matrix.M41;
            float num14 = matrix.M42;
            float num15 = matrix.M43;
            float num16 = matrix.M44;
            float num17 = (float)((double)num11 * (double)num16 - (double)num12 * (double)num15);
            float num18 = (float)((double)num10 * (double)num16 - (double)num12 * (double)num14);
            float num19 = (float)((double)num10 * (double)num15 - (double)num11 * (double)num14);
            float num20 = (float)((double)num9 * (double)num16 - (double)num12 * (double)num13);
            float num21 = (float)((double)num9 * (double)num15 - (double)num11 * (double)num13);
            float num22 = (float)((double)num9 * (double)num14 - (double)num10 * (double)num13);
            float num23 = (float)((double)num6 * (double)num17 - (double)num7 * (double)num18 + (double)num8 * (double)num19);
            float num24 = (float)-((double)num5 * (double)num17 - (double)num7 * (double)num20 + (double)num8 * (double)num21);
            float num25 = (float)((double)num5 * (double)num18 - (double)num6 * (double)num20 + (double)num8 * (double)num22);
            float num26 = (float)-((double)num5 * (double)num19 - (double)num6 * (double)num21 + (double)num7 * (double)num22);
            float num27 = (float)(1.0 / ((double)num1 * (double)num23 + (double)num2 * (double)num24 + (double)num3 * (double)num25 + (double)num4 * (double)num26));

            result.M11 = num23 * num27;
            result.M21 = num24 * num27;
            result.M31 = num25 * num27;
            result.M41 = num26 * num27;
            result.M12 = (float)-((double)num2 * (double)num17 - (double)num3 * (double)num18 + (double)num4 * (double)num19) * num27;
            result.M22 = (float)((double)num1 * (double)num17 - (double)num3 * (double)num20 + (double)num4 * (double)num21) * num27;
            result.M32 = (float)-((double)num1 * (double)num18 - (double)num2 * (double)num20 + (double)num4 * (double)num22) * num27;
            result.M42 = (float)((double)num1 * (double)num19 - (double)num2 * (double)num21 + (double)num3 * (double)num22) * num27;
            float num28 = (float)((double)num7 * (double)num16 - (double)num8 * (double)num15);
            float num29 = (float)((double)num6 * (double)num16 - (double)num8 * (double)num14);
            float num30 = (float)((double)num6 * (double)num15 - (double)num7 * (double)num14);
            float num31 = (float)((double)num5 * (double)num16 - (double)num8 * (double)num13);
            float num32 = (float)((double)num5 * (double)num15 - (double)num7 * (double)num13);
            float num33 = (float)((double)num5 * (double)num14 - (double)num6 * (double)num13);
            result.M13 = (float)((double)num2 * (double)num28 - (double)num3 * (double)num29 + (double)num4 * (double)num30) * num27;
            result.M23 = (float)-((double)num1 * (double)num28 - (double)num3 * (double)num31 + (double)num4 * (double)num32) * num27;
            result.M33 = (float)((double)num1 * (double)num29 - (double)num2 * (double)num31 + (double)num4 * (double)num33) * num27;
            result.M43 = (float)-((double)num1 * (double)num30 - (double)num2 * (double)num32 + (double)num3 * (double)num33) * num27;
            float num34 = (float)((double)num7 * (double)num12 - (double)num8 * (double)num11);
            float num35 = (float)((double)num6 * (double)num12 - (double)num8 * (double)num10);
            float num36 = (float)((double)num6 * (double)num11 - (double)num7 * (double)num10);
            float num37 = (float)((double)num5 * (double)num12 - (double)num8 * (double)num9);
            float num38 = (float)((double)num5 * (double)num11 - (double)num7 * (double)num9);
            float num39 = (float)((double)num5 * (double)num10 - (double)num6 * (double)num9);
            result.M14 = (float)-((double)num2 * (double)num34 - (double)num3 * (double)num35 + (double)num4 * (double)num36) * num27;
            result.M24 = (float)((double)num1 * (double)num34 - (double)num3 * (double)num37 + (double)num4 * (double)num38) * num27;
            result.M34 = (float)-((double)num1 * (double)num35 - (double)num2 * (double)num37 + (double)num4 * (double)num39) * num27;
            result.M44 = (float)((double)num1 * (double)num36 - (double)num2 * (double)num38 + (double)num3 * (double)num39) * num27;
        }

        /// <summary>
        /// Fast path, and handles nil matrices.
        /// </summary>
        public FMatrix Inverse()
        {
            FMatrix result;

            // Check for zero scale matrix to invert
            if (GetScaledAxis(EAxis.X).IsNearlyZero(FMath.SmallNumber) &&
                GetScaledAxis(EAxis.Y).IsNearlyZero(FMath.SmallNumber) &&
                GetScaledAxis(EAxis.Z).IsNearlyZero(FMath.SmallNumber))
            {
                // just set to zero - avoids unsafe inverse of zero and duplicates what QNANs were resulting in before (scaling away all children)
                result = FMatrix.Identity;
            }
            else
            {
                float det = Determinant();

                if (det == 0.0f)
                {
                    result = FMatrix.Identity;
                }
                else
                {
                    VectorMatrixInverse(out result, ref this);
                }
            }

            return result;
        }

        public FMatrix TransposeAdjoint()
        {
            FMatrix ta;

            ta.M11 = this.M22 * this.M33 - this.M23 * this.M32;
            ta.M12 = this.M23 * this.M31 - this.M21 * this.M33;
            ta.M13 = this.M21 * this.M32 - this.M22 * this.M31;
            ta.M14 = 0.0f;

            ta.M21 = this.M32 * this.M13 - this.M33 * this.M12;
            ta.M22 = this.M33 * this.M11 - this.M31 * this.M13;
            ta.M23 = this.M31 * this.M12 - this.M32 * this.M11;
            ta.M24 = 0.0f;

            ta.M31 = this.M12 * this.M23 - this.M13 * this.M22;
            ta.M32 = this.M13 * this.M21 - this.M11 * this.M23;
            ta.M33 = this.M11 * this.M22 - this.M12 * this.M21;
            ta.M34 = 0.0f;

            ta.M41 = 0.0f;
            ta.M42 = 0.0f;
            ta.M43 = 0.0f;
            ta.M44 = 1.0f;

            return ta;
        }

        /// <summary>
        /// NOTE: There is some compiler optimization issues with WIN64 that cause FORCEINLINE to cause a crash
        /// Remove any scaling from this matrix (ie magnitude of each row is 1) with error Tolerance
        /// </summary>
        public void RemoveScaling(float tolerance = FMath.SmallNumber)
        {
            // For each row, find magnitude, and if its non-zero re-scale so its unit length.
            float SquareSum0 = (M11 * M11) + (M12 * M12) + (M13 * M13);
            float SquareSum1 = (M21 * M21) + (M22 * M22) + (M23 * M23);
            float SquareSum2 = (M31 * M31) + (M32 * M32) + (M33 * M33);
            float Scale0 = FMath.FloatSelect(SquareSum0 - tolerance, FMath.InvSqrt(SquareSum0), 1.0f);
            float Scale1 = FMath.FloatSelect(SquareSum1 - tolerance, FMath.InvSqrt(SquareSum1), 1.0f);
            float Scale2 = FMath.FloatSelect(SquareSum2 - tolerance, FMath.InvSqrt(SquareSum2), 1.0f);
            M11 *= Scale0;
            M12 *= Scale0;
            M13 *= Scale0;
            M21 *= Scale1;
            M22 *= Scale1;
            M23 *= Scale1;
            M31 *= Scale2;
            M32 *= Scale2;
            M33 *= Scale2;
        }

        /// <summary>
        /// Returns matrix after RemoveScaling with error Tolerance
        /// </summary>
        public FMatrix GetMatrixWithoutScale(float tolerance = FMath.SmallNumber)
        {
            FMatrix result = this;
            result.RemoveScaling(tolerance);
            return result;
        }

        /// <summary>
        /// Remove any scaling from this matrix (ie magnitude of each row is 1) and return the 3D scale vector that was initially present with error Tolerance
        /// </summary>
        public FVector ExtractScaling(float tolerance = FMath.SmallNumber)
        {
            FVector scale3D = new FVector(0, 0, 0);

            // For each row, find magnitude, and if its non-zero re-scale so its unit length.
            float squareSum0 = (M11 * M11) + (M12 * M12) + (M13 * M13);
            float squareSum1 = (M21 * M21) + (M22 * M22) + (M23 * M23);
            float squareSum2 = (M31 * M31) + (M32 * M32) + (M33 * M33);

            if (squareSum0 > tolerance)
            {
                float scale0 = FMath.Sqrt(squareSum0);
                scale3D[0] = scale0;
                float invScale0 = 1.0f / scale0;
                M11 *= invScale0;
                M12 *= invScale0;
                M13 *= invScale0;
            }
            else
            {
                scale3D[0] = 0;
            }

            if (squareSum1 > tolerance)
            {
                float Scale1 = FMath.Sqrt(squareSum1);
                scale3D[1] = Scale1;
                float invScale1 = 1.0f / Scale1;
                M21 *= invScale1;
                M22 *= invScale1;
                M23 *= invScale1;
            }
            else
            {
                scale3D[1] = 0;
            }

            if (squareSum2 > tolerance)
            {
                float scale2 = FMath.Sqrt(squareSum2);
                scale3D[2] = scale2;
                float invScale2 = 1.0f / scale2;
                M31 *= invScale2;
                M32 *= invScale2;
                M33 *= invScale2;
            }
            else
            {
                scale3D[2] = 0;
            }

            return scale3D;
        }

        /// <summary>
        /// return a 3D scale vector calculated from this matrix (where each component is the magnitude of a row vector) with error Tolerance.
        /// </summary>
        public FVector GetScaleVector(float tolerance = FMath.SmallNumber)
        {
            FVector scale3D = new FVector(1, 1, 1);

            // For each row, find magnitude, and if its non-zero re-scale so its unit length.
            for (int i = 0; i < 3; i++)
            {
                float squareSum = (this[i, 0] * this[i, 0]) + (this[i, 1] * this[i, 1]) + (this[i, 2] * this[i, 2]);
                if (squareSum > tolerance)
                {
                    scale3D[i] = FMath.Sqrt(squareSum);
                }
                else
                {
                    scale3D[i] = 0.0f;
                }
            }

            return scale3D;
        }

        /// <summary>
        /// Remove any translation from this matrix
        /// </summary>
        public FMatrix RemoveTranslation()
        {
            FMatrix result = this;
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            return result;
        }

        /// <summary>
        /// Returns a matrix with an additional translation concatenated.
        /// </summary>
        public FMatrix ConcatTranslation(FVector translation)
        {
            FMatrix result = this;
            result.M41 += translation.X;
            result.M42 += translation.Y;
            result.M43 += translation.Z;
            return result;
        }

        /// <summary>
        /// Returns true if any element of this matrix is NaN
        /// </summary>
        public bool ContainsNaN()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (!FMath.IsFinite(this[i, j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Scale the translation part of the matrix by the supplied vector.
        /// </summary>
        public void ScaleTranslation(FVector scale3D)
        {
            M41 *= scale3D.X;
            M42 *= scale3D.Y;
            M43 *= scale3D.Z;
        }

        /// <summary>
        /// Returns the maximum magnitude of any row of the matrix.
        /// </summary>
        public float GetMaximumAxisScale()
        {
            float MaxRowScaleSquared = FMath.Max(
                GetScaledAxis(EAxis.X).SizeSquared(),
                FMath.Max(
                    GetScaledAxis(EAxis.Y).SizeSquared(),
                    GetScaledAxis(EAxis.Z).SizeSquared()));
            return FMath.Sqrt(MaxRowScaleSquared);
        }

        /// <summary>
        /// Apply Scale to this matrix
        /// </summary>
        public FMatrix ApplyScale(float scale)
        {
            FMatrix scaleMatrix = new FMatrix(
                scale, 0.0f, 0.0f, 0.0f,
                0.0f, scale, 0.0f, 0.0f,
                0.0f, 0.0f, scale, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
            return scaleMatrix * this;
        }

        /// <summary>
        /// Returns the origin of the co-ordinate system
        /// </summary>
        public FVector GetOrigin()
        {
            return new FVector(M41, M42, M43);
        }

        /// <summary>
        /// get axis of this matrix scaled by the scale of the matrix
        /// </summary>
        /// <param name="axis">index into the axis of the matrix</param>
        /// <returns>vector of the axis</returns>
        public FVector GetScaledAxis(EAxis axis)
        {
            switch (axis)
            {
                case EAxis.X:
                    return new FVector(M11, M12, M13);
                case EAxis.Y:
                    return new FVector(M21, M22, M23);
                case EAxis.Z:
                    return new FVector(M31, M32, M33);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// get axes of this matrix scaled by the scale of the matrix
        /// </summary>
        /// <param name="X">axes returned to this param</param>
        /// <param name="Y">axes returned to this param</param>
        /// <param name="Z">axes returned to this param</param>
        public void GetScaledAxes(out FVector X, out FVector Y, out FVector Z)
        {
            X.X = M11; X.Y = M12; X.Z = M13;
            Y.X = M21; Y.Y = M22; Y.Z = M23;
            Z.X = M31; Z.Y = M32; Z.Z = M33;
        }

        /// <summary>
        /// get unit length axis of this matrix
        /// </summary>
        /// <param name="axis">index into the axis of the matrix</param>
        /// <returns>vector of the axis</returns>
        public FVector GetUnitAxis(EAxis axis)
        {
            return GetScaledAxis(axis).GetSafeNormal();
        }

        /// <summary>
        /// get unit length axes of this matrix
        /// </summary>
        /// <param name="X">axes returned to this param</param>
        /// <param name="Y">axes returned to this param</param>
        /// <param name="Z">axes returned to this param</param>
        public void GetUnitAxes(out FVector X, out FVector Y, out FVector Z)
        {
            GetScaledAxes(out X, out Y, out Z);
        }

        /// <summary>
        /// set an axis of this matrix
        /// </summary>
        /// <param name="i">index into the axis of the matrix</param>
        /// <param name="axis">vector of the axis</param>
        public void SetAxis(int i, FVector axis)
        {
            Debug.Assert(i >= 0 && i < 2);
            this[i, 0] = axis.X;
            this[i, 1] = axis.Y;
            this[i, 2] = axis.Z;
        }

        /// <summary>
        /// Set the origin of the coordinate system to the given vector
        /// </summary>
        public void SetOrigin(FVector newOrigin)
        {
            M41 = newOrigin.X;
            M42 = newOrigin.Y;
            M43 = newOrigin.Z;
        }

        /// <summary>
        /// Update the axes of the matrix if any value is NULL do not update that axis
        /// </summary>
        /// <param name="axis0">set matrix row 0</param>
        /// <param name="axis1">set matrix row 1</param>
        /// <param name="axis2">set matrix row 2</param>
        /// <param name="origin">set matrix row 3</param>
        public void SetAxes(FVector? axis0 = null, FVector? axis1 = null, FVector? axis2 = null, FVector? origin = null)
        {
            if (axis0 != null)
            {
                FVector v = axis0.Value;
                M11 = v.X;
                M12 = v.Y;
                M13 = v.Z;
            }
            if (axis1 != null)
            {
                FVector v = axis1.Value;
                M21 = v.X;
                M22 = v.Y;
                M23 = v.Z;
            }
            if (axis2 != null)
            {
                FVector v = axis2.Value;
                M31 = v.X;
                M32 = v.Y;
                M33 = v.Z;
            }
            if (origin != null)
            {
                FVector v = origin.Value;
                M41 = v.X;
                M42 = v.Y;
                M43 = v.Z;
            }
        }

        /// <summary>
        /// get a row of this matrix
        /// </summary>
        /// <param name="i">index into the row of the matrix</param>
        /// <returns>vector of row column </returns>
        public FVector GetRow(int i)
        {
            Debug.Assert(i >= 0 && i <= 3);
            return new FVector(this[i, 0], this[i, 1], this[i, 2]);
        }

        /// <summary>
        /// get a column of this matrix
        /// </summary>
        /// <param name="i">index into the column of the matrix</param>
        /// <returns>vector of the column </returns>
        public FVector GetColumn(int i)
        {
            Debug.Assert(i >= 0 && i <= 3);
            return new FVector(this[0, i], this[1, i], this[2, i]);
        }

        /// <summary>
        /// Returns a rotator representation of this matrix
        /// </summary>
        public FRotator Rotator()
        {
            FVector xAxis = GetScaledAxis(EAxis.X);
            FVector yAxis = GetScaledAxis(EAxis.Y);
            FVector zAxis = GetScaledAxis(EAxis.Z);

            FRotator rotator = new FRotator(
                FMath.Atan2(xAxis.Z, FMath.Sqrt(FMath.Square(xAxis.X) + FMath.Square(xAxis.Y))) * 180.0f / FMath.PI,
                FMath.Atan2(xAxis.Y, xAxis.X) * 180.0f / FMath.PI,
                0);

            FVector syAxis = FMatrix.CreateRotation(rotator).GetScaledAxis(EAxis.Y);
            rotator.Roll = FMath.Atan2(zAxis | syAxis, yAxis | syAxis) * 180.0f / FMath.PI;

            rotator.DiagnosticCheckNaN();
            return rotator;
        }

        /// <summary>
        /// Transform a rotation matrix into a quaternion.
        /// 
        /// @warning rotation part will need to be unit length for this to be right!
        /// </summary>
        public FQuat ToQuat()
        {
            FQuat result = new FQuat(this);
            return result;
        }

        private static bool MakeFrustumPlane(float a, float b, float c, float d, out FPlane plane)
        {
            float LengthSquared = a * a + b * b + c * c;
            if (LengthSquared > FMath.Delta * FMath.Delta)
            {
                float InvLength = FMath.InvSqrt(LengthSquared);
                plane = new FPlane(-a * InvLength, -b * InvLength, -c * InvLength, d * InvLength);
                return true;
            }
            else
            {
                plane = default(FPlane);
                return false;
            }
        }

        /// <summary>
        /// Frustum plane extraction.
        /// </summary>
        /// <param name="plane">the near plane of the Frustum of this matrix</param>
        public bool GetFrustumNearPlane(out FPlane plane)
        {
            return MakeFrustumPlane(M13, M23, M33, M43, out plane);
        }

        /// <summary>
        /// Frustum plane extraction.
        /// </summary>
        /// <param name="plane">the far plane of the Frustum of this matrix</param>
        public bool GetFrustumFarPlane(out FPlane plane)
        {
            return MakeFrustumPlane(M14 - M13, M24 - M23, M34 - M33, M44 - M43, out plane);
        }

        /// <summary>
        /// Frustum plane extraction.
        /// </summary>
        /// <param name="plane">the left plane of the Frustum of this matrix</param>
        public bool GetFrustumLeftPlane(out FPlane plane)
        {
            return MakeFrustumPlane(M14 + M11, M24 + M21, M34 + M31, M44 + M41, out plane);
        }

        /// <summary>
        /// Frustum plane extraction.
        /// </summary>
        /// <param name="plane">the right plane of the Frustum of this matrix</param>
        public bool GetFrustumRightPlane(out FPlane plane)
        {
            return MakeFrustumPlane(M14 - M11, M24 - M21, M34 - M31, M44 - M41, out plane);
        }

        /// <summary>
        /// Frustum plane extraction.
        /// </summary>
        /// <param name="plane">the top plane of the Frustum of this matrix</param>
        public bool GetFrustumTopPlane(out FPlane plane)
        {
            return MakeFrustumPlane(M14 - M12, M24 - M22, M34 - M32, M44 - M42, out plane);
        }

        /// <summary>
        /// Frustum plane extraction.
        /// </summary>
        /// <param name="plane">the bottom plane of the Frustum of this matrix</param>
        public bool GetFrustumBottomPlane(out FPlane plane)
        {
            return MakeFrustumPlane(M14 + M12, M24 + M22, M34 + M32, M44 + M42, out plane);
        }

        /// <summary>
        /// Utility for mirroring this transform across a certain plane, and flipping one of the axis as well.
        /// </summary>
        public void Mirror(EAxis mirrorAxis, EAxis flipAxis)
        {
            if (mirrorAxis == EAxis.X)
            {
                M11 *= -1.0f;
                M21 *= -1.0f;
                M31 *= -1.0f;

                M41 *= -1.0f;
            }
            else if (mirrorAxis == EAxis.Y)
            {
                M12 *= -1.0f;
                M22 *= -1.0f;
                M32 *= -1.0f;

                M42 *= -1.0f;
            }
            else if (mirrorAxis == EAxis.Z)
            {
                M13 *= -1.0f;
                M23 *= -1.0f;
                M33 *= -1.0f;

                M43 *= -1.0f;
            }

            if (flipAxis == EAxis.X)
            {
                M11 *= -1.0f;
                M12 *= -1.0f;
                M13 *= -1.0f;
            }
            else if (flipAxis == EAxis.Y)
            {
                M21 *= -1.0f;
                M22 *= -1.0f;
                M23 *= -1.0f;
            }
            else if (flipAxis == EAxis.Z)
            {
                M31 *= -1.0f;
                M32 *= -1.0f;
                M33 *= -1.0f;
            }
        }

        public override string ToString()
        {
            //%3.3f (%g is actually used here)
            string fmt = "000.000";

            string output =
                "[" + M11.ToString(fmt) + " " + M12.ToString(fmt) + " " + M13.ToString(fmt) + " " + M14.ToString(fmt) + "] " +
                "[" + M21.ToString(fmt) + " " + M22.ToString(fmt) + " " + M23.ToString(fmt) + " " + M24.ToString(fmt) + "] " +
                "[" + M31.ToString(fmt) + " " + M32.ToString(fmt) + " " + M33.ToString(fmt) + " " + M34.ToString(fmt) + "] " +
                "[" + M41.ToString(fmt) + " " + M42.ToString(fmt) + " " + M43.ToString(fmt) + " " + M44.ToString(fmt) + "]";

            return output;
        }

        /// <summary>
        /// Output ToString
        /// </summary>
        public void DebugPrint()
        {
            FMessage.Log(FMath.LogUnrealMath, ELogVerbosity.Log, ToString());
        }

        /// <summary>
        /// For debugging purpose, could be changed
        /// </summary>
        public uint ComputeHash()
        {
            uint ret = 0;

            for (int i = 0; i < 16; ++i)
            {
                float val = this[i];
                unsafe
                {
                    ret ^= *((uint*)&val);
                }
            }

            return ret;
        }

        /// <summary>
        /// Convert this Atom to the 3x4 transpose of the transformation matrix.
        /// </summary>
        public void To3x4MatrixTranspose(ref FMatrix result)
        {
            result[0] = this[0];    // [0][0]
            result[1] = this[4];    // [1][0]
            result[2] = this[8];    // [2][0]
            result[3] = this[12];   // [3][0]

            result[4] = this[1];    // [0][1]
            result[5] = this[5];    // [1][1]
            result[6] = this[9];    // [2][1]
            result[7] = this[13];   // [3][1]

            result[8] = this[2];    // [0][2]
            result[9] = this[6];    // [1][2]
            result[10] = this[10];  // [2][2]
            result[11] = this[14];  // [3][2]
        }

        /// <summary>
        /// very high quality 4x4 matrix inverse
        /// </summary>
        public static double[] Inverse4x4(ref FMatrix src)
        {
            double[] dst = new double[16];
            Inverse4x4(ref dst, ref src);
            return dst;
        }

        /// <summary>
        /// very high quality 4x4 matrix inverse
        /// </summary>
        public static void Inverse4x4(ref double[] dst, ref FMatrix src)
        {
            double s0 = (double)(src[0]); double s1 = (double)(src[1]); double s2 = (double)(src[2]); double s3 = (double)(src[3]);
            double s4 = (double)(src[4]); double s5 = (double)(src[5]); double s6 = (double)(src[6]); double s7 = (double)(src[7]);
            double s8 = (double)(src[8]); double s9 = (double)(src[9]); double s10 = (double)(src[10]); double s11 = (double)(src[11]);
            double s12 = (double)(src[12]); double s13 = (double)(src[13]); double s14 = (double)(src[14]); double s15 = (double)(src[15]);

            double[] inv = new double[16];
            inv[0] = s5 * s10 * s15 - s5 * s11 * s14 - s9 * s6 * s15 + s9 * s7 * s14 + s13 * s6 * s11 - s13 * s7 * s10;
            inv[1] = -s1 * s10 * s15 + s1 * s11 * s14 + s9 * s2 * s15 - s9 * s3 * s14 - s13 * s2 * s11 + s13 * s3 * s10;
            inv[2] = s1 * s6 * s15 - s1 * s7 * s14 - s5 * s2 * s15 + s5 * s3 * s14 + s13 * s2 * s7 - s13 * s3 * s6;
            inv[3] = -s1 * s6 * s11 + s1 * s7 * s10 + s5 * s2 * s11 - s5 * s3 * s10 - s9 * s2 * s7 + s9 * s3 * s6;
            inv[4] = -s4 * s10 * s15 + s4 * s11 * s14 + s8 * s6 * s15 - s8 * s7 * s14 - s12 * s6 * s11 + s12 * s7 * s10;
            inv[5] = s0 * s10 * s15 - s0 * s11 * s14 - s8 * s2 * s15 + s8 * s3 * s14 + s12 * s2 * s11 - s12 * s3 * s10;
            inv[6] = -s0 * s6 * s15 + s0 * s7 * s14 + s4 * s2 * s15 - s4 * s3 * s14 - s12 * s2 * s7 + s12 * s3 * s6;
            inv[7] = s0 * s6 * s11 - s0 * s7 * s10 - s4 * s2 * s11 + s4 * s3 * s10 + s8 * s2 * s7 - s8 * s3 * s6;
            inv[8] = s4 * s9 * s15 - s4 * s11 * s13 - s8 * s5 * s15 + s8 * s7 * s13 + s12 * s5 * s11 - s12 * s7 * s9;
            inv[9] = -s0 * s9 * s15 + s0 * s11 * s13 + s8 * s1 * s15 - s8 * s3 * s13 - s12 * s1 * s11 + s12 * s3 * s9;
            inv[10] = s0 * s5 * s15 - s0 * s7 * s13 - s4 * s1 * s15 + s4 * s3 * s13 + s12 * s1 * s7 - s12 * s3 * s5;
            inv[11] = -s0 * s5 * s11 + s0 * s7 * s9 + s4 * s1 * s11 - s4 * s3 * s9 - s8 * s1 * s7 + s8 * s3 * s5;
            inv[12] = -s4 * s9 * s14 + s4 * s10 * s13 + s8 * s5 * s14 - s8 * s6 * s13 - s12 * s5 * s10 + s12 * s6 * s9;
            inv[13] = s0 * s9 * s14 - s0 * s10 * s13 - s8 * s1 * s14 + s8 * s2 * s13 + s12 * s1 * s10 - s12 * s2 * s9;
            inv[14] = -s0 * s5 * s14 + s0 * s6 * s13 + s4 * s1 * s14 - s4 * s2 * s13 - s12 * s1 * s6 + s12 * s2 * s5;
            inv[15] = s0 * s5 * s10 - s0 * s6 * s9 - s4 * s1 * s10 + s4 * s2 * s9 + s8 * s1 * s6 - s8 * s2 * s5;

            double det = s0 * inv[0] + s1 * inv[4] + s2 * inv[8] + s3 * inv[12];
            if (det != 0.0)
            {
                det = 1.0 / det;
            }

            if (dst == null || dst.Length < 16)
            {
                dst = new double[16];
            }
            for (int i = 0; i < 16; i++)
            {
                dst[i] = inv[i] * det;
            }
        }

        // These methods are for creating a FMatrix. In C++ they map to the following classes:
        // FRotationMatrix, FRotationTranslationMatrix, FTranslationMatrix, FScaleMatrix, FScaleRotationTranslationMatrix,
        // FQuatRotationTranslationMatrix, FQuatRotationMatrix, FInverseRotationMatrix, FRotationAboutPointMatrix, FMirrorMatrix, 
        // FOrthoMatrix, FPerspectiveMatrix, FClipProjectionMatrix

        // [X] FRotationMatrix
        // [X] FRotationTranslationMatrix
        // [X] FTranslationMatrix
        // [X] FScaleMatrix
        // [X] FScaleRotationTranslationMatrix
        // [X] FQuatRotationTranslationMatrix
        // [X] FQuatRotationMatrix
        // [X] FInverseRotationMatrix
        // [X] FRotationAboutPointMatrix
        // [X] FMirrorMatrix
        // [X] FOrthoMatrix
        // [X] FPerspectiveMatrix
        // [ ] FClipProjectionMatrix

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Math\RotationTranslationMatrix.h
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Combined rotation and translation matrix
        /// </summary>
        /// <param name="rot">rotation</param>
        /// <param name="origin">translation to apply</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateRotationTranslation(FRotator rot, FVector origin)
        {
            float sp, sy, sr;
            float cp, cy, cr;
            FMath.SinCos(out sp, out cp, FMath.DegreesToRadians(rot.Pitch));
            FMath.SinCos(out sy, out cy, FMath.DegreesToRadians(rot.Yaw));
            FMath.SinCos(out sr, out cr, FMath.DegreesToRadians(rot.Roll));

            FMatrix result;

            result.M11 = cp * cy;
            result.M12 = cp * sy;
            result.M13 = sp;
            result.M14 = 0.0f;

            result.M21 = sr * sp * cy - cr * sy;
            result.M22 = sr * sp * sy + cr * cy;
            result.M23 = -sr * cp;
            result.M24 = 0.0f;

            result.M31 = -(cr * sp * cy + sr * sy);
            result.M32 = cy * sr - cr * sp * sy;
            result.M33 = cr * cp;
            result.M34 = 0.0f;

            result.M41 = origin.X;
            result.M42 = origin.Y;
            result.M43 = origin.Z;
            result.M44 = 1.0f;

            return result;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Math\RotationMatrix.h
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Rotation matrix no translation
        /// </summary>
        /// <param name="rot">rotation</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateRotation(FRotator rot)
        {
            return CreateRotationTranslation(rot, FVector.ZeroVector);
        }

        /// <summary>
        /// Builds a rotation matrix given only a XAxis. Y and Z are unspecified but will be orthonormal. XAxis need not be normalized.
        /// </summary>
        public static FMatrix CreateRotationX(FVector xAxis)
        {
            FVector newX = xAxis.GetSafeNormal();

            // try to use up if possible
            FVector upVector = (FMath.Abs(newX.Z) < (1.0f - FMath.KindaSmallNumber)) ? new FVector(0, 0, 1.0f) : new FVector(1.0f, 0, 0);

            FVector newY = (upVector ^ newX).GetSafeNormal();
            FVector newZ = newX ^ newY;

            return new FMatrix(newX, newY, newZ, FVector.ZeroVector);
        }

        /// <summary>
        /// Builds a rotation matrix given only a YAxis. X and Z are unspecified but will be orthonormal. YAxis need not be normalized.
        /// </summary>
        public static FMatrix CreateRotationY(FVector yAxis)
        {
            FVector newY = yAxis.GetSafeNormal();

            // try to use up if possible
            FVector upVector = (FMath.Abs(newY.Z) < (1.0f - FMath.KindaSmallNumber)) ? new FVector(0, 0, 1.0f) : new FVector(1.0f, 0, 0);

            FVector newZ = (upVector ^ newY).GetSafeNormal();
            FVector newX = newY ^ newZ;

            return new FMatrix(newX, newY, newZ, FVector.ZeroVector);
        }

        /// <summary>
        /// Builds a rotation matrix given only a ZAxis. X and Y are unspecified but will be orthonormal. ZAxis need not be normalized.
        /// </summary>
        public static FMatrix CreateRotationZ(FVector zAxis)
        {
            FVector newZ = zAxis.GetSafeNormal();

            // try to use up if possible
            FVector upVector = (FMath.Abs(newZ.Z) < (1.0f - FMath.KindaSmallNumber)) ? new FVector(0, 0, 1.0f) : new FVector(1.0f, 0, 0);

            FVector newX = (upVector ^ newZ).GetSafeNormal();
            FVector newY = newZ ^ newX;

            return new FMatrix(newX, newY, newZ, FVector.ZeroVector);
        }

        /// <summary>
        /// Builds a matrix with given X and Y axes. X will remain fixed, Y may be changed minimally to enforce orthogonality. Z will be computed. Inputs need not be normalized.
        /// </summary>
        public static FMatrix CreateRotationXY(FVector xAxis, FVector yAxis)
        {
            FVector NewX = xAxis.GetSafeNormal();
            FVector Norm = yAxis.GetSafeNormal();

            // if they're almost same, we need to find arbitrary vector
            if (FMath.IsNearlyEqual(FMath.Abs(NewX | Norm), 1.0f))
            {
                // make sure we don't ever pick the same as NewX
                Norm = (FMath.Abs(NewX.Z) < (1.0f - FMath.KindaSmallNumber)) ? new FVector(0, 0, 1.0f) : new FVector(1.0f, 0, 0);
            }

            FVector NewZ = (NewX ^ Norm).GetSafeNormal();
            FVector NewY = NewZ ^ NewX;

            return new FMatrix(NewX, NewY, NewZ, FVector.ZeroVector);
        }

        /// <summary>
        /// Builds a matrix with given X and Z axes. X will remain fixed, Z may be changed minimally to enforce orthogonality. Y will be computed. Inputs need not be normalized.
        /// </summary>
        public static FMatrix CreateRotationXZ(FVector xAxis, FVector zAxis)
        {
            FVector NewX = xAxis.GetSafeNormal();
            FVector Norm = zAxis.GetSafeNormal();

            // if they're almost same, we need to find arbitrary vector
            if (FMath.IsNearlyEqual(FMath.Abs(NewX | Norm), 1.0f))
            {
                // make sure we don't ever pick the same as NewX
                Norm = (FMath.Abs(NewX.Z) < (1.0f - FMath.KindaSmallNumber)) ? new FVector(0, 0, 1.0f) : new FVector(1.0f, 0, 0);
            }

            FVector NewY = (Norm ^ NewX).GetSafeNormal();
            FVector NewZ = NewX ^ NewY;

            return new FMatrix(NewX, NewY, NewZ, FVector.ZeroVector);
        }

        /// <summary>
        /// Builds a matrix with given Y and X axes. Y will remain fixed, X may be changed minimally to enforce orthogonality. Z will be computed. Inputs need not be normalized.
        /// </summary>
        public static FMatrix CreateRotationYX(FVector yAxis, FVector xAxis)
        {
            FVector NewY = yAxis.GetSafeNormal();
            FVector Norm = xAxis.GetSafeNormal();

            // if they're almost same, we need to find arbitrary vector
            if (FMath.IsNearlyEqual(FMath.Abs(NewY | Norm), 1.0f))
            {
                // make sure we don't ever pick the same as NewX
                Norm = (FMath.Abs(NewY.Z) < (1.0f - FMath.KindaSmallNumber)) ? new FVector(0, 0, 1.0f) : new FVector(1.0f, 0, 0);
            }

            FVector NewZ = (Norm ^ NewY).GetSafeNormal();
            FVector NewX = NewY ^ NewZ;

            return new FMatrix(NewX, NewY, NewZ, FVector.ZeroVector);
        }

        /// <summary>
        /// Builds a matrix with given Y and Z axes. Y will remain fixed, Z may be changed minimally to enforce orthogonality. X will be computed. Inputs need not be normalized.
        /// </summary>
        public static FMatrix CreateRotationYZ(FVector yAxis, FVector zAxis)
        {
            FVector NewY = yAxis.GetSafeNormal();
            FVector Norm = zAxis.GetSafeNormal();

            // if they're almost same, we need to find arbitrary vector
            if (FMath.IsNearlyEqual(FMath.Abs(NewY | Norm), 1.0f))
            {
                // make sure we don't ever pick the same as NewX
                Norm = (FMath.Abs(NewY.Z) < (1.0f - FMath.KindaSmallNumber)) ? new FVector(0, 0, 1.0f) : new FVector(1.0f, 0, 0);
            }

            FVector NewX = (NewY ^ Norm).GetSafeNormal();
            FVector NewZ = NewX ^ NewY;

            return new FMatrix(NewX, NewY, NewZ, FVector.ZeroVector);
        }

        /// <summary>
        /// Builds a matrix with given Z and X axes. Z will remain fixed, X may be changed minimally to enforce orthogonality. Y will be computed. Inputs need not be normalized.
        /// </summary>
        public static FMatrix CreateRotationZX(FVector zAxis, FVector xAxis)
        {
            FVector NewZ = zAxis.GetSafeNormal();
            FVector Norm = xAxis.GetSafeNormal();

            // if they're almost same, we need to find arbitrary vector
            if (FMath.IsNearlyEqual(FMath.Abs(NewZ | Norm), 1.0f))
            {
                // make sure we don't ever pick the same as NewX
                Norm = (FMath.Abs(NewZ.Z) < (1.0f - FMath.KindaSmallNumber)) ? new FVector(0, 0, 1.0f) : new FVector(1.0f, 0, 0);
            }

            FVector NewY = (NewZ ^ Norm).GetSafeNormal();
            FVector NewX = NewY ^ NewZ;

            return new FMatrix(NewX, NewY, NewZ, FVector.ZeroVector);
        }

        /// <summary>
        /// Builds a matrix with given Z and Y axes. Z will remain fixed, Y may be changed minimally to enforce orthogonality. X will be computed. Inputs need not be normalized.
        /// </summary>
        public static FMatrix CreateRotationZY(FVector zAxis, FVector yAxis)
        {
            FVector NewZ = zAxis.GetSafeNormal();
            FVector Norm = yAxis.GetSafeNormal();

            // if they're almost same, we need to find arbitrary vector
            if (FMath.IsNearlyEqual(FMath.Abs(NewZ | Norm), 1.0f))
            {
                // make sure we don't ever pick the same as NewX
                Norm = (FMath.Abs(NewZ.Z) < (1.0f - FMath.KindaSmallNumber)) ? new FVector(0, 0, 1.0f) : new FVector(1.0f, 0, 0);
            }

            FVector NewX = (Norm ^ NewZ).GetSafeNormal();
            FVector NewY = NewZ ^ NewX;

            return new FMatrix(NewX, NewY, NewZ, FVector.ZeroVector);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Math\TranslationMatrix.h
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create a translation matrix based on given vector
        /// </summary>
        public static FMatrix CreateTranslation(FVector v)
        {
            return new FMatrix(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f, 0.0f,
                v.X, v.Y, v.Z, 1.0f);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Math\ScaleMatrix.h
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create a Scale matrix.
        /// </summary>
        /// <param name="scale">uniform scale to apply to matrix.</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateScale(float scale)
        {
            return new FMatrix(
                scale, 0.0f, 0.0f, 0.0f,
                0.0f, scale, 0.0f, 0.0f,
                0.0f, 0.0f, scale, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        /// Create a Scale matrix.
        /// </summary>
        /// <param name="scale">Non-uniform scale to apply to matrix.</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateScale(FVector scale)
        {
            return new FMatrix(
                scale.X, 0.0f, 0.0f, 0.0f,
                0.0f, scale.Y, 0.0f, 0.0f,
                0.0f, 0.0f, scale.Z, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Math\ScaleRotationTranslationMatrix.h
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void GetSinCos(out float s, out float c, float degrees)
        {
            if (degrees == 0.0f)
            {
                s = 0.0f;
                c = 1.0f;
            }
            else if (degrees == 90.0f)
            {
                s = 1.0f;
                c = 0.0f;
            }
            else if (degrees == 180.0f)
            {
                s = 0.0f;
                c = -1.0f;
            }
            else if (degrees == 270.0f)
            {
                s = -1.0f;
                c = 0.0f;
            }
            else
            {
                FMath.SinCos(out s, out c, FMath.DegreesToRadians(degrees));
            }
        }

        /// <summary>
        /// Create a combined Scale rotation and translation matrix
        /// </summary>
        /// <param name="scale">scale to apply to matrix</param>
        /// <param name="rot">rotation</param>
        /// <param name="origin">translation to apply</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateScaleRotationTranslation(FVector scale, FRotator rot, FVector origin)
        {
            float sp, sy, sr;
            float cp, cy, cr;
            GetSinCos(out sp, out cp, rot.Pitch);
            GetSinCos(out sy, out cy, rot.Yaw);
            GetSinCos(out sr, out cr, rot.Roll);

            FMatrix result;

            result.M11 = (cp * cy) * scale.X;
            result.M12 = (cp * sy) * scale.X;
            result.M13 = (sp) * scale.X;
            result.M14 = 0.0f;

            result.M21 = (sr * sp * cy - cr * sy) * scale.Y;
            result.M22 = (sr * sp * sy + cr * cy) * scale.Y;
            result.M23 = (-sr * cp) * scale.Y;
            result.M24 = 0.0f;

            result.M31 = (-(cr * sp * cy + sr * sy)) * scale.Z;
            result.M32 = (cy * sr - cr * sp * sy) * scale.Z;
            result.M33 = (cr * cp) * scale.Z;
            result.M34 = 0.0f;

            result.M41 = origin.X;
            result.M42 = origin.Y;
            result.M43 = origin.Z;
            result.M44 = 1.0f;

            return result;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Math\QuatRotationTranslationMatrix.h
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create a rotation and translation matrix using quaternion rotation
        /// </summary>
        /// <param name="q">rotation</param>
        /// <param name="origin">translation to apply</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateQuatRotationTranslation(FQuat q, FVector origin)
        {
            // #if !(UE_BUILD_SHIPPING || UE_BUILD_TEST) && WITH_EDITORONLY_DATA
            // Make sure Quaternion is normalized
            FMessage.EnsureDebug(q.IsNormalized(), "Make sure Quaternion is normalized");
            // #endif

            float x2 = q.X + q.X; float y2 = q.Y + q.Y; float z2 = q.Z + q.Z;
            float xx = q.X * x2; float xy = q.X * y2; float xz = q.X * z2;
            float yy = q.Y * y2; float yz = q.Y * z2; float zz = q.Z * z2;
            float wx = q.W * x2; float wy = q.W * y2; float wz = q.W * z2;

            FMatrix result;

            result.M11 = 1.0f - (yy + zz);
            result.M21 = xy - wz;
            result.M31 = xz + wy;
            result.M41 = origin.X;

            result.M12 = xy + wz;
            result.M22 = 1.0f - (xx + zz);
            result.M32 = yz - wx;
            result.M42 = origin.Y;

            result.M13 = xz - wy;
            result.M23 = yz + wx;
            result.M33 = 1.0f - (xx + yy);
            result.M43 = origin.Z;

            result.M14 = 0.0f;
            result.M24 = 0.0f;
            result.M34 = 0.0f;
            result.M44 = 1.0f;

            return result;
        }

        /// <summary>
        /// Create a rotation matrix using quaternion rotation
        /// </summary>
        /// <param name="q">rotation</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateQuatRotation(FQuat q)
        {
            return CreateQuatRotationTranslation(q, FVector.ZeroVector);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Math\InverseRotationMatrix.h
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public static FMatrix CreateInverseRotation(FRotator rot)
        {
            FMatrix yaw = new FMatrix(
                +FMath.Cos(rot.Yaw * FMath.PI / 180.0f), -FMath.Sin(rot.Yaw * FMath.PI / 180.0f), 0.0f, 0.0f,
                +FMath.Sin(rot.Yaw * FMath.PI / 180.0f), +FMath.Cos(rot.Yaw * FMath.PI / 180.0f), 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);

            FMatrix pitch = new FMatrix(
                +FMath.Cos(rot.Pitch * FMath.PI / 180.0f), 0.0f, -FMath.Sin(rot.Pitch * FMath.PI / 180.0f), 0.0f,
                0.0f, 1.0f, 0.0f, 0.0f,
                +FMath.Sin(rot.Pitch * FMath.PI / 180.0f), 0.0f, +FMath.Cos(rot.Pitch * FMath.PI / 180.0f), 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);

            FMatrix roll = new FMatrix(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, +FMath.Cos(rot.Roll * FMath.PI / 180.0f), +FMath.Sin(rot.Roll * FMath.PI / 180.0f), 0.0f,
                0.0f, -FMath.Sin(rot.Roll * FMath.PI / 180.0f), +FMath.Cos(rot.Roll * FMath.PI / 180.0f), 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);

            return yaw * pitch * roll;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Math\RotationAboutPointMatrix.h
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Rotates about an Origin point.
        /// </summary>
        /// <param name="rot">rotation</param>
        /// <param name="origin">about which to rotate.</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateRotationAboutPoint(FRotator rot, FVector origin)
        {
            FMatrix result = CreateRotationTranslation(rot, origin);

            // FRotationTranslationMatrix generates R * T.
            // We need -T * R * T, so prepend that translation:
            FVector XAxis = new FVector(result.M11, result.M21, result.M31);
            FVector YAxis = new FVector(result.M12, result.M22, result.M32);
            FVector ZAxis = new FVector(result.M13, result.M23, result.M33);

            result.M41 -= XAxis | origin;
            result.M42 -= YAxis | origin;
            result.M43 -= ZAxis | origin;

            return result;
        }

        /// <summary>
        /// Rotates about an Origin point.
        /// </summary>
        /// <param name="rot">rotation</param>
        /// <param name="origin">about which to rotate.</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateRotationAboutPoint(FQuat rot, FVector origin)
        {
            return CreateRotationAboutPoint(rot.Rotator(), origin);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Math\MirrorMatrix.h
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Mirrors a point about an abitrary plane
        /// </summary>
        /// <param name="plane">source plane for mirroring (assumed normalized)</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateMirror(FPlane plane)
        {
            //check( FMath::Abs(1.f - Plane.SizeSquared()) < KINDA_SMALL_NUMBER && TEXT("not normalized"));
            return new FMatrix(
                -2.0f * plane.X * plane.X + 1.0f, -2.0f * plane.Y * plane.X, -2.0f * plane.Z * plane.X, 0.0f,
                -2.0f * plane.X * plane.Y, -2.0f * plane.Y * plane.Y + 1.0f, -2.0f * plane.Z * plane.Y, 0.0f,
                -2.0f * plane.X * plane.Z, -2.0f * plane.Y * plane.Z, -2.0f * plane.Z * plane.Z + 1.0f, 0.0f,
                 2.0f * plane.X * plane.W, 2.0f * plane.Y * plane.W, 2.0f * plane.Z * plane.W, 1.0f);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Math\OrthoMatrix.h
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create an orthographic projection matrix.
        /// </summary>
        /// <param name="width">view space width</param>
        /// <param name="height">view space height</param>
        /// <param name="zScale">scale in the Z axis</param>
        /// <param name="zOffset">offset in the Z axis</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateOrtho(float width, float height, float zScale, float zOffset)
        {
            return new FMatrix(
                (width != 0) ? (1.0f / width) : 1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, (height != 0) ? (1.0f / height) : 1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, zScale, 0.0f,
                0.0f, 0.0f, zOffset * zScale, 1.0f);
        }

        public static FMatrix ReversedZOrtho(float width, float height, float zScale, float zOffset)
        {
            return new FMatrix(
                (width != 0) ? (1.0f / width) : 1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, (height != 0) ? (1.0f / height) : 1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -zScale, 0.0f,
                0.0f, 0.0f, 1.0f - zOffset * zScale, 1.0f);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Math\PerspectiveMatrix.h
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        // Note: the value of this must match the mirror in Common.usf!
        const float Z_PRECISION	= 0.0f;

        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        /// <param name="halfFOVX">Half FOV in the X axis</param>
        /// <param name="halfFOVY">Half FOV in the Y axis</param>
        /// <param name="multFOVX">multiplier on the X axis</param>
        /// <param name="multFOVY">multiplier on the y axis</param>
        /// <param name="minZ">distance to the near Z plane</param>
        /// <param name="maxZ">distance to the far Z plane</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreatePerspective(float halfFOVX, float halfFOVY, float multFOVX, float multFOVY, float minZ, float maxZ)
        {
            return new FMatrix(
                multFOVX / FMath.Tan(halfFOVX), 0.0f, 0.0f, 0.0f,
                0.0f, multFOVY / FMath.Tan(halfFOVY), 0.0f, 0.0f,
                0.0f, 0.0f, ((minZ == maxZ) ? (1.0f - Z_PRECISION) : maxZ / (maxZ - minZ)), 1.0f,
                0.0f, 0.0f, -minZ * ((minZ == maxZ) ? (1.0f - Z_PRECISION) : maxZ / (maxZ - minZ)), 0.0f);
        }

        /// <summary>
        /// Creates a perspective projection matrix.
        /// @note that the FOV you pass in is actually half the FOV, unlike most perspective matrix functions (D3DXMatrixPerspectiveFovLH).
        /// </summary>
        /// <param name="halfFOV">half Field of View in the Y direction</param>
        /// <param name="width">view space width</param>
        /// <param name="height">view space height</param>
        /// <param name="minZ">distance to the near Z plane</param>
        /// <param name="maxZ">distance to the far Z plane</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreatePerspective(float halfFOV, float width, float height, float minZ, float maxZ)
        {
            return new FMatrix(
                1.0f / FMath.Tan(halfFOV), 0.0f, 0.0f, 0.0f,
                0.0f, width / FMath.Tan(halfFOV) / height, 0.0f, 0.0f,
                0.0f, 0.0f, ((minZ == maxZ) ? (1.0f - Z_PRECISION) : maxZ / (maxZ - minZ)), 1.0f,
                0.0f, 0.0f, -minZ * ((minZ == maxZ) ? (1.0f - Z_PRECISION) : maxZ / (maxZ - minZ)), 0.0f);
        }

        /// <summary>
        /// Creates a perspective projection matrix.
        /// @note that the FOV you pass in is actually half the FOV, unlike most perspective matrix functions (D3DXMatrixPerspectiveFovLH).
        /// </summary>
        /// <param name="halfFOV">half Field of View in the Y direction</param>
        /// <param name="width">view space width</param>
        /// <param name="height">view space height</param>
        /// <param name="minZ">distance to the near Z plane</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreatePerspective(float halfFOV, float width, float height, float minZ)
        {
            return new FMatrix(
                1.0f / FMath.Tan(halfFOV), 0.0f, 0.0f, 0.0f,
                0.0f, width / FMath.Tan(halfFOV) / height, 0.0f, 0.0f,
                0.0f, 0.0f, (1.0f - Z_PRECISION), 1.0f,
                0.0f, 0.0f, -minZ * (1.0f - Z_PRECISION), 0.0f);
        }

        // Reversed Z perspective functions

        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        /// <param name="halfFOVX">Half FOV in the X axis</param>
        /// <param name="halfFOVY">Half FOV in the Y axis</param>
        /// <param name="multFOVX">multiplier on the X axis</param>
        /// <param name="multFOVY">multiplier on the y axis</param>
        /// <param name="minZ">distance to the near Z plane</param>
        /// <param name="maxZ">distance to the far Z plane</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateReversedZPerspective(float halfFOVX, float halfFOVY, float multFOVX, float multFOVY, float minZ, float maxZ)
        {
            return new FMatrix(
                multFOVX / FMath.Tan(halfFOVX), 0.0f, 0.0f, 0.0f,
                0.0f, multFOVY / FMath.Tan(halfFOVY), 0.0f, 0.0f,
                0.0f, 0.0f, ((minZ == maxZ) ? 0.0f : minZ / (minZ - maxZ)), 1.0f,
                0.0f, 0.0f, ((minZ == maxZ) ? minZ : -maxZ * minZ / (minZ - maxZ)), 0.0f);
        }

        /// <summary>
        /// Creates a perspective projection matrix.
        /// @note that the FOV you pass in is actually half the FOV, unlike most perspective matrix functions (D3DXMatrixPerspectiveFovLH).
        /// </summary>
        /// <param name="halfFOV">half Field of View in the Y direction</param>
        /// <param name="width">view space width</param>
        /// <param name="height">view space height</param>
        /// <param name="minZ">distance to the near Z plane</param>
        /// <param name="maxZ">distance to the far Z plane</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateReversedZPerspective(float halfFOV, float width, float height, float minZ, float maxZ)
        {
            return new FMatrix(
                1.0f / FMath.Tan(halfFOV), 0.0f, 0.0f, 0.0f,
                0.0f, width / FMath.Tan(halfFOV) / height, 0.0f, 0.0f,
                0.0f, 0.0f, ((minZ == maxZ) ? 0.0f : minZ / (minZ - maxZ)), 1.0f,
                0.0f, 0.0f, ((minZ == maxZ) ? minZ : -maxZ * minZ / (minZ - maxZ)), 0.0f);
        }

        /// <summary>
        /// Creates a perspective projection matrix.
        /// @note that the FOV you pass in is actually half the FOV, unlike most perspective matrix functions (D3DXMatrixPerspectiveFovLH).
        /// </summary>
        /// <param name="halfFOV">half Field of View in the Y direction</param>
        /// <param name="width">view space width</param>
        /// <param name="height">view space height</param>
        /// <param name="minZ">distance to the near Z plane</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateReversedZPerspective(float halfFOV, float width, float height, float minZ)
        {
            return new FMatrix(
                1.0f / FMath.Tan(halfFOV), 0.0f, 0.0f, 0.0f,
                0.0f, width / FMath.Tan(halfFOV) / height, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, minZ, 0.0f);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Math\ClipProjectionMatrix.h
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Realigns the near plane for an existing projection matrix
        /// with an arbitrary clip plane
        /// from: http://sourceforge.net/mailarchive/message.php?msg_id=000901c26324%242181ea90%24a1e93942%40firefly
        /// Updated for the fact that our FPlane uses Ax+By+Cz=D.
        /// </summary>
        /// <param name="srcProjMat">source projection matrix to premultiply with the clip matrix</param>
        /// <param name="plane">clipping plane used to build the clip matrix (assumed to be in camera space)</param>
        /// <returns>The matrix.</returns>
        public static FMatrix CreateClipProjection(FMatrix srcProjMat, FPlane plane)
        {
            FMatrix result = srcProjMat;

            // Calculate the clip-space corner point opposite the clipping plane
            // as (sgn(clipPlane.x), sgn(clipPlane.y), 1, 1) and
            // transform it into camera space by multiplying it
            // by the inverse of the projection matrix
            FPlane CornerPlane = new FPlane(
                        sgn(plane.X) / srcProjMat.M11,
                        sgn(plane.Y) / srcProjMat.M22,
                        1.0f,
                        -(1.0f - srcProjMat.M33) / srcProjMat.M43);

            // Calculate the scaled plane vector
            FPlane projPlane = (plane * (1.0f / (plane | CornerPlane)));

            // use the projected space clip plane in z column 
            // Note: (account for our negated W coefficient)
            result.M13 = projPlane.X;
            result.M23 = projPlane.Y;
            result.M33 = projPlane.Z;
            result.M43 = -projPlane.W;

            return result;
        }

        private static float sgn(float a)
        {
            if (a > 0.0f) return (1.0f);
            if (a < 0.0f) return (-1.0f);
            return (0.0f);
        }
    }
}
