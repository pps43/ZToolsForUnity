using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZTools.Core;
using ZTools.Game;
using ZTools.Editor.CustomAttribute;
namespace ZTools.Demo
{
    public enum EnemyType
    {
        none,
        walker,
        flyer,
    }


    public class Enemy : BaseActor
    {
        [SerializeField] private EnemyType _type = EnemyType.none;
        public override int TypeID => (int)_type;

        public float Health { get; private set; }
        public FSM<Enemy, LocalMsg> FSM { get; private set; }

        public override void Init()
        {
            base.Init();
            Health = 100f;
            Singleton<EventDispatcher<EventID,EventData>>.Instance.AddListener(EventID.OnDamage, OnDamageHandler);
            FSM = FSMManager.Instance.createFSM(this, new IdleState(), new GlobalState());
            FSM.Start();
        }

        private void OnDamageHandler(EventData data)
        {
            if (data is DamageData msg)
            {
                
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            FSM.Stop();
        }


        #region Sense Layer, receive game event
        

        //this event may from collision module
        private void OnHurt()
        {
            Health -= 10f;
            FSM.OnMessage(new LocalMsg(LocalMsg.ID.onHurt));
        }

        //this event may from animation module
        private void OnAttackEnd()
        {
            FSM.OnMessage(new LocalMsg(LocalMsg.ID.onAttackEnd));
        }

        #endregion

        #region Action Layer, called by fsm

        public void DoIdle()
        {
            //logic do sth
            //animator do sth
        }

        public void DoAttack()
        {
            //logic do sth
            //animator do sth
        }

        public void DoDie()
        {
            //Destroy(gameObject);
        }

        #endregion

    }
}