using System.Collections.Generic;
using ZTools.DebugUtil;

namespace ZTools.Event
{
    /// <summary>
    /// Use this object to fire/receive msg.
    /// </summary>
    public class EventHelper
    {
        public long ID { get; private set; }
        private Dictionary<EventID, EventHandler> registeredListener;

        public EventHelper()
        {
            ID = ReceiverIDAllocator.getID();
            registeredListener = new Dictionary<EventID, EventHandler>();
        }

        ~EventHelper()
        {
            removeAllListener();
        }

        /// <summary>
        /// fire event to receiverID. default is broadcasting
        /// </summary>
        public void fireEvent(EventID eventID, object data = null, long receiverID = ReceiverIDAllocator.GLOBALID)
        {
            EventDispatcher.Instance.FireEvent(eventID, ID, receiverID, data);
        }


        public void addListener(EventID eventID, EventHandler handler)
        {
            if (registeredListener.ContainsKey(eventID))
            {
                ZLog.warn("Fail to addListener. one eventID to one handler");
            }
            else
            {
                registeredListener.Add(eventID, handler);
                EventDispatcher.Instance.AddListener(eventID, ID, handler);
            }
        }

        public void removeListener(EventID eventID, EventHandler handler)
        {
            if (registeredListener.ContainsKey(eventID))
            {
                registeredListener.Remove(eventID);
                EventDispatcher.Instance.RemoveListener(eventID, ID, handler);
            }
            else
            {
                ZLog.warn(eventID.ToString() + " does not register handler:" + handler.ToString());
            }
        }

        /// <summary>
        /// auto remove in case of forget
        /// </summary>
        private void removeAllListener()
        {
            foreach (var kvp in registeredListener)
            {
                removeListener(kvp.Key, kvp.Value);
            }
        }
    }
}