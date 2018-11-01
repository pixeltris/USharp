using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Vector4.h

    /// <summary>
    /// A 4D homogeneous vector, 4x1 FLOATs, 16-byte aligned.
    /// </summary>
    [UStruct(Flags = 0x0000E838), BlueprintType, UMetaPath("/Script/CoreUObject.Vector4", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FVector4 : IEquatable<FVector4>
    {
        static bool X_IsValid;
        static int X_Offset;
        /// <summary>
        /// The vector's X-component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector4:X")]
        public float X;

        static bool Y_IsValid;
        static int Y_Offset;
        /// <summary>
        /// The vector's Y-component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector4:Y")]
        public float Y;

        static bool Z_IsValid;
        static int Z_Offset;
        /// <summary>
        /// The vector's Z-component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector4:Z")]
        public float Z;

        static bool W_IsValid;
        static int W_Offset;
        /// <summary>
        /// The vector's W-component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector4:W")]
        public float W;

        static int FVector4_StructSize;

        public FVector4 Copy()
        {
            FVector4 result = this;
            return result;
        }

        static FVector4()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FVector4)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FVector4));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Vector4");
            FVector4_StructSize = NativeReflection.GetStructSize(classAddress);
            X_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "X");
            X_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "X", Classes.UFloatProperty);
            Y_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Y");
            Y_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Y", Classes.UFloatProperty);
            Z_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Z");
            Z_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Z", Classes.UFloatProperty);
            W_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "W");
            W_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "W", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FVector4));
        }        

        /// <summary>
        /// A zero vector (0,0,0,0)
        /// </summary>
        public static readonly FVector4 ZeroVector = new FVector4(0, 0, 0, 0);

        /// <summary>
        /// One vector (1,1,1,1)
        /// </summary>
        public static readonly FVector4 OneVector = new FVector4(1, 1, 1, 1);

        /// <summary>
        /// World up vector (0,0,1,0)
        /// </summary>
        public static readonly FVector4 UpVector = new FVector4(0, 0, 1, 0);

        /// <summary>
        /// Unreal forward vector (1,0,0,0)
        /// </summary>
        public static readonly FVector4 ForwardVector = new FVector4(1, 0, 0, 0);

        /// <summary>
        /// Unreal right vector (0,1,0,0)
        /// </summary>
        public static readonly FVector4 RightVector = new FVector4(0, 1, 0, 0);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="vector">3D Vector to set first three components.</param>
        /// <param name="w">W Coordinate.</param>
        public FVector4(FVector vector, float w = 1.0f)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            W = w;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Creates and initializes a new vector from a color value.
        /// </summary>
        /// <param name="color">Color used to set vector.</param>
        public FVector4(FLinearColor color)
        {
            X = color.R;
            Y = color.G;
            Z = color.B;
            W = color.A;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Creates and initializes a new vector from the specified components.
        /// </summary>
        /// <param name="x">X Coordinate.</param>
        /// <param name="y">Y Coordinate.</param>
        /// <param name="z">Z Coordinate.</param>
        /// <param name="w">W Coordinate.</param>
        public FVector4(float x = 0.0f, float y = 0.0f, float z = 0.0f, float w = 0.0f)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Creates and initializes a new vector from the specified 2D vectors.
        /// </summary>
        /// <param name="xy">A 2D vector holding the X- and Y-components.</param>
        /// <param name="zw">A 2D vector holding the Z- and W-components.</param>
        public FVector4(FVector2D xy, FVector2D zw)
        {
            X = xy.X;
            Y = xy.Y;
            Z = zw.X;
            W = zw.Y;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Operator for implicit conversion from FVector4 to FVector
        /// </summary>
        public static implicit operator FVector(FVector4 v)
        {
            return new FVector(v);
        }

        /// <summary>
        /// Operator for implicit conversion from FVector to FVector4
        /// </summary>
        public static implicit operator FVector4(FVector v)
        {
            return new FVector4(v);
        }

        /// <summary>
        /// Operator for explicit conversion from FVector4 to FPlane
        /// </summary>
        public static explicit operator FPlane(FVector4 v)
        {
            return new FPlane(v);
        }

        /// <summary>
        /// Access a specific component of the vector.
        /// </summary>
        /// <param name="index">The index of the component.</param>
        /// <returns>The desired component.</returns>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    case 3: return W;
                    default:
                        throw new IndexOutOfRangeException("Invalid FVector4 index (" + index + ")");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FVector4 index (" + index + ")");
                }
            }
        }

        /// <summary>
        /// Gets a negated copy of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <returns>A negated copy of the vector.</returns>
        public static FVector4 operator -(FVector4 v)
        {
            return new FVector4(-v.X, -v.Y, -v.Z, -v.W);
        }

        /// <summary>
        /// Gets the result of component-wise addition of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of vector addition.</returns>
        public static FVector4 operator +(FVector4 a, FVector4 b)
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
        public static FVector4 Add(FVector4 a, FVector4 b)
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
        public static void Add(ref FVector4 a, ref FVector4 b, out FVector4 result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
            result.W = a.W + b.W;
        }

        /// <summary>
        /// Gets the result of component-wise subtraction of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of vector subtraction.</returns>
        public static FVector4 operator -(FVector4 a, FVector4 b)
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
        public static FVector4 Subtract(FVector4 a, FVector4 b)
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
        public static void Subtract(ref FVector4 a, ref FVector4 b, out FVector4 result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
            result.Z = a.Z - b.Z;
            result.W = a.W - b.W;
        }

        /// <summary>
        /// Gets the result of subtracting from each component of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="bias">How much to subtract from each component.</param>
        /// <returns>The result of subtraction.</returns>
        public static FVector4 operator -(FVector4 v, float bias)
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
        public static void Subtract(ref FVector4 v, float bias, out FVector4 result)
        {
            result.X = v.X - bias;
            result.Y = v.Y - bias;
            result.Z = v.Z - bias;
            result.W = v.W - bias;
        }

        /// <summary>
        /// Gets the result of adding to each component of the vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="bias">How much to add to each component.</param>
        /// <returns>The result of addition.</returns>
        public static FVector4 operator +(FVector4 v, float bias)
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
        public static void Add(ref FVector4 v, float bias, out FVector4 result)
        {
            result.X = v.X + bias;
            result.Y = v.Y + bias;
            result.Z = v.Z + bias;
            result.W = v.W + bias;
        }

        /// <summary>
        /// Gets the result of scaling the vector (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">Scale What to multiply each component by.</param>
        /// <returns>The result of multiplication.</returns>
        public static FVector4 operator *(float scale, FVector4 v)
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
        public static FVector4 operator *(FVector4 v, float scale)
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
        public static void Multiply(ref FVector4 v, float scale, out FVector4 result)
        {
            result.X = v.X * scale;
            result.Y = v.Y * scale;
            result.Z = v.Z * scale;
            result.W = v.W * scale;
        }

        /// <summary>
        /// Gets the result of dividing each component of the vector by a value.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">What to divide each component by.</param>
        /// <returns>The result of division.</returns>
        public static FVector4 operator /(FVector4 v, float scale)
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
        public static void Divide(ref FVector4 v, float scale, out FVector4 result)
        {
            float factor = 1.0f / scale;
            result.X = v.X * factor;
            result.Y = v.Y * factor;
            result.Z = v.Z * factor;
            result.W = v.W * factor;
        }

        /// <summary>
        /// Gets the result of component-wise multiplication of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of multiplication.</returns>
        public static FVector4 operator *(FVector4 a, FVector4 b)
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
        public static FVector4 Multiply(FVector4 a, FVector4 b)
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
        public static void Multiply(ref FVector4 a, ref FVector4 b, out FVector4 result)
        {
            result.X = a.X * b.X;
            result.Y = a.Y * b.Y;
            result.Z = a.Z * b.Z;
            result.W = a.W * b.W;
        }

        /// <summary>
        /// Gets the result of component-wise division of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The result of division.</returns>
        public static FVector4 operator /(FVector4 a, FVector4 b)
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
        public static FVector4 Divide(FVector4 a, FVector4 b)
        {
            Divide(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise division of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="result">The result of division.</param>
        public static void Divide(ref FVector4 a, ref FVector4 b, out FVector4 result)
        {
            result.X = a.X / b.X;
            result.Y = a.Y / b.Y;
            result.Z = a.Z / b.Z;
            result.W = a.W / b.W;
        }

        /// <summary>
        /// Checks two vectors for equality.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The first vector.</param>
        /// <returns>true if the vectors are equal, false otherwise.</returns>
        public static bool operator ==(FVector4 a, FVector4 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W != b.W;
        }

        /// <summary>
        /// Checks two vectors for inequality.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The first vector.</param>
        /// <returns>true if the vectors are not equal, false otherwise.</returns>
        public static bool operator !=(FVector4 a, FVector4 b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FVector4))
            {
                return false;
            }

            return Equals((FVector4)obj);
        }

        public bool Equals(FVector4 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = X.GetHashCode();
                hashcode = (hashcode * 397) ^ Y.GetHashCode();
                hashcode = (hashcode * 397) ^ Z.GetHashCode();
                hashcode = (hashcode * 397) ^ W.GetHashCode();
                return hashcode;
            }
        }

        /// <summary>
        /// Calculates 3D Dot product of two 4D vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The 3D Dot product.</returns>
        public static float Dot3(FVector4 a, FVector4 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// Calculates 4D Dot product.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The 4D Dot Product.</returns>
        public static float Dot4(FVector4 a, FVector4 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

        /// <summary>
        /// Calculate Cross product between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The Cross product.</returns>
        public static FVector4 operator ^(FVector4 a, FVector4 b)
        {
            return new FVector4(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X,
                0.0f);
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
        /// Error tolerant comparison.
        /// </summary>
        /// <param name="v">Vector to compare against.</param>
        /// <param name="tolerance">Error Tolerance.</param>
        /// <returns>true if the two vectors are equal within specified tolerance, otherwise false.</returns>
        public bool Equals(FVector4 v, float tolerance = FMath.KindaSmallNumber)
        {
            return
                FMath.Abs(X - v.X) <= tolerance &&
                FMath.Abs(Y - v.Y) <= tolerance &&
                FMath.Abs(Z - v.Z) <= tolerance &&
                FMath.Abs(W - v.W) <= tolerance;
        }

        /// <summary>
        /// Check if the vector is of unit length, with specified tolerance.
        /// </summary>
        /// <param name="lengthSquaredTolerance">Tolerance against squared length.</param>
        /// <returns>true if the vector is a unit vector within the specified tolerance.</returns>
        public bool IsUnit3(float lengthSquaredTolerance = FMath.KindaSmallNumber)
        {
            return FMath.Abs(1.0f - SizeSquared3()) < lengthSquaredTolerance;
        }

        public override string ToString()
        {
            //%3.3f
            string numericFormat = "000.000";
            return "X=" + X.ToString(numericFormat) +
                " Y=" + Y.ToString(numericFormat) +
                " Z=" + Z.ToString(numericFormat) +
                " W=" + W.ToString(numericFormat);
        }

        /// <summary>
        /// Initialize this Vector based on an FString. The String is expected to contain X=, Y=, Z=, W=.
        /// The FVector4 will be bogus when InitFromString returns false.
        /// </summary>
        /// <param name="sourceString">String containing the vector values.</param>
        /// <returns>true if the X,Y,Z values were read successfully; false otherwise.</returns>
        public bool InitFromString(string sourceString)
        {
            X = Y = Z = 0;
            W = 1.0f;

            // The initialization is only successful if the X, Y, and Z values can all be parsed from the string
            bool successful =
                FParse.Value(sourceString, "X=", ref X) &&
                FParse.Value(sourceString, "Y=", ref Y) &&
                FParse.Value(sourceString, "Z=", ref Z);

            // W is optional, so don't factor in its presence (or lack thereof) in determining initialization success
            FParse.Value(sourceString, "W=", ref W);

            return successful;
        }

        /// <summary>
        /// Returns a normalized copy of the vector if safe to normalize.
        /// </summary>
        /// <param name="tolerance">Minimum squared length of vector for normalization.</param>
        /// <returns>Minimum squared length of vector for normalization.</returns>
        public FVector4 GetSafeNormal(float tolerance = FMath.SmallNumber)
        {
            float squareSum = X * X + Y * Y + Z * Z;
            if (squareSum > tolerance)
            {
                float scale = FMath.InvSqrt(squareSum);
                return new FVector4(X * scale, Y * scale, Z * scale, 0.0f);
            }
            return FVector4.ZeroVector;
        }

        /// <summary>
        /// Calculates normalized version of vector without checking if it is non-zero.
        /// </summary>
        /// <returns>Normalized version of vector.</returns>
        public FVector4 GetUnsafeNormal3()
        {
            float scale = FMath.InvSqrt(X * X + Y * Y + Z * Z);
            return new FVector4(X * scale, Y * scale, Z * scale, 0.0f);
        }

        /// <summary>
        /// Return the FRotator orientation corresponding to the direction in which the vector points.
        /// Sets Yaw and Pitch to the proper numbers, and sets roll to zero because the roll can't be determined from a vector.
        /// </summary>
        /// <returns>FRotator from the Vector's direction.</returns>
        public FRotator ToOrientationRotator()
        {
            FRotator r;

            // Find yaw.
            r.Yaw = FMath.Atan2(Y, X) * (180.0f / FMath.PI);

            // Find pitch.
            r.Pitch = FMath.Atan2(Z, FMath.Sqrt(X * X + Y * Y)) * (180.0f / FMath.PI);

            // Find roll.
            r.Roll = 0;

            r.DiagnosticCheckNaN("FVector4::Rotation(): Rotator result " + r + " contains NaN! Input FVector4 = " + this);

            return r;
        }

        /// <summary>
        /// Return the Quaternion orientation corresponding to the direction in which the vector points.
        /// </summary>
        /// <returns>Quaternion from the Vector's direction.</returns>
        public FQuat ToOrientationQuat()
        {
            // Essentially an optimized Vector->Rotator->Quat made possible by knowing Roll == 0, and avoiding radians->degrees->radians.
            // This is done to avoid adding any roll (which our API states as a constraint).
            float yawRad = FMath.Atan2(Y, X);
            float pitchRad = FMath.Atan2(Z, FMath.Sqrt(X * X + Y * Y));

            float divideByTwo = 0.5f;
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
        /// Sets Yaw and Pitch to the proper numbers, and sets roll to zero because the roll can't be determined from a vector.
        /// Identical to 'ToOrientationRotator()'.
        /// </summary>
        /// <returns>FRotator from the Vector's direction.</returns>
        /// <see cref="ToOrientationRotator()"/>
        public FRotator Rotation()
        {
            return ToOrientationRotator();
        }

        /// <summary>
        /// Set all of the vectors coordinates.
        /// </summary>
        /// <param name="x">New X Coordinate.</param>
        /// <param name="y">New Y Coordinate.</param>
        /// <param name="z">New Z Coordinate.</param>
        /// <param name="w">New W Coordinate.</param>
        public void Set(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;

            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Get the length of this vector not taking W component into account.
        /// </summary>
        /// <returns>The length of this vector.</returns>
        public float Size3()
        {
            return FMath.Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>
        /// Get the squared length of this vector not taking W component into account.
        /// </summary>
        /// <returns>The squared length of this vector.</returns>
        public float SizeSquared3()
        {
            return X * X + Y * Y + Z * Z;
        }

        /// <summary>
        /// Get the length (magnitude) of this vector, taking the W component into account
        /// </summary>
        /// <returns>The length of this vector</returns>
        public float Size()
        {
            return FMath.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }

        /// <summary>
        /// Get the squared length of this vector, taking the W component into account
        /// </summary>
        /// <returns>The squared length of this vector</returns>
        public float SizeSquared()
        {
            return X * X + Y * Y + Z * Z + W * W;
        }

        /// <summary>
        /// Utility to check if there are any non-finite values (NaN or Inf) in this vector.
        /// </summary>
        public bool ContainsNaN()
        {
            return (!FMath.IsFinite(X) || !FMath.IsFinite(Y) || !FMath.IsFinite(Z) || !FMath.IsFinite(W));
        }

        /// <summary>
        /// Utility to check if all of the components of this vector are nearly zero given the tolerance.
        /// </summary>
        public bool IsNearlyZero3(float tolerance = FMath.KindaSmallNumber)
        {
            return FMath.Abs(X) <= tolerance && FMath.Abs(Y) <= tolerance && FMath.Abs(Z) <= tolerance;
        }

        /// <summary>
        /// Reflect vector.
        /// </summary>
        public FVector4 Reflect3(FVector4 normal)
        {
            return 2.0f * Dot3(this, normal) * normal - this;
        }

        /// <summary>
        /// Find good arbitrary axis vectors to represent U and V axes of a plane,
        /// given just the normal.
        /// </summary>
        public void FindBestAxisVectors3(FVector4 axis1, FVector4 axis2)
        {
            float nx = FMath.Abs(X);
            float ny = FMath.Abs(Y);
            float nz = FMath.Abs(Z);

            // Find best basis vectors.
            if (nz > nx && nz > ny)
            {
                axis1 = new FVector4(1, 0, 0);
            }
            else
            {
                axis1 = new FVector4(0, 0, 1);
            }

            axis1 = (axis1 - this * Dot3(axis1, this)).GetSafeNormal();
            axis2 = axis1 ^ this;
        }

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN()
        {
            if (ContainsNaN())
            {
                FMath.LogOrEnsureNanError("FVector contains NaN: " + ToString());
                this = FVector4.ZeroVector;
            }
        }
    }
}
