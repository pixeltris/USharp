using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Quat.h

    /// <summary>
    /// Floating point quaternion that can represent a rotation about an axis in 3-D space.
    /// The X, Y, Z, W components also double as the Axis/Angle format.
    /// 
    /// Order matters when composing quaternions: C = A * B will yield a quaternion C that logically
    /// first applies B then A to any subsequent transformation (right first, then left).
    /// Note that this is the opposite order of FTransform multiplication.
    /// 
    /// Example: LocalToWorld = (LocalToWorld * DeltaRotation) will change rotation in local space by DeltaRotation.
    /// Example: LocalToWorld = (DeltaRotation * LocalToWorld) will change rotation in world space by DeltaRotation.
    /// </summary>
    [UStruct(Flags = 0x00406438), BlueprintType, UMetaPath("/Script/CoreUObject.Quat")]
    [StructLayout(LayoutKind.Sequential)]
    public struct FQuat : IEquatable<FQuat>
    {
        static bool X_IsValid;
        static int X_Offset;
        /// <summary>
        /// The quaternion's X-component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Quat:X")]
        public float X;

        static bool Y_IsValid;
        static int Y_Offset;
        /// <summary>
        /// The quaternion's Y-component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Quat:Y")]
        public float Y;

        static bool Z_IsValid;
        static int Z_Offset;
        /// <summary>
        /// The quaternion's Z-component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Quat:Z")]
        public float Z;

        static bool W_IsValid;
        static int W_Offset;
        /// <summary>
        /// The quaternion's W-component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Quat:W")]
        public float W;

        static int FQuat_StructSize;

        public FQuat Copy()
        {
            FQuat result = this;
            return result;
        }

        static FQuat()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FQuat)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FQuat));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Quat");
            FQuat_StructSize = NativeReflection.GetStructSize(classAddress);
            X_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "X");
            X_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "X", Classes.UFloatProperty);
            Y_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Y");
            Y_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Y", Classes.UFloatProperty);
            Z_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Z");
            Z_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Z", Classes.UFloatProperty);
            W_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "W");
            W_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "W", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FQuat));
        }

        /// <summary>
        /// Identity quaternion.
        /// </summary>
        public static readonly FQuat Identity = new FQuat(0, 0, 0, 1);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X component of the quaternion</param>
        /// <param name="y">Y component of the quaternion</param>
        /// <param name="z">Z component of the quaternion</param>
        /// <param name="w">W component of the quaternion</param>
        public FQuat(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="q">A FQuat object to use to create new quaternion from.</param>
        public FQuat(FQuat q)
        {
            X = q.X;
            Y = q.Y;
            Z = q.Z;
            W = q.W;
        }

        /// <summary>
        /// Creates and initializes a new quaternion from the given matrix.
        /// </summary>
        /// <param name="m">The rotation matrix to initialize from.</param>
        public FQuat(FMatrix m)
        {
            // If Matrix is NULL, return Identity quaternion. If any of them is 0, you won't be able to construct rotation
            // if you have two plane at least, we can reconstruct the frame using cross product, but that's a bit expensive op to do here
            // for now, if you convert to matrix from 0 scale and convert back, you'll lose rotation. Don't do that. 
            if (m.GetScaledAxis(EAxis.X).IsNearlyZero() || m.GetScaledAxis(EAxis.Y).IsNearlyZero() || m.GetScaledAxis(EAxis.Z).IsNearlyZero())
            {
                this = FQuat.Identity;
                return;
            }

            //#if !(UE_BUILD_SHIPPING || UE_BUILD_TEST)
            // Make sure the Rotation part of the Matrix is unit length.
            // Changed to this (same as RemoveScaling) from RotDeterminant as using two different ways of checking unit length matrix caused inconsistency. 
            if (!FMessage.Ensure(
                (FMath.Abs(1.0f - m.GetScaledAxis(EAxis.X).SizeSquared()) <= FMath.KindaSmallNumber) &&
                (FMath.Abs(1.0f - m.GetScaledAxis(EAxis.Y).SizeSquared()) <= FMath.KindaSmallNumber) &&
                (FMath.Abs(1.0f - m.GetScaledAxis(EAxis.Z).SizeSquared()) <= FMath.KindaSmallNumber)
                , "Make sure the Rotation part of the Matrix is unit length."))
            {
                this = FQuat.Identity;
                return;
            }
            //#endif

            //const MeReal *const t = (MeReal *) tm;
            float s;

            // Check diagonal (trace)
            float tr = m[0, 0] + m[1, 1] + m[2, 2];

            if (tr > 0.0f)
            {
                float invS = FMath.InvSqrt(tr + 1.0f);
                this.W = 0.5f * (1.0f / invS);
                s = 0.5f * invS;

                this.X = (m[1, 2] - m[2, 1]) * s;
                this.Y = (m[2, 0] - m[0, 2]) * s;
                this.Z = (m[0, 1] - m[1, 0]) * s;
            }
            else
            {
                // diagonal is negative
                int i = 0;

                if (m[1, 1] > m[0, 0])
                {
                    i = 1;
                }

                if (m[2, 2] > m[i, i])
                {
                    i = 2;
                }

                int[] nxt = { 1, 2, 0 };
                int j = nxt[i];
                int k = nxt[j];

                s = m[i, i] - m[j, j] - m[k, k] + 1.0f;

                float InvS = FMath.InvSqrt(s);

                float[] qt = new float[4];
                qt[i] = 0.5f * (1.0f / InvS);

                s = 0.5f * InvS;

                qt[3] = (m[j, k] - m[k, j]) * s;
                qt[j] = (m[i, j] + m[j, i]) * s;
                qt[k] = (m[i, k] + m[k, i]) * s;

                this.X = qt[0];
                this.Y = qt[1];
                this.Z = qt[2];
                this.W = qt[3];

                DiagnosticCheckNaN();
            }
        }

        /// <summary>
        /// Creates and initializes a new quaternion from the given rotator.
        /// </summary>
        /// <param name="r">The rotator to initialize from.</param>
        public FQuat(FRotator r)
        {
            this = r.Quaternion();
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Creates and initializes a new quaternion from the a rotation around the given axis.
        /// </summary>
        /// <param name="axis">assumed to be a normalized vector</param>
        /// <param name="angleRad">angle to rotate above the given axis (in radians)</param>
        public FQuat(FVector axis, float angleRad)
        {
            float halfA = 0.5f * angleRad;
            float s, c;
            FMath.SinCos(out s, out c, halfA);

            X = s * axis.X;
            Y = s * axis.Y;
            Z = s * axis.Z;
            W = c;

            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Gets the result of adding two quaternions.
        /// </summary>
        /// <param name="a">The first quaternion.</param>
        /// <param name="b">The second quaternion.</param>
        /// <returns>The result of the addition.</returns>
        public static FQuat operator +(FQuat a, FQuat b)
        {
            a.X += b.X;
            a.Y += b.Y;
            a.Z += b.Z;
            a.W += b.W;
            a.DiagnosticCheckNaN();
            return a;
        }

        /// <summary>
        /// Gets the result of subtracting two quaternions.
        /// </summary>
        /// <param name="a">The first quaternion.</param>
        /// <param name="b">The second quaternion.</param>
        /// <returns>The result of the subtraction.</returns>
        public static FQuat operator -(FQuat a, FQuat b)
        {
            a.X -= b.X;
            a.Y -= b.Y;
            a.Z -= b.Z;
            a.W -= b.W;
            a.DiagnosticCheckNaN();
            return a;
        }

        /// <summary>
        /// Gets the result of multiplying two quaternions.
        /// 
        /// Order matters when composing quaternions: C = A * B will yield a quaternion C that logically
        /// first applies B then A to any subsequent transformation (right first, then left).
        /// </summary>
        /// <param name="a">The first quaternion.</param>
        /// <param name="b">The second quaternion.</param>
        /// <returns>The result of the multiplication.</returns>
        public static FQuat operator *(FQuat a, FQuat b)
        {
            FQuat quaternion;
            float x = a.X;
            float y = a.Y;
            float z = a.Z;
            float w = a.W;
            float num4 = b.X;
            float num3 = b.Y;
            float num2 = b.Z;
            float num = b.W;
            float num12 = (y * num2) - (z * num3);
            float num11 = (z * num4) - (x * num2);
            float num10 = (x * num3) - (y * num4);
            float num9 = ((x * num4) + (y * num3)) + (z * num2);
            quaternion.X = ((x * num) + (num4 * w)) + num12;
            quaternion.Y = ((y * num) + (num3 * w)) + num11;
            quaternion.Z = ((z * num) + (num2 * w)) + num10;
            quaternion.W = (w * num) - num9;
            quaternion.DiagnosticCheckNaN();
            return quaternion;
        }

        /// <summary>
        /// Gets the result of scaling the quaternion.
        /// </summary>
        /// <param name="scale">The scaling factor.</param>
        /// <param name="q">The quaternion.</param>
        /// <returns>The result of the multiplication.</returns>
        public static FQuat operator *(float scale, FQuat q)
        {
            return q * scale;
        }

        /// <summary>
        /// Gets the result of scaling the quaternion.
        /// </summary>
        /// <param name="q">The quaternion.</param>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>The result of the multiplication.</returns>
        public static FQuat operator *(FQuat q, float scale)
        {
            q.X *= scale;
            q.Y *= scale;
            q.Z *= scale;
            q.W *= scale;
            q.DiagnosticCheckNaN();
            return q;
        }

        /// <summary>
        /// Divide the quaternion by scale.
        /// </summary>
        /// <param name="q">The quaternion.</param>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>The result of the scaling.</returns>
        public static FQuat operator /(float scale, FQuat q)
        {
            return q / scale;
        }

        /// <summary>
        /// Divide the quaternion by scale.
        /// </summary>
        /// <param name="q">The quaternion.</param>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>The result of the scaling.</returns>
        public static FQuat operator /(FQuat q, float scale)
        {
            float factor = 1.0f / scale;
            q.X *= factor;
            q.Y *= factor;
            q.Z *= factor;
            q.W *= factor;
            q.DiagnosticCheckNaN();
            return q;
        }

        /// <summary>
        /// Gets the result of dividing two quaternions.
        /// </summary>
        /// <param name="a">The first quaternion.</param>
        /// <param name="b">The second quaternion.</param>
        /// <returns>The result of the division.</returns>
        public static FQuat operator /(FQuat a, FQuat b)
        {
            FQuat quaternion;
            float x = a.X;
            float y = a.Y;
            float z = a.Z;
            float w = a.W;
            float num14 = (((b.X * b.X) + (b.Y * b.Y)) + (b.Z * b.Z)) + (b.W * b.W);
            float num5 = 1f / num14;
            float num4 = -b.X * num5;
            float num3 = -b.Y * num5;
            float num2 = -b.Z * num5;
            float num = b.W * num5;
            float num13 = (y * num2) - (z * num3);
            float num12 = (z * num4) - (x * num2);
            float num11 = (x * num3) - (y * num4);
            float num10 = ((x * num4) + (y * num3)) + (z * num2);
            quaternion.X = ((x * num) + (num4 * w)) + num13;
            quaternion.Y = ((y * num) + (num3 * w)) + num12;
            quaternion.Z = ((z * num) + (num2 * w)) + num11;
            quaternion.W = (w * num) - num10;
            quaternion.DiagnosticCheckNaN();
            return quaternion;
        }

        /// <summary>
        /// Rotate a vector by the quaternion.
        /// </summary>
        /// <param name="q">The quaternion.</param>
        /// <param name="v">The vector to be rotated.</param>
        /// <returns>vector after rotation</returns>
        /// <see cref="RotateVector"/>
        public static FVector operator *(FQuat q, FVector v)
        {
            return q.RotateVector(v);
        }

        /// <summary>
        /// Multiply the quaternion by a matrix.
        /// This matrix conversion came from
        /// http://www.m-hikari.com/ija/ija-password-2008/ija-password17-20-2008/aristidouIJA17-20-2008.pdf
        /// used for non-uniform scaling transform.
        /// </summary>
        /// <param name="q">The quaternion.</param>
        /// <param name="m">Matrix to multiply by.</param>
        /// <returns>Matrix result after multiplication.</returns>
        /// <see cref="RotateVector"/>
        public static FMatrix operator *(FQuat q, FMatrix m)
        {
            FMatrix result = default(FMatrix);
            FQuat vt, vr;
            FQuat inv = q.Inverse();
            for (int i = 0; i < 4; ++i)
            {
                FQuat vq = new FQuat(m[i, 0], m[i, 1], m[i, 2], m[i, 3]);
                vt = q * vq;
                vr = vt * inv;
                result[i, 0] = vr.X;
                result[i, 1] = vr.Y;
                result[i, 2] = vr.Z;
                result[i, 3] = vr.W;
            }
            return result;
        }

        /// <summary>
        /// Calculates dot product of two quaternions.
        /// </summary>
        /// <param name="a">The first quaternion.</param>
        /// <param name="b">The second quaternion.</param>
        /// <returns>The dot product.</returns>
        public static float operator |(FQuat a, FQuat b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

        /// <summary>
        /// Checks whether two quaternions are identical.
        /// This is an exact comparison per-component;see Equals() for a comparison
        /// that allows for a small error tolerance and flipped axes of rotation.
        /// </summary>
        /// <param name="a">The first quaternion.</param>
        /// <param name="b">The first quaternion.</param>
        /// <returns>true if two quaternion are identical, otherwise false.</returns>
        public static bool operator ==(FQuat a, FQuat b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }

        /// <summary>
        /// Checks whether two quaternions are not identical.
        /// </summary>
        /// <param name="a">The first quaternion.</param>
        /// <param name="b">The first quaternion.</param>
        /// <returns>true if two quaternion are not identical, otherwise false.</returns>
        public static bool operator !=(FQuat a, FQuat b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FQuat))
            {
                return false;
            }

            return Equals((FQuat)obj);
        }

        public bool Equals(FQuat other)
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
        /// Checks whether another Quaternion is equal to this, within specified tolerance.
        /// </summary>
        /// <param name="other">The other Quaternion.</param>
        /// <param name="tolerance">Error tolerance for comparison with other Quaternion.</param>
        /// <returns>true if two Quaternions are equal, within specified tolerance, otherwise false.</returns>
        public bool Equals(FQuat other, float tolerance = FMath.KindaSmallNumber)
        {
            return (FMath.Abs(X - other.X) <= tolerance && FMath.Abs(Y - other.Y) <= tolerance && FMath.Abs(Z - other.Z) <= tolerance && FMath.Abs(W - other.W) <= tolerance)
                || (FMath.Abs(X + other.X) <= tolerance && FMath.Abs(Y + other.Y) <= tolerance && FMath.Abs(Z + other.Z) <= tolerance && FMath.Abs(W + other.W) <= tolerance);
        }

        /// <summary>
        /// Checks whether this Quaternion is an Identity Quaternion.
        /// Assumes Quaternion tested is normalized.
        /// </summary>
        /// <param name="tolerance">Error tolerance for comparison with Identity Quaternion.</param>
        /// <returns>true if Quaternion is a normalized Identity Quaternion.</returns>
        public bool IsIdentity(float tolerance = FMath.SmallNumber)
        {
            return Equals(FQuat.Identity, tolerance);
        }

        /// <summary>
        /// Convert a vector of floating-point Euler angles (in degrees) into a Quaternion.
        /// </summary>
        /// <param name="euler">the Euler angles</param>
        /// <returns>constructed FQuat</returns>
        public static FQuat MakeFromEuler(FVector euler)
        {
            return FRotator.MakeFromEuler(euler).Quaternion();
        }

        /// <summary>
        /// Convert a Quaternion into floating-point Euler angles (in degrees).
        /// </summary>
        public FVector Euler()
        {
            return Rotator().Euler();
        }

        /// <summary>
        /// Normalize this quaternion if it is large enough.
        /// If it is too small, returns an identity quaternion.
        /// </summary>
        /// <param name="tolerance">Minimum squared length of quaternion for normalization.</param>
        public void Normalize(float tolerance = FMath.SmallNumber)
        {
            float squareSum = X * X + Y * Y + Z * Z + W * W;

            if (squareSum >= tolerance)
            {
                float Scale = FMath.InvSqrt(squareSum);

                X *= Scale;
                Y *= Scale;
                Z *= Scale;
                W *= Scale;
            }
            else
            {
                this = FQuat.Identity;
            }
        }

        /// <summary>
        /// Get a normalized copy of this quaternion.
        /// If it is too small, returns an identity quaternion.
        /// </summary>
        /// <param name="tolerance">Minimum squared length of quaternion for normalization.</param>
        public FQuat GetNormalized(float tolerance = FMath.SmallNumber)
        {
            FQuat result = this;
            result.Normalize(tolerance);
            return result;
        }

        /// <summary>
        /// Return true if this quaternion is normalized
        /// </summary>
        public bool IsNormalized()
        {
            return (FMath.Abs(1.0f - SizeSquared()) < FMath.THRESH_QUAT_NORMALIZED);
        }

        /// <summary>
        /// Get the length of this quaternion.
        /// </summary>
        /// <returns>The length of this quaternion.</returns>
        public float Size()
        {
            return FMath.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }

        /// <summary>
        /// Get the length squared of this quaternion.
        /// </summary>
        /// <returns>The length of this quaternion.</returns>
        public float SizeSquared()
        {
            return (X * X + Y * Y + Z * Z + W * W);
        }

        /// <summary>
        /// Get the angle of this quaternion
        /// </summary>
        public float GetAngle()
        {
            return 2.0f * FMath.Acos(W);
        }

        /// <summary>
        /// get the axis and angle of rotation of this quaternion (assumes normalized quaternions).
        /// </summary>
        /// <param name="axis">vector of axis of the quaternion</param>
        /// <param name="angle">angle of the quaternion</param>
        public void ToAxisAndAngle(out FVector axis, out float angle)
        {
            angle = GetAngle();
            axis = GetRotationAxis();
        }

        /// <summary>
        /// Get the swing and twist decomposition for a specified axis (assumes normalised quaternion and twist axis).
        /// </summary>
        /// <param name="twistAxis">Axis to use for decomposition</param>
        /// <param name="swing">swing component quaternion</param>
        /// <param name="twist">Twist component quaternion</param>
        public void ToSwingTwist(FVector twistAxis, out FQuat swing, out FQuat twist)
        {
            // Vector part projected onto twist axis
            FVector Projection = FVector.DotProduct(twistAxis, new FVector(X, Y, Z)) * twistAxis;

            // Twist quaternion
            twist = new FQuat(Projection.X, Projection.Y, Projection.Z, W);

            // Singularity close to 180deg
            if (twist.SizeSquared() == 0.0f)
            {
                twist = FQuat.Identity;
            }
            else
            {
                twist.Normalize();
            }

            // Set swing
            swing = this * twist.Inverse();
        }

        /// <summary>
        /// Rotate a vector by this quaternion.
        /// </summary>
        /// <param name="v">the vector to be rotated</param>
        /// <returns>vector after rotation</returns>
        public FVector RotateVector(FVector v)
        {
            // http://people.csail.mit.edu/bkph/articles/Quaternions.pdf
            // V' = V + 2w(Q x V) + (2Q x (Q x V))
            // refactor:
            // V' = V + w(2(Q x V)) + (Q x (2(Q x V)))
            // T = 2(Q x V);
            // V' = V + w*(T) + (Q x T)

            FVector q = new FVector(X, Y, Z);
            FVector t = 2.0f * FVector.CrossProduct(q, v);
            FVector result = v + (W * t) + FVector.CrossProduct(q, t);
            return result;
        }

        /// <summary>
        /// Rotate a vector by the inverse of this quaternion.
        /// </summary>
        /// <param name="v">the vector to be rotated</param>
        /// <returns>vector after rotation by the inverse of this quaternion.</returns>
        public FVector UnrotateVector(FVector v)
        {
            //return Inverse().RotateVector(V);

            FVector q = new FVector(-X, -Y, -Z); // Inverse
            FVector t = 2.0f * FVector.CrossProduct(q, v);
            FVector result = v + (W * t) + FVector.CrossProduct(q, t);
            return result;
        }

        /// <summary>
        /// Returns quaternion with W=0 and V=theta*v.
        /// </summary>
        public FQuat Log()
        {
            FQuat result;
            result.W = 0.0f;

            if (FMath.Abs(W) < 1.0f)
            {
                float angle = FMath.Acos(W);
                float sinAngle = FMath.Sin(angle);

                if (FMath.Abs(sinAngle) >= FMath.SmallNumber)
                {
                    float scale = angle / sinAngle;
                    result.X = scale * X;
                    result.Y = scale * Y;
                    result.Z = scale * Z;
                    return result;
                }
            }

            result.X = X;
            result.Y = Y;
            result.Z = Z;
            return result;
        }

        /// <summary>
        /// @note Exp should really only be used after Log.
        /// Assumes a quaternion with W=0 and V=theta*v (where |v| = 1).
        /// Exp(q) = (sin(theta)*v, cos(theta))
        /// </summary>
        public FQuat Exp()
        {
            float angle = FMath.Sqrt(X * X + Y * Y + Z * Z);
            float sinAngle = FMath.Sin(angle);

            FQuat result;
            result.W = FMath.Cos(angle);

            if (FMath.Abs(sinAngle) >= FMath.SmallNumber)
            {
                float scale = sinAngle / angle;
                result.X = scale * X;
                result.Y = scale * Y;
                result.Z = scale * Z;
            }
            else
            {
                result.X = X;
                result.Y = Y;
                result.Z = Z;
            }

            return result;
        }

        /// <summary>
        /// Returns the inverse of this quaternion
        /// </summary>
        public FQuat Inverse()
        {
            Debug.Assert(IsNormalized());
            return new FQuat(-X, -Y, -Z, W);
        }

        /// <summary>
        /// Enforce that the delta between this Quaternion and another one represents
        /// the shortest possible rotation angle
        /// </summary>
        public void EnforceShortestArcWith(FQuat other)
        {
            float dotResult = (other | this);
            float bias = FMath.FloatSelect(dotResult, 1.0f, -1.0f);

            X *= bias;
            Y *= bias;
            Z *= bias;
            W *= bias;
        }

        /// <summary>
        /// Get the forward direction (X axis) after it has been rotated by this Quaternion.
        /// </summary>
        public FVector GetAxisX()
        {
            return RotateVector(new FVector(1.0f, 0.0f, 0.0f));
        }

        /// <summary>
        /// Get the right direction (Y axis) after it has been rotated by this Quaternion.
        /// </summary>
        public FVector GetAxisY()
        {
            return RotateVector(new FVector(0.0f, 1.0f, 0.0f));
        }

        /// <summary>
        /// Get the up direction (Z axis) after it has been rotated by this Quaternion.
        /// </summary>
        public FVector GetAxisZ()
        {
            return RotateVector(new FVector(0.0f, 0.0f, 1.0f));
        }

        /// <summary>
        /// Get the forward direction (X axis) after it has been rotated by this Quaternion.
        /// </summary>
        public FVector GetForwardVector()
        {
            return GetAxisX();
        }

        /// <summary>
        /// Get the right direction (Y axis) after it has been rotated by this Quaternion.
        /// </summary>
        public FVector GetRightVector()
        {
            return GetAxisY();
        }

        /// <summary>
        /// Get the up direction (Z axis) after it has been rotated by this Quaternion.
        /// </summary>
        public FVector GetUpVector()
        {
            return GetAxisZ();
        }

        /// <summary>
        /// Convert a rotation into a unit vector facing in its direction. Equivalent to GetForwardVector().
        /// </summary>
        /// <returns></returns>
        public FVector Vector()
        {
            return GetAxisX();
        }

        /// <summary>
        /// Get the FRotator representation of this Quaternion.
        /// </summary>
        public FRotator Rotator()
        {
            DiagnosticCheckNaN();

            float singularityTest = Z * X - W * Y;
            float yawY = 2.0f * (W * Z + X * Y);
            float yawX = (1.0f - 2.0f * (FMath.Square(Y) + FMath.Square(Z)));

            // reference 
            // http://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToEuler/

            // this value was found from experience, the above websites recommend different values
            // but that isn't the case for us, so I went through different testing, and finally found the case 
            // where both of world lives happily. 
            const float singularityThreshold = 0.4999995f;
            const float radToDeg = (180.0f) / FMath.PI;
            FRotator rotatorFromQuat;

            if (singularityTest < -singularityThreshold)
            {
                rotatorFromQuat.Pitch = -90.0f;
                rotatorFromQuat.Yaw = FMath.Atan2(yawY, yawX) * radToDeg;
                rotatorFromQuat.Roll = FRotator.NormalizeAxis(-rotatorFromQuat.Yaw - (2.0f * FMath.Atan2(X, W) * radToDeg));
            }
            else if (singularityTest > singularityThreshold)
            {
                rotatorFromQuat.Pitch = 90.0f;
                rotatorFromQuat.Yaw = FMath.Atan2(yawY, yawX) * radToDeg;
                rotatorFromQuat.Roll = FRotator.NormalizeAxis(rotatorFromQuat.Yaw - (2.0f * FMath.Atan2(X, W) * radToDeg));
            }
            else
            {
                rotatorFromQuat.Pitch = FMath.FastAsin(2.0f * (singularityTest)) * radToDeg;
                rotatorFromQuat.Yaw = FMath.Atan2(yawY, yawX) * radToDeg;
                rotatorFromQuat.Roll = FMath.Atan2(-2.0f * (W * X + Y * Z), (1.0f - 2.0f * (FMath.Square(X) + FMath.Square(Y)))) * radToDeg;
            }

            rotatorFromQuat.DiagnosticCheckNaN("FQuat::Rotator(): Rotator result " + rotatorFromQuat + " contains NaN! Quat = " + this.ToString() +
                ", YawY = " + yawY + ", YawX = " + yawX);

            return rotatorFromQuat;
        }

        /// <summary>
        /// Get the axis of rotation of the Quaternion.
        /// This is the axis around which rotation occurs to transform the canonical coordinate system to the target orientation.
        /// For the identity Quaternion which has no such rotation, FVector(1,0,0) is returned.
        /// </summary>
        public FVector GetRotationAxis()
        {
            // Ensure we never try to sqrt a neg number
            float s = FMath.Sqrt(FMath.Max(1.0f - (W * W), 0.0f));

            if (s >= 0.0001f)
            {
                return new FVector(X / s, Y / s, Z / s);
            }

            return new FVector(1.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Find the angular distance between two rotation quaternions (in radians)
        /// </summary>
        public float AngularDistance(FQuat q)
        {
            float InnerProd = X * q.X + Y * q.Y + Z * q.Z + W * q.W;
            return FMath.Acos((2 * InnerProd * InnerProd) - 1.0f);
        }

        /// <summary>
        /// Utility to check if there are any non-finite values (NaN or Inf) in this Quat.
        /// </summary>
        /// <returns>true if there are any non-finite values in this Quaternion, otherwise false.</returns>
        public bool ContainsNaN()
        {
            return (!FMath.IsFinite(X) || !FMath.IsFinite(Y) || !FMath.IsFinite(Z) || !FMath.IsFinite(W));
        }

        public override string ToString()
        {
            return "X=" + X + " Y=" + Y + " Z=" + Z + " W=" + W;
        }

        /// <summary>
        /// Initialize this FQuat from a string.
        /// The string is expected to contain X=, Y=, Z=, W=, otherwise 
        /// this FQuat will have indeterminate (invalid) values.
        /// </summary>
        /// <param name="sourceString">String containing the quaternion values.</param>
        /// <returns>true if the FQuat was initialized; false otherwise.</returns>
        public bool InitFromString(string sourceString)
        {
            X = Y = Z = 0.0f;
            W = 1.0f;

            bool success = 
                FParse.Value(sourceString, "X=", ref X) &&
                FParse.Value(sourceString, "Y=", ref Y) &&
                FParse.Value(sourceString, "Z=", ref Z) &&
                FParse.Value(sourceString, "W=", ref W);
            DiagnosticCheckNaN();
            return success;
        }

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN()
        {
            if (ContainsNaN())
            {
                FMath.LogOrEnsureNanError("FQuat contains NaN: " + ToString());
                this = FQuat.Identity;
            }
        }

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN(string message)
        {
            if (ContainsNaN())
            {
                FMath.LogOrEnsureNanError(message + ": FQuat contains NaN: " + ToString());
                this = FQuat.Identity;
            }
        }

        // Based on:
        // http://lolengine.net/blog/2014/02/24/quaternion-from-two-vectors-final
        // http://www.euclideanspace.com/maths/algebra/vectors/angleBetween/index.htm
        private static FQuat FindBetween_Helper(FVector a, FVector b, float normAB)
        {
            float w = normAB + FVector.DotProduct(a, b);
            FQuat result;

            if (w >= 1e-6f * normAB)
            {
                //Axis = FVector::CrossProduct(A, B);
                result = new FQuat(
                    a.Y * b.Z - a.Z * b.Y,
                    a.Z * b.X - a.X * b.Z,
                    a.X * b.Y - a.Y * b.X,
                    w);
            }
            else
            {
                // A and B point in opposite directions
                w = 0.0f;
                result = FMath.Abs(a.X) > FMath.Abs(a.Y)
                    ? new FQuat(-a.Z, 0.0f, a.X, w)
                    : new FQuat(0.0f, -a.Z, a.Y, w);
            }

            result.Normalize();
            return result;
        }

        /// <summary>
        /// Generates the 'smallest' (geodesic) rotation between two vectors of arbitrary length.
        /// </summary>
        public static FQuat FindBetween(FVector vector1, FVector vector2)
        {
            return FindBetweenVectors(vector1, vector2);
        }

        /// <summary>
        /// Generates the 'smallest' (geodesic) rotation between two normals (assumed to be unit length).
        /// </summary>
        public static FQuat FindBetweenNormals(FVector normal1, FVector normal2)
        {
            const float normAB = 1.0f;
            return FindBetween_Helper(normal1, normal2, normAB);
        }

        /// <summary>
        /// Generates the 'smallest' (geodesic) rotation between two vectors of arbitrary length.
        /// </summary>
        public static FQuat FindBetweenVectors(FVector vector1, FVector vector2)
        {
            float normAB = FMath.Sqrt(vector1.SizeSquared() * vector2.SizeSquared());
            return FindBetween_Helper(vector1, vector2, normAB);
        }

        /// <summary>
        /// Error measure (angle) between two quaternions, ranged [0..1].
        /// Returns the hypersphere-angle between two quaternions; alignment shouldn't matter, though 
        /// @note normalized input is expected.
        /// </summary>
        public static float Error(FQuat q1, FQuat q2)
        {
            float cosom = FMath.Abs(q1.X * q2.X + q1.Y * q2.Y + q1.Z * q2.Z + q1.W * q2.W);
            return (FMath.Abs(cosom) < 0.9999999f) ? FMath.Acos(cosom) * (1.0f / FMath.PI) : 0.0f;
        }

        /// <summary>
        /// FQuat::Error with auto-normalization.
        /// </summary>
        public static float ErrorAutoNormalize(FQuat a, FQuat b)
        {
            FQuat q1 = a;
            q1.Normalize();

            FQuat q2 = b;
            q2.Normalize();

            return FQuat.Error(q1, q2);
        }

        /// <summary>
        /// Fast Linear Quaternion Interpolation.
        /// Result is NOT normalized.
        /// </summary>
        public static FQuat FastLerp(FQuat a, FQuat b, float alpha)
        {
            // To ensure the 'shortest route', we make sure the dot product between the both rotations is positive.
            float dotResult = (a | b);
            float bias = FMath.FloatSelect(dotResult, 1.0f, -1.0f);
            return (b * alpha) + (a * (bias * (1.0f - alpha)));
        }

        /// <summary>
        /// Bi-Linear Quaternion interpolation.
        /// Result is NOT normalized.
        /// </summary>
        public static FQuat FastBilerp(FQuat p00, FQuat p10, FQuat p01, FQuat p11, float fracX, float fracY)
        {
            return FQuat.FastLerp(
                FQuat.FastLerp(p00, p10, fracX),
                FQuat.FastLerp(p01, p11, fracX),
                fracY);
        }

        /// <summary>
        /// Spherical interpolation. Will correct alignment. Result is NOT normalized.
        /// </summary>
        public static FQuat Slerp_NotNormalized(FQuat quat1, FQuat quat2, float slerp)
        {
            // Get cosine of angle between quats.
            float rawCosom =
                quat1.X * quat2.X +
                quat1.Y * quat2.Y +
                quat1.Z * quat2.Z +
                quat1.W * quat2.W;
            // Unaligned quats - compensate, results in taking shorter route.
            float cosom = FMath.FloatSelect(rawCosom, rawCosom, -rawCosom);

            float scale0, scale1;

            if (cosom < 0.9999f)
            {
                float omega = FMath.Acos(cosom);
                float invSin = 1.0f / FMath.Sin(omega);
                scale0 = FMath.Sin((1.0f - slerp) * omega) * invSin;
                scale1 = FMath.Sin(slerp * omega) * invSin;
            }
            else
            {
                // Use linear interpolation.
                scale0 = 1.0f - slerp;
                scale1 = slerp;
            }

            // In keeping with our flipped Cosom:
            scale1 = FMath.FloatSelect(rawCosom, scale1, -scale1);

            FQuat result;

            result.X = scale0 * quat1.X + scale1 * quat2.X;
            result.Y = scale0 * quat1.Y + scale1 * quat2.Y;
            result.Z = scale0 * quat1.Z + scale1 * quat2.Z;
            result.W = scale0 * quat1.W + scale1 * quat2.W;

            return result;
        }

        /// <summary>
        /// Spherical interpolation. Will correct alignment. Result is normalized.
        /// </summary>
        public static FQuat Slerp(FQuat Quat1, FQuat Quat2, float Slerp)
        {
            return Slerp_NotNormalized(Quat1, Quat2, Slerp).GetNormalized();
        }

        /// <summary>
        /// Simpler Slerp that doesn't do any checks for 'shortest distance' etc.
        /// We need this for the cubic interpolation stuff so that the multiple Slerps dont go in different directions.
        /// Result is NOT normalized.
        /// </summary>
        public static FQuat SlerpFullPath_NotNormalized(FQuat quat1, FQuat quat2, float alpha)
        {
            float cosAngle = FMath.Clamp(quat1 | quat2, -1.0f, 1.0f);
            float angle = FMath.Acos(cosAngle);

            //UE_LOG(LogUnrealMath, Log,  TEXT("CosAngle: %f Angle: %f"), CosAngle, Angle );

            if (FMath.Abs(angle) < FMath.KindaSmallNumber)
            {
                return quat1;
            }

            float sinAngle = FMath.Sin(angle);
            float invSinAngle = 1.0f / sinAngle;

            float scale0 = FMath.Sin((1.0f - alpha) * angle) * invSinAngle;
            float scale1 = FMath.Sin(alpha * angle) * invSinAngle;

            return quat1 * scale0 + quat2 * scale1;
        }

        /// <summary>
        /// Simpler Slerp that doesn't do any checks for 'shortest distance' etc.
        /// We need this for the cubic interpolation stuff so that the multiple Slerps dont go in different directions.
        /// Result is normalized.
        /// </summary>
        public static FQuat SlerpFullPath(FQuat quat1, FQuat quat2, float alpha)
        {
            return SlerpFullPath_NotNormalized(quat1, quat2, alpha).GetNormalized();
        }

        /// <summary>
        /// Given start and end quaternions of quat1 and quat2, and tangents at those points tang1 and tang2, calculate the point at Alpha (between 0 and 1) between them. Result is normalized.
        /// This will correct alignment by ensuring that the shortest path is taken.
        /// </summary>
        public static FQuat Squad(FQuat quat1, FQuat tang1, FQuat quat2, FQuat tang2, float alpha)
        {
            // Always slerp along the short path from quat1 to quat2 to prevent axis flipping.
            // This approach is taken by OGRE engine, amongst others.
            FQuat q1 = FQuat.Slerp_NotNormalized(quat1, quat2, alpha);
            FQuat q2 = FQuat.SlerpFullPath_NotNormalized(tang1, tang2, alpha);
            FQuat result = FQuat.SlerpFullPath(q1, q2, 2.0f * alpha * (1.0f - alpha));
            return result;
        }

        /// <summary>
        /// Simpler Squad that doesn't do any checks for 'shortest distance' etc.
        /// Given start and end quaternions of quat1 and quat2, and tangents at those points tang1 and tang2, calculate the point at Alpha (between 0 and 1) between them. Result is normalized.
        /// </summary>
        public static FQuat SquadFullPath(FQuat quat1, FQuat tang1, FQuat quat2, FQuat tang2, float alpha)
        {
            FQuat q1 = FQuat.SlerpFullPath_NotNormalized(quat1, quat2, alpha);
            FQuat q2 = FQuat.SlerpFullPath_NotNormalized(tang1, tang2, alpha);
            FQuat result = FQuat.SlerpFullPath(q1, q2, 2.0f * alpha * (1.0f - alpha));
            return result;
        }

        /// <summary>
        /// Calculate tangents between given points
        /// </summary>
        /// <param name="prevP">quaternion at P-1</param>
        /// <param name="p">quaternion to return the tangent</param>
        /// <param name="nextP">quaternion P+1</param>
        /// <param name="tension">@todo document</param>
        /// <param name="tan">Out control point</param>
        public static void CalcTangents(FQuat prevP, FQuat p, FQuat nextP, float tension, out FQuat tan)
        {
            FQuat InvP = p.Inverse();
            FQuat Part1 = (InvP * prevP).Log();
            FQuat Part2 = (InvP * nextP).Log();

            FQuat PreExp = (Part1 + Part2) * -0.5f;

            tan = p * PreExp.Exp();
        }

        // FMath::Lerp
        // FMath::BiLerp
        // FMath::CubicInterp

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FQuat Lerp(FQuat a, FQuat b, float alpha)
        {
            return FQuat.Slerp(a, b, alpha);
        }

        /// <summary>
        /// Performs a 2D linear interpolation between four values values, FracX, FracY ranges from 0-1
        /// </summary>
        public static FQuat BiLerp(FQuat p00, FQuat p10, FQuat p01, FQuat p11, float fracX, float fracY)
        {
            FQuat result;

            result = Lerp(
                FQuat.Slerp_NotNormalized(p00, p10, fracX),
                FQuat.Slerp_NotNormalized(p01, p11, fracX),
                fracY);

            return result;
        }

        /// <summary>
        /// Performs a cubic interpolation
        /// </summary>
        /// <param name="p0">end points</param>
        /// <param name="t0">tangent directions at end points</param>
        /// <param name="p1">end points</param>
        /// <param name="t1">tangent directions at end points</param>
        /// <param name="a">distance along spline</param>
        /// <returns>Interpolated value</returns>
        public static FQuat CubicInterp(FQuat p0, FQuat t0, FQuat p1, FQuat t1, float a)
        {
            return FQuat.Squad(p0, t0, p1, t1, a);
        }
    }
}
