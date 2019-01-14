using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZTools.FSM;

namespace ZTools.Game
{
    /// <summary>
    /// Base class of an actor,
    /// which can be equiped with some BaseAbilities, and an FSM as AI.
    /// 
    /// Commented code below is an example of subclass of BaseActor
    /// </summary>
    public class BaseActor : BaseObject, IUpdatable
    {
        public event Action updateEvent;

        //[SerializeField] private xxAbility;
        //[SerializeField] private yyAbility;

        //public FSM<Enemy, SelfEvent, CommonEvent> fsm { get; private set; }


        public override void Init()
        {
            base.Init();

            //xxAbility.Init(this);
            //yyAbility.Init(this);

            //fsm = FSMFactory.createFSM();
            //fsm.Start();
        }

        public override void UnInit()
        {
            base.UnInit();

            //xxAbility.UnInit(this);
            //yyAbility.UnInit(this);

            // fsm.Stop();
        }

        void LateUpdate()
        {
            updateEvent?.Invoke();
        }
    }
}