using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\UnrealMathUtility.h
    // Interpolation Functions

    // TODO: Add overloads for the functions which are templated in C++
    // 
    // Lerp, LerpStable, BiLerp, CubicInterp, CubicInterpDerivative, CubicInterpSecondDerivative, 
    // 
    // InterpEaseIn, InterpEaseOut, InterpEaseInOut, InterpStep, InterpSinIn, InterpSinOut, InterpSinInOut, InterpExpoIn, InterpExpoOut, InterpExpoInOut,
    // InterpCircularIn, InterpCircularOut, InterpCircularInOut
    // 
    // CubicCRSplineInterp, CubicCRSplineInterpSafe

    public static partial class FMath
    {
        /// <summary>
        /// Calculates the percentage along a line from MinValue to MaxValue that Value is.
        /// </summary>
        public static float GetRangePct(float minValue, float maxValue, float value)
        {
            // Avoid Divide by Zero.
            // But also if our range is a point, output whether Value is before or after.
            float divisor = maxValue - minValue;
            if (FMath.IsNearlyZero(divisor))
            {
                return (value >= maxValue) ? 1.0f : 0.0f;
            }
            return (value - minValue) / divisor;
        }

        /// <summary>
        /// Calculates the percentage along a line from MinValue to MaxValue that Value is.
        /// </summary>
        /// <returns></returns>
        public static float GetRangePct(FVector2D range, float value)
        {
            return GetRangePct(range.X, range.Y, value);
        }

        /// <summary>
        /// Basically a Vector2d version of Lerp.
        /// </summary>
        public static float GetRangeValue(FVector2D range, float pct)
        {
            return Lerp(range.X, range.Y, pct);
        }

        /// <summary>
        /// For the given Value clamped to the [Input:Range] inclusive, returns the corresponding percentage in [Output:Range] Inclusive.
        /// </summary>
        public static float GetMappedRangeValueClamped(FVector2D inputRange, FVector2D outputRange, float value)
        {
            float clampedPct = Clamp(GetRangePct(inputRange, value), 0.0f, 1.0f);
            return GetRangeValue(outputRange, clampedPct);
        }

        /// <summary>
        /// Transform the given Value relative to the input range to the Output Range.
        /// </summary>
        public static float GetMappedRangeValueUnclamped(FVector2D inputRange, FVector2D outputRange, float value)
        {
            return GetRangeValue(outputRange, GetRangePct(inputRange, value));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static float Lerp(float a, float b, float alpha)
        {
            return a + alpha * (b - a);
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static double Lerp(double a, double b, double alpha)
        {
            return a + alpha * (b - a);
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FLinearColor Lerp(FLinearColor a, FLinearColor b, float alpha)
        {
            return new FLinearColor(
                FMath.Lerp(a.R, b.R, alpha),
                FMath.Lerp(a.G, b.G, alpha),
                FMath.Lerp(a.B, b.B, alpha),
                FMath.Lerp(a.A, b.A, alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FColor Lerp(FColor a, FColor b, float alpha)
        {
            return new FColor(
                (byte)FMath.Lerp(a.R, b.R, alpha),
                (byte)FMath.Lerp(a.G, b.G, alpha),
                (byte)FMath.Lerp(a.B, b.B, alpha),
                (byte)FMath.Lerp(a.A, b.A, alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FVector2D Lerp(FVector2D a, FVector2D b, float alpha)
        {
            return new FVector2D(
                FMath.Lerp(a.X, b.X, alpha),
                FMath.Lerp(a.Y, b.Y, alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FVector Lerp(FVector a, FVector b, float alpha)
        {
            return new FVector(
                FMath.Lerp(a.X, b.X, alpha),
                FMath.Lerp(a.Y, b.Y, alpha),
                FMath.Lerp(a.Z, b.Z, alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FVector4 Lerp(FVector4 a, FVector4 b, float alpha)
        {
            return new FVector4(
                FMath.Lerp(a.X, b.X, alpha),
                FMath.Lerp(a.Y, b.Y, alpha),
                FMath.Lerp(a.Z, b.Z, alpha),
                FMath.Lerp(a.W, b.W, alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FIntPoint Lerp(FIntPoint a, FIntPoint b, float alpha)
        {
            return new FIntPoint(
                (int)FMath.Lerp(a.X, b.X, alpha),
                (int)FMath.Lerp(a.Y, b.Y, alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1.
        /// </summary>
        public static float LerpStable(float a, float b, float alpha)
        {
            return ((a * (1.0f - alpha)) + (b * alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1.
        /// </summary>
        public static double LerpStable(double a, double b, double alpha)
        {
            return ((a * (1.0 - alpha)) + (b * alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1.
        /// </summary>
        public static FLinearColor LerpStable(FLinearColor a, FLinearColor b, float alpha)
        {
            return new FLinearColor(
                FMath.LerpStable(a.R, b.R, alpha),
                FMath.LerpStable(a.G, b.G, alpha),
                FMath.LerpStable(a.B, b.B, alpha),
                FMath.LerpStable(a.A, b.A, alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1.
        /// </summary>
        public static FColor LerpStable(FColor a, FColor b, float alpha)
        {
            return new FColor(
                (byte)FMath.LerpStable(a.R, b.R, alpha),
                (byte)FMath.LerpStable(a.G, b.G, alpha),
                (byte)FMath.LerpStable(a.B, b.B, alpha),
                (byte)FMath.LerpStable(a.A, b.A, alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FVector2D LerpStable(FVector2D a, FVector2D b, float alpha)
        {
            return new FVector2D(
                FMath.LerpStable(a.X, b.X, alpha),
                FMath.LerpStable(a.Y, b.Y, alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FVector LerpStable(FVector a, FVector b, float alpha)
        {
            return new FVector(
                FMath.LerpStable(a.X, b.X, alpha),
                FMath.LerpStable(a.Y, b.Y, alpha),
                FMath.LerpStable(a.Z, b.Z, alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FVector4 LerpStable(FVector4 a, FVector4 b, float alpha)
        {
            return new FVector4(
                FMath.LerpStable(a.X, b.X, alpha),
                FMath.LerpStable(a.Y, b.Y, alpha),
                FMath.LerpStable(a.Z, b.Z, alpha),
                FMath.LerpStable(a.W, b.W, alpha));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FIntPoint LerpStable(FIntPoint a, FIntPoint b, float alpha)
        {
            return new FIntPoint(
                (int)FMath.LerpStable(a.X, b.X, alpha),
                (int)FMath.LerpStable(a.Y, b.Y, alpha));
        }

        /// <summary>
        /// Performs a 2D linear interpolation between four values values, FracX, FracY ranges from 0-1
        /// </summary>
        public static float BiLerp(float p00, float p10, float p01, float p11, float fracX, float fracY)
        {
            return Lerp(
                Lerp(p00, p10, fracX),
                Lerp(p01, p11, fracX),
                fracY);
        }

        /// <summary>
        /// Performs a 2D linear interpolation between four values values, FracX, FracY ranges from 0-1
        /// </summary>
        public static double BiLerp(double p00, double p10, double p01, double p11, double fracX, double fracY)
        {
            return Lerp(
                Lerp(p00, p10, fracX),
                Lerp(p01, p11, fracX),
                fracY);
        }

        /// <summary>
        /// Performs a cubic interpolation
        /// </summary>
        /// <param name="p0">end points</param>
        /// <param name="t0">tangent directions at end points</param>
        /// <param name="p1">end points</param>
        /// <param name="t1">tangent directions at end points</param>
        /// <param name="alpha">distance along spline</param>
        /// <returns>Interpolated value</returns>
        public static float CubicInterp(float p0, float t0, float p1, float t1, float alpha)
        {
            float a2 = alpha * alpha;
            float a3 = a2 * alpha;

            return (((2 * a3) - (3 * a2) + 1) * p0) + ((a3 - (2 * a2) + alpha) * t0) + ((a3 - a2) * t1) + (((-2 * a3) + (3 * a2)) * p1);
        }

        /// <summary>
        /// Performs a first derivative cubic interpolation
        /// </summary>
        /// <param name="p0">end points</param>
        /// <param name="t0">tangent directions at end points</param>
        /// <param name="p1">end points</param>
        /// <param name="t1">tangent directions at end points</param>
        /// <param name="alpha">distance along spline</param>
        /// <returns>Interpolated value</returns>
        public static float CubicInterpDerivative(float p0, float t0, float p1, float t1, float alpha)
        {
            float a = 6.0f * p0 + 3.0f * t0 + 3.0f * t1 - 6.0f * p1;
            float b = -6.0f * p0 - 4.0f * t0 - 2.0f * t1 + 6.0f * p1;
            float c = t0;

            float a2 = a * a;
            return (a * a2) + (b * alpha) + c;
        }

        /// <summary>
        /// Performs a second derivative cubic interpolation
        /// </summary>
        /// <param name="p0">end points</param>
        /// <param name="t0">tangent directions at end points</param>
        /// <param name="p1">end points</param>
        /// <param name="t1">tangent directions at end points</param>
        /// <param name="alpha">distance along spline</param>
        /// <returns>Interpolated value</returns>
        public static float CubicInterpSecondDerivative(float p0, float t0, float p1, float t1, float alpha)
        {
            float a = 12.0f * p0 + 6.0f * t0 + 6.0f * t1 - 12.0f * p1;
            float b = -6.0f * p0 - 4.0f * t0 - 2.0f * t1 + 6.0f * p1;

            return (a * alpha) + b;
        }

        /// <summary>
        /// Interpolate between A and B, applying an ease in function.  Exp controls the degree of the curve.
        /// </summary>
        public static float InterpEaseIn(float a, float b, float alpha, float exp)
        {
            float modifiedAlpha = Pow(alpha, exp);
            return Lerp(a, b, modifiedAlpha);
        }

        /// <summary>
        /// Interpolate between A and B, applying an ease out function.  Exp controls the degree of the curve.
        /// </summary>
        public static float InterpEaseOut(float a, float b, float alpha, float exp)
        {
            float modifiedAlpha = 1.0f - Pow(1.0f - alpha, exp);
            return Lerp(a, b, modifiedAlpha);
        }

        /// <summary>
        /// Interpolate between A and B, applying an ease in/out function.  Exp controls the degree of the curve.
        /// </summary>
        public static float InterpEaseInOut(float a, float b, float alpha, float exp)
        {
            return Lerp(a, b, (alpha < 0.5f) ?
                InterpEaseIn(0.0f, 1.0f, alpha * 2.0f, exp) * 0.5f :
                InterpEaseOut(0.0f, 1.0f, alpha * 2.0f - 1.0f, exp) * 0.5f + 0.5f);
        }

        /// <summary>
        /// Interpolation between A and B, applying a step function.
        /// </summary>
        public static float InterpStep(float a, float b, float alpha, int steps)
        {
            if (steps <= 1 || alpha <= 0)
            {
                return a;
            }
            else if (alpha >= 1)
            {
                return b;
            }

            float stepsAsFloat = (float)steps;
            float numIntervals = stepsAsFloat - 1.0f;
            float modifiedAlpha = FloorToFloat(alpha * stepsAsFloat) / numIntervals;
            return Lerp(a, b, modifiedAlpha);
        }

        /// <summary>
        /// Interpolation between A and B, applying a sinusoidal in function.
        /// </summary>
        public static float InterpSinIn(float a, float b, float alpha)
        {
            float modifiedAlpha = -1.0f * Cos(alpha * HalfPI) + 1.0f;
            return Lerp(a, b, modifiedAlpha);
        }

        /// <summary>
        /// Interpolation between A and B, applying a sinusoidal out function.
        /// </summary>
        public static float InterpSinOut(float a, float b, float alpha)
        {
            float modifiedAlpha = Sin(alpha * HalfPI);
            return Lerp(a, b, modifiedAlpha);
        }

        /// <summary>
        /// Interpolation between A and B, applying a sinusoidal in/out function.
        /// </summary>
        public static float InterpSinInOut(float a, float b, float alpha)
        {
            return Lerp(a, b, (alpha < 0.5f) ?
                InterpSinIn(0.0f, 1.0f, alpha * 2.0f) * 0.5f :
                InterpSinOut(0.0f, 1.0f, alpha * 2.0f - 1.0f) * 0.5f + 0.5f);
        }

        /// <summary>
        /// Interpolation between A and B, applying an exponential in function.
        /// </summary>
        public static float InterpExpoIn(float a, float b, float alpha)
        {
            float modifiedAlpha = (alpha == 0.0f) ? 0.0f : Pow(2.0f, 10.0f * (alpha - 1.0f));
            return Lerp(a, b, modifiedAlpha);
        }

        /// <summary>
        /// Interpolation between A and B, applying an exponential out function.
        /// </summary>
        public static float InterpExpoOut(float a, float b, float alpha)
        {
            float modifiedAlpha = (alpha == 1.0f) ? 1.0f : -Pow(2.0f, -10.0f * alpha) + 1.0f;
            return Lerp(a, b, modifiedAlpha);
        }

        /// <summary>
        /// Interpolation between A and B, applying an exponential in/out function.
        /// </summary>
        public static float InterpExpoInOut(float a, float b, float alpha)
        {
            return Lerp(a, b, (alpha < 0.5f) ?
                InterpExpoIn(0.0f, 1.0f, alpha * 2.0f) * 0.5f :
                InterpExpoOut(0.0f, 1.0f, alpha * 2.0f - 1.0f) * 0.5f + 0.5f);
        }

        /// <summary>
        /// Interpolation between A and B, applying a circular in function.
        /// </summary>
        public static float InterpCircularIn(float a, float b, float alpha)
        {
            float modifiedAlpha = -1.0f * (Sqrt(1.0f - alpha * alpha) - 1.0f);
            return Lerp(a, b, modifiedAlpha);
        }

        /// <summary>
        /// Interpolation between A and B, applying a circular out function.
        /// </summary>
        public static float InterpCircularOut(float a, float b, float alpha)
        {
            alpha -= 1.0f;
            float modifiedAlpha = Sqrt(1.0f - alpha * alpha);
            return Lerp(a, b, modifiedAlpha);
        }

        /// <summary>
        /// Interpolation between A and B, applying a circular in/out function.
        /// </summary>
        public static float InterpCircularInOut(float a, float b, float alpha)
        {
            return Lerp(a, b, (alpha < 0.5f) ?
                InterpCircularIn(0.0f, 1.0f, alpha * 2.0f) * 0.5f :
                InterpCircularOut(0.0f, 1.0f, alpha * 2.0f - 1.0f) * 0.5f + 0.5f);
        }

        // Rotator specific interpolation

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FRotator Lerp(FRotator a, FRotator b, float alpha)
        {
            return a + (b - a).GetNormalized() * alpha;
        }

        /// <summary>
        /// Similar to Lerp, but does not take the shortest path. Allows interpolation over more than 180 degrees.
        /// </summary>
        public static FRotator LerpRange(FRotator a, FRotator b, float alpha)
        {
            return (a * (1 - alpha) + b * alpha).GetNormalized();
        }

        // Quat-specific interpolation

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
            return Lerp(
                FQuat.Slerp_NotNormalized(p00, p10, fracX),
                FQuat.Slerp_NotNormalized(p01, p11, fracX),
                fracY);
        }

        /// <summary>
        /// Performs a cubic interpolation - In the case of quaternions, we use a bezier like approach.
        /// </summary>
        /// <param name="p0">end points</param>
        /// <param name="t0">tangent directions at end points</param>
        /// <param name="p1">end points</param>
        /// <param name="t1">tangent directions at end points</param>
        /// <param name="alpha">distance along spline</param>
        /// <returns>Interpolated value</returns>
        public static FQuat CubicInterp(FQuat p0, FQuat t0, FQuat p1, FQuat t1, float alpha)
        {
            return FQuat.Squad(p0, t0, p1, t1, alpha);
        }

        /// <summary>
        /// Cubic Catmull-Rom Spline interpolation. Based on http://www.cemyuksel.com/research/catmullrom_param/catmullrom.pdf 
        /// Curves are guaranteed to pass through the control points and are easily chained together.
        /// Equation supports abitrary parameterization. eg. Uniform=0,1,2,3 ; chordal= |Pn - Pn-1| ; centripetal = |Pn - Pn-1|^0.5
        /// </summary>
        /// <param name="p0">The control point preceding the interpolation range.</param>
        /// <param name="p1">The control point starting the interpolation range.</param>
        /// <param name="p2">The control point ending the interpolation range.</param>
        /// <param name="p3">The control point following the interpolation range.</param>
        /// <param name="t0">The interpolation parameters for the corresponding control points.</param>
        /// <param name="t1">The interpolation parameters for the corresponding control points.</param>
        /// <param name="t2">The interpolation parameters for the corresponding control points.</param>
        /// <param name="t3">The interpolation parameters for the corresponding control points.</param>
        /// <param name="t">The interpolation factor in the range 0 to 1. 0 returns P1. 1 returns P2.</param>
        /// <returns></returns>
        public static float CubicCRSplineInterp(float p0, float p1, float p2, float p3, float t0, float t1, float t2, float t3, float t)
        {
            //Based on http://www.cemyuksel.com/research/catmullrom_param/catmullrom.pdf 
            float invT1MinusT0 = 1.0f / (t1 - t0);
            float l01 = (p0 * ((t1 - t) * invT1MinusT0)) + (p1 * ((t - t0) * invT1MinusT0));
            float invT2MinusT1 = 1.0f / (t2 - t1);
            float l12 = (p1 * ((t2 - t) * invT2MinusT1)) + (p2 * ((t - t1) * invT2MinusT1));
            float invT3MinusT2 = 1.0f / (t3 - t2);
            float l23 = (p2 * ((t3 - t) * invT3MinusT2)) + (p3 * ((t - t2) * invT3MinusT2));

            float invT2MinusT0 = 1.0f / (t2 - t0);
            float l012 = (l01 * ((t2 - t) * invT2MinusT0)) + (l12 * ((t - t0) * invT2MinusT0));
            float invT3MinusT1 = 1.0f / (t3 - t1);
            float l123 = (l12 * ((t3 - t) * invT3MinusT1)) + (l23 * ((t - t1) * invT3MinusT1));

            return ((l012 * ((t2 - t) * invT2MinusT1)) + (l123 * ((t - t1) * invT2MinusT1)));
        }

        /// <summary>
        /// Same as CubicCRSplineInterp but with additional saftey checks. If the checks fail P1 is returned.
        /// </summary>
        public static float CubicCRSplineInterpSafe(float p0, float p1, float p2, float p3, float t0, float t1, float t2, float t3, float t)
        {
            //Based on http://www.cemyuksel.com/research/catmullrom_param/catmullrom.pdf 
            float t1MinusT0 = (t1 - t0);
            float t2MinusT1 = (t2 - t1);
            float t3MinusT2 = (t3 - t2);
            float t2MinusT0 = (t2 - t0);
            float t3MinusT1 = (t3 - t1);
            if (IsNearlyZero(t1MinusT0) || IsNearlyZero(t2MinusT1) || IsNearlyZero(t3MinusT2) || IsNearlyZero(t2MinusT0) || IsNearlyZero(t3MinusT1))
            {
                //There's going to be a divide by zero here so just bail out and return P1
                return p1;
            }

            float invT1MinusT0 = 1.0f / (t1 - t0);
            float l01 = (p0 * ((t1 - t) * invT1MinusT0)) + (p1 * ((t - t0) * invT1MinusT0));
            float invT2MinusT1 = 1.0f / (t2 - t1);
            float l12 = (p1 * ((t2 - t) * invT2MinusT1)) + (p2 * ((t - t1) * invT2MinusT1));
            float invT3MinusT2 = 1.0f / (t3 - t2);
            float l23 = (p2 * ((t3 - t) * invT3MinusT2)) + (p3 * ((t - t2) * invT3MinusT2));

            float invT2MinusT0 = 1.0f / (t2 - t0);
            float l012 = (l01 * ((t2 - t) * invT2MinusT0)) + (l12 * ((t - t0) * invT2MinusT0));
            float invT3MinusT1 = 1.0f / (t3 - t1);
            float l123 = (l12 * ((t3 - t) * invT3MinusT1)) + (l23 * ((t - t1) * invT3MinusT1));

            return ((l012 * ((t2 - t) * invT2MinusT1)) + (l123 * ((t - t1) * invT2MinusT1)));
        }

        // Special-case interpolation

        /// <summary>
        /// Interpolate a normal vector Current to Target, by interpolating the angle between those vectors with constant step.
        /// </summary>
        public static FVector VInterpNormalRotationTo(FVector current, FVector target, float deltaTime, float rotationSpeedDegrees)
        {
            // Find delta rotation between both normals.
            FQuat deltaQuat = FQuat.FindBetween(current, target);

            // Decompose into an axis and angle for rotation
            FVector deltaAxis;
            float deltaAngle;
            deltaQuat.ToAxisAndAngle(out deltaAxis, out deltaAngle);

            // Find rotation step for this frame
            float rotationStepRadians = rotationSpeedDegrees * (PI / 180) * deltaTime;

            if (FMath.Abs(deltaAngle) > rotationStepRadians)
            {
                deltaAngle = FMath.Clamp(deltaAngle, -rotationStepRadians, rotationStepRadians);
                deltaQuat = new FQuat(deltaAxis, deltaAngle);
                return deltaQuat.RotateVector(current);
            }
            return target;
        }

        /// <summary>
        /// Interpolate vector from Current to Target with constant step
        /// </summary>
        public static FVector VInterpConstantTo(FVector current, FVector target, float deltaTime, float interpSpeed)
        {
            FVector delta = target - current;
            float deltaM = delta.Size();
            float maxStep = interpSpeed * deltaTime;

            if (deltaM > maxStep)
            {
                if (maxStep > 0.0f)
                {
                    FVector deltaN = delta / deltaM;
                    return current + deltaN * maxStep;
                }
                else
                {
                    return current;
                }
            }

            return target;
        }

        /// <summary>
        /// Interpolate vector from Current to Target. Scaled by distance to Target, so it has a strong start speed and ease out.
        /// </summary>
        public static FVector VInterpTo(FVector current, FVector target, float deltaTime, float interpSpeed)
        {
            // If no interp speed, jump to target value
            if (interpSpeed <= 0.0f)
            {
                return target;
            }

            // Distance to reach
            FVector dist = target - current;

            // If distance is too small, just set the desired location
            if (dist.SizeSquared() < KindaSmallNumber)
            {
                return target;
            }

            // Delta Move, Clamp so we do not over shoot.
            FVector deltaMove = dist * FMath.Clamp(deltaTime * interpSpeed, 0.0f, 1.0f);

            return current + deltaMove;
        }

        /// <summary>
        /// Interpolate vector2D from Current to Target with constant step
        /// </summary>
        public static FVector2D Vector2DInterpConstantTo(FVector2D current, FVector2D target, float deltaTime, float interpSpeed)
        {
            FVector2D delta = target - current;
            float deltaM = delta.Size();
            float maxStep = interpSpeed * deltaTime;

            if (deltaM > maxStep)
            {
                if (maxStep > 0.0f)
                {
                    FVector2D deltaN = delta / deltaM;
                    return current + deltaN * maxStep;
                }
                else
                {
                    return current;
                }
            }

            return target;
        }

        /// <summary>
        /// Interpolate vector2D from Current to Target. Scaled by distance to Target, so it has a strong start speed and ease out.
        /// </summary>
        public static FVector2D Vector2DInterpTo(FVector2D current, FVector2D target, float deltaTime, float interpSpeed)
        {
            if (interpSpeed <= 0.0f)
            {
                return target;
            }

            FVector2D dist = target - current;
            if (dist.SizeSquared() < KindaSmallNumber)
            {
                return target;
            }

            FVector2D deltaMove = dist * FMath.Clamp(deltaTime * interpSpeed, 0.0f, 1.0f);
            return current + deltaMove;
        }

        /// <summary>
        /// Interpolate rotator from Current to Target with constant step
        /// </summary>
        public static FRotator RInterpConstantTo(FRotator current, FRotator target, float deltaTime, float interpSpeed)
        {
            // if DeltaTime is 0, do not perform any interpolation (Location was already calculated for that frame)
            if (deltaTime == 0.0f || current == target)
            {
                return current;
            }

            // If no interp speed, jump to target value
            if (interpSpeed <= 0.0f)
            {
                return target;
            }

            float deltaInterpSpeed = interpSpeed * deltaTime;

            FRotator deltaMove = (target - current).GetNormalized();
            FRotator result = current;
            result.Pitch += FMath.Clamp(deltaMove.Pitch, -deltaInterpSpeed, deltaInterpSpeed);
            result.Yaw += FMath.Clamp(deltaMove.Yaw, -deltaInterpSpeed, deltaInterpSpeed);
            result.Roll += FMath.Clamp(deltaMove.Roll, -deltaInterpSpeed, deltaInterpSpeed);
            return result.GetNormalized();
        }

        /// <summary>
        /// Interpolate rotator from Current to Target. Scaled by distance to Target, so it has a strong start speed and ease out.
        /// </summary>
        public static FRotator RInterpTo(FRotator current, FRotator target, float deltaTime, float interpSpeed)
        {
            // if DeltaTime is 0, do not perform any interpolation (Location was already calculated for that frame)
            if (deltaTime == 0.0f || current == target)
            {
                return current;
            }

            // If no interp speed, jump to target value
            if (interpSpeed <= 0.0f)
            {
                return target;
            }

            float deltaInterpSpeed = interpSpeed * deltaTime;

            FRotator delta = (target - current).GetNormalized();

            // If steps are too small, just return Target and assume we have reached our destination.
            if (delta.IsNearlyZero())
            {
                return target;
            }

            // Delta Move, Clamp so we do not over shoot.
            FRotator deltaMove = delta * FMath.Clamp(deltaInterpSpeed, 0.0f, 1.0f);
            return (current + deltaMove).GetNormalized();
        }

        /// <summary>
        /// Interpolate float from Current to Target with constant step
        /// </summary>
        public static float FInterpConstantTo(float current, float target, float deltaTime, float interpSpeed)
        {
            float dist = target - current;

            // If distance is too small, just set the desired location
            if (FMath.Square(dist) < SmallNumber)
            {
                return target;
            }

            float step = interpSpeed * deltaTime;
            return current + FMath.Clamp(dist, -step, step);
        }

        /// <summary>
        /// Interpolate float from Current to Target. Scaled by distance to Target, so it has a strong start speed and ease out.
        /// </summary>
        public static float FInterpTo(float current, float target, float deltaTime, float interpSpeed)
        {
            // If no interp speed, jump to target value
            if (interpSpeed <= 0.0f)
            {
                return target;
            }

            // Distance to reach
            float dist = target - current;

            // If distance is too small, just set the desired location
            if (FMath.Square(dist) < SmallNumber)
            {
                return target;
            }

            // Delta Move, Clamp so we do not over shoot.
            float deltaMove = dist * FMath.Clamp(deltaTime * interpSpeed, 0.0f, 1.0f);

            return current + deltaMove;
        }

        /// <summary>
        /// Interpolate Linear Color from Current to Target. Scaled by distance to Target, so it has a strong start speed and ease out.
        /// </summary>
        public static FLinearColor CInterpTo(FLinearColor current, FLinearColor target, float deltaTime, float interpSpeed)
        {
            // If no interp speed, jump to target value
            if (interpSpeed <= 0.0f)
            {
                return target;
            }

            // Difference between colors
            float dist = FLinearColor.Dist(target, current);

            // If distance is too small, just set the desired color
            if (dist < KindaSmallNumber)
            {
                return target;
            }

            // Delta change, Clamp so we do not over shoot.
            FLinearColor deltaMove = (target - current) * FMath.Clamp(deltaTime * interpSpeed, 0.0f, 1.0f);

            return current + deltaMove;
        }

        /// <summary>
        /// Interpolate quaternion from Current to Target with constant step (in radians)
        /// </summary>
        public static FQuat QInterpConstantTo(FQuat current, FQuat target, float deltaTime, float interpSpeed)
        {
            // If no interp speed, jump to target value
            if (interpSpeed <= 0.0f)
            {
                return target;
            }

            // If the values are nearly equal, just return Target and assume we have reached our destination.
            if (current.Equals(target))
            {
                return target;
            }

            float deltaInterpSpeed = FMath.Clamp(deltaTime * interpSpeed, 0.0f, 1.0f);
            float angularDistance = FMath.Max(SmallNumber, target.AngularDistance(current));
            float alpha = FMath.Clamp(deltaInterpSpeed / angularDistance, 0.0f, 1.0f);

            return FQuat.Slerp(current, target, alpha);
        }

        /// <summary>
        /// Interpolate quaternion from Current to Target. Scaled by angle to Target, so it has a strong start speed and ease out.
        /// </summary>
        public static FQuat QInterpTo(FQuat current, FQuat target, float deltaTime, float interpSpeed)
        {
            // If no interp speed, jump to target value
            if (interpSpeed <= 0.0f)
            {
                return target;
            }

            // If the values are nearly equal, just return Target and assume we have reached our destination.
            if (current.Equals(target))
            {
                return target;
            }

            return FQuat.Slerp(current, target, FMath.Clamp(interpSpeed * deltaTime, 0.0f, 1.0f));
        }

        /// <summary>
        /// Simple function to create a pulsating scalar value
        /// </summary>
        /// <param name="currentTime">Current absolute time</param>
        /// <param name="pulsesPerSecond">How many full pulses per second?</param>
        /// <param name="phase">Optional phase amount, between 0.0 and 1.0 (to synchronize pulses)</param>
        /// <returns>Pulsating value (0.0-1.0)</returns>
        public static float MakePulsatingValue(double currentTime, float pulsesPerSecond, float phase = 0.0f)
        {
            return 0.5f + 0.5f * FMath.Sin((float)(((0.25f + phase) * PI * 2.0) + (currentTime * PI * 2.0) * pulsesPerSecond));
        }
    }
}
