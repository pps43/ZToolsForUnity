using ZTools.Event;
using ZTools.FSM;

namespace ZTools.Demo
{
    //this shortcut is only valid in this file
    using BaseState = BaseState<Enemy, LocalMsg>;

    //event definition. use struct to avoid GC
    public struct LocalMsg
    {
        public enum ID
        {
            onHurt,
            onAttackEnd,
            onTurn,
        }

        public ID eventID;
        public object eventData;

        public LocalMsg(ID id, object data = null)
        {
            eventID = id;
            eventData = data;
        }
    }

    /// <summary>
    /// 
    /// State definition. Class is used to utilize inheritance. Cache state obj if you care.
    /// 
    /// In state defination, we write structral behavior of owner, not specific hebavior.
    /// In other words, we should be as abstract as possible to write fsm code, 
    /// focusing on state-change logic, leaving how-to-do logic to owner itself.
    /// e.g. we don't care how owner performs its attack behavior, just call owner.doAttack();
    /// 
    /// Usually we have a class called GlobalState which is always alive when FSM running, 
    /// it's very useful to handle msg that every state needs.
    /// 
    /// </summary>

    public class GlobalState : BaseState
    {
        public override object OnMessage(Enemy owner, LocalMsg msg)
        {
            if (msg.eventID == LocalMsg.ID.onHurt)
            {
                if(owner.Health <= 0)
                {
                    owner.FSM.ChangeState(new DeadState());
                }
            }
            return null;
        }
    }


    public class IdleState : BaseState
    {
        public override void Enter(Enemy owner, object param)
        {
            base.Enter(owner, param);
            owner.DoIdle();
        }

        public override object OnMessage(Enemy owner, LocalMsg msg)
        {
            if(msg.eventID == LocalMsg.ID.onTurn)
            {
                owner.FSM.ChangeState(new AttackState("someparameter"));
            }

            return null;
        }
    }


    public class AttackState : BaseState
    {
        private string _attackParam;

        //use this way to pass parameter when enter another state
        public AttackState(string attackParam = null)
        {
            _attackParam = attackParam;
        }

        public override void Enter(Enemy owner, object param)
        {
            base.Enter(owner, param);
            owner.DoAttack();
        }

        //receive selfEvent
        public override object OnMessage(Enemy owner, LocalMsg msg)
        {
            if(msg.eventID == LocalMsg.ID.onAttackEnd)
            {
                owner.FSM.ChangeState(new IdleState());
            }

            return null;
        }
    }


    public class DeadState : BaseState
    {
        public override void Enter(Enemy owner, object param)
        {
            base.Enter(owner, param);
            owner.DoDie();
        }
    }


}