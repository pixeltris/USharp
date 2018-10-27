using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\InterpCurvePoint.h

    // Generic version of FInterpCurve, this may be better to use but in terms of code gen we would need to map
    // the blueprint generated versions to this struct instead (also this might complicate things with type mapping
    // when attempting to resolve types such as with the IL rewriter)

    /*[StructLayout(LayoutKind.Sequential)]
    public struct FInterpCurve<T> : IEquatable<FInterpCurve<T>> where T : IEquatable<T>
    {
        public float InVal;
        public T OutVal;
        public T ArriveTangent;
        public T LeaveTangent;
        public EInterpCurveMode InterpMode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inVal">input value that corresponds to this key</param>
        /// <param name="outVal">Output value of templated type</param>
        public FInterpCurve(float inVal, T outVal)
        {
            InVal = inVal;
            OutVal = outVal;
            ArriveTangent = default(T);
            LeaveTangent = default(T);
            InterpMode = EInterpCurveMode.Linear;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inVal">input value that corresponds to this key</param>
        /// <param name="outVal">Output value of templated type</param>
        /// <param name="arriveTangent">Tangent of curve arriving at this point. </param>
        /// <param name="leaveTangent">Tangent of curve leaving from this point.</param>
        /// <param name="interpMode">interpolation mode to use</param>
        public FInterpCurve(float inVal, T outVal, T arriveTangent, T leaveTangent, EInterpCurveMode interpMode)
        {
            InVal = inVal;
            OutVal = outVal;
            ArriveTangent = arriveTangent;
            LeaveTangent = leaveTangent;
            InterpMode = interpMode;
        }

        /// <summary>
        /// Returns true if the key value is using a curve interp mode, otherwise false
        /// </summary>
        public bool IsCurveKey()
        {
            return
                InterpMode == EInterpCurveMode.CurveAuto ||
                InterpMode == EInterpCurveMode.CurveAutoClamped ||
                InterpMode == EInterpCurveMode.CurveUser ||
                InterpMode == EInterpCurveMode.CurveBreak;
        }

        public static bool operator ==(FInterpCurve<T> a, FInterpCurve<T> b)
        {
            return
                a.InVal == b.InVal &&
                EqualityComparer<T>.Default.Equals(a.OutVal, b.OutVal) &&
                EqualityComparer<T>.Default.Equals(a.ArriveTangent, b.ArriveTangent) &&
                EqualityComparer<T>.Default.Equals(a.LeaveTangent, b.LeaveTangent) &&
                a.InterpMode == b.InterpMode;
        }

        public static bool operator !=(FInterpCurve<T> a, FInterpCurve<T> b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FInterpCurve<T>))
            {
                return false;
            }

            return Equals((FInterpCurve<T>)obj);
        }

        public bool Equals(FInterpCurve<T> other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = InVal.GetHashCode();
                hashcode = (hashcode * 397) ^ OutVal.GetHashCode();
                hashcode = (hashcode * 397) ^ ArriveTangent.GetHashCode();
                hashcode = (hashcode * 397) ^ LeaveTangent.GetHashCode();
                hashcode = (hashcode * 397) ^ InterpMode.GetHashCode();
                return hashcode;
            }
        }
    }*/
}
