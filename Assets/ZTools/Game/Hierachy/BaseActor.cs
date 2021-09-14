using System;
using ZTools.Core;

namespace ZTools.Game
{
    /// <summary>
    /// Base class of an actor,
    /// which can be equiped with some BaseAbilities, and an FSM as AI.
    /// </summary>
    public abstract class BaseActor : BaseObject
    {
        public override void Init()
        {
            base.Init();

            //init ability
            //create fsm via FSMFactory
        }

        public override void UnInit()
        {
            base.UnInit();

            //unInit ability
            //stop fsm
        }

    }
}