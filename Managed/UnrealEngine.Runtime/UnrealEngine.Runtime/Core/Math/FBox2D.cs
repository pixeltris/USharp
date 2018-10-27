using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // This struct isn't blittable as it contains a native bool type

    // Engine\Source\Runtime\Core\Public\Math\Box2D.h

    /// <summary>
    /// Implements a rectangular 2D Box.
    /// </summary>
    [UStruct(Flags = 0x00009038), BlueprintType, UMetaPath("/Script/CoreUObject.Box2D", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FBox2D : IEquatable<FBox2D>
    {
        static bool Min_IsValid;
        static int Min_Offset;
        /// <summary>
        /// Holds the box's minimum point.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000005), UMetaPath("/Script/CoreUObject.Box2D:Min")]
        public FVector2D Min;

        static bool Max_IsValid;
        static int Max_Offset;
        /// <summary>
        /// Holds the box's maximum point.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000005), UMetaPath("/Script/CoreUObject.Box2D:Max")]
        public FVector2D Max;

        static bool bIsValid_IsValid;
        static int bIsValid_Offset;
        /// <summary>
        /// Holds a flag indicating whether this box is valid.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000200), UMetaPath("/Script/CoreUObject.Box2D:bIsValid")]
        private byte bIsValid;

        /// <summary>
        /// Holds a flag indicating whether this box is valid.
        /// </summary>
        public bool IsValid
        {
            get { return bIsValid != 0; }
            set { bIsValid = (byte)(value ? 1 : 0); }
        }

        static bool FBox2D_IsValid;
        static int FBox2D_StructSize;

        public FBox2D Copy()
        {
            FBox2D result = this;
            return result;
        }        

        static FBox2D()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FBox2D)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FBox2D));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Box2D");
            FBox2D_StructSize = NativeReflection.GetStructSize(classAddress);
            Min_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Min");
            Min_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Min", Classes.UStructProperty);
            Max_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Max");
            Max_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Max", Classes.UStructProperty);
            bIsValid_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "bIsValid");
            bIsValid_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "bIsValid", Classes.UByteProperty);
            FBox2D_IsValid = classAddress != IntPtr.Zero && Min_IsValid && Max_IsValid && bIsValid_IsValid;
            NativeReflection.LogStructIsValid("/Script/CoreUObject.Box2D", FBox2D_IsValid);
        }

        /// <summary>
        /// Creates and initializes a new box from the specified parameters.
        /// </summary>
        /// <param name="min">The box's minimum point.</param>
        /// <param name="max">The box's maximum point.</param>
        public FBox2D(FVector2D min, FVector2D max)
        {
            Min = min;
            Max = max;
            bIsValid = 1;
        }

        /// <summary>
        /// Compares two boxes for equality.
        /// </summary>
        /// <param name="a">The first box.</param>
        /// <param name="b">The second box.</param>
        /// <returns>true if the boxes are equal, false otherwise.</returns>
        public static bool operator ==(FBox2D a, FBox2D b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        /// <summary>
        /// Compare two boxes for inequality.
        /// </summary>
        /// <param name="a">The first box.</param>
        /// <param name="b">The second box.</param>
        /// <returns>true if the boxes are not equal, false otherwise.</returns>
        public static bool operator !=(FBox2D a, FBox2D b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FBox2D))
            {
                return false;
            }

            return Equals((FBox2D)obj);
        }

        public bool Equals(FBox2D other)
        {
            return Min == other.Min && Max == other.Max;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = Min.GetHashCode();
                hashcode = (hashcode * 397) ^ Max.GetHashCode();
                hashcode = (hashcode * 397) ^ bIsValid.GetHashCode();
                return hashcode;
            }
        }

        /// <summary>
        /// Adds to the bounding box to include a given point.
        /// </summary>
        /// <param name="box">The rectangle.</param>
        /// <param name="other">The point to increase the bounding volume to.</param>
        /// <returns>The bounding box after resizing to include the other point.</returns>
        public static FBox2D operator +(FBox2D box, FVector2D other)
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
        public static FBox2D Add(FBox2D box, FVector2D other)
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
        public static void Add(ref FBox2D box, ref FVector2D other, out FBox2D result)
        {
            if (box.IsValid)
            {
                result.Min.X = FMath.Min(box.Min.X, other.X);
                result.Min.Y = FMath.Min(box.Min.Y, other.Y);

                result.Max.X = FMath.Max(box.Max.X, other.X);
                result.Max.Y = FMath.Max(box.Max.Y, other.Y);

                result.bIsValid = 1;
            }
            else
            {
                result.Min = result.Max = other;
                result.bIsValid = 1;
            }
        }

        /// <summary>
        /// Gets the result of adding two bounding boxes together.
        /// </summary>
        /// <param name="a">The first bounding box.</param>
        /// <param name="b">The second bounding box.</param>
        /// <returns>A new bounding volume.</returns>
        public static FBox2D operator +(FBox2D a, FBox2D b)
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
        public static FBox2D Add(FBox2D a, FBox2D b)
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
        public static void Add(ref FBox2D a, ref FBox2D b, out FBox2D result)
        {
            if (a.IsValid && b.IsValid)
            {
                result.Min.X = FMath.Min(a.Min.X, b.Min.X);
                result.Min.Y = FMath.Min(a.Min.Y, b.Min.Y);

                result.Max.X = FMath.Max(a.Max.X, b.Max.X);
                result.Max.Y = FMath.Max(a.Max.Y, b.Max.Y);

                result.bIsValid = 1;
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
        /// Gets either the min or max of this bounding volume.
        /// </summary>
        /// <param name="index">the index into points of the bounding volume.</param>
        /// <returns>Copy of the point of the bounding volume.</returns>
        public FVector2D this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Min;
                    case 1: return Max;
                    default:
                        throw new IndexOutOfRangeException("Invalid FBox2D index (" + index + ")");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: Min = value; break;
                    case 1: Max = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FBox2D index (" + index + ")");
                }
            }
        }

        /// <summary>
        /// Calculates the distance of a point to this box.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The distance.</returns>
        public float ComputeSquaredDistanceToPoint(FVector2D point)
        {
            // Accumulates the distance as we iterate axis
            float distSquared = 0.0f;

            if (point.X < Min.X)
            {
                distSquared += FMath.Square(point.X - Min.X);
            }
            else if (point.X > Max.X)
            {
                distSquared += FMath.Square(point.X - Max.X);
            }

            if (point.Y < Min.Y)
            {
                distSquared += FMath.Square(point.Y - Min.Y);
            }
            else if (point.Y > Max.Y)
            {
                distSquared += FMath.Square(point.Y - Max.Y);
            }

            return distSquared;
        }

        /// <summary>
        /// Increase the bounding box volume.
        /// </summary>
        /// <param name="w">The size to increase volume by.</param>
        /// <returns>A new bounding box increased in size.</returns>
        public FBox2D ExpandBy(float w)
        {
            return new FBox2D(Min - new FVector2D(w, w), Max + new FVector2D(w, w));
        }

        /// <summary>
        /// Gets the box area.
        /// </summary>
        /// <returns>Box area.</returns>
        /// <see cref="GetCenter"/>
        /// <see cref="GetCenterAndExtents"/>
        /// <see cref="GetExtent"/>
        /// <see cref="GetSize"/>
        public float GetArea()
        {
            return (Max.X - Min.X) * (Max.Y - Min.Y);
        }

        /// <summary>
        /// Gets the box's center point.
        /// </summary>
        /// <returns>The center point.</returns>
        /// <see cref="GetArea"/>
        /// <see cref="GetCenterAndExtents"/>
        /// <see cref="GetExtent"/>
        /// <see cref="GetSize"/>
        public FVector2D GetCenter()
        {
            return (Min + Max) * 0.5f;
        }

        /// <summary>
        /// Get the center and extents
        /// </summary>
        /// <param name="center">reference to center point</param>
        /// <param name="extents">reference to the extent around the center</param>
        /// <see cref="GetArea"/>
        /// <see cref="GetCenter"/>
        /// <see cref="GetExtent"/>
        /// <see cref="GetSize"/>
        public void GetCenterAndExtents(out FVector2D center, out FVector2D extents)
        {
            extents = GetExtent();
            center = Min + extents;
        }

        /// <summary>
        /// Calculates the closest point on or inside the box to a given point in space.
        /// </summary>
        /// <param name="point">The point in space.</param>
        /// <returns>The closest point on or inside the box.</returns>
        public FVector2D GetClosestPointTo(FVector2D point)
        {
            // start by considering the point inside the box
            FVector2D closestPoint = point;

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

            return closestPoint;
        }

        /// <summary>
        /// Gets the box extents around the center.
        /// </summary>
        /// <returns>Box extents.</returns>
        /// <see cref="GetArea"/>
        /// <see cref="GetCenter"/>
        /// <see cref="GetCenterAndExtents"/>
        /// <see cref="GetSize"/>
        public FVector2D GetExtent()
        {
            return 0.5f * (Max - Min);
        }

        /// <summary>
        /// Gets the box size.
        /// </summary>
        /// <returns>Box size.</returns>
        /// <see cref="GetArea"/>
        /// <see cref="GetCenter"/>
        /// <see cref="GetCenterAndExtents"/>
        /// <see cref="GetExtent"/>
        public FVector2D GetSize()
        {
            return (Max - Min);
        }

        /// <summary>
        /// Set the initial values of the bounding box to Zero.
        /// </summary>
        public void Init()
        {
            Min = Max = FVector2D.ZeroVector;
            bIsValid = 0;
        }

        /// <summary>
        /// Checks whether the given box intersects this box.
        /// </summary>
        /// <param name="other">bounding box to test intersection</param>
        /// <returns>true if boxes intersect, false otherwise.</returns>
        public bool Intersect(FBox2D other)
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
        /// Checks whether the given point is inside this box.
        /// </summary>
        /// <param name="testPoint">The point to test.</param>
        /// <returns>true if the point is inside this box, otherwise false.</returns>
        public bool IsInside(FVector2D testPoint)
        {
            return ((testPoint.X > Min.X) && (testPoint.X < Max.X) && (testPoint.Y > Min.Y) && (testPoint.Y < Max.Y));
        }

        /// <summary>
        /// Checks whether the given box is fully encapsulated by this box.
        /// </summary>
        /// <param name="other">The box to test for encapsulation within the bounding volume.</param>
        /// <returns>true if box is inside this volume, false otherwise.</returns>
        public bool IsInside(FBox2D other)
        {
            return (IsInside(other.Min) && IsInside(other.Max));
        }

        /// <summary>
        /// Shift bounding box position.
        /// </summary>
        /// <param name="offset">The offset vector to shift by.</param>
        /// <returns>A new shifted bounding box.</returns>
        public FBox2D ShiftBy(FVector2D offset)
        {
            return new FBox2D(Min + offset, Max + offset);
        }

        public override string ToString()
        {
            return "bIsValid=" + (IsValid ? "true" : "false") + ", Min=(" + Min + "), Max=(" + Max + ")";
        }
    }
}
