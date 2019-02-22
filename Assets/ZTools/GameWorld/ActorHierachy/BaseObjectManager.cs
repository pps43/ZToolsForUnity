using System.Collections.Generic;
using UnityEngine;

namespace ZTools.Game
{
    //TODO use BaseObjectFactory instead
    public interface IBaseObjectFactory
    {
        BaseObject Generate(string prefabPath);
        bool Recycle(BaseObject objectToRecycle); //Better to use Object Pooling
    }

    /// <summary>
    /// Manage a group of BaseObject: Create/Destroy, Init/UnInit,
    /// Access by GamePlay singleton. e.g. GamePlay.Instance.xxxManager
    /// 
    /// <ObjectType> means differenct prefabs that can be create.
    /// e.g. EnemyManager can Generate different types of enemies identified by EnemyType Enum.
    /// </summary>
    public class BaseObjectManager<ObjectType>
    {
        protected List<BaseObject> _allObject = new List<BaseObject>(); //or use Dictionary
        protected IBaseObjectFactory _objectFactory = null;

        /// <summary>
        /// if not factory assigned, this manager is unable to generate new actor.
        /// </summary>
        /// <param name="factory"></param>
        public virtual void Init(IBaseObjectFactory factory = null)
        {
            _objectFactory = factory;
        }

        public virtual void UnInit()
        {
            _objectFactory = null;
        }


        public int ActorNum { get { return _allObject?.Count ?? 0; } }

        public bool Has(BaseObject obj)
        {
            return _allObject?.Contains(obj) ?? false;
        }


        public BaseObject Generate(ObjectType objType)
        {
            if (_objectFactory != null)
            {
                string prefabPath = "";
                //prefabPath = TypeToPath(objType);
                BaseObject newObj = _objectFactory.Generate(prefabPath);
                if (newObj)
                {
                    Add(newObj);
                    return newObj;
                }
            }
            return null;
        }

        public void Add(BaseObject obj)
        {
            if (!Has(obj))
            {
                _allObject.Add(obj);
                obj.DisposeEvent += Remove;  //will trigger when actor Dispose()
                obj.Init();
            }
        }

        public void Remove(BaseObject obj)
        {
            if (Has(obj))
            {
                _allObject.Remove(obj);
                obj.DisposeEvent -= Remove;

                if (obj.HasInit) // if actor dispose itself, need not call UnInit()
                {
                    obj.UnInit();
                }

                if (_objectFactory != null)
                {
                    _objectFactory.Recycle(obj);
                }
                else
                {
                    GameObject.Destroy(obj.gameObject);
                }
            }
        }

    }
}
