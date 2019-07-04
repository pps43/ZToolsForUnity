using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZTools.DebugUtil;

namespace ZTools.ActionSequence
{
    /// <summary>
    /// Chains some actions sequentially. 
    /// 
    /// How to use:
    /// ActionSequence seq = ActionSequenceManager.create();
    /// seq.Then(()=> dosth)).Then(doCoroutine()).Then(new WaitForSeconds(1f));
    /// seq.Run();
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

            System.Text.StringBuilder _sb = new System.Text.StringBuilder();

            public ActionItem()
            {
                coroutine = null;
                func = null;
                funcWithParamAndCallBack = null;
                funcWithParam = null;

                param = null;
                timeOut = -1f;
            }


            public override string ToString()
            {
                _sb.Length = 0;

                if (func != null)
                {
                    string s = func.Method.Name;
                    if (s.StartsWith("<"))
                    {
                        _sb.Append("Anomymous Func:");
                    }
                    _sb.Append(s);
                }
                else if (yieldInstruction != null)
                {
                    var type = yieldInstruction.GetType();
                    if (type == typeof(WaitForSeconds))
                    {
                        var prop = typeof(WaitForSeconds).GetField("m_Seconds",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var va = prop.GetValue(yieldInstruction);
                        string s = va.ToString();

                        _sb.Append("WaitForSeconds: ");
                        _sb.Append(s);
                    }

                }

                return _sb.ToString();
            }
        }

        private Queue<ActionItem> _actionSequence = new Queue<ActionItem>();

        private bool _isFnished = false;//It should be regard as running just after creation

        #region public func

        public bool IsRunning()
        {
            return !_isFnished;
        }


        public void Run()
        {
            DoNextAction();
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
                Enqueue(item);
            }
            return this;
        }


        public ActionSequence Then(YieldInstruction yieldInstruction)
        {
            if (yieldInstruction != null)
            {
                ActionItem item = new ActionItem
                {
                    name = yieldInstruction.ToString(),
                    yieldInstruction = yieldInstruction,
                };
                Enqueue(item);
            }

            return this;
        }


        public ActionSequence Then(CustomYieldInstruction customYield)
        {
            if (customYield != null)
            {
                ActionItem item = new ActionItem
                {
                    name = customYield.ToString(),
                    customYield = customYield,
                };
                Enqueue(item);
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
                Enqueue(item);
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
                Enqueue(item);
            }
            return this;
        }


        public ActionSequence Then(Action<object, Action> func, object param, Action myCallBack)
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
                Enqueue(item);
            }
            return this;
        }

        #endregion


        #region private func

        private void Enqueue(ActionItem item)
        {
            _isFnished = false;

            ZLog.log("[", GetInstanceID().ToString(), "] enqueue:", item.ToString());

            _actionSequence.Enqueue(item);
        }


        private void DoNextAction()
        {
            if (_actionSequence.Count > 0)
            {
                ActionItem item = _actionSequence.Peek();
                _actionSequence.Dequeue();

                ZLog.log("[", GetInstanceID().ToString(), "] dequeue:", item.ToString());

                if (item.funcWithParamAndCallBack != null)
                {
                    item.funcWithParamAndCallBack(item.param, item.func);
                    FinishOne();
                }
                else if (item.funcWithParam != null)
                {
                    item.funcWithParam(item.param);
                    FinishOne();
                }
                else if (item.func != null)
                {
                    item.func();
                    FinishOne();
                }
                else if (item.coroutine != null)
                {
                    StartCoroutine(doItemCoroutine(item));
                }
                else if (item.yieldInstruction != null)
                {
                    StartCoroutine(doItemYieldInstruction(item));
                }
                else if (item.customYield != null)
                {
                    StartCoroutine(doItemCustomYieldInstruction(item));
                }
            }
            else
            {
                _isFnished = true;
            }
        }


        private void FinishOne()
        {
            //ZLog.log("finishOne, rest action =", _actionSequence.Count.ToString());

            DoNextAction();
        }


        private IEnumerator doItemCoroutine(ActionItem item)
        {
            yield return item.coroutine;
            FinishOne();
        }


        private IEnumerator doItemYieldInstruction(ActionItem item)
        {
            yield return item.yieldInstruction;
            FinishOne();
        }


        private IEnumerator doItemCustomYieldInstruction(ActionItem item)
        {
            yield return item.customYield;
            FinishOne();
        }
        #endregion


    }
}