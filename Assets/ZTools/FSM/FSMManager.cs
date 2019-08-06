using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZTools.DebugUtil;

namespace ZTools.FSM
{
    /// <summary>
    /// create and drive FSM
    /// </summary>
    public class FSMManager : MonoBehaviour
    {
        private List<BaseFSM> _allFSMList = null;

        private static FSMManager _ins;
        public static FSMManager instance
        {
            get
            {
                if (_ins == null)
                {
                    GameObject obj = GameObject.Find("FSMManager");
                    if (obj == null)
                    {
                        obj = new GameObject("FSMManager");
                    }
                    _ins = obj.AddComponent<FSMManager>();
                    DontDestroyOnLoad(obj);
                }
                return _ins;
            }
        }


        public FSM<T, M> createFSM<T, M>(T owner, BaseState<T, M> state, BaseState<T, M> globalState) where T : class where M:struct 
        {
            var newFSM = new FSM<T, M>(owner, state, globalState);

            if (_allFSMList == null)
            {
                _allFSMList = new List<BaseFSM>();
            }
            _allFSMList.Add(newFSM);

            newFSM.DisposeEvent += onFSMStop;

            return newFSM;
        }

        private void onFSMStop(BaseFSM fsm)
        {
            int idx = -1;
            if (_allFSMList != null && fsm != null)
            {
                idx = _allFSMList.IndexOf(fsm);
                if (idx >= 0)
                {
                    fsm.DisposeEvent -= onFSMStop;

                    if (!fsm.IsRunning)
                    {
                        _allFSMList.RemoveAt(idx);
                    }
                    else
                    {
                        ZLog.error(fsm.ToString(), "should stop before delete from list");
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (_allFSMList != null)
            {
                for (int i = 0; i < _allFSMList.Count; i++)
                {
                    if (_allFSMList[i].IsRunning)
                    {
                        _allFSMList[i].Update();
                    }
                }
            }
        }

    }
}