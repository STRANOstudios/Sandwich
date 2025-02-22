using UnityEngine;

namespace AndreaFrigerio.Framework.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Calculates the intersection point of a ray with a plane.
        /// </summary>
        /// <param name="ray">The ray that intersects the plane.</param>
        /// <param name="planeNormal">The normal vector of the plane. It defines the plane's orientation.</param>
        /// <param name="planePoint">A point on the plane, used to define the plane's position in space.</param>
        /// <returns>
        /// The point where the ray intersects the plane, or <c>Vector3.zero</c> if there is no intersection (i.e., the ray is parallel to the plane).
        /// </returns>
        public static Vector3 IntersectRayPlane(Ray ray, Vector3 planeNormal, Vector3 planePoint)
        {
            float denominator = Vector3.Dot(ray.direction, planeNormal);
            if (denominator == 0) return Vector3.zero;
            float dist = Vector3.Dot(planePoint - ray.origin, planeNormal) / denominator;
            return ray.origin + ray.direction * dist;
        }
    }
}
