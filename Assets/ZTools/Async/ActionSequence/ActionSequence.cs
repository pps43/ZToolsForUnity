using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZTools.ActionSequence
{
    /// <summary>
    /// Chains some actions sequentially. 
    /// 
    /// How to use:
    /// ActionSequence seq = ActionSequenceManager.getActionSequence();
    /// seq.Then(doSth).Then(doCoroutine());
    /// 
    /// Support action type:
    /// normal function、IEnumerator、YieldInstruction、CustomYieldInstruction.
    /// Support freature:
    /// callback after one action finish.
    /// </summary>
    public class ActionSequence : MonoBehaviour
    {
        class ActionItem
        {
            public string name;
            public IEnumerator coroutine;
            public YieldInstruction yieldInstruction;
            public CustomYieldInstruction customYield;
            public Action func;
            public Action<object> funcWithParam;
            public Action<object, Action> funcWithParamAndCallBack;

            public object param;
            public float timeOut;  //not used now. in case of blocking

            public ActionItem()
            {
                coroutine = null;
                func = null;
                funcWithParamAndCallBack = null;
                funcWithParam = null;

                param = null;
                timeOut = -1f;
            }
        }

        private Queue<ActionItem> _actionSequence = new Queue<ActionItem>();

        #region public func

        public bool isRunning()
        {
            return _actionSequence.Count > 0;
        }

        public void Stop()
        {
            StopAllCoroutines();
            _actionSequence.Clear();
        }

        public ActionSequence Then(IEnumerator enumerator)
        {
            if (enumerator != null)
            {
                ActionItem item = new ActionItem
                {
                    name = enumerator.ToString(),
                    coroutine = enumerator
                };
                enqueue(item);
            }
            return this;
        }

        public ActionSequence Then(YieldInstruction yieldInstruction)
        {
            if(yieldInstruction != null)
            {
                ActionItem item = new ActionItem
                {
                    name = yieldInstruction.ToString(),
                    yieldInstruction = yieldInstruction,
                };
                enqueue(item);
            }

            return this;
        }

        public ActionSequence Then(CustomYieldInstruction customYield)
        {
            if(customYield != null)
            {
                ActionItem item = new ActionItem
                {
                    name = customYield.ToString(),
                    customYield = customYield,
                };
                enqueue(item);
            }
            return this;
        }

        public ActionSequence Then(Action func)
        {
            if (func != null)
            {
                ActionItem item = new ActionItem
                {
                    name = func.ToString(),
                    func = func,
                };
                enqueue(item);
            }
            return this;
        }

        public ActionSequence Then(Action<object> func, object param1)
        {
            if (func != null)
            {
                ActionItem item = new ActionItem
                {
                    name = func.ToString() + "with param",
                    funcWithParam = func,
                    param = param1,
                };
                enqueue(item);
            }
            return this;
        }

        public ActionSequence Then(Action<object,Action> func, object param, Action myCallBack)
        {
            if (func != null)
            {
                ActionItem item = new ActionItem
                {
                    name = func.ToString() + "with param & callback",
                    funcWithParamAndCallBack = func,
                    func = myCallBack,
                    param = param,
                };
                enqueue(item);
            }
            return this;
        }

        #endregion
        #region private func

        private void enqueue(ActionItem item)
        {
            if (_actionSequence.Count > 0)
            {
                _actionSequence.Enqueue(item);
            }
            else
            {
                _actionSequence.Enqueue(item);
                doNextAction();
            }
        }

        private void doNextAction()
        {
            if (_actionSequence.Count > 0)
            {
                ActionItem item = _actionSequence.Peek();

                if (item.funcWithParamAndCallBack != null)
                {
                    item.funcWithParamAndCallBack(item.param, item.func);
                    finishOne();
                }
                else if (item.funcWithParam != null)
                {
                    item.funcWithParam(item.param);
                    finishOne();
                }
                else if (item.func != null)
                {
                    item.func();
                    finishOne();
                }
                else if (item.coroutine != null)
                {
                    StartCoroutine(doItemCoroutine(item));
                }
                else if(item.yieldInstruction != null)
                {
                    StartCoroutine(doItemYieldInstruction(item));
                }
                else if(item.customYield != null)
                {
                    StartCoroutine(doItemCustomYieldInstruction(item));
                }
            }
        }

        private void finishOne()
        {
            if (_actionSequence.Count > 0)
            {
                _actionSequence.Dequeue();
                //ZLog.log("finish", _actionSequence.Peek().name);
                doNextAction();
            }
        }

        private IEnumerator doItemCoroutine(ActionItem item)
        {
            yield return item.coroutine;
            finishOne();
        }

        private IEnumerator doItemYieldInstruction(ActionItem item)
        {
            yield return item.yieldInstruction;
            finishOne();
        }

        private IEnumerator doItemCustomYieldInstruction(ActionItem item)
        {
            yield return item.customYield;
            finishOne();
        }
        #endregion


    }
}