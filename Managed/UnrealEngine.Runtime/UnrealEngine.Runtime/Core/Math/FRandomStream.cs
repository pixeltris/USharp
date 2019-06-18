using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\RandomStream.h

    /// <summary>
    /// Implements a thread-safe SRand based RNG.
    /// 
    /// Very bad quality in the lower bits. Don't use the modulus (%) operator.
    /// </summary>
    [UStruct(Flags = 0x00019008), BlueprintType, UMetaPath("/Script/CoreUObject.RandomStream")]
    [StructLayout(LayoutKind.Sequential)]
    public struct FRandomStream
    {
        static bool InitialSeed_IsValid;
        static int InitialSeed_Offset;
        /// <summary>
        /// Holds the initial seed.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.RandomStream:InitialSeed")]
        public int InitialSeed;

        static bool Seed_IsValid;
        static int Seed_Offset;
        /// <summary>
        /// Holds the current seed.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000200), UMetaPath("/Script/CoreUObject.RandomStream:Seed")]
        public int Seed;

        static bool FRandomStream_IsValid;
        static int FRandomStream_StructSize;

        public FRandomStream Copy()
        {
            FRandomStream result = this;
            return result;
        }

        static FRandomStream()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FRandomStream)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FRandomStream));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.RandomStream");
            FRandomStream_StructSize = NativeReflection.GetStructSize(classAddress);
            InitialSeed_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "InitialSeed");
            InitialSeed_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "InitialSeed", Classes.UIntProperty);
            Seed_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Seed");
            Seed_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Seed", Classes.UIntProperty);
            FRandomStream_IsValid = classAddress != IntPtr.Zero && InitialSeed_IsValid && Seed_IsValid;
            NativeReflection.LogStructIsValid("/Script/CoreUObject.RandomStream", FRandomStream_IsValid);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FRandomStream));
        }

        /// <summary>
        /// Creates and initializes a new random stream from the specified seed value.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        public FRandomStream(int seed)
        {
            InitialSeed = seed;
            Seed = seed;
        }

        /// <summary>
        /// Initializes this random stream with the specified seed value.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        public void Initialize(int seed)
        {
            InitialSeed = seed;
            Seed = seed;
        }

        /// <summary>
        /// Resets this random stream to the initial seed value.
        /// </summary>
        public void Reset()
        {
            Seed = InitialSeed;
        }

        public int GetInitialSeed()
        {
            return InitialSeed;
        }

        public void GenerateNewSeed()
        {
            Initialize(FMath.Rand());
        }

        /// <summary>
        /// Returns a random number between 0 and 1.
        /// </summary>
        /// <returns>Random number.</returns>
        public unsafe float GetFraction()
        {
            MutateSeed();

            float sRandTemp = 1.0f;
            float result = 0;

            *(int*)&result = (int)(*(int*)&sRandTemp & 0xff800000) | (Seed & 0x007fffff);

            return FMath.Fractional(result);
        }

        /// <summary>
        /// Returns a random number between 0 and MAXUINT.
        /// </summary>
        /// <returns>Random number.</returns>
        public uint GetUnsignedInt()
        {
            MutateSeed();

            return (uint)Seed;
        }

        /// <summary>
        /// Returns a random vector of unit size.
        /// </summary>
        /// <returns>Random unit vector.</returns>
        public FVector GetUnitVector()
        {
            FVector result;
            float l;

            do
            {
                // Check random vectors in the unit sphere so result is statistically uniform.
                result.X = GetFraction() * 2.0f - 1.0f;
                result.Y = GetFraction() * 2.0f - 1.0f;
                result.Z = GetFraction() * 2.0f - 1.0f;
                l = result.SizeSquared();
            }
            while (l > 1.0f || l < FMath.KindaSmallNumber);

            return result.GetUnsafeNormal();
        }

        /// <summary>
        /// Gets the current seed.
        /// </summary>
        /// <returns>Current seed.</returns>
        public int GetCurrentSeed()
        {
            return Seed;
        }

        /// <summary>
        /// Mirrors the random number API in FMath
        /// </summary>
        /// <returns>Random number.</returns>
        public float FRand()
        {
            return GetFraction();
        }

        /// <summary>
        /// Helper function for rand implementations.
        /// </summary>
        /// <param name="a"></param>
        /// <returns>A random number in [0..A)</returns>
        public int RandHelper(int a)
        {
            // Can't just multiply GetFraction by A, as GetFraction could be == 1.0f
            return ((a > 0) ? FMath.TruncToInt(GetFraction() * ((float)a - FMath.Delta)) : 0);
        }

        /// <summary>
        /// Helper function for rand implementations.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>A random number >= Min and &lt;= Max</returns>
        public int RandRange(int min, int max)
        {
            int range = (max - min) + 1;
            return min + RandHelper(range);
        }

        /// <summary>
        /// Helper function for rand implementations.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>A random number >= Min and &lt;= Max</returns>
        public float FRandRange(float min, float max)
        {
            return min + (max - min) * FRand();
        }

        /// <summary>
        /// Returns a random vector of unit size.
        /// </summary>
        /// <returns>Random unit vector.</returns>
        public FVector VRand()
        {
            return GetUnitVector();
        }

        /// <summary>
        /// Returns a random unit vector, uniformly distributed, within the specified cone.
        /// </summary>
        /// <param name="dir">The center direction of the cone</param>
        /// <param name="coneHalfAngleRad">Half-angle of cone, in radians.</param>
        /// <returns>Normalized vector within the specified cone.</returns>
        public FVector VRandCone(FVector dir, float coneHalfAngleRad)
        {
            if (coneHalfAngleRad > 0.0f)
            {
                float randU = FRand();
                float randV = FRand();

                // Get spherical coords that have an even distribution over the unit sphere
                // Method described at http://mathworld.wolfram.com/SpherePointPicking.html	
                float theta = 2.0f * FMath.PI * randU;
                float phi = FMath.Acos((2.0f * randV) - 1.0f);

                // restrict phi to [0, ConeHalfAngleRad]
                // this gives an even distribution of points on the surface of the cone
                // centered at the origin, pointing upward (z), with the desired angle
                phi = FMath.Fmod(phi, coneHalfAngleRad);

                // get axes we need to rotate around
                FMatrix dirMat = FMatrix.CreateRotation(dir.Rotation());
                // note the axis translation, since we want the variation to be around X
                FVector dirZ = dirMat.GetUnitAxis(EAxis.X);
                FVector dirY = dirMat.GetUnitAxis(EAxis.Y);

                FVector result = dir.RotateAngleAxis(phi * 180.0f / FMath.PI, dirY);
                result = result.RotateAngleAxis(theta * 180.0f / FMath.PI, dirZ);

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
        /// Returns a random unit vector, uniformly distributed, within the specified cone.
        /// </summary>
        /// <param name="dir">The center direction of the cone</param>
        /// <param name="horizontalConeHalfAngleRad">Horizontal half-angle of cone, in radians.</param>
        /// <param name="verticalConeHalfAngleRad">Vertical half-angle of cone, in radians.</param>
        /// <returns>Normalized vector within the specified cone.</returns>
        public FVector VRandCone(FVector dir, float horizontalConeHalfAngleRad, float verticalConeHalfAngleRad)
        {
            if ((verticalConeHalfAngleRad > 0.0f) && (horizontalConeHalfAngleRad > 0.0f))
            {
                float randU = FRand();
                float randV = FRand();

                // Get spherical coords that have an even distribution over the unit sphere
                // Method described at http://mathworld.wolfram.com/SpherePointPicking.html	
                float theta = 2.0f * FMath.PI * randU;
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
                FVector dirZ = dirMat.GetUnitAxis(EAxis.X);
                FVector dirY = dirMat.GetUnitAxis(EAxis.Y);

                FVector result = dir.RotateAngleAxis(phi * 180.0f / FMath.PI, dirY);
                result = result.RotateAngleAxis(theta * 180.0f / FMath.PI, dirZ);

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
        /// Mutates the current seed into the next seed.
        /// </summary>
        public void MutateSeed()
        {
            Seed = (Seed * 196314165) + 907633515;
        }
    }
}
