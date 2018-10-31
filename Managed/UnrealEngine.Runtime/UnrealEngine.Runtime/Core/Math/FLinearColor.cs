using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Color.h

    /// <summary>
    /// A linear, 32-bit/component floating point RGBA color.
    /// </summary>
    [UStruct(Flags = 0x0040EC38), BlueprintType, UMetaPath("/Script/CoreUObject.LinearColor", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FLinearColor : IEquatable<FLinearColor>
    {
        static bool R_IsValid;
        static int R_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.LinearColor:R")]
        public float R;

        static bool G_IsValid;
        static int G_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.LinearColor:G")]
        public float G;

        static bool B_IsValid;
        static int B_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.LinearColor:B")]
        public float B;

        static bool A_IsValid;
        static int A_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.LinearColor:A")]
        public float A;

        static int FLinearColor_StructSize;

        public FLinearColor Copy()
        {
            FLinearColor result = this;
            return result;
        }

        static FLinearColor()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FLinearColor)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FLinearColor));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.LinearColor");
            FLinearColor_StructSize = NativeReflection.GetStructSize(classAddress);
            R_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "R");
            R_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "R", Classes.UFloatProperty);
            G_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "G");
            G_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "G", Classes.UFloatProperty);
            B_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "B");
            B_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "B", Classes.UFloatProperty);
            A_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "A");
            A_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "A", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FLinearColor));
        }

        public FLinearColor(float r, float g, float b, float a = 1.0f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Converts an FColor which is assumed to be in sRGB space, into linear color space.
        /// </summary>
        /// <param name="color">The sRGB color that needs to be converted into linear space.</param>
        public FLinearColor(FColor color)
        {
            R = Tables.sRGBToLinearTable[color.R];
            G = Tables.sRGBToLinearTable[color.G];
            B = Tables.sRGBToLinearTable[color.B];
            A = (float)(color.A) * OneOver255;
        }

        public FLinearColor(FVector vector)
        {
            R = vector.X;
            G = vector.Y;
            B = vector.Z;
            A = 1.0f;
        }

        public FLinearColor(FVector4 vector)
        {
            R = vector.X;
            G = vector.Y;
            B = vector.Z;
            A = vector.W;
        }

        /// <summary>
        /// Convert from float to RGBE as outlined in Gregory Ward's Real Pixels article, Graphics Gems II, page 80.
        /// </summary>
        public FColor ToRGBE()
        {
            float primary = FMath.Max3(R, G, B);
            FColor color = default(FColor);

            if (primary < 1E-32)
            {
                color = default(FColor);
            }
            else
            {
                int Exponent;
                float Scale = (float)FMath.frexp(primary, out Exponent) / primary * 255.0f;
                
                color.R = (byte)FMath.Clamp(FMath.TruncToInt(R * Scale), 0, 255);
                color.G = (byte)FMath.Clamp(FMath.TruncToInt(G * Scale), 0, 255);
                color.B = (byte)FMath.Clamp(FMath.TruncToInt(B * Scale), 0, 255);
                color.A = (byte)(FMath.Clamp(FMath.TruncToInt(Exponent), -128, 127) + 128);
            }

            return color;
        }

        /// <summary>
        /// Converts an FColor coming from an observed sRGB output, into a linear color.
        /// </summary>
        /// <param name="color">The sRGB color that needs to be converted into linear space.</param>
        public static FLinearColor FromSRGBColor(FColor color)
        {
            FLinearColor linearColor;
            linearColor.R = Tables.sRGBToLinearTable[color.R];
            linearColor.G = Tables.sRGBToLinearTable[color.G];
            linearColor.B = Tables.sRGBToLinearTable[color.B];
            linearColor.A = (float)(color.A) * OneOver255;

            return linearColor;
        }

        /// <summary>
        /// Converts an FColor coming from an observed Pow(1/2.2) output, into a linear color.
        /// </summary>
        /// <param name="color">The Pow(1/2.2) color that needs to be converted into linear space.</param>
        public static FLinearColor FromPow22Color(FColor color)
        {
            FLinearColor linearColor;
            linearColor.R = Tables.Pow22OneOver255Table[color.R];
            linearColor.G = Tables.Pow22OneOver255Table[color.G];
            linearColor.B = Tables.Pow22OneOver255Table[color.B];
            linearColor.A = (float)(color.A) * OneOver255;

            return linearColor;
        }

        public float Component(int index)
        {
            return this[index];
        }
        
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return R;
                    case 1: return G;
                    case 2: return B;
                    case 3: return A;
                    default:
                        throw new IndexOutOfRangeException("Invalid FLinearColor index (" + index + ")");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: R = value; break;
                    case 1: G = value; break;
                    case 2: B = value; break;
                    case 3: A = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FLinearColor index (" + index + ")");
                }
            }
        }

        public static FLinearColor operator +(FLinearColor a, FLinearColor b)
        {
            return new FLinearColor(
                a.R + b.R,
                a.G + b.G,
                a.B + b.B,
                a.A + b.A);
        }

        public static FLinearColor operator -(FLinearColor a, FLinearColor b)
        {
            return new FLinearColor(
                a.R - b.R,
                a.G - b.G,
                a.B - b.B,
                a.A - b.A);
        }

        public static FLinearColor operator *(FLinearColor a, FLinearColor b)
        {
            return new FLinearColor(
                a.R * b.R,
                a.G * b.G,
                a.B * b.B,
                a.A * b.A);
        }

        public static FLinearColor operator *(float scale, FLinearColor a)
        {
            return new FLinearColor(
               a.R * scale,
               a.G * scale,
               a.B * scale,
               a.A * scale);
        }

        public static FLinearColor operator *(FLinearColor a, float scale)
        {
            return new FLinearColor(
                a.R * scale,
                a.G * scale,
                a.B * scale,
                a.A * scale);
        }

        public static FLinearColor operator /(FLinearColor a, FLinearColor b)
        {
            return new FLinearColor(
                a.R / b.R,
                a.G / b.G,
                a.B / b.B,
                a.A / b.A);
        }

        public static FLinearColor operator /(FLinearColor a, float scalar)
        {
            float invScalar = 1.0f / scalar;
            return new FLinearColor(
                a.R * invScalar,
                a.G * invScalar,
                a.B * invScalar,
                a.A * invScalar);
        }

        /// <summary>
        /// clamped in 0..1 range
        /// </summary>
        public FLinearColor GetClamped(float min = 0.0f, float max = 1.0f)
        {
            FLinearColor ret;

            ret.R = FMath.Clamp(R, min, max);
            ret.G = FMath.Clamp(G, min, max);
            ret.B = FMath.Clamp(B, min, max);
            ret.A = FMath.Clamp(A, min, max);

            return ret;
        }

        /// <summary>
        /// Compares two colors for equality.
        /// </summary>
        /// <param name="a">The first color.</param>
        /// <param name="b">The second color.</param>
        /// <returns>true if the colors are equal, false otherwise.</returns>
        public static bool operator ==(FLinearColor a, FLinearColor b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
        }

        /// <summary>
        /// Compare two colors for inequality.
        /// </summary>
        /// <param name="a">The first color.</param>
        /// <param name="b">The second color.</param>
        /// <returns>true if the colors are not equal, false otherwise.</returns>
        public static bool operator !=(FLinearColor a, FLinearColor b)
        {
            return a.R != b.R || a.G != b.G || a.B == b.B || a.A != b.A;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FLinearColor))
            {
                return false;
            }

            return Equals((FLinearColor)obj);
        }

        public bool Equals(FLinearColor other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = R.GetHashCode();
                hashcode = (hashcode * 397) ^ G.GetHashCode();
                hashcode = (hashcode * 397) ^ B.GetHashCode();
                hashcode = (hashcode * 397) ^ A.GetHashCode();
                return hashcode;
            }
        }

        /// <summary>
        /// Error-tolerant comparison.
        /// </summary>
        public bool Equals(FLinearColor other, float tolerance = FMath.KindaSmallNumber)
        {
            return 
                FMath.Abs(R - other.R) < tolerance && 
                FMath.Abs(G - other.G) < tolerance && 
                FMath.Abs(B - other.B) < tolerance && 
                FMath.Abs(A - other.A) < tolerance;
        }

        public FLinearColor CopyWithNewOpacity(float newOpacicty)
        {
            FLinearColor newCopy = this;
            newCopy.A = newOpacicty;
            return newCopy;
        }

        /// <summary>
        /// Converts byte hue-saturation-brightness to floating point red-green-blue.
        /// </summary>
        public static FLinearColor FGetHSV(byte h, byte s, byte v)
        {
            float brightness = v * 1.4f / 255.0f;
            brightness *= 0.7f / (0.01f + FMath.Sqrt(brightness));
            brightness = FMath.Clamp(brightness, 0.0f, 1.0f);
            FVector hue = (h < 86) ? 
                new FVector((85 - h) / 85.0f, (h - 0) / 85.0f, 0) : (h < 171) ? 
                new FVector(0, (170 - h) / 85.0f, (h - 85) / 85.0f) : new FVector((h - 170) / 85.0f, 0, (255 - h) / 84.0f);
            FVector colorVector = (hue + s / 255.0f * (new FVector(1, 1, 1) - hue)) * brightness;
            return new FLinearColor(colorVector.X, colorVector.Y, colorVector.Z, 1);
        }

        /// <summary>
        /// Makes a random but quite nice color.
        /// </summary>
        public static FLinearColor MakeRandomColor()
        {
            byte Hue = (byte)(FMath.FRand() * 255.0f);
            return FLinearColor.FGetHSV(Hue, 0, 255);
        }

        /// <summary>
        /// Converts temperature in Kelvins of a black body radiator to RGB chromaticity.
        /// </summary>
        public static FLinearColor MakeFromColorTemperature(float temp)
        {
            temp = FMath.Clamp(temp, 1000.0f, 15000.0f);

            // Approximate Planckian locus in CIE 1960 UCS
            float u = (0.860117757f + 1.54118254e-4f * temp + 1.28641212e-7f * temp * temp) / (1.0f + 8.42420235e-4f * temp + 7.08145163e-7f * temp * temp);
            float v = (0.317398726f + 4.22806245e-5f * temp + 4.20481691e-8f * temp * temp) / (1.0f - 2.89741816e-5f * temp + 1.61456053e-7f * temp * temp);

            float x = 3.0f * u / (2.0f * u - 8.0f * v + 4.0f);
            float y = 2.0f * v / (2.0f * u - 8.0f * v + 4.0f);
            float z = 1.0f - x - y;

            float Y = 1.0f;
            float X = Y / y * x;
            float Z = Y / y * z;

            // XYZ to RGB with BT.709 primaries
            float r = 3.2404542f * X + -1.5371385f * Y + -0.4985314f * Z;
            float g = -0.9692660f * X + 1.8760108f * Y + 0.0415560f * Z;
            float b = 0.0556434f * X + -0.2040259f * Y + 1.0572252f * Z;

            return new FLinearColor(r, g, b);
        }

        /// <summary>
        /// Euclidean distance between two points.
        /// </summary>
        public static float Dist(FLinearColor v1, FLinearColor v2)
        {
            return FMath.Sqrt(FMath.Square(v2.R - v1.R) + FMath.Square(v2.G - v1.G) + FMath.Square(v2.B - v1.B) + FMath.Square(v2.A - v1.A));
        }

        /// <summary>
        /// Generates a list of sample points on a Bezier curve defined by 2 points.
        /// </summary>
        /// <param name="controlPoints">Array of 4 Linear Colors (vert1, controlpoint1, controlpoint2, vert2).</param>
        /// <param name="numPoints">Number of samples.</param>
        /// <param name="points">Receives the output samples.</param>
        /// <returns>Path length.</returns>
        public static float EvaluateBezier(FLinearColor[] controlPoints, int numPoints, out FLinearColor[] points)
        {
            // Copy of FVector.EvaluateBezier

            Debug.Assert(controlPoints != null && controlPoints.Length >= 4);
            Debug.Assert(numPoints >= 2);

            points = new FLinearColor[numPoints];

            // var q is the change in t between successive evaluations.
            float q = 1.0f / (numPoints - 1); // q is dependent on the number of GAPS = POINTS-1

            // recreate the names used in the derivation
            FLinearColor p0 = controlPoints[0];
            FLinearColor p1 = controlPoints[1];
            FLinearColor p2 = controlPoints[2];
            FLinearColor p3 = controlPoints[3];

            // coefficients of the cubic polynomial that we're FDing -
            FLinearColor a = p0;
            FLinearColor b = 3 * (p1 - p0);
            FLinearColor c = 3 * (p2 - 2 * p1 + p0);
            FLinearColor d = p3 - 3 * p2 + 3 * p1 - p0;

            // initial values of the poly and the 3 diffs -
            FLinearColor s = a;						// the poly value
            FLinearColor u = b * q + c * q * q + d * q * q * q;	// 1st order diff (quadratic)
            FLinearColor v = 2 * c * q * q + 6 * d * q * q * q;	// 2nd order diff (linear)
            FLinearColor w = 6 * d * q * q * q;				// 3rd order diff (constant)

            // Path length.
            float length = 0.0f;

            FLinearColor oldPos = p0;
            points[0] = p0;// first point on the curve is always P0.

            for (int i = 1; i < numPoints; ++i)
            {
                // calculate the next value and update the deltas
                s += u;			// update poly value
                u += v;			// update 1st order diff value
                v += w;         // update 2st order diff value
                // 3rd order diff is constant => no update needed.

                // Update Length.
                length += FLinearColor.Dist(s, oldPos);
                oldPos = s;

                points[i] = s;
            }

            // Return path length as experienced in sequence (linear interpolation between points).
            return length;
        }

        /// <summary>
        /// Converts a linear space RGB color to an HSV color
        /// </summary>
        public FLinearColor LinearRGBToHSV()
        {
            float rgbMin = FMath.Min3(R, G, B);
            float rgbMax = FMath.Max3(R, G, B);
            float rgbRange = rgbMax - rgbMin;

            float hue = (rgbMax == rgbMin ? 0.0f :
                rgbMax == R ? FMath.Fmod((((G - B) / rgbRange) * 60.0f) + 360.0f, 360.0f) :
                rgbMax == G ? (((B - R) / rgbRange) * 60.0f) + 120.0f :
                rgbMax == B ? (((R - G) / rgbRange) * 60.0f) + 240.0f :
                0.0f);

            float saturation = (rgbMax == 0.0f ? 0.0f : rgbRange / rgbMax);
            float value = rgbMax;

            // In the new color, R = H, G = S, B = V, A = A
            return new FLinearColor(hue, saturation, value, A);
        }

        /// <summary>
        /// Converts an HSV color to a linear space RGB color
        /// </summary>
        public FLinearColor HSVToLinearRGB()
        {
            // In this color, R = H, G = S, B = V
            float hue = R;
            float saturation = G;
            float value = B;

            float hDiv60 = hue / 60.0f;
            float hDiv60_Floor = FMath.FloorToFloat(hDiv60);
            float hDiv60_Fraction = hDiv60 - hDiv60_Floor;

            float[] rgbValues = new float[4]
            {
                value,
                value * (1.0f - saturation),
                value * (1.0f - (hDiv60_Fraction * saturation)),
                value * (1.0f - ((1.0f - hDiv60_Fraction) * saturation)),
            };
            uint[,] rgbSwizzle = new uint[,]
            {
                { 0, 3, 1 },
                { 2, 0, 1 },
                { 1, 0, 3 },
                { 1, 2, 0 },
                { 3, 1, 0 },
                { 0, 1, 2 }
            };
            uint swizzleIndex = ((uint)hDiv60_Floor) % 6;

            return new FLinearColor(
                rgbValues[rgbSwizzle[swizzleIndex, 0]],
                rgbValues[rgbSwizzle[swizzleIndex, 1]],
                rgbValues[rgbSwizzle[swizzleIndex, 2]],
                A);
        }

        /// <summary>
        /// Linearly interpolates between two colors by the specified progress amount.  The interpolation is performed in HSV color space
        /// taking the shortest path to the new color's hue.  This can give better results than FMath::Lerp(), but is much more expensive.
        /// The incoming colors are in RGB space, and the output color will be RGB.  The alpha value will also be interpolated.
        /// </summary>
        /// <param name="from">The color and alpha to interpolate from as linear RGBA</param>
        /// <param name="to">The color and alpha to interpolate to as linear RGBA</param>
        /// <param name="progress">Scalar interpolation amount (usually between 0.0 and 1.0 inclusive)</param>
        /// <returns>The interpolated color in linear RGB space along with the interpolated alpha value</returns>
        public static FLinearColor LerpUsingHSV(FLinearColor from, FLinearColor to, float progress)
        {
            FLinearColor fromHSV = from.LinearRGBToHSV();
            FLinearColor toHSV = to.LinearRGBToHSV();

            float fromHue = fromHSV.R;
            float toHue = toHSV.R;

            // Take the shortest path to the new hue
            if (FMath.Abs(fromHue - toHue) > 180.0f)
            {
                if (toHue > fromHue)
                {
                    fromHue += 360.0f;
                }
                else
                {
                    toHue += 360.0f;
                }
            }

            float newHue = FMath.Lerp(fromHue, toHue, progress);

            newHue = FMath.Fmod(newHue, 360.0f);
            if (newHue < 0.0f)
            {
                newHue += 360.0f;
            }

            float newSaturation = FMath.Lerp(fromHSV.G, toHSV.G, progress);
            float newValue = FMath.Lerp(fromHSV.B, toHSV.B, progress);
            FLinearColor interpolated = new FLinearColor(newHue, newSaturation, newValue).HSVToLinearRGB();

            float newAlpha = FMath.Lerp(from.A, to.A, progress);
            interpolated.A = newAlpha;

            return interpolated;
        }

        /// <summary>
        /// Quantizes the linear color and returns the result as a FColor. This bypasses the SRGB conversion.
        /// </summary>
        public FColor Quantize()
        {
            return new FColor(
                (byte)FMath.Clamp(FMath.TruncToInt(R * 255.0f), 0, 255),
                (byte)FMath.Clamp(FMath.TruncToInt(G * 255.0f), 0, 255),
                (byte)FMath.Clamp(FMath.TruncToInt(B * 255.0f), 0, 255),
                (byte)FMath.Clamp(FMath.TruncToInt(A * 255.0f), 0, 255));
        }

        /// <summary>
        /// Quantizes the linear color with rounding and returns the result as a FColor.  This bypasses the SRGB conversion.
        /// </summary>
        public FColor QuantizeRound()
        {
            return new FColor(
                (byte)FMath.Clamp(FMath.RoundToInt(R * 255.0f), 0, 255),
                (byte)FMath.Clamp(FMath.RoundToInt(G * 255.0f), 0, 255),
                (byte)FMath.Clamp(FMath.RoundToInt(B * 255.0f), 0, 255),
                (byte)FMath.Clamp(FMath.RoundToInt(A * 255.0f), 0, 255));
        }

        /// <summary>
        /// Quantizes the linear color and returns the result as a FColor with optional sRGB conversion and quality as goal.
        /// </summary>
        public FColor ToFColor(bool srgb)
        {
            float FloatR = FMath.Clamp(R, 0.0f, 1.0f);
            float FloatG = FMath.Clamp(G, 0.0f, 1.0f);
            float FloatB = FMath.Clamp(B, 0.0f, 1.0f);
            float FloatA = FMath.Clamp(A, 0.0f, 1.0f);

            if (srgb)
            {
                FloatR = FloatR <= 0.0031308f ? FloatR * 12.92f : FMath.Pow(FloatR, 1.0f / 2.4f) * 1.055f - 0.055f;
                FloatG = FloatG <= 0.0031308f ? FloatG * 12.92f : FMath.Pow(FloatG, 1.0f / 2.4f) * 1.055f - 0.055f;
                FloatB = FloatB <= 0.0031308f ? FloatB * 12.92f : FMath.Pow(FloatB, 1.0f / 2.4f) * 1.055f - 0.055f;
            }

            FColor ret = default(FColor);

            ret.A = (byte)FMath.FloorToInt(FloatA * 255.999f);
            ret.R = (byte)FMath.FloorToInt(FloatR * 255.999f);
            ret.G = (byte)FMath.FloorToInt(FloatG * 255.999f);
            ret.B = (byte)FMath.FloorToInt(FloatB * 255.999f);

            return ret;
        }

        /// <summary>
        /// Returns a desaturated color, with 0 meaning no desaturation and 1 == full desaturation
        /// </summary>
        /// <param name="desaturation">Desaturation factor in range [0..1]</param>
        /// <returns>color</returns>
        public FLinearColor Desaturate(float desaturation)
        {
            float lum = ComputeLuminance();
            return FMath.Lerp(this, new FLinearColor(lum, lum, lum, 0), desaturation);
        }

        /// <summary>
        /// Computes the perceptually weighted luminance value of a color.
        /// </summary>
        public float ComputeLuminance()
        {
            return R * 0.3f + G * 0.59f + B * 0.11f;
        }

        /// <summary>
        /// Returns the maximum value in this color structure
        /// </summary>
        /// <returns>The maximum color channel value</returns>
        public float GetMax()
        {
            return FMath.Max(FMath.Max(FMath.Max(R, G), B), A);
        }

        /// <summary>
        /// useful to detect if a light contribution needs to be rendered
        /// </summary>
        public bool IsAlmostBlack()
        {
            return FMath.Square(R) < FMath.Delta && FMath.Square(G) < FMath.Delta && FMath.Square(B) < FMath.Delta;
        }

        /// <summary>
        /// Returns the minimum value in this color structure
        /// </summary>
        /// <returns>The minimum color channel value</returns>
        public float GetMin()
        {
            return FMath.Min(FMath.Min(FMath.Min(R, G), B), A);
        }

        public float GetLuminance()
        {
            return R * 0.3f + G * 0.59f + B * 0.11f;
        }

        public override string ToString()
        {
            return "(R=" + R + ",G=" + G + ",B=" + B + ",A=" + A + ")";
        }

        /// <summary>
        /// Initialize this Color based on an FString. The String is expected to contain R=, G=, B=, A=.
        /// The FLinearColor will be bogus when InitFromString returns false.
        /// </summary>
        /// <param name="sourceString">String containing the color values.</param>
        /// <returns>true if the R,G,B values were read successfully; false otherwise.</returns>
        public bool InitFromString(string sourceString)
        {
            R = G = B = 0.0f;
            A = 1.0f;

            // The initialization is only successful if the R, G, and B values can all be parsed from the string
            bool successful = 
                FParse.Value(sourceString, "R=", ref R) && 
                FParse.Value(sourceString, "G=", ref G) && 
                FParse.Value(sourceString, "B=", ref B);

            // Alpha is optional, so don't factor in its presence (or lack thereof) in determining initialization success
            FParse.Value(sourceString, "A=", ref A);

            return successful;
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FLinearColor Lerp(FLinearColor a, FLinearColor b, float alpha)
        {
            return FMath.Lerp(a, b, alpha);
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1.
        /// </summary>
        public static FLinearColor LerpStable(FLinearColor a, FLinearColor b, float alpha)
        {
            return FMath.Lerp(a, b, alpha);
        }

        /// <summary>
        /// Computes a brightness and a fixed point color from a floating point color.
        /// </summary>
        public static void ComputeAndFixedColorAndIntensity(FLinearColor linearColor, out FColor color, out float intensity)
        {
            float maxComponent = FMath.Max(FMath.Delta, FMath.Max(linearColor.R, FMath.Max(linearColor.G, linearColor.B)));
            color = (linearColor / maxComponent).ToFColor(true);
            intensity = maxComponent;
        }

        /// <summary>
        /// Helper used by FColor -> FLinearColor conversion. We don't use a lookup table as unlike pow, multiplication is fast.
        /// </summary>
        const float OneOver255 = 1.0f / 255.0f;

        public static readonly FLinearColor White = new FLinearColor(1.0f, 1.0f, 1.0f);
        public static readonly FLinearColor Gray = new FLinearColor(0.5f, 0.5f, 0.5f);
        public static readonly FLinearColor Black = new FLinearColor(0, 0, 0);
        public static readonly FLinearColor Transparent = new FLinearColor(0, 0, 0, 0);
        public static readonly FLinearColor Red = new FLinearColor(1.0f, 0, 0);
        public static readonly FLinearColor Green = new FLinearColor(0, 1.0f, 0);
        public static readonly FLinearColor Blue = new FLinearColor(0, 0, 1.0f);
        public static readonly FLinearColor Yellow = new FLinearColor(1.0f, 1.0f, 0);

        // Colors taken from XNA
        public static readonly FLinearColor TransparentBlack = new FLinearColor(FColor.TransparentBlack);
        //public static readonly FLinearColor Transparent = new FLinearColor(FColor.Transparent);
        public static readonly FLinearColor AliceBlue = new FLinearColor(FColor.AliceBlue);
        public static readonly FLinearColor AntiqueWhite = new FLinearColor(FColor.AntiqueWhite);
        public static readonly FLinearColor Aqua = new FLinearColor(FColor.Aqua);
        public static readonly FLinearColor Aquamarine = new FLinearColor(FColor.Aquamarine);
        public static readonly FLinearColor Azure = new FLinearColor(FColor.Azure);
        public static readonly FLinearColor Beige = new FLinearColor(FColor.Beige);
        public static readonly FLinearColor Bisque = new FLinearColor(FColor.Bisque);
        //public static readonly FLinearColor Black = new FLinearColor(FColor.Black);
        public static readonly FLinearColor BlanchedAlmond = new FLinearColor(FColor.BlanchedAlmond);
        //public static readonly FLinearColor Blue = new FLinearColor(FColor.Blue);
        public static readonly FLinearColor BlueViolet = new FLinearColor(FColor.BlueViolet);
        public static readonly FLinearColor Brown = new FLinearColor(FColor.Brown);
        public static readonly FLinearColor BurlyWood = new FLinearColor(FColor.BurlyWood);
        public static readonly FLinearColor CadetBlue = new FLinearColor(FColor.CadetBlue);
        public static readonly FLinearColor Chartreuse = new FLinearColor(FColor.Chartreuse);
        public static readonly FLinearColor Chocolate = new FLinearColor(FColor.Chocolate);
        public static readonly FLinearColor Coral = new FLinearColor(FColor.Coral);
        public static readonly FLinearColor CornflowerBlue = new FLinearColor(FColor.CornflowerBlue);
        public static readonly FLinearColor Cornsilk = new FLinearColor(FColor.Cornsilk);
        public static readonly FLinearColor Crimson = new FLinearColor(FColor.Crimson);
        public static readonly FLinearColor Cyan = new FLinearColor(FColor.Cyan);
        public static readonly FLinearColor DarkBlue = new FLinearColor(FColor.DarkBlue);
        public static readonly FLinearColor DarkCyan = new FLinearColor(FColor.DarkCyan);
        public static readonly FLinearColor DarkGoldenrod = new FLinearColor(FColor.DarkGoldenrod);
        public static readonly FLinearColor DarkGray = new FLinearColor(FColor.DarkGray);
        public static readonly FLinearColor DarkGreen = new FLinearColor(FColor.DarkGreen);
        public static readonly FLinearColor DarkKhaki = new FLinearColor(FColor.DarkKhaki);
        public static readonly FLinearColor DarkMagenta = new FLinearColor(FColor.DarkMagenta);
        public static readonly FLinearColor DarkOliveGreen = new FLinearColor(FColor.DarkOliveGreen);
        public static readonly FLinearColor DarkOrange = new FLinearColor(FColor.DarkOrange);
        public static readonly FLinearColor DarkOrchid = new FLinearColor(FColor.DarkOrchid);
        public static readonly FLinearColor DarkRed = new FLinearColor(FColor.DarkRed);
        public static readonly FLinearColor DarkSalmon = new FLinearColor(FColor.DarkSalmon);
        public static readonly FLinearColor DarkSeaGreen = new FLinearColor(FColor.DarkSeaGreen);
        public static readonly FLinearColor DarkSlateBlue = new FLinearColor(FColor.DarkSlateBlue);
        public static readonly FLinearColor DarkSlateGray = new FLinearColor(FColor.DarkSlateGray);
        public static readonly FLinearColor DarkTurquoise = new FLinearColor(FColor.DarkTurquoise);
        public static readonly FLinearColor DarkViolet = new FLinearColor(FColor.DarkViolet);
        public static readonly FLinearColor DeepPink = new FLinearColor(FColor.DeepPink);
        public static readonly FLinearColor DeepSkyBlue = new FLinearColor(FColor.DeepSkyBlue);
        public static readonly FLinearColor DimGray = new FLinearColor(FColor.DimGray);
        public static readonly FLinearColor DodgerBlue = new FLinearColor(FColor.DodgerBlue);
        public static readonly FLinearColor Firebrick = new FLinearColor(FColor.Firebrick);
        public static readonly FLinearColor FloralWhite = new FLinearColor(FColor.FloralWhite);
        public static readonly FLinearColor ForestGreen = new FLinearColor(FColor.ForestGreen);
        public static readonly FLinearColor Fuchsia = new FLinearColor(FColor.Fuchsia);
        public static readonly FLinearColor Gainsboro = new FLinearColor(FColor.Gainsboro);
        public static readonly FLinearColor GhostWhite = new FLinearColor(FColor.GhostWhite);
        public static readonly FLinearColor Gold = new FLinearColor(FColor.Gold);
        public static readonly FLinearColor Goldenrod = new FLinearColor(FColor.Goldenrod);
        //public static readonly FLinearColor Gray = new FLinearColor(FColor.Gray);
        //public static readonly FLinearColor Green = new FLinearColor(FColor.Green);
        public static readonly FLinearColor GreenYellow = new FLinearColor(FColor.GreenYellow);
        public static readonly FLinearColor Honeydew = new FLinearColor(FColor.Honeydew);
        public static readonly FLinearColor HotPink = new FLinearColor(FColor.HotPink);
        public static readonly FLinearColor IndianRed = new FLinearColor(FColor.IndianRed);
        public static readonly FLinearColor Indigo = new FLinearColor(FColor.Indigo);
        public static readonly FLinearColor Ivory = new FLinearColor(FColor.Ivory);
        public static readonly FLinearColor Khaki = new FLinearColor(FColor.Khaki);
        public static readonly FLinearColor Lavender = new FLinearColor(FColor.Lavender);
        public static readonly FLinearColor LavenderBlush = new FLinearColor(FColor.LavenderBlush);
        public static readonly FLinearColor LawnGreen = new FLinearColor(FColor.LawnGreen);
        public static readonly FLinearColor LemonChiffon = new FLinearColor(FColor.LemonChiffon);
        public static readonly FLinearColor LightBlue = new FLinearColor(FColor.LightBlue);
        public static readonly FLinearColor LightCoral = new FLinearColor(FColor.LightCoral);
        public static readonly FLinearColor LightCyan = new FLinearColor(FColor.LightCyan);
        public static readonly FLinearColor LightGoldenrodYellow = new FLinearColor(FColor.LightGoldenrodYellow);
        public static readonly FLinearColor LightGray = new FLinearColor(FColor.LightGray);
        public static readonly FLinearColor LightGreen = new FLinearColor(FColor.LightGreen);
        public static readonly FLinearColor LightPink = new FLinearColor(FColor.LightPink);
        public static readonly FLinearColor LightSalmon = new FLinearColor(FColor.LightSalmon);
        public static readonly FLinearColor LightSeaGreen = new FLinearColor(FColor.LightSeaGreen);
        public static readonly FLinearColor LightSkyBlue = new FLinearColor(FColor.LightSkyBlue);
        public static readonly FLinearColor LightSlateGray = new FLinearColor(FColor.LightSlateGray);
        public static readonly FLinearColor LightSteelBlue = new FLinearColor(FColor.LightSteelBlue);
        public static readonly FLinearColor LightYellow = new FLinearColor(FColor.LightYellow);
        public static readonly FLinearColor Lime = new FLinearColor(FColor.Lime);
        public static readonly FLinearColor LimeGreen = new FLinearColor(FColor.LimeGreen);
        public static readonly FLinearColor Linen = new FLinearColor(FColor.Linen);
        public static readonly FLinearColor Magenta = new FLinearColor(FColor.Magenta);
        public static readonly FLinearColor Maroon = new FLinearColor(FColor.Maroon);
        public static readonly FLinearColor MediumAquamarine = new FLinearColor(FColor.MediumAquamarine);
        public static readonly FLinearColor MediumBlue = new FLinearColor(FColor.MediumBlue);
        public static readonly FLinearColor MediumOrchid = new FLinearColor(FColor.MediumOrchid);
        public static readonly FLinearColor MediumPurple = new FLinearColor(FColor.MediumPurple);
        public static readonly FLinearColor MediumSeaGreen = new FLinearColor(FColor.MediumSeaGreen);
        public static readonly FLinearColor MediumSlateBlue = new FLinearColor(FColor.MediumSlateBlue);
        public static readonly FLinearColor MediumSpringGreen = new FLinearColor(FColor.MediumSpringGreen);
        public static readonly FLinearColor MediumTurquoise = new FLinearColor(FColor.MediumTurquoise);
        public static readonly FLinearColor MediumVioletRed = new FLinearColor(FColor.MediumVioletRed);
        public static readonly FLinearColor MidnightBlue = new FLinearColor(FColor.MidnightBlue);
        public static readonly FLinearColor MintCream = new FLinearColor(FColor.MintCream);
        public static readonly FLinearColor MistyRose = new FLinearColor(FColor.MistyRose);
        public static readonly FLinearColor Moccasin = new FLinearColor(FColor.Moccasin);
        public static readonly FLinearColor MonoGameOrange = new FLinearColor(FColor.MonoGameOrange);
        public static readonly FLinearColor NavajoWhite = new FLinearColor(FColor.NavajoWhite);
        public static readonly FLinearColor Navy = new FLinearColor(FColor.Navy);
        public static readonly FLinearColor OldLace = new FLinearColor(FColor.OldLace);
        public static readonly FLinearColor Olive = new FLinearColor(FColor.Olive);
        public static readonly FLinearColor OliveDrab = new FLinearColor(FColor.OliveDrab);
        public static readonly FLinearColor Orange = new FLinearColor(FColor.Orange);
        public static readonly FLinearColor OrangeRed = new FLinearColor(FColor.OrangeRed);
        public static readonly FLinearColor Orchid = new FLinearColor(FColor.Orchid);
        public static readonly FLinearColor PaleGoldenrod = new FLinearColor(FColor.PaleGoldenrod);
        public static readonly FLinearColor PaleGreen = new FLinearColor(FColor.PaleGreen);
        public static readonly FLinearColor PaleTurquoise = new FLinearColor(FColor.PaleTurquoise);
        public static readonly FLinearColor PaleVioletRed = new FLinearColor(FColor.PaleVioletRed);
        public static readonly FLinearColor PapayaWhip = new FLinearColor(FColor.PapayaWhip);
        public static readonly FLinearColor PeachPuff = new FLinearColor(FColor.PeachPuff);
        public static readonly FLinearColor Peru = new FLinearColor(FColor.Peru);
        public static readonly FLinearColor Pink = new FLinearColor(FColor.Pink);
        public static readonly FLinearColor Plum = new FLinearColor(FColor.Plum);
        public static readonly FLinearColor PowderBlue = new FLinearColor(FColor.PowderBlue);
        public static readonly FLinearColor Purple = new FLinearColor(FColor.Purple);
        //public static readonly FLinearColor Red = new FLinearColor(FColor.Red);
        public static readonly FLinearColor RosyBrown = new FLinearColor(FColor.RosyBrown);
        public static readonly FLinearColor RoyalBlue = new FLinearColor(FColor.RoyalBlue);
        public static readonly FLinearColor SaddleBrown = new FLinearColor(FColor.SaddleBrown);
        public static readonly FLinearColor Salmon = new FLinearColor(FColor.Salmon);
        public static readonly FLinearColor SandyBrown = new FLinearColor(FColor.SandyBrown);
        public static readonly FLinearColor SeaGreen = new FLinearColor(FColor.SeaGreen);
        public static readonly FLinearColor SeaShell = new FLinearColor(FColor.SeaShell);
        public static readonly FLinearColor Sienna = new FLinearColor(FColor.Sienna);
        public static readonly FLinearColor Silver = new FLinearColor(FColor.Silver);
        public static readonly FLinearColor SkyBlue = new FLinearColor(FColor.SkyBlue);
        public static readonly FLinearColor SlateBlue = new FLinearColor(FColor.SlateBlue);
        public static readonly FLinearColor SlateGray = new FLinearColor(FColor.SlateGray);
        public static readonly FLinearColor Snow = new FLinearColor(FColor.Snow);
        public static readonly FLinearColor SpringGreen = new FLinearColor(FColor.SpringGreen);
        public static readonly FLinearColor SteelBlue = new FLinearColor(FColor.SteelBlue);
        public static readonly FLinearColor Tan = new FLinearColor(FColor.Tan);
        public static readonly FLinearColor Teal = new FLinearColor(FColor.Teal);
        public static readonly FLinearColor Thistle = new FLinearColor(FColor.Thistle);
        public static readonly FLinearColor Tomato = new FLinearColor(FColor.Tomato);
        public static readonly FLinearColor Turquoise = new FLinearColor(FColor.Turquoise);
        public static readonly FLinearColor Violet = new FLinearColor(FColor.Violet);
        public static readonly FLinearColor Wheat = new FLinearColor(FColor.Wheat);
        //public static readonly FLinearColor White = new FLinearColor(FColor.White);
        public static readonly FLinearColor WhiteSmoke = new FLinearColor(FColor.WhiteSmoke);
        //public static readonly FLinearColor Yellow = new FLinearColor(FColor.Yellow);
        public static readonly FLinearColor YellowGreen = new FLinearColor(FColor.YellowGreen);

        class Tables
        {
            /// <summary>
            /// Pow table for fast FColor -> FLinearColor conversion.
            /// 
            /// FMath::Pow( i / 255.f, 2.2f )
            /// </summary>
            public static readonly float[] Pow22OneOver255Table = new float[256]
            {
                0f, 5.07705190066176E-06f, 2.33280046660989E-05f, 5.69217657121931E-05f, 0.000107187362341244f, 0.000175123977503027f, 0.000261543754548491f, 0.000367136269815943f, 0.000492503787191433f,
                0.000638182842167022f, 0.000804658499513058f, 0.000992374304074325f, 0.0012017395224384f, 0.00143313458967186f, 0.00168691531678928f, 0.00196341621339647f, 0.00226295316070643f,
                0.00258582559623417f, 0.00293231832393836f, 0.00330270303200364f, 0.00369723957890013f, 0.00411617709328275f, 0.00455975492252602f, 0.00502820345685554f, 0.00552174485023966f,
                0.00604059365484981f, 0.00658495738258168f, 0.00715503700457303f, 0.00775102739766061f, 0.00837311774514858f, 0.00902149189801213f, 0.00969632870165823f, 0.0103978022925553f,
                0.0111260823683832f, 0.0118813344348137f, 0.0126637200315821f, 0.0134733969401426f, 0.0143105193748841f, 0.0151752381596252f, 0.0160677008908869f, 0.01698805208925f, 0.0179364333399502f,
                0.0189129834237215f, 0.0199178384387857f, 0.0209511319147811f, 0.0220129949193365f, 0.0231035561579214f, 0.0242229420675342f, 0.0253712769047346f, 0.0265486828284729f, 0.027755279978126f,
                0.0289911865471078f, 0.0302565188523887f, 0.0315513914002264f, 0.0328759169483838f, 0.034230206565082f, 0.0356143696849188f, 0.0370285141619602f, 0.0384727463201946f, 0.0399471710015256f,
                0.0414518916114625f, 0.0429870101626571f, 0.0445526273164214f, 0.0461488424223509f, 0.0477757535561706f, 0.049433457555908f, 0.0511220500564934f, 0.052841625522879f, 0.0545922772817603f,
                0.0563740975519798f, 0.0581871774736854f, 0.0600316071363132f, 0.0619074756054558f, 0.0638148709486772f, 0.0657538802603301f, 0.0677245896854243f, 0.0697270844425988f, 0.0717614488462391f,
                0.0738277663277846f, 0.0759261194562648f, 0.0780565899581019f, 0.080219258736215f, 0.0824142058884592f, 0.0846415107254295f, 0.0869012517876603f, 0.0891935068622478f, 0.0915183529989195f,
                0.0938758665255778f, 0.0962661230633397f, 0.0986891975410945f, 0.1011451642096f, 0.103634096655137f, 0.106156067812744f, 0.108711149979039f, 0.11129941482466f, 0.113920933406333f,
                0.116575776178572f, 0.119264013005047f, 0.121985713169619f, 0.124740945387051f, 0.127529777813422f, 0.130352278056244f, 0.1332085131843f, 0.136098549737202f, 0.139022453734703f,
                0.141980290685736f, 0.144972125597231f, 0.147998022982685f, 0.151058046870511f, 0.154152260812165f, 0.157280727890073f, 0.160443510725344f, 0.16364067148529f, 0.166872271890766f,
                0.170138373223312f, 0.173439036332135f, 0.176774321640903f, 0.18014428915439f, 0.183548998464951f, 0.186988508758844f, 0.190462878822409f, 0.193972167048093f, 0.19751643144034f,
                0.201095729621346f, 0.204710118836677f, 0.208359655960767f, 0.212044397502288f, 0.215764399609395f, 0.219519718074868f, 0.223310408341127f, 0.227136525505149f, 0.230998124323267f,
                0.23489525921588f, 0.238827984272048f, 0.242796353254002f, 0.24680041960155f, 0.2508402364364f, 0.254915856566385f, 0.259027332489606f, 0.263174716398492f, 0.267358060183772f,
                0.271577415438375f, 0.275832833461245f, 0.280124365261085f, 0.284452061560024f, 0.288815972797219f, 0.293216149132375f, 0.297652640449211f, 0.302125496358853f, 0.306634766203158f,
                0.311180499057984f, 0.315762743736397f, 0.32038154879181f, 0.325036962521076f, 0.329729032967515f, 0.334457807923889f, 0.339223334935327f, 0.344025661302187f, 0.348864834082879f,
                0.353740900096629f, 0.358653905926199f, 0.363603897920553f, 0.368590922197487f, 0.373615024646202f, 0.37867625092984f, 0.383774646487975f, 0.388910256539059f, 0.394083126082829f,
                0.399293299902674f, 0.404540822567962f, 0.409825738436323f, 0.415148091655907f, 0.420507926167587f, 0.425905285707146f, 0.43134021380741f, 0.436812753800359f, 0.442322948819202f,
                0.44787084180041f, 0.453456475485731f, 0.45907989242416f, 0.46474113497389f, 0.470440245304218f, 0.47617726539744f, 0.481952237050698f, 0.487765201877811f, 0.493616201311074f,
                0.49950527660303f, 0.505432468828216f, 0.511397818884879f, 0.517401367496673f, 0.523443155214325f, 0.529523222417277f, 0.535641609315311f, 0.541798355950137f, 0.547993502196972f,
                0.554227087766085f, 0.560499152204328f, 0.566809734896638f, 0.573158875067523f, 0.579546611782525f, 0.585972983949661f, 0.592438030320847f, 0.598941789493296f, 0.605484299910907f,
                0.612065599865624f, 0.61868572749878f, 0.625344720802427f, 0.632042617620641f, 0.638779455650817f, 0.645555272444934f, 0.652370105410821f, 0.659223991813387f, 0.666116968775851f,
                0.673049073280942f, 0.680020342172095f, 0.687030812154625f, 0.694080519796882f, 0.701169501531402f, 0.708297793656032f, 0.715465432335048f, 0.722672453600255f, 0.729918893352071f,
                0.737204787360605f, 0.744530171266715f, 0.751895080583051f, 0.759299550695091f, 0.766743616862161f, 0.774227314218442f, 0.781750677773962f, 0.789313742415586f, 0.796916542907978f,
                0.804559113894567f, 0.81224148989849f, 0.819963705323528f, 0.827725794455034f, 0.835527791460841f, 0.843369730392169f, 0.851251645184515f, 0.859173569658532f, 0.867135537520905f,
                0.875137582365205f, 0.883179737672745f, 0.891262036813419f, 0.899384513046529f, 0.907547199521614f, 0.915750129279253f, 0.923993335251873f, 0.932276850264543f, 0.940600707035753f,
                0.948964938178195f, 0.957369576199527f, 0.96581465350313f, 0.974300202388861f, 0.982826255053791f, 0.99139284359294f, 1f
            };

            /// <summary>
            /// Table for fast FColor -> FLinearColor conversion.
            /// 
            /// Color > 0.04045 ? pow( Color * (1.0 / 1.055) + 0.0521327, 2.4 ) : Color * (1.0 / 12.92);
            /// </summary>
            public static readonly float[] sRGBToLinearTable = new float[256]
            {
                0,
                0.000303526983548838f, 0.000607053967097675f, 0.000910580950646512f, 0.00121410793419535f, 0.00151763491774419f,
                0.00182116190129302f, 0.00212468888484186f, 0.0024282158683907f, 0.00273174285193954f, 0.00303526983548838f,
                0.00334653564113713f, 0.00367650719436314f, 0.00402471688178252f, 0.00439144189356217f, 0.00477695332960869f,
                0.005181516543916f, 0.00560539145834456f, 0.00604883284946662f, 0.00651209061157708f, 0.00699540999852809f,
                0.00749903184667767f, 0.00802319278093555f, 0.0085681254056307f, 0.00913405848170623f, 0.00972121709156193f,
                0.0103298227927056f, 0.0109600937612386f, 0.0116122449260844f, 0.012286488094766f, 0.0129830320714536f,
                0.0137020827679224f, 0.0144438433080002f, 0.0152085141260192f, 0.0159962930597398f, 0.0168073754381669f,
                0.0176419541646397f, 0.0185002197955389f, 0.0193823606149269f, 0.0202885627054049f, 0.0212190100154473f,
                0.0221738844234532f, 0.02315336579873f, 0.0241576320596103f, 0.0251868592288862f, 0.0262412214867272f,
                0.0273208912212394f, 0.0284260390768075f, 0.0295568340003534f, 0.0307134432856324f, 0.0318960326156814f,
                0.0331047661035236f, 0.0343398063312275f, 0.0356013143874111f, 0.0368894499032755f, 0.0382043710872463f,
                0.0395462347582974f, 0.0409151963780232f, 0.0423114100815264f, 0.0437350287071788f, 0.0451862038253117f,
                0.0466650857658898f, 0.0481718236452158f, 0.049706565391714f, 0.0512694577708345f, 0.0528606464091205f,
                0.0544802758174765f, 0.0561284894136735f, 0.0578054295441256f, 0.0595112375049707f, 0.0612460535624849f,
                0.0630100169728596f, 0.0648032660013696f, 0.0666259379409563f, 0.0684781691302512f, 0.070360094971063f,
                0.0722718499453493f, 0.0742135676316953f, 0.0761853807213167f, 0.0781874210336082f, 0.0802198195312533f,
                0.0822827063349132f, 0.0843762107375113f, 0.0865004612181274f, 0.0886555854555171f, 0.0908417103412699f,
                0.0930589619926197f, 0.0953074657649191f, 0.0975873462637915f, 0.0998987273569704f, 0.102241732185838f,
                0.104616483176675f, 0.107023102051626f, 0.109461709839399f, 0.1119324268857f, 0.114435372863418f,
                0.116970666782559f, 0.119538426999953f, 0.122138771228724f, 0.124771816547542f, 0.127437679409664f,
                0.130136475651761f, 0.132868320502552f, 0.135633328591233f, 0.138431613955729f, 0.141263290050755f,
                0.144128469755705f, 0.147027265382362f, 0.149959788682454f, 0.152926150855031f, 0.155926462553701f,
                0.158960833893705f, 0.162029374458845f, 0.16513219330827f, 0.168269398983119f, 0.171441099513036f,
                0.174647402422543f, 0.17788841473729f, 0.181164242990184f, 0.184474993227387f, 0.187820771014205f,
                0.191201681440861f, 0.194617829128147f, 0.198069318232982f, 0.201556252453853f, 0.205078735036156f,
                0.208636868777438f, 0.212230756032542f, 0.215860498718652f, 0.219526198320249f, 0.223227955893977f,
                0.226965872073417f, 0.23074004707378f, 0.23455058069651f, 0.238397572333811f, 0.242281120973093f,
                0.246201325201334f, 0.250158283209375f, 0.254152092796134f, 0.258182851372752f, 0.262250655966664f,
                0.266355603225604f, 0.270497789421545f, 0.274677310454565f, 0.278894261856656f, 0.283148738795466f,
                0.287440836077983f, 0.291770648154158f, 0.296138269120463f, 0.300543792723403f, 0.304987312362961f,
                0.309468921095997f, 0.313988711639584f, 0.3185467763743f, 0.323143207347467f, 0.32777809627633f,
                0.332451534551205f, 0.337163613238559f, 0.341914423084057f, 0.346704054515559f, 0.351532597646068f,
                0.356400142276637f, 0.361306777899234f, 0.36625259369956f, 0.371237678559833f, 0.376262121061519f,
                0.381326009488037f, 0.386429431827418f, 0.39157247577492f, 0.396755228735618f, 0.401977777826949f,
                0.407240209881218f, 0.41254261144808f, 0.417885068796976f, 0.423267667919539f, 0.428690494531971f,
                0.434153634077377f, 0.439657171728079f, 0.445201192387887f, 0.450785780694349f, 0.456411021020965f,
                0.462076997479369f, 0.467783793921492f, 0.473531493941681f, 0.479320180878805f, 0.485149937818323f,
                0.491020847594331f, 0.496932992791578f, 0.502886455747457f, 0.50888131855397f, 0.514917663059676f,
                0.520995570871595f, 0.527115123357109f, 0.533276401645826f, 0.539479486631421f, 0.545724458973463f,
                0.552011399099209f, 0.558340387205378f, 0.56471150325991f, 0.571124827003694f, 0.577580437952282f,
                0.584078415397575f, 0.590618838409497f, 0.597201785837643f, 0.603827336312907f, 0.610495568249093f,
                0.617206559844509f, 0.623960389083534f, 0.630757133738175f, 0.637596871369601f, 0.644479679329661f,
                0.651405634762384f, 0.658374814605461f, 0.665387295591707f, 0.672443154250516f, 0.679542466909286f,
                0.686685309694841f, 0.693871758534824f, 0.701101889159085f, 0.708375777101046f, 0.71569349769906f,
                0.723055126097739f, 0.730460737249286f, 0.737910405914797f, 0.745404206665559f, 0.752942213884326f,
                0.760524501766589f, 0.768151144321824f, 0.775822215374732f, 0.783537788566466f, 0.791297937355839f,
                0.799102735020525f, 0.806952254658248f, 0.81484656918795f, 0.822785751350956f, 0.830769873712124f,
                0.838799008660978f, 0.846873228412837f, 0.854992605009927f, 0.863157210322481f, 0.871367116049835f,
                0.879622393721502f, 0.887923114698241f, 0.896269350173118f, 0.904661171172551f, 0.913098648557343f,
                0.921581853023715f, 0.930110855104312f, 0.938685725169219f, 0.947306533426946f, 0.955973349925421f,
                0.964686244552961f, 0.973445287039244f, 0.982250546956257f, 0.991102093719252f, 1.0f,
            };
        }
    }


}
