﻿using System;
using UnityEngine;
using ZTools.Editor.CustomAttribute;
using ZTools.Debug;

namespace ZTools.Game.Collision
{
    /// <summary>
    /// You may use this enum to identify colliders,
    /// or just delete this and use tags instead.
    /// </summary>
    public enum ColliderType
    {
        none,
        hero,
        sword,
        enemy,
    }

    /// <summary>
    /// Collision filtering takes 2 step:
    /// 1. Use Layer collision matrix defined in project settings.
    /// 2. TypeTest (Optional) using self-defined matrix.
    /// 3. ExclusiveTest (Optional).
    /// 
    /// However, there is dispute in step 2's necessity and redundance,  
    /// since listerners of OnGameCollisionEnter event always need to 
    /// use "if" to filter different types for different game logic.
    /// 
    /// </summary>
    public class CollisionAbility : BaseAbility
    {

        /// <summary>
        /// 2D and 3D objects cannot collide together.
        /// </summary>
        public bool Is2DMode = true;

        /// <summary>
        /// Use user-defined collision matrix to filter collision.
        /// You can turn it off if you find it's annoying.
        /// </summary>
        public bool UseTypeTest = true;

        /// <summary>
        /// The value "true" means this object can only be in one pair of collision.
        /// For example,
        /// A collide with B and C simultaneously, which means there are 2 pair of collisions.
        /// If we use exclusive test, A will only handle the first collision with B or C.
        /// </summary>
        public bool UseExclusiveTest = false;

        /// <summary>
        /// Turn it on to receive both collison and trigger event.
        /// </summary>
        public bool CanReceiveBothCollisionAndTrigger = true;

        /// <summary>
        /// Sometimes when handling a pair of collision,
        /// we want to block out all other collision without calling UnInit(). 
        /// 
        /// Assign this value to false in this scenario.
        /// 
        /// e.g. when an enemy dies, it may no longer collide with other weapon according to your game design,
        /// but if you use FSM, it will turn to dead state in next frame, so weapon still collides in this frame.
        /// If you call UnInit() immediately, it may cause none object reference error in another BaseObject's script,
        /// since ontrigger function's order is ramdom in one collision pair.
        /// </summary>
        [ReadOnly] public bool CanReceiveMoreCollision = false;


        public event Action<CollisionAbility> OnGameCollisionEnter;
        public event Action<CollisionAbility> OnGameCollisionExit;

        public ColliderType Type { get { return _type; } }
        [SerializeField] private ColliderType _type = ColliderType.none; //modify in editor

        public override void Init(BaseObject ownerObject)
        {
            base.Init(ownerObject);
            CanReceiveMoreCollision = true;
        }

        public override void UnInit()
        {
            base.UnInit();
            CanReceiveMoreCollision = false;
        }

        #region Unity Collision Enter event

        private void OnCollisionEnter(UnityEngine.Collision other)
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

            if (CanReceiveBothCollisionAndTrigger)
            {
                CollisionAbility ca = other.gameObject.GetComponent<CollisionAbility>();
                ProcessCollisionEnter(ca);
            }

        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!HasInit || other == null) { return; }
            if (!Is2DMode) { return; }

            if (CanReceiveBothCollisionAndTrigger)
            {
                CollisionAbility ca = other.gameObject.GetComponent<CollisionAbility>();
                ProcessCollisionEnter(ca);
            }
        }
        #endregion

        #region Unity Collison Exit event

        private void OnCollisionExit(UnityEngine.Collision other)
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

            if (CanReceiveBothCollisionAndTrigger)
            {
                CollisionAbility ca = other.gameObject.GetComponent<CollisionAbility>();
                ProcessCollisionExit(ca);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!HasInit || other == null) { return; }
            if (!Is2DMode) { return; }

            if (CanReceiveBothCollisionAndTrigger)
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
            if (CanCollideWith(other))
            {
                OnGameCollisionExit?.Invoke(other);
            }
        }


        private bool CanCollideWith(CollisionAbility other)
        {
            if (!HasInit || other == null) { return false; }

            bool passTypeTest = UseTypeTest ?
                GamePlay.instance.collisionManager.CanPassTypeTest(this, other) : true;

            if (passTypeTest)
            {
                bool passExclusiveTest = UseExclusiveTest ?
                    GamePlay.instance.collisionManager.CanPassExclusiveTest(this, other) : true;

                if (passExclusiveTest)
                {
                    return true;
                }
                else
                {
                    ZLog.log(owner.name, "fails Exclusive Test with:", other.owner.name);
                }
            }
            else
            {
                ZLog.log(owner.name, "fails Type Test with type:", other.Type);
            }
            return false;
        }
    }
}