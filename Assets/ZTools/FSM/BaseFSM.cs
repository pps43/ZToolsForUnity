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
        public abstract bool isRunning { get;}
        public abstract void start();
        public abstract void pause();
        public abstract void resume();
        public abstract void stop();//dispose
        public abstract void update();

        public Action<BaseFSM> disposeEvent;
    }


    /// <summary>
    /// FSM (Finite state machine)
    /// 
    /// <para name="Feature">
    /// <list type="bullet">
    /// <item>
    /// Provide two type of msg: P, Q. This is commonly defined as local msgtype as P,
    /// common msgtype as Q. This is to reduce global msgType contamination.
    /// </item>
    /// <item>
    /// Change state is done in next frame to ensure certainty of state. 
    /// If not, you can get different state in one frame, and this may cause confusion.
    /// </item>
    /// <item>
    /// All onMessage function return a bool value.
    /// This is used to be compatible with ZTool.Event module, 
    /// whose event handler always has a bool ret-value to identify whether to "eat" this event.
    /// </item>
    /// </list>
    /// </para>
    /// 
    /// <para name="How to use">
    /// <list type="bullet">
    /// <item> fsm = FSMFactory.createFSM(owner, new IdleState(), new GlobalState());</item>
    /// <item> fsm.start();</item>
    /// <item> fsm.onMessage(new TypePEvent(), out _);</item>
    /// <item> fsm.onMessage(new TypeQEvent(), out _);</item>
    /// <item> fsm.stop();</item>
    /// </list>
    /// </para>
    /// 
    /// </summary>
    public class FSM<T, P, Q> : BaseFSM where T : class
    {
        private T _owner;
        private BaseState<T, P, Q> _curState;//current state
        private BaseState<T, P, Q> _lastSate;//previous state
        private BaseState<T, P, Q> _globalState;//logic for all state

        private bool _needChangeState;
        public override bool isRunning => _isRunning;
        private bool _isRunning;

        public class CachedStateToChange
        {
            public BaseState<T, P, Q> state;
            public object stateData;
            public CachedStateToChange(BaseState<T, P, Q> stateToCache, object data)
            {
                state = stateToCache;
                stateData = data;
            }
        }

        //change state requests in same frame will be cached. Then they will be executed in next frame.
        List<CachedStateToChange> _cachedStates;


        public FSM(T owner, BaseState<T, P, Q> state, BaseState<T, P, Q> globalState)
        {
            _isRunning = false;
            _cachedStates = new List<CachedStateToChange>();
            _owner = owner;
            _lastSate = state;
            _curState = state;
            _globalState = globalState;
            _needChangeState = false;
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

        //will call at the end of current frame by FSMManager
        public override void update()
        {
            if (_isRunning)
            {
                if (_needChangeState)
                {
                    _needChangeState = false;
                    for (int i = 0; i < _cachedStates.Count; i++)
                    {
                        realChangeState(_cachedStates[i].state, _cachedStates[i].stateData);
                    }
                    _cachedStates.Clear();
                }
                else
                {
                    if (_curState != null) { _curState.Update(_owner); }
                    if (_globalState != null) { _globalState.Update(_owner); }
                }
            }
        }

        /// <summary>
        /// This function provide a extra msgRet from _curState.
        /// If you don't care about it, use: onMessage(msg, out _);
        /// </summary>
        /// <param name="innerMsg"></param>
        /// <param name="msgRet"></param>
        /// <returns></returns>
        public bool onMessage(P innerMsg, out object msgRet)
        {
            msgRet = null;
            if (_isRunning)
            {
                if (_curState != null) { msgRet = _curState.OnMessage(_owner, innerMsg); }
                if (_globalState != null) { _globalState.OnMessage(_owner, innerMsg); }
            }
            return false;
        }

        /// <summary>
        /// This function provide a extra msgRetGlobal from _globalState.
        /// </summary>
        public bool onMessage(P innerMsg, out object msgRet, out object msgRetGlobal)
        {
            msgRet = null;
            msgRetGlobal = null;
            if (_isRunning)
            {
                if (_curState != null) { msgRet = _curState.OnMessage(_owner, innerMsg); }
                if (_globalState != null) { msgRetGlobal = _globalState.OnMessage(_owner, innerMsg); }
            }
            return false;
        }

        /// This is for type-Q message
        public bool onMessage(Q outerMsg, out object msgRet)
        {
            msgRet = null;
            if (_isRunning)
            {
                if (_curState != null) { msgRet = _curState.OnMessage(_owner, outerMsg); }
                if (_globalState != null) { _globalState.OnMessage(_owner, outerMsg); }
            }
            return false;
        }

        /// This is for type-Q message
        public bool onMessage(Q outerMsg, out object msgRet, out object msgRetGlobal)
        {
            msgRet = null;
            msgRetGlobal = null;
            if (_isRunning)
            {
                if (_curState != null) { msgRet = _curState.OnMessage(_owner, outerMsg); }
                if (_globalState != null) { msgRetGlobal = _globalState.OnMessage(_owner, outerMsg); }
            }
            return false;
        }


        public void changeState(BaseState<T, P, Q> newState, object param = null)
        {
            //ZLog.log(_owner.ToString() + " try change State to:" + newState.ToString());
            if (_isRunning)
            {
                _needChangeState = true;
                _cachedStates.Add(new CachedStateToChange(newState, param));
            }
        }

        private void realChangeState(BaseState<T, P, Q> newState, object param = null)
        {
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