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

        public static void DrawArrow(Vector3 begin, Vector3 end, Color newColor)
        {
            Color ori = Gizmos.color;
            Gizmos.color = newColor;

            Gizmos.DrawLine(begin, end);

            float arrowHeadAngle = 20;
            float arrowHeadLength = 5;

            Vector3 direction = end - begin;
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
            Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;

            Gizmos.DrawRay(end, right * arrowHeadLength);
            Gizmos.DrawRay(end, left * arrowHeadLength);

            Gizmos.color = ori;
        }

        public static void DrawCircle2D(Vector3 center, float radius, Color newColor)
        {
#if UNITY_EDITOR
            Color ori = UnityEditor.Handles.color;
            UnityEditor.Handles.color = newColor;

            UnityEditor.Handles.DrawWireDisc(center, Vector3.back, radius);
            UnityEditor.Handles.color = ori;
#endif

        }
    }
}