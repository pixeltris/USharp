using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Color.h

    // NOTE: On little endian machines this is BGRA, on big endian machines this is ARGB

    /// <summary>
    /// A Color (BGRA).
    /// Stores a color with 8 bits of precision per channel.
    /// Note: Linear color values should always be converted to gamma space before stored in an FColor, as 8 bits of precision is not enough to store linear space colors!
    /// This can be done with FLinearColor::ToFColor(true)
    /// </summary>
    [UStruct(Flags = 0x0000E838), BlueprintType, UMetaPath("/Script/CoreUObject.Color", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FColor : IEquatable<FColor>
    {
        private uint packedValue;

        public byte B
        {
            get
            {
                if (BitConverter.IsLittleEndian)
                {
                    return (byte)(packedValue >> 24);
                }
                else
                {
                    return (byte)(packedValue);
                }
            }
            set
            {
                if (BitConverter.IsLittleEndian)
                {
                    packedValue = (packedValue & 0x00FFFFFF) | (uint)(value << 24);
                }
                else
                {
                    packedValue = (packedValue & 0xFFFFFF00) | value;
                }
            }
        }

        public byte G
        {
            get
            {
                if (BitConverter.IsLittleEndian)
                {
                    return (byte)(packedValue >> 16);
                }
                else
                {
                    return (byte)(packedValue >> 8);
                }
            }
            set
            {
                if (BitConverter.IsLittleEndian)
                {
                    packedValue = (packedValue & 0xFF00FFFF) | (uint)(value << 16);
                }
                else
                {
                    packedValue = (packedValue & 0xFFFF00FF) | (uint)(value << 8);
                }
            }
        }

        public byte R
        {
            get
            {
                if (BitConverter.IsLittleEndian)
                {
                    return (byte)(packedValue >> 8);
                }
                else
                {
                    return (byte)(packedValue >> 16);
                }
            }
            set
            {
                if (BitConverter.IsLittleEndian)
                {
                    packedValue = (packedValue & 0xFFFF00FF) | (uint)(value << 8);
                }
                else
                {
                    packedValue = (packedValue & 0xFF00FFFF) | (uint)(value << 16);
                }
            }
        }

        public byte A
        {
            get
            {
                if (BitConverter.IsLittleEndian)
                {
                    return (byte)(packedValue);
                }
                else
                {
                    return (byte)(packedValue >> 24);
                }
            }
            set
            {
                if (BitConverter.IsLittleEndian)
                {
                    packedValue = (packedValue & 0xFFFFFF00) | value;
                }
                else
                {
                    packedValue = (packedValue & 0x00FFFFFF) | (uint)(value << 24);
                }
            }
        }

        static bool B_IsValid;
        static int B_Offset;
        //[UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Color:B")]
        //public byte B;

        static bool G_IsValid;
        static int G_Offset;
        //[UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Color:G")]
        //public byte G;

        static bool R_IsValid;
        static int R_Offset;
        //[UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Color:R")]
        //public byte R;

        static bool A_IsValid;
        static int A_Offset;
        //[UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Color:A")]
        //public byte A;

        public uint PackedValue
        {
            get
            {
                return packedValue;
            }
            set
            {
                packedValue = value;
            }
        }

        static int FColor_StructSize;

        public FColor Copy()
        {
            FColor result = this;
            return result;
        }

        static FColor()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FColor)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FColor));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Color");
            FColor_StructSize = NativeReflection.GetStructSize(classAddress);
            B_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "B");
            B_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "B", Classes.UByteProperty);
            G_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "G");
            G_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "G", Classes.UByteProperty);
            R_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "R");
            R_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "R", Classes.UByteProperty);
            A_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "A");
            A_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "A", Classes.UByteProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FColor));
        }

        public FColor(byte r, byte g, byte b, byte a = 255)
        {
            if (BitConverter.IsLittleEndian)
            {
                packedValue = (uint)((b << 24) | (g << 16) | (r << 8) | (a));
            }
            else
            {
                packedValue = (uint)((a << 24) | (r << 16) | (g << 8) | (b));
            }
        }

        public FColor(uint color)
        {
            packedValue = color;
        }

        /// <summary>
        /// Compares two colors for equality.
        /// </summary>
        /// <param name="a">The first color.</param>
        /// <param name="b">The second color.</param>
        /// <returns>true if the colors are equal, false otherwise.</returns>
        public static bool operator ==(FColor a, FColor b)
        {
            return a.packedValue == b.packedValue;
        }

        /// <summary>
        /// Compare two colors for inequality.
        /// </summary>
        /// <param name="a">The first color.</param>
        /// <param name="b">The second color.</param>
        /// <returns>true if the colors are not equal, false otherwise.</returns>
        public static bool operator !=(FColor a, FColor b)
        {
            return a.packedValue != b.packedValue;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FColor))
            {
                return false;
            }

            return Equals((FColor)obj);
        }

        public bool Equals(FColor other)
        {
            return packedValue == other.packedValue;
        }

        public override int GetHashCode()
        {
            return packedValue.GetHashCode();
        }

        public static FColor operator +(FColor a, FColor b)
        {
            return new FColor(
                (byte)Math.Min((int)a.R + (int)b.R, 255),
                (byte)Math.Min((int)a.G + (int)b.G, 255),
                (byte)Math.Min((int)a.B + (int)b.B, 255),
                (byte)Math.Min((int)a.A + (int)b.A, 255));
        }

        /// <summary>
        /// Convert from RGBE to float as outlined in Gregory Ward's Real Pixels article, Graphics Gems II, page 80.
        /// </summary>
        public FLinearColor FromRGBE()
        {
            if (A == 0)
            {
                return FLinearColor.Black;
            }
            else
            {
                float scale = (float)FMath.lpexp(1 / 255.0, A - 128);
                return new FLinearColor(R * scale, G * scale, B * scale, 1.0f);
            }
        }

        /// <summary>
        /// Creates a color value from the given hexadecimal string.
        /// 
        /// Supported formats are: RGB, RRGGBB, RRGGBBAA, #RGB, #RRGGBB, #RRGGBBAA
        /// </summary>
        /// <param name="hexString">The hexadecimal string.</param>
        /// <returns>The corresponding color value.</returns>
        /// <see cref="ToHex"/>
        public static FColor FromHex(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
            {
                return default(FColor);
            }

            int StartIndex = (hexString[0] == '#') ? 1 : 0;

            if (hexString.Length == 3 + StartIndex)
            {
                int R = FParse.HexDigit(hexString[StartIndex++]);
                int G = FParse.HexDigit(hexString[StartIndex++]);
                int B = FParse.HexDigit(hexString[StartIndex]);

                return new FColor((byte)((R << 4) + R), (byte)((G << 4) + G), (byte)((B << 4) + B), 255);
            }

            if (hexString.Length == 6 + StartIndex)
            {
                FColor result = default(FColor);

                result.R = (byte)((FParse.HexDigit(hexString[StartIndex + 0]) << 4) + FParse.HexDigit(hexString[StartIndex + 1]));
                result.G = (byte)((FParse.HexDigit(hexString[StartIndex + 2]) << 4) + FParse.HexDigit(hexString[StartIndex + 3]));
                result.B = (byte)((FParse.HexDigit(hexString[StartIndex + 4]) << 4) + FParse.HexDigit(hexString[StartIndex + 5]));
                result.A = 255;

                return result;
            }

            if (hexString.Length == 8 + StartIndex)
            {
                FColor result = default(FColor);

                result.R = (byte)((FParse.HexDigit(hexString[StartIndex + 0]) << 4) + FParse.HexDigit(hexString[StartIndex + 1]));
                result.G = (byte)((FParse.HexDigit(hexString[StartIndex + 2]) << 4) + FParse.HexDigit(hexString[StartIndex + 3]));
                result.B = (byte)((FParse.HexDigit(hexString[StartIndex + 4]) << 4) + FParse.HexDigit(hexString[StartIndex + 5]));
                result.A = (byte)((FParse.HexDigit(hexString[StartIndex + 6]) << 4) + FParse.HexDigit(hexString[StartIndex + 7]));

                return result;
            }

            return default(FColor);
        }

        /// <summary>
        /// Makes a random but quite nice color.
        /// </summary>
        public static FColor MakeRandomColor()
        {
            return FLinearColor.MakeRandomColor().ToFColor(true);
        }

        /// <summary>
        /// Makes a color red->green with the passed in scalar (e.g. 0 is red, 1 is green)
        /// </summary>
        public static FColor MakeRedToGreenColorFromScalar(float scalar)
        {
            float redSclr = FMath.Clamp((1.0f - scalar) / 0.5f, 0.0f, 1.0f);
            float greenSclr = FMath.Clamp((scalar / 0.5f), 0.0f, 1.0f);
            int r = FMath.TruncToInt(255 * redSclr);
            int g = FMath.TruncToInt(255 * greenSclr);
            int b = 0;
            return new FColor((byte)r, (byte)g, (byte)b);
        }

        /// <summary>
        /// Converts temperature in Kelvins of a black body radiator to RGB chromaticity.
        /// </summary>
        public static FColor MakeFromColorTemperature(float temp)
        {
            return FLinearColor.MakeFromColorTemperature(temp).ToFColor(true);
        }

        /// <summary>
        /// Returns a new FColor based of this color with the new alpha value.
        /// Usage: const FColor& MyColor = FColorList::Green.WithAlpha(128);
        /// </summary>
        public FColor WithAlpha(byte alpha)
        {
            return new FColor(R, G, B, alpha);
        }

        /// <summary>
        /// Reinterprets the color as a linear color.
        /// </summary>
        /// <returns>The linear color representation.</returns>
        public FLinearColor ReinterpretAsLinear()
        {
            return new FLinearColor(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f);
        }

        public string ToHex()
        {
            return R.ToString("X2") + G.ToString("X2") + B.ToString("X2") + A.ToString("X2");
        }

        public override string ToString()
        {
            return "(R=" + R + ",G=" + G + ",B=" + B + ",A=" + A + ")";
        }

        /// <summary>
        /// Initialize this Color based on an FString. The String is expected to contain R=, G=, B=, A=.
        /// The FColor will be bogus when InitFromString returns false.
        /// </summary>
        /// <param name="sourceString">String containing the color values.</param>
        /// <returns>true if the R,G,B values were read successfully; false otherwise.</returns>
        public bool InitFromString(string sourceString)
        {
            packedValue = 0;
            A = 255;

            // The initialization is only successful if the R, G, and B values can all be parsed from the string
            byte r = R, g = G, b = B, a = A;
            bool successful = 
                FParse.Value(sourceString, "R=", ref r) && 
                FParse.Value(sourceString, "G=", ref g) && 
                FParse.Value(sourceString, "B=", ref b);
            R = r;
            G = g;
            B = b;

            // Alpha is optional, so don't factor in its presence (or lack thereof) in determining initialization success
            FParse.Value(sourceString, "A=", ref a);
            A = a;

            return successful;
        }

        /// <summary>
        /// Gets the color in a packed uint32 format packed in the order ARGB.
        /// </summary>
        public uint ToPackedARGB()
        {
            return (uint)((A << 24) | (R << 16) | (G << 8) | (B << 0));
        }

        /// <summary>
        /// Gets the color in a packed uint32 format packed in the order ABGR.
        /// </summary>
        public uint ToPackedABGR()
        {
            return (uint)((A << 24) | (B << 16) | (G << 8) | (R << 0));
        }

        /// <summary>
        /// Gets the color in a packed uint32 format packed in the order RGBA.
        /// </summary>
        public uint ToPackedRGBA()
        {
            return (uint)((R << 24) | (G << 16) | (B << 8) | (A << 0));
        }

        /// <summary>
        /// Gets the color in a packed uint32 format packed in the order BGRA.
        /// </summary>
        public uint ToPackedBGRA()
        {
            return (uint)((B << 24) | (G << 16) | (R << 8) | (A << 0));
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1
        /// </summary>
        public static FColor Lerp(FColor a, FColor b, float alpha)
        {
            return FMath.Lerp(a, b, alpha);
        }

        /// <summary>
        /// Performs a linear interpolation between two values, Alpha ranges from 0-1.
        /// </summary>
        public static FColor LerpStable(FColor a, FColor b, float alpha)
        {
            return FMath.LerpStable(a, b, alpha);
        }

        public static readonly FColor White = new FColor(255, 255, 255);
        public static readonly FColor Black = new FColor(0, 0, 0);
        public static readonly FColor Transparent = new FColor(0, 0, 0, 0);
        public static readonly FColor Red = new FColor(255, 0, 0);
        public static readonly FColor Green = new FColor(0, 255, 0);
        public static readonly FColor Blue = new FColor(0, 0, 255);
        public static readonly FColor Yellow = new FColor(255, 255, 0);
        public static readonly FColor Cyan = new FColor(0, 255, 255);
        public static readonly FColor Magenta = new FColor(255, 0, 255);
        public static readonly FColor Orange = new FColor(243, 156, 18);
        public static readonly FColor Purple = new FColor(169, 7, 228);
        public static readonly FColor Turquoise = new FColor(26, 188, 156);
        public static readonly FColor Silver = new FColor(189, 195, 199);
        public static readonly FColor Emerald = new FColor(46, 204, 113);

        // Colors taken from XNA
        public static readonly FColor TransparentBlack = new FColor(0);
        //public static readonly FColor Transparent = new FColor(0);
        public static readonly FColor AliceBlue = new FColor(0xfffff8f0);
        public static readonly FColor AntiqueWhite = new FColor(0xffd7ebfa);
        public static readonly FColor Aqua = new FColor(0xffffff00);
        public static readonly FColor Aquamarine = new FColor(0xffd4ff7f);
        public static readonly FColor Azure = new FColor(0xfffffff0);
        public static readonly FColor Beige = new FColor(0xffdcf5f5);
        public static readonly FColor Bisque = new FColor(0xffc4e4ff);
        //public static readonly FColor Black = new FColor(0xff000000);
        public static readonly FColor BlanchedAlmond = new FColor(0xffcdebff);
        //public static readonly FColor Blue = new FColor(0xffff0000);
        public static readonly FColor BlueViolet = new FColor(0xffe22b8a);
        public static readonly FColor Brown = new FColor(0xff2a2aa5);
        public static readonly FColor BurlyWood = new FColor(0xff87b8de);
        public static readonly FColor CadetBlue = new FColor(0xffa09e5f);
        public static readonly FColor Chartreuse = new FColor(0xff00ff7f);
        public static readonly FColor Chocolate = new FColor(0xff1e69d2);
        public static readonly FColor Coral = new FColor(0xff507fff);
        public static readonly FColor CornflowerBlue = new FColor(0xffed9564);
        public static readonly FColor Cornsilk = new FColor(0xffdcf8ff);
        public static readonly FColor Crimson = new FColor(0xff3c14dc);
        //public static readonly FColor Cyan = new FColor(0xffffff00);
        public static readonly FColor DarkBlue = new FColor(0xff8b0000);
        public static readonly FColor DarkCyan = new FColor(0xff8b8b00);
        public static readonly FColor DarkGoldenrod = new FColor(0xff0b86b8);
        public static readonly FColor DarkGray = new FColor(0xffa9a9a9);
        public static readonly FColor DarkGreen = new FColor(0xff006400);
        public static readonly FColor DarkKhaki = new FColor(0xff6bb7bd);
        public static readonly FColor DarkMagenta = new FColor(0xff8b008b);
        public static readonly FColor DarkOliveGreen = new FColor(0xff2f6b55);
        public static readonly FColor DarkOrange = new FColor(0xff008cff);
        public static readonly FColor DarkOrchid = new FColor(0xffcc3299);
        public static readonly FColor DarkRed = new FColor(0xff00008b);
        public static readonly FColor DarkSalmon = new FColor(0xff7a96e9);
        public static readonly FColor DarkSeaGreen = new FColor(0xff8bbc8f);
        public static readonly FColor DarkSlateBlue = new FColor(0xff8b3d48);
        public static readonly FColor DarkSlateGray = new FColor(0xff4f4f2f);
        public static readonly FColor DarkTurquoise = new FColor(0xffd1ce00);
        public static readonly FColor DarkViolet = new FColor(0xffd30094);
        public static readonly FColor DeepPink = new FColor(0xff9314ff);
        public static readonly FColor DeepSkyBlue = new FColor(0xffffbf00);
        public static readonly FColor DimGray = new FColor(0xff696969);
        public static readonly FColor DodgerBlue = new FColor(0xffff901e);
        public static readonly FColor Firebrick = new FColor(0xff2222b2);
        public static readonly FColor FloralWhite = new FColor(0xfff0faff);
        public static readonly FColor ForestGreen = new FColor(0xff228b22);
        public static readonly FColor Fuchsia = new FColor(0xffff00ff);
        public static readonly FColor Gainsboro = new FColor(0xffdcdcdc);
        public static readonly FColor GhostWhite = new FColor(0xfffff8f8);
        public static readonly FColor Gold = new FColor(0xff00d7ff);
        public static readonly FColor Goldenrod = new FColor(0xff20a5da);
        public static readonly FColor Gray = new FColor(0xff808080);
        //public static readonly FColor Green = new FColor(0xff008000);
        public static readonly FColor GreenYellow = new FColor(0xff2fffad);
        public static readonly FColor Honeydew = new FColor(0xfff0fff0);
        public static readonly FColor HotPink = new FColor(0xffb469ff);
        public static readonly FColor IndianRed = new FColor(0xff5c5ccd);
        public static readonly FColor Indigo = new FColor(0xff82004b);
        public static readonly FColor Ivory = new FColor(0xfff0ffff);
        public static readonly FColor Khaki = new FColor(0xff8ce6f0);
        public static readonly FColor Lavender = new FColor(0xfffae6e6);
        public static readonly FColor LavenderBlush = new FColor(0xfff5f0ff);
        public static readonly FColor LawnGreen = new FColor(0xff00fc7c);
        public static readonly FColor LemonChiffon = new FColor(0xffcdfaff);
        public static readonly FColor LightBlue = new FColor(0xffe6d8ad);
        public static readonly FColor LightCoral = new FColor(0xff8080f0);
        public static readonly FColor LightCyan = new FColor(0xffffffe0);
        public static readonly FColor LightGoldenrodYellow = new FColor(0xffd2fafa);
        public static readonly FColor LightGray = new FColor(0xffd3d3d3);
        public static readonly FColor LightGreen = new FColor(0xff90ee90);
        public static readonly FColor LightPink = new FColor(0xffc1b6ff);
        public static readonly FColor LightSalmon = new FColor(0xff7aa0ff);
        public static readonly FColor LightSeaGreen = new FColor(0xffaab220);
        public static readonly FColor LightSkyBlue = new FColor(0xffface87);
        public static readonly FColor LightSlateGray = new FColor(0xff998877);
        public static readonly FColor LightSteelBlue = new FColor(0xffdec4b0);
        public static readonly FColor LightYellow = new FColor(0xffe0ffff);
        public static readonly FColor Lime = new FColor(0xff00ff00);
        public static readonly FColor LimeGreen = new FColor(0xff32cd32);
        public static readonly FColor Linen = new FColor(0xffe6f0fa);
        //public static readonly FColor Magenta = new FColor(0xffff00ff);
        public static readonly FColor Maroon = new FColor(0xff000080);
        public static readonly FColor MediumAquamarine = new FColor(0xffaacd66);
        public static readonly FColor MediumBlue = new FColor(0xffcd0000);
        public static readonly FColor MediumOrchid = new FColor(0xffd355ba);
        public static readonly FColor MediumPurple = new FColor(0xffdb7093);
        public static readonly FColor MediumSeaGreen = new FColor(0xff71b33c);
        public static readonly FColor MediumSlateBlue = new FColor(0xffee687b);
        public static readonly FColor MediumSpringGreen = new FColor(0xff9afa00);
        public static readonly FColor MediumTurquoise = new FColor(0xffccd148);
        public static readonly FColor MediumVioletRed = new FColor(0xff8515c7);
        public static readonly FColor MidnightBlue = new FColor(0xff701919);
        public static readonly FColor MintCream = new FColor(0xfffafff5);
        public static readonly FColor MistyRose = new FColor(0xffe1e4ff);
        public static readonly FColor Moccasin = new FColor(0xffb5e4ff);
        public static readonly FColor MonoGameOrange = new FColor(0xff003ce7);
        public static readonly FColor NavajoWhite = new FColor(0xffaddeff);
        public static readonly FColor Navy = new FColor(0xff800000);
        public static readonly FColor OldLace = new FColor(0xffe6f5fd);
        public static readonly FColor Olive = new FColor(0xff008080);
        public static readonly FColor OliveDrab = new FColor(0xff238e6b);
        //public static readonly FColor Orange = new FColor(0xff00a5ff);
        public static readonly FColor OrangeRed = new FColor(0xff0045ff);
        public static readonly FColor Orchid = new FColor(0xffd670da);
        public static readonly FColor PaleGoldenrod = new FColor(0xffaae8ee);
        public static readonly FColor PaleGreen = new FColor(0xff98fb98);
        public static readonly FColor PaleTurquoise = new FColor(0xffeeeeaf);
        public static readonly FColor PaleVioletRed = new FColor(0xff9370db);
        public static readonly FColor PapayaWhip = new FColor(0xffd5efff);
        public static readonly FColor PeachPuff = new FColor(0xffb9daff);
        public static readonly FColor Peru = new FColor(0xff3f85cd);
        public static readonly FColor Pink = new FColor(0xffcbc0ff);
        public static readonly FColor Plum = new FColor(0xffdda0dd);
        public static readonly FColor PowderBlue = new FColor(0xffe6e0b0);
        //public static readonly FColor Purple = new FColor(0xff800080);
        //public static readonly FColor Red = new FColor(0xff0000ff);
        public static readonly FColor RosyBrown = new FColor(0xff8f8fbc);
        public static readonly FColor RoyalBlue = new FColor(0xffe16941);
        public static readonly FColor SaddleBrown = new FColor(0xff13458b);
        public static readonly FColor Salmon = new FColor(0xff7280fa);
        public static readonly FColor SandyBrown = new FColor(0xff60a4f4);
        public static readonly FColor SeaGreen = new FColor(0xff578b2e);
        public static readonly FColor SeaShell = new FColor(0xffeef5ff);
        public static readonly FColor Sienna = new FColor(0xff2d52a0);
        //public static readonly FColor Silver = new FColor(0xffc0c0c0);
        public static readonly FColor SkyBlue = new FColor(0xffebce87);
        public static readonly FColor SlateBlue = new FColor(0xffcd5a6a);
        public static readonly FColor SlateGray = new FColor(0xff908070);
        public static readonly FColor Snow = new FColor(0xfffafaff);
        public static readonly FColor SpringGreen = new FColor(0xff7fff00);
        public static readonly FColor SteelBlue = new FColor(0xffb48246);
        public static readonly FColor Tan = new FColor(0xff8cb4d2);
        public static readonly FColor Teal = new FColor(0xff808000);
        public static readonly FColor Thistle = new FColor(0xffd8bfd8);
        public static readonly FColor Tomato = new FColor(0xff4763ff);
        //public static readonly FColor Turquoise = new FColor(0xffd0e040);
        public static readonly FColor Violet = new FColor(0xffee82ee);
        public static readonly FColor Wheat = new FColor(0xffb3def5);
        //public static readonly FColor White = new FColor(uint.MaxValue);
        public static readonly FColor WhiteSmoke = new FColor(0xfff5f5f5);
        //public static readonly FColor Yellow = new FColor(0xff00ffff);
        public static readonly FColor YellowGreen = new FColor(0xff32cd9a);
    }
}
