namespace ZTools.FSM
{
    /// <summary>
    /// base class for state, used in FSM (Finite state machine)
    /// </summary>
    public abstract class BaseState<T, P, Q> /*where P : BaseEvent where Q: BaseEvent*/
    {
        public virtual object OnMessage(T owner, P innerMsg) { return null; }
        public virtual object OnMessage(T owner, Q outerMsg) { return null; }

        public virtual void Enter(T owner, object param) { }

        public virtual void Update(T owner) { }

        public virtual void Exit(T owner) { }

        public override string ToString()
        {
            return GetType().Name;
        }
    }

}