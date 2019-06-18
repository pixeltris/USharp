using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\OrientedBox.h

    /// <summary>
    /// Structure for arbitrarily oriented boxes (not necessarily axis-aligned).
    /// </summary>
    [UStruct(Flags = 0x00006038), UMetaPath("/Script/CoreUObject.OrientedBox")]
    [StructLayout(LayoutKind.Sequential)]
    public struct FOrientedBox : IEquatable<FOrientedBox>
    {
        static bool Center_IsValid;
        static int Center_Offset;
        /// <summary>
        /// Holds the center of the box.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000001), UMetaPath("/Script/CoreUObject.OrientedBox:Center")]
        public FVector Center;

        static bool AxisX_IsValid;
        static int AxisX_Offset;
        /// <summary>
        /// Holds the x-axis vector of the box. Must be a unit vector.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000001), UMetaPath("/Script/CoreUObject.OrientedBox:AxisX")]
        public FVector AxisX;

        static bool AxisY_IsValid;
        static int AxisY_Offset;
        /// <summary>
        /// Holds the y-axis vector of the box. Must be a unit vector.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000001), UMetaPath("/Script/CoreUObject.OrientedBox:AxisY")]
        public FVector AxisY;

        static bool AxisZ_IsValid;
        static int AxisZ_Offset;
        /// <summary>
        /// Holds the z-axis vector of the box. Must be a unit vector.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000001), UMetaPath("/Script/CoreUObject.OrientedBox:AxisZ")]
        public FVector AxisZ;

        static bool ExtentX_IsValid;
        static int ExtentX_Offset;
        /// <summary>
        /// Holds the extent of the box along its x-axis.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000201), UMetaPath("/Script/CoreUObject.OrientedBox:ExtentX")]
        public float ExtentX;

        static bool ExtentY_IsValid;
        static int ExtentY_Offset;
        /// <summary>
        /// Holds the extent of the box along its y-axis.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000201), UMetaPath("/Script/CoreUObject.OrientedBox:ExtentY")]
        public float ExtentY;

        static bool ExtentZ_IsValid;
        static int ExtentZ_Offset;
        /// <summary>
        /// Holds the extent of the box along its z-axis.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000201), UMetaPath("/Script/CoreUObject.OrientedBox:ExtentZ")]
        public float ExtentZ;

        static int FOrientedBox_StructSize;

        public FOrientedBox Copy()
        {
            FOrientedBox result = this;
            return result;
        }

        static FOrientedBox()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FOrientedBox)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FOrientedBox));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.OrientedBox");
            FOrientedBox_StructSize = NativeReflection.GetStructSize(classAddress);
            Center_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Center");
            Center_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Center", Classes.UStructProperty);
            AxisX_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "AxisX");
            AxisX_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "AxisX", Classes.UStructProperty);
            AxisY_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "AxisY");
            AxisY_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "AxisY", Classes.UStructProperty);
            AxisZ_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "AxisZ");
            AxisZ_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "AxisZ", Classes.UStructProperty);
            ExtentX_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "ExtentX");
            ExtentX_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "ExtentX", Classes.UFloatProperty);
            ExtentY_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "ExtentY");
            ExtentY_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "ExtentY", Classes.UFloatProperty);
            ExtentZ_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "ExtentZ");
            ExtentZ_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "ExtentZ", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FOrientedBox));
        }

        public static readonly FOrientedBox Default = new FOrientedBox()
        {
            Center = new FVector(0.0f),
            AxisX = new FVector(1.0f, 0.0f, 0.0f),
            AxisY = new FVector(0.0f, 1.0f, 0.0f),
            AxisZ = new FVector(0.0f, 0.0f, 1.0f),
            ExtentX = 1.0f,
            ExtentY = 1.0f,
            ExtentZ = 1.0f
        };

        /// <summary>
        /// Fills in the Verts array with the eight vertices of the box.
        /// </summary>
        /// <param name="verts">The array to fill in with the vertices.</param>
        public void CalcVertices(FVector[] verts)
        {
            float[] signs = { -1.0f, 1.0f };

            int index = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        verts[index] = Center + signs[i] * AxisX * ExtentX + signs[j] * AxisY * ExtentY + signs[k] * AxisZ * ExtentZ;
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Finds the projection interval of the box when projected onto Axis.
        /// </summary>
        /// <param name="axis">The unit vector defining the axis to project the box onto.</param>
        public FFloatInterval Project(FVector axis)
        {
            float[] signs = { -1.0f, 1.0f };

            // calculate the projections of the box center and the extent-scaled axes.
            float projectedCenter = axis | Center;
            float projectedAxisX = axis | (ExtentX * AxisX);
            float projectedAxisY = axis | (ExtentY * AxisY);
            float projectedAxisZ = axis | (ExtentZ * AxisZ);

            FFloatInterval projectionInterval = FFloatInterval.Default;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        // project the box vertex onto the axis.
                        float projectedVertex = projectedCenter + signs[i] * projectedAxisX + signs[j] * projectedAxisY + signs[k] * projectedAxisZ;

                        // if necessary, expand the projection interval to include the box vertex projection.
                        projectionInterval.Include(projectedVertex);
                    }
                }
            }

            return projectionInterval;
        }

        /// <summary>
        /// Compares two boxes for equality.
        /// </summary>
        /// <param name="a">The first box.</param>
        /// <param name="b">The second box.</param>
        /// <returns>true if the boxes are equal, false otherwise.</returns>
        public static bool operator ==(FOrientedBox a, FOrientedBox b)
        {
            return a.Center == b.Center && 
                a.AxisX == b.AxisX && a.AxisY == b.AxisY && a.AxisZ == b.AxisZ &&
                a.ExtentX == b.ExtentX && a.ExtentY == b.ExtentY && a.ExtentZ == b.ExtentZ;
        }

        /// <summary>
        /// Compares two boxes for inequality.
        /// </summary>
        /// <param name="a">The first box.</param>
        /// <param name="b">The second box.</param>
        /// <returns>true if the boxes are different, false otherwise.</returns>
        public static bool operator !=(FOrientedBox a, FOrientedBox b)
        {
            return a.Center != b.Center ||
                a.AxisX != b.AxisX || a.AxisY != b.AxisY || a.AxisZ != b.AxisZ ||
                a.ExtentX != b.ExtentX || a.ExtentY != b.ExtentY || a.ExtentZ != b.ExtentZ;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FOrientedBox))
            {
                return false;
            }

            return Equals((FOrientedBox)obj);
        }

        public bool Equals(FOrientedBox other)
        {
            return Center == other.Center &&
                AxisX == other.AxisX && AxisY == other.AxisY && AxisZ == other.AxisZ &&
                ExtentX == other.ExtentX && ExtentY == other.ExtentY && ExtentZ == other.ExtentZ;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = Center.GetHashCode();
                hashcode = (hashcode * 397) ^ AxisX.GetHashCode();
                hashcode = (hashcode * 397) ^ AxisY.GetHashCode();
                hashcode = (hashcode * 397) ^ AxisZ.GetHashCode();
                hashcode = (hashcode * 397) ^ ExtentX.GetHashCode();
                hashcode = (hashcode * 397) ^ ExtentY.GetHashCode();
                hashcode = (hashcode * 397) ^ ExtentZ.GetHashCode();
                return hashcode;
            }
        }
    }
}
