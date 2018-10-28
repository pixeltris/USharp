using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Rotator.h

    /// <summary>
    /// Implements a container for rotation information.
    /// 
    /// All rotation values are stored in degrees.
    /// </summary>
    [UStruct(Flags = 0x0040EC38), BlueprintType, UMetaPath("/Script/CoreUObject.Rotator", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FRotator : IEquatable<FRotator>
    {
        static bool Pitch_IsValid;
        static int Pitch_Offset;
        /// <summary>Rotation around the right axis (around Y axis), Looking up and down (0=Straight Ahead, +Up, -Down)</summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Rotator:Pitch")]
        public float Pitch;

        static bool Yaw_IsValid;
        static int Yaw_Offset;
        /// <summary>Rotation around the up axis (around Z axis), Running in circles 0=East, +North, -South.</summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Rotator:Yaw")]
        public float Yaw;

        static bool Roll_IsValid;
        static int Roll_Offset;
        /// <summary>Rotation around the forward axis (around X axis), Tilting your head, 0=Straight, +Clockwise, -CCW.</summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Rotator:Roll")]
        public float Roll;

        static int FRotator_StructSize;

        public FRotator Copy()
        {
            FRotator result = this;
            return result;
        }

        static FRotator()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FRotator)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FRotator));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Rotator");
            FRotator_StructSize = NativeReflection.GetStructSize(classAddress);
            Pitch_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Pitch");
            Pitch_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Pitch", Classes.UFloatProperty);
            Yaw_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Yaw");
            Yaw_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Yaw", Classes.UFloatProperty);
            Roll_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Roll");
            Roll_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Roll", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FRotator));
        }

        /// <summary>
        /// A rotator of zero degrees on each axis.
        /// </summary>
        public static readonly FRotator ZeroRotator = new FRotator(0.0f, 0.0f, 0.0f);

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN()
        {
            if (ContainsNaN())
            {
                FMath.LogOrEnsureNanError("FRotator contains NaN: " + ToString());
                this = ZeroRotator;
            }
        }

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN(string message)
        {
            if (ContainsNaN())
            {
                FMath.LogOrEnsureNanError(message + ": FRotator contains NaN: " + ToString());
                this = ZeroRotator;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="f">Value to set all components to.</param>
        public FRotator(float f)
        {
            Pitch = f;
            Yaw = f;
            Roll = f;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pitch">Pitch in degrees.</param>
        /// <param name="yaw">Yaw in degrees.</param>
        /// <param name="roll">Roll in degrees.</param>
        public FRotator(float pitch, float yaw, float roll)
        {
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="quat">Quaternion used to specify rotation.</param>
        public FRotator(FQuat quat)
        {
            this = quat.Rotator();
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Gets the result of adding two rotators.
        /// </summary>
        /// <param name="a">The first rotator.</param>
        /// <param name="b">The second rotator.</param>
        /// <returns>The result of the addition.</returns>
        public static FRotator operator +(FRotator a, FRotator b)
        {
            return new FRotator(a.Pitch + b.Pitch, a.Yaw + b.Yaw, a.Roll + b.Roll);
        }

        /// <summary>
        /// Gets the result of subtracting two rotators.
        /// </summary>
        /// <param name="a">The first rotator.</param>
        /// <param name="b">The second rotator.</param>
        /// <returns>The result of the subtraction.</returns>
        public static FRotator operator -(FRotator a, FRotator b)
        {
            return new FRotator(a.Pitch - b.Pitch, a.Yaw - b.Yaw, a.Roll - b.Roll);
        }

        /// <summary>
        /// Gets the result of scaling the rotator.
        /// </summary>
        /// <param name="scale">The scaling factor.</param>
        /// <param name="r">The rotator.</param>        
        /// <returns>The result of scaling.</returns>
        public static FRotator operator *(float scale, FRotator r)
        {
            return r * scale;
        }

        /// <summary>
        /// Gets the result of scaling the rotator.
        /// </summary>
        /// <param name="r">The rotator.</param>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>The result of scaling.</returns>
        public static FRotator operator *(FRotator r, float scale)
        {
            return new FRotator(r.Pitch * scale, r.Yaw * scale, r.Roll * scale);
        }

        /// <summary>
        /// Checks two rotators for equality.
        /// </summary>
        /// <param name="a">The first rotator.</param>
        /// <param name="b">The first rotator.</param>
        /// <returns>true if the rotators are equal, false otherwise.</returns>
        public static bool operator ==(FRotator a, FRotator b)
        {
            return a.Pitch == b.Pitch && a.Yaw == b.Yaw && a.Roll == b.Roll;
        }

        /// <summary>
        /// Checks two rotators for inequality.
        /// </summary>
        /// <param name="a">The first rotator.</param>
        /// <param name="b">The first rotator.</param>
        /// <returns>true if the rotators are not equal, false otherwise.</returns>
        public static bool operator !=(FRotator a, FRotator b)
        {
            return a.Pitch != b.Pitch || a.Yaw != b.Yaw || a.Roll == b.Roll;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FRotator))
            {
                return false;
            }

            return Equals((FRotator)obj);
        }

        public bool Equals(FRotator other)
        {
            return Pitch == other.Pitch && Yaw == other.Yaw && Roll == other.Roll;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = Pitch.GetHashCode();
                hashcode = (hashcode * 397) ^ Yaw.GetHashCode();
                hashcode = (hashcode * 397) ^ Roll.GetHashCode();
                return hashcode;
            }
        }

        /// <summary>
        /// Checks whether rotator is nearly zero within specified tolerance, when treated as an orientation.
        /// This means that FRotator(0, 0, 360) is "zero", because it is the same final orientation as the zero rotator.
        /// </summary>
        /// <param name="tolerance">Error Tolerance.</param>
        /// <returns>true if rotator is nearly zero, within specified tolerance, otherwise false.</returns>
        public bool IsNearlyZero(float tolerance = FMath.KindaSmallNumber)
        {
            return
                FMath.Abs(NormalizeAxis(Pitch)) <= tolerance &&
                FMath.Abs(NormalizeAxis(Yaw)) <= tolerance &&
                FMath.Abs(NormalizeAxis(Roll)) <= tolerance;
        }

        /// <summary>
        /// Checks whether this has exactly zero rotation, when treated as an orientation.
        /// This means that FRotator(0, 0, 360) is "zero", because it is the same final orientation as the zero rotator.
        /// </summary>
        /// <returns>true if this has exactly zero rotation, otherwise false.</returns>
        public bool IsZero()
        {
            return (ClampAxis(Pitch) == 0.0f) && (ClampAxis(Yaw) == 0.0f) && (ClampAxis(Roll) == 0.0f);
        }

        /// <summary>
        /// Checks whether two rotators are equal within specified tolerance, when treated as an orientation.
        /// This means that FRotator(0, 0, 360).Equals(FRotator(0,0,0)) is true, because they represent the same final orientation.
        /// </summary>
        /// <param name="other">The other rotator.</param>
        /// <param name="tolerance">Error Tolerance.</param>
        /// <returns>true if two rotators are equal, within specified tolerance, otherwise false.</returns>
        public bool Equals(FRotator other, float tolerance = FMath.KindaSmallNumber)
        {
            return 
                (FMath.Abs(NormalizeAxis(Pitch - other.Pitch)) <= tolerance) &&
                (FMath.Abs(NormalizeAxis(Yaw - other.Yaw)) <= tolerance) &&
                (FMath.Abs(NormalizeAxis(Roll - other.Roll)) <= tolerance);
        }

        /// <summary>
        /// Adds to each component of the rotator.
        /// </summary>
        /// <param name="deltaPitch">Change in pitch. (+/-)</param>
        /// <param name="deltaYaw">Change in yaw. (+/-)</param>
        /// <param name="deltaRoll">Change in roll. (+/-)</param>
        /// <returns>Copy of rotator after addition.</returns>
        public FRotator Add(float deltaPitch, float deltaYaw, float deltaRoll)
        {
            Yaw += deltaYaw;
            Pitch += deltaPitch;
            Roll += deltaRoll;
            DiagnosticCheckNaN();
            return this;
        }

        /// <summary>
        /// Returns the inverse of the rotator.
        /// </summary>
        public FRotator GetInverse()
        {
            return Quaternion().Inverse().Rotator();
        }

        /// <summary>
        /// Get the rotation, snapped to specified degree segments.
        /// </summary>
        /// <param name="rotGrid">A Rotator specifying how to snap each component.</param>
        /// <returns>Snapped version of rotation.</returns>
        public FRotator GridSnap(FRotator rotGrid)
        {
            return new FRotator(
                FMath.GridSnap(Pitch, rotGrid.Pitch),
                FMath.GridSnap(Yaw, rotGrid.Yaw),
                FMath.GridSnap(Roll, rotGrid.Roll));
        }

        /// <summary>
        /// Convert a rotation into a unit vector facing in its direction.
        /// </summary>
        /// <returns>Rotation as a unit direction vector.</returns>
        public FVector Vector()
        {
            float cp, sp, cy, sy;
            FMath.SinCos(out sp, out cp, FMath.DegreesToRadians(Pitch));
            FMath.SinCos(out sy, out cy, FMath.DegreesToRadians(Yaw));
            return new FVector(cp * cy, cp * sy, sp);
        }

        /// <summary>
        /// Get Rotation as a quaternion.
        /// </summary>
        /// <returns>Rotation as a quaternion.</returns>
        public FQuat Quaternion()
        {
            DiagnosticCheckNaN();

            float degToRad = FMath.PI / (180.0f);
            float divideByTwo = degToRad / 2.0f;
            float sp, sy, sr;
            float cp, cy, cr;

            FMath.SinCos(out sp, out cp, Pitch * divideByTwo);
            FMath.SinCos(out sy, out cy, Yaw * divideByTwo);
            FMath.SinCos(out sr, out cr, Roll * divideByTwo);

            FQuat rotationQuat;
            rotationQuat.X = cr * sp * sy - sr * cp * cy;
            rotationQuat.Y = -cr * sp * cy - sr * cp * sy;
            rotationQuat.Z = cr * cp * sy - sr * sp * cy;
            rotationQuat.W = cr * cp * cy + sr * sp * sy;

            rotationQuat.DiagnosticCheckNaN("Invalid input to FRotator::Quaternion - generated NaN output: " + rotationQuat);

            return rotationQuat;
        }

        /// <summary>
        /// Convert a Rotator into floating-point Euler angles (in degrees). Rotator now stored in degrees.
        /// </summary>
        /// <returns>Rotation as a Euler angle vector.</returns>
        public FVector Euler()
        {
            return new FVector(Roll, Pitch, Yaw);
        }

        /// <summary>
        /// Rotate a vector rotated by this rotator.
        /// </summary>
        /// <param name="v">The vector to rotate.</param>
        /// <returns>The rotated vector.</returns>
        public FVector RotateVector(FVector v)
        {
            return FMatrix.CreateRotation(this).TransformVector(v);
        }

        /// <summary>
        /// Returns the vector rotated by the inverse of this rotator.
        /// </summary>
        /// <param name="v">The vector to rotate.</param>
        /// <returns>The rotated vector.</returns>
        public FVector UnrotateVector(FVector v)
        {
            return FMatrix.CreateRotation(this).GetTransposed().TransformVector(v);
        }

        /// <summary>
        /// Gets the rotation values so they fall within the range [0,360]
        /// </summary>
        /// <returns>Clamped version of rotator.</returns>
        public FRotator Clamp()
        {
            return new FRotator(ClampAxis(Pitch), ClampAxis(Yaw), ClampAxis(Roll));
        }

        /// <summary>
        /// Create a copy of this rotator and normalize, removes all winding and creates the "shortest route" rotation. 
        /// </summary>
        /// <returns>Normalized copy of this rotator</returns>
        public FRotator GetNormalized()
        {
            FRotator rot = this;
            rot.Normalize();
            return rot;
        }

        /// <summary>
        /// Create a copy of this rotator and denormalize, clamping each axis to 0 - 360. 
        /// </summary>
        /// <returns>Denormalized copy of this rotator</returns>
        public FRotator GetDenormalized()
        {
            FRotator rot = this;
            rot.Pitch = ClampAxis(rot.Pitch);
            rot.Yaw = ClampAxis(rot.Yaw);
            rot.Roll = ClampAxis(rot.Roll);
            return rot;
        }

        /// <summary>
        /// Get a specific component of the vector, given a specific axis by enum
        /// </summary>
        public float GetComponentForAxis(EAxis axis)
        {
            switch (axis)
            {
                case EAxis.X:
                    return Roll;
                case EAxis.Y:
                    return Pitch;
                case EAxis.Z:
                    return Yaw;
                default:
                    return 0.0f;
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
                    Roll = component;
                    break;
                case EAxis.Y:
                    Pitch = component;
                    break;
                case EAxis.Z:
                    Yaw = component;
                    break;
            }
        }

        /// <summary>
        /// In-place normalize, removes all winding and creates the "shortest route" rotation.
        /// </summary>
        public void Normalize()
        {
            Pitch = NormalizeAxis(Pitch);
            Yaw = NormalizeAxis(Yaw);
            Roll = NormalizeAxis(Roll);
            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Decompose this Rotator into a Winding part (multiples of 360) and a Remainder part. 
        /// Remainder will always be in [-180, 180] range.
        /// </summary>
        /// <param name="winding">the Winding part of this Rotator</param>
        /// <param name="remainder">the Remainder</param>
        public void GetWindingAndRemainder(out FRotator winding, out FRotator remainder)
        {
            //// YAW
            remainder.Yaw = NormalizeAxis(Yaw);

            winding.Yaw = Yaw - remainder.Yaw;

            //// PITCH
            remainder.Pitch = NormalizeAxis(Pitch);

            winding.Pitch = Pitch - remainder.Pitch;

            //// ROLL
            remainder.Roll = NormalizeAxis(Roll);

            winding.Roll = Roll - remainder.Roll;
        }

        /// <summary>
        /// Return the manhattan distance in degrees between this Rotator and the passed in one.
        /// </summary>
        /// <param name="rotator">the Rotator we are comparing with.</param>
        /// <returns>Distance(Manhattan) between the two rotators. </returns>
        public float GetManhattanDistance(FRotator rotator)
        {
            return FMath.Abs(Yaw - rotator.Yaw) + FMath.Abs(Pitch - rotator.Pitch) + FMath.Abs(Roll - rotator.Roll);
        }

        /// <summary>
        /// Return a Rotator that has the same rotation but has different degree values for Yaw, Pitch, and Roll.
        /// This rotator should be within -180,180 range,
        /// </summary>
        /// <returns>A Rotator with the same rotation but different degrees.</returns>
        public FRotator GetEquivalentRotator()
        {
            return new FRotator(180.0f - Pitch, Yaw + 180.0f, Roll + 180.0f);
        }

        /// <summary>
        /// Modify if necessary the passed in rotator to be the closest rotator to it based upon it's equivalent.
        /// This Rotator should be within (-180, 180], usually just constructed from a Matrix or a Quaternion.
        /// </summary>
        /// <param name="makeClosest">the Rotator we want to make closest to us. Should be between 
        /// (-180, 180]. This Rotator may change if we need to use different degree values to make it closer.</param>
        public void SetClosestToMe(ref FRotator makeClosest)
        {
            FRotator otherChoice = makeClosest.GetEquivalentRotator();
            float firstDiff = GetManhattanDistance(makeClosest);
            float SecondDiff = GetManhattanDistance(otherChoice);
            if (SecondDiff < firstDiff)
            {
                makeClosest = otherChoice;
            }
        }

        public override string ToString()
        {
            return "P=" + Pitch + " Y=" + Yaw + " R=" + Roll;
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
                return "R(0)";
            }

            string returnString = "R(";
            bool isEmptyString = true;
            if (!FMath.IsNearlyZero(Pitch))
            {
                returnString += "P=" + Pitch.ToString(numericFormat);
                isEmptyString = false;
            }
            if (!FMath.IsNearlyZero(Yaw))
            {
                if (!isEmptyString)
                {
                    returnString += ", ";
                }
                returnString += "Y=" + Yaw.ToString(numericFormat);
                isEmptyString = false;
            }
            if (!FMath.IsNearlyZero(Roll))
            {
                if (!isEmptyString)
                {
                    returnString += ", ";
                }
                returnString += "R=" + Roll.ToString(numericFormat);
                isEmptyString = false;
            }
            returnString += ")";
            return returnString;
        }

        /// <summary>
        /// Initialize this Rotator based on an FString. The String is expected to contain P=, Y=, R=.
        /// The FRotator will be bogus when InitFromString returns false.
        /// </summary>
        /// <param name="sourceString">String containing the rotator values.</param>
        /// <returns>true if the P,Y,R values were read successfully; false otherwise.</returns>
        public bool InitFromString(string sourceString)
        {
            Pitch = Yaw = Roll = 0;

            // The initialization is only successful if the X, Y, and Z values can all be parsed from the string
            bool successful = 
                FParse.Value(sourceString, "P=", ref Pitch) &&
                FParse.Value(sourceString, "Y=", ref Yaw) &&
                FParse.Value(sourceString, "R=", ref Roll);
            DiagnosticCheckNaN();
            return successful;
        }

        /// <summary>
        /// Utility to check if there are any non-finite values (NaN or Inf) in this Rotator.
        /// </summary>
        /// <returns>true if there are any non-finite values in this Rotator, otherwise false.</returns>
        public bool ContainsNaN()
        {
            return (!FMath.IsFinite(Pitch) || !FMath.IsFinite(Yaw) || !FMath.IsFinite(Roll));
        }

        /// <summary>
        /// Clamps an angle to the range of [0, 360).
        /// </summary>
        /// <param name="angle">Angle The angle to clamp.</param>
        /// <returns>The clamped angle.</returns>
        public static float ClampAxis(float angle)
        {
            // returns Angle in the range (-360,360)
            angle = FMath.Fmod(angle, 360.0f);

            if (angle < 0.0f)
            {
                // shift to [0,360) range
                angle += 360.0f;
            }

            return angle;
        }

        /// <summary>
        /// Clamps an angle to the range of (-180, 180].
        /// </summary>
        /// <param name="angle">The Angle to clamp.</param>
        /// <returns>The clamped angle.</returns>
        public static float NormalizeAxis(float angle)
        {
            // returns Angle in the range [0,360)
            angle = ClampAxis(angle);

            if (angle > 180.0f)
            {
                // shift to (-180,180]
                angle -= 360.0f;
            }

            return angle;
        }

        /// <summary>
        /// Compresses a floating point angle into a byte.
        /// </summary>
        /// <param name="angle">The angle to compress.</param>
        /// <returns>The angle as a byte.</returns>
        public static byte CompressAxisToByte(float angle)
        {
            // map [0->360) to [0->256) and mask off any winding
            return (byte)(FMath.RoundToInt(angle * 256.0f / 360.0f) & 0xFF);
        }

        /// <summary>
        /// Decompress a word into a floating point angle.
        /// </summary>
        /// <param name="angle">The word angle.</param>
        /// <returns>The decompressed angle.</returns>
        public static float DecompressAxisFromByte(byte angle)
        {
            // map [0->256) to [0->360)
            return (angle * 360.0f / 256.0f);
        }

        /// <summary>
        /// Compress a floating point angle into a word.
        /// </summary>
        /// <param name="angle">The angle to compress.</param>
        /// <returns>The decompressed angle.</returns>
        public static ushort CompressAxisToShort(float angle)
        {
            // map [0->360) to [0->65536) and mask off any winding
            return (ushort)(FMath.RoundToInt(angle * 65536.0f / 360.0f) & 0xFFFF);
        }

        /// <summary>
        /// Decompress a short into a floating point angle.
        /// </summary>
        /// <param name="angle">The word angle.</param>
        /// <returns>The decompressed angle.</returns>
        public static float DecompressAxisFromShort(ushort angle)
        {
            // map [0->65536) to [0->360)
            return (angle * 360.0f / 65536.0f);
        }

        /// <summary>
        /// Convert a vector of floating-point Euler angles (in degrees) into a Rotator. Rotator now stored in degrees
        /// </summary>
        /// <param name="euler">Euler angle vector.</param>
        /// <returns>A rotator from a Euler angle.</returns>
        public static FRotator MakeFromEuler(FVector euler)
        {
            return new FRotator(euler.Y, euler.Z, euler.X);
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FRotator Lerp(FRotator a, FRotator b, float alpha)
        {
            return FMath.Lerp(a, b, alpha);
        }

        /// <summary>
        /// Similar to Lerp, but does not take the shortest path. Allows interpolation over more than 180 degrees.
        /// </summary>
        public static FRotator LerpRange(FRotator a, FRotator b, float alpha)
        {
            return FMath.LerpRange(a, b, alpha);
        }
    }
}
