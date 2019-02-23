using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZTools.Game
{
    /// <summary>
    /// Manage a group of [T]: Create/Destroy, Init/UnInit,
    /// Access by GamePlay singleton. e.g. GamePlay.Instance.xxxManager
    /// 
    /// [ObjectTypeEnum] means differenct prefabs that can be create.
    /// e.g. EnemyManager can Generate different types of enemies identified by EnemyType Enum.
    /// </summary>
    public class BaseObjectManager<T, ENUM> where T : BaseObject where ENUM : struct, IConvertible
    {
        protected Dictionary<ulong, T> _allObject = null;
        protected BaseObjectFactory<T> _objectFactory = null;
        public int ActorNum { get { return _allObject?.Count ?? 0; } }


        /// <summary>
        /// if not factory assigned, this manager is unable to generate new actor.
        /// </summary>
        /// <param name="factory"></param>
        public virtual void Init(BaseObjectFactory<T> factory = null)
        {
            _objectFactory = factory;
            _allObject = new Dictionary<ulong, T>();
        }

        public virtual void UnInit()
        {
            _objectFactory = null;
            RemoveALl();
        }

        public bool Has(ulong guid)
        {
            return _allObject?.ContainsKey(guid) ?? false;
        }


        public virtual T Generate(ENUM objType, Transform parent = null, Vector3 pos = default, bool isLocalPos = true)
        {
            if (_objectFactory != null)
            {
                T newObj = _objectFactory.GetObject(objType, parent, pos, isLocalPos);
                if (newObj != null)
                {
                    Add(newObj);
                    return newObj;
                }
            }
            return null;
        }

        public void Add(T obj)
        {
            if (!Has(obj.GUID))
            {
                _allObject.Add(obj.GUID, obj);
                obj.DisposeEvent += Remove;  //will trigger when actor Dispose()
                obj.Init();
            }
        }

        public void Remove(ulong guid)
        {
            if (Has(guid))
            {
                T obj = _allObject[guid];
                obj.DisposeEvent -= Remove;
                _allObject.Remove(guid);

                if (obj.HasInit) // if actor dispose itself, need not call UnInit()
                {
                    obj.UnInit();
                }

                if (_objectFactory != null)
                {
                    _objectFactory.ReturnObject(obj);
                }
                else
                {
                    GameObject.Destroy(obj.gameObject);
                }
            }
        }

        private void RemoveALl()
        {
            //TODO
        }

    }
}
