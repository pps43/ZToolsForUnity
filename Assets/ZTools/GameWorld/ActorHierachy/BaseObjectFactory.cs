using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZTools.DebugUtil;

namespace ZTools.Game
{
    /// <summary>
    /// abstract Factory
    /// </summary>
    // child class example：
    // public class EnemyFactory : BaseObjectFactory<BaseEnemy, EnemyType>
    // {
    //     protected override void ListToDic()
    //     {
    //         if (_prefabList != null)
    //         {
    //             for (int i = 0; i < _prefabList.Length; i++)
    //             {
    //                  _prefabDic.Add(_prefabList[i].type, _prefabList[i]);
    //             }
    //         }
    //     }
    // }
    public abstract class BaseObjectFactory<T, ENUM> : MonoBehaviour where T : BaseObject
    {
        private GameObjectPool<T, ENUM> _pool;

        [Header("Prefab's type is described in itself")]
        [SerializeField] protected T[] _prefabList;
        protected Dictionary<ENUM, T> _prefabDic;

        private void Awake()
        {
            _pool = new GameObjectPool<T, ENUM>(transform);
            _prefabDic = new Dictionary<ENUM, T>();
            ListToDic();
        }

        private void OnDestroy()
        {
            _pool.clear();
            _prefabDic.Clear();
        }


        public T GetObject(ENUM type, Transform parent, Vector3 pos, bool isLocalPos)
        {
            T newObj = _pool.getObject(type);

            if (newObj == null && _prefabDic.ContainsKey(type))
            {
                newObj = GameObject.Instantiate<T>(_prefabDic[type]);
            }

            if (newObj != null)
            {
                newObj.transform.SetParent(parent, false);
                if (isLocalPos)
                {
                    newObj.transform.localPosition = pos;
                }
                else
                {
                    newObj.transform.position = pos;
                }
            }
            else
            {
                ZLog.error(gameObject.name, "generate type failed:", type.ToString());
            }

            return newObj;
        }


        public bool ReturnObject(ENUM type, T obj)
        {
            return _pool.returnObject(type, obj);
        }

        //这里通过子类实现来实现，避开在基类中泛型难以转化为具体类型的限制
        protected abstract void ListToDic();

    }
}