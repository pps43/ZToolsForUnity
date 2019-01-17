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

        public bool CanPassTypeTest(CollisionAbility me, CollisionAbility other)
        {
            if (me == null || other == null)
                return false;

            return _collisionConfig[(int)me.Type, (int)other.Type];
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

        //public bool canPassExclusiveTest(CollisionJudgeAbility me, CollisionJudgeAbility other)
        //{
        //    if (me == null || other == null)
        //        return false;

        //    foreach (ColliderInfo info in _collisionSnapshootSet)
        //    {
        //        if ((info.c1 == me && info.c2 == other) || (info.c1 == other && info.c2 == me))
        //            return true;
        //    }


        //    if (me.canReceiveCollision && other.canReceiveCollision)//两者都是enable，可碰撞并记录在字典里
        //    {
        //        foreach (ColliderInfo info in _collisionSnapshootSet)
        //        {
        //            if ((info.c1 == me && info.c2 == other) || (info.c1 == other && info.c2 == me))
        //                return true;
        //        }
        //        _collisionSnapshootSet.Add(new ColliderInfo(me, other));
        //        return true;
        //    }
        //    else if (me.canReceiveCollision || other.canReceiveCollision)//否则查看是否在字典里有记录，因为两个actor的碰撞回调有先后顺序，所以不能交给actor个体自己判断对方的enable。
        //    {
        //        foreach (ColliderInfo info in _collisionSnapshootSet)
        //        {
        //            if ((info.c1 == me && info.c2 == other) || (info.c1 == other && info.c2 == me))
        //                return true;
        //        }
        //    }


        //    return false;
        //}
    }
}
