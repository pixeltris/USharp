using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\UnrealMathUtility.h

    // TODO: Add inlining attribute to methods [MethodImpl(MethodImplOptions.AggressiveInlining)]

    /// <summary>
    /// Structure for all math helper functions, inherits from platform math to pick up platform-specific implementations
    /// Check GenericPlatformMath.h for additional math functions
    /// </summary>
    public static partial class FMath
    {
        public const string LogUnrealMath = "LogUnrealMath";

        public static readonly float LogToLog2 = 1.0f / Loge(2.0f);

        public const float PI = 3.1415926535897932f;
        public const float SmallNumber = 1.0e-8f;
        public const float KindaSmallNumber = 1.0e-4f;
        public const float BigNumber = 3.4e+38f;
        public const float EulersNumber = 2.71828182845904523536f;

        // Copied from float.h
        public const float MaxFloat = 3.402823466e+38f;// MAX_FLT 3.402823466e+38F (slightly different to float.MaxValue)

        // Aux constants.
        public const float InvPI = 0.31830988618f;
        public const float HalfPI = 1.57079632679f;

        // Magic numbers for numerical precision.
        public const float Delta = 0.00001f;

        /// <summary>
        /// Lengths of normalized vectors (These are half their maximum values
        /// to assure that dot products with normalized vectors don't overflow).
        /// </summary>
        public const float FLOAT_NORMAL_THRESH = 0.0001f;

        // Magic numbers for numerical precision.
        /// <summary>
        /// Thickness of plane for front/back/inside test
        /// </summary>
        public const float THRESH_POINT_ON_PLANE = 0.10f;
        /// <summary>
        /// Thickness of polygon side's side-plane for point-inside/outside/on side test
        /// </summary>
        public const float THRESH_POINT_ON_SIDE = 0.20f;
        /// <summary>
        /// Two points are same if within this distance
        /// </summary>
        public const float THRESH_POINTS_ARE_SAME = 0.00002f;
        /// <summary>
        /// Two points are near if within this distance and can be combined if imprecise math is ok
        /// </summary>
        public const float THRESH_POINTS_ARE_NEAR = 0.015f;
        /// <summary>
        /// Two normal points are same if within this distance
        /// </summary>
        public const float THRESH_NORMALS_ARE_SAME = 0.00002f;
        /// <summary>
        /// Two UV are same if within this threshold (1.0f/1024f)
        /// Making this too large results in incorrect CSG classification and disaster
        /// </summary>
        public const float THRESH_UVS_ARE_SAME = 0.0009765625f;
        /// <summary>
        /// Two vectors are near if within this distance and can be combined if imprecise math is ok
        /// Making this too large results in lighting problems due to inaccurate texture coordinates
        /// </summary>
        public const float THRESH_VECTORS_ARE_NEAR = 0.0004f;
        /// <summary>
        /// A plane splits a polygon in half
        /// </summary>
        public const float THRESH_SPLIT_POLY_WITH_PLANE = 0.25f;
        /// <summary>
        /// A plane exactly splits a polygon
        /// </summary>
        public const float THRESH_SPLIT_POLY_PRECISELY = 0.01f;
        /// <summary>
        /// Size of a unit normal that is considered "zero", squared
        /// </summary>
        public const float THRESH_ZERO_NORM_SQUARED = 0.0001f;
        /// <summary>
        /// Two unit vectors are parallel if abs(A dot B) is greater than or equal to this. This is roughly cosine(1.0 degrees).
        /// </summary>
        public const float THRESH_NORMALS_ARE_PARALLEL = 0.999845f;
        /// <summary>
        /// Two unit vectors are orthogonal (perpendicular) if abs(A dot B) is less than or equal this. This is roughly cosine(89.0 degrees).
        /// </summary>
        public const float THRESH_NORMALS_ARE_ORTHOGONAL = 0.017455f;
        /// <summary>
        /// Allowed error for a normalized vector (against squared magnitude)
        /// </summary>
        public const float THRESH_VECTOR_NORMALIZED = 0.01f;
        /// <summary>
        /// Allowed error for a normalized quaternion (against squared magnitude)
        /// </summary>
        public const float THRESH_QUAT_NORMALIZED = 0.01f;

        /// <summary>
        /// 32 bit values where BitFlag[x] == (1<<x)
        /// </summary>
        public static readonly uint[] BitFlag = new uint[32]
        {
            (1U << 0),  (1U << 1),  (1U << 2),  (1U << 3),
            (1U << 4),  (1U << 5),  (1U << 6),  (1U << 7),
            (1U << 8),  (1U << 9),  (1U << 10), (1U << 11),
            (1U << 12), (1U << 13), (1U << 14), (1U << 15),
            (1U << 16), (1U << 17), (1U << 18), (1U << 19),
            (1U << 20), (1U << 21), (1U << 22), (1U << 23),
            (1U << 24), (1U << 25), (1U << 26), (1U << 27),
            (1U << 28), (1U << 29), (1U << 30), (1U << 31),
        };

        public static void LogOrEnsureNanError(string format, params object[] args)
        {
            // TODO: Check GEnsureOnNANDiagnostic and log accordingly
            FMessage.Log(LogUnrealMath, ELogVerbosity.Error, string.Format(format, args));
        }

        /// <summary>
        /// Helper function for rand implementations. Returns a random number in [0..A)
        /// </summary>
        public static int RandHelper(int a)
        {
            // Note that on some platforms RAND_MAX is a large number so we cannot do ((rand()/(RAND_MAX+1)) * A)
            // or else we may include the upper bound results, which should be excluded.
            return a > 0 ? Min(TruncToInt(FRand() * a), a - 1) : 0;
        }

        /// <summary>
        /// Helper function for rand implementations. Returns a random number >= Min and <= Max
        /// </summary>
        public static int RandRange(int min, int max)
        {
            int range = (max - min) + 1;
            return min + RandHelper(range);
        }

        /// <summary>
        /// Util to generate a random number in a range. Overloaded to distinguish from int32 version, where passing a float is typically a mistake.
        /// </summary>
        public static float RandRange(float min, float max)
        {
            return FRandRange(min, max);
        }

        /// <summary>
        /// Util to generate a random number in a range.
        /// </summary>
        public static float FRandRange(float min, float max)
        {
            return min + (max - min) + FRand();
        }

        /// <summary>
        /// Util to generate a random boolean.
        /// </summary>
        public static bool RandBool()
        {
            return RandRange(0, 1) == 1;
        }

        /// <summary>
        /// Return a uniformly distributed random unit length vector = point on the unit sphere surface.
        /// </summary>
        /// <returns></returns>
        public static FVector VRand()
        {
            FVector result;
            float l;

            do
            {
                // Check random vectors in the unit sphere so result is statistically uniform.
                result.X = FRand() * 2.0f - 1.0f;
                result.Y = FRand() * 2.0f - 1.0f;
                result.Z = FRand() * 2.0f - 1.0f;
                l = result.SizeSquared();
            }
            while (l < 1.0f || l < KindaSmallNumber);

            return result * (1.0f / Sqrt(l));
        }

        /// <summary>
        /// Returns a random unit vector, uniformly distributed, within the specified cone
        /// ConeHalfAngleRad is the half-angle of cone, in radians.  Returns a normalized vector. 
        /// </summary>
        public static FVector VRandCone(FVector dir, float coneHalfAngleRad)
        {
            if (coneHalfAngleRad > 0.0f)
            {
                float randU = FMath.FRand();
                float randV = FMath.FRand();

                // Get spherical coords that have an even distribution over the unit sphere
                // Method described at http://mathworld.wolfram.com/SpherePointPicking.html	
                float theta = 2.0f * PI * randU;
                float phi = FMath.Acos((2.0f * randV) - 1.0f);

                // restrict phi to [0, ConeHalfAngleRad]
                // this gives an even distribution of points on the surface of the cone
                // centered at the origin, pointing upward (z), with the desired angle
                phi = FMath.Fmod(phi, coneHalfAngleRad);

                // get axes we need to rotate around
                FMatrix dirMat = FMatrix.CreateRotation(dir.Rotation());
                FVector dirZ = dirMat.GetScaledAxis(EAxis.X);
                FVector dirY = dirMat.GetScaledAxis(EAxis.Y);

                FVector result = dir.RotateAngleAxis(phi * 180.0f / PI, dirY);
                result = result.RotateAngleAxis(theta * 180.0f / PI, dirZ);

                // ensure it's a unit vector (might not have been passed in that way)
                result = result.GetSafeNormal();

                return result;
            }
            else
            {
                return dir.GetSafeNormal();
            }
        }

        /// <summary>
        /// This is a version of VRandCone that handles "squished" cones, i.e. with different angle limits in the Y and Z axes.
        /// Assumes world Y and Z, although this could be extended to handle arbitrary rotations.
        /// </summary>
        public static FVector VRandCone(FVector dir, float horizontalConeHalfAngleRad, float verticalConeHalfAngleRad)
        {
            if ((verticalConeHalfAngleRad > 0.0f) && (horizontalConeHalfAngleRad > 0.0f))
            {
                float randU = FMath.FRand();
                float randV = FMath.FRand();

                // Get spherical coords that have an even distribution over the unit sphere
                // Method described at http://mathworld.wolfram.com/SpherePointPicking.html	
                float theta = 2.0f * PI * randU;
                float phi = FMath.Acos((2.0f * randV) - 1.0f);

                // restrict phi to [0, ConeHalfAngleRad]
                // where ConeHalfAngleRad is now a function of Theta
                // (specifically, radius of an ellipse as a function of angle)
                // function is ellipse function (x/a)^2 + (y/b)^2 = 1, converted to polar coords
                float coneHalfAngleRad = FMath.Square(FMath.Cos(theta) / verticalConeHalfAngleRad) + FMath.Square(FMath.Sin(theta) / horizontalConeHalfAngleRad);
                coneHalfAngleRad = FMath.Sqrt(1.0f / coneHalfAngleRad);

                // clamp to make a cone instead of a sphere
                phi = FMath.Fmod(phi, coneHalfAngleRad);

                // get axes we need to rotate around
                FMatrix dirMat = FMatrix.CreateRotation(dir.Rotation());
                // note the axis translation, since we want the variation to be around X
                FVector dirZ = dirMat.GetScaledAxis(EAxis.X);
                FVector dirY = dirMat.GetScaledAxis(EAxis.Y);

                FVector result = dir.RotateAngleAxis(phi * 180.0f / PI, dirY);
                result = result.RotateAngleAxis(theta * 180.0f / PI, dirZ);

                // ensure it's a unit vector (might not have been passed in that way)
                result = result.GetSafeNormal();

                return result;
            }
            else
            {
                return dir.GetSafeNormal();
            }
        }

        /// <summary>
        /// Returns a random point, uniformly distributed, within the specified radius
        /// </summary>
        public static FVector2D RandPointInCircle(float circleRadius)
        {
            FVector2D point;
            float l;

            do
            {
                // Check random vectors in the unit circle so result is statistically uniform.
                point.X = FRand() * 2.0f - 1.0f;
                point.Y = FRand() * 2.0f - 1.0f;
                l = point.SizeSquared();
            }
            while (l > 1.0f);

            return point * circleRadius;
        }

        /// <summary>
        /// Returns a random point within the passed in bounding box
        /// </summary>
        public static FVector RandPointInBox(FBox box)
        {
            return new FVector(
                FRandRange(box.Min.X, box.Max.X),
                FRandRange(box.Min.Y, box.Max.Y),
                FRandRange(box.Min.Z, box.Max.Z));
        }

        /// <summary>
        /// Given a direction vector and a surface normal, returns the vector reflected across the surface normal.
        /// Produces a result like shining a laser at a mirror!
        /// </summary>
        /// <param name="direction">Direction vector the ray is coming from.</param>
        /// <param name="surfaceNormal">A normal of the surface the ray should be reflected on.</param>
        /// <returns>Reflected vector.</returns>
        public static FVector GetReflectionVector(FVector direction, FVector surfaceNormal)
        {
            return direction - 2 * (direction | surfaceNormal.GetSafeNormal()) * surfaceNormal.GetSafeNormal();
        }

        /// <summary>
        /// Checks if two floating point numbers are nearly equal.
        /// </summary>
        /// <param name="a">First number to compare</param>
        /// <param name="b">Second number to compare</param>
        /// <param name="errorTolerance">Maximum allowed difference for considering them as 'nearly equal'</param>
        /// <returns>true if A and B are nearly equal</returns>
        public static bool IsNearlyEqual(float a, float b, float errorTolerance = SmallNumber)
        {
            return Abs(a - b) <= errorTolerance;
        }

        /// <summary>
        /// Checks if two floating point numbers are nearly equal.
        /// </summary>
        /// <param name="a">First number to compare</param>
        /// <param name="b">Second number to compare</param>
        /// <param name="errorTolerance">Maximum allowed difference for considering them as 'nearly equal'</param>
        /// <returns>true if A and B are nearly equal</returns>
        public static bool IsNearlyEqual(double a, double b, double errorTolerance = SmallNumber)
        {
            return Abs(a - b) <= errorTolerance;
        }

        /// <summary>
        /// Checks if a floating point number is nearly zero.
        /// </summary>
        /// <param name="value">Number to compare</param>
        /// <param name="errorTolerance">Maximum allowed difference for considering Value as 'nearly zero'</param>
        /// <returns>true if Value is nearly zero</returns>
        public static bool IsNearlyZero(float value, float errorTolerance = SmallNumber)
        {
            return Abs(value) <= errorTolerance;
        }

        /// <summary>
        /// Checks if a floating point number is nearly zero.
        /// </summary>
        /// <param name="value">Number to compare</param>
        /// <param name="errorTolerance">Maximum allowed difference for considering Value as 'nearly zero'</param>
        /// <returns>true if Value is nearly zero</returns>
        public static bool IsNearlyZero(double value, double errorTolerance = SmallNumber)
        {
            return Abs(value) <= errorTolerance;
        }

        /// <summary>
        /// Checks whether a number is a power of two.
        /// </summary>
        /// <param name="value">Number to check</param>
        /// <returns>true if Value is a power of two</returns>
        public static bool IsPowerOfTwo(sbyte value)
        {
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Snaps a value to the nearest grid multiple
        /// </summary>
        public static float GridSnap(float location, float grid)
        {
            if (grid == 0.0f)
            {
                return location;
            }
            return FloorToFloat((location + 0.5f * grid) / grid) * grid;
        }

        /// <summary>
        /// Snaps a value to the nearest grid multiple
        /// </summary>
        public static double GridSnap(double location, double grid)
        {
            if (grid == 0.0)
            {
                return location;
            }
            return FloorToDouble((location + 0.5 * grid) / grid) * grid;
        }

        ///// <summary>
        ///// Computes the base 2 logarithm of the specified value
        ///// </summary>
        ///// <param name="value">the value to perform the log on</param>
        ///// <returns>the base 2 log of the value</returns>
        //public static float Log2(float value)
        //{
        //    // Cached value for fast conversions
        //    //const float LogToLog2 = 1.0f / Loge(2.0f);
        //    // Do the platform specific log and convert using the cached value
        //    return Loge(value) * LogToLog2;
        //}

        /// <summary>
        /// Computes the sine and cosine of a scalar float.
        /// </summary>
        /// <param name="scalarSin">Sin result</param>
        /// <param name="scalarCos">Cos result</param>
        /// <param name="value">input angles</param>
        public static void SinCos(out float scalarSin, out float scalarCos, float value)
        {
            // Map Value to y in [-pi,pi], x = 2*pi*quotient + remainder.
            float quotient = (InvPI * 0.5f) * value;
            if (value >= 0.0f)
            {
                quotient = (float)((int)(quotient + 0.5f));
            }
            else
            {
                quotient = (float)((int)(quotient - 0.5f));
            }
            float y = value - (2.0f * PI) * quotient;

            // Map y to [-pi/2,pi/2] with sin(y) = sin(Value).
            float sign;
            if (y > HalfPI)
            {
                y = PI - y;
                sign = -1.0f;
            }
            else if (y < -HalfPI)
            {
                y = -PI - y;
                sign = -1.0f;
            }
            else
            {
                sign = +1.0f;
            }

            float y2 = y * y;

            // 11-degree minimax approximation
            scalarSin = (((((-2.3889859e-08f * y2 + 2.7525562e-06f) * y2 - 0.00019840874f) * y2 + 0.0083333310f) * y2 - 0.16666667f) * y2 + 1.0f) * y;

            // 10-degree minimax approximation
            float p = ((((-2.6051615e-07f * y2 + 2.4760495e-05f) * y2 - 0.0013888378f) * y2 + 0.041666638f) * y2 - 0.5f) * y2 + 1.0f;
            scalarCos = sign * p;
        }

        /// <summary>
        /// Computes the ASin of a scalar float.
        /// </summary>
        /// <param name="value">input angle</param>
        /// <returns>ASin of Value</returns>
        public static float FastAsin(float value)
        {
            // Note:  We use FASTASIN_HALF_PI instead of HALF_PI inside of FastASin(), since it was the value that accompanied the minimax coefficients below.
            // It is important to use exactly the same value in all places inside this function to ensure that FastASin(0.0f) == 0.0f.
            // For comparison:
            //		HALF_PI				== 1.57079632679f == 0x3fC90FDB
            //		FASTASIN_HALF_PI	== 1.5707963050f  == 0x3fC90FDA
            const float fastAsinHalfPI = 1.5707963050f;

            // Clamp input to [-1,1].
            bool nonnegative = (value >= 0.0f);
            float x = FMath.Abs(value);
            float omx = 1.0f - x;
            if (omx < 0.0f)
            {
                omx = 0.0f;
            }
            float root = FMath.Sqrt(omx);
            // 7-degree minimax approximation
            float result = ((((((-0.0012624911f * x + 0.0066700901f) * x - 0.0170881256f) * x + 0.0308918810f) * x - 0.0501743046f) * x + 0.0889789874f) * x - 0.2145988016f) * x + fastAsinHalfPI;
            result *= root;  // acos(|x|)
            // acos(x) = pi - acos(-x) when x < 0, asin(x) = pi/2 - acos(x)
            return (nonnegative ? fastAsinHalfPI - result : result - fastAsinHalfPI);
        }

        /// <summary>
        /// Clamps an arbitrary angle to be between the given angles.  Will clamp to nearest boundary.
        /// </summary>
        /// <param name="angleDegrees">"from" angle that defines the beginning of the range of valid angles (sweeping clockwise)</param>
        /// <param name="minAngleDegrees">"to" angle that defines the end of the range of valid angles</param>
        /// <param name="maxAngleDegrees"></param>
        /// <returns>Returns clamped angle in the range -180..180.</returns>
        public static float ClampAngle(float angleDegrees, float minAngleDegrees, float maxAngleDegrees)
        {
            float maxDelta = FRotator.ClampAxis(maxAngleDegrees - minAngleDegrees) * 0.5f;// 0..180
            float rangeCenter = FRotator.ClampAxis(minAngleDegrees + maxDelta);// 0..360
            float deltaFromCenter = FRotator.NormalizeAxis(angleDegrees - rangeCenter);// -180..180

            // maybe clamp to nearest edge
            if (deltaFromCenter > maxDelta)
            {
                return FRotator.NormalizeAxis(rangeCenter + maxDelta);
            }
            else if (deltaFromCenter < -maxDelta)
            {
                return FRotator.NormalizeAxis(rangeCenter - maxDelta);
            }

            // already in range, just return it
            return FRotator.NormalizeAxis(angleDegrees);
        }

        /// <summary>
        /// Find the smallest angle between two headings (in degrees)
        /// </summary>
        public static float FindDeltaAngleDegrees(float a1, float a2)
        {
            // Find the difference
            float delta = a2 - a1;

            // If change is larger than 180
            if (delta > 180.0f)
            {
                // Flip to negative equivalent
                delta = delta - 360.0f;
            }
            else if (delta < -180.0f)
            {
                // Otherwise, if change is smaller than -180
                // Flip to positive equivalent
                delta = delta + 360.0f;
            }

            // Return delta in [-180,180] range
            return delta;
        }

        /// <summary>
        /// Find the smallest angle between two headings (in radians)
        /// </summary>
        public static float FindDeltaAngleRadians(float a1, float a2)
        {
            // Find the difference
            float delta = a2 - a1;

            // If change is larger than PI
            if (delta > PI)
            {
                // Flip to negative equivalent
                delta = delta - (PI * 2.0f);
            }
            else if (delta < -PI)
            {
                // Otherwise, if change is smaller than -PI
                // Flip to positive equivalent
                delta = delta + (PI * 2.0f);
            }

            // Return delta in [-PI,PI] range
            return delta;
        }

        /// <summary>
        /// Given a heading which may be outside the +/- PI range, 'unwind' it back into that range.
        /// </summary>
        public static float UnwindRadians(float a)
        {
            while (a > PI)
            {
                a -= ((float)PI * 2.0f);
            }

            while (a < -PI)
            {
                a += ((float)PI * 2.0f);
            }

            return a;
        }

        /// <summary>
        /// Utility to ensure angle is between +/- 180 degrees by unwinding.
        /// </summary>
        public static float UnwindDegrees(float a)
        {
            while (a > 180.0f)
            {
                a -= 360.0f;
            }

            while (a < -180.0f)
            {
                a += 360.0f;
            }

            return a;
        }

        /// <summary>
        /// Given two angles in degrees, 'wind' the rotation in Angle1 so that it avoids >180 degree flips.
        /// Good for winding rotations previously expressed as quaternions into a euler-angle representation.
        /// </summary>
        /// <param name="angle0">The first angle that we wind relative to.</param>
        /// <param name="angle1">The second angle that we may wind relative to the first.</param>
        public static void WindRelativeAnglesDegrees(float angle0, ref float angle1)
        {
            float diff = angle0 - angle1;
            float absDiff = Abs(diff);
            if (absDiff > 180.0f)
            {
                angle1 += 360.0f * Sign(diff) * FloorToFloat((absDiff / 360.0f) + 0.5f);
            }
        }

        /// <summary>
        /// Returns a new rotation component value
        /// </summary>
        /// <param name="current">is the current rotation value</param>
        /// <param name="desired">is the desired rotation value</param>
        /// <param name="deltaRate">is the rotation amount to apply</param>
        /// <returns>a new rotation component value</returns>
        public static float FixedTurn(float current, float desired, float deltaRate)
        {
            if (deltaRate == 0.0f)
            {
                return FRotator.ClampAxis(current);
            }

            if (deltaRate >= 360.0f)
            {
                return FRotator.ClampAxis(desired);
            }

            float result = FRotator.ClampAxis(current);
            current = result;
            desired = FRotator.ClampAxis(desired);

            if (current > desired)
            {
                if (current - desired < 180.0f)
                {
                    result -= FMath.Min((current - desired), FMath.Abs(deltaRate));
                }
                else
                {
                    result += FMath.Min((desired + 360.0f - current), FMath.Abs(deltaRate));
                }
            }
            else
            {
                if (desired - current < 180.0f)
                {
                    result += FMath.Min((desired - current), FMath.Abs(deltaRate));
                }
                else
                {
                    result -= FMath.Min((current + 360.0f - desired), FMath.Abs(deltaRate));
                }
            }
            return FRotator.ClampAxis(result);
        }

        /// <summary>
        /// Converts given Cartesian coordinate pair to Polar coordinate system.
        /// </summary>
        public static void CartesianToPolar(float x, float y, out float rad, out float ang)
        {
            rad = Sqrt(Square(x) + Square(y));
            ang = Atan2(y, x);
        }

        /// <summary>
        /// Converts given Cartesian coordinate pair to Polar coordinate system.
        /// </summary>
        public static void CartesianToPolar(FVector2D cart, out FVector2D polar)
        {
            polar = new FVector2D(
                Sqrt(Square(cart.X) + Square(cart.Y)),
                Atan2(cart.Y, cart.X));
        }

        /// <summary>
        /// Converts given Polar coordinate pair to Cartesian coordinate system.
        /// </summary>
        public static void PolarToCartesian(float rad, float ang, out float x, out float y)
        {
            x = rad * Cos(ang);
            y = rad * Sin(ang);
        }

        /// <summary>
        /// Converts given Polar coordinate pair to Cartesian coordinate system.
        /// </summary>
        public static void PolarToCartesian(FVector2D polar, out FVector2D cart)
        {
            cart = new FVector2D(
                polar.X * Cos(polar.Y),
                polar.X * Sin(polar.Y));
        }

        /// <summary>
        /// Calculates the dotted distance of vector 'Direction' to coordinate system O(AxisX,AxisY,AxisZ).
        /// 
        /// Orientation: (consider 'O' the first person view of the player, and 'Direction' a vector pointing to an enemy)
        /// - positive azimuth means enemy is on the right of crosshair. (negative means left).
        /// - positive elevation means enemy is on top of crosshair, negative means below.
        /// 
        /// @Note: 'Azimuth' (.X) sign is changed to represent left/right and not front/behind. front/behind is the funtion's return value.
        /// </summary>
        /// <param name="dotDist">
        /// .X = 'Direction' dot AxisX relative to plane (AxisX,AxisZ). (== Cos(Azimuth))
        /// .Y = 'Direction' dot AxisX relative to plane (AxisX,AxisY). (== Sin(Elevation))</param>
        /// <param name="direction">direction of target.</param>
        /// <param name="axisX">X component of reference system.</param>
        /// <param name="axisY">Y component of reference system.</param>
        /// <param name="axisZ">Z component of reference system.</param>
        /// <returns>true if 'Direction' is facing AxisX (Direction dot AxisX >= 0.f)</returns>
        public static bool GetDotDistance(out FVector2D dotDist, FVector direction, FVector axisX, FVector axisY, FVector axisZ)
        {
            return GetDotDistance(out dotDist, ref direction, ref axisX, ref axisY, ref axisZ);
        }

        /// <summary>
        /// Calculates the dotted distance of vector 'Direction' to coordinate system O(AxisX,AxisY,AxisZ).
        /// 
        /// Orientation: (consider 'O' the first person view of the player, and 'Direction' a vector pointing to an enemy)
        /// - positive azimuth means enemy is on the right of crosshair. (negative means left).
        /// - positive elevation means enemy is on top of crosshair, negative means below.
        /// 
        /// @Note: 'Azimuth' (.X) sign is changed to represent left/right and not front/behind. front/behind is the funtion's return value.
        /// </summary>
        /// <param name="dotDist">
        /// .X = 'Direction' dot AxisX relative to plane (AxisX,AxisZ). (== Cos(Azimuth))
        /// .Y = 'Direction' dot AxisX relative to plane (AxisX,AxisY). (== Sin(Elevation))</param>
        /// <param name="direction">direction of target.</param>
        /// <param name="axisX">X component of reference system.</param>
        /// <param name="axisY">Y component of reference system.</param>
        /// <param name="axisZ">Z component of reference system.</param>
        /// <returns>true if 'Direction' is facing AxisX (Direction dot AxisX >= 0.f)</returns>
        public static bool GetDotDistance(out FVector2D dotDist, ref FVector direction, ref FVector axisX, ref FVector axisY, ref FVector axisZ)
        {
            FVector normalDir = direction.GetSafeNormal();

            // Find projected point (on AxisX and AxisY, remove AxisZ component)
            FVector noZProjDir = (normalDir - (normalDir | axisZ) * axisZ).GetSafeNormal();

            // Figure out if projection is on right or left.
            float azimuthSign = ((noZProjDir | axisY) < 0.0f) ? -1.0f : 1.0f;

            float dirDotX = noZProjDir | axisX;
            dotDist = new FVector2D(
                azimuthSign * FMath.Abs(dirDotX),
                normalDir | axisZ);

            return (dirDotX >= 0.0f);
        }

        /// <summary>
        /// Returns Azimuth and Elevation of vector 'Direction' in coordinate system O(AxisX,AxisY,AxisZ).
        /// 
        /// Orientation: (consider 'O' the first person view of the player, and 'Direction' a vector pointing to an enemy)
        /// - positive azimuth means enemy is on the right of crosshair. (negative means left).
        /// - positive elevation means enemy is on top of crosshair, negative means below.
        /// </summary>
        /// <param name="direction">Direction of target.</param>
        /// <param name="axisX">X component of reference system.</param>
        /// <param name="axisY">Y component of reference system.</param>
        /// <param name="axisZ">Z component of reference system.</param>
        /// <returns>FVector2D { X = Azimuth angle (in radians) (-PI, +PI), Y = Elevation angle (in radians) (-PI/2, +PI/2) }</returns>
        public static FVector2D GetAzimuthAndElevation(FVector direction, FVector axisX, FVector axisY, FVector axisZ)
        {
            return GetAzimuthAndElevation(ref direction, ref axisX, ref axisY, ref axisZ);
        }

        /// <summary>
        /// Returns Azimuth and Elevation of vector 'Direction' in coordinate system O(AxisX,AxisY,AxisZ).
        /// 
        /// Orientation: (consider 'O' the first person view of the player, and 'Direction' a vector pointing to an enemy)
        /// - positive azimuth means enemy is on the right of crosshair. (negative means left).
        /// - positive elevation means enemy is on top of crosshair, negative means below.
        /// </summary>
        /// <param name="direction">Direction of target.</param>
        /// <param name="axisX">X component of reference system.</param>
        /// <param name="axisY">Y component of reference system.</param>
        /// <param name="axisZ">Z component of reference system.</param>
        /// <returns>FVector2D { X = Azimuth angle (in radians) (-PI, +PI), Y = Elevation angle (in radians) (-PI/2, +PI/2) }</returns>
        public static FVector2D GetAzimuthAndElevation(ref FVector direction, ref FVector axisX, ref FVector axisY, ref FVector axisZ)
        {
            FVector normalDir = direction.GetSafeNormal();
            // Find projected point (on AxisX and AxisY, remove AxisZ component)
            FVector noZProjDir = (normalDir - (normalDir | axisZ) * axisZ).GetSafeNormal();
            // Figure out if projection is on right or left.
            float azimuthSign = ((noZProjDir | axisY) < 0.0f) ? -1.0f : 1.0f;
            float elevationSin = (normalDir | axisZ);
            float azimuthCos = (noZProjDir | axisX);

            // Convert to Angles in Radian.
            return new FVector2D(FMath.Acos(azimuthCos) * azimuthSign, FMath.Asin(elevationSin));
        }

        private static float TruncateToHalfIfClose(float f)
        {
            float valueToFudgeIntegralPart = 0.0f;
            float valueToFudgeFractionalPart = FMath.Modf(f, out valueToFudgeIntegralPart);
            if (f < 0.0f)
            {
                return valueToFudgeIntegralPart + ((FMath.IsNearlyEqual(valueToFudgeFractionalPart, -0.5f)) ? -0.5f : valueToFudgeFractionalPart);
            }
            else
            {
                return valueToFudgeIntegralPart + ((FMath.IsNearlyEqual(valueToFudgeFractionalPart, 0.5f)) ? 0.5f : valueToFudgeFractionalPart);
            }
        }

        private static double TruncateToHalfIfClose(double f)
        {
            double valueToFudgeIntegralPart = 0.0;
            double valueToFudgeFractionalPart = FMath.Modf(f, out valueToFudgeIntegralPart);
            if (f < 0.0)
            {
                return valueToFudgeIntegralPart + ((FMath.IsNearlyEqual(valueToFudgeFractionalPart, -0.5)) ? -0.5 : valueToFudgeFractionalPart);
            }
            else
            {
                return valueToFudgeIntegralPart + ((FMath.IsNearlyEqual(valueToFudgeFractionalPart, 0.5)) ? 0.5 : valueToFudgeFractionalPart);
            }
        }

        /// <summary>
        /// Converts a floating point number to the nearest integer, equidistant ties go to the value which is closest to an even value: 1.5 becomes 2, 0.5 becomes 0
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static float RoundHalfToEven(float f)
        {
            f = TruncateToHalfIfClose(f);

            bool isNegative = f < 0.0f;
            bool valueIsEven = (uint)(FloorToFloat(((isNegative) ? -f : f))) % 2 == 0;
            if (valueIsEven)
            {
                // Round towards value (eg, value is -2.5 or 2.5, and should become -2 or 2)
                return (isNegative) ? FloorToFloat(f + 0.5f) : CeilToFloat(f - 0.5f);
            }
            else
            {
                // Round away from value (eg, value is -3.5 or 3.5, and should become -4 or 4)
                return (isNegative) ? CeilToFloat(f - 0.5f) : FloorToFloat(f + 0.5f);
            }
        }

        /// <summary>
        /// Converts a floating point number to the nearest integer, equidistant ties go to the value which is closest to an even value: 1.5 becomes 2, 0.5 becomes 0
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static double RoundHalfToEven(double f)
        {
            f = TruncateToHalfIfClose(f);

            bool negative = f < 0.0;
            bool valueIsEven = (ulong)(FMath.FloorToDouble(((negative) ? -f : f))) % 2 == 0;
            if (valueIsEven)
            {
                // Round towards value (eg, value is -2.5 or 2.5, and should become -2 or 2)
                return (negative) ? FloorToDouble(f + 0.5) : CeilToDouble(f - 0.5);
            }
            else
            {
                // Round away from value (eg, value is -3.5 or 3.5, and should become -4 or 4)
                return (negative) ? CeilToDouble(f - 0.5) : FloorToDouble(f + 0.5);
            }
        }

        /// <summary>
        /// Converts a floating point number to the nearest integer, equidistant ties go to the value which is further from zero: -0.5 becomes -1.0, 0.5 becomes 1.0
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static float RoundHalfFromZero(float f)
        {
            f = TruncateToHalfIfClose(f);
            return (f < 0.0f) ? CeilToFloat(f - 0.5f) : FloorToFloat(f + 0.5f);
        }

        /// <summary>
        /// Converts a floating point number to the nearest integer, equidistant ties go to the value which is further from zero: -0.5 becomes -1.0, 0.5 becomes 1.0
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static double RoundHalfFromZero(double f)
        {
            f = TruncateToHalfIfClose(f);
            return (f < 0.0) ? CeilToDouble(f - 0.5) : FloorToDouble(f + 0.5);
        }

        /// <summary>
        /// Converts a floating point number to the nearest integer, equidistant ties go to the value which is closer to zero: -0.5 becomes 0, 0.5 becomes 0
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static float RoundHalfToZero(float f)
        {
            f = TruncateToHalfIfClose(f);
            return (f < 0.0f) ? FloorToFloat(f + 0.5f) : CeilToFloat(f - 0.5f);
        }

        /// <summary>
        /// Converts a floating point number to the nearest integer, equidistant ties go to the value which is closer to zero: -0.5 becomes 0, 0.5 becomes 0
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static double RoundHalfToZero(double f)
        {
            f = TruncateToHalfIfClose(f);
            return (f < 0.0) ? FloorToDouble(f + 0.5) : CeilToDouble(f - 0.5);
        }

        /// <summary>
        /// Converts a floating point number to an integer which is further from zero, "larger" in absolute value: 0.1 becomes 1, -0.1 becomes -1
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static float RoundFromZero(float f)
        {
            return (f < 0.0f) ? FloorToFloat(f) : CeilToFloat(f);
        }

        /// <summary>
        /// Converts a floating point number to an integer which is further from zero, "larger" in absolute value: 0.1 becomes 1, -0.1 becomes -1
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static double RoundFromZero(double f)
        {
            return (f < 0.0) ? FloorToDouble(f) : CeilToDouble(f);
        }

        /// <summary>
        /// Converts a floating point number to an integer which is closer to zero, "smaller" in absolute value: 0.1 becomes 0, -0.1 becomes 0
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static float RoundToZero(float f)
        {
            return (f < 0.0f) ? CeilToFloat(f) : FloorToFloat(f);
        }

        /// <summary>
        /// Converts a floating point number to an integer which is closer to zero, "smaller" in absolute value: 0.1 becomes 0, -0.1 becomes 0
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static double RoundToZero(double f)
        {
            return (f < 0.0) ? CeilToDouble(f) : FloorToDouble(f);
        }

        /// <summary>
        /// Converts a floating point number to an integer which is more negative: 0.1 becomes 0, -0.1 becomes -1
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static float RoundToNegativeInfinity(float f)
        {
            return FloorToFloat(f);
        }

        /// <summary>
        /// Converts a floating point number to an integer which is more negative: 0.1 becomes 0, -0.1 becomes -1
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static double RoundToNegativeInfinity(double f)
        {
            return FloorToDouble(f);
        }

        /// <summary>
        /// Converts a floating point number to an integer which is more positive: 0.1 becomes 1, -0.1 becomes 0
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static float RoundToPositiveInfinity(float f)
        {
            return CeilToFloat(f);
        }

        /// <summary>
        /// Converts a floating point number to an integer which is more positive: 0.1 becomes 1, -0.1 becomes 0
        /// </summary>
        /// <param name="f">Floating point value to convert</param>
        /// <returns>The rounded integer</returns>
        public static double RoundToPositiveInfinity(double f)
        {
            return CeilToDouble(f);
        }

        // Formatting functions

        /// <summary>
        /// Formats an integer value into a human readable string (i.e. 12345 becomes "12,345")
        /// </summary>
        /// <param name="val">The value to use</param>
        /// <returns>The human readable string</returns>
        public static string FormatIntToHumanReadable(int val)
        {
            return val.ToString("N0", CultureInfo.InvariantCulture);
        }

        // Utilities

        /// <summary>
        /// Tests a memory region to see that it's working properly.
        /// </summary>
        /// <param name="baseAddress">Starting address</param>
        /// <param name="numBytes">Number of bytes to test (will be rounded down to a multiple of 4)</param>
        /// <returns>true if the memory region passed the test</returns>
        public static unsafe bool MemoryTest(IntPtr baseAddress, uint numBytes)
        {
            return Native_FMath.MemoryTest(baseAddress, numBytes);
        }

        /// <summary>
        /// Evaluates a numerical equation.
        /// 
        /// Operators and precedence: 1:+- 2:/% 3:* 4:^ 5:&|
        /// Unary: -
        /// Types: Numbers (0-9.), Hex ($0-$f)
        /// Grouping: ( )
        /// </summary>
        /// <param name="str">String containing the equation.</param>
        /// <param name="value">Resulting value.</param>
        /// <returns>1 if successful, 0 if equation fails.</returns>
        public static bool Eval(string str, out float value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            {
                return Native_FMath.Eval(ref strUnsafe.Array, out value);
            }
        }

        /// <summary>
        /// Computes the barycentric coordinates for a given point in a triangle - simpler version
        /// </summary>
        /// <param name="point">point to convert to barycentric coordinates (in plane of ABC)</param>
        /// <param name="a">three non-colinear points defining a triangle in CCW</param>
        /// <param name="b">three non-colinear points defining a triangle in CCW</param>
        /// <param name="c">three non-colinear points defining a triangle in CCW</param>
        /// <returns>Vector containing the three weights a,b,c such that Point = a*A + b*B + c*C
        ///                                                           or Point = A + b*(B-A) + c*(C-A) = (1-b-c)*A + b*B + c*C
        /// </returns>
        public static FVector GetBaryCentric2D(FVector point, FVector a, FVector b, FVector c)
        {
            float _a = ((b.Y - c.Y) * (point.X - c.X) + (c.X - b.X) * (point.Y - c.Y)) / ((b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y));
            float _b = ((c.Y - a.Y) * (point.X - c.X) + (a.X - c.X) * (point.Y - c.Y)) / ((b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y));
            return new FVector(_a, _b, 1.0f - _a - _b);
        }

        /// <summary>
        /// Computes the barycentric coordinates for a given point in a triangle
        /// </summary>
        /// <param name="point">point to convert to barycentric coordinates (in plane of ABC)</param>
        /// <param name="a">three non-collinear points defining a triangle in CCW</param>
        /// <param name="b">three non-collinear points defining a triangle in CCW</param>
        /// <param name="c">three non-collinear points defining a triangle in CCW</param>
        /// <returns>Vector containing the three weights a,b,c such that Point = a*A + b*B + c*C
        ///                                                           or Point = A + b*(B-A) + c*(C-A) = (1-b-c)*A + b*B + c*C
        /// </returns>
        public static FVector ComputeBaryCentric2D(FVector point, FVector a, FVector b, FVector c)
        {
            // Compute the normal of the triangle
            FVector triNorm = (b - a) ^ (c - a);

            // Check the size of the triangle is reasonable (TriNorm.Size() will be twice the triangle area)
            if (triNorm.SizeSquared() <= SmallNumber)
            {
                FMessage.Log(LogUnrealMath, ELogVerbosity.Warning, "Small triangle detected in FMath::ComputeBaryCentric2D(), can't compute valid barycentric coordinate.");
                return new FVector(0.0f, 0.0f, 0.0f);
            }

            FVector n = triNorm.GetSafeNormal();

            // Compute twice area of triangle ABC
            float areaABCInv = 1.0f / (n | triNorm);

            // Compute a contribution
            float areaPBC = n | ((b - point) ^ (c - point));
            float _a = areaPBC * areaABCInv;

            // Compute b contribution
            float areaPCA = n | ((c - point) ^ (a - point));
            float _b = areaPCA * areaABCInv;

            // Compute c contribution
            return new FVector(_a, _b, 1.0f - _a - _b);
        }

        /// <summary>
        /// Computes the barycentric coordinates for a given point on a tetrahedron (3D)
        /// </summary>
        /// <param name="point">point to convert to barycentric coordinates</param>
        /// <param name="a">four points defining a tetrahedron</param>
        /// <param name="b">four points defining a tetrahedron</param>
        /// <param name="c">four points defining a tetrahedron</param>
        /// <param name="d">four points defining a tetrahedron</param>
        /// <returns>Vector containing the four weights a,b,c,d such that Point = a*A + b*B + c*C + d*D</returns>
        public static FVector4 ComputeBaryCentric3D(FVector point, FVector a, FVector b, FVector c, FVector d)
        {
            //http://www.devmaster.net/wiki/Barycentric_coordinates
            //Pick A as our origin and
            //Setup three basis vectors AB, AC, AD
            FVector b1 = (b - a);
            FVector b2 = (c - a);
            FVector b3 = (d - a);

            //check co-planarity of A,B,C,D
            Debug.Assert(Abs(b1 | (b2 ^ b3)) > SmallNumber, "Coplanar points in FMath::ComputeBaryCentric3D()");

            //Transform Point into this new space
            FVector v = (point - a);

            //Create a matrix of linearly independent vectors
            FMatrix solvMat = new FMatrix(b1, b2, b3, FVector.ZeroVector);

            //The point V can be expressed as Ax=v where x is the vector containing the weights {w1...wn}
            //Solve for x by multiplying both sides by AInv   (AInv * A)x = AInv * v ==> x = AInv * v
            FMatrix invSolvMat = solvMat.Inverse();
            FPlane baryCoords = (FPlane)invSolvMat.TransformVector(v);

            //Reorder the weights to be a, b, c, d
            return new FVector4(1.0f - baryCoords.X - baryCoords.Y - baryCoords.Z, baryCoords.X, baryCoords.Y, baryCoords.Z);
        }

        /// <summary>
        /// Returns a smooth Hermite interpolation between 0 and 1 for the value X (where X ranges between A and B)
        /// Clamped to 0 for X <= A and 1 for X >= B.
        /// </summary>
        /// <param name="a">Minimum value of X</param>
        /// <param name="b">Maximum value of X</param>
        /// <param name="x">Parameter</param>
        /// <returns>Smoothed value between 0 and 1</returns>
        public static float SmoothStep(float a, float b, float x)
        {
            if (x < a)
            {
                return 0.0f;
            }
            else if (x >= b)
            {
                return 1.0f;
            }
            float interpFraction = (x - a) / (b - a);
            return interpFraction * interpFraction * (3.0f - 2.0f * interpFraction);
        }

        /// <summary>
        /// Get a bit in memory created from bitflags (uint32 Value:1), used for EngineShowFlags,
        /// TestBitFieldFunctions() tests the implementation
        /// </summary>
        public static bool ExtractBoolFromBitfield(byte[] bitField, int index)
        {
            int byteIndex = index / 8;
            byte byteMask = (byte)(1 << (index & 0x7));
            return (bitField[byteIndex] & byteMask) != 0;
        }

        /// <summary>
        /// Set a bit in memory created from bitflags (uint32 Value:1), used for EngineShowFlags,
        /// TestBitFieldFunctions() tests the implementation
        /// </summary>
        public static void SetBoolInBitField(byte[] bitField, int index, bool set)
        {
            int byteIndex = index / 8;
            byte byteMask = (byte)(1 << (index & 0x7));

            if (set)
            {
                bitField[byteIndex] |= byteMask;
            }
            else
            {
                bitField[byteIndex] &= (byte)~byteMask;
            }
        }

        /// <summary>
        /// Handy to apply scaling in the editor
        /// </summary>
        public static void ApplyScaleToFloat(ref float dst, FVector deltaScale, float magnitude = 1.0f)
        {
            float multiplier = (deltaScale.X > 0.0f || deltaScale.Y > 0.0f || deltaScale.Z > 0.0f) ? magnitude : -magnitude;
            dst += multiplier * deltaScale.Size();
            dst = FMath.Max(0.0f, dst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">assumed to be in this range: 0..1</param>
        /// <returns>0..255</returns>
        public static byte Quantize8UnsignedByte(float x)
        {
            // 0..1 -> 0..255
            int ret = (int)(x * 255.999f);

            Debug.Assert(ret >= 0);
            Debug.Assert(ret <= 255);

            return (byte)ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">assumed to be in this range: -1..1</param>
        /// <returns>0..255</returns>
        public static byte Quantize8SignedByte(float x)
        {
            // -1..1 -> 0..1
            float y = x * 0.5f + 0.5f;

            return Quantize8UnsignedByte(y);
        }

        /// <summary>
        /// Use the Euclidean method to find the GCD
        /// </summary>
        public static int GreatestCommonDivisor(int a, int b)
        {
            while (b != 0)
            {
                int t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        /// <summary>
        /// LCM = a/gcd * b
        /// a and b are the number we want to find the lcm
        /// </summary>
        public static int LeastCommonMultiplier(int a, int b)
        {
            int currentGcd = GreatestCommonDivisor(a, b);
            return currentGcd == 0 ? 0 : (a / currentGcd) * b;
        }
    }
}
