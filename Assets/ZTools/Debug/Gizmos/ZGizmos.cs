using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZTools.Debug
{
    /// <summary>
    /// draw gizmos with color
    /// </summary>
    public class ZGizmos
    {
        public static void DrawLine(Vector3 from, Vector3 to, Color newColor)
        {
            Color ori = Gizmos.color;
            Gizmos.color = newColor;
            Gizmos.DrawLine(from, to);
            Gizmos.color = ori;
        }

        public static void DrawSphere(Vector3 center, float radius, Color newColor)
        {
            Color ori = Gizmos.color;
            Gizmos.color = newColor;
            Gizmos.DrawSphere(center, radius);
            Gizmos.color = ori;
        }
    }
}