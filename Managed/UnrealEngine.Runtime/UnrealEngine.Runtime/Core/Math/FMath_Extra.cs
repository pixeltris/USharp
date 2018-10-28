using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Extra methods which aren't part of the native FMath class

    public static partial class FMath
    {
        public static double lpexp(double x, int exp)
        {
            return x * Math.Pow(2, exp);
        }

        public static double frexp(double x, out int exp)
        {
            exp = (int)Math.Floor(Math.Log(x) / Math.Log(2)) + 1;
            return 1 - (Math.Pow(2, exp) - x) / Math.Pow(2, exp);
        }
    }
}
