using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZTools.Game
{
    /// <summary>
    /// GameObjectPool for Monobehavior.
    /// 
    /// Use 'int' as subType ID.
    /// This is useful when put different types of Enemy in same pool
    /// </summary>
    public class GameObjectPool<T> where T : MonoBehaviour
    {
        public GameObjectPool(Transform root)
        {
            poolRoot = root;
            _pool = new Dictionary<int, List<T>>();
        }

        public Transform poolRoot { get; private set; }
        private Dictionary<int, List<T>> _pool;

        public T getObject(int type, bool needSetActive = true)
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

        public bool returnObject(int type, T obj)
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