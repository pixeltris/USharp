using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\UnrealMathUtility.h
    // Geometry intersection

    public static partial class FMath
    {
        /// <summary>
        /// Find the intersection of a ray and a plane.  The ray has a start point with an infinite length.  Assumes that the
        /// line and plane do indeed intersect; you must make sure they're not parallel before calling.
        /// </summary>
        /// <param name="rayOrigin">The start point of the ray</param>
        /// <param name="rayDirection">The direction the ray is pointing (normalized vector)</param>
        /// <param name="plane">The plane to intersect with</param>
        /// <returns></returns>
        public static FVector RayPlaneIntersection(FVector rayOrigin, FVector rayDirection, FPlane plane)
        {
            FVector planeNormal = new FVector(plane.X, plane.Y, plane.Z);
            FVector planeOrigin = planeNormal * plane.W;

            float distance = FVector.DotProduct((planeOrigin - rayOrigin), planeNormal) / FVector.DotProduct(rayDirection, planeNormal);
            return rayOrigin + rayDirection * distance;
        }

        /// <summary>
        /// Find the intersection of a line and an offset plane. Assumes that the
        /// line and plane do indeed intersect; you must make sure they're not
        /// parallel before calling.
        /// </summary>
        /// <param name="point1">the first point defining the line</param>
        /// <param name="point2">the second point defining the line</param>
        /// <param name="planeOrigin">the origin of the plane</param>
        /// <param name="planeNormal">the normal of the plane</param>
        /// <returns>The point of intersection between the line and the plane.</returns>
        public static FVector LinePlaneIntersection(FVector point1, FVector point2, FVector planeOrigin, FVector planeNormal)
        {
            return point1 + (point2 - point1) *
                (((planeOrigin - point1) | planeNormal) / ((point2 - point1) | planeNormal));
        }

        /// <summary>
        /// Find the intersection of a line and a plane. Assumes that the line and
        /// plane do indeed intersect; you must make sure they're not parallel before
        /// calling.
        /// </summary>
        /// <param name="point1">the first point defining the line</param>
        /// <param name="point2">the second point defining the line</param>
        /// <param name="plane">the plane</param>
        /// <returns>The point of intersection between the line and the plane.</returns>
        public static FVector LinePlaneIntersection(FVector point1, FVector point2, FPlane plane)
        {
            return point1 + (point2 - point1) *
                ((plane.W - (point1 | plane)) / ((point2 - point1) | plane));
        }

        /// <summary>
        /// Compute the screen bounds of a point light along one axis.
        /// Based on http://www.gamasutra.com/features/20021011/lengyel_06.htm
        /// and http://sourceforge.net/mailarchive/message.php?msg_id=10501105
        /// </summary>
        public static bool ComputeProjectedSphereShaft(
            float lightX, float lightZ, float radius, FMatrix projMatrix, FVector axis, float axisSign, ref int minX, ref int maxX)
        {
            float viewX = minX;
            float viewSizeX = maxX - minX;

            // Vertical planes: T = <Nx, 0, Nz, 0>
            float discriminant = (FMath.Square(lightX) - FMath.Square(radius) + FMath.Square(lightZ)) * FMath.Square(lightZ);
            if (discriminant >= 0)
            {
                float sqrtDiscriminant = FMath.Sqrt(discriminant);
                float invLightSquare = 1.0f / (FMath.Square(lightX) + FMath.Square(lightZ));

                float nxa = (radius * lightX - sqrtDiscriminant) * invLightSquare;
                float nxb = (radius * lightX + sqrtDiscriminant) * invLightSquare;
                float nza = (radius - nxa * lightX) / lightZ;
                float nzb = (radius - nxb * lightX) / lightZ;
                float pza = lightZ - radius * nza;
                float pzb = lightZ - radius * nzb;

                // Tangent a
                if (pza > 0)
                {
                    float pxa = -pza * nza / nxa;
                    FVector4 p = projMatrix.TransformFVector4(new FVector4(axis.X * pxa, axis.Y * pxa, pza, 1));
                    float x = (FVector4.Dot3(p, axis) / p.W + 1.0f * axisSign) / 2.0f * axisSign;
                    if (FMath.IsNegativeFloat(nxa) ^ FMath.IsNegativeFloat(axisSign))
                    {
                        maxX = FMath.Min(FMath.CeilToInt(viewSizeX * x + viewX), maxX);
                    }
                    else
                    {
                        minX = FMath.Max(FMath.FloorToInt(viewSizeX * x + viewX), minX);
                    }
                }

                // Tangent b
                if (pzb > 0)
                {
                    float pxb = -pzb * nzb / nxb;
                    FVector4 p = projMatrix.TransformFVector4(new FVector4(axis.X * pxb, axis.Y * pxb, pzb, 1));
                    float x = (FVector4.Dot3(p, axis) / p.W + 1.0f * axisSign) / 2.0f * axisSign;
                    if (FMath.IsNegativeFloat(nxb) ^ FMath.IsNegativeFloat(axisSign))
                    {
                        maxX = FMath.Min(FMath.CeilToInt(viewSizeX * x + viewX), maxX);
                    }
                    else
                    {
                        minX = FMath.Max(FMath.FloorToInt(viewSizeX * x + viewX), minX);
                    }
                }
            }

            return minX <= maxX;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scissorRect">should be set to View.ViewRect before the call</param>
        /// <param name="sphereOrigin"></param>
        /// <param name="radius"></param>
        /// <param name="viewOrigin"></param>
        /// <param name="viewMatrix"></param>
        /// <param name="projMatrix"></param>
        /// <returns>0: light is not visible, 1:use scissor rect, 2: no scissor rect needed</returns>
        public static uint ComputeProjectedSphereScissorRect(
            ref FIntRect scissorRect, FVector sphereOrigin, float radius, FVector viewOrigin, FMatrix viewMatrix, FMatrix projMatrix)
        {
            // Calculate a scissor rectangle for the light's radius.
            if ((sphereOrigin - viewOrigin).SizeSquared() > FMath.Square(radius))
            {
                FVector lightVector = viewMatrix.TransformPosition(sphereOrigin);

                if (!ComputeProjectedSphereShaft(
                    lightVector.X,
                    lightVector.Z,
                    radius,
                    projMatrix,
                    new FVector(+1, 0, 0),
                    +1,
                    ref scissorRect.Min.X,
                    ref scissorRect.Max.X))
                {
                    return 0;
                }

                if (!ComputeProjectedSphereShaft(
                    lightVector.Y,
                    lightVector.Z,
                    radius,
                    projMatrix,
                    new FVector(0, +1, 0),
                    -1,
                    ref scissorRect.Min.Y,
                    ref scissorRect.Max.Y))
                {
                    return 0;
                }

                return 1;
            }
            else
            {
                return 2;
            }
        }

        /// <summary>
        /// Computes minimal bounding sphere encompassing given cone
        /// </summary>
        /// <param name="coneOrigin">Cone origin</param>
        /// <param name="coneDirection">Cone direction</param>
        /// <param name="coneRadius">Cone Radius</param>
        /// <param name="cosConeAngle">Cos of the cone angle</param>
        /// <param name="sinConeAngle">Sin of the cone angle</param>
        /// <returns>Minimal bounding sphere encompassing given cone</returns>
        public static FSphere ComputeBoundingSphereForCone(FVector coneOrigin, FVector coneDirection, float coneRadius, float cosConeAngle, float sinConeAngle)
        {
            // Based on: https://bartwronski.com/2017/04/13/cull-that-cone/
            const float COS_PI_OVER_4 = 0.707107f; // Cos(Pi/4);
            if (cosConeAngle < COS_PI_OVER_4)
            {
                return new FSphere(coneOrigin + coneDirection * coneRadius * cosConeAngle, coneRadius * sinConeAngle);
            }
            else
            {
                float boundingRadius = coneRadius / (2.0f * cosConeAngle);
                return new FSphere(coneOrigin + coneDirection * boundingRadius, boundingRadius);
            }
        }

        /// <summary>
        /// Determine if a plane and an AABB intersect
        /// </summary>
        /// <param name="p">the plane to test</param>
        /// <param name="aabb">the axis aligned bounding box to test</param>
        /// <returns>if collision occurs</returns>
        public static unsafe bool PlaneAABBIntersection(FPlane p, FBox aabb)
        {
            // find diagonal most closely aligned with normal of plane
            FVector vmin = new FVector(), vmax = new FVector();

            // Bypass the slow FVector[] operator. Not RESTRICT because it won't update Vmin, Vmax
            float* vminPtr = (float*)&vmin;
            float* vmaxPtr = (float*)&vmax;

            // Use restrict to get better instruction scheduling and to bypass the slow FVector[] operator
            float* aabbMinPtr = (float*)&aabb.Min;
            float* aabbMaxPtr = (float*)&aabb.Max;
            float* planePtr = (float*)&p;

            for (int i = 0; i < 3; ++i)
            {
                if (planePtr[i] >= 0.0f)
                {
                    vminPtr[i] = aabbMinPtr[i];
                    vmaxPtr[i] = aabbMaxPtr[i];
                }
                else
                {
                    vminPtr[i] = aabbMaxPtr[i];
                    vmaxPtr[i] = aabbMinPtr[i];
                }
            }

            // if either diagonal is right on the plane, or one is on either side we have an interesection
            float dMax = p.PlaneDot(vmax);
            float dMin = p.PlaneDot(vmin);

            // if Max is below plane, or Min is above we know there is no intersection.. otherwise there must be one
            return (dMax >= 0.0f && dMin <= 0.0f);
        }

        /// <summary>
        /// Performs a sphere vs box intersection test using Arvo's algorithm:
        /// 
        /// for each i in (x, y, z)
        /// 	if (SphereCenter(i) &lt; BoxMin(i)) d2 += (SphereCenter(i) - BoxMin(i)) ^ 2
        /// 	else if (SphereCenter(i) > BoxMax(i)) d2 += (SphereCenter(i) - BoxMax(i)) ^ 2	
        /// </summary>
        /// <param name="sphereCenter">the center of the sphere being tested against the AABB</param>
        /// <param name="radiusSquared">the size of the sphere being tested</param>
        /// <param name="aabb">the box being tested against</param>
        /// <returns>Whether the sphere/box intersect or not.</returns>
        public static bool SphereAABBIntersection(FVector sphereCenter, float radiusSquared, FBox aabb)
        {
            // Accumulates the distance as we iterate axis
            float distSquared = 0.0f;
            // Check each axis for min/max and add the distance accordingly
            // NOTE: Loop manually unrolled for > 2x speed up
            if (sphereCenter.X < aabb.Min.X)
            {
                distSquared += FMath.Square(sphereCenter.X - aabb.Min.X);
            }
            else if (sphereCenter.X > aabb.Max.X)
            {
                distSquared += FMath.Square(sphereCenter.X - aabb.Max.X);
            }
            if (sphereCenter.Y < aabb.Min.Y)
            {
                distSquared += FMath.Square(sphereCenter.Y - aabb.Min.Y);
            }
            else if (sphereCenter.Y > aabb.Max.Y)
            {
                distSquared += FMath.Square(sphereCenter.Y - aabb.Max.Y);
            }
            if (sphereCenter.Z < aabb.Min.Z)
            {
                distSquared += FMath.Square(sphereCenter.Z - aabb.Min.Z);
            }
            else if (sphereCenter.Z > aabb.Max.Z)
            {
                distSquared += FMath.Square(sphereCenter.Z - aabb.Max.Z);
            }
            // If the distance is less than or equal to the radius, they intersect
            return distSquared <= radiusSquared;
        }

        /// <summary>
        /// Converts a sphere into a point plus radius squared for the test above
        /// </summary>
        public static bool SphereAABBIntersection(FSphere sphere, FBox aabb)
        {
            float radiusSquared = FMath.Square(sphere.W);
            // If the distance is less than or equal to the radius, they intersect
            return SphereAABBIntersection(sphere.Center, radiusSquared, aabb);
        }

        /// <summary>
        /// Determines whether a point is inside a box.
        /// </summary>
        public static bool PointBoxIntersection(FVector point, FBox box)
        {
            return
                point.X >= box.Min.X && point.X <= box.Max.X &&
                point.Y >= box.Min.Y && point.Y <= box.Max.Y &&
                point.Z >= box.Min.Z && point.Z <= box.Max.Z;
        }

        /// <summary>
        /// Determines whether a line intersects a box.
        /// </summary>
        public static bool LineBoxIntersection(FBox box, FVector start, FVector end, FVector direction)
        {
            return LineBoxIntersection(box, start, end, direction, direction.Reciprocal());
        }

        /// <summary>
        /// Determines whether a line intersects a box. This overload avoids the need to do the reciprocal every time.
        /// </summary>
        public static bool LineBoxIntersection(FBox box, FVector start, FVector end, FVector direction, FVector oneOverDirection)
        {
            FVector time;
            bool startIsOutside = false;

            if (start.X < box.Min.X)
            {
                startIsOutside = true;
                if (end.X >= box.Min.X)
                {
                    time.X = (box.Min.X - start.X) * oneOverDirection.X;
                }
                else
                {
                    return false;
                }
            }
            else if (start.X > box.Max.X)
            {
                startIsOutside = true;
                if (end.X <= box.Max.X)
                {
                    time.X = (box.Max.X - start.X) * oneOverDirection.X;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                time.X = 0.0f;
            }

            if (start.Y < box.Min.Y)
            {
                startIsOutside = true;
                if (end.Y >= box.Min.Y)
                {
                    time.Y = (box.Min.Y - start.Y) * oneOverDirection.Y;
                }
                else
                {
                    return false;
                }
            }
            else if (start.Y > box.Max.Y)
            {
                startIsOutside = true;
                if (end.Y <= box.Max.Y)
                {
                    time.Y = (box.Max.Y - start.Y) * oneOverDirection.Y;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                time.Y = 0.0f;
            }

            if (start.Z < box.Min.Z)
            {
                startIsOutside = true;
                if (end.Z >= box.Min.Z)
                {
                    time.Z = (box.Min.Z - start.Z) * oneOverDirection.Z;
                }
                else
                {
                    return false;
                }
            }
            else if (start.Z > box.Max.Z)
            {
                startIsOutside = true;
                if (end.Z <= box.Max.Z)
                {
                    time.Z = (box.Max.Z - start.Z) * oneOverDirection.Z;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                time.Z = 0.0f;
            }

            if (startIsOutside)
            {
                float maxTime = Max3(time.X, time.Y, time.Z);

                if (maxTime >= 0.0f && maxTime <= 1.0f)
                {
                    FVector hit = start + direction * maxTime;
                    const float BOX_SIDE_THRESHOLD = 0.1f;
                    if (hit.X > box.Min.X - BOX_SIDE_THRESHOLD && hit.X < box.Max.X + BOX_SIDE_THRESHOLD &&
                        hit.Y > box.Min.Y - BOX_SIDE_THRESHOLD && hit.Y < box.Max.Y + BOX_SIDE_THRESHOLD &&
                        hit.Z > box.Min.Z - BOX_SIDE_THRESHOLD && hit.Z < box.Max.Z + BOX_SIDE_THRESHOLD)
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Swept-Box vs Box test
        /// </summary>
        public static bool LineExtentBoxIntersection(FBox inBox, FVector start, FVector end, FVector extent, out FVector hitLocation, out FVector hitNormal, out float hitTime)
        {
            FBox box = inBox;
            box.Max.X += extent.X;
            box.Max.Y += extent.Y;
            box.Max.Z += extent.Z;

            box.Min.X -= extent.X;
            box.Min.Y -= extent.Y;
            box.Min.Z -= extent.Z;

            FVector dir = (end - start);

            FVector time;
            bool inside = true;
            float[] faceDir = { 1, 1, 1 };

            /////////////// X
            if (start.X < box.Min.X)
            {
                if (dir.X <= 0.0f)
                {
                    hitLocation = default(FVector);
                    hitNormal = default(FVector);
                    hitTime = default(float);
                    return false;
                }
                else
                {
                    inside = false;
                    faceDir[0] = -1;
                    time.X = (box.Min.X - start.X) / dir.X;
                }
            }
            else if (start.X > box.Max.X)
            {
                if (dir.X >= 0.0f)
                {
                    hitLocation = default(FVector);
                    hitNormal = default(FVector);
                    hitTime = default(float);
                    return false;
                }
                else
                {
                    inside = false;
                    time.X = (box.Max.X - start.X) / dir.X;
                }
            }
            else
            {
                time.X = 0.0f;
            }

            /////////////// Y
            if (start.Y < box.Min.Y)
            {
                if (dir.Y <= 0.0f)
                {
                    hitLocation = default(FVector);
                    hitNormal = default(FVector);
                    hitTime = default(float);
                    return false;
                }
                else
                {
                    inside = false;
                    faceDir[1] = -1;
                    time.Y = (box.Min.Y - start.Y) / dir.Y;
                }
            }
            else if (start.Y > box.Max.Y)
            {
                if (dir.Y >= 0.0f)
                {
                    hitLocation = default(FVector);
                    hitNormal = default(FVector);
                    hitTime = default(float);
                    return false;
                }
                else
                {
                    inside = false;
                    time.Y = (box.Max.Y - start.Y) / dir.Y;
                }
            }
            else
            {
                time.Y = 0.0f;
            }

            /////////////// Z
            if (start.Z < box.Min.Z)
            {
                if (dir.Z <= 0.0f)
                {
                    hitLocation = default(FVector);
                    hitNormal = default(FVector);
                    hitTime = default(float);
                    return false;
                }
                else
                {
                    inside = false;
                    faceDir[2] = -1;
                    time.Z = (box.Min.Z - start.Z) / dir.Z;
                }
            }
            else if (start.Z > box.Max.Z)
            {
                if (dir.Z >= 0.0f)
                {
                    hitLocation = default(FVector);
                    hitNormal = default(FVector);
                    hitTime = default(float);
                    return false;
                }
                else
                {
                    inside = false;
                    time.Z = (box.Max.Z - start.Z) / dir.Z;
                }
            }
            else
            {
                time.Z = 0.0f;
            }

            // If the line started inside the box (ie. player started in contact with the fluid)
            if (inside)
            {
                hitLocation = start;
                hitNormal = new FVector(0, 0, 1);
                hitTime = 0;
                return true;
            }
            // Otherwise, calculate when hit occured
            else
            {
                if (time.Y > time.Z)
                {
                    hitTime = time.Y;
                    hitNormal = new FVector(0, faceDir[1], 0);
                }
                else
                {
                    hitTime = time.Z;
                    hitNormal = new FVector(0, 0, faceDir[2]);
                }

                if (time.X > hitTime)
                {
                    hitTime = time.X;
                    hitNormal = new FVector(faceDir[0], 0, 0);
                }

                if (hitTime >= 0.0f && hitTime <= 1.0f)
                {
                    hitLocation = start + dir * hitTime;
                    const float BOX_SIDE_THRESHOLD = 0.1f;
                    if (hitLocation.X > box.Min.X - BOX_SIDE_THRESHOLD && hitLocation.X < box.Max.X + BOX_SIDE_THRESHOLD &&
                        hitLocation.Y > box.Min.Y - BOX_SIDE_THRESHOLD && hitLocation.Y < box.Max.Y + BOX_SIDE_THRESHOLD &&
                        hitLocation.Z > box.Min.Z - BOX_SIDE_THRESHOLD && hitLocation.Z < box.Max.Z + BOX_SIDE_THRESHOLD)
                    {
                        return true;
                    }
                }

                hitLocation = default(FVector);
                return false;
            }
        }

        /// <summary>
        /// Determines whether a line intersects a sphere.
        /// </summary>
        public static bool LineSphereIntersection(FVector start, FVector dir, float length, FVector origin, float radius)
        {
            FVector eo = start - origin;
            float v = (dir | (origin - start));
            float disc = radius * radius - ((eo | eo) - v * v);

            if (disc >= 0.0f)
            {
                float Time = (v - Sqrt(disc)) / length;

                if (Time >= 0.0f && Time <= 1.0f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Assumes the cone tip is at 0,0,0 (means the SphereCenter is relative to the cone tip)
        /// </summary>
        /// <returns>true: cone and sphere do intersect, false otherwise</returns>
        public static bool SphereConeIntersection(FVector sphereCenter, float sphereRadius, FVector coneAxis, float coneAngleSin, float coneAngleCos)
        {
            /*
             * from http://www.geometrictools.com/Documentation/IntersectionSphereCone.pdf
             * (Copyright c 1998-2008. All Rights Reserved.) http://www.geometrictools.com (boost license)
             */

            // the following code assumes the cone tip is at 0,0,0 (means the SphereCenter is relative to the cone tip)

            FVector u = coneAxis * (-sphereRadius / coneAngleSin);
            FVector d = sphereCenter - u;
            float dsqr = d | d;
            float e = coneAxis | d;

            if (e > 0 && e * e >= dsqr * FMath.Square(coneAngleCos))
            {
                dsqr = sphereCenter | sphereCenter;
                e = -coneAxis | sphereCenter;
                if (e > 0 && e * e >= dsqr * FMath.Square(coneAngleSin))
                {
                    return dsqr <= FMath.Square(sphereRadius);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find the point on the line segment from LineStart to LineEnd which is closest to Point
        /// </summary>
        public static FVector ClosestPointOnLine(FVector lineStart, FVector lineEnd, FVector point)
        {
            // Solve to find alpha along line that is closest point
            // Weisstein, Eric W. "Point-Line Distance--3-Dimensional." From MathWorld--A Switchram Web Resource. http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html 
            float a = (lineStart - point) | (lineEnd - lineStart);
            float b = (lineEnd - lineStart).SizeSquared();
            // This should be robust to B == 0 (resulting in NaN) because clamp should return 1.
            float t = FMath.Clamp(-a / b, 0.0f, 1.0f);

            // Generate closest point
            FVector ClosestPoint = lineStart + (t * (lineEnd - lineStart));

            return ClosestPoint;
        }

        /// <summary>
        /// Find the point on the infinite line between two points (LineStart, LineEnd) which is closest to Point
        /// </summary>
        public static FVector ClosestPointOnInfiniteLine(FVector lineStart, FVector lineEnd, FVector point)
        {
            float a = (lineStart - point) | (lineEnd - lineStart);
            float b = (lineEnd - lineStart).SizeSquared();
            if (b < SmallNumber)
            {
                return lineStart;
            }
            float t = -a / b;

            // Generate closest point
            FVector closestPoint = lineStart + (t * (lineEnd - lineStart));
            return closestPoint;
        }

        /// <summary>
        /// Compute intersection point of three planes. Return 1 if valid, 0 if infinite.
        /// </summary>
        public static bool IntersectPlanes3(out FVector i, FPlane p1, FPlane p2, FPlane p3)
        {
            // Compute determinant, the triple product P1|(P2^P3)==(P1^P2)|P3.
            float det = (p1 ^ p2) | p3;
            if (Square(det) < Square(0.001f))
            {
                // Degenerate.
                i = FVector.ZeroVector;
                return false;
            }
            else
            {
                // Compute the intersection point, guaranteed valid if determinant is nonzero.
                i = (p1.W * (p2 ^ p3) + p2.W * (p3 ^ p1) + p3.W * (p1 ^ p2)) / det;
            }
            return true;
        }

        /// <summary>
        /// Compute intersection point and direction of line joining two planes.
        /// Return 1 if valid, 0 if infinite.
        /// </summary>
        public static bool IntersectPlanes2(out FVector i, out FVector d, FPlane p1, FPlane p2)
        {
            // Compute line direction, perpendicular to both plane normals.
            d = (p1 ^ p2);
            float dd = d.SizeSquared();
            if (dd < Square(0.001f))
            {
                // Parallel or nearly parallel planes.
                d = i = FVector.ZeroVector;
                return false;
            }
            else
            {
                // Compute intersection.
                i = (p1.W * (p2 ^ d) + p2.W * (d ^ p1)) / dd;
                d.Normalize();
                return true;
            }
        }

        /// <summary>
        /// Calculates the distance of a given Point in world space to a given line,
        /// defined by the vector couple (Origin, Direction).
        /// </summary>
        /// <param name="point">Point to check distance to line</param>
        /// <param name="direction">Vector indicating the direction of the line. Not required to be normalized.</param>
        /// <param name="origin">Point of reference used to calculate distance</param>
        /// <param name="closestPoint">optional point that represents the closest point projected onto Axis</param>
        /// <returns>distance of Point from line defined by (Origin, Direction)</returns>
        public static float PointDistToLine(FVector point, FVector direction, FVector origin, out FVector closestPoint)
        {
            FVector SafeDir = direction.GetSafeNormal();
            closestPoint = origin + (SafeDir * ((point - origin) | SafeDir));
            return (closestPoint - point).Size();
        }

        /// <summary>
        /// Calculates the distance of a given Point in world space to a given line,
        /// defined by the vector couple (Origin, Direction).
        /// </summary>
        /// <param name="point">Point to check distance to line</param>
        /// <param name="direction">Vector indicating the direction of the line. Not required to be normalized.</param>
        /// <param name="origin">Point of reference used to calculate distance</param>
        /// <returns>distance of Point from line defined by (Origin, Direction)</returns>
        public static float PointDistToLine(FVector point, FVector direction, FVector origin)
        {
            FVector SafeDir = direction.GetSafeNormal();
            FVector closestPoint = origin + (SafeDir * ((point - origin) | SafeDir));
            return (closestPoint - point).Size();
        }

        /// <summary>
        /// Returns closest point on a segment to a given point.
        /// The idea is to project point on line formed by segment.
        /// Then we see if the closest point on the line is outside of segment or inside.
        /// </summary>
        /// <param name="point">point for which we find the closest point on the segment</param>
        /// <param name="startPoint">StartPoint of segment</param>
        /// <param name="endPoint">EndPoint of segment</param>
        /// <returns>point on the segment defined by (StartPoint, EndPoint) that is closest to Point.</returns>
        public static FVector ClosestPointOnSegment(FVector point, FVector startPoint, FVector endPoint)
        {
            FVector segment = endPoint - startPoint;
            FVector vectToPoint = point - startPoint;

            // See if closest point is before StartPoint
            float dot1 = vectToPoint | segment;
            if (dot1 <= 0)
            {
                return startPoint;
            }

            // See if closest point is beyond EndPoint
            float dot2 = segment | segment;
            if (dot2 <= dot1)
            {
                return endPoint;
            }

            // Closest Point is within segment
            return startPoint + segment * (dot1 / dot2);
        }

        /// <summary>
        /// FVector2D version of ClosestPointOnSegment.
        /// Returns closest point on a segment to a given 2D point.
        /// The idea is to project point on line formed by segment.
        /// Then we see if the closest point on the line is outside of segment or inside.
        /// </summary>
        /// <param name="point">point for which we find the closest point on the segment</param>
        /// <param name="startPoint">StartPoint of segment</param>
        /// <param name="endPoint">EndPoint of segment</param>
        /// <returns>point on the segment defined by (StartPoint, EndPoint) that is closest to Point.</returns>
        public static FVector2D ClosestPointOnSegment2D(FVector2D point, FVector2D startPoint, FVector2D endPoint)
        {
            FVector2D segment = endPoint - startPoint;
            FVector2D vectToPoint = point - startPoint;

            // See if closest point is before StartPoint
            float dot1 = FVector2D.DotProduct(vectToPoint, segment);
            if (dot1 <= 0)
            {
                return startPoint;
            }

            // See if closest point is beyond EndPoint
            float dot2 = FVector2D.DotProduct(segment, segment);
            if (dot2 <= dot1)
            {
                return endPoint;
            }

            // Closest Point is within segment
            return startPoint + segment * (dot1 / dot2);
        }

        /// <summary>
        /// Returns distance from a point to the closest point on a segment.
        /// </summary>
        /// <param name="point">point to check distance for</param>
        /// <param name="startPoint">StartPoint of segment</param>
        /// <param name="endPoint">EndPoint of segment</param>
        /// <returns>closest distance from Point to segment defined by (StartPoint, EndPoint).</returns>
        public static float PointDistToSegment(FVector point, FVector startPoint, FVector endPoint)
        {
            FVector closestPoint = ClosestPointOnSegment(point, startPoint, endPoint);
            return (point - closestPoint).Size();
        }

        /// <summary>
        /// Returns square of the distance from a point to the closest point on a segment.
        /// </summary>
        /// <param name="point">point to check distance for</param>
        /// <param name="startPoint">StartPoint of segment</param>
        /// <param name="endPoint">EndPoint of segment</param>
        /// <returns>square of the closest distance from Point to segment defined by (StartPoint, EndPoint).</returns>
        public static float PointDistToSegmentSquared(FVector point, FVector startPoint, FVector endPoint)
        {
            FVector closestPoint = ClosestPointOnSegment(point, startPoint, endPoint);
            return (point - closestPoint).SizeSquared();
        }

        struct SegmentDistToSegment_Solver
        {
            public bool LinesAreNearlyParallel;

            public FVector A1;
            public FVector A2;

            public FVector S1;
            public FVector S2;
            public FVector S3;

            public SegmentDistToSegment_Solver(ref FVector a1, ref FVector b1, ref FVector a2, ref FVector b2)
            {
                LinesAreNearlyParallel = false;
                A1 = a1;
                A2 = a2;
                S1 = b1 - a1;
                S2 = b2 - a2;
                S3 = a1 - a2;
            }

            public void Solve(out FVector outP1, out FVector outP2)
            {
                float dot11 = S1 | S1;
                float dot12 = S1 | S2;
                float dot13 = S1 | S3;
                float dot22 = S2 | S2;
                float dot23 = S2 | S3;

                float d = dot11 * dot22 - dot12 * dot12;

                float d1 = d;
                float d2 = d;

                float n1;
                float n2;

                if (LinesAreNearlyParallel || d < KindaSmallNumber)
                {
                    // the lines are almost parallel
                    n1 = 0.0f;// force using point A on segment S1
                    d1 = 1.0f;// to prevent possible division by 0 later
                    n2 = dot23;
                    d2 = dot22;
                }
                else
                {
                    // get the closest points on the infinite lines
                    n1 = (dot12 * dot23 - dot22 * dot13);
                    n2 = (dot11 * dot23 - dot12 * dot13);

                    if (n1 < 0.0f)
                    {
                        // t1 < 0.f => the s==0 edge is visible
                        n1 = 0.0f;
                        n2 = dot23;
                        d2 = dot22;
                    }
                    else if (n1 > d1)
                    {
                        // t1 > 1 => the t1==1 edge is visible
                        n1 = d1;
                        n2 = dot23 + dot12;
                        d2 = dot22;
                    }
                }

                if (n2 < 0.0f)
                {
                    // t2 < 0 => the t2==0 edge is visible
                    n2 = 0.0f;

                    // recompute t1 for this edge
                    if (-dot13 < 0.0f)
                    {
                        n1 = 0.0f;
                    }
                    else if (-dot13 > dot11)
                    {
                        n1 = d1;
                    }
                    else
                    {
                        n1 = -dot13;
                        d1 = dot11;
                    }
                }
                else if (n2 > d2)
                {
                    // t2 > 1 => the t2=1 edge is visible
                    n2 = d2;

                    // recompute t1 for this edge
                    if ((-dot13 + dot12) < 0.0f)
                    {
                        n1 = 0.0f;
                    }
                    else if ((-dot13 + dot12) > dot11)
                    {
                        n1 = d1;
                    }
                    else
                    {
                        n1 = (-dot13 + dot12);
                        d1 = dot11;
                    }
                }

                // finally do the division to get the points' location
                float t1 = (FMath.Abs(n1) < KindaSmallNumber ? 0.0f : n1 / d1);
                float t2 = (FMath.Abs(n2) < KindaSmallNumber ? 0.0f : n2 / d2);

                // return the closest points
                outP1 = A1 + t1 * S1;
                outP2 = A2 + t2 * S2;
            }
        }

        /// <summary>
        /// Find closest points between 2 segments.
        /// </summary>
        /// <param name="a1">defines the first segment.</param>
        /// <param name="b1">defines the first segment.</param>
        /// <param name="a2">defines the second segment.</param>
        /// <param name="b2">defines the second segment.</param>
        /// <param name="p1">Closest point on segment 1 to segment 2.</param>
        /// <param name="p2">Closest point on segment 2 to segment 1.</param>
        public static void SegmentDistToSegment(FVector a1, FVector b1, FVector a2, FVector b2, out FVector p1, out FVector p2)
        {
            new SegmentDistToSegment_Solver(ref a1, ref b1, ref a2, ref b2).Solve(out p1, out p2);
        }

        /// <summary>
        /// Find closest points between 2 segments.
        /// 
        /// This is the safe version, and will check both segments' lengths.
        /// Use this if either (or both) of the segments lengths may be 0.
        /// </summary>
        /// <param name="a1">defines the first segment.</param>
        /// <param name="b1">defines the first segment.</param>
        /// <param name="a2">defines the second segment.</param>
        /// <param name="b2">defines the second segment.</param>
        /// <param name="p1">Closest point on segment 1 to segment 2.</param>
        /// <param name="p2">Closest point on segment 2 to segment 1.</param>
        public static void SegmentDistToSegmentSafe(FVector a1, FVector b1, FVector a2, FVector b2, out FVector p1, out FVector p2)
        {
            SegmentDistToSegment_Solver solver = new SegmentDistToSegment_Solver(ref a1, ref b1, ref a2, ref b2);

            FVector s1_norm = solver.S1.GetSafeNormal();
            FVector s2_norm = solver.S2.GetSafeNormal();

            bool s1IsPoint = s1_norm.IsZero();
            bool s2IsPoint = s2_norm.IsZero();

            if (s1IsPoint && s2IsPoint)
            {
                p1 = a1;
                p2 = a2;
            }
            else if (s2IsPoint)
            {
                p1 = ClosestPointOnSegment(a2, a1, b1);
                p2 = a2;
            }
            else if (s1IsPoint)
            {
                p1 = a1;
                p2 = ClosestPointOnSegment(a1, a2, b2);
            }
            else
            {
                float dot11_norm = s1_norm | s1_norm; // always >= 0
                float dot22_norm = s2_norm | s2_norm; // always >= 0
                float dot12_norm = s1_norm | s2_norm;
                float d_norm = dot11_norm * dot22_norm - dot12_norm * dot12_norm; // always >= 0

                solver.LinesAreNearlyParallel = d_norm < KindaSmallNumber;
                solver.Solve(out p1, out p2);
            }
        }

        /// <summary>
        /// returns the time (t) of the intersection of the passed segment and a plane (could be &lt;0 or >1)
        /// </summary>
        /// <param name="startPoint">start point of segment</param>
        /// <param name="endPoint">end point of segment</param>
        /// <param name="plane">plane to intersect with</param>
        /// <returns>time(T) of intersection</returns>
        public static float GetTForSegmentPlaneIntersect(FVector startPoint, FVector endPoint, FPlane plane)
        {
            return (plane.W - (startPoint | plane)) / ((endPoint - startPoint) | plane);
        }

        /// <summary>
        /// Returns true if there is an intersection between the segment specified by StartPoint and Endpoint, and
        /// the plane on which polygon Plane lies. If there is an intersection, the point is placed in out_IntersectionPoint
        /// </summary>
        /// <param name="startPoint">start point of segment</param>
        /// <param name="endPoint">end point of segment</param>
        /// <param name="plane">plane to intersect with</param>
        /// <param name="intersectionPoint">out var for the point on the segment that intersects the mesh (if any)</param>
        /// <returns>true if intersection occurred</returns>
        public static bool SegmentPlaneIntersection(FVector startPoint, FVector endPoint, FPlane plane, out FVector intersectionPoint)
        {
            float t = FMath.GetTForSegmentPlaneIntersect(startPoint, endPoint, plane);
            // If the parameter value is not between 0 and 1, there is no intersection
            if (t > -KindaSmallNumber && t < 1.0f + KindaSmallNumber)
            {
                intersectionPoint = startPoint + t * (endPoint - startPoint);
                return true;
            }
            intersectionPoint = default(FVector);
            return false;
        }

        /// <summary>
        /// Returns true if there is an intersection between the segment specified by StartPoint and Endpoint, and
        /// the Triangle defined by A, B and C. If there is an intersection, the point is placed in out_IntersectionPoint
        /// </summary>
        /// <param name="startPoint">start point of segment</param>
        /// <param name="endPoint">end point of segment</param>
        /// <param name="a">points defining the triangle </param>
        /// <param name="b">points defining the triangle </param>
        /// <param name="c">points defining the triangle </param>
        /// <param name="intersectPoint">out var for the point on the segment that intersects the triangle (if any)</param>
        /// <param name="triangleNormal">out var for the triangle normal</param>
        /// <returns>true if intersection occurred</returns>
        public static bool SegmentTriangleIntersection(FVector startPoint, FVector endPoint, FVector a, FVector b, FVector c, out FVector intersectPoint, out FVector triangleNormal)
        {
            FVector ba = a - b;
            FVector cb = b - c;
            FVector triNormal = ba ^ cb;

            bool collide = FMath.SegmentPlaneIntersection(startPoint, endPoint, new FPlane(a, triNormal), out intersectPoint);
            if (!collide)
            {
                triangleNormal = default(FVector);
                return false;
            }

            FVector baryCentric = FMath.ComputeBaryCentric2D(intersectPoint, a, b, c);
            if (baryCentric.X > 0.0f && baryCentric.Y > 0.0f && baryCentric.Z > 0.0f)
            {
                triangleNormal = triNormal;
                return true;
            }

            triangleNormal = default(FVector);
            return false;
        }

        /// <summary>
        /// Returns true if there is an intersection between the segment specified by SegmentStartA and SegmentEndA, and
        /// the segment specified by SegmentStartB and SegmentEndB, in 2D space. If there is an intersection, the point is placed in out_IntersectionPoint
        /// </summary>
        /// <param name="segmentStartA">start point of first segment</param>
        /// <param name="segmentEndA">end point of first segment</param>
        /// <param name="segmentStartB">start point of second segment</param>
        /// <param name="segmentEndB">end point of second segment</param>
        /// <param name="intersectionPoint">out var for the intersection point (if any)</param>
        /// <returns>true if intersection occurred</returns>
        public static bool SegmentIntersection2D(FVector segmentStartA, FVector segmentEndA, FVector segmentStartB, FVector segmentEndB, out FVector intersectionPoint)
        {
            FVector vectorA = segmentEndA - segmentStartA;
            FVector vectorB = segmentEndB - segmentStartB;

            float s = (-vectorA.Y * (segmentStartA.X - segmentStartB.X) + vectorA.X * (segmentStartA.Y - segmentStartB.Y)) / (-vectorB.X * vectorA.Y + vectorA.X * vectorB.Y);
            float t = (vectorB.X * (segmentStartA.Y - segmentStartB.Y) - vectorB.Y * (segmentStartA.X - segmentStartB.X)) / (-vectorB.X * vectorA.Y + vectorA.X * vectorB.Y);

            bool intersects = (s >= 0 && s <= 1 && t >= 0 && t <= 1);

            intersectionPoint = default(FVector);
            if (intersects)
            {
                intersectionPoint.X = segmentStartA.X + (t * vectorA.X);
                intersectionPoint.Y = segmentStartA.Y + (t * vectorA.Y);
                intersectionPoint.Z = segmentStartA.Z + (t * vectorA.Z);
            }

            return intersects;
        }

        /// <summary>
        /// Returns closest point on a triangle to a point.
        /// The idea is to identify the halfplanes that the point is
        /// in relative to each triangle segment "plane"
        /// </summary>
        /// <param name="point">point to check distance for</param>
        /// <param name="a">counter clockwise ordering of points defining a triangle</param>
        /// <param name="b">counter clockwise ordering of points defining a triangle</param>
        /// <param name="c">counter clockwise ordering of points defining a triangle</param>
        /// <returns></returns>
        public static FVector ClosestPointOnTriangleToPoint(FVector point, FVector a, FVector b, FVector c)
        {
            //Figure out what region the point is in and compare against that "point" or "edge"
            FVector ba = a - b;
            FVector ac = c - a;
            FVector cb = b - c;
            FVector triNormal = ba ^ cb;

            // Get the planes that define this triangle
            // edges BA, AC, BC with normals perpendicular to the edges facing outward
            FPlane[] planes =
            {
                new FPlane(b, triNormal ^ ba),
                new FPlane(a, triNormal ^ ac),
                new FPlane(c, triNormal ^ cb)
            };
            int planeHalfspaceBitmask = 0;

            //Determine which side of each plane the test point exists
            for (int i = 0; i < 3; i++)
            {
                if (planes[i].PlaneDot(point) > 0.0f)
                {
                    planeHalfspaceBitmask |= (1 << i);
                }
            }

            FVector result = new FVector(point.X, point.Y, point.Z);
            switch (planeHalfspaceBitmask)
            {
                case 0: //000 Inside
                    return FVector.PointPlaneProject(point, a, b, c);
                case 1:	//001 Segment BA
                    result = FMath.ClosestPointOnSegment(point, b, a);
                    break;
                case 2:	//010 Segment AC
                    result = FMath.ClosestPointOnSegment(point, a, c);
                    break;
                case 3:	//011 point A
                    return a;
                case 4: //100 Segment BC
                    result = FMath.ClosestPointOnSegment(point, b, c);
                    break;
                case 5: //101 point B
                    return b;
                case 6: //110 point C
                    return c;
                default:
                    FMessage.Log(LogUnrealMath, ELogVerbosity.Log, "Impossible result in FMath::ClosestPointOnTriangleToPoint");
                    break;
            }

            return result;
        }

        /// <summary>
        /// Returns closest point on a tetrahedron to a point.
        /// The idea is to identify the halfplanes that the point is
        /// in relative to each face of the tetrahedron
        /// </summary>
        /// <param name="point">point to check distance for</param>
        /// <param name="a">four points defining a tetrahedron</param>
        /// <param name="b">four points defining a tetrahedron</param>
        /// <param name="c">four points defining a tetrahedron</param>
        /// <param name="d">four points defining a tetrahedron</param>
        /// <returns>Point on tetrahedron ABCD closest to given point</returns>
        public static FVector ClosestPointOnTetrahedronToPoint(FVector point, FVector a, FVector b, FVector c, FVector d)
        {
            //Check for coplanarity of all four points
            Debug.Assert(Abs((c - a) | ((b - a) ^ (d - c))) > 0.0001f, "Coplanar points in FMath::ComputeBaryCentric3D()");

            //http://osdir.com/ml/games.devel.algorithms/2003-02/msg00394.html
            //     D
            //    /|\		  C-----------B
            //   / | \		   \         /
            //  /  |  \	   or	\  \A/	/
            // C   |   B		 \	|  /
            //  \  |  /			  \	| /
            //   \ | /			   \|/
            //     A				D

            // Figure out the ordering (is D in the direction of the CCW triangle ABC)
            FVector pt1 = a, pt2 = b, pt3 = c, pt4 = d;
            FPlane abc = new FPlane(a, b, c);
            if (abc.PlaneDot(d) < 0.0f)
            {
                //Swap two points to maintain CCW orders
                pt3 = d;
                pt4 = c;
            }

            //Tetrahedron made up of 4 CCW faces - DCA, DBC, DAB, ACB
            FPlane[] planes =
            {
                new FPlane(pt4, pt3, pt1),
                new FPlane(pt4, pt2, pt3),
                new FPlane(pt4, pt1, pt2),
                new FPlane(pt1, pt3, pt2)
            };

            //Determine which side of each plane the test point exists
            int planeHalfspaceBitmask = 0;
            for (int i = 0; i < 4; i++)
            {
                if (planes[i].PlaneDot(point) > 0.0f)
                {
                    planeHalfspaceBitmask |= (1 << i);
                }
            }

            //Verts + Faces - Edges = 2	(Euler)
            FVector result = new FVector(point.X, point.Y, point.Z);
            switch (planeHalfspaceBitmask)
            {
                case 0:	 //inside (0000)
                    //@TODO - could project point onto any face
                    break;
                case 1:	 //0001 Face	DCA
                    return FMath.ClosestPointOnTriangleToPoint(point, pt4, pt3, pt1);
                case 2:	 //0010 Face	DBC
                    return FMath.ClosestPointOnTriangleToPoint(point, pt4, pt2, pt3);
                case 3:  //0011	Edge	DC
                    result = FMath.ClosestPointOnSegment(point, pt4, pt3);
                    break;
                case 4:	 //0100 Face	DAB
                    return FMath.ClosestPointOnTriangleToPoint(point, pt4, pt1, pt2);
                case 5:  //0101	Edge	DA
                    result = FMath.ClosestPointOnSegment(point, pt4, pt1);
                    break;
                case 6:  //0110	Edge	DB
                    result = FMath.ClosestPointOnSegment(point, pt4, pt2);
                    break;
                case 7:	 //0111 Point	D
                    return pt4;
                case 8:	 //1000 Face	ACB
                    return FMath.ClosestPointOnTriangleToPoint(point, pt1, pt3, pt2);
                case 9:  //1001	Edge	AC	
                    result = FMath.ClosestPointOnSegment(point, pt1, pt3);
                    break;
                case 10: //1010	Edge	BC
                    result = FMath.ClosestPointOnSegment(point, pt2, pt3);
                    break;
                case 11: //1011 Point	C
                    return pt3;
                case 12: //1100	Edge	BA
                    result = FMath.ClosestPointOnSegment(point, pt2, pt1);
                    break;
                case 13: //1101 Point	A
                    return pt1;
                case 14: //1110 Point	B
                    return pt2;
                default: //impossible (1111)
                    FMessage.Log(LogUnrealMath, ELogVerbosity.Log, "FMath::ClosestPointOnTetrahedronToPoint() : impossible result");
                    break;
            }

            return result;
        }

        /// <summary>
        /// Find closest point on a Sphere to a Line.
        /// When line intersects		Sphere, then closest point to LineOrigin is returned.
        /// </summary>
        /// <param name="sphereOrigin">Origin of Sphere</param>
        /// <param name="sphereRadius">Radius of Sphere</param>
        /// <param name="lineOrigin">Origin of line</param>
        /// <param name="normalizedLineDir">Direction of line. Needs to be normalized!!</param>
        /// <param name="closestPoint">Closest point on sphere to given line.</param>
        public static void SphereDistToLine(FVector sphereOrigin, float sphereRadius, FVector lineOrigin, FVector normalizedLineDir, out FVector closestPoint)
        {
            //const float A = NormalizedLineDir | NormalizedLineDir  (this is 1 because normalized)
            //solving quadratic formula in terms of t where closest point = LineOrigin + t * NormalizedLineDir
            FVector lineOriginToSphereOrigin = sphereOrigin - lineOrigin;
            float b = -2.0f * (normalizedLineDir | lineOriginToSphereOrigin);
            float c = lineOriginToSphereOrigin.SizeSquared() - FMath.Square(sphereRadius);
            float d = FMath.Square(b) - 4.0f * c;

            if (d <= KindaSmallNumber)
            {
                // line is not intersecting sphere (or is tangent at one point if D == 0 )
                FVector pointOnLine = lineOrigin + (-b * 0.5f) * normalizedLineDir;
                closestPoint = sphereOrigin + (pointOnLine - sphereOrigin).GetSafeNormal() * sphereRadius;
            }
            else
            {
                // Line intersecting sphere in 2 points. Pick closest to line origin.
                float E = FMath.Sqrt(d);
                float t1 = (-b + E) * 0.5f;
                float t2 = (-b - E) * 0.5f;
                float t = FMath.Abs(t1) == FMath.Abs(t2) ? FMath.Abs(t1) : FMath.Abs(t1) < FMath.Abs(t2) ? t1 : t2;// In the case where both points are exactly the same distance we take the one in the direction of LineDir

                closestPoint = lineOrigin + t * normalizedLineDir;
            }
        }

        /// <summary>
        /// Calculates whether a Point is within a cone segment, and also what percentage within the cone (100% is along the center line, whereas 0% is along the edge)
        /// </summary>
        /// <param name="point">The Point in question</param>
        /// <param name="coneStartPoint">the beginning of the cone (with the smallest radius)</param>
        /// <param name="coneLine">the line out from the start point that ends at the largest radius point of the cone</param>
        /// <param name="radiusAtStart">the radius at the ConeStartPoint (0 for a 'proper' cone)</param>
        /// <param name="radiusAtEnd">the largest radius of the cone</param>
        /// <param name="percentage">output variable the holds how much within the cone the point is (1 = on center line, 0 = on exact edge or outside cone).</param>
        /// <returns>true if the point is within the cone, false otherwise.</returns>
        public static bool GetDistanceWithinConeSegment(FVector point, FVector coneStartPoint, FVector coneLine, float radiusAtStart, float radiusAtEnd, out float percentage)
        {
            Debug.Assert(radiusAtStart >= 0.0f && radiusAtEnd >= 0.0f && coneLine.SizeSquared() > 0);
            // -- First we'll draw out a line from the ConeStartPoint down the ConeLine. We'll find the closest point on that line to Point.
            //    If we're outside the max distance, or behind the StartPoint, we bail out as that means we've no chance to be in the cone.

            FVector pointOnCone;// Stores the point on the cone's center line closest to our target point.

            float distance = FMath.PointDistToLine(point, coneLine, coneStartPoint, out pointOnCone);// distance is how far from the viewline we are

            percentage = 0.0f;// start assuming we're outside cone until proven otherwise.

            FVector vectToStart = coneStartPoint - pointOnCone;
            FVector vectToEnd = (coneStartPoint + coneLine) - pointOnCone;

            float coneLengthSqr = coneLine.SizeSquared();
            float distToStartSqr = vectToStart.SizeSquared();
            float distToEndSqr = vectToEnd.SizeSquared();

            if (distToStartSqr > coneLengthSqr || distToEndSqr > coneLengthSqr)
            {
                //Outside cone
                return false;
            }

            float percentAlongCone = FMath.Sqrt(distToStartSqr) / FMath.Sqrt(coneLengthSqr); // don't have to catch outside 0->1 due to above code (saves 2 sqrts if outside)
            float radiusAtPoint = radiusAtStart + ((radiusAtEnd - radiusAtStart) * percentAlongCone);

            if (distance > radiusAtPoint) // target is farther from the line than the radius at that distance)
            {
                return false;
            }

            percentage = radiusAtPoint > 0.0f ? (radiusAtPoint - distance) / radiusAtPoint : 1.0f;

            return true;
        }

        /// <summary>
        /// Determines whether a given set of points are coplanar, with a tolerance. Any three points or less are always coplanar.
        /// </summary>
        /// <param name="points">The set of points to determine coplanarity for.</param>
        /// <param name="tolerance">Larger numbers means more variance is allowed.</param>
        /// <returns>Whether the points are relatively coplanar, based on the tolerance</returns>
        public static bool PointsAreCoplanar(FVector[] points, float tolerance = 0.1f)
        {
            //less than 4 points = coplanar
            if (points.Length < 4)
            {
                return true;
            }

            //Get the Normal for plane determined by first 3 points
            FVector normal = FVector.CrossProduct(points[2] - points[0], points[1] - points[0]).GetSafeNormal();

            int total = points.Length;
            for (int v = 3; v < total; v++)
            {
                //Abs of PointPlaneDist, dist should be 0
                if (FMath.Abs(FVector.PointPlaneDist(points[v], points[0], normal)) > tolerance)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
