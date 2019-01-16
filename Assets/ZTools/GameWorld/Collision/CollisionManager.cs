using System.Collections.Generic;
using UnityEngine;

namespace ZTools.Game.CollisionUtil
{

    /// <summary>
    /// When 3 object collide in one frame, everyone will receive a collision event.
    /// This is not always desirable.
    /// Sometimes we want to collision only happen in two object in one frame,
    /// Then we need a center to record some info.
    /// 
    /// Notice, the order of onTriggerEnter function call is random !
    /// 
    /// </summary>
    public class CollisionManager : MonoBehaviour
    {
        internal class CollisionPairInfo
        {
            public CollisionPairInfo(CollisionAbility c1, CollisionAbility c2)
            {
                _c1 = c1;
                _c2 = c2;
            }
            private CollisionAbility _c1 = null;
            private CollisionAbility _c2 = null;

            public bool has(CollisionAbility c)
            {
                return _c1 == c || _c2 == c;
            }
        }

        private static HashSet<CollisionPairInfo> _collisionCache = new HashSet<CollisionPairInfo>();

        [SerializeField] private CollisionConfig _collisionConfig;

        //clear before next frame
        void LateUpdate()
        {
            _collisionCache.Clear();
        }

        public static bool CanPassTypeTest(CollisionAbility me, CollisionAbility other)
        {
            //TODO : use type matrix? or typebitmask?

            return false;
        }

        /// <summary>
        /// c1 and c2 must be in same pair or not recorded at all to return a valid collison
        /// </summary>
        public static bool CanPassExclusiveTest(CollisionAbility me, CollisionAbility other)
        {
            if (me == null || other == null /*|| c1.abilityOwner == null || c2.abilityOwner == null*/)
            {
                return false;
            }

            bool ret = true;
            bool needAdd = true;
            foreach (var pairInfo in _collisionCache)
            {
                if(pairInfo.has(me))
                {
                    if (!pairInfo.has(other))
                    {
                        ret = false;
                    }
                    needAdd = false;
                }
                else
                {
                    if(pairInfo.has(other))
                    {
                        ret = false;
                        needAdd = false;
                    }
                }
            }

            if(needAdd)
            {
                //ZLog.verbose("add", c1.abilityOwner.name, ",", c2.abilityOwner.name);
                _collisionCache.Add(new CollisionPairInfo(me, other));
            }

            //if(ret)
            //{
            //    ZLog.verbose("collision accept", c1.abilityOwner.name, ",", c2.abilityOwner.name);
            //}

            return ret;

        }
    }
}
