using System;
using UnityEngine;
using ZTools.DebugUtil;

namespace ZTools.Game
{
    /// <summary>
    /// Base class of a individual game object, 
    /// which has an Guid, and need to be managed (Init/UnInit, Create/Destroy) in your Game.
    /// e.g. Enemies, Items, TerrainBlocks, NPCs, etc
    /// 
    /// Do not use UnityEvents like Awake(), Start(), OnDestroy() in subclass,
    /// unless no BaseObjectManager is controlling it (if DisposeEvent == null),
    /// which means BaseObject should take care of itself.
    /// </summary>
    public abstract class BaseObject : MonoBehaviour
    {
        //public ulong Guid { get; private set; }
        public bool HasInit { get; private set; }

        public event Action<BaseObject> DisposeEvent;
        

        #region Init/UnInit

        
        public virtual void Init()
        {
            if (HasInit) { ZLog.error(gameObject.name, "cannot Init twice!"); }
            HasInit = true;

            // register event listener, init children, etc
        }

        
        public virtual void UnInit()
        {
            if (!HasInit) { ZLog.error(gameObject.name, "cannot UnInit twice!"); }
            HasInit = false;

            // do: unregister event listener, uninit children
        }

        #endregion


        #region Get/Set

        #endregion


        #region Event

        #endregion


        /// <summary>
        /// call this rather than destroy.
        /// </summary>
        protected void Dispose()
        {
            if (!HasInit) { ZLog.warn(gameObject.name, "has't Init. Dispose() makes no sense.");  return; }

            if (DisposeEvent != null)
            {
                if(DisposeEvent.GetInvocationList().Length > 1)
                {
                    ZLog.error(gameObject.name, "disposeEvent should only have one listener(Manager)");
                }

                DisposeEvent(this);
            }
            else
            {
                ZLog.warn(gameObject.name, "no manager attached, destroy itself");
                UnInit();
                Destroy(gameObject);
            }
        }



    }
}