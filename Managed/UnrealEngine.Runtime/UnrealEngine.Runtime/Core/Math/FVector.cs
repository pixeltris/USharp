using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Vector.h

    /// <summary>
    /// A vector in 3-D space composed of components (X, Y, Z) with floating point precision.
    /// </summary>
    [UStruct(Flags = 0x0040EC38), BlueprintType, UMetaPath("/Script/CoreUObject.Vector", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FVector : IEquatable<FVector>
    {
        static bool X_IsValid;
        static int X_Offset;
        /// <summary>
        /// Vector's X component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector:X")]
        public float X;

        static bool Y_IsValid;
        static int Y_Offset;
        /// <summary>
        /// Vector's Y component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector:Y")]
        public float Y;

        static bool Z_IsValid;
        static int Z_Offset;
        /// <summary>
        /// Vector's Z component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector:Z")]
        public float Z;

        static int FVector_StructSize;

        public FVector Copy()
        {
            FVector result = this;
            return result;
        }

        static FVector()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FVector)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FVector));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Vector");
            FVector_StructSize = NativeReflection.GetStructSize(classAddress);
            X_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "X");
            X_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "X", Classes.UFloatProperty);
            Y_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Y");
            Y_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Y", Classes.UFloatProperty);
            Z_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Z");
            Z_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Z", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FVector));
        }

        /// <summary>
        /// A zero vector (0,0,0)
        /// </summary>
        public static readonly FVector ZeroVector = new FVector(0, 0, 0);

        /// <summary>
        /// One vector (1,1,1)
        /// </summary>
        public static readonly FVector OneVector = new FVector(1, 1, 1);

        /// <summary>
        /// World up vector (0,0,1)
        /// </summary>
        public static readonly FVector UpVector = new FVector(0, 0, 1);

        /// <summary>
        /// Unreal forward vector (1,0,0)
        /// </summary>
        public static readonly FVector ForwardVector = new FVector(1, 0, 0);

        /// <summary>
        /// Unreal right vector (0,1,0)
        /// </summary>
        public static readonly FVector RightVector = new FVector(0, 1, 0);

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN()
        {
            if (ContainsNaN())
            {
                FMath.LogOrEnsureNanError("FVector contains NaN: " + ToString());
                this = ZeroVector;
            }
        }

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN(string message)
        {
            if (ContainsNaN())
            {
                FMath.LogOrEnsureNanError(message + ": FVector contains NaN: " + ToString());
                this = ZeroVector;
            }
        }

        /// <summary>
        /// Constructor initializing all components to a single float value.
        /// </summary>
        /// <param name="value">Value to set all components to.</param>
        public FVector(float value)
        {
            X = value;
            Y = value;
            Z = value;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Constructor using initial values for each component.
        /// </summary>
        /// <param name="x">X Coordinate.</param>
        /// <param name="y">Y Coordinate.</param>
        /// <param name="z">Z Coordinate.</param>
        public FVector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Constructs a vector from an FVector2D and Z value.
        /// </summary>
        /// <param name="v">Vector to copy from.</param>
        /// <param name="z">Z Coordinate.</param>
        public FVector(FVector2D v, float z)
        {
            X = v.X;
            Y = v.Y;
            Z = z;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Constructor using the XYZ components from a 4D vector.
        /// </summary>
        /// <param name="v">4D Vector to copy from.</param>
        public FVector(FVector4 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Constructs a vector from an FLinearColor.
        /// </summary>
        /// <param name="color">Color to copy from.</param>
        public FVector(FLinearColor color)
        {
            X = color.R;
            Y = color.G;
            Z = color.B;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Constructs a vector from an FIntVector.
        /// </summary>
        /// <param name="vector">FIntVector to copy from.</param>
        public FVector(FIntVector vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Constructs a vector from an FIntPoint.
        /// </summary>
        /// <param name="a">Int Point used to set X and Y coordinates, Z is set to zero.</param>
        public FVector(FIntPoint a)
        {
            X = a.X;
            Y = a.Y;
            Z = 0;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Calculate the cross product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The cross product.</returns>
        public static FVector operator ^(FVector a, FVector b)
        {
            CrossProduct(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Calculate the cross product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The cross product.</returns>
        public static FVector CrossProduct(FVector a, FVector b)
        {
            CrossProduct(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Calculate the cross product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The cross product.</returns>
        public static FVector CrossProduct(ref FVector a, ref FVector b)
        {
            FVector result;
            CrossProduct(ref a, ref b, out result);
            return result;
        }

        /// <summary>
        /// Calculate the cross product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The cross product.</param>
        public static void CrossProduct(ref FVector a, ref FVector b, out FVector result)
        {
            float x = a.Y * b.Z - a.Z * b.Y;
            float y = a.Z * b.X - a.X * b.Z;
            float z = a.X * b.Y - a.Y * b.X;
            result.X = x;
            result.Y = y;
            result.Z = z;
        }

        /// <summary>
        /// Calculate the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static float operator |(FVector a, FVector b)
        {
            return DotProduct(ref a, ref b);
        }

        /// <summary>
        /// Calculate the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static float DotProduct(FVector a, FVector b)
        {
            return DotProduct(ref a, ref b);
        }

        /// <summary>
        /// Calculate the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static float DotProduct(ref FVector a, ref FVector b)
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
        public static void DotProduct(ref FVector a, ref FVector b, out float result)
        {
            result = a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// Gets the result of component-wise addition of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of vector addition.</returns>
        public static FVector operator +(FVector a, FVector b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise addition of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of vector addition.</returns>
        public static FVector Add(FVector a, FVector b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise addition of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The result of vector addition.</param>
        public static void Add(ref FVector a, ref FVector b, out FVector result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
        }

        /// <summary>
        /// Gets the result of component-wise subtraction of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of vector subtraction.</returns>
        public static FVector operator -(FVector a, FVector b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise subtraction of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of vector subtraction.</returns>
        public static FVector Subtract(FVector a, FVector b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise subtraction of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The result of vector subtraction.</param>
        public static void Subtract(ref FVector a, ref FVector b, out FVector result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
            result.Z = a.Z - b.Z;
        }

        /// <summary>
        /// Gets the result of subtracting from each component of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="bias">How much to subtract from each component.</param>
        /// <returns>The result of subtraction.</returns>
        public static FVector operator -(FVector v, float bias)
        {
            Subtract(ref v, bias, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of subtracting from each component of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="bias">How much to subtract from each component.</param>
        /// <param name="result">The result of subtraction.</param>
        public static void Subtract(ref FVector v, float bias, out FVector result)
        {
            result.X = v.X - bias;
            result.Y = v.Y - bias;
            result.Z = v.Z - bias;
        }

        /// <summary>
        /// Gets the result of adding to each component of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="bias">How much to add to each component.</param>
        /// <returns>The result of addition.</returns>
        public static FVector operator +(FVector v, float bias)
        {
            Add(ref v, bias, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of adding to each component of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="bias">How much to add to each component.</param>
        /// <param name="result">The result of addition.</param>
        public static void Add(ref FVector v, float bias, out FVector result)
        {
            result.X = v.X + bias;
            result.Y = v.Y + bias;
            result.Z = v.Z + bias;
        }

        /// <summary>
        /// Gets the result of scaling the vector (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">Scale What to multiply each component by.</param>
        /// <returns>The result of multiplication.</returns>
        public static FVector operator *(float scale, FVector v)
        {
            Multiply(ref v, scale, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of scaling the vector (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">Scale What to multiply each component by.</param>
        /// <returns>The result of multiplication.</returns>
        public static FVector operator *(FVector v, float scale)
        {
            Multiply(ref v, scale, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of scaling the vector (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">Scale What to multiply each component by.</param>
        /// <param name="result">The result of multiplication.</param>
        public static void Multiply(ref FVector v, float scale, out FVector result)
        {
            result.X = v.X * scale;
            result.Y = v.Y * scale;
            result.Z = v.Z * scale;
        }

        /// <summary>
        /// Gets the result of dividing each component of the vector by a value.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">What to divide each component by.</param>
        /// <returns>The result of division.</returns>
        public static FVector operator /(FVector v, float scale)
        {
            Divide(ref v, scale, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of dividing each component of the vector by a value.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">What to divide each component by.</param>
        /// <param name="result">The result of division.</param>
        public static void Divide(ref FVector v, float scale, out FVector result)
        {
            float factor = 1.0f / scale;
            result.X = v.X * factor;
            result.Y = v.Y * factor;
            result.Z = v.Z * factor;
        }

        /// <summary>
        /// Gets the result of component-wise multiplication of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of multiplication.</returns>
        public static FVector operator *(FVector a, FVector b)
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
        public static FVector Multiply(FVector a, FVector b)
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
        public static void Multiply(ref FVector a, ref FVector b, out FVector result)
        {
            result.X = a.X * b.X;
            result.Y = a.Y * b.Y;
            result.Z = a.Z * b.Z;
        }

        /// <summary>
        /// Gets the result of component-wise division of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of division.</returns>
        public static FVector operator /(FVector a, FVector b)
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
        public static FVector Divide(FVector a, FVector b)
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
        public static void Divide(ref FVector a, ref FVector b, out FVector result)
        {
            result.X = a.X / b.X;
            result.Y = a.Y / b.Y;
            result.Z = a.Z / b.Z;
        }

        /// <summary>
        /// Checks two vectors for equality.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if the vectors are equal, false otherwise.</returns>
        public static bool operator ==(FVector a, FVector b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        /// <summary>
        /// Checks two vectors for inequality.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if the vectors are not equal, false otherwise.</returns>
        public static bool operator !=(FVector a, FVector b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FVector))
            {
                return false;
            }

            return Equals((FVector)obj);
        }

        public bool Equals(FVector other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = X.GetHashCode();
                hashcode = (hashcode * 397) ^ Y.GetHashCode();
                hashcode = (hashcode * 397) ^ Z.GetHashCode();
                return hashcode;
            }
        }

        /// <summary>
        /// Check against another vector for equality, within specified error limits.
        /// </summary>
        /// <param name="v">The vector to check against.</param>
        /// <param name="tolerance">Error tolerance. (default to FMath.KindaSmallNumber)</param>
        /// <returns>true if the vectors are equal within tolerance limits, false otherwise.</returns>
        public bool Equals(FVector v, float tolerance)
        {
            // Change this function name so that we can make use of the default param?
            return
                FMath.Abs(X - v.X) <= tolerance &&
                FMath.Abs(Y - v.Y) <= tolerance &&
                FMath.Abs(Z - v.Z) <= tolerance;
        }

        /// <summary>
        /// Checks whether all components of this vector are the same, within a tolerance.
        /// </summary>
        /// <param name="tolerance">Error tolerance.</param>
        /// <returns>true if the vectors are equal within tolerance limits, false otherwise.</returns>
        public bool AllComponentsEqual(float tolerance = FMath.KindaSmallNumber)
        {
            return
                FMath.Abs(X - Y) <= tolerance &&
                FMath.Abs(X - Z) <= tolerance &&
                FMath.Abs(Y - Z) <= tolerance;
        }

        /// <summary>
        /// Get a negated copy of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <returns>A negated copy of the vector.</returns>
        public static FVector operator -(FVector v)
        {
            v.X = -v.X;
            v.Y = -v.Y;
            v.Z = -v.Z;
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
                    case 2: return Z;
                    default:
                        throw new IndexOutOfRangeException("Invalid FVector index (" + index + ")");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FVector index (" + index + ")");
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
        /// Get a specific component of the vector, given a specific axis by enum
        /// </summary>
        public float GetComponentForAxis(EAxis axis)
        {
            switch (axis)
            {
                case EAxis.X: return X;
                case EAxis.Y: return Y;
                case EAxis.Z: return Z;
                default: return 0;
            }
        }

        /// <summary>
        /// Set a specified componet of the vector, given a specific axis by enum
        /// </summary>
        public void SetComponentForAxis(EAxis axis, float component)
        {
            switch (axis)
            {
                case EAxis.X:
                    X = component;
                    break;
                case EAxis.Y:
                    Y = component;
                    break;
                case EAxis.Z:
                    Z = component;
                    break;
            }
        }

        // Simple functions.

        /// <summary>
        /// Set the values of the vector directly.
        /// </summary>
        /// <param name="x">New X coordinate.</param>
        /// <param name="y">New Y coordinate.</param>
        /// <param name="z">New Z coordinate.</param>
        public void Set(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = y;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Get the maximum value of the vector's components.
        /// </summary>
        /// <returns>The maximum value of the vector's components.</returns>
        public float GetMax()
        {
            return FMath.Max(FMath.Max(X, Y), Z);
        }

        /// <summary>
        /// Get the maximum absolute value of the vector's components.
        /// </summary>
        /// <returns>The maximum absolute value of the vector's components.</returns>
        public float GetAbsMax()
        {
            return FMath.Max(FMath.Max(FMath.Abs(X), FMath.Abs(Y)), FMath.Abs(Z));
        }

        /// <summary>
        /// Get the minimum value of the vector's components.
        /// </summary>
        /// <returns>The minimum value of the vector's components.</returns>
        public float GetMin()
        {
            return FMath.Min(FMath.Min(X, Y), Z);
        }

        /// <summary>
        /// Get the minimum absolute value of the vector's components.
        /// </summary>
        /// <returns>The minimum absolute value of the vector's components.</returns>
        public float GetAbsMin()
        {
            return FMath.Min(FMath.Min(FMath.Abs(X), FMath.Abs(Y)), FMath.Abs(Z));
        }

        /// <summary>
        /// Gets the component-wise min of two vectors.
        /// </summary>
        public FVector ComponentMin(FVector other)
        {
            ComponentMin(ref this, ref other, out other);
            return other;
        }

        /// <summary>
        /// Gets the component-wise min of two vectors.
        /// </summary>
        public static FVector ComponentMin(FVector a, FVector b)
        {
            ComponentMin(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the component-wise min of two vectors.
        /// </summary>
        public static void ComponentMin(ref FVector a, ref FVector b, out FVector result)
        {
            result.X = FMath.Min(a.X, b.X);
            result.Y = FMath.Min(a.Y, b.Y);
            result.Z = FMath.Min(a.Z, b.Z);
        }

        /// <summary>
        /// Gets the component-wise max of two vectors.
        /// </summary>
        public FVector ComponentMax(FVector other)
        {
            ComponentMax(ref this, ref other, out other);
            return other;
        }

        /// <summary>
        /// Gets the component-wise max of two vectors.
        /// </summary>
        public static FVector ComponentMax(FVector a, FVector b)
        {
            ComponentMin(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the component-wise max of two vectors.
        /// </summary>
        public static void ComponentMax(ref FVector a, ref FVector b, out FVector result)
        {
            result.X = FMath.Max(a.X, b.X);
            result.Y = FMath.Max(a.Y, b.Y);
            result.Z = FMath.Max(a.Z, b.Z);
        }

        /// <summary>
        /// Get a copy of this vector with absolute value of each component.
        /// </summary>
        /// <returns>A copy of this vector with absolute value of each component.</returns>
        public FVector GetAbs()
        {
            return new FVector(FMath.Abs(X), FMath.Abs(Y), FMath.Abs(Z));
        }

        /// <summary>
        /// Get the length (magnitude) of this vector.
        /// </summary>
        /// <returns>The length of this vector.</returns>
        public float Size()
        {
            return FMath.Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>
        /// Get the squared length of this vector.
        /// </summary>
        /// <returns>The squared length of this vector.</returns>
        public float SizeSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        /// <summary>
        /// Get the length of the 2D components of this vector.
        /// </summary>
        /// <returns>The 2D length of this vector.</returns>
        public float Size2D()
        {
            return FMath.Sqrt(X * X + Y * Y);
        }

        /// <summary>
        /// Get the squared length of the 2D components of this vector.
        /// </summary>
        /// <returns>The squared 2D length of this vector.</returns>
        public float SizeSquared2D()
        {
            return X * X + Y * Y;
        }

        /// <summary>
        /// Checks whether vector is near to zero within a specified tolerance.
        /// </summary>
        /// <param name="tolerance">Error tolerance.</param>
        /// <returns>true if the vector is near to zero, false otherwise.</returns>
        public bool IsNearlyZero(float tolerance = FMath.KindaSmallNumber)
        {
            return 
                FMath.Abs(X) <= tolerance && 
                FMath.Abs(Y) <= tolerance && 
                FMath.Abs(Z) <= tolerance;
        }

        /// <summary>
        /// Checks whether all components of the vector are exactly zero.
        /// </summary>
        /// <returns>true if the vector is exactly zero, false otherwise.</returns>
        public bool IsZero()
        {
            return X == 0.0f && Y == 0.0f && Z == 0.0f;
        }

        /// <summary>
        /// Normalize this vector in-place if it is larger than a given tolerance. Leaves it unchanged if not.
        /// </summary>
        /// <param name="tolerance">Minimum squared length of vector for normalization.</param>
        /// <returns>true if the vector was normalized correctly, false otherwise.</returns>
        public bool Normalize(float tolerance = FMath.SmallNumber)
        {
            float squareSum = X * X + Y * Y + Z * Z;
            if (squareSum > tolerance)
            {
                float scale = FMath.InvSqrt(squareSum);
                X *= scale; Y *= scale; Z *= scale;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether vector is normalized.
        /// </summary>
        /// <returns>true if Normalized, false otherwise.</returns>
        public bool IsNormalized()
        {
            return (FMath.Abs(1.0 - SizeSquared()) < FMath.THRESH_VECTOR_NORMALIZED);
        }

        /// <summary>
        /// Util to convert this vector into a unit direction vector and its original length.
        /// </summary>
        /// <param name="dir">Unit direction vector.</param>
        /// <param name="length">Length of the vector.</param>
        public void ToDirectionAndLength(out FVector dir, out float length)
        {
            length = Size();
            if (length > FMath.SmallNumber)
            {
                float oneOverLength = 1.0f / length;
                dir = new FVector(X * oneOverLength, Y * oneOverLength, Z * oneOverLength);
            }
            else
            {
                dir = FVector.ZeroVector;
            }
        }

        /// <summary>
        /// Get a copy of the vector as sign only.
        /// Each component is set to +1 or -1, with the sign of zero treated as +1.
        /// </summary>
        /// <returns>A copy of the vector with each component set to +1 or -1</returns>
        public FVector GetSignVector()
        {
            return new FVector(
                FMath.FloatSelect(X, 1.0f, -1.0f),
                FMath.FloatSelect(Y, 1.0f, -1.0f),
                FMath.FloatSelect(Z, 1.0f, -1.0f));
        }

        /// <summary>
        /// Projects 2D components of vector based on Z.
        /// </summary>
        /// <returns>Projected version of vector based on Z.</returns>
        public FVector Projection()
        {
            float rz = 1.0f / Z;
            return new FVector(X * rz, Y * rz, 1);
        }

        /// <summary>
        /// Calculates normalized version of vector without checking for zero length.
        /// </summary>
        /// <returns>Normalized version of vector.</returns>
        /// <see cref="GetSafeNormal"/>
        public FVector GetUnsafeNormal()
        {
            float scale = FMath.InvSqrt(X * X + Y * Y + Z * Z);
            return new FVector(X * scale, Y * scale, Z * scale);
        }

        /// <summary>
        /// Gets a copy of this vector snapped to a grid.
        /// </summary>
        /// <param name="gridSz">Grid dimension.</param>
        /// <returns>A copy of this vector snapped to a grid.</returns>
        /// <see cref="FMath.GridSnap"/>
        public FVector GridSnap(float gridSz)
        {
            return new FVector(
                FMath.GridSnap(X, gridSz), 
                FMath.GridSnap(Y, gridSz), 
                FMath.GridSnap(Z, gridSz));
        }

        /// <summary>
        /// Get a copy of this vector, clamped inside of a cube.
        /// </summary>
        /// <param name="radius">Half size of the cube.</param>
        /// <returns>A copy of this vector, bound by cube.</returns>
        public FVector BoundToCube(float radius)
        {
            return new FVector(
                FMath.Clamp(X, -radius, radius),
                FMath.Clamp(Y, -radius, radius),
                FMath.Clamp(Z, -radius, radius));
        }

        /// <summary>
        /// Get a copy of this vector, clamped inside of a cube.
        /// </summary>
        public FVector BoundToBox(FVector min, FVector max)
        {
            return new FVector(
                FMath.Clamp(X, min.X, max.X),
                FMath.Clamp(Y, min.Y, max.Y),
                FMath.Clamp(Z, min.Z, max.Z));
        }

        /// <summary>
        /// Create a copy of this vector, with its magnitude clamped between Min and Max.
        /// </summary>
        public FVector GetClampedToSize(float min, float max)
        {
            float vecSize = Size();
            FVector vecDir = (vecSize > FMath.SmallNumber) ? (this / vecSize) : FVector.ZeroVector;

            vecSize = FMath.Clamp(vecSize, min, max);

            return vecSize * vecDir;
        }

        /// <summary>
        /// Create a copy of this vector, with the 2D magnitude clamped between Min and Max. Z is unchanged.
        /// </summary>
        public FVector GetClampedToSize2D(float min, float max)
        {
            float vecSize2D = Size2D();
            FVector vecDir = (vecSize2D > FMath.SmallNumber) ? (this / vecSize2D) : FVector.ZeroVector;

            vecSize2D = FMath.Clamp(vecSize2D, min, max);

            return new FVector(vecSize2D * vecDir.X, vecSize2D * vecDir.Y, Z);
        }

        /// <summary>
        /// Create a copy of this vector, with its maximum magnitude clamped to MaxSize.
        /// </summary>
        public FVector GetClampedToMaxSize(float maxSize)
        {
            if (maxSize < FMath.KindaSmallNumber)
            {
                return FVector.ZeroVector;
            }

            float vsq = SizeSquared();
            if (vsq > FMath.Square(maxSize))
            {
                float scale = maxSize * FMath.InvSqrt(vsq);
                return new FVector(X * scale, Y * scale, Z * scale);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Create a copy of this vector, with the maximum 2D magnitude clamped to MaxSize. Z is unchanged.
        /// </summary>
        public FVector GetClampedToMaxSize2D(float maxSize)
        {
            if (maxSize < FMath.KindaSmallNumber)
            {
                return new FVector(0.0f, 0.0f, Z);
            }

            float vsq2D = SizeSquared2D();
            if (vsq2D > FMath.Square(maxSize))
            {
                float scale = maxSize * FMath.InvSqrt(vsq2D);
                return new FVector(X * scale, Y * scale, Z);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Add a vector to this and clamp the result in a cube.
        /// </summary>
        /// <param name="v">Vector to add.</param>
        /// <param name="radius">Half size of the cube.</param>
        public void AddBounded(FVector v, float radius = short.MaxValue)
        {
            this = (this + v).BoundToCube(radius);
        }

        /// <summary>
        /// Gets the reciprocal of this vector, avoiding division by zero.
        /// Zero components are set to BIG_NUMBER.
        /// </summary>
        /// <returns>Reciprocal of this vector.</returns>
        public FVector Reciprocal()
        {
            FVector recVector;
            if (X != 0.0f)
            {
                recVector.X = 1.0f / X;
            }
            else
            {
                recVector.X = FMath.BigNumber;
            }
            if (Y != 0.0f)
            {
                recVector.Y = 1.0f / Y;
            }
            else
            {
                recVector.Y = FMath.BigNumber;
            }
            if (Z != 0.0f)
            {
                recVector.Z = 1.0f / Z;
            }
            else
            {
                recVector.Z = FMath.BigNumber;
            }

            return recVector;
        }

        /// <summary>
        /// Check whether X, Y and Z are nearly equal.
        /// </summary>
        /// <param name="tolerance">Specified Tolerance.</param>
        /// <returns>true if X == Y == Z within the specified tolerance.</returns>
        public bool IsUniform(float tolerance = FMath.KindaSmallNumber)
        {
            return AllComponentsEqual(tolerance);
        }

        /// <summary>
        /// Mirror a vector about a normal vector.
        /// </summary>
        /// <param name="mirrorNormal">Normal vector to mirror about.</param>
        /// <returns>Mirrored vector.</returns>
        public FVector MirrorByVector(FVector mirrorNormal)
        {
            return this - mirrorNormal * (2.0f * (this | mirrorNormal));
        }

        /// <summary>
        /// Mirrors a vector about a plane.
        /// </summary>
        /// <param name="plane">Plane to mirror about.</param>
        /// <returns>Mirrored vector.</returns>
        public FVector MirrorByPlane(FPlane plane)
        {
            return this - plane * (2.0f * plane.PlaneDot(this));
        }

        /// <summary>
        /// Rotates around Axis (assumes Axis.Size() == 1).
        /// </summary>
        /// <param name="angleDeg">Angle to rotate (in degrees).</param>
        /// <param name="axis">Axis to rotate around.</param>
        /// <returns>Rotated Vector.</returns>
        public FVector RotateAngleAxis(float angleDeg, FVector axis)
        {
            float s, c;
            FMath.SinCos(out s, out c, FMath.DegreesToRadians(angleDeg));

            float xx = axis.X * axis.X;
            float yy = axis.Y * axis.Y;
            float zz = axis.Z * axis.Z;

            float xy = axis.X * axis.Y;
            float yz = axis.Y * axis.Z;
            float zx = axis.Z * axis.X;

            float xs = axis.X * s;
            float ys = axis.Y * s;
            float zs = axis.Z * s;

            float omc = 1.0f - c;

            return new FVector(
                (omc * xx + c) * X + (omc * xy - zs) * Y + (omc * zx + ys) * Z,
                (omc * xy + zs) * X + (omc * yy + c) * Y + (omc * yz - xs) * Z,
                (omc * zx - ys) * X + (omc * yz + xs) * Y + (omc * zz + c) * Z);
        }

        /// <summary>
        /// Gets a normalized copy of the vector, checking it is safe to do so based on the length.
        /// Returns zero vector if vector length is too small to safely normalize.
        /// </summary>
        /// <param name="tolerance">Minimum squared vector length.</param>
        /// <returns>A normalized copy if safe, (0,0,0) otherwise.</returns>
        public FVector GetSafeNormal(float tolerance = FMath.SmallNumber)
        {
            float squareSum = X * X + Y * Y + Z * Z;

            // Not sure if it's safe to add tolerance in there. Might introduce too many errors
            if (squareSum == 1.0f)
            {
                return this;
            }
            else if (squareSum < tolerance)
            {
                return FVector.ZeroVector;
            }
            float scale = FMath.InvSqrt(squareSum);
            return new FVector(X * scale, Y * scale, Z * scale);
        }

        /// <summary>
        /// Gets a normalized copy of the 2D components of the vector, checking it is safe to do so. Z is set to zero. 
        /// Returns zero vector if vector length is too small to normalize.
        /// </summary>
        /// <param name="tolerance">Minimum squared vector length.</param>
        /// <returns>Normalized copy if safe, otherwise returns zero vector.</returns>
        public FVector GetSafeNormal2D(float tolerance = FMath.SmallNumber)
        {
            float squareSum = X * X + Y * Y;

            // Not sure if it's safe to add tolerance in there. Might introduce too many errors
            if (squareSum == 1.0f)
            {
                if (Z == 0.0f)
                {
                    return this;
                }
                else
                {
                    return new FVector(X, Y, 0.0f);
                }
            }
            else if (squareSum < tolerance)
            {
                return FVector.ZeroVector;
            }

            float scale = FMath.InvSqrt(squareSum);
            return new FVector(X * scale, Y * scale, 0.0f);
        }

        /// <summary>
        /// Returns the cosine of the angle between this vector and another projected onto the XY plane (no Z).
        /// </summary>
        /// <param name="b">the other vector to find the 2D cosine of the angle with.</param>
        /// <returns>The cosine.</returns>
        public float CosineAngle2D(FVector b)
        {
            FVector a = this;
            a.Z = 0.0f;
            b.Z = 0.0f;
            a.Normalize();
            b.Normalize();
            return a | b;
        }

        /// <summary>
        /// Gets a copy of this vector projected onto the input vector.
        /// </summary>
        /// <param name="a">Vector to project onto, does not assume it is normalized.</param>
        /// <returns>Projected vector.</returns>
        public FVector ProjectOnTo(FVector a)
        {
            return (a * ((this | a) / (a | a)));
        }

        /// <summary>
        /// Gets a copy of this vector projected onto the input vector, which is assumed to be unit length.
        /// </summary>
        /// <param name="normal">Vector to project onto (assumed to be unit length).</param>
        /// <returns>Projected vector.</returns>
        public FVector ProjectOnToNormal(FVector normal)
        {
            return (normal * (this | normal));
        }

        /// <summary>
        /// Return the FRotator orientation corresponding to the direction in which the vector points.
        /// Sets Yaw and Pitch to the proper numbers, and sets Roll to zero because the roll can't be determined from a vector.
        /// </summary>
        /// <returns>FRotator from the Vector's direction, without any roll.</returns>
        /// <see cref="ToOrientationQuat"/>
        public FRotator ToOrientationRotator()
        {
            FRotator r;

            // Find yaw.
            r.Yaw = FMath.Atan2(Y, X) * (180.0f / FMath.PI);

            // Find pitch.
            r.Pitch = FMath.Atan2(Z, FMath.Sqrt(X * X + Y * Y)) * (180.0f / FMath.PI);

            // Find roll.
            r.Roll = 0;

#if ENABLE_NAN_DIAGNOSTIC
            if (r.ContainsNaN())
            {
                FMath.LogOrEnsureNanError("FVector::Rotation(): Rotator result " + r.ToString() + " contains NaN! Input FVector = " + ToString());
                r = FRotator.ZeroRotator;
            }
#endif
            return r;
        }

        /// <summary>
        /// Return the Quaternion orientation corresponding to the direction in which the vector points.
        /// Similar to the FRotator version, returns a result without roll such that it preserves the up vector.
        /// 
        /// @note If you don't care about preserving the up vector and just want the most direct rotation, you can use the faster
        /// 'FQuat::FindBetweenVectors(FVector::ForwardVector, YourVector)' or 'FQuat::FindBetweenNormals(...)' if you know the vector is of unit length.
        /// </summary>
        /// <returns>Quaternion from the Vector's direction, without any roll.</returns>
        /// <see cref="ToOrientationRotator()"/>
        /// <see cref="FQuat.FindBetweenVectors()"/>
        public FQuat ToOrientationQuat()
        {
            // Essentially an optimized Vector->Rotator->Quat made possible by knowing Roll == 0, and avoiding radians->degrees->radians.
            // This is done to avoid adding any roll (which our API states as a constraint).
            float yawRad = FMath.Atan2(Y, X);
            float pitchRad = FMath.Atan2(Z, FMath.Sqrt(X * X + Y * Y));

            const float divideByTwo = 0.5f;
            float sp, sy;
            float cp, cy;

            FMath.SinCos(out sp, out cp, pitchRad * divideByTwo);
            FMath.SinCos(out sy, out cy, yawRad * divideByTwo);

            FQuat rotationQuat;
            rotationQuat.X = sp * sy;
            rotationQuat.Y = -sp * cy;
            rotationQuat.Z = cp * sy;
            rotationQuat.W = cp * cy;
            return rotationQuat;
        }

        /// <summary>
        /// Return the FRotator orientation corresponding to the direction in which the vector points.
        /// Sets Yaw and Pitch to the proper numbers, and sets Roll to zero because the roll can't be determined from a vector.
        /// @note Identical to 'ToOrientationRotator()' and preserved for legacy reasons.
        /// </summary>
        /// <returns>FRotator from the Vector's direction.</returns>
        /// <see cref="ToOrientationRotator()"/>
        /// <see cref="ToOrientationQuat()"/>
        public FRotator Rotation()
        {
            return ToOrientationRotator();
        }

        /// <summary>
        /// Find good arbitrary axis vectors to represent U and V axes of a plane,
        /// using this vector as the normal of the plane.
        /// </summary>
        /// <param name="axis1">Reference to first axis.</param>
        /// <param name="axis2">Reference to second axis.</param>
        public void FindBestAxisVectors(out FVector axis1, FVector axis2)
        {
            float nx = FMath.Abs(X);
            float ny = FMath.Abs(Y);
            float nz = FMath.Abs(Z);

            // Find best basis vectors.
            if (nz > nx && nz > ny)
            {
                axis1 = new FVector(1, 0, 0);
            }
            else
            {
                axis1 = new FVector(0, 0, 1);
            }

            axis1 = (axis1 - this * (axis1 | this)).GetSafeNormal();
            axis2 = axis1 ^ this;
        }

        /// <summary>
        /// When this vector contains Euler angles (degrees), ensure that angles are between +/-180
        /// </summary>
        public void UnwindEuler()
        {
            X = FMath.UnwindDegrees(X);
            Y = FMath.UnwindDegrees(Y);
            Z = FMath.UnwindDegrees(Z);
        }

        /// <summary>
        /// Utility to check if there are any non-finite values (NaN or Inf) in this vector.
        /// </summary>
        /// <returns>true if there are any non-finite values in this vector, false otherwise.</returns>
        public bool ContainsNaN()
        {
            return (!FMath.IsFinite(X) || !FMath.IsFinite(Y) || !FMath.IsFinite(Z));
        }

        /// <summary>
        /// Check if the vector is of unit length, with specified tolerance.
        /// </summary>
        /// <param name="lengthSquaredTolerance">Tolerance against squared length.</param>
        /// <returns>true if the vector is a unit vector within the specified tolerance.</returns>
        public bool IsUnit(float lengthSquaredTolerance = FMath.KindaSmallNumber)
        {
            return FMath.Abs(1.0f - SizeSquared()) < lengthSquaredTolerance;
        }

        public override string ToString()
        {
            //%3.3f
            string numericFormat = "000.000";
            return "X=" + X.ToString(numericFormat) + " Y=" + Y.ToString(numericFormat) + " Z=" + Z.ToString(numericFormat);
        }

        /// <summary>
        /// Get a short textural representation of this vector, for compact readable logging.
        /// </summary>
        public string ToCompactString()
        {
            //%.2f
            string numericFormat = "0.00";
            if (IsNearlyZero())
            {
                return "V(0)";
            }

            string returnString = "V(";
            bool isEmptyString = true;
            if (!FMath.IsNearlyZero(X))
            {
                returnString += "X=" + X.ToString(numericFormat);
                isEmptyString = false;
            }
            if (!FMath.IsNearlyZero(Y))
            {
                if (!isEmptyString)
                {
                    returnString += ", ";
                }
                returnString += "Y=" + X.ToString(numericFormat);
                isEmptyString = false;
            }
            if (!FMath.IsNearlyZero(Z))
            {
                if (!isEmptyString)
                {
                    returnString += ", ";
                }
                returnString += "Z=" + X.ToString(numericFormat);
                isEmptyString = false;
            }
            returnString += ")";
            return returnString;
        }

        /// <summary>
        /// Initialize this Vector based on an FString. The String is expected to contain X=, Y=, Z=.
        /// The FVector will be bogus when InitFromString returns false.
        /// </summary>
        /// <param name="sourceString">String containing the vector values.</param>
        /// <returns>true if the X,Y,Z values were read successfully; false otherwise.</returns>
        public bool InitFromString(string sourceString)
        {
            X = Y = Z = 0;

            // The initialization is only successful if the X, Y, and Z values can all be parsed from the string
            bool successful = 
                FParse.Value(sourceString, "X=", ref X) && 
                FParse.Value(sourceString, "Y=", ref Y) && 
                FParse.Value(sourceString, "Z=", ref Z);
            return successful;
        }

        /// <summary>
        /// Converts a Cartesian unit vector into spherical coordinates on the unit sphere.
        /// </summary>
        /// <returns>Output Theta will be in the range [0, PI], and output Phi will be in the range [-PI, PI]. </returns>
        public FVector2D UnitCartesianToSpherical()
        {
            Debug.Assert(IsUnit());
            float Theta = FMath.Acos(Z / Size());
            float Phi = FMath.Atan2(Y, X);
            return new FVector2D(Theta, Phi);
        }

        /// <summary>
        /// Convert a direction vector into a 'heading' angle.
        /// </summary>
        /// <returns>'Heading' angle between +/-PI. 0 is pointing down +X.</returns>
        public float HeadingAngle()
        {
            // Project Dir into Z plane.
            FVector planeDir = this;
            planeDir.Z = 0.0f;
            planeDir = planeDir.GetSafeNormal();

            float angle = FMath.Acos(planeDir.X);

            if (planeDir.Y < 0.0f)
            {
                angle *= -1.0f;
            }

            return angle;
        }

        /// <summary>
        /// Create an orthonormal basis from a basis with at least two orthogonal vectors.
        /// It may change the directions of the X and Y axes to make the basis orthogonal,
        /// but it won't change the direction of the Z axis.
        /// All axes will be normalized.
        /// </summary>
        /// <param name="xAxis">The input basis' XAxis, and upon return the orthonormal basis' XAxis.</param>
        /// <param name="yAxis">The input basis' YAxis, and upon return the orthonormal basis' YAxis.</param>
        /// <param name="zAxis">The input basis' ZAxis, and upon return the orthonormal basis' ZAxis.</param>
        public static void CreateOrthonormalBasis(ref FVector xAxis, ref FVector yAxis, ref FVector zAxis)
        {
            // Project the X and Y axes onto the plane perpendicular to the Z axis.
            xAxis -= (xAxis | zAxis) / (zAxis | zAxis) * zAxis;
            yAxis -= (yAxis | zAxis) / (zAxis | zAxis) * zAxis;

            // If the X axis was parallel to the Z axis, choose a vector which is orthogonal to the Y and Z axes.
            if (xAxis.SizeSquared() < FMath.Delta * FMath.Delta)
            {
                xAxis = yAxis ^ zAxis;
            }

            // If the Y axis was parallel to the Z axis, choose a vector which is orthogonal to the X and Z axes.
            if (yAxis.SizeSquared() < FMath.Delta * FMath.Delta)
            {
                yAxis = xAxis ^ zAxis;
            }

            // Normalize the basis vectors.
            xAxis.Normalize();
            yAxis.Normalize();
            zAxis.Normalize();
        }

        /// <summary>
        /// Compare two points and see if they're the same, using a threshold.
        /// </summary>
        /// <param name="p">First vector.</param>
        /// <param name="q">Second vector.</param>
        /// <returns>Whether points are the same within a threshold. Uses fast distance approximation (linear per-component distance).</returns>
        public static bool PointsAreSame(FVector p, FVector q)
        {
            float temp;
            temp = p.X - q.X;
            if ((temp > -FMath.THRESH_POINTS_ARE_SAME) && (temp < FMath.THRESH_POINTS_ARE_SAME))
            {
                temp = p.Y - q.Y;
                if ((temp > -FMath.THRESH_POINTS_ARE_SAME) && (temp < FMath.THRESH_POINTS_ARE_SAME))
                {
                    temp = p.Z - q.Z;
                    if ((temp > -FMath.THRESH_POINTS_ARE_SAME) && (temp < FMath.THRESH_POINTS_ARE_SAME))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Compare two points and see if they're within specified distance.
        /// </summary>
        /// <param name="point1">First vector.</param>
        /// <param name="point2">Second vector.</param>
        /// <param name="dist">Specified distance.</param>
        /// <returns>Whether two points are within the specified distance. Uses fast distance approximation (linear per-component distance).</returns>
        public static bool PointsAreNear(FVector point1, FVector point2, float dist)
        {
            float temp;
            temp = (point1.X - point2.X); if (FMath.Abs(temp) >= dist) return false;
            temp = (point1.Y - point2.Y); if (FMath.Abs(temp) >= dist) return false;
            temp = (point1.Z - point2.Z); if (FMath.Abs(temp) >= dist) return false;
            return true;
        }

        /// <summary>
        /// Calculate the signed distance (in the direction of the normal) between a point and a plane.
        /// </summary>
        /// <param name="point">The Point we are checking.</param>
        /// <param name="planeBase">The Base Point in the plane.</param>
        /// <param name="planeNormal">The Normal of the plane (assumed to be unit length).</param>
        /// <returns>Signed distance between point and plane.</returns>
        public static float PointPlaneDist(FVector point, FVector planeBase, FVector planeNormal)
        {
            return (point - planeBase) | planeNormal;
        }

        /// <summary>
        /// Calculate the projection of a point on the given plane.
        /// </summary>
        /// <param name="point">The point to project onto the plane</param>
        /// <param name="plane">The plane</param>
        /// <returns>Projection of Point onto Plane</returns>
        public static FVector PointPlaneProject(FVector point, FPlane plane)
        {
            //Find the distance of X from the plane
            //Add the distance back along the normal from the point
            return point - plane.PlaneDot(point) * plane;
        }

        /// <summary>
        /// Calculate the projection of a point on the plane defined by counter-clockwise (CCW) points A,B,C.
        /// </summary>
        /// <param name="point">The point to project onto the plane</param>
        /// <param name="a">1st of three points in CCW order defining the plane </param>
        /// <param name="b">2nd of three points in CCW order defining the plane</param>
        /// <param name="c">3rd of three points in CCW order defining the plane</param>
        /// <returns>Projection of Point onto plane ABC</returns>
        public static FVector PointPlaneProject(FVector point, FVector a, FVector b, FVector c)
        {
            //Compute the plane normal from ABC
            FPlane Plane = new FPlane(a, b, c);

            //Find the distance of X from the plane
            //Add the distance back along the normal from the point
            return point - Plane.PlaneDot(point) * Plane;
        }

        /// <summary>
        /// Calculate the projection of a point on the plane defined by PlaneBase and PlaneNormal.
        /// </summary>
        /// <param name="point">The point to project onto the plane</param>
        /// <param name="planeBase">Point on the plane</param>
        /// <param name="planeNormal">Normal of the plane (assumed to be unit length).</param>
        /// <returns>Projection of Point onto plane</returns>
        public static FVector PointPlaneProject(FVector point, FVector planeBase, FVector planeNormal)
        {
            //Find the distance of X from the plane
            //Add the distance back along the normal from the point
            return point - FVector.PointPlaneDist(point, planeBase, planeNormal) * planeNormal;
        }

        /// <summary>
        /// Calculate the projection of a vector on the plane defined by PlaneNormal.
        /// </summary>
        /// <param name="v">The vector to project onto the plane.</param>
        /// <param name="planeNormal">Normal of the plane (assumed to be unit length).</param>
        /// <returns>Projection of V onto plane.</returns>
        public static FVector VectorPlaneProject(FVector v, FVector planeNormal)
        {
            return v - v.ProjectOnToNormal(planeNormal);
        }

        /// <summary>
        /// Euclidean distance between two points.
        /// </summary>
        /// <param name="v1">The first point.</param>
        /// <param name="v2">The second point.</param>
        /// <returns>The distance between two points.</returns>
        public static float Dist(FVector v1, FVector v2)
        {
            return FMath.Sqrt(FVector.DistSquared(v1, v2));
        }

        /// <summary>
        /// Euclidean distance between two points.
        /// </summary>
        /// <param name="v1">The first point.</param>
        /// <param name="v2">The second point.</param>
        /// <returns>The distance between two points.</returns>
        public static float Distance(FVector v1, FVector v2)
        {
            return Dist(v1, v2);
        }

        /// <summary>
        /// Euclidean distance between two points in the XY plane (ignoring Z).
        /// </summary>
        /// <param name="v1">The first point.</param>
        /// <param name="v2">The second point.</param>
        /// <returns>The distance between two points in the XY plane.</returns>
        public static float DistXY(FVector v1, FVector v2)
        {
            return FMath.Sqrt(FVector.DistSquaredXY(v1, v2));
        }

        /// <summary>
        /// Euclidean distance between two points in the XY plane (ignoring Z).
        /// </summary>
        /// <param name="v1">The first point.</param>
        /// <param name="v2">The second point.</param>
        /// <returns>The distance between two points in the XY plane.</returns>
        public static float Dist2D(FVector v1, FVector v2)
        {
            return DistXY(v1, v2);
        }

        /// <summary>
        /// Squared distance between two points.
        /// </summary>
        /// <param name="v1">The first point.</param>
        /// <param name="v2">The second point.</param>
        /// <returns>The squared distance between two points.</returns>
        public static float DistSquared(FVector v1, FVector v2)
        {
            return FMath.Square(v2.X - v1.X) + FMath.Square(v2.Y - v1.Y) + FMath.Square(v2.Z - v1.Z);
        }

        /// <summary>
        /// Squared distance between two points in the XY plane only.
        /// </summary>
        /// <param name="v1">The first point.</param>
        /// <param name="v2">The second point.</param>
        /// <returns>The squared distance between two points in the XY plane</returns>
        public static float DistSquaredXY(FVector v1, FVector v2)
        {
            return FMath.Square(v2.X - v1.X) + FMath.Square(v2.Y - v1.Y);
        }

        /// <summary>
        /// Squared distance between two points in the XY plane only.
        /// </summary>
        /// <param name="v1">The first point.</param>
        /// <param name="v2">The second point.</param>
        /// <returns>The squared distance between two points in the XY plane</returns>
        public static float DistSquared2D(FVector v1, FVector v2)
        {
            return DistSquaredXY(v1, v2);
        }

        /// <summary>
        /// Compute pushout of a box from a plane.
        /// </summary>
        /// <param name="normal">The plane normal.</param>
        /// <param name="size">The size of the box.</param>
        /// <returns>Pushout required.</returns>
        public static float BoxPushOut(FVector normal, FVector size)
        {
            return FMath.Abs(normal.X * size.X) + FMath.Abs(normal.Y * size.Y) + FMath.Abs(normal.Z * size.Z);
        }

        /// <summary>
        /// See if two normal vectors are nearly parallel, meaning the angle between them is close to 0 degrees.
        /// </summary>
        /// <param name="normal1">First normalized vector.</param>
        /// <param name="normal2">Second normalized vector.</param>
        /// <param name="parallelCosineThreshold">Normals are parallel if absolute value of dot product (cosine of angle between them) is greater than or equal to this. For example: cos(1.0 degrees).</param>
        /// <returns>true if vectors are nearly parallel, false otherwise.</returns>
        public static bool Parallel(FVector normal1, FVector normal2, float parallelCosineThreshold = FMath.THRESH_NORMALS_ARE_PARALLEL)
        {
            float normalDot = normal1 | normal2;
            return FMath.Abs(normalDot) >= parallelCosineThreshold;
        }

        /// <summary>
        /// See if two normal vectors are coincident (nearly parallel and point in the same direction).
        /// </summary>
        /// <param name="normal1">First normalized vector.</param>
        /// <param name="normal2">Second normalized vector.</param>
        /// <param name="parallelCosineThreshold">Normals are coincident if dot product (cosine of angle between them) is greater than or equal to this. For example: cos(1.0 degrees).</param>
        /// <returns>true if vectors are coincident (nearly parallel and point in the same direction), false otherwise.</returns>
        public static bool Coincident(FVector normal1, FVector normal2, float parallelCosineThreshold = FMath.THRESH_NORMALS_ARE_PARALLEL)
        {
            float normalDot = normal1 | normal2;
            return normalDot >= parallelCosineThreshold;
        }

        /// <summary>
        /// See if two normal vectors are nearly orthogonal (perpendicular), meaning the angle between them is close to 90 degrees.
        /// </summary>
        /// <param name="normal1">First normalized vector.</param>
        /// <param name="normal2">Second normalized vector.</param>
        /// <param name="orthogonalCosineThreshold">Normals are orthogonal if absolute value of dot product (cosine of angle between them) is less than or equal to this. For example: cos(89.0 degrees).</param>
        /// <returns>true if vectors are orthogonal (perpendicular), false otherwise.</returns>
        public static bool Orthogonal(FVector normal1, FVector normal2, float orthogonalCosineThreshold = FMath.THRESH_NORMALS_ARE_ORTHOGONAL)
        {
            float normalDot = normal1 | normal2;
            return FMath.Abs(normalDot) <= orthogonalCosineThreshold;
        }

        /// <summary>
        /// See if two planes are coplanar. They are coplanar if the normals are nearly parallel and the planes include the same set of points.
        /// </summary>
        /// <param name="base1">The base point in the first plane.</param>
        /// <param name="normal1">The normal of the first plane.</param>
        /// <param name="base2">The base point in the second plane.</param>
        /// <param name="normal2">The normal of the second plane.</param>
        /// <param name="parallelCosineThreshold">Normals are parallel if absolute value of dot product is greater than or equal to this.</param>
        /// <returns>true if the planes are coplanar, false otherwise.</returns>
        public static bool Coplanar(FVector base1, FVector normal1, FVector base2, FVector normal2, float parallelCosineThreshold = FMath.THRESH_NORMALS_ARE_PARALLEL)
        {
            if (!FVector.Parallel(normal1, normal2, parallelCosineThreshold))
            {
                return false;
            }
            else if (FVector.PointPlaneDist(base2, base1, normal1) > FMath.THRESH_POINT_ON_PLANE)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Triple product of three vectors: X dot (Y cross Z).
        /// </summary>
        /// <param name="X">The first vector.</param>
        /// <param name="Y">The second vector.</param>
        /// <param name="Z">The third vector.</param>
        /// <returns>The triple product: X dot (Y cross Z).</returns>
        public static float Triple(FVector X, FVector Y, FVector Z)
        {
            return
                (X.X * (Y.Y * Z.Z - Y.Z * Z.Y)) +
                (X.Y * (Y.Z * Z.X - Y.X * Z.Z)) +
                (X.Z * (Y.X * Z.Y - Y.Y * Z.X));
        }

        /// <summary>
        /// Generates a list of sample points on a Bezier curve defined by 2 points.
        /// </summary>
        /// <param name="controlPoints">Array of 4 FVectors (vert1, controlpoint1, controlpoint2, vert2).</param>
        /// <param name="numPoints">Number of samples.</param>
        /// <param name="points">Receives the output samples.</param>
        /// <returns>The path length.</returns>
        public static float EvaluateBezier(FVector[] controlPoints, int numPoints, out FVector[] points)
        {
            Debug.Assert(controlPoints != null && controlPoints.Length >= 4);
            Debug.Assert(numPoints >= 2);

            points = new FVector[numPoints];

            // var q is the change in t between successive evaluations.
            float q = 1.0f / (numPoints - 1); // q is dependent on the number of GAPS = POINTS-1

            // recreate the names used in the derivation
            FVector p0 = controlPoints[0];
            FVector p1 = controlPoints[1];
            FVector p2 = controlPoints[2];
            FVector p3 = controlPoints[3];

            // coefficients of the cubic polynomial that we're FDing -
            FVector a = p0;
            FVector b = 3 * (p1 - p0);
            FVector c = 3 * (p2 - 2 * p1 + p0);
            FVector d = p3 - 3 * p2 + 3 * p1 - p0;

            // initial values of the poly and the 3 diffs -
            FVector s = a;						// the poly value
            FVector u = b * q + c * q * q + d * q * q * q;	// 1st order diff (quadratic)
            FVector v = 2 * c * q * q + 6 * d * q * q * q;	// 2nd order diff (linear)
            FVector w = 6 * d * q * q * q;				// 3rd order diff (constant)

            // Path length.
            float length = 0.0f;

            FVector oldPos = p0;
            points[0] = p0;// first point on the curve is always P0.

            for (int i = 1; i < numPoints; ++i)
            {
                // calculate the next value and update the deltas
                s += u;			// update poly value
                u += v;			// update 1st order diff value
                v += w;         // update 2st order diff value
                // 3rd order diff is constant => no update needed.

                // Update Length.
                length += FVector.Dist(s, oldPos);
                oldPos = s;

                points[i] = s;
            }

            // Return path length as experienced in sequence (linear interpolation between points).
            return length;
        }

        /// <summary>
        /// Converts a vector containing radian values to a vector containing degree values.
        /// </summary>
        /// <param name="radVector">Vector containing radian values</param>
        /// <returns>Vector containing degree values</returns>
        public static FVector RadiansToDegrees(FVector radVector)
        {
            return radVector * (180.0f / FMath.PI);
        }

        /// <summary>
        /// Converts a vector containing degree values to a vector containing radian values.
        /// </summary>
        /// <param name="degVector">Vector containing degree values</param>
        /// <returns>Vector containing radian values</returns>
        public static FVector DegreesToRadians(FVector degVector)
        {
            return degVector * (FMath.PI / 180.0f);
        }

        struct FClusterMovedHereToMakeCompile
        {
            public FVector ClusterPosAccum;
            public int ClusterSize;
        }

        /// <summary>
        /// Given a current set of cluster centers, a set of points, iterate N times to move clusters to be central. 
        /// </summary>
        /// <param name="clusters">Reference to array of Clusters.</param>
        /// <param name="points">Set of points.</param>
        /// <param name="numIterations">Number of iterations.</param>
        /// <param name="numConnectionsToBeValid">Sometimes you will have long strings that come off the mass of points
        /// which happen to have been chosen as Cluster starting points.  You want to be able to disregard those.</param>
        public static void GenerateClusterCenters(List<FVector> clusters, FVector[] points, int numIterations, int numConnectionsToBeValid)
        {
            // Check we have >0 points and clusters
            if (points.Length == 0 || clusters.Count == 0)
            {
                return;
            }

            // Temp storage for each cluster that mirrors the order of the passed in Clusters array
            FClusterMovedHereToMakeCompile[] clusterData = new FClusterMovedHereToMakeCompile[clusters.Count];

            // Then iterate
            for (int itCount = 0; itCount < numIterations; itCount++)
            {
                // Classify each point - find closest cluster center
                for (int i = 0; i < points.Length; i++)
                {
                    FVector pos = points[i];

                    // Iterate over all clusters to find closes one
                    int nearestClusterIndex = -1;
                    float nearestClusterDistSqr = FMath.BigNumber;
                    for (int j = 0; j < clusters.Count; j++)
                    {
                        float distSqr = (pos - clusters[j]).SizeSquared();
                        if (distSqr < nearestClusterDistSqr)
                        {
                            nearestClusterDistSqr = distSqr;
                            nearestClusterIndex = j;
                        }
                    }
                    // Update its info with this point
                    if (nearestClusterIndex != -1)
                    {
                        clusterData[nearestClusterIndex].ClusterPosAccum += pos;
                        clusterData[nearestClusterIndex].ClusterSize++;
                    }
                }

                // All points classified - update cluster center as average of membership
                for (int i = 0; i < clusters.Count; i++)
                {
                    if (clusterData[i].ClusterSize > 0)
                    {
                        clusters[i] = clusterData[i].ClusterPosAccum / (float)clusterData[i].ClusterSize;
                    }
                }
            }

            // so now after we have possible cluster centers we want to remove the ones that are outliers and not part of the main cluster
            for (int i = 0; i < clusterData.Length; i++)
            {
                if (clusterData[i].ClusterSize < numConnectionsToBeValid)
                {
                    clusters.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Util to calculate distance from a point to a bounding box
        /// </summary>
        /// <param name="mins">3D Point defining the lower values of the axis of the bound box</param>
        /// <param name="maxs">3D Point defining the lower values of the axis of the bound box</param>
        /// <param name="point">3D position of interest</param>
        /// <returns>the distance from the Point to the bounding box.</returns>
        public static float ComputeSquaredDistanceFromBoxToPoint(FVector mins, FVector maxs, FVector point)
        {
            // Accumulates the distance as we iterate axis
            float distSquared = 0.0f;

            // Check each axis for min/max and add the distance accordingly
            // NOTE: Loop manually unrolled for > 2x speed up
            if (point.X < mins.X)
            {
                distSquared += FMath.Square(point.X - mins.X);
            }
            else if (point.X > maxs.X)
            {
                distSquared += FMath.Square(point.X - maxs.X);
            }

            if (point.Y < mins.Y)
            {
                distSquared += FMath.Square(point.Y - mins.Y);
            }
            else if (point.Y > maxs.Y)
            {
                distSquared += FMath.Square(point.Y - maxs.Y);
            }

            if (point.Z < mins.Z)
            {
                distSquared += FMath.Square(point.Z - mins.Z);
            }
            else if (point.Z > maxs.Z)
            {
                distSquared += FMath.Square(point.Z - maxs.Z);
            }

            return distSquared;
        }

        /// <summary>
        /// Replicates one element into all elements and returns the new vector.
        /// </summary>
        /// <param name="v">Source vector</param>
        /// <param name="index">Index (0-3) of the element to replicate</param>
        /// <returns>A new vector with the given element in all elements of the returned vector.</returns>
        public static FVector Replicate(FVector v, int index)
        {
            float value = v[index];
            return new FVector(value, value, value);
        }

        /// <summary>
        /// Calculates the dot3 product of two vectors and returns a vector with the result in all components.
        /// Only really efficient on Xbox 360.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>d = dot3(Vec1.xyz, Vec2.xyz), FVector( d, d, d )</returns>
        public static FVector VectorDot3(FVector a, FVector b)
        {
            float d = DotProduct(ref a, ref b);
            return new FVector(d, d, d);
        }
    }
}
