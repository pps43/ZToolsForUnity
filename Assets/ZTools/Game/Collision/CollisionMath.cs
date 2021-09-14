using UnityEngine;

namespace ZTools.Game.Collision
{
    public class CollisionMath
    {
        private const int MAX_OVERLAPPED_CACHE = 50;
        private static Collider2D[] _overlappedCache = new Collider2D[MAX_OVERLAPPED_CACHE];


        #region None physics-based method. Only need collider.

        #region Overlaping


        /// <summary>
        /// Judge if a point is inside my collider.
        /// </summary>
        /// <returns></returns>
        public bool IsPointInside(Collider2D my, Vector2 point)
        {
            return my.OverlapPoint(point);

        }

        /// <summary>
        /// Get all the filtered colliders that overlap with my collider.
        /// 
        /// filter example:
        /// 
        /// ContactFilter2D filter = new ContactFilter2D();
        /// filter.SetLayerMask(LayerMask.GetMask("Default"));
        /// filter.useLayerMask = true;
        /// </summary>
        /// <returns> The number of valid results in res array</returns>
        public int GetAllOverlappedColliders(Collider2D my, ContactFilter2D filter, Collider2D[] res)
        {
            return my.OverlapCollider(filter, res);
        }

        /// <summary>
        /// Judge if collider other overlaps with my collider after filtering.
        /// </summary>
        /// <returns></returns>
        public bool IsTwoColliderOverlap(Collider2D my, Collider2D other, ContactFilter2D filter)
        {
            int num = GetAllOverlappedColliders(my, filter, _overlappedCache);
            if (num > 0)
            {
                for (int i = 0; i < num; i++)
                {
                    if (_overlappedCache[i] == other)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion


        #region Casting

        #endregion


        #endregion



        #region Physics-based method. Need rigidbody and collider.

        #region Ray Casting

        //if (Physics.Raycast(originPos, dir, out hit, range, layerMask ))
        //{
        // hit.transform.name  碰到的物体名称
        // hit.point  碰撞点
        // hit.normal  碰撞点的表面法线（向外）
        //} 

        #endregion

        #region Shape Casting

        #endregion


        #endregion


    }
}