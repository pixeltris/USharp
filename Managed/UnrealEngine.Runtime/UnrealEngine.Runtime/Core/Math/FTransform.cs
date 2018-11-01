using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Transform.h

    // NOTE: Native FTransform has a non-zero default ctor. Use FTransform.Default/FTransform.Identity for default init.

    /// <summary>
    /// Transform composed of Scale, Rotation (as a quaternion), and Translation.
    /// 
    /// Transforms can be used to convert from one space to another, for example by transforming
    /// positions and directions from local space to world space.
    /// 
    /// Transformation of position vectors is applied in the order:  Scale -> Rotate -> Translate.
    /// Transformation of direction vectors is applied in the order: Scale -> Rotate.
    /// 
    /// Order matters when composing transforms: C = A * B will yield a transform C that logically
    /// first applies A then B to any subsequent transformation. Note that this is the opposite order of quaternion (FQuat) multiplication.
    /// 
    /// Example: LocalToWorld = (DeltaRotation * LocalToWorld) will change rotation in local space by DeltaRotation.
    /// Example: LocalToWorld = (LocalToWorld * DeltaRotation) will change rotation in world space by DeltaRotation.
    /// </summary>
    [UStruct(Flags = 0x00006008), BlueprintType, UMetaPath("/Script/CoreUObject.Transform", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FTransform : IEquatable<FTransform>
    {
        private const string logTransform = "LogTransform";

        /// <summary>
        /// Below this weight threshold, animations won't be blended in.
        /// </summary>
        private const float ZeroAnimWeightThresh = 0.00001f;

        static bool Rotation_IsValid;
        static int Rotation_Offset;
        /// <summary>
        /// Rotation of this transformation, as a quaternion.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0010001041000005), UMetaPath("/Script/CoreUObject.Transform:Rotation")]
        public FQuat Rotation;

        static bool Translation_IsValid;
        static int Translation_Offset;
        /// <summary>
        /// Translation of this transformation, as a vector.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000005), UMetaPath("/Script/CoreUObject.Transform:Translation")]
        public FVector Translation;

        static bool Scale3D_IsValid;
        static int Scale3D_Offset;
        /// <summary>
        /// 3D scale (always applied in local space) as a vector.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000005), UMetaPath("/Script/CoreUObject.Transform:Scale3D")]
        public FVector Scale3D;

        static int FTransform_StructSize;

        public FTransform Copy()
        {
            FTransform result = this;
            return result;
        }

        static FTransform()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FTransform)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FTransform));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Transform");
            FTransform_StructSize = NativeReflection.GetStructSize(classAddress);
            Rotation_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Rotation");
            Rotation_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Rotation", Classes.UStructProperty);
            Translation_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Translation");
            Translation_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Translation", Classes.UStructProperty);
            Scale3D_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Scale3D");
            Scale3D_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Scale3D", Classes.UStructProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FTransform));
        }

        /// <summary>
        /// The identity transformation (Rotation = FQuat::Identity, Translation = FVector::ZeroVector, Scale3D = (1,1,1))
        /// </summary>
        public static readonly FTransform Identity = new FTransform(new FQuat(0.0f, 0.0f, 0.0f, 1.0f), new FVector(0.0f), new FVector(1.0f));

        /// <summary>
        /// Used in place of the native C++ FTransform constructor (this is same as FTransform.Identity)
        /// </summary>
        public static readonly FTransform Default = Identity;

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN_Scale3D()
        {
            if (Scale3D.ContainsNaN())
            {
                FMath.LogOrEnsureNanError("FTransform Scale3D contains NaN: " + Scale3D.ToString());
                Scale3D = FVector.OneVector;
            }
        }

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN_Translate()
        {
            if (Translation.ContainsNaN())
            {
                FMath.LogOrEnsureNanError("FTransform Translation contains NaN: " + Translation.ToString());
                Translation = FVector.ZeroVector;
            }
        }

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN_Rotate()
        {
            if (Rotation.ContainsNaN())
            {
                FMath.LogOrEnsureNanError("FTransform Rotation contains NaN: " + Rotation.ToString());
                Rotation = FQuat.Identity;
            }
        }

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN_All()
        {
            DiagnosticCheckNaN_Scale3D();
            DiagnosticCheckNaN_Rotate();
            DiagnosticCheckNaN_Translate();
        }

        [Conditional("DEBUG")]
        public void DiagnosticCheck_IsValid()
        {
            DiagnosticCheckNaN_All();
            if (!IsValid())
            {
                FMath.LogOrEnsureNanError("FTransform transform is not valid: " + ToHumanReadableString());
            }
        }

        /// <summary>
        /// Constructor with an initial translation
        /// </summary>
        /// <param name="translation">The value to use for the translation component</param>
        public FTransform(FVector translation)
        {
            Rotation = FQuat.Identity;
            Translation = translation;
            Scale3D = FVector.OneVector;
            DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Constructor with an initial rotation
        /// </summary>
        /// <param name="rotation">The value to use for rotation component</param>
        public FTransform(FQuat rotation)
        {
            Rotation = rotation;
            Translation = FVector.ZeroVector;
            Scale3D = FVector.OneVector;
            DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Constructor with an initial rotation
        /// </summary>
        /// <param name="rotation">The value to use for rotation component  (after being converted to a quaternion)</param>
        public FTransform(FRotator rotation)
        {
            Rotation = rotation.Quaternion();
            Translation = FVector.ZeroVector;
            Scale3D = FVector.OneVector;
            DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Constructor with all components initialized (scale=1)
        /// </summary>
        /// <param name="rotation">The value to use for rotation component</param>
        /// <param name="translation">The value to use for the translation component</param>
        public FTransform(FQuat rotation, FVector translation)
            : this(rotation, translation, FVector.OneVector)
        {
        }

        /// <summary>
        /// Constructor with all components initialized
        /// </summary>
        /// <param name="rotation">The value to use for rotation component</param>
        /// <param name="translation">The value to use for the translation component</param>
        /// <param name="scale3D">The value to use for the scale component</param>
        public FTransform(FQuat rotation, FVector translation, FVector scale3D)
        {
            Rotation = rotation;
            Translation = translation;
            Scale3D = scale3D;
            DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Constructor with all components initialized, taking a FRotator as the rotation component (scale=1)
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        public FTransform(FRotator rotation, FVector translation)
            : this(rotation, translation, FVector.OneVector)
        {
        }

        /// <summary>
        /// Constructor with all components initialized, taking a FRotator as the rotation component
        /// </summary>
        /// <param name="rotation">The value to use for rotation component (after being converted to a quaternion)</param>
        /// <param name="translation">The value to use for the translation component</param>
        /// <param name="scale3D">The value to use for the scale component</param>
        public FTransform(FRotator rotation, FVector translation, FVector scale3D)
        {
            Rotation = rotation.Quaternion();
            Translation = translation;
            Scale3D = scale3D;
            DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Constructor for converting a Matrix (including scale) into a FTransform.
        /// </summary>
        public FTransform(FMatrix matrix)
        {
            this = default(FTransform);
            SetFromMatrix(matrix);
            DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Constructor that takes basis axes and translation
        /// </summary>
        public FTransform(FVector x, FVector y, FVector z, FVector translation)
        {
            this = default(FTransform);
            SetFromMatrix(new FMatrix(x, y, z, translation));
            DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Does a debugf of the contents of this Transform.
        /// </summary>
        public void DebugPrint()
        {
            FMessage.Log(logTransform, ELogVerbosity.Log, ToHumanReadableString());
        }

        /// <summary>
        /// Debug purpose only
        /// </summary>
        public bool DebugEqualMatrix(FMatrix matrix)
        {
            FTransform testResult = new FTransform(matrix);
            if (!Equals(testResult, FMath.KindaSmallNumber))
            {
                // see now which one isn't equal
                if (!Scale3D.Equals(testResult.Scale3D, 0.01f))
                {
                    FMessage.Log(logTransform, ELogVerbosity.Log, "Matrix(S)\t" + testResult.Scale3D);
                    FMessage.Log(logTransform, ELogVerbosity.Log, "VQS(S)\t" + Scale3D);
                }

                // see now which one isn't equal
                if (!Rotation.Equals(testResult.Rotation))
                {
                    FMessage.Log(logTransform, ELogVerbosity.Log, "Matrix(R)\t" + testResult.Rotation);
                    FMessage.Log(logTransform, ELogVerbosity.Log, "VQS(R)\t" + Rotation);
                }

                // see now which one isn't equal
                if (!Translation.Equals(testResult.Translation, 0.01f))
                {
                    FMessage.Log(logTransform, ELogVerbosity.Log, "Matrix(T)\t" + testResult.Translation);
                    FMessage.Log(logTransform, ELogVerbosity.Log, "VQS(T)\t" + Translation);
                }

                return false;
            }
            return true;
        }

        /// <summary>
        /// Convert FTransform contents to a string
        /// </summary>
        public string ToHumanReadableString()
        {
            FRotator r = GetRotation().Rotator();
            FVector t = GetTranslation();
            FVector s = GetScale3D();

            string output =
                "Rotation: Pitch " + r.Pitch + " Yaw " + r.Yaw + " Roll " + r.Roll + "\r\n" +
                "Translation: " + t.X + " " + t.Y + " " + t.Z + "\r\n" +
                "Scale3D: " + s.X + " " + s.Y + " " + s.Z;

            return output;
        }

        public override string ToString()
        {
            FRotator r = GetRotation().Rotator();
            FVector t = GetTranslation();
            FVector s = GetScale3D();

            return t.X + "," + t.Y + "," + t.Z + "|" + r.Pitch + "," + r.Yaw + "," + r.Roll + "|" + s.X + "," + s.Y + "," + s.Z;
        }

        /// <summary>
        /// Convert this Transform to a transformation matrix with scaling.
        /// </summary>
        public FMatrix ToMatrixWithScale()
        {
            FMatrix outMatrix;

            //#if !(UE_BUILD_SHIPPING || UE_BUILD_TEST) && WITH_EDITORONLY_DATA
            // Make sure Rotation is normalized when we turn it into a matrix.
            FMessage.EnsureDebug(IsRotationNormalized(), "Make sure Rotation is normalized when we turn it into a matrix");
            //#endif

            outMatrix.M41 = Translation.X;
            outMatrix.M42 = Translation.Y;
            outMatrix.M43 = Translation.Z;

            float x2 = Rotation.X + Rotation.X;
            float y2 = Rotation.Y + Rotation.Y;
            float z2 = Rotation.Z + Rotation.Z;
            {
                float xx2 = Rotation.X * x2;
                float yy2 = Rotation.Y * y2;
                float zz2 = Rotation.Z * z2;

                outMatrix.M11 = (1.0f - (yy2 + zz2)) * Scale3D.X;
                outMatrix.M22 = (1.0f - (xx2 + zz2)) * Scale3D.Y;
                outMatrix.M33 = (1.0f - (xx2 + yy2)) * Scale3D.Z;
            }
            {
                float yz2 = Rotation.Y * z2;
                float wx2 = Rotation.W * x2;

                outMatrix.M32 = (yz2 - wx2) * Scale3D.Z;
                outMatrix.M23 = (yz2 + wx2) * Scale3D.Y;
            }
            {
                float xy2 = Rotation.X * y2;
                float wz2 = Rotation.W * z2;

                outMatrix.M21 = (xy2 - wz2) * Scale3D.Y;
                outMatrix.M12 = (xy2 + wz2) * Scale3D.X;
            }
            {
                float xz2 = Rotation.X * z2;
                float wy2 = Rotation.W * y2;

                outMatrix.M31 = (xz2 + wy2) * Scale3D.Z;
                outMatrix.M13 = (xz2 - wy2) * Scale3D.X;
            }

            outMatrix.M14 = 0.0f;
            outMatrix.M24 = 0.0f;
            outMatrix.M34 = 0.0f;
            outMatrix.M44 = 1.0f;

            return outMatrix;
        }

        /// <summary>
        /// Convert this Transform to matrix with scaling and compute the inverse of that.
        /// </summary>
        public FMatrix ToInverseMatrixWithScale()
        {
            // todo: optimize
            return ToMatrixWithScale().Inverse();
        }

        /// <summary>
        /// Convert this Transform to inverse.
        /// </summary>
        public FTransform Inverse()
        {
            FQuat invRotation = Rotation.Inverse();
            // this used to cause NaN if Scale contained 0 
            FVector invScale3D = GetSafeScaleReciprocal(Scale3D);
            FVector invTranslation = invRotation * (invScale3D * -Translation);

            return new FTransform(invRotation, invTranslation, invScale3D);
        }

        /// <summary>
        /// Convert this Transform to a transformation matrix, ignoring its scaling
        /// </summary>
        public FMatrix ToMatrixNoScale()
        {
            FMatrix outMatrix;

            //#if !(UE_BUILD_SHIPPING || UE_BUILD_TEST) && WITH_EDITORONLY_DATA
            // Make sure Rotation is normalized when we turn it into a matrix.
            FMessage.EnsureDebug(IsRotationNormalized(), "Make sure Rotation is normalized when we turn it into a matrix");
            //#endif

            outMatrix.M41 = Translation.X;
            outMatrix.M42 = Translation.Y;
            outMatrix.M43 = Translation.Z;

            float x2 = Rotation.X + Rotation.X;
            float y2 = Rotation.Y + Rotation.Y;
            float z2 = Rotation.Z + Rotation.Z;
            {
                float xx2 = Rotation.X * x2;
                float yy2 = Rotation.Y * y2;
                float zz2 = Rotation.Z * z2;

                outMatrix.M11 = (1.0f - (yy2 + zz2));
                outMatrix.M22 = (1.0f - (xx2 + zz2));
                outMatrix.M33 = (1.0f - (xx2 + yy2));
            }
            {
                float yz2 = Rotation.Y * z2;
                float wx2 = Rotation.W * x2;

                outMatrix.M32 = (yz2 - wx2);
                outMatrix.M23 = (yz2 + wx2);
            }
            {
                float xy2 = Rotation.X * y2;
                float wz2 = Rotation.W * z2;

                outMatrix.M21 = (xy2 - wz2);
                outMatrix.M12 = (xy2 + wz2);
            }
            {
                float xz2 = Rotation.X * z2;
                float wy2 = Rotation.W * y2;

                outMatrix.M31 = (xz2 + wy2);
                outMatrix.M13 = (xz2 - wy2);
            }

            outMatrix.M14 = 0.0f;
            outMatrix.M24 = 0.0f;
            outMatrix.M34 = 0.0f;
            outMatrix.M44 = 1.0f;

            return outMatrix;
        }

        /// <summary>
        /// Set this transform to the weighted blend of the supplied two transforms.
        /// </summary>
        public void Blend(FTransform atom1, FTransform atom2, float alpha)
        {
            //#if !(UE_BUILD_SHIPPING || UE_BUILD_TEST) && WITH_EDITORONLY_DATA
            // Check that all bone atoms coming from animation are normalized
            FMessage.EnsureDebug(atom1.IsRotationNormalized(), "Check that all bone atoms coming from animation are normalized");
            FMessage.EnsureDebug(atom2.IsRotationNormalized(), "Check that all bone atoms coming from animation are normalized");
            //#endif

            if (alpha <= ZeroAnimWeightThresh)
            {
                // if blend is all the way for child1, then just copy its bone atoms
                this = atom1;
            }
            else if (alpha >= 1.0f - ZeroAnimWeightThresh)
            {
                // if blend is all the way for child2, then just copy its bone atoms
                this = atom2;
            }
            else
            {
                // Simple linear interpolation for translation and scale.
                Translation = FMath.Lerp(atom1.Translation, atom2.Translation, alpha);
                Scale3D = FMath.Lerp(atom1.Scale3D, atom2.Scale3D, alpha);
                Rotation = FQuat.FastLerp(atom1.Rotation, atom2.Rotation, alpha);

                // ..and renormalize
                Rotation.Normalize();
            }
        }

        /// <summary>
        /// Set this Transform to the weighted blend of it and the supplied Transform.
        /// </summary>
        public void BlendWith(FTransform otherAtom, float alpha)
        {
            //#if !(UE_BUILD_SHIPPING || UE_BUILD_TEST) && WITH_EDITORONLY_DATA
            // Check that all bone atoms coming from animation are normalized
            FMessage.EnsureDebug(IsRotationNormalized(), "Check that all bone atoms coming from animation are normalized");
            FMessage.EnsureDebug(otherAtom.IsRotationNormalized(), "Check that all bone atoms coming from animation are normalized");
            //#endif

            if (alpha > ZeroAnimWeightThresh)
            {
                if (alpha >= 1.0f - ZeroAnimWeightThresh)
                {
                    // if blend is all the way for child2, then just copy its bone atoms
                    this = otherAtom;
                }
                else
                {
                    // Simple linear interpolation for translation and scale.
                    Translation = FMath.Lerp(Translation, otherAtom.Translation, alpha);
                    Scale3D = FMath.Lerp(Scale3D, otherAtom.Scale3D, alpha);
                    Rotation = FQuat.FastLerp(Rotation, otherAtom.Rotation, alpha);

                    // ..and renormalize
                    Rotation.Normalize();
                }
            }
        }

        /// <summary>
        /// Checks two transforms for equality.
        /// </summary>
        /// <param name="a">The first transform.</param>
        /// <param name="b">The second transform.</param>
        /// <returns>true if the transforms are equal, false otherwise.</returns>
        public static bool operator ==(FTransform a, FTransform b)
        {
            return a.Rotation == b.Rotation && a.Translation == b.Translation && a.Scale3D == b.Scale3D;
        }

        /// <summary>
        /// Checks two transforms for inequality.
        /// </summary>
        /// <param name="a">The first transform.</param>
        /// <param name="b">The second transform.</param>
        /// <returns>true if the transforms are not equal, false otherwise.</returns>
        public static bool operator !=(FTransform a, FTransform b)
        {
            return a.Rotation != b.Rotation || a.Translation != b.Translation || a.Scale3D != b.Scale3D;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FTransform))
            {
                return false;
            }

            return Equals((FTransform)obj);
        }

        public bool Equals(FTransform other)
        {
            return Rotation == other.Rotation && Translation == other.Translation && Scale3D == other.Scale3D;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = Rotation.GetHashCode();
                hashcode = (hashcode * 397) ^ Translation.GetHashCode();
                hashcode = (hashcode * 397) ^ Scale3D.GetHashCode();
                return hashcode;
            }
        }

        /// <summary>
        /// Gets the result of adding two transforms.
        /// 
        /// Quaternion addition is wrong here. This is just a special case for linear interpolation.
        /// Use only within blends!!
        /// Rotation part is NOT normalized!!
        /// </summary>
        /// <param name="a">The first transform.</param>
        /// <param name="b">The second transform.</param>
        /// <returns>The result of the addition.</returns>
        public static FTransform operator +(FTransform a, FTransform b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of adding two transforms.
        /// 
        /// Quaternion addition is wrong here. This is just a special case for linear interpolation.
        /// Use only within blends!!
        /// Rotation part is NOT normalized!!
        /// </summary>
        /// <param name="a">The first transform.</param>
        /// <param name="b">The second transform.</param>
        /// <returns>The result of the addition.</returns>
        public static FTransform Add(FTransform a, FTransform b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of adding two transforms.
        /// 
        /// Quaternion addition is wrong here. This is just a special case for linear interpolation.
        /// Use only within blends!!
        /// Rotation part is NOT normalized!!
        /// </summary>
        /// <param name="a">The first transform.</param>
        /// <param name="b">The second transform.</param>
        /// <param name="result">The result of the addition.</param>
        public static void Add(ref FTransform a, ref FTransform b, out FTransform result)
        {
            result.Translation = a.Translation + b.Translation;
            result.Rotation = a.Rotation + b.Rotation;
            result.Scale3D = a.Scale3D + b.Scale3D;
            result.DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Gets the result of scaling the transform.
        /// </summary>
        /// <param name="scale">The scaling factor.</param>
        /// <param name="t">The transform.</param>
        /// <returns>The result of the transform multiplication with a scalar.</returns>
        public static FTransform operator *(float scale, FTransform t)
        {
            Multiply(ref t, scale, out t);
            return t;
        }

        /// <summary>
        /// Gets the result of scaling the transform.
        /// </summary>
        /// <param name="scale">The scaling factor.</param>
        /// <param name="t">The transform.</param>
        /// <returns>The result of the transform multiplication with a scalar.</returns>
        public static FTransform operator *(FTransform t, float scale)
        {
            Multiply(ref t, scale, out t);
            return t;
        }

        /// <summary>
        /// Gets the result of scaling the transform.
        /// </summary>
        /// <param name="scale">The scaling factor.</param>
        /// <param name="t">The transform.</param>
        /// <param name="result">The result of the multiplication.</param>
        public static void Multiply(ref FTransform t, float scale, out FTransform result)
        {
            result.Translation = t.Translation * scale;
            result.Rotation = t.Rotation * scale;
            result.Scale3D = t.Scale3D * scale;
            result.DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Gets the result of multiplying two transforms.
        /// 
        /// Order matters when composing transforms : C = A * B will yield a transform C that logically first applies A then B to any subsequent transformation.
        /// </summary>
        /// <param name="a">The first transform.</param>
        /// <param name="b">The second transform.</param>
        /// <returns>The result of the multiplication.</returns>
        public static FTransform operator *(FTransform a, FTransform b)
        {
            Multiply(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of multiplying two transforms.
        /// 
        /// Order matters when composing transforms : C = A * B will yield a transform C that logically first applies A then B to any subsequent transformation.
        /// </summary>
        /// <param name="a">The first transform.</param>
        /// <param name="b">The second transform.</param>
        /// <returns>The result of the multiplication.</returns>
        public static FTransform Multiply(FTransform a, FTransform b)
        {
            Multiply(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of multiplying two transforms.
        /// 
        /// Order matters when composing transforms : C = A * B will yield a transform C that logically first applies A then B to any subsequent transformation.
        /// </summary>
        /// <param name="a">The first transform.</param>
        /// <param name="b">The second transform.</param>
        /// <param name="result">The result of the multiplication.</param>
        public static void Multiply(ref FTransform a, ref FTransform b, out FTransform result)
        {
            a.DiagnosticCheckNaN_All();
            b.DiagnosticCheckNaN_All();

            Debug.Assert(a.IsRotationNormalized());
            Debug.Assert(b.IsRotationNormalized());

            //	When Q = quaternion, S = single scalar scale, and T = translation
            //	QST(A) = Q(A), S(A), T(A), and QST(B) = Q(B), S(B), T(B)

            //	QST (AxB) 

            // QST(A) = Q(A)*S(A)*P*-Q(A) + T(A)
            // QST(AxB) = Q(B)*S(B)*QST(A)*-Q(B) + T(B)
            // QST(AxB) = Q(B)*S(B)*[Q(A)*S(A)*P*-Q(A) + T(A)]*-Q(B) + T(B)
            // QST(AxB) = Q(B)*S(B)*Q(A)*S(A)*P*-Q(A)*-Q(B) + Q(B)*S(B)*T(A)*-Q(B) + T(B)
            // QST(AxB) = [Q(B)*Q(A)]*[S(B)*S(A)]*P*-[Q(B)*Q(A)] + Q(B)*S(B)*T(A)*-Q(B) + T(B)

            //	Q(AxB) = Q(B)*Q(A)
            //	S(AxB) = S(A)*S(B)
            //	T(AxB) = Q(B)*S(B)*T(A)*-Q(B) + T(B)

            if (AnyHasNegativeScale(a.Scale3D, b.Scale3D))
            {
                // @note, if you have 0 scale with negative, you're going to lose rotation as it can't convert back to quat
                MultiplyUsingMatrixWithScale(out result, ref a, ref b);
            }
            else
            {
                result.Rotation = b.Rotation * a.Rotation;
                result.Scale3D = a.Scale3D * b.Scale3D;
                result.Translation = b.Rotation * (b.Scale3D * a.Translation) + b.Translation;
            }

            // we do not support matrix transform when non-uniform
            // that was removed at rev 21 with UE4
            result.DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Create a new transform: OutTransform = A * B using the matrix while keeping the scale that's given by A and B
        /// Please note that this operation is a lot more expensive than normal Multiply
        /// 
        /// Order matters when composing transforms : A * B will yield a transform that logically first applies A then B to any subsequent transformation.
        /// </summary>
        /// <param name="outTransform">pointer to transform that will store the result of A * B.</param>
        /// <param name="a">Transform A.</param>
        /// <param name="b">Transform B.</param>
        private static void MultiplyUsingMatrixWithScale(out FTransform outTransform, ref FTransform a, ref FTransform b)
        {
            // the goal of using M is to get the correct orientation
            // but for translation, we still need scale
            FMatrix aMatrix = a.ToMatrixWithScale();
            FMatrix bMatrix = b.ToMatrixWithScale();
            FVector desiredScale = a.Scale3D * b.Scale3D;
            ConstructTransformFromMatrixWithDesiredScale(ref aMatrix, ref bMatrix, ref desiredScale, out outTransform);
        }

        /// <summary>
        /// Create a new transform from multiplications of given to matrices (AMatrix*BMatrix) using desired scale
        /// This is used by MultiplyUsingMatrixWithScale and GetRelativeTransformUsingMatrixWithScale
        /// This is only used to handle negative scale
        /// </summary>
        /// <param name="aMatrix">first Matrix of operation</param>
        /// <param name="bMatrix">second Matrix of operation</param>
        /// <param name="desiredScale">there is no check on if the magnitude is correct here. It assumes that is correct.</param>
        /// <param name="outTransform">the constructed transform</param>
        private static void ConstructTransformFromMatrixWithDesiredScale(ref FMatrix aMatrix, ref FMatrix bMatrix, ref FVector desiredScale, out FTransform outTransform)
        {
            // the goal of using M is to get the correct orientation
            // but for translation, we still need scale
            FMatrix m = aMatrix * bMatrix;
            m.RemoveScaling();

            // apply negative scale back to axes
            FVector signedScale = desiredScale.GetSignVector();

            m.SetAxis(0, signedScale.X * m.GetScaledAxis(EAxis.X));
            m.SetAxis(1, signedScale.Y * m.GetScaledAxis(EAxis.Y));
            m.SetAxis(2, signedScale.Z * m.GetScaledAxis(EAxis.Z));

            // @note: if you have negative with 0 scale, this will return rotation that is identity
            // since matrix loses that axes
            FQuat rotation = new FQuat(m);
            rotation.Normalize();

            // set values back to output
            outTransform.Scale3D = desiredScale;
            outTransform.Rotation = rotation;

            // technically I could calculate this using FTransform but then it does more quat multiplication 
            // instead of using Scale in matrix multiplication
            // it's a question of between RemoveScaling vs using FTransform to move translation
            outTransform.Translation = m.GetOrigin();
        }

        /// <summary>
        /// Create a new transform: OutTransform = Base * Relative(-1) using the matrix while keeping the scale that's given by Base and Relative
        /// Please note that this operation is a lot more expensive than normal GetRelativeTrnasform
        /// </summary>
        /// <param name="outTransform">pointer to transform that will store the result of Base * Relative(-1).</param>
        /// <param name="baseTransform">Transform Base.</param>
        /// <param name="relativeTransform">Transform Relative.</param>
        private static void GetRelativeTransformUsingMatrixWithScale(out FTransform outTransform, ref FTransform baseTransform, ref FTransform relativeTransform)
        {
            // the goal of using M is to get the correct orientation
            // but for translation, we still need scale
            FMatrix am = baseTransform.ToMatrixWithScale();
            FMatrix bm = relativeTransform.ToMatrixWithScale();
            FMatrix bmInverse = bm.Inverse();
            // get combined scale
            FVector safeRecipScale3D = GetSafeScaleReciprocal(relativeTransform.Scale3D, FMath.SmallNumber);
            FVector desiredScale3D = baseTransform.Scale3D * safeRecipScale3D;
            ConstructTransformFromMatrixWithDesiredScale(ref am, ref bmInverse, ref desiredScale3D, out outTransform);
        }

        /// <summary>
        /// Return a transform that is the result of a transform multiplied by another transform (made only from a rotation).
        /// 
        /// Order matters when composing transforms : C = A * B will yield a transform C that logically first applies A then B to any subsequent transformation.
        /// </summary>
        /// <param name="t">The transform.</param>
        /// <param name="q">The quaternion.</param>
        /// <returns>The result of the multiplication.</returns>
        public static FTransform operator *(FTransform t, FQuat q)
        {
            Multiply(ref t, ref q, out t);
            return t;
        }

        /// <summary>
        /// Return a transform that is the result of a transform multiplied by another transform (made only from a rotation).
        /// 
        /// Order matters when composing transforms : C = A * B will yield a transform C that logically first applies A then B to any subsequent transformation.
        /// </summary>
        /// <param name="t">The transform.</param>
        /// <param name="q">The quaternion.</param>
        /// <returns>The result of the multiplication.</returns>
        public static FTransform Multiply(FTransform t, FQuat q)
        {
            Multiply(ref t, ref q, out t);
            return t;
        }

        /// <summary>
        /// Return a transform that is the result of a transform multiplied by another transform (made only from a rotation).
        /// 
        /// Order matters when composing transforms : C = A * B will yield a transform C that logically first applies A then B to any subsequent transformation.
        /// </summary>
        /// <param name="t">The transform.</param>
        /// <param name="q">The quaternion.</param>
        /// <param name="result">The result of the multiplication.</param>
        public static void Multiply(ref FTransform t, ref FQuat q, out FTransform result)
        {
            FTransform otherTransform = new FTransform(q, FVector.ZeroVector, FVector.OneVector);
            Multiply(ref t, ref otherTransform, out result);
        }

        public static bool AnyHasNegativeScale(FVector scale3D, FVector otherScale3D)
        {
            return (scale3D.X < 0.0f || scale3D.Y < 0.0f || scale3D.Z < 0.0f
                || otherScale3D.X < 0.0f || otherScale3D.Y < 0.0f || otherScale3D.Z < 0.0f);
        }

        /// <summary>
        /// Scale the translation part of the Transform by the supplied vector.
        /// </summary>
        public void ScaleTranslation(FVector scale3D)
        {
            Translation *= scale3D;
            DiagnosticCheckNaN_Translate();
        }

        /// <summary>
        /// Scale the translation part of the Transform by the supplied scalar.
        /// </summary>
        public void ScaleTranslation(float scale)
        {
            Translation *= scale;
            DiagnosticCheckNaN_Translate();
        }

        /// <summary>
        /// this function is from matrix, and all it does is to normalize rotation portion
        /// </summary>
        public void RemoveScaling(float tolerance = FMath.SmallNumber)
        {
            Scale3D = new FVector(1, 1, 1);
            Rotation.Normalize();

            DiagnosticCheckNaN_Rotate();
            DiagnosticCheckNaN_Scale3D();
        }

        /// <summary>
        /// same version of FMatrix::GetMaximumAxisScale function
        /// </summary>
        /// <returns>the maximum magnitude of all components of the 3D scale.</returns>
        public float GetMaximumAxisScale()
        {
            DiagnosticCheckNaN_Scale3D();
            return Scale3D.GetAbsMax();
        }

        /// <summary>
        /// Returns the minimum magnitude of all components of the 3D scale.
        /// </summary>
        /// <returns>the minimum magnitude of all components of the 3D scale.</returns>
        public float GetMinimumAxisScale()
        {
            DiagnosticCheckNaN_Scale3D();
            return Scale3D.GetAbsMin();
        }

        // Inverse does not work well with VQS format(in particular non-uniform), so removing it, but made two below functions to be used instead. 

        /*******************************************************************************************
         * The below 2 functions are the ones to get delta transform and return FTransform format that can be concatenated
         * Inverse itself can't concatenate with VQS format(since VQS always transform from S->Q->T, where inverse happens from T(-1)->Q(-1)->S(-1))
         * So these 2 provides ways to fix this
         * GetRelativeTransform returns this*Other(-1) and parameter is Other(not Other(-1))
         * GetRelativeTransformReverse returns this(-1)*Other, and parameter is Other.
         ******************************************************************************************/
        public FTransform GetRelativeTransform(FTransform other)
        {
            // A * B(-1) = VQS(B)(-1) (VQS (A))
            // 
            // Scale = S(A)/S(B)
            // Rotation = Q(B)(-1) * Q(A)
            // Translation = 1/S(B) *[Q(B)(-1)*(T(A)-T(B))*Q(B)]
            // where A = this, B = Other
            FTransform result;

            if (AnyHasNegativeScale(Scale3D, other.GetScale3D()))
            {
                // @note, if you have 0 scale with negative, you're going to lose rotation as it can't convert back to quat
                GetRelativeTransformUsingMatrixWithScale(out result, ref this, ref other);
            }
            else
            {
                FVector safeRecipScale3D = GetSafeScaleReciprocal(other.Scale3D, FMath.SmallNumber);
                result.Scale3D = Scale3D * safeRecipScale3D;

                if (other.Rotation.IsNormalized() == false)
                {
                    return FTransform.Identity;
                }

                FQuat Inverse = other.Rotation.Inverse();
                result.Rotation = Inverse * Rotation;

                result.Translation = (Inverse * (Translation - other.Translation)) * (safeRecipScale3D);

                //#if DEBUG_INVERSE_TRANSFORM
                //FMatrix am = ToMatrixWithScale();
                //FMatrix bm = other.ToMatrixWithScale();
                //result.DebugEqualMatrix(am * bm.InverseFast());
                //#endif
            }

            return result;
        }

        public FTransform GetRelativeTransformReverse(FTransform other)
        {
            // A (-1) * B = VQS(B)(VQS (A)(-1))
            // 
            // Scale = S(B)/S(A)
            // Rotation = Q(B) * Q(A)(-1)
            // Translation = T(B)-S(B)/S(A) *[Q(B)*Q(A)(-1)*T(A)*Q(A)*Q(B)(-1)]
            // where A = this, and B = Other
            FTransform result;

            FVector safeRecipScale3D = GetSafeScaleReciprocal(Scale3D);
            result.Scale3D = other.Scale3D * safeRecipScale3D;

            result.Rotation = other.Rotation * Rotation.Inverse();

            result.Translation = other.Translation - result.Scale3D * (result.Rotation * Translation);

            //#if DEBUG_INVERSE_TRANSFORM
            //FMatrix am = ToMatrixWithScale();
            //FMatrix bm = other.ToMatrixWithScale();
            //result.DebugEqualMatrix(am.InverseFast() * bm);
            //#endif

            return result;
        }

        /// <summary>
        /// Set current transform and the relative to ParentTransform.
        /// Equates to This = This->GetRelativeTransform(Parent), but saves the intermediate FTransform storage and copy.
        /// </summary>
        public void SetToRelativeTransform(FTransform parentTransform)
        {
            // A * B(-1) = VQS(B)(-1) (VQS (A))
            // 
            // Scale = S(A)/S(B)
            // Rotation = Q(B)(-1) * Q(A)
            // Translation = 1/S(B) *[Q(B)(-1)*(T(A)-T(B))*Q(B)]
            // where A = this, B = Other

            //#if DEBUG_INVERSE_TRANSFORM
            //FMatrix am = ToMatrixWithScale();
            //FMatrix bm = parentTransform.ToMatrixWithScale();
            //#endif

            FVector SafeRecipScale3D = GetSafeScaleReciprocal(parentTransform.Scale3D, FMath.SmallNumber);
            FQuat InverseRot = parentTransform.Rotation.Inverse();

            Scale3D *= SafeRecipScale3D;
            Translation = (InverseRot * (Translation - parentTransform.Translation)) * SafeRecipScale3D;
            Rotation = InverseRot * Rotation;

            //#if DEBUG_INVERSE_TRANSFORM
            //DebugEqualMatrix(am * bm.InverseFast());
            //#endif
        }

        /// <summary>
        /// Transform FVector4
        /// </summary>
        public FVector4 TransformFVector4(FVector4 v)
        {
            DiagnosticCheckNaN_All();

            // if not, this won't work
            Debug.Assert(v.W == 0.0f || v.W == 1.0f);

            //Transform using QST is following
            //QST(P) = Q*S*P*-Q + T where Q = quaternion, S = scale, T = translation

            FVector4 transform = new FVector4(Rotation.RotateVector(Scale3D * (FVector)v), 0.0f);
            if (v.W == 1.0f)
            {
                transform += new FVector4(Translation, 1.0f);
            }

            return transform;
        }

        /// <summary>
        /// Transform homogenous FVector4, ignoring the scaling part of this transform
        /// </summary>
        public FVector4 TransformFVector4NoScale(FVector4 v)
        {
            DiagnosticCheckNaN_All();

            // if not, this won't work
            Debug.Assert(v.W == 0.0f || v.W == 1.0f);

            //Transform using QST is following
            //QST(P) = Q*S*P*-Q + T where Q = quaternion, S = scale, T = translation
            FVector4 transform = new FVector4(Rotation.RotateVector((FVector)v), 0.0f);
            if (v.W == 1.0f)
            {
                transform += new FVector4(Translation, 1.0f);
            }

            return transform;
        }

        public FVector TransformPosition(FVector v)
        {
            DiagnosticCheckNaN_All();
            return Rotation.RotateVector(Scale3D * v) + Translation;
        }

        public FVector TransformPositionNoScale(FVector v)
        {
            DiagnosticCheckNaN_All();
            return Rotation.RotateVector(v) + Translation;
        }

        /** Inverts the transform and then transforms V - correctly handles scaling in this transform. */
        public FVector InverseTransformPosition(FVector v)
        {
            // do backward operation when inverse, translation -> rotation -> scale
            DiagnosticCheckNaN_All();
            return (Rotation.UnrotateVector(v - Translation)) * GetSafeScaleReciprocal(Scale3D);
        }

        public FVector InverseTransformPositionNoScale(FVector v)
        {
            // do backward operation when inverse, translation -> rotation
            DiagnosticCheckNaN_All();
            return (Rotation.UnrotateVector(v - Translation));
        }

        public FVector TransformVector(FVector v)
        {
            DiagnosticCheckNaN_All();
            return Rotation.RotateVector(Scale3D * v);
        }

        public FVector TransformVectorNoScale(FVector v)
        {
            DiagnosticCheckNaN_All();
            return Rotation.RotateVector(v);
        }

        /// <summary>
        /// Transform a direction vector by the inverse of this transform - will not take into account translation part.
        /// If you want to transform a surface normal (or plane) and correctly account for non-uniform scaling you should use TransformByUsingAdjointT with adjoint of matrix inverse.
        /// </summary>
        public FVector InverseTransformVector(FVector v)
        {
            // do backward operation when inverse, translation -> rotation -> scale
            DiagnosticCheckNaN_All();
            return (Rotation.UnrotateVector(v)) * GetSafeScaleReciprocal(Scale3D);
        }

        public FVector InverseTransformVectorNoScale(FVector v)
        {
            // do backward operation when inverse, translation -> rotation
            DiagnosticCheckNaN_All();
            return (Rotation.UnrotateVector(v));
        }

        /// <summary>
        /// Transform a rotation.
        /// For example if this is a LocalToWorld transform, TransformRotation(Q) would transform Q from local to world space.
        /// </summary>
        public FQuat TransformRotation(FQuat q)
        {
            return GetRotation() * q;
        }

        /// <summary>
        /// Inverse transform a rotation.
        /// For example if this is a LocalToWorld transform, InverseTransformRotation(Q) would transform Q from world to local space.
        /// </summary>
        public FQuat InverseTransformRotation(FQuat q)
        {
            return GetRotation().Inverse() * q;
        }

        /// <summary>
        /// Apply Scale to this transform
        /// </summary>
        public FTransform GetScaled(float scale)
        {
            FTransform a = this;
            a.Scale3D *= scale;

            a.DiagnosticCheckNaN_Scale3D();

            return a;
        }

        /// <summary>
        /// Apply Scale to this transform
        /// </summary>
        public FTransform GetScaled(FVector scale)
        {
            FTransform a = this;
            a.Scale3D *= scale;

            a.DiagnosticCheckNaN_Scale3D();

            return a;
        }

        public FVector GetScaledAxis(EAxis axis)
        {
            switch (axis)
            {
                case EAxis.X:
                    return TransformVectorNoScale(new FVector(1.0f, 0.0f, 0.0f));
                case EAxis.Y:
                    return TransformVectorNoScale(new FVector(0.0f, 1.0f, 0.0f));
                case EAxis.Z:
                    return TransformVectorNoScale(new FVector(0.0f, 0.0f, 1.0f));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Mirror(EAxis mirrorAxis, EAxis flipAxis)
        {
            // We do convert to Matrix for mirroring.
            FMatrix m = ToMatrixWithScale();
            m.Mirror(mirrorAxis, flipAxis);
            SetFromMatrix(m);
        }

        public static FVector GetSafeScaleReciprocal(FVector scale, float tolerance = FMath.SmallNumber)
        {
            // mathematically if you have 0 scale, it should be infinite, 
            // however, in practice if you have 0 scale, and relative transform doesn't make much sense 
            // anymore because you should be instead of showing gigantic infinite mesh
            // also returning BIG_NUMBER causes sequential NaN issues by multiplying 
            // so we hardcode as 0

            FVector safeReciprocalScale;
            if (FMath.Abs(scale.X) <= tolerance)
            {
                safeReciprocalScale.X = 0.0f;
            }
            else
            {
                safeReciprocalScale.X = 1 / scale.X;
            }

            if (FMath.Abs(scale.Y) <= tolerance)
            {
                safeReciprocalScale.Y = 0.0f;
            }
            else
            {
                safeReciprocalScale.Y = 1 / scale.Y;
            }

            if (FMath.Abs(scale.Z) <= tolerance)
            {
                safeReciprocalScale.Z = 0.0f;
            }
            else
            {
                safeReciprocalScale.Z = 1 / scale.Z;
            }

            return safeReciprocalScale;
        }

        public FVector GetLocation()
        {
            return GetTranslation();
        }

        public FRotator Rotator()
        {
            return Rotation.Rotator();
        }

        public float GetDeterminant()
        {
            return Scale3D.X * Scale3D.Y * Scale3D.Z;
        }

        /// <summary>
        /// Set the translation of this transformation
        /// </summary>
        public void SetLocation(FVector origin)
        {
            Translation = origin;
            DiagnosticCheckNaN_Translate();
        }

        /// <summary>
        /// Checks the components for non-finite values (NaN or Inf).
        /// </summary>
        /// <returns>Returns true if any component (rotation, translation, or scale) is not finite.</returns>
        public bool ContainsNaN()
        {
            return (Translation.ContainsNaN() || Rotation.ContainsNaN() || Scale3D.ContainsNaN());
        }

        public bool IsValid()
        {
            if (ContainsNaN())
            {
                return false;
            }

            if (!Rotation.IsNormalized())
            {
                return false;
            }

            return true;
        }

        private bool Private_RotationEquals(FQuat rotation, float tolerance = FMath.KindaSmallNumber)
        {
            return Rotation.Equals(rotation, tolerance);
        }

        private bool Private_TranslationEquals(FVector translation, float tolerance = FMath.KindaSmallNumber)
        {
            return Translation.Equals(translation, tolerance);
        }

        private bool Private_Scale3DEquals(FVector scale3D, float tolerance = FMath.KindaSmallNumber)
        {
            return Scale3D.Equals(scale3D, tolerance);
        }

        /// <summary>
        /// Test if A's rotation equals B's rotation, within a tolerance.
        /// </summary>
        public static bool AreRotationsEqual(FTransform a, FTransform b, float tolerance = FMath.KindaSmallNumber)
        {
            return a.Private_RotationEquals(b.Rotation, tolerance);
        }

        /// <summary>
        /// Test if A's translation equals B's translation, within a tolerance.
        /// </summary>
        public static bool AreTranslationsEqual(FTransform a, FTransform b, float tolerance = FMath.KindaSmallNumber)
        {
            return a.Private_TranslationEquals(b.Translation, tolerance);
        }

        /// <summary>
        /// Test if A's scale equals B's scale, within a tolerance.
        /// </summary>
        public static bool AreScale3DsEqual(FTransform a, FTransform b, float tolerance = FMath.KindaSmallNumber)
        {
            return a.Private_Scale3DEquals(b.Scale3D, tolerance);
        }

        /// <summary>
        /// Test if this Transform's rotation equals another's rotation, within a tolerance.
        /// </summary>
        public bool RotationEquals(FTransform other, float tolerance = FMath.KindaSmallNumber)
        {
            return AreRotationsEqual(this, other, tolerance);
        }

        /// <summary>
        /// Test if this Transform's translation equals another's translation, within a tolerance.
        /// </summary>
        public bool TranslationEquals(FTransform other, float tolerance = FMath.KindaSmallNumber)
        {
            return AreTranslationsEqual(this, other, tolerance);
        }

        /// <summary>
        /// Test if this Transform's scale equals another's scale, within a tolerance. Preferred over "GetScale3D().Equals(Other.GetScale3D())" because it is faster on some platforms.
        /// </summary>
        public bool Scale3DEquals(FTransform other, float tolerance = FMath.KindaSmallNumber)
        {
            return AreScale3DsEqual(this, other, tolerance);
        }

        /// <summary>
        /// Test if all components of the transforms are equal, within a tolerance.
        /// </summary>
        public bool Equals(FTransform other, float tolerance = FMath.KindaSmallNumber)
        {
            return
                Private_TranslationEquals(other.Translation, tolerance) &&
                Private_RotationEquals(other.Rotation, tolerance) &&
                Private_Scale3DEquals(other.Scale3D, tolerance);
        }

        /// <summary>
        /// Test if rotation and translation components of the transforms are equal, within a tolerance.
        /// </summary>
        public bool EqualsNoScale(FTransform other, float tolerance = FMath.KindaSmallNumber)
        {
            return Private_TranslationEquals(other.Translation, tolerance) && Private_RotationEquals(other.Rotation, tolerance);
        }

        /// <summary>
        /// Sets the components
        /// </summary>
        /// <param name="rotation">The new value for the Rotation component</param>
        /// <param name="translation">The new value for the Translation component</param>
        /// <param name="scale3D">The new value for the Scale3D component</param>
        public void SetComponents(FQuat rotation, FVector translation, FVector scale3D)
        {
            Rotation = rotation;
            Translation = translation;
            Scale3D = scale3D;

            DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Sets the components to the identity transform:
        ///   Rotation = (0,0,0,1)
        ///   Translation = (0,0,0)
        ///   Scale3D = (1,1,1)
        /// </summary>
        public void SetIdentity()
        {
            Rotation = FQuat.Identity;
            Translation = FVector.ZeroVector;
            Scale3D = new FVector(1, 1, 1);
        }

        /// <summary>
        /// Scales the Scale3D component by a new factor
        /// </summary>
        /// <param name="scale3DMultiplier">The value to multiply Scale3D with</param>
        public void MultiplyScale3D(FVector scale3DMultiplier)
        {
            Scale3D *= scale3DMultiplier;
            DiagnosticCheckNaN_Scale3D();
        }

        /// <summary>
        /// Sets the translation component
        /// </summary>
        /// <param name="newTranslation">The new value for the translation component</param>
        public void SetTranslation(FVector newTranslation)
        {
            Translation = newTranslation;
            DiagnosticCheckNaN_Translate();
        }

        /// <summary>
        /// Copy translation from another FTransform.
        /// </summary>
        public void CopyTranslation(FTransform other)
        {
            Translation = other.Translation;
        }

        /// <summary>
        /// Concatenates another rotation to this transformation
        /// </summary>
        /// <param name="deltaRotation">The rotation to concatenate in the following fashion: Rotation = Rotation * DeltaRotation</param>
        public void ConcatenateRotation(FQuat deltaRotation)
        {
            Rotation = Rotation * deltaRotation;
            DiagnosticCheckNaN_Rotate();
        }

        /// <summary>
        /// Adjusts the translation component of this transformation
        /// </summary>
        /// <param name="deltaTranslation">The translation to add in the following fashion: Translation += DeltaTranslation</param>
        public void AddToTranslation(FVector deltaTranslation)
        {
            Translation += deltaTranslation;
            DiagnosticCheckNaN_Translate();
        }

        /// <summary>
        /// Add the translations from two FTransforms and return the result.
        /// </summary>
        /// <param name="a">The first transform.</param>
        /// <param name="b">The second transform.</param>
        /// <returns>a.Translation + b.Translation</returns>
        public static FVector AddTranslations(FTransform a, FTransform b)
        {
            return a.Translation + b.Translation;
        }

        /// <summary>
        /// Subtract translations from two FTransforms and return the difference.
        /// </summary>
        /// <param name="a">The first transform.</param>
        /// <param name="b">The second transform.</param>
        /// <returns>a.Translation - b.Translation.</returns>
        public static FVector SubtractTranslations(FTransform a, FTransform b)
        {
            return a.Translation - b.Translation;
        }

        /// <summary>
        /// Sets the rotation component
        /// </summary>
        /// <param name="newRotation">The new value for the rotation component</param>
        public void SetRotation(FQuat newRotation)
        {
            Rotation = newRotation;
            DiagnosticCheckNaN_Rotate();
        }

        /// <summary>
        /// Copy rotation from another FTransform.
        /// </summary>
        /// <param name="other"></param>
        public void CopyRotation(FTransform other)
        {
            Rotation = other.Rotation;
        }

        /// <summary>
        /// Sets the Scale3D component
        /// </summary>
        /// <param name="newScale3D">The new value for the Scale3D component</param>
        public void SetScale3D(FVector newScale3D)
        {
            Scale3D = newScale3D;
            DiagnosticCheckNaN_Scale3D();
        }

        /// <summary>
        /// Copy scale from another FTransform.
        /// </summary>
        /// <param name="other"></param>
        public void CopyScale3D(FTransform other)
        {
            Scale3D = other.Scale3D;
        }

        /// <summary>
        /// Sets both the translation and Scale3D components at the same time
        /// </summary>
        /// <param name="newTranslation">The new value for the translation component</param>
        /// <param name="newScale3D">The new value for the Scale3D component</param>
        public void SetTranslationAndScale3D(FVector newTranslation, FVector newScale3D)
        {
            Translation = newTranslation;
            Scale3D = newScale3D;

            DiagnosticCheckNaN_Translate();
            DiagnosticCheckNaN_Scale3D();
        }

        /// <summary>
        /// Accumulates another transform with this one
        /// 
        /// Rotation is accumulated multiplicatively (Rotation = SourceAtom.Rotation * Rotation)
        /// Translation is accumulated additively (Translation += SourceAtom.Translation)
        /// Scale3D is accumulated multiplicatively (Scale3D *= SourceAtom.Scale3D)
        /// </summary>
        /// <param name="sourceAtom">The other transform to accumulate into this one</param>
        public void Accumulate(FTransform sourceAtom)
        {
            // Add ref pose relative animation to base animation, only if rotation is significant.
            if (FMath.Square(sourceAtom.Rotation.W) < 1.0f - FMath.Delta * FMath.Delta)
            {
                Rotation = sourceAtom.Rotation * Rotation;
            }

            Translation += sourceAtom.Translation;
            Scale3D *= sourceAtom.Scale3D;

            DiagnosticCheckNaN_All();

            Debug.Assert(IsRotationNormalized());
        }

        /// <summary>
        /// Accumulates another transform with this one, with a blending weight
        /// 
        /// Let SourceAtom = Atom * BlendWeight
        /// Rotation is accumulated multiplicatively (Rotation = SourceAtom.Rotation * Rotation).
        /// Translation is accumulated additively (Translation += SourceAtom.Translation)
        /// Scale3D is accumulated multiplicatively (Scale3D *= SourceAtom.Scale3D)
        /// 
        /// Note: Rotation will not be normalized! Will have to be done manually.
        /// </summary>
        /// <param name="atom">The other transform to accumulate into this one</param>
        /// <param name="blendWeight">The weight to multiply Atom by before it is accumulated.</param>
        public void Accumulate(FTransform atom, float blendWeight)
        {
            FTransform sourceAtom = atom * blendWeight;

            // Add ref pose relative animation to base animation, only if rotation is significant.
            if (FMath.Square(sourceAtom.Rotation.W) < 1.0f - FMath.Delta * FMath.Delta)
            {
                Rotation = sourceAtom.Rotation * Rotation;
            }

            Translation += sourceAtom.Translation;
            Scale3D *= sourceAtom.Scale3D;

            DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Accumulates another transform with this one, with an optional blending weight
        /// 
        /// Rotation is accumulated additively, in the shortest direction (Rotation = Rotation +/- DeltaAtom.Rotation * Weight)
        /// Translation is accumulated additively (Translation += DeltaAtom.Translation * Weight)
        /// Scale3D is accumulated additively (Scale3D += DeltaAtom.Scale3D * Weight)
        /// </summary>
        /// <param name="deltaAtom">The other transform to accumulate into this one</param>
        /// <param name="blendWeight">The weight to multiply DeltaAtom by before it is accumulated.</param>
        public void AccumulateWithShortestRotation(FTransform deltaAtom, float blendWeight)
        {
            FTransform atom = deltaAtom * blendWeight;

            // To ensure the 'shortest route', we make sure the dot product between the accumulator and the incoming child atom is positive.
            if ((atom.Rotation | Rotation) < 0.0f)
            {
                Rotation.X -= atom.Rotation.X;
                Rotation.Y -= atom.Rotation.Y;
                Rotation.Z -= atom.Rotation.Z;
                Rotation.W -= atom.Rotation.W;
            }
            else
            {
                Rotation.X += atom.Rotation.X;
                Rotation.Y += atom.Rotation.Y;
                Rotation.Z += atom.Rotation.Z;
                Rotation.W += atom.Rotation.W;
            }

            Translation += atom.Translation;
            Scale3D += atom.Scale3D;

            DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Accumulates another transform with this one, with a blending weight
        /// 
        /// Let SourceAtom = Atom * BlendWeight
        /// Rotation is accumulated multiplicatively (Rotation = SourceAtom.Rotation * Rotation).
        /// Translation is accumulated additively (Translation += SourceAtom.Translation)
        /// Scale3D is accumulated assuming incoming scale is additive scale (Scale3D *= (1 + SourceAtom.Scale3D))
        /// 
        /// When we create additive, we create additive scale based on [TargetScale/SourceScale -1]
        /// because that way when you apply weight of 0.3, you don't shrink. We only saves the % of grow/shrink
        /// when we apply that back to it, we add back the 1, so that it goes back to it.
        /// This solves issue where you blend two additives with 0.3, you don't come back to 0.6 scale, but 1 scale at the end
        /// because [1 + [1-1]*0.3 + [1-1]*0.3] becomes 1, so you don't shrink by applying additive scale
        /// 
        /// Note: Rotation will not be normalized! Will have to be done manually.
        /// </summary>
        /// <param name="atom">The other transform to accumulate into this one</param>
        /// <param name="blendWeight">The weight to multiply Atom by before it is accumulated.</param>
        public void AccumulateWithAdditiveScale(FTransform atom, float blendWeight)
        {
            FVector DefaultScale = FVector.OneVector;

            FTransform sourceAtom = atom * blendWeight;

            // Add ref pose relative animation to base animation, only if rotation is significant.
            if (FMath.Square(sourceAtom.Rotation.W) < 1.0f - FMath.Delta * FMath.Delta)
            {
                Rotation = sourceAtom.Rotation * Rotation;
            }

            Translation += sourceAtom.Translation;
            Scale3D *= (DefaultScale + sourceAtom.Scale3D);

            DiagnosticCheckNaN_All();
        }

        /// <summary>
        /// Set the translation and Scale3D components of this transform to a linearly interpolated combination of two other transforms
        /// 
        /// Translation = FMath::Lerp(SourceAtom1.Translation, SourceAtom2.Translation, Alpha)
        /// Scale3D = FMath::Lerp(SourceAtom1.Scale3D, SourceAtom2.Scale3D, Alpha)
        /// </summary>
        /// <param name="sourceAtom1">The starting point source atom (used 100% if Alpha is 0)</param>
        /// <param name="sourceAtom2">The ending point source atom (used 100% if Alpha is 1)</param>
        /// <param name="alpha">The blending weight between SourceAtom1 and SourceAtom2</param>
        public void LerpTranslationScale3D(FTransform sourceAtom1, FTransform sourceAtom2, float alpha)
        {
            Translation = FMath.Lerp(sourceAtom1.Translation, sourceAtom2.Translation, alpha);
            Scale3D = FMath.Lerp(sourceAtom1.Scale3D, sourceAtom2.Scale3D, alpha);

            DiagnosticCheckNaN_Translate();
            DiagnosticCheckNaN_Scale3D();
        }

        /// <summary>
        /// Normalize the rotation component of this transformation
        /// </summary>
        public void NormalizeRotation()
        {
            Rotation.Normalize();
            DiagnosticCheckNaN_Rotate();
        }

        /// <summary>
        /// Checks whether the rotation component is normalized or not
        /// </summary>
        /// <returns>true if the rotation component is normalized, and false otherwise.</returns>
        public bool IsRotationNormalized()
        {
            return Rotation.IsNormalized();
        }

        /// <summary>
        /// Blends the Identity transform with a weighted source transform and accumulates that into a destination transform
        /// 
        /// SourceAtom = Blend(Identity, SourceAtom, BlendWeight)
        /// FinalAtom.Rotation = SourceAtom.Rotation * FinalAtom.Rotation
        /// FinalAtom.Translation += SourceAtom.Translation
        /// FinalAtom.Scale3D *= SourceAtom.Scale3D
        /// </summary>
        /// <param name="finalAtom">The atom to accumulate the blended source atom into</param>
        /// <param name="sourceAtom">The target transformation (used when BlendWeight = 1); this is modified during the process</param>
        /// <param name="blendWeight">The blend weight between Identity and SourceAtom</param>
        public static void BlendFromIdentityAndAccumulate(ref FTransform finalAtom, ref FTransform sourceAtom, float blendWeight)
        {
            FTransform AdditiveIdentity = new FTransform(FQuat.Identity, FVector.ZeroVector, FVector.ZeroVector);
            FVector DefaultScale = FVector.OneVector;

            // Scale delta by weight
            if (blendWeight < (1.0f - ZeroAnimWeightThresh))
            {
                sourceAtom.Blend(AdditiveIdentity, sourceAtom, blendWeight);
            }

            // Add ref pose relative animation to base animation, only if rotation is significant.
            if (FMath.Square(sourceAtom.Rotation.W) < 1.0f - FMath.Delta * FMath.Delta)
            {
                finalAtom.Rotation = sourceAtom.Rotation * finalAtom.Rotation;
            }

            finalAtom.Translation += sourceAtom.Translation;
            finalAtom.Scale3D *= (DefaultScale + sourceAtom.Scale3D);

            finalAtom.DiagnosticCheckNaN_All();

            Debug.Assert(finalAtom.IsRotationNormalized());
        }

        /// <summary>
        /// Returns the rotation component
        /// </summary>
        /// <returns>The rotation component</returns>
        public FQuat GetRotation()
        {
            DiagnosticCheckNaN_Rotate();
            return Rotation;
        }

        /// <summary>
        /// Returns the translation component
        /// </summary>
        /// <returns>The translation component</returns>
        public FVector GetTranslation()
        {
            DiagnosticCheckNaN_Translate();
            return Translation;
        }

        /// <summary>
        /// Returns the Scale3D component
        /// </summary>
        /// <returns>The Scale3D component</returns>
        public FVector GetScale3D()
        {
            DiagnosticCheckNaN_Scale3D();
            return Scale3D;
        }

        /// <summary>
        /// Sets the Rotation and Scale3D of this transformation from another transform
        /// </summary>
        /// <param name="srcBA">The transform to copy rotation and Scale3D from</param>
        public void CopyRotationPart(FTransform srcBA)
        {
            Rotation = srcBA.Rotation;
            Scale3D = srcBA.Scale3D;

            DiagnosticCheckNaN_Rotate();
            DiagnosticCheckNaN_Scale3D();
        }

        /// <summary>
        /// Sets the Translation and Scale3D of this transformation from another transform
        /// </summary>
        /// <param name="srcBA">The transform to copy translation and Scale3D from</param>
        public void CopyTranslationAndScale3D(FTransform srcBA)
        {
            Translation = srcBA.Translation;
            Scale3D = srcBA.Scale3D;

            DiagnosticCheckNaN_Translate();
            DiagnosticCheckNaN_Scale3D();
        }

        public void SetFromMatrix(FMatrix matrix)
        {
            FMatrix m = matrix;

            // Get the 3D scale from the matrix
            Scale3D = m.ExtractScaling();

            // If there is negative scaling going on, we handle that here
            if (matrix.Determinant() < 0.0f)
            {
                // Assume it is along X and modify transform accordingly. 
                // It doesn't actually matter which axis we choose, the 'appearance' will be the same
                Scale3D.X *= -1.0f;
                m.SetAxis(0, -m.GetScaledAxis(EAxis.X));
            }

            Rotation = new FQuat(m);
            Translation = matrix.GetOrigin();

            // Normalize rotation
            Rotation.Normalize();
        }
    }
}
