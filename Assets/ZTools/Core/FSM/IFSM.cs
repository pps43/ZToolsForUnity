using System;
using System.Collections.Generic;
using ZTools.Debug;

namespace ZTools.Core
{
    /// <summary>
    /// Provide a unified interface managed by <see cref="FSMManager"/>.
    /// </summary>
    public interface IFSM
    {
        public bool IsRunning { get; set; }
        public void Start();
        public void Pause();
        public void Resume();
        public void Stop();
        public void Update();

        public delegate void OnDispose(IFSM fsm);
        public event OnDispose OnSelfDispose;
    }


    /// <summary>
    /// FSM (Finite state machine)
    /// 
    /// T is FSM's owner's type.
    /// M is custom msg type.
    /// 
    /// Change state immediately rather than next frame to keep simplicity and robustness.
    /// 
    /// Notice that one state may not call update() if time interval between enter() and exit() is less than one frame interval.
    /// 
    /// All onMessage function return an object.
    /// This is useful when you want to get some data/response immediately.
    /// 
    /// globalState is a state that receive msg at anytime.
    /// This is very useful when all your states need to respond the same msg.
    /// E.g. when HP less than 0, die whatever state.
    /// 
    /// -----Example code----
    /// fsm = FSMFactory.createFSM(owner, new IdleState(), new GlobalState());
    /// fsm.start();
    /// fsm.onMessage(new YourMsgType());
    /// fsm.stop();
    /// 
    /// </summary>
    public class FSM<T, M> : IFSM where T : class
    {
        public bool IsRunning { get; set; }

        private T owner;
        private BaseState<T, M> curState;
        private BaseState<T, M> lastState;
        private BaseState<T, M> globalState;

        public event IFSM.OnDispose OnSelfDispose;

        public FSM(T owner, BaseState<T, M> state, BaseState<T, M> globalState)
        {
            this.IsRunning = false;
            this.owner = owner;
            this.lastState = state;
            this.curState = state;
            this.globalState = globalState;
        }

        public void Start()
        {
            if (!this.IsRunning)
            {
                this.IsRunning = true;
                this.curState.Enter(this.owner, null);
            }
        }

        public void Pause()
        {
            this.IsRunning = false;
        }

        public void Resume()
        {
            this.IsRunning = true;
        }

        public void Stop()
        {
            if (this.IsRunning)
            {
                this.curState.Exit(this.owner);
                this.IsRunning = false;
                this.OnSelfDispose?.Invoke(this);
            }
        }

        public void Update()
        {
            if (this.IsRunning)
            {
                if (this.curState != null) { this.curState.Update(this.owner); }
                if (this.globalState != null) { this.globalState.Update(this.owner); }
            }
        }

        /// <summary>
        /// dispatch a message to fsm and get a return msg.
        /// </summary>
        public object OnMessage(M msg, bool retFromGlobal = false)
        {
            object msgRet = null;
            object msgRetGlobal = null;
            if (this.IsRunning)
            {
                if (this.curState != null) { msgRet = this.curState.OnMessage(this.owner, msg); }
                if (this.globalState != null) { msgRetGlobal = this.globalState.OnMessage(this.owner, msg); }
            }
            return retFromGlobal ? msgRetGlobal : msgRet;
        }

        /// <summary>
        /// change state immediately with some data
        /// </summary>
        public void ChangeState(BaseState<T, M> newState, object param = null)
        {
            if (!this.IsRunning)
            {
                ZLog.error("Cannot change state, FSM is not runnning");
                return;
            }

            if (newState == null)
            {
                ZLog.error(owner, "cannot change state to null");
                return;
            }
            if (this.lastState == null || this.curState == null)
            {
                ZLog.error(owner, "Fatal error: _lastSate || _curState = null, newState=", newState);
                return;
            }

            if (newState.GetType().Equals(this.curState.GetType()))
            {
                ZLog.warn(this.owner, "cannot change to the same state:", this.curState);
                return;
            }

            if (this.owner == null)
            {
                ZLog.error("_owner = null");
                return;
            }

            this.lastState = this.curState;
            this.curState = newState;

            ZLog.log(this.owner, this.lastState, this.curState);

            this.lastState.Exit(this.owner);
            this.curState.Enter(this.owner, param);
        }

        public bool IsInState(Type type)
        {
            return this.curState.GetType().Equals(type);
        }

        public string GetStateName()
        {
            return this.curState.GetType().Name;
        }
    }
}