using System;
using UnityEngine;
using ZTools.Event;

namespace ZTools.FSM.Demo
{
    public class Enemy : MonoBehaviour, IUpdatable
    {
        public FSM<Enemy, SelfEvent, CommonEvent> fsm { get; private set; }
        public event Action updateEvent;
        private EventHelper _eventHelper = new EventHelper();

        public float health { get; private set; }

        #region UnityEvent

        private void Start()
        {
            health = 100f;
            _eventHelper.addListener(EventID.onTurn, onGameTurn);
            fsm = FSMFactory.createFSM(this, new IdleState(), new GlobalState());
            fsm.start();
        }

        private void OnDestroy()
        {
            fsm.stop();
        }

        private void LateUpdate()
        {
            updateEvent?.Invoke();
        }

        #endregion

        #region Sense Layer, receive game event

        //this event comes from EventDispachter
        private bool onGameTurn(CommonEvent eventObj)
        {
            return fsm.onMessage(eventObj, out _);
        }

        //this event may from collision module
        private void onHurt()
        {
            health -= 10f;
            fsm.onMessage(new SelfEvent(SelfEvent.ID.onHurt), out _);
        }

        //this event may from animation module
        private void onAttackEnd()
        {
            fsm.onMessage(new SelfEvent(SelfEvent.ID.onAttackEnd), out _);
        }

        #endregion

        #region Action Layer, called by fsm

        public void doIdle()
        {
            //logic do sth
            //animator do sth
        }

        public void doAttack()
        {
            //logic do sth
            //animator do sth
        }

        public void doDie()
        {
            Destroy(gameObject);
        }

        #endregion

    }
}