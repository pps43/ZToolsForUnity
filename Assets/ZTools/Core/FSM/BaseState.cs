﻿namespace ZTools.Core
{
    /// <summary>
    /// base class for state, used in FSM (Finite state machine)
    /// </summary>
    public abstract class BaseState<T, M> where T : class
    {
        public virtual object OnMessage(T owner, M msg) { return null; }

        public virtual void Enter(T owner, object param) { }

        public virtual void Update(T owner) { }

        public virtual void Exit(T owner) { }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }

}