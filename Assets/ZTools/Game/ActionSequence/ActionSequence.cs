using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZTools.Debug;

namespace ZTools.Game
{
    /// <summary>
    /// Chains some actions sequentially ( with less coding than coroutine).
    /// 
    /// - How to use:
    /// 
    /// ActionSequence seq = ActionSequenceManager.create();
    /// seq.Then(()=> dosth)).Then(doCoroutine()).Then(new WaitForSeconds(1f));
    /// seq.Run();
    /// 
    /// - Notice:
    /// 
    /// You'd better **not cache** seq as class member or sth in order to reuse,
    /// because seq's life is controlled by its manager, not you.
    /// 
    /// - Support action type:
    /// 
    /// Normal function、IEnumerator、YieldInstruction、CustomYieldInstruction.
    /// 
    /// - Support feature:
    /// 
    /// Callback after one action finish.
    /// Nested seq.
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

        public bool IsFinshed { get; set; }//lifecycle must be controlled by ActionSequenceManager.

        public event Action<ActionSequence> OnFinished;


        #region public func

        public void Run()
        {
            DoNextAction();
        }


        public void Stop()
        {
            StopAllCoroutines();
            _actionSequence.Clear();
            OnFinished?.Invoke(this);
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
                    name = $"{func.ToString()} with param",
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
                    name = $"{func.ToString()} with param & callback",
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
            IsFinshed = false;

            ZLog.log($"[{GetInstanceID().ToString()}] dequeue:{item.ToString()}");

            _actionSequence.Enqueue(item);
        }


        private void DoNextAction()
        {
            if (_actionSequence.Count > 0)
            {
                ActionItem item = _actionSequence.Peek();
                _actionSequence.Dequeue();

                ZLog.log($"[{GetInstanceID().ToString()}] dequeue:{item.ToString()}");

                // exceptions may raise, but ActionSequence should keep operating.
                try
                {
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
                        StartCoroutine(DoItemCoroutine(item));
                    }
                    else if (item.yieldInstruction != null)
                    {
                        StartCoroutine(DoItemYieldInstruction(item));
                    }
                    else if (item.customYield != null)
                    {
                        StartCoroutine(DoItemCustomYieldInstruction(item));
                    }
                }
                catch(Exception e)
                {
                    ZLog.warn(e.ToString());
                    OnFinished?.Invoke(this);
                }
            }
            else
            {
                OnFinished?.Invoke(this);
            }
        }


        private void FinishOne()
        {
            //ZLog.log("finishOne, rest action =", _actionSequence.Count.ToString());
            ZLog.log($"[{GetInstanceID().ToString()}] finishOne, rest action = { _actionSequence.Count.ToString()}");

            DoNextAction();
        }


        private IEnumerator DoItemCoroutine(ActionItem item)
        {
            yield return item.coroutine;
            FinishOne();
        }


        private IEnumerator DoItemYieldInstruction(ActionItem item)
        {
            yield return item.yieldInstruction;
            FinishOne();
        }


        private IEnumerator DoItemCustomYieldInstruction(ActionItem item)
        {
            yield return item.customYield;
            FinishOne();
        }
        #endregion


    }
}