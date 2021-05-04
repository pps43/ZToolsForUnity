using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZTools.DebugUtil;
using ZTools.Game.Singleton;

namespace ZTools.FSM
{
    /// <summary>
    /// create and drive FSM
    /// </summary>
    public class FSMManager : USingleton<FSMManager>
    {
        private List<IFSM> allFSMs = null;

        public FSM<T, M> createFSM<T, M>(T owner, BaseState<T, M> state, BaseState<T, M> globalState) where T : class where M:struct 
        {
            var newFSM = new FSM<T, M>(owner, state, globalState);

            if (this.allFSMs == null)
            {
                this.allFSMs = new List<IFSM>();
            }

            this.allFSMs.Add(newFSM);

            newFSM.OnSelfDispose += onFSMStop;

            return newFSM;
        }

        private void onFSMStop(IFSM fsm)
        {
            int idx = -1;

            if (this.allFSMs != null && fsm != null)
            {
                idx = this.allFSMs.IndexOf(fsm);
                if (idx >= 0)
                {
                    fsm.OnSelfDispose -= onFSMStop;

                    if (!fsm.IsRunning)
                    {
                        this.allFSMs.RemoveAt(idx);
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
            if (this.allFSMs == null)
            {
                return;
            }

            foreach (var fsm in this.allFSMs)
            {
                if(fsm.IsRunning)
                {
                    fsm.Update();
                }
            }
        }
    }
}