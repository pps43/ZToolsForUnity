using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZTools.DebugUtil;

namespace ZTools.Game
{
    /// <summary>
    /// abstract Factory
    /// 
    /// Use 'int' as subType ID.
    /// This is compatible with GameObjectPool.
    /// </summary>
    // child class example：
    // public class EnemyFactory : BaseObjectFactory<BaseEnemy>
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
    public abstract class BaseObjectFactory<TEnum, T> : MonoBehaviour where T : BaseObject where TEnum : struct, IConvertible
    {
        private GameObjectPool<T> _pool;

        [Header("Prefab's type is described in itself")]
        [SerializeField] protected T[] _prefabList;
        protected Dictionary<int, T> _prefabDic;

        private void Awake()
        {
            _pool = new GameObjectPool<T>(transform);
            _prefabDic = new Dictionary<int, T>();
            ListToDic();
        }

        private void OnDestroy()
        {
            _pool.clear();
            _prefabDic.Clear();
        }


        public T GetObject(TEnum objType, Transform parent, Vector3 pos, bool isLocalPos)
        {
            int typeID = Convert.ToInt32(objType);// TODO : optimize GC here

            T newObj = _pool.getObject(typeID);

            if (newObj == null && _prefabDic.ContainsKey(typeID))
            {
                newObj = GameObject.Instantiate<T>(_prefabDic[typeID]);
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
                ZLog.error(gameObject.name, "generate type failed:", typeID.ToString());
            }

            return newObj;
        }


        public bool ReturnObject(T obj)
        {
            return _pool.returnObject(obj.TypeID, obj);
        }

        //这里通过子类实现来实现，避开在基类中泛型难以转化为具体类型的限制
        protected abstract void ListToDic();

    }
}