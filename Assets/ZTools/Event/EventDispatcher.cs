using System.Collections.Generic;
namespace ZTools.Event
{
    public delegate bool EventHandler(CommonEvent eventObj);

    /// <summary>
    /// Use createInstance/destroyInstance rather than new()
    /// need call update from outside. 
    /// </summary>
    public class EventDispatcher
    {
        public class EventHandlerWithID
        {
            public long handlerID;
            public EventHandler handler;
            public EventHandlerWithID(long ID, EventHandler hdler)
            {
                handlerID = ID;
                handler = hdler;
            }
        }

        public bool isLocked;
        List<CommonEvent> _cachedEvents;
        Dictionary<EventID, List<EventHandlerWithID>> _allHandlers;

        #region singleton
        private static EventDispatcher _instance;
        public static EventDispatcher instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = createInstance();
                }
                return _instance;
            }
        }

        private EventDispatcher()
        {
            isLocked = false;
            _cachedEvents = new List<CommonEvent>();
            //EventIDComparer is used for eliminating GC caused by Enum as Key
            _allHandlers = new Dictionary<EventID, List<EventHandlerWithID>>(new EventIDComparer());
        }
        public static EventDispatcher createInstance()
        {
            return new EventDispatcher();
        }

        public static void destroyInstance()
        {
            _instance = null;
        }
        #endregion


        /// <summary>
        /// register, must be paired with unregister
        /// </summary>
        public void addListener(EventID eventID, long receiverID, EventHandler handler)
        {
            if (_allHandlers.ContainsKey(eventID))
            {
                _allHandlers[eventID].Add(new EventHandlerWithID(receiverID, handler));
            }
            else
            {
                _allHandlers[eventID] = new List<EventHandlerWithID> {
                    new EventHandlerWithID(receiverID,handler)
                };
            }
        }

        /// <summary>
        /// unregister
        /// </summary>
        public void removeListener(EventID eventID, long receiverID, EventHandler handler)
        {
            if (_allHandlers.ContainsKey(eventID))
            {
                List<EventHandlerWithID> handlers = _allHandlers[eventID];
                for (int i = 0; i < handlers.Count; i++)
                {
                    if (receiverID == handlers[i].handlerID && handler == handlers[i].handler)
                    {
                        _allHandlers[eventID].RemoveAt(i);

                    }
                }
            }
        }

        /// <summary>
        /// will activate handler in next frame
        /// </summary>
        public void fireEvent(EventID eventID, long senderID, long receiverID, object data)
        {
            if (senderID == -1 || receiverID == -1)
            {
                return;
            }
            var eventObj = new CommonEvent(eventID, senderID, receiverID, data);
            _cachedEvents.Add(eventObj);
        }


        /// <summary>
        /// drive by game logic
        /// </summary>
        public void update()
        {
            if (!isLocked)
            {
                dispatchCachedEvent();
            }
        }

        private void dispatchCachedEvent()
        {
            //use try-catch-finnaly to ensure cached event can be cleared, regardless of exception and error
            try
            {
                for (int i = 0; i < _cachedEvents.Count; i++)
                {
                    CommonEvent param = _cachedEvents[i];
                    var eventID = (EventID)_cachedEvents[i].eventID;

                    if (_allHandlers.ContainsKey(eventID))
                    {
                        for (int j = 0; j < _allHandlers[eventID].Count; j++) //iterate all handler
                        {
                            EventHandlerWithID selectedHandlers = _allHandlers[eventID][j];

                            if (param.receiverID == ReceiverIDAllocator.GLOBALID)//broadcast
                            {
                                if (selectedHandlers.handler(param))
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (selectedHandlers.handlerID == param.receiverID)
                                {
                                    selectedHandlers.handler(param);
                                    break;//break off
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {

                throw;
            }
            finally
            {
                _cachedEvents.Clear();

            }
        }
    }
    
}