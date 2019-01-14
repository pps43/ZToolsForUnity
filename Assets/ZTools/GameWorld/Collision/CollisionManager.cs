using System.Collections.Generic;
using UnityEngine;

namespace ZTools.Game.Collision
{
    //temp
    public class CollideAbility
    {

    }


    /// <summary>
    /// When 3 object collide in one frame, everyone will receive a collision event.
    /// This is not always desirable.
    /// Sometimes we want to collision only happen in two object in one frame,
    /// Then we need a center to record some info.
    /// 
    /// Notice, the order of onTriggerEnter function call is random !
    /// 
    /// </summary>
    public class CollsionManager : MonoBehaviour
    {
        internal class CollisionPairInfo
        {
            public CollisionPairInfo(CollideAbility c1, CollideAbility c2)
            {
                _c1 = c1;
                _c2 = c2;
            }
            private CollideAbility _c1 = null;
            private CollideAbility _c2 = null;

            public bool has(CollideAbility c)
            {
                return _c1 == c || _c2 == c;
            }
        }

        private HashSet<CollisionPairInfo> _collisionCache = new HashSet<CollisionPairInfo>();

        //clear before next frame
        void LateUpdate()
        {
            _collisionCache.Clear();
        }

        /// <summary>
        /// c1 and c2 must be in same pair or not recorded at all to return a valid collison
        /// </summary>
        public bool CanCollideExclusively(CollideAbility c1, CollideAbility c2)
        {
            if (c1 == null || c2 == null /*|| c1.abilityOwner == null || c2.abilityOwner == null*/)
            {
                return false;
            }

            bool ret = true;
            bool needAdd = true;
            foreach (var pairInfo in _collisionCache)
            {
                if(pairInfo.has(c1))
                {
                    if (!pairInfo.has(c2))
                    {
                        ret = false;
                    }
                    needAdd = false;
                }
                else
                {
                    if(pairInfo.has(c2))
                    {
                        ret = false;
                        needAdd = false;
                    }
                }
            }

            if(needAdd)
            {
                //ZLog.verbose("add", c1.abilityOwner.name, ",", c2.abilityOwner.name);
                _collisionCache.Add(new CollisionPairInfo(c1, c2));
            }

            //if(ret)
            //{
            //    ZLog.verbose("collision accept", c1.abilityOwner.name, ",", c2.abilityOwner.name);
            //}

            return ret;

        }
    }
}
