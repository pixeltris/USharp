using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Additional methods defined in InterpCurvePoint.h

    // Engine\Source\Runtime\Core\Public\Math\InterpCurvePoint.h
    // Engine\Source\Runtime\Core\Private\Math\UnrealMath.cpp

    public static partial class FMath
    {
        /// <summary>
        /// Clamps a tangent formed by the specified control point values
        /// </summary>
        public static float ClampFloatTangent(float prevPointVal, float prevTime, float curPointVal, float curTime, float nextPointVal, float nextTime)
        {
            float prevToNextTimeDiff = FMath.Max(FMath.KindaSmallNumber, nextTime - prevTime);
            float prevToCurTimeDiff = FMath.Max(FMath.KindaSmallNumber, curTime - prevTime);
            float curToNextTimeDiff = FMath.Max(FMath.KindaSmallNumber, nextTime - curTime);

            float outTangentVal = 0.0f;

            float prevToNextHeightDiff = nextPointVal - prevPointVal;
            float prevToCurHeightDiff = curPointVal - prevPointVal;
            float curToNextHeightDiff = nextPointVal - curPointVal;

            // Check to see if the current point is crest
            if ((prevToCurHeightDiff >= 0.0f && curToNextHeightDiff <= 0.0f) ||
                (prevToCurHeightDiff <= 0.0f && curToNextHeightDiff >= 0.0f))
            {
                // Neighbor points are both both on the same side, so zero out the tangent
                outTangentVal = 0.0f;
            }
            else
            {
                // The three points form a slope

                // Constants
                const float clampThreshold = 0.333f;

                // Compute height deltas
                float curToNextTangent = curToNextHeightDiff / curToNextTimeDiff;
                float prevToCurTangent = prevToCurHeightDiff / prevToCurTimeDiff;
                float prevToNextTangent = prevToNextHeightDiff / prevToNextTimeDiff;

                // Default to not clamping
                float unclampedTangent = prevToNextTangent;
                float clampedTangent = unclampedTangent;

                const float lowerClampThreshold = clampThreshold;
                const float upperClampThreshold = 1.0f - clampThreshold;

                // @todo: Would we get better results using percentange of TIME instead of HEIGHT?
                float curHeightAlpha = prevToCurHeightDiff / prevToNextHeightDiff;

                if (prevToNextHeightDiff > 0.0f)
                {
                    if (curHeightAlpha < lowerClampThreshold)
                    {
                        // 1.0 = maximum clamping (flat), 0.0 = minimal clamping (don't touch)
                        float clampAlpha = 1.0f - curHeightAlpha / clampThreshold;
                        float lowerClamp = FMath.Lerp(prevToNextTangent, prevToCurTangent, clampAlpha);
                        clampedTangent = FMath.Min(clampedTangent, lowerClamp);
                    }

                    if (curHeightAlpha > upperClampThreshold)
                    {
                        // 1.0 = maximum clamping (flat), 0.0 = minimal clamping (don't touch)
                        float clampAlpha = (curHeightAlpha - upperClampThreshold) / clampThreshold;
                        float upperClamp = FMath.Lerp(prevToNextTangent, curToNextTangent, clampAlpha);
                        clampedTangent = FMath.Min(clampedTangent, upperClamp);
                    }
                }
                else
                {
                    if (curHeightAlpha < lowerClampThreshold)
                    {
                        // 1.0 = maximum clamping (flat), 0.0 = minimal clamping (don't touch)
                        float clampAlpha = 1.0f - curHeightAlpha / clampThreshold;
                        float lowerClamp = FMath.Lerp(prevToNextTangent, prevToCurTangent, clampAlpha);
                        clampedTangent = FMath.Max(clampedTangent, lowerClamp);
                    }

                    if (curHeightAlpha > upperClampThreshold)
                    {
                        // 1.0 = maximum clamping (flat), 0.0 = minimal clamping (don't touch)
                        float clampAlpha = (curHeightAlpha - upperClampThreshold) / clampThreshold;
                        float upperClamp = FMath.Lerp(prevToNextTangent, curToNextTangent, clampAlpha);
                        clampedTangent = FMath.Max(clampedTangent, upperClamp);
                    }
                }

                outTangentVal = clampedTangent;
            }

            return outTangentVal;
        }

        /// <summary>
        /// Computes Tangent for a curve segment
        /// </summary>
        public static void AutoCalcTangent(float prevP, float p, float nextP, float tension, out float outTan)
        {
            outTan = (1.0f - tension) * ((p - prevP) + (nextP - p));
        }

        /// <summary>
        /// Computes Tangent for a curve segment
        /// </summary>
        public static void AutoCalcTangent(FVector prevP, FVector p, FVector nextP, float tension, out FVector outTan)
        {
            outTan = (1.0f - tension) * ((p - prevP) + (nextP - p));
        }

        /// <summary>
        /// Computes Tangent for a curve segment
        /// </summary>
        public static void AutoCalcTangent(FVector2D prevP, FVector2D p, FVector2D nextP, float tension, out FVector2D outTan)
        {
            outTan = (1.0f - tension) * ((p - prevP) + (nextP - p));
        }

        /// <summary>
        /// Computes Tangent for a curve segment
        /// </summary>
        public static void AutoCalcTangent(FTwoVectors prevP, FTwoVectors p, FTwoVectors nextP, float tension, out FTwoVectors outTan)
        {
            outTan = (1.0f - tension) * ((p - prevP) + (nextP - p));
        }

        /// <summary>
        /// This actually returns the control point not a tangent. This is expected by the CubicInterp function for Quaternions
        /// </summary>
        public static void AutoCalcTangent(FQuat prevP, FQuat p, FQuat nextP, float tension, out FQuat outTan)
        {
            FQuat.CalcTangents(prevP, p, nextP, tension, out outTan);
        }

        private static unsafe void ComputeClampableFloatVectorCurveTangentClamped(
            float prevTime, float* prevPoint,
            float curTime, float* curPoint,
            float nextTime, float* nextPoint,
            float tension, int typeSize, float* outTangent)
        {
            // NOTE: We always treat the type as an array of floats
            float* prevPointVal = prevPoint;
            float* curPointVal = curPoint;
            float* nextPointVal = nextPoint;
            float* outTangentVal = outTangent;

            for (int curValPos = 0; curValPos < typeSize; curValPos += sizeof(float))
            {
                // Clamp it!
                float clampedTangent = ClampFloatTangent(
                    *prevPointVal, prevTime,
                    *curPointVal, curTime,
                    *nextPointVal, nextTime);

                // Apply tension value
                *outTangentVal = (1.0f - tension) * clampedTangent;

                // Advance pointers
                ++outTangentVal;
                ++prevPointVal;
                ++curPointVal;
                ++nextPointVal;
            }
        }

        /// <summary>
        /// Computes a tangent for the specified control point.  Special case for float types; supports clamping.
        /// </summary>
        public static unsafe void ComputeCurveTangent(
            float prevTime, float prevPoint,
            float curTime, float curPoint,
            float nextTime, float nextPoint,
            float tension, bool wantClamping, out float outTangent)
        {
            if (wantClamping)
            {
                float result;
                ComputeClampableFloatVectorCurveTangentClamped(
                    prevTime, &prevPoint,
                    curTime, &curPoint,
                    nextTime, &nextPoint,
                    tension, sizeof(float), &result);
                outTangent = result;
            }
            else
            {
                AutoCalcTangent(prevPoint, curPoint, nextPoint, tension, out outTangent);
                float prevToNextTimeDiff = FMath.Max(FMath.KindaSmallNumber, nextTime - prevTime);
                outTangent /= prevToNextTimeDiff;
            }
        }

        /// <summary>
        /// Computes a tangent for the specified control point.  Special case for FVector types; supports clamping.
        /// </summary>
        public static unsafe void ComputeCurveTangent(
            float prevTime, FVector prevPoint,
            float curTime, FVector curPoint,
            float nextTime, FVector nextPoint,
            float tension, bool wantClamping, out FVector outTangent)
        {
            if (wantClamping)
            {
                FVector result;
                ComputeClampableFloatVectorCurveTangentClamped(
                    prevTime, (float*)&prevPoint,
                    curTime, (float*)&curPoint,
                    nextTime, (float*)&nextPoint,
                    tension, sizeof(FVector), (float*)&result);
                outTangent = result;
            }
            else
            {
                AutoCalcTangent(prevPoint, curPoint, nextPoint, tension, out outTangent);
                float prevToNextTimeDiff = FMath.Max(FMath.KindaSmallNumber, nextTime - prevTime);
                outTangent /= prevToNextTimeDiff;
            }
        }

        /// <summary>
        /// Computes a tangent for the specified control point.  Special case for FVector2D types; supports clamping.
        /// </summary>
        public static unsafe void ComputeCurveTangent(
            float prevTime, FVector2D prevPoint,
            float curTime, FVector2D curPoint,
            float nextTime, FVector2D nextPoint,
            float tension, bool wantClamping, out FVector2D outTangent)
        {
            if (wantClamping)
            {
                FVector2D result;
                ComputeClampableFloatVectorCurveTangentClamped(
                    prevTime, (float*)&prevPoint,
                    curTime, (float*)&curPoint,
                    nextTime, (float*)&nextPoint,
                    tension, sizeof(FVector2D), (float*)&result);
                outTangent = result;
            }
            else
            {
                AutoCalcTangent(prevPoint, curPoint, nextPoint, tension, out outTangent);
                float prevToNextTimeDiff = FMath.Max(FMath.KindaSmallNumber, nextTime - prevTime);
                outTangent /= prevToNextTimeDiff;
            }
        }

        /// <summary>
        /// Computes a tangent for the specified control point.  Special case for FTwoVectors types; supports clamping.
        /// </summary>
        public static unsafe void ComputeCurveTangent(
            float prevTime, FTwoVectors prevPoint,
            float curTime, FTwoVectors curPoint,
            float nextTime, FTwoVectors nextPoint,
            float tension, bool wantClamping, out FTwoVectors outTangent)
        {
            if (wantClamping)
            {
                FTwoVectors result;
                ComputeClampableFloatVectorCurveTangentClamped(
                    prevTime, (float*)&prevPoint,
                    curTime, (float*)&curPoint,
                    nextTime, (float*)&nextPoint,
                    tension, sizeof(FTwoVectors), (float*)&result);
                outTangent = result;
            }
            else
            {
                AutoCalcTangent(prevPoint, curPoint, nextPoint, tension, out outTangent);
                float prevToNextTimeDiff = FMath.Max(FMath.KindaSmallNumber, nextTime - prevTime);
                outTangent /= prevToNextTimeDiff;
            }
        }

        private static void FindBounds(
            out float outMin, out float outMax, 
            float start, float startLeaveTan, float startT, 
            float end, float endArriveTan, float endT, 
            bool curve)
        {
            outMin = FMath.Min(start, end);
            outMax = FMath.Max(start, end);

            // Do we need to consider extermeties of a curve?
            if (curve)
            {
                // Scale tangents based on time interval, so this code matches the behaviour in FInterpCurve::Eval
                float diff = endT - startT;
                startLeaveTan *= diff;
                endArriveTan *= diff;

                float a = 6.0f * start + 3.0f * startLeaveTan + 3.0f * endArriveTan - 6.0f * end;
                float b = -6.0f * start - 4.0f * startLeaveTan - 2.0f * endArriveTan + 6.0f * end;
                float c = startLeaveTan;

                float discriminant = (b * b) - (4.0f * a * c);
                if (discriminant > 0.0f && !FMath.IsNearlyZero(a)) // Solving doesn't work if a is zero, which usually indicates co-incident start and end, and zero tangents anyway
                {
                    float sqrtDisc = FMath.Sqrt(discriminant);

                    float x0 = (-b + sqrtDisc) / (2.0f * a); // x0 is the 'Alpha' ie between 0 and 1
                    float t0 = startT + x0 * (endT - startT); // Then t0 is the actual 'time' on the curve
                    if (t0 > startT && t0 < endT)
                    {
                        float val = FMath.CubicInterp(start, startLeaveTan, end, endArriveTan, x0);

                        outMin = FMath.Min(outMin, val);
                        outMax = FMath.Max(outMax, val);
                    }

                    float x1 = (-b - sqrtDisc) / (2.0f * a);
                    float t1 = startT + x1 * (endT - startT);
                    if (t1 > startT && t1 < endT)
                    {
                        float val = FMath.CubicInterp(start, startLeaveTan, end, endArriveTan, x1);

                        outMin = FMath.Min(outMin, val);
                        outMax = FMath.Max(outMax, val);
                    }
                }
            }
        }

        /// <summary>
        /// Calculate bounds of float inervals
        /// </summary>
        /// <param name="start">interp curve point at Start</param>
        /// <param name="end">curve point at End</param>
        /// <param name="currentMin">Input and Output could be updated if needs new interval minimum bound</param>
        /// <param name="currentMax">Input and Output could be updated if needs new interval maximmum bound</param>
        public static void CurveFindIntervalBounds(
            FInterpCurvePointFloat start, FInterpCurvePointFloat end,
            ref float currentMin, ref float currentMax)
        {
            bool isCurve = start.IsCurveKey();

            float min, max;

            FindBounds(out min, out max, start.OutVal, start.LeaveTangent, start.InVal, end.OutVal, end.ArriveTangent, end.InVal, isCurve);

            currentMin = FMath.Min(currentMin, min);
            currentMax = FMath.Max(currentMax, max);
        }

        /// <summary>
        /// Calculate bounds of 2D vector intervals
        /// </summary>
        /// <param name="start">interp curve point at Start</param>
        /// <param name="end">interp curve point at End</param>
        /// <param name="currentMin">Input and Output could be updated if needs new interval minimum bound</param>
        /// <param name="currentMax">Input and Output could be updated if needs new interval maximmum bound</param>
        public static void CurveFindIntervalBounds(
            FInterpCurvePointVector2D start, FInterpCurvePointVector2D end,
            ref FVector2D currentMin, ref FVector2D currentMax)
        {
            bool isCurve = start.IsCurveKey();

            float min, max;

            FindBounds(out min, out max, start.OutVal.X, start.LeaveTangent.X, start.InVal, end.OutVal.X, end.ArriveTangent.X, end.InVal, isCurve);
            currentMin.X = FMath.Min(currentMin.X, min);
            currentMax.X = FMath.Max(currentMax.X, max);

            FindBounds(out min, out max, start.OutVal.Y, start.LeaveTangent.Y, start.InVal, end.OutVal.Y, end.ArriveTangent.Y, end.InVal, isCurve);
            currentMin.Y = FMath.Min(currentMin.Y, min);
            currentMax.Y = FMath.Max(currentMax.Y, max);
        }

        /// <summary>
        /// Calculate bounds of vector intervals
        /// </summary>
        /// <param name="start">interp curve point at Start</param>
        /// <param name="end">interp curve point at End</param>
        /// <param name="currentMin">Input and Output could be updated if needs new interval minimum bound</param>
        /// <param name="currentMax">Input and Output could be updated if needs new interval maximmum bound</param>
        public static void CurveFindIntervalBounds(
            FInterpCurvePointVector start, FInterpCurvePointVector end,
            ref FVector currentMin, ref FVector currentMax)
        {
            bool isCurve = start.IsCurveKey();

            float min, max;

            FindBounds(out min, out max, start.OutVal.X, start.LeaveTangent.X, start.InVal, end.OutVal.X, end.ArriveTangent.X, end.InVal, isCurve);
            currentMin.X = FMath.Min(currentMin.X, min);
            currentMax.X = FMath.Max(currentMax.X, max);

            FindBounds(out min, out max, start.OutVal.Y, start.LeaveTangent.Y, start.InVal, end.OutVal.Y, end.ArriveTangent.Y, end.InVal, isCurve);
            currentMin.Y = FMath.Min(currentMin.Y, min);
            currentMax.Y = FMath.Max(currentMax.Y, max);

            FindBounds(out min, out max, start.OutVal.Z, start.LeaveTangent.Z, start.InVal, end.OutVal.Z, end.ArriveTangent.Z, end.InVal, isCurve);
            currentMin.Z = FMath.Min(currentMin.Z, min);
            currentMax.Z = FMath.Max(currentMax.Z, max);
        }

        /// <summary>
        /// Calculate bounds of twovector intervals
        /// </summary>
        /// <param name="start">interp curve point at Start</param>
        /// <param name="end">interp curve point at End</param>
        /// <param name="currentMin">Input and Output could be updated if needs new interval minimum bound</param>
        /// <param name="currentMax">Input and Output could be updated if needs new interval maximmum bound</param>
        public static void CurveFindIntervalBounds(
            FInterpCurvePointTwoVectors start, FInterpCurvePointTwoVectors end,
            ref FTwoVectors currentMin, ref FTwoVectors currentMax)
        {
            bool isCurve = start.IsCurveKey();

            float min, max;

            // Do the first curve
            FindBounds(out min, out max, start.OutVal.V1.X, start.LeaveTangent.V1.X, start.InVal, end.OutVal.V1.X, end.ArriveTangent.V1.X, end.InVal, isCurve);
            currentMin.V1.X = FMath.Min(currentMin.V1.X, min);
            currentMax.V1.X = FMath.Max(currentMax.V1.X, max);

            FindBounds(out min, out max, start.OutVal.V1.Y, start.LeaveTangent.V1.Y, start.InVal, end.OutVal.V1.Y, end.ArriveTangent.V1.Y, end.InVal, isCurve);
            currentMin.V1.Y = FMath.Min(currentMin.V1.Y, min);
            currentMax.V1.Y = FMath.Max(currentMax.V1.Y, max);

            FindBounds(out min, out max, start.OutVal.V1.Z, start.LeaveTangent.V1.Z, start.InVal, end.OutVal.V1.Z, end.ArriveTangent.V1.Z, end.InVal, isCurve);
            currentMin.V1.Z = FMath.Min(currentMin.V1.Z, min);
            currentMax.V1.Z = FMath.Max(currentMax.V1.Z, max);

            // Do the second curve
            FindBounds(out min, out max, start.OutVal.V2.X, start.LeaveTangent.V2.X, start.InVal, end.OutVal.V2.X, end.ArriveTangent.V2.X, end.InVal, isCurve);
            currentMin.V2.X = FMath.Min(currentMin.V2.X, min);
            currentMax.V2.X = FMath.Max(currentMax.V2.X, max);

            FindBounds(out min, out max, start.OutVal.V2.Y, start.LeaveTangent.V2.Y, start.InVal, end.OutVal.V2.Y, end.ArriveTangent.V2.Y, end.InVal, isCurve);
            currentMin.V2.Y = FMath.Min(currentMin.V2.Y, min);
            currentMax.V2.Y = FMath.Max(currentMax.V2.Y, max);

            FindBounds(out min, out max, start.OutVal.V2.Z, start.LeaveTangent.V2.Z, start.InVal, end.OutVal.V2.Z, end.ArriveTangent.V2.Z, end.InVal, isCurve);
            currentMin.V2.Z = FMath.Min(currentMin.V2.Z, min);
            currentMax.V2.Z = FMath.Max(currentMax.V2.Z, max);
        }

        /// <summary>
        /// Calculate bounds of color intervals
        /// </summary>
        /// <param name="start">interp curve point at Start</param>
        /// <param name="end">interp curve point at End</param>
        /// <param name="currentMin">Input and Output could be updated if needs new interval minimum bound</param>
        /// <param name="currentMax">Input and Output could be updated if needs new interval maximmum bound</param>
        public static void CurveFindIntervalBounds(
            FInterpCurvePointLinearColor start, FInterpCurvePointLinearColor end,
            ref FLinearColor currentMin, ref FLinearColor currentMax)
        {
            bool isCurve = start.IsCurveKey();

            float min, max;

            FindBounds(out min, out max, start.OutVal.R, start.LeaveTangent.R, start.InVal, end.OutVal.R, end.ArriveTangent.R, end.InVal, isCurve);
            currentMin.R = FMath.Min(currentMin.R, min);
            currentMax.R = FMath.Max(currentMax.R, max);

            FindBounds(out min, out max, start.OutVal.G, start.LeaveTangent.G, start.InVal, end.OutVal.G, end.ArriveTangent.G, end.InVal, isCurve);
            currentMin.G = FMath.Min(currentMin.G, min);
            currentMax.G = FMath.Max(currentMax.G, max);

            FindBounds(out min, out max, start.OutVal.B, start.LeaveTangent.B, start.InVal, end.OutVal.B, end.ArriveTangent.B, end.InVal, isCurve);
            currentMin.B = FMath.Min(currentMin.B, min);
            currentMax.B = FMath.Max(currentMax.B, max);

            FindBounds(out min, out max, start.OutVal.A, start.LeaveTangent.A, start.InVal, end.OutVal.A, end.ArriveTangent.A, end.InVal, isCurve);
            currentMin.A = FMath.Min(currentMin.A, min);
            currentMax.A = FMath.Max(currentMax.A, max);
        }
    }
}
