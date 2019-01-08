using System.Collections.Generic;
namespace ZTools.Event
{
    public delegate bool EventHandler(CommonEvent eventObj);

    /// <summary>
    /// Use createInstance/destroyInstance rather than new()
    /// 需要外部驱动 Update
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

        #region 单例
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
            //传入 EventIDComparer， 去除Enum作为Key的Boxing、UnBoxing，消除GC
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
        /// 添加监听
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
        /// 移除监听
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
        /// 发送消息，下一帧触发响应
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

        ///// <summary>
        ///// 延迟发送消息
        ///// </summary>
        //public void fireDelayedEvent()
        //{
        //    //todo
        //}

        /// <summary>
        /// 游戏主逻辑驱动
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
            //用 try catch finnaly 保证 缓存消息可以被清空。
            //否则可能出现因错误中断导致某个消息反复触发，增加了调试难度
            try
            {
                for (int i = 0; i < _cachedEvents.Count; i++)
                {
                    CommonEvent param = _cachedEvents[i];
                    var eventID = (EventID)_cachedEvents[i].eventID;

                    if (_allHandlers.ContainsKey(eventID))
                    {
                        for (int j = 0; j < _allHandlers[eventID].Count; j++) //遍历所有监听器
                        {
                            EventHandlerWithID selectedHandlers = _allHandlers[eventID][j];

                            if (param.receiverID == ReceiverIDAllocator.GLOBALID)//广播消息
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