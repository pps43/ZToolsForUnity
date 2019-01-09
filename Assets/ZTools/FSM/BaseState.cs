namespace ZTools.FSM
{
    /// <summary>
    /// base class for state, used in FSM (Finite state machine)
    /// </summary>
    public abstract class BaseState<T, P, Q> /*where P : BaseEvent where Q: BaseEvent*/
    {
        public virtual object onMessage(T owner, P innerMsg) { return null; }
        public virtual object onMessage(T owner, Q outerMsg) { return null; }

        public virtual void enter(T owner, object param) { }

        public virtual void update(T owner) { }

        public virtual void exit(T owner) { }

        public override string ToString()
        {
            return GetType().Name;
        }
    }

}