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
        public abstract bool isRunning { get; }
        public abstract void start();
        public abstract void pause();
        public abstract void resume();
        public abstract void stop();//dispose
        public abstract void update();

        protected Action<BaseFSM> disposeEvent;
    }


    /// <summary>
    /// FSM (Finite state machine)
    /// 
    /// Provide two type of msg: P, Q. This is commonly defined as local msgtype as P,
    /// common msgtype as Q. This is to reduce global msgType contamination.
    /// 
    /// Change state is done immediately rather than next frame to keep simplicity and robustness.
    /// 
    /// Notice that one state may not call update() if time interval between enter() and exit() is less than one frame interval.
    /// 
    /// All onMessage function return an object.
    /// This is useful when you want to get some data immediately.
    /// 
    /// -----Example code----
    /// fsm = FSMFactory.createFSM(owner, new IdleState(), new GlobalState());
    /// fsm.start();
    /// fsm.onMessage(new TypePEvent());
    /// fsm.onMessage(new TypeQEvent());
    /// fsm.stop();
    /// 
    /// </summary>
    public class FSM<T, P, Q> : BaseFSM where T : class
    {
        private T _owner;
        private BaseState<T, P, Q> _curState;//current state
        private BaseState<T, P, Q> _lastSate;//previous state
        private BaseState<T, P, Q> _globalState;//logic for all state

        public override bool isRunning => _isRunning;
        private bool _isRunning;


        public FSM(T owner, BaseState<T, P, Q> state, BaseState<T, P, Q> globalState)
        {
            _isRunning = false;
            _owner = owner;
            _lastSate = state;
            _curState = state;
            _globalState = globalState;
        }

        public override void start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _curState.Enter(_owner, null);
            }
        }

        public override void pause()
        {
            _isRunning = false;
        }

        public override void resume()
        {
            _isRunning = true;
        }

        public override void stop()
        {
            if (_isRunning)
            {
                _curState.Exit(_owner);
                _isRunning = false;
                disposeEvent?.Invoke(this);
            }
        }

        public override void update()
        {
            if (_isRunning)
            {
                if (_curState != null) { _curState.Update(_owner); }
                if (_globalState != null) { _globalState.Update(_owner); }
            }
        }

        /// <summary>
        /// dispatch a message of type P to fsm and get a return msg.
        /// </summary>
        /// <param name="innerMsg"></param>
        /// <param name="retFromGlobal"></param>
        /// <returns></returns>
        public object onMessage(P innerMsg, bool retFromGlobal = false)
        {
            object msgRet = null;
            object msgRetGlobal = null;
            if (_isRunning)
            {
                if (_curState != null) { msgRet = _curState.OnMessage(_owner, innerMsg); }
                if (_globalState != null) { msgRetGlobal = _globalState.OnMessage(_owner, innerMsg); }
            }
            return retFromGlobal ? msgRetGlobal : msgRet;
        }


        /// <summary>
        /// dispatch a message of type Q to fsm and get a return msg.
        /// </summary>
        /// <param name="outerMsg"></param>
        /// <param name="retFromGlobal"></param>
        /// <returns></returns>
        public object onMessage(Q outerMsg, bool retFromGlobal = false)
        {
            object msgRet = null;
            object msgRetGlobal = null;
            if (_isRunning)
            {
                if (_curState != null) { msgRet = _curState.OnMessage(_owner, outerMsg); }
                if (_globalState != null) { msgRetGlobal = _globalState.OnMessage(_owner, outerMsg); }
            }
            return retFromGlobal ? msgRetGlobal : msgRet;
        }

        private void changeState(BaseState<T, P, Q> newState, object param = null)
        {
            if (!isRunning)
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

        public void revertState()
        {
            if (_isRunning)
            {
                //TODO，support revert state that requires param
                changeState(_lastSate);
            }
        }

        public bool isInState(Type type)
        {
            return _curState.GetType().Equals(type);
        }

        public string getStateName()
        {
            return _curState.GetType().Name;
        }

    }
}