using System;
using System.Collections.Generic;
using ZTools.DebugUtil;

namespace ZTools.FSM
{
    /// <summary>
    /// 方便 FSMManager 管理
    /// </summary>
    public abstract class BaseFSM
    {
        public abstract bool IsRunning { get; protected set; }
        public abstract void Start();
        public abstract void Pause();
        public abstract void Resume();
        public abstract void Stop();//dispose
        public abstract void Update();

        public Action<BaseFSM> DisposeEvent;
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
    /// _globalState is a state that receive msg at anytime.
    /// This is very useful when all your states need to respond the same msg.
    /// E.g. when HP less than 0, die whatever state.
    /// 
    /// -----Example code----
    /// fsm = FSMFactory.createFSM(owner, new IdleState(), new GlobalState());
    /// fsm.start();
    /// fsm.onMessage(new YourMsgStruct());
    /// fsm.stop();
    /// 
    /// </summary>
    public class FSM<T, M> : BaseFSM where T : class where M : struct
    {
        public override bool IsRunning { get; protected set; }

        private T _owner;
        private BaseState<T, M> _curState;//current state
        private BaseState<T, M> _lastSate;//previous state
        private BaseState<T, M> _globalState;//logic for all state


        public FSM(T owner, BaseState<T, M> state, BaseState<T, M> globalState)
        {
            IsRunning = false;
            _owner = owner;
            _lastSate = state;
            _curState = state;
            _globalState = globalState;
        }


        public override void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                _curState.Enter(_owner, null);
            }
        }


        public override void Pause()
        {
            IsRunning = false;
        }


        public override void Resume()
        {
            IsRunning = true;
        }


        public override void Stop()
        {
            if (IsRunning)
            {
                _curState.Exit(_owner);
                IsRunning = false;
                DisposeEvent?.Invoke(this);
            }
        }


        public override void Update()
        {
            if (IsRunning)
            {
                if (_curState != null) { _curState.Update(_owner); }
                if (_globalState != null) { _globalState.Update(_owner); }
            }
        }


        /// <summary>
        /// dispatch a message to fsm and get a return msg.
        /// </summary>
        public object OnMessage(M msg, bool retFromGlobal = false)
        {
            object msgRet = null;
            object msgRetGlobal = null;
            if (IsRunning)
            {
                if (_curState != null) { msgRet = _curState.OnMessage(_owner, msg); }
                if (_globalState != null) { msgRetGlobal = _globalState.OnMessage(_owner, msg); }
            }
            return retFromGlobal ? msgRetGlobal : msgRet;
        }


        /// <summary>
        /// change state immediately with some data
        /// </summary>
        public void ChangeState(BaseState<T, M> newState, object param = null)
        {
            if (!IsRunning)
            {
                ZLog.error("Cannot change state, FSM is not runnning");
                return;
            }

            if (newState == null)
            {
                ZLog.error(_owner.ToString(), "cannot change state to null");
                return;
            }
            if (_lastSate == null || _curState == null)
            {
                ZLog.error(_owner.ToString(), "Fatal error: _lastSate || _curState = null, newState=", newState.ToString());
                return;
            }

            if (newState.GetType().Equals(_curState.GetType()))
            {
                ZLog.warn(_owner.ToString(), "cannot change to the same state:", _curState.ToString());
                return;
            }

            if (_owner == null)
            {
                ZLog.error("_owner = null");
                return;
            }

            _lastSate = _curState;
            _curState = newState;

            ZLog.verbose(_owner.ToString() + ": " + _lastSate.ToString() + " -> " + _curState.ToString());

            _lastSate.Exit(_owner);
            _curState.Enter(_owner, param);
        }


        /// <summary>
        /// go back to previous state
        /// </summary>
        public void RevertState()
        {
            if (IsRunning)
            {
                //TODO，support revert state that requires param
                ChangeState(_lastSate);
            }
        }


        public bool IsInState(Type type)
        {
            return _curState.GetType().Equals(type);
        }


        public string GetStateName()
        {
            return _curState.GetType().Name;
        }

    }
}