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


        public FSM<T, P, Q> createFSM<T, P, Q>(T owner, BaseState<T, P, Q> state, BaseState<T, P, Q> globalState) where T : class
        {
            var newFSM = new FSM<T, P, Q>(owner, state, globalState);

            if (_allFSMList == null)
            {
                _allFSMList = new List<BaseFSM>();
            }
            _allFSMList.Add(newFSM);

            newFSM.disposeEvent += onFSMStop;

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
                    if (!fsm.isRunning)
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
                    if (_allFSMList[i].isRunning)
                    {
                        _allFSMList[i].update();
                    }
                }
            }
        }

    }
}