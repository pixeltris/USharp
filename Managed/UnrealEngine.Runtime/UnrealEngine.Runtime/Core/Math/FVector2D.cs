using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// A point or direction FVector in 2d space.
    /// The full C++ class is located here: Engine\Source\Runtime\Core\Public\Math\Vector2D.h
    /// </summary>
    [UStruct(Flags = 0x0040EC38), BlueprintType, UMetaPath("/Script/CoreUObject.Vector2D", "CoreUObject", UnrealModuleType.Engine)]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FVector2D : IEquatable<FVector2D>
    {
        static bool X_IsValid;
        static int X_Offset;
        /// <summary>
        /// Vector's X component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector2D:X")]
        public float X;

        static bool Y_IsValid;
        static int Y_Offset;
        /// <summary>
        /// Vector's Y component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector2D:Y")]
        public float Y;

        static int FVector2D_StructSize;

        public FVector2D Copy()
        {
            FVector2D result = this;
            return result;
        }

        static FVector2D()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FVector2D)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FVector2D));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Vector2D");
            FVector2D_StructSize = NativeReflection.GetStructSize(classAddress);
            X_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "X");
            X_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "X", Classes.UFloatProperty);
            Y_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Y");
            Y_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Y", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FVector2D));
        }

        /// <summary>
        /// Global 2D zero vector constant (0,0)
        /// </summary>
        public static readonly FVector2D ZeroVector = new FVector2D(0, 0);

        /// <summary>
        /// Global 2D unit vector constant (1,1)
        /// </summary>
        public static readonly FVector2D UnitVector = new FVector2D(1, 1);

        /// <summary>
        /// Constructor using initial values for each component.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public FVector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs a vector from an FIntPoint.
        /// </summary>
        /// <param name="pos">Integer point used to set this vector.</param>
        public FVector2D(FIntPoint pos)
        {
            X = pos.X;
            Y = pos.Y;
        }

        /// <summary>
        /// Constructs a vector from an FVector.
        /// Copies the X and Y components from the FVector.
        /// </summary>
        /// <param name="v">Vector to copy from.</param>
        public FVector2D(FVector v)
        {
            X = v.X;
            Y = v.Y;
        }

        /// <summary>
        /// Gets the result of adding two vectors together.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of adding the vectors together.</returns>
        public static FVector2D operator +(FVector2D a, FVector2D b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of adding two vectors together.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of adding the vectors together.</returns>
        public static FVector2D Add(FVector2D a, FVector2D b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of adding two vectors together.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The result of adding the vectors together.</param>
        public static void Add(ref FVector2D a, ref FVector2D b, out FVector2D result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
        }

        /// <summary>
        /// Gets the result of subtracting two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of vector subtraction.</returns>
        public static FVector2D operator -(FVector2D a, FVector2D b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of subtracting two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of vector subtraction.</returns>
        public static FVector2D Subtract(FVector2D a, FVector2D b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of subtracting two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The result of vector subtraction.</param>
        public static void Subtract(ref FVector2D a, ref FVector2D b, out FVector2D result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
        }

        /// <summary>
        /// Gets the result of scaling the vector (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">How much to scale the vector by.</param>
        /// <returns>The result of scaling the vector.</returns>
        public static FVector2D operator *(float scale, FVector2D v)
        {
            Multiply(ref v, scale, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of scaling the vector (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">How much to scale the vector by.</param>
        /// <returns>The result of scaling the vector.</returns>
        public static FVector2D operator *(FVector2D v, float scale)
        {
            Multiply(ref v, scale, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of scaling the vector (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">How much to scale the vector by.</param>
        /// <returns>The result of scaling the vector.</returns>
        public static FVector2D Multiply(FVector2D v, float scale)
        {
            Multiply(ref v, scale, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of scaling the vector (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">How much to scale the vector by.</param>
        /// <param name="result">The result of scaling the vector.</param>
        public static void Multiply(ref FVector2D v, float scale, out FVector2D result)
        {
            result.X = v.X * scale;
            result.Y = v.Y * scale;
        }

        /// <summary>
        /// Gets the result of dividing each component of the vector by a value.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">Scale How much to divide the vector by.</param>
        /// <returns>The result of division on the vector.</returns>
        public static FVector2D operator /(FVector2D v, float scale)
        {
            Divide(ref v, scale, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of dividing each component of the vector by a value.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">Scale How much to divide the vector by.</param>
        /// <returns>The result of division on the vector.</returns>
        public static FVector2D Divide(FVector2D v, float scale)
        {
            Divide(ref v, scale, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of dividing each component of the vector by a value.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">Scale How much to divide the vector by.</param>
        /// <param name="result">The result of division on the vector.</param>
        public static void Divide(ref FVector2D v, float scale, out FVector2D result)
        {
            float factor = 1.0f / scale;
            result.X = v.X * factor;
            result.Y = v.Y * factor;
        }

        /// <summary>
        /// Gets the result of a vector + float F
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="f">Float to add to each component.</param>
        /// <returns>The result of the vector + float F.</returns>
        public static FVector2D operator +(FVector2D v, float f)
        {
            Add(ref v, f, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of a vector + float F
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="f">Float to add to each component.</param>
        /// <returns>The result of the vector + float F.</returns>
        public static FVector2D Add(FVector2D v, float f)
        {
            Add(ref v, f, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of adding to each component of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="f">Float to add to each component.</param>
        /// <param name="result">The result of the vector + float F.</param>
        public static void Add(ref FVector2D v, float f, out FVector2D result)
        {
            result.X = v.X + f;
            result.Y = v.Y + f;
        }

        /// <summary>
        /// Gets the result of subtracting from each component of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="f">Float to subtract to each component.</param>
        /// <returns>The result of the vector - float F.</returns>
        public static FVector2D operator -(FVector2D v, float f)
        {
            Subtract(ref v, f, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of subtracting from each component of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="f">Float to subtract to each component.</param>
        /// <returns>The result of the vector - float F.</returns>
        public static FVector2D Subtract(FVector2D v, float f)
        {
            Subtract(ref v, f, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of subtracting from each component of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="f">Float to subtract to each component.</param>
        /// <param name="result">The result of the vector - float F.</param>
        public static void Subtract(ref FVector2D v, float f, out FVector2D result)
        {
            result.X = v.X - f;
            result.Y = v.Y - f;
        }

        /// <summary>
        /// Gets the result of component-wise multiplication of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of multiplication.</returns>
        public static FVector2D operator *(FVector2D a, FVector2D b)
        {
            Multiply(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise multiplication of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of multiplication.</returns>
        public static FVector2D Multiply(FVector2D a, FVector2D b)
        {
            Multiply(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise multiplication of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The result of multiplication.</param>
        public static void Multiply(ref FVector2D a, ref FVector2D b, out FVector2D result)
        {
            result.X = a.X * b.X;
            result.Y = a.Y * b.Y;
        }

        /// <summary>
        /// Gets the result of component-wise division of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of division.</returns>
        public static FVector2D operator /(FVector2D a, FVector2D b)
        {
            Divide(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise division of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of division.</returns>
        public static FVector2D Divide(FVector2D a, FVector2D b)
        {
            Divide(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise division of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of division.</returns>
        public static void Divide(ref FVector2D a, ref FVector2D b, out FVector2D result)
        {
            result.X = a.X / b.X;
            result.Y = a.Y / b.Y;
        }

        /// <summary>
        /// Calculate the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static float operator |(FVector2D a, FVector2D b)
        {
            return DotProduct(ref a, ref b);
        }

        /// <summary>
        /// Calculate the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static float DotProduct(FVector2D a, FVector2D b)
        {
            return DotProduct(ref a, ref b);
        }

        /// <summary>
        /// Calculate the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static float DotProduct(ref FVector2D a, ref FVector2D b)
        {
            float result;
            DotProduct(ref a, ref b, out result);
            return result;
        }

        /// <summary>
        /// Calculate the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The dot product.</param>
        public static void DotProduct(ref FVector2D a, ref FVector2D b, out float result)
        {
            result = a.X * b.X + a.Y * b.Y;
        }

        /// <summary>
        /// Calculate the cross product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The cross product.</returns>
        public static float operator ^(FVector2D a, FVector2D b)
        {
            float result;
            CrossProduct(ref a, ref b, out result);
            return result;
        }

        /// <summary>
        /// Calculate the cross product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The cross product.</returns>
        public static float CrossProduct(FVector2D a, FVector2D b)
        {
            float result;
            CrossProduct(ref a, ref b, out result);
            return result;
        }

        /// <summary>
        /// Calculate the cross product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The cross product.</returns>
        public static float CrossProduct(ref FVector2D a, ref FVector2D b)
        {
            float result;
            CrossProduct(ref a, ref b, out result);
            return result;
        }

        /// <summary>
        /// Calculate the cross product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The cross product.</param>
        public static void CrossProduct(ref FVector2D a, ref FVector2D b, out float result)
        {
            result = a.X * b.Y - a.Y * b.X;
        }

        /// <summary>
        /// Checks two vectors for equality.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if the vectors are equal, false otherwise.</returns>
        public static bool operator ==(FVector2D a, FVector2D b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        /// <summary>
        /// Checks two vectors for inequality.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if the vectors are not equal, false otherwise.</returns>
        public static bool operator !=(FVector2D a, FVector2D b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FVector2D))
            {
                return false;
            }

            return Equals((FVector2D)obj);
        }

        public bool Equals(FVector2D other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        /// <summary>
        /// Check against another vector for equality, within specified error limits.
        /// </summary>
        /// <param name="v">The vector to check against.</param>
        /// <param name="tolerance">Error tolerance. (default to FMath.KindaSmallNumber)</param>
        /// <returns>true if the vectors are equal within tolerance limits, false otherwise.</returns>
        public bool Equals(FVector2D v, float tolerance)
        {
            // Change this function name so that we can make use of the default param?
            return
                FMath.Abs(X - v.X) <= tolerance &&
                FMath.Abs(Y - v.Y) <= tolerance;
        }

        /// <summary>
        /// Checks whether both components of vector A are greater than vector B.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if vector A is the larger vector, otherwise false.</returns>
        public static bool operator >(FVector2D a, FVector2D b)
        {
            return a.X > b.X && a.Y > b.Y;
        }

        /// <summary>
        /// Checks whether both components of vector A are less than vector B.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if vector A is the smaller vector, otherwise false.</returns>
        public static bool operator <(FVector2D a, FVector2D b)
        {
            return a.X < b.X && a.Y < b.Y;
        }

        /// <summary>
        /// Checks whether both components of vector A are greater than or equal to vector B.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if vector A is greater than or equal to vector B, otherwise false.</returns>
        public static bool operator >=(FVector2D a, FVector2D b)
        {
            return a.X >= b.X && a.Y >= b.Y;
        }

        /// <summary>
        /// Checks whether both components of vector A are less than or equal to vector B.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if vector A is smaller than or equal to vector B, otherwise false.</returns>
        public static bool operator <=(FVector2D a, FVector2D b)
        {
            return a.X <= b.X && a.Y <= b.Y;
        }

        /// <summary>
        /// Gets a negated copy of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <returns>A negated copy of the vector.</returns>
        public static FVector2D operator -(FVector2D v)
        {
            v.X = -v.X;
            v.Y = -v.Y;
            return v;
        }

        /// <summary>
        /// Gets specific component of the vector.
        /// </summary>
        /// <param name="index">the index of vector component</param>
        /// <returns>Copy of the component.</returns>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    default:
                        throw new IndexOutOfRangeException("Invalid FVector2D index (" + index + ")");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FVector2D index (" + index + ")");
                }
            }
        }

        /// <summary>
        /// Gets a specific component of the vector.
        /// </summary>
        /// <param name="index">The index of the component required.</param>
        /// <returns>Copy of the specified component.</returns>
        public float Component(int index)
        {
            return this[index];
        }

        /// <summary>
        /// Squared distance between two 2D points.
        /// </summary>
        /// <param name="v1">The first point.</param>
        /// <param name="v2">The second point.</param>
        /// <returns>The squared distance between two 2D points.</returns>
        public static float DistSquared(FVector2D v1, FVector2D v2)
        {
            return FMath.Square(v2.X - v1.X) + FMath.Square(v2.Y - v1.Y);
        }

        /// <summary>
        /// Distance between two 2D points.
        /// </summary>
        /// <param name="v1">The first point.</param>
        /// <param name="v2">The second point.</param>
        /// <returns>The distance between two 2D points.</returns>
        public static float Distance(FVector2D v1, FVector2D v2)
        {
            return FMath.Sqrt(FVector2D.DistSquared(v1, v2));
        }

        /// <summary>
        /// Set the values of the vector directly.
        /// </summary>
        /// <param name="x">New X coordinate.</param>
        /// <param name="y">New Y coordinate.</param>
        public void Set(int x, int y)
        {
            X = y;
            Y = y;
        }

        /// <summary>
        /// Get the maximum value of the vector's components.
        /// </summary>
        /// <returns>The maximum value of the vector's components.</returns>
        public float GetMax()
        {
            return FMath.Max(X, Y);
        }

        /// <summary>
        /// Get the maximum absolute value of the vector's components.
        /// </summary>
        /// <returns>The maximum absolute value of the vector's components.</returns>
        public float GetAbsMax()
        {
            return FMath.Max(FMath.Abs(X), FMath.Abs(Y));
        }

        /// <summary>
        /// Get the minimum value of the vector's components.
        /// </summary>
        /// <returns>The minimum value of the vector's components.</returns>
        public float GetMin()
        {
            return FMath.Min(X, Y);
        }

        /// <summary>
        /// Get the length (magnitude) of this vector.
        /// </summary>
        /// <returns>The length of this vector.</returns>
        public float Size()
        {
            return FMath.Sqrt(X * X + Y * Y);
        }

        /// <summary>
        /// Get the squared length of this vector.
        /// </summary>
        /// <returns>The squared length of this vector.</returns>
        public float SizeSquared()
        {
            return X * X + Y * Y;
        }

        /// <summary>
        /// Rotates around axis (0,0,1)
        /// </summary>
        /// <param name="angleDeg">Angle to rotate (in degrees)</param>
        /// <returns>Rotated Vector</returns>
        public FVector2D GetRotated(float angleDeg)
        {
            // Based on FVector::RotateAngleAxis with Axis(0,0,1)

            float s, c;
            FMath.SinCos(out s, out c, FMath.DegreesToRadians(angleDeg));

            float omc = 1.0f - c;

            return new FVector2D(
                c * X - s * Y,
                s * X + c * Y);
        }

        /// <summary>
        /// Gets a normalized copy of the vector, checking it is safe to do so based on the length.
        /// Returns zero vector if vector length is too small to safely normalize.
        /// </summary>
        /// <param name="tolerance">Minimum squared length of vector for normalization.</param>
        /// <returns>A normalized copy of the vector if safe, (0,0) otherwise.</returns>
        public FVector2D GetSafeNormal(float tolerance = FMath.SmallNumber)
        {
            float squareSum = X * X + Y * Y;
            if (squareSum > tolerance)
            {
                float scale = FMath.InvSqrt(squareSum);
                return new FVector2D(X * scale, Y * scale);
            }
            return new FVector2D(0.0f, 0.0f);
        }

        /// <summary>
        /// Normalize this vector in-place if it is large enough, set it to (0,0) otherwise.
        /// </summary>
        /// <param name="tolerance">Minimum squared length of vector for normalization.</param>
        /// <see cref="GetSafeNormal"/>
        public void Normalize(float tolerance = FMath.SmallNumber)
        {
            float squareSum = X * X + Y * Y;
            if (squareSum > tolerance)
            {
                float scale = FMath.InvSqrt(squareSum);
                X *= scale;
                Y *= scale;
                return;
            }
            X = 0.0f;
            Y = 0.0f;
        }

        /// <summary>
        /// Checks whether vector is near to zero within a specified tolerance.
        /// </summary>
        /// <param name="tolerance">Error tolerance.</param>
        /// <returns>true if vector is in tolerance to zero, otherwise false.</returns>
        public bool IsNearlyZero(float tolerance = FMath.KindaSmallNumber)
        {
            return FMath.Abs(X) <= tolerance && FMath.Abs(Y) <= tolerance;
        }

        /// <summary>
        /// Util to convert this vector into a unit direction vector and its original length.
        /// </summary>
        /// <param name="dir">Reference passed in to store unit direction vector.</param>
        /// <param name="length">Reference passed in to store length of the vector.</param>
        public void ToDirectionAndLength(out FVector2D dir, out float length)
        {
            length = Size();
            if (length > FMath.SmallNumber)
            {
                float oneOverLength = 1.0f / length;
                dir = new FVector2D(X * oneOverLength, Y * oneOverLength);
            }
            else
            {
                dir = FVector2D.ZeroVector;
            }
        }

        /// <summary>
        /// Checks whether all components of the vector are exactly zero.
        /// </summary>
        /// <returns>true if vector is exactly zero, otherwise false.</returns>
        public bool IsZero()
        {
            return X == 0.0f && Y == 0.0f;
        }

        /// <summary>
        /// Get this vector as an Int Point.
        /// </summary>
        /// <returns>New Int Point from this vector.</returns>
        public FIntPoint IntPoint()
        {
            return new FIntPoint(FMath.RoundToInt(X), FMath.RoundToInt(Y));
        }

        /// <summary>
        /// Get this vector as a vector where each component has been rounded to the nearest int.
        /// </summary>
        /// <returns>New FVector2D from this vector that is rounded.</returns>
        public FVector2D RoundToVector()
        {
            return new FVector2D(FMath.RoundToInt(X), FMath.RoundToInt(Y));
        }

        /// <summary>
        /// Creates a copy of this vector with both axes clamped to the given range.
        /// </summary>
        /// <param name="minAxisVal"></param>
        /// <param name="maxAxisVal"></param>
        /// <returns>New vector with clamped axes.</returns>
        public FVector2D ClampAxes(float minAxisVal, float maxAxisVal)
        {
            return new FVector2D(FMath.Clamp(X, minAxisVal, maxAxisVal), FMath.Clamp(Y, minAxisVal, maxAxisVal));
        }

        /// <summary>
        /// Get a copy of the vector as sign only.
        /// Each component is set to +1 or -1, with the sign of zero treated as +1.
        /// </summary>
        /// <returns>A copy of the vector with each component set to +1 or -1</returns>
        public FVector2D GetSignVector()
        {
            return new FVector2D(
                FMath.FloatSelect(X, 1.0f, -1.0f),
                FMath.FloatSelect(Y, 10.0f, -1.0f));
        }

        /// <summary>
        /// Get a copy of this vector with absolute value of each component.
        /// </summary>
        /// <returns>A copy of this vector with absolute value of each component.</returns>
        public FVector2D GetAbs()
        {
            return new FVector2D(FMath.Abs(X), FMath.Abs(Y));
        }

        /// <summary>
        /// Get a textual representation of the vector.
        /// </summary>
        /// <returns>Text describing the vector.</returns>
        public override string ToString()
        {
            //%3.3f
            string numericFormat = "000.000";
            return "X=" + X.ToString(numericFormat) + " Y=" + Y.ToString(numericFormat);
        }

        /// <summary>
        /// Initialize this Vector based on an FString. The String is expected to contain X=, Y=.
        /// The FVector2D will be bogus when InitFromString returns false.
        /// </summary>
        /// <param name="sourceString">String containing the vector values.</param>
        /// <returns>true if the X,Y values were read successfully; false otherwise.</returns>
        public bool InitFromString(string sourceString)
        {
            X = Y = 0;

            // The initialization is only successful if the X, Y, and Z values can all be parsed from the string
            bool successful =
                FParse.Value(sourceString, "X=", ref X) &&
                FParse.Value(sourceString, "Y=", ref Y);
            return successful;
        }

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN()
        {
            if (ContainsNaN())
            {
                FMath.LogOrEnsureNanError("FVector2D contains NaN: " + ToString());
                this = ZeroVector;
            }
        }

        /// <summary>
        /// Utility to check if there are any non-finite values (NaN or Inf) in this vector.
        /// </summary>
        /// <returns>true if there are any non-finite values in this vector, false otherwise.</returns>
        public bool ContainsNaN()
        {
            return (!FMath.IsFinite(X) || !FMath.IsFinite(Y));
        }

        /// <summary>
        /// Converts spherical coordinates on the unit sphere into a Cartesian unit length vector.
        /// </summary>
        public FVector SphericalToUnitCartesian()
        {
            float SinTheta = FMath.Sin(X);
            return new FVector(FMath.Cos(Y) * SinTheta, FMath.Sin(Y) * SinTheta, FMath.Cos(X));
        }
    }
}
