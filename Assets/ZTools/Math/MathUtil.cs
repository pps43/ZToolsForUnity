using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZTools.MathUtil
{
    public class MathUtil : MonoBehaviour
    {
        #region Random
        public static bool RandomBool()
        {
            return Random.value > 0.5f;
        }
        #endregion

        #region Space Geometry

        #region Projection/Cast a Point to Line segment (2D)

        /// <summary>
        /// Project point p along with castDir to line segment (v,w). when isRay defines the projection is on a ray or line.
        /// Return 0 means the projection point is inside the line segment.
        /// Return -1 means the projection point is on the left side of line segment.
        /// Return 1 means right side.
        /// 
        /// To understand this method, refer to twoLineSegmentInterSection、point2LineSegmentDistance
        /// </summary>
        public static int castPointToLineSegment(Vector2 p, Vector2 q1, Vector2 q2, Vector2 castDir, bool isRay, out Vector2 interSectPoint)
        {
            // Ray: P = p + castDir * t (t>=0)
            // LineSeg: Q = v + (w-v) * s (0<=s<=1)
            interSectPoint = Vector2.positiveInfinity;

            Vector2 p1 = p;
            Vector2 p2 = p + castDir;
            int isInside = int.MaxValue;

            //put q1 as left point
            if (q1.x > q2.x)
            {
                Vector2 qTemp = q1;
                q1 = q2; q2 = qTemp;
            }

            float d = (p2.x - p1.x) * (q2.y - q1.y) - (p2.y - p1.y) * (q2.x - q1.x);

            if (d != 0f) // not parallel, nor colinear
            {
                float t = ((q1.x - p1.x) * (q2.y - q1.y) - (q1.y - p1.y) * (q2.x - q1.x)) / d;
                float s = ((q1.x - p1.x) * (p2.y - p1.y) - (q1.y - p1.y) * (p2.x - p1.x)) / d;
                if (t >= 0f && isRay == true || isRay == false)
                {
                    isInside = s < 0f ? -1 : (s > 1f ? 1 : 0);
                    s = Mathf.Clamp(s, 0, 1);//if outside line, project back
                    interSectPoint = q1 + s * (q2 - q1);
                }
            }

            return isInside;

        }

        #endregion

        #region Distance between Point and Line segment (2D)

        /// <summary>
        /// Shortest distance between point p and line segment (v,w).
        /// Also returns t which can calculate the project point of p  = v + t * (w - v)
        /// Also returns distance vector as distVector.
        /// </summary>
        public static float point2LineSegmentDistance(Vector2 p, Vector2 v, Vector2 w, out float t, out Vector2 distVector)
        {
            float sqrLen = (v - w).sqrMagnitude;
            if (sqrLen == 0f)
            {
                t = 0;
                distVector = v - p;
            }
            else
            {
                t = Mathf.Clamp(Vector2.Dot(p - v, w - v) / sqrLen, 0, 1);
                Vector2 pProjection = v + t * (w - v);
                distVector = pProjection - p;
            }

            return distVector.magnitude;
        }

        /// <summary>
        /// Shortest distance between point p and line segment (v,w).
        /// </summary>
        public static float point2LineSegmentDistance(Vector2 p, Vector2 v, Vector2 w)
        {
            return point2LineSegmentDistance(p, v, w, out _, out _);
        }
        #endregion

        #region Distance between two 2D Line segment (2D)

        /// <summary>
        /// Shortest distance between two line segment（p1,p2） and（q1,q2）。If intersects, return 0
        /// </summary>
        /// <returns></returns>
        public static float twoLineSegmentDistance(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            if (twoLineSegmentInterSection(p1, p2, q1, q2))
            {
                return 0f;
            }
            else
            {
                float d1 = point2LineSegmentDistance(p1, q1, q2);
                float d2 = point2LineSegmentDistance(p2, q1, q2);
                float d3 = point2LineSegmentDistance(q1, p1, p2);
                float d4 = point2LineSegmentDistance(q2, p1, p2);
                return Mathf.Min(d1, d2, d3, d4);
            }
        }
        #endregion

        #region Intersection between two line segments (2D)

        /// <summary>
        /// Judge if two line segments intersects with each other, and return intersect point.
        /// Two line segments are defined as （p1,p2） and （q1,q2）, respectively.
        /// </summary>
        /// <returns> is intersected or not.</returns>
        public static bool twoLineSegmentInterSection(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, out Vector2 interSectPoint)
        {
            interSectPoint = Vector2.negativeInfinity;

            float d = (p2.x - p1.x) * (q2.y - q1.y) - (p2.y - p1.y) * (q2.x - q1.x);

            if (d == 0f)
            {
                return false; //parallel, or colinear
            }

            float u = ((q1.x - p1.x) * (q2.y - q1.y) - (q1.y - p1.y) * (q2.x - q1.x)) / d;
            float v = ((q1.x - p1.x) * (p2.y - p1.y) - (q1.y - p1.y) * (p2.x - p1.x)) / d;

            if (u < 0f || u > 1f || v < 0f || v > 1f)
            {
                return false; // outside
            }

            interSectPoint = p1 + u * (p2 - p1);

            return true;
        }

        /// <summary>
        /// Judge if two line segments intersects with each other.
        /// Two line segments are defined as （p1,p2） and （q1,q2）, respectively.
        /// </summary>
        /// <returns> is intersected or not.</returns>
        public static bool twoLineSegmentInterSection(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            return twoLineSegmentInterSection(p1, p2, q1, q2, out _);
        }

        #endregion

        #endregion
    }
}