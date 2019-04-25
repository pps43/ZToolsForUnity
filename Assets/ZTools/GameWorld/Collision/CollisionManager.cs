using System.Collections.Generic;
using UnityEngine;
using ZTools.DebugUtil;

namespace ZTools.Game.CollisionUtil
{

    /// <summary>
    /// When 3 object collide in one frame, everyone will receive a collision event.
    /// This is not always desirable.
    /// Sometimes we want to collision only happen in two object in one frame,
    /// Then we need a center to record some info.
    /// 
    /// Notice, the call order of onTriggerEnter function is random !
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

            public bool isEqual(CollisionAbility c1, CollisionAbility c2)
            {
                if((_c1 == c1 && _c2==c2) || (_c1 == c2 && _c2 == c1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static HashSet<CollisionPairInfo> _beInCollisionCache = new HashSet<CollisionPairInfo>();

        [SerializeField] private CollisionConfig _collisionConfig = null;

        //clear before next frame
        void LateUpdate()
        {
            _beInCollisionCache.Clear();
        }

        public bool CanPassTypeTest(CollisionAbility me, CollisionAbility other)
        {
            if (me == null || other == null)
                return false;

            if(_collisionConfig != null)
            {
                return _collisionConfig[(int)me.Type, (int)other.Type];
            }
            else
            {
                ZLog.warn("no collison config file attached.");
                return true;
            }
        }

        /// <summary>
        /// c1 and c2 must be in same pair or not recorded at all to return a valid collison
        /// </summary>
        public bool CanPassExclusiveTest(CollisionAbility me, CollisionAbility other)
        {
            //illegal
            if (me == null || other == null /*|| c1.abilityOwner == null || c2.abilityOwner == null*/)
            {
                return false;
            }

            //same pair
            foreach (var pairInfo in _beInCollisionCache)
            {
                if(pairInfo.isEqual(me, other))
                {
                    return true;
                }
            }

            //maybe new pair. examine CanCollideMoreInCollision
            if(me.CanReceiveMoreCollision && other.CanReceiveMoreCollision)
            {
                //ZLog.verbose("add", c1.abilityOwner.name, ",", c2.abilityOwner.name);
                _beInCollisionCache.Add(new CollisionPairInfo(me, other));
                return true;
            }

            return false;
        }
    }
}
