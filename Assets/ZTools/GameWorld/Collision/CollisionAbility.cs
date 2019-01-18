using System;
using UnityEngine;
using ZTools.EditorUtil.CustomAttribute;
using ZTools.DebugUtil;

namespace ZTools.Game.CollisionUtil
{
    public enum ColliderType
    {
        hero,
        sword,
        enemy,
    }

    /// <summary>
    /// Collision filtering takes 2 step:
    /// 1. layer and collision matrix defined in project settings.
    /// 2. this script
    /// </summary>
    public class CollisionAbility : BaseAbility
    {

        /// <summary>
        /// 2D and 3D cannot collide together.
        /// </summary>
        public bool Is2DMode = true;

        /// <summary>
        /// Turn it on to receive both collison and trigger event.
        /// </summary>
        public bool CanReceiveTrigger = true;

        /// <summary>
        /// Sometimes when handling a pair of collision,
        /// we want to block out all other collision without calling UnInit(). 
        /// Assign this value to false in this scenario.
        /// 
        /// e.g. when an enemy dies, it may no longer collide with other weapon according to your design,
        /// but if you use FSM, it will turn to dead state in next frame, so following weapon still collides with it.
        /// If you call UnInit() immediately, it may cause none object reference error in another BaseObject's script,
        /// since ontrigger function's order is ramdom in one collision pair.
        /// </summary>
        [ReadOnly] public bool CanCollideMoreInCollision = false;


        public event Action<CollisionAbility> OnGameCollisionEnter;
        public event Action<CollisionAbility> OnGameCollisionExit;

        public ColliderType Type { get { return _type; } }
        [SerializeField] private ColliderType _type = ColliderType.hero; //modify in editor

        public override void Init(BaseObject ownerObject)
        {
            base.Init(ownerObject);
            CanCollideMoreInCollision = true;
        }

        public override void UnInit()
        {
            base.UnInit();
            CanCollideMoreInCollision = false;
        }

        #region Unity Collision Enter event

        private void OnCollisionEnter(Collision other)
        {
            if (!HasInit || other == null) { return; }
            if (Is2DMode) { return; }

            CollisionAbility ca = other.gameObject.GetComponent<CollisionAbility>();
            ProcessCollisionEnter(ca);
        }
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!HasInit || other == null) { return; }
            if (!Is2DMode) { return; }

            CollisionAbility ca = other.gameObject.GetComponent<CollisionAbility>();
            ProcessCollisionEnter(ca);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!HasInit || other == null) { return; }
            if (Is2DMode) { return; }

            if (CanReceiveTrigger)
            {
                CollisionAbility ca = other.gameObject.GetComponent<CollisionAbility>();
                ProcessCollisionEnter(ca);
            }
            
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!HasInit || other == null) { return; }
            if (!Is2DMode) { return; }

            if (CanReceiveTrigger)
            {
                CollisionAbility ca = other.gameObject.GetComponent<CollisionAbility>();
                ProcessCollisionEnter(ca);
            }
        }
        #endregion

        #region Unity Collison Exit event

        private void OnCollisionExit(Collision other)
        {
            if (!HasInit || other == null) { return; }
            if (Is2DMode) { return; }

            CollisionAbility ca = other.gameObject.GetComponent<CollisionAbility>();
            ProcessCollisionExit(ca);
        }
        private void OnCollisionExit2D(Collision2D other)
        {
            if (!HasInit || other == null) { return; }
            if (!Is2DMode) { return; }

            CollisionAbility ca = other.gameObject.GetComponent<CollisionAbility>();
            ProcessCollisionExit(ca);
        }
        private void OnTriggerExit(Collider other)
        {
            if (!HasInit || other == null) { return; }
            if (Is2DMode) { return; }

            if (CanReceiveTrigger)
            {
                CollisionAbility ca = other.gameObject.GetComponent<CollisionAbility>();
                ProcessCollisionExit(ca);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!HasInit || other == null) { return; }
            if (!Is2DMode) { return; }

            if (CanReceiveTrigger)
            {
                CollisionAbility ca = other.gameObject.GetComponent<CollisionAbility>();
                ProcessCollisionExit(ca);
            }
        }

        #endregion


        private void ProcessCollisionEnter(CollisionAbility other)
        {
            if (CanCollideWith(other))
            {
                OnGameCollisionEnter?.Invoke(other);
            }
        }
        private void ProcessCollisionExit(CollisionAbility other)
        {
            if(CanCollideWith(other))
            {
                OnGameCollisionExit?.Invoke(other);
            }
        }

        private bool CanCollideWith(CollisionAbility other)
        {
            //extra protection
            if (!HasInit || other == null) { return false; }

            //bool isTypeCompatible = other.colliderTypeBit & this.colliderMask) > 0 ? true : false;
            bool isTypeCompatible = GamePlay.instance.collisionManager.CanPassTypeTest(this, other); // TODO do we need GamePlay ref?

            if (isTypeCompatible)
            {
                bool isPassExclusiveTest = GamePlay.instance.collisionManager.CanPassExclusiveTest(this, other);
                if(isPassExclusiveTest)
                {
                    return true;
                }
                else
                {
                    ZLog.verbose(owner.name, "fail to pass exclusive test with:", other.owner.name);
                }
            }
            else
            {
                ZLog.verbose(owner.name, "cannot collide with type:", other.Type.ToString());
            }
            return false;
        }
    }
}