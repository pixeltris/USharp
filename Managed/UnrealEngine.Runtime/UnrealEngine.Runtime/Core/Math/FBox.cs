using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Box.h

    /// <summary>
    /// Implements an axis-aligned box.
    /// 
    /// Boxes describe an axis-aligned extent in three dimensions. They are used for many different things in the
    /// Engine and in games, such as bounding volumes, collision detection and visibility calculation.
    /// </summary>
    [UStruct(Flags = 0x0000E838), BlueprintType, UMetaPath("/Script/CoreUObject.Box", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FBox : IEquatable<FBox>
    {
        static bool Min_IsValid;
        static int Min_Offset;
        /// <summary>
        /// Holds the box's minimum point.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000005), UMetaPath("/Script/CoreUObject.Box:Min")]
        public FVector Min;

        static bool Max_IsValid;
        static int Max_Offset;
        /// <summary>
        /// Holds the box's maximum point.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000005), UMetaPath("/Script/CoreUObject.Box:Max")]
        public FVector Max;

        static bool IsValid_IsValid;
        static int IsValid_Offset;
        /// <summary>
        /// Holds a flag indicating whether this box is valid.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000200), UMetaPath("/Script/CoreUObject.Box:IsValid")]
        private byte isValid;

        /// <summary>
        /// Holds a flag indicating whether this box is valid.
        /// </summary>
        public bool IsValid
        {
            get { return isValid != 0; }
            set { isValid = (byte)(value ? 1 : 0); }
        }

        static int FBox_StructSize;

        public FBox Copy()
        {
            FBox result = this;
            return result;
        }

        static FBox()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FBox)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FBox));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Box");
            FBox_StructSize = NativeReflection.GetStructSize(classAddress);
            Min_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Min");
            Min_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Min", Classes.UStructProperty);
            Max_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Max");
            Max_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Max", Classes.UStructProperty);
            IsValid_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "IsValid");
            IsValid_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "IsValid", Classes.UByteProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FBox));
        }

        /// <summary>
        /// Creates and initializes a new box from the specified extents.
        /// </summary>
        /// <param name="min">The box's minimum point.</param>
        /// <param name="max">The box's maximum point.</param>
        public FBox(FVector min, FVector max)
        {
            Min = min;
            Max = max;
            isValid = 1;
        }

        /// <summary>
        /// Creates and initializes a new box from an array of points.
        /// </summary>
        /// <param name="points">Array of Points to create for the bounding volume.</param>
        public FBox(FVector[] points)
        {
            Min = Max = FVector.ZeroVector;
            isValid = 0;
            for (int i = 0; i < points.Length; i++)
            {
                this += points[i];
            }
        }

        /// <summary>
        /// Compares two boxes for equality.
        /// </summary>
        /// <param name="a">The first box.</param>
        /// <param name="b">The second box.</param>
        /// <returns>true if the boxes are equal, false otherwise.</returns>
        public static bool operator ==(FBox a, FBox b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        /// <summary>
        /// Compare two boxes for inequality.
        /// </summary>
        /// <param name="a">The first box.</param>
        /// <param name="b">The second box.</param>
        /// <returns>true if the boxes are not equal, false otherwise.</returns>
        public static bool operator !=(FBox a, FBox b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FBox))
            {
                return false;
            }

            return Equals((FBox)obj);
        }

        public bool Equals(FBox other)
        {
            return Min == other.Min && Max == other.Max;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = Min.GetHashCode();
                hashcode = (hashcode * 397) ^ Max.GetHashCode();
                hashcode = (hashcode * 397) ^ isValid.GetHashCode();
                return hashcode;
            }
        }

        /// <summary>
        /// Adds to the bounding box to include a given point.
        /// </summary>
        /// <param name="box">The rectangle.</param>
        /// <param name="other">The point to increase the bounding volume to.</param>
        /// <returns>The bounding box after resizing to include the other point.</returns>
        public static FBox operator +(FBox box, FVector other)
        {
            Add(ref box, ref other, out box);
            return box;
        }

        /// <summary>
        /// Adds to the bounding box to include a given point.
        /// </summary>
        /// <param name="box">The rectangle.</param>
        /// <param name="other">The point to increase the bounding volume to.</param>
        /// <returns>The bounding box after resizing to include the other point.</returns>
        public static FBox Add(FBox box, FVector other)
        {
            Add(ref box, ref other, out box);
            return box;
        }

        /// <summary>
        /// Adds to the bounding box to include a given point.
        /// </summary>
        /// <param name="box">The rectangle.</param>
        /// <param name="other">The point to increase the bounding volume to.</param>
        /// <param name="result">The bounding box after resizing to include the other point.</param>
        public static void Add(ref FBox box, ref FVector other, out FBox result)
        {
            if (box.IsValid)
            {
                result.Min.X = FMath.Min(box.Min.X, other.X);
                result.Min.Y = FMath.Min(box.Min.Y, other.Y);
                result.Min.Z = FMath.Min(box.Min.Z, other.Z);

                result.Max.X = FMath.Max(box.Max.X, other.X);
                result.Max.Y = FMath.Max(box.Max.Y, other.Y);
                result.Max.Z = FMath.Max(box.Max.Z, other.Z);

                result.isValid = 1;
            }
            else
            {
                result.Min = result.Max = other;
                result.isValid = 1;
            }
        }

        /// <summary>
        /// Gets the result of adding two bounding boxes together.
        /// </summary>
        /// <param name="a">The first bounding box.</param>
        /// <param name="b">The second bounding box.</param>
        /// <returns>A new bounding volume.</returns>
        public static FBox operator +(FBox a, FBox b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of adding two bounding boxes together.
        /// </summary>
        /// <param name="a">The first bounding box.</param>
        /// <param name="b">The second bounding box.</param>
        /// <returns>A new bounding volume.</returns>
        public static FBox Add(FBox a, FBox b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of adding two bounding boxes together.
        /// </summary>
        /// <param name="a">The first bounding box.</param>
        /// <param name="b">The second bounding box.</param>
        /// <param name="result">A new bounding volume.</param>
        public static void Add(ref FBox a, ref FBox b, out FBox result)
        {
            if (a.IsValid && b.IsValid)
            {
                result.Min.X = FMath.Min(a.Min.X, b.Min.X);
                result.Min.Y = FMath.Min(a.Min.Y, b.Min.Y);
                result.Min.Z = FMath.Min(a.Min.Z, b.Min.Z);

                result.Max.X = FMath.Max(a.Max.X, b.Max.X);
                result.Max.Y = FMath.Max(a.Max.Y, b.Max.Y);
                result.Max.Z = FMath.Max(a.Max.Z, b.Max.Z);

                result.isValid = 1;
            }
            else if (b.IsValid)
            {
                result = b;
            }
            else
            {
                result = a;
            }
        }

        /// <summary>
        /// Gets either the min or max of this bounding box.
        /// </summary>
        /// <param name="index">the index into points of the bounding box.</param>
        /// <returns>Copy of the point of the bounding box.</returns>
        public FVector this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Min;
                    case 1: return Max;
                    default:
                        throw new IndexOutOfRangeException("Invalid FBox index (" + index + ")");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: Min = value; break;
                    case 1: Max = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FBox index (" + index + ")");
                }
            }
        }

        /// <summary>
        /// Calculates the distance of a point to this box.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The distance.</returns>
        public float ComputeSquaredDistanceToPoint(FVector point)
        {
            return FVector.ComputeSquaredDistanceFromBoxToPoint(Min, Max, point);
        }

        /// <summary>
        /// Increases the box size.
        /// </summary>
        /// <param name="w">The size to increase the volume by.</param>
        /// <returns>A new bounding box.</returns>
        public FBox ExpandBy(float w)
        {
            return new FBox(Min - new FVector(w, w, w), Max + new FVector(w, w, w));
        }

        /// <summary>
        /// Increases the box size.
        /// </summary>
        /// <param name="v">The size to increase the volume by.</param>
        /// <returns>A new bounding box.</returns>
        public FBox ExpandBy(FVector v)
        {
            return new FBox(Min - v, Max + v);
        }

        /// <summary>
        /// Increases the box size.
        /// </summary>
        /// <param name="neg">The size to increase the volume by in the negative direction (positive values move the bounds outwards)</param>
        /// <param name="pos">The size to increase the volume by in the positive direction (positive values move the bounds outwards)</param>
        /// <returns>A new bounding box.</returns>
        public FBox ExpandBy(FVector neg, FVector pos)
        {
            return new FBox(Min - neg, Max + pos);
        }

        /// <summary>
        /// Shifts the bounding box position.
        /// </summary>
        /// <param name="offset">The vector to shift the box by.</param>
        /// <returns>A new bounding box.</returns>
        public FBox ShiftBy(FVector offset)
        {
            return new FBox(Min + offset, Max + offset);
        }

        /// <summary>
        /// Moves the center of bounding box to new destination.
        /// </summary>
        /// <param name="destination">The destination point to move center of box to.</param>
        /// <returns>A new bounding box.</returns>
        public FBox MoveTo(FVector destination)
        {
            FVector offset = destination - GetCenter();
            return new FBox(Min + offset, Max + offset);
        }

        /// <summary>
        /// Gets the center point of this box.
        /// </summary>
        /// <returns>The center point.</returns>
        /// <see cref="GetCenterAndExtents"/>
        /// <see cref="GetExtent"/>
        /// <see cref="GetSize"/>
        /// <see cref="GetVolume"/>
        public FVector GetCenter()
        {
            return (Min + Max) * 0.5f;
        }

        /// <summary>
        /// Gets the center and extents of this box.
        /// </summary>
        /// <param name="center">Will contain the box center point.</param>
        /// <param name="extents">Will contain the extent around the center.</param>
        /// <see cref="GetCenter"/>
        /// <see cref="GetExtent"/>
        /// <see cref="GetSize"/>
        /// <see cref="GetVolume"/>
        public void GetCenterAndExtents(out FVector center, out FVector extents)
        {
            extents = GetExtent();
            center = Min + extents;
        }

        /// <summary>
        /// Calculates the closest point on or inside the box to a given point in space.
        /// </summary>
        /// <param name="point">The point in space.</param>
        /// <returns>The closest point on or inside the box.</returns>
        public FVector GetClosestPointTo(FVector point)
        {
            // start by considering the point inside the box
            FVector closestPoint = point;

            // now clamp to inside box if it's outside
            if (point.X < Min.X)
            {
                closestPoint.X = Min.X;
            }
            else if (point.X > Max.X)
            {
                closestPoint.X = Max.X;
            }

            // now clamp to inside box if it's outside
            if (point.Y < Min.Y)
            {
                closestPoint.Y = Min.Y;
            }
            else if (point.Y > Max.Y)
            {
                closestPoint.Y = Max.Y;
            }

            // Now clamp to inside box if it's outside.
            if (point.Z < Min.Z)
            {
                closestPoint.Z = Min.Z;
            }
            else if (point.Z > Max.Z)
            {
                closestPoint.Z = Max.Z;
            }

            return closestPoint;
        }

        /// <summary>
        /// Gets the extents of this box.
        /// </summary>
        /// <returns>The box extents.</returns>
        /// <see cref="GetCenter"/>
        /// <see cref="GetCenterAndExtents"/>
        /// <see cref="GetSize"/>
        /// <see cref="GetVolume"/>
        public FVector GetExtent()
        {
            return 0.5f * (Max - Min);
        }

        /// <summary>
        /// Gets a reference to the specified point of the bounding box.
        /// </summary>
        /// <param name="pointIndex">The index of the extrema point to return.</param>
        /// <returns>A reference to the point.</returns>
        public FVector GetExtrema(int pointIndex)
        {
            return this[pointIndex];
        }

        /// <summary>
        /// Gets the size of this box.
        /// </summary>
        /// <returns>The box size.</returns>
        /// <see cref="GetCenter"/>
        /// <see cref="GetCenterAndExtents"/>
        /// <see cref="GetExtent"/> 
        /// <see cref="GetVolume"/> 
        public FVector GetSize()
        {
            return (Max - Min);
        }

        /// <summary>
        /// Gets the volume of this box.
        /// </summary>
        /// <returns>The box volume.</returns>
        /// <see cref="GetCenter"/>
        /// <see cref="GetCenterAndExtents"/> 
        /// <see cref="GetExtent"/> 
        /// <see cref="GetSize"/> 
        public float GetVolume()
        {
            return ((Max.X - Min.X) * (Max.Y - Min.Y) * (Max.Z - Min.Z));
        }

        /// <summary>
        /// Set the initial values of the bounding box to Zero.
        /// </summary>
        public void Init()
        {
            Min = Max = FVector.ZeroVector;
            isValid = 0;
        }

        /// <summary>
        /// Checks whether the given bounding box intersects this bounding box.
        /// </summary>
        /// <param name="other">The bounding box to intersect with.</param>
        /// <returns>true if the boxes intersect, false otherwise.</returns>
        public bool Intersect(FBox other)
        {
            if ((Min.X > other.Max.X) || (other.Min.X > Max.X))
            {
                return false;
            }

            if ((Min.Y > other.Max.Y) || (other.Min.Y > Max.Y))
            {
                return false;
            }

            if ((Min.Z > other.Max.Z) || (other.Min.Z > Max.Z))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether the given bounding box intersects this bounding box in the XY plane.
        /// </summary>
        /// <param name="other">The bounding box to test intersection.</param>
        /// <returns>true if the boxes intersect in the XY Plane, false otherwise.</returns>
        public bool IntersectXY(FBox other)
        {
            if ((Min.X > other.Max.X) || (other.Min.X > Max.X))
            {
                return false;
            }

            if ((Min.Y > other.Max.Y) || (other.Min.Y > Max.Y))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the overlap FBox of two box
        /// </summary>
        /// <param name="other">The bounding box to test overlap</param>
        /// <returns>the overlap box. It can be 0 if they don't overlap</returns>
        public FBox Overlap(FBox other)
        {
            if (Intersect(other) == false)
            {
                return default(FBox);
            }

            // otherwise they overlap
            // so find overlapping box
            FVector minVector, maxVector;

            minVector.X = FMath.Max(Min.X, other.Min.X);
            maxVector.X = FMath.Min(Max.X, other.Max.X);

            minVector.Y = FMath.Max(Min.Y, other.Min.Y);
            maxVector.Y = FMath.Min(Max.Y, other.Max.Y);

            minVector.Z = FMath.Max(Min.Z, other.Min.Z);
            maxVector.Z = FMath.Min(Max.Z, other.Max.Z);

            return new FBox(minVector, maxVector);
        }

        /// <summary>
        /// Gets a bounding volume transformed by an inverted FTransform object.
        /// </summary>
        /// <param name="m">The transformation object to perform the inversely transform this box with.</param>
        /// <returns>The transformed box.</returns>
        public FBox InverseTransformBy(FTransform m)
        {
            FVector[] vertices = new FVector[8]
                {
                    Min,
                    new FVector(Min.X, Min.Y, Max.Z),
                    new FVector(Min.X, Max.Y, Min.Z),
                    new FVector(Max.X, Min.Y, Min.Z),
                    new FVector(Max.X, Max.Y, Min.Z),
                    new FVector(Max.X, Min.Y, Max.Z),
                    new FVector(Min.X, Max.Y, Max.Z),
                    Max
                };

            FBox newBox = default(FBox);

            for (int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++)
            {
                FVector4 projectedVertex = m.InverseTransformPosition(vertices[vertexIndex]);
                newBox += (FVector)projectedVertex;
            }

            return newBox;
        }

        /// <summary>
        /// Checks whether the given location is inside this box.
        /// </summary>
        /// <param name="v">The location to test for inside the bounding volume.</param>
        /// <returns>true if location is inside this volume.</returns>
        public bool IsInside(FVector v)
        {
            return ((v.X > Min.X) && (v.X < Max.X) && (v.Y > Min.Y) && (v.Y < Max.Y) && (v.Z > Min.Z) && (v.Z < Max.Z));
        }

        /// <summary>
        /// Checks whether the given location is inside or on this box.
        /// </summary>
        /// <param name="v">The location to test for inside the bounding volume.</param>
        /// <returns>true if location is inside this volume.</returns>
        /// <see cref="IsInsideXY(FVector)"/>
        public bool IsInsideOrOn(FVector v)
        {
            return ((v.X >= Min.X) && (v.X <= Max.X) && (v.Y >= Min.Y) && (v.Y <= Max.Y) && (v.Z >= Min.Z) && (v.Z <= Max.Z));
        }

        /// <summary>
        /// Checks whether a given box is fully encapsulated by this box.
        /// </summary>
        /// <param name="other">The box to test for encapsulation within the bounding volume.</param>
        /// <returns>true if box is inside this volume.</returns>
        public bool IsInside(FBox other)
        {
            return (IsInside(other.Min) && IsInside(other.Max));
        }

        /// <summary>
        /// Checks whether the given location is inside this box in the XY plane.
        /// </summary>
        /// <param name="v">The location to test for inside the bounding box.</param>
        /// <returns>true if location is inside this box in the XY plane.</returns>
        /// <see cref="IsInside(FVector)"/>
        public bool IsInsideXY(FVector v)
        {
            return ((v.X > Min.X) && (v.X < Max.X) && (v.Y > Min.Y) && (v.Y < Max.Y));
        }

        /// <summary>
        /// Checks whether the given box is fully encapsulated by this box in the XY plane.
        /// </summary>
        /// <param name="other">The box to test for encapsulation within the bounding box.</param>
        /// <returns>true if box is inside this box in the XY plane.</returns>
        public bool IsInsideXY(FBox other)
        {
            return (IsInsideXY(other.Min) && IsInsideXY(other.Max));
        }

        /// <summary>
        /// Gets a bounding volume transformed by a matrix.
        /// </summary>
        /// <param name="m">The matrix to transform by.</param>
        /// <returns>The transformed box.</returns>
        /// <see cref="TransformProjectBy"/>
        public FBox TransformBy(FMatrix m)
        {
            // if we are not valid, return another invalid box.
            if (!IsValid)
            {
                return default(FBox);
            }

            FVector vecMin = Min;
            FVector vecMax = Max;

            FVector m0 = m.GetRow(0);
            FVector m1 = m.GetRow(1);
            FVector m2 = m.GetRow(2);
            FVector m3 = m.GetRow(3);

            FVector half = new FVector(0.5f, 0.5f, 0.5f);
            FVector origin = (vecMax + vecMin) * half;
            FVector extent = (vecMax - vecMin) * half;

            FVector newOrigin = FVector.Replicate(origin, 0) * m0;
            newOrigin += (FVector.Replicate(origin, 1) * m1);
            newOrigin += (FVector.Replicate(origin, 2) * m2);
            newOrigin += m3;

            FVector newExtent = (FVector.Replicate(extent, 0) * m0).GetAbs();
            newExtent += (FVector.Replicate(extent, 1) * m1).GetAbs();
            newExtent += (FVector.Replicate(extent, 2) * m2).GetAbs();

            FVector newVecMin = newOrigin - newExtent;
            FVector newVecMax = newOrigin + newExtent;

            FBox newBox;
            newBox.Min = newVecMin;
            newBox.Max = newVecMax;
            newBox.isValid = 1;
            return newBox;
        }

        /// <summary>
        /// Gets a bounding volume transformed by a FTransform object.
        /// </summary>
        /// <param name="m">The transformation object.</param>
        /// <returns>The transformed box.</returns>
        /// <see cref="TransformProjectBy"/>
        public FBox TransformBy(FTransform m)
        {
            return TransformBy(m.ToMatrixWithScale());
        }

        /// <summary>
        /// Transforms and projects a world bounding box to screen space
        /// </summary>
        /// <param name="projM">The projection matrix.</param>
        /// <returns>The transformed box.</returns>
        /// <see cref="TransformBy(FMatrix)"/>
        public FBox TransformProjectBy(FMatrix projM)
        {
            FVector[] vertices = new FVector[8]
                {
                    Min,
                    new FVector(Min.X, Min.Y, Max.Z),
                    new FVector(Min.X, Max.Y, Min.Z),
                    new FVector(Max.X, Min.Y, Min.Z),
                    new FVector(Max.X, Max.Y, Min.Z),
                    new FVector(Max.X, Min.Y, Max.Z),
                    new FVector(Min.X, Max.Y, Max.Z),
                    Max
                };

            FBox newBox = default(FBox);

            for (int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++)
            {
                FVector4 projectedVertex = projM.TransformPosition(vertices[vertexIndex]);
                newBox += ((FVector)projectedVertex) / projectedVertex.W;
            }

            return newBox;
        }

        public override string ToString()
        {
            return "IsValid=" + (IsValid ? "true" : "false") + ", Min=(" + Min + "), Max=(" + Max + ")";
        }

        /// <summary>
        /// Utility function to build an AABB from Origin and Extent
        /// </summary>
        /// <param name="origin">The location of the bounding box.</param>
        /// <param name="extent">Half size of the bounding box.</param>
        /// <returns>A new axis-aligned bounding box.</returns>
        public static FBox BuildAABB(FVector origin, FVector extent)
        {
            return new FBox(origin - extent, origin + extent);
        }
    }
}
