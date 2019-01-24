using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZTools.Game
{
    /// <summary>
    /// 游戏对象池泛型类（用于 Monobehavior），
    /// 
    /// 之所以用Dictionary是因为这样可以同类但不同子类的对象可以用一个对象池管理，比如不同种类的敌人
    /// </summary>
    public class GameObjectPool<T, ENUM> where T : MonoBehaviour
    {
        public GameObjectPool(Transform root)
        {
            poolRoot = root;
            _pool = new Dictionary<ENUM, List<T>>();
        }

        public Transform poolRoot { get; private set; }
        private Dictionary<ENUM, List<T>> _pool;

        public T getObject(ENUM type, bool needSetActive = true)
        {
            T retObj = null;

            if (_pool.ContainsKey(type) && _pool[type] != null && _pool[type].Count > 0)
            {
                retObj = _pool[type][0];
                if (needSetActive)
                {
                    retObj.gameObject.SetActive(true);
                }

                _pool[type].RemoveAt(0);
            }

            return retObj;
        }

        public bool returnObject(ENUM type, T obj)
        {
            if (obj == null) { return false; }

            obj.transform.SetParent(poolRoot, false);
            obj.transform.localPosition = Vector3.zero;
            obj.gameObject.SetActive(false);

            if (!_pool.ContainsKey(type))
            {
                _pool.Add(type, new List<T> { obj });
            }
            else
            {
                _pool[type].Add(obj);
            }



            return true;
        }

        public void clear()
        {
            _pool.Clear();
        }
    }
}