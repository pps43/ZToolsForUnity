using UnityEngine;
using ZTools.Debug;

namespace ZTools.Game
{
    /// <summary>
    /// Base class of some functions added to individual game object.
    /// It's driven by BaseObject.
    /// </summary>
    public abstract class BaseAbility : MonoBehaviour
    {
        public BaseObject owner;

        public bool HasInit { get; private set; }

        public virtual void Init(BaseObject ownerObject)
        {
            if (HasInit) { ZLog.error("cannot Init twice!"); }
            HasInit = true;
            owner = ownerObject;
        }


        public virtual void UnInit()
        {
            if (!HasInit) { ZLog.error("cannot UnInit twice!"); }
            HasInit = false;
            owner = null;
        }

    }
}