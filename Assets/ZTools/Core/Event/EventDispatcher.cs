using System.Collections.Generic;

namespace ZTools.Core
{
    public delegate void EventHandler<in TData>(TData data);

    public interface IEventDispatcher<TEventID, TData>
    {
        void AddListener(TEventID id, EventHandler<TData> handler);
        void RemoveListener(TEventID id, EventHandler<TData> handler);
        void Dispatch(TEventID id, TData data);
    }
    
    /// <summary>
    /// Dispatch event immediately. Can use it as T in <see cref = "ZTools.Core.Singleton{T}"/> to make it singleton.
    /// <para>
    /// (1) Event data type is <see cref="System.object"/>, but it may cause boxing when you pass a value type such as an int.
    /// Use <see cref="EventDispatcher{TEventID,TData}"/> to define your unified data type if you need.
    /// </para>
    /// <para>
    /// (2) Internally it use default IEqualityComparer for TEventID.
    /// If you use enum as TEventID in Unity5.6 or older, it will cause boxing.
    /// If you use custom struct as TEventID, it will cause boxing, too.
    /// Use <see cref="EventDispatcher{TEventID,TData,TComparer}"/> to define your own IEqualityComparer to eliminate boxing.
    /// </para>
    /// </summary>
    public class EventDispatcher<TEventID> : IEventDispatcher<TEventID, object>
    {
        private EventDispatcher<TEventID, object> _internalDispatcher;

        public EventDispatcher()
        {
            _internalDispatcher = new EventDispatcher<TEventID, object>();
        }

        public void AddListener(TEventID id, EventHandler<object> handler)
        {
            _internalDispatcher.AddListener(id, handler);
        }

        public void RemoveListener(TEventID id, EventHandler<object> handler)
        {
            _internalDispatcher.RemoveListener(id, handler);
        }
        
        public void Dispatch(TEventID id, object data)
        {
            _internalDispatcher.Dispatch(id,data);
        }
    }
    
    /// <summary>
    /// EventDispatcher with custom data type.
    /// Can use it as T in <see cref = "ZTools.Core.Singleton{T}"/> to make it singleton.
    /// </summary>
    public class EventDispatcher<TEventID, TData> : IEventDispatcher<TEventID, TData>
    {
        private EventDispatcher<TEventID, TData, FakeComparer> _internalDispatcher;

        public EventDispatcher()
        {
            _internalDispatcher = new EventDispatcher<TEventID, TData, FakeComparer>(null);
        }

        public void AddListener(TEventID id, EventHandler<TData> handler)
        {
            _internalDispatcher.AddListener(id, handler);
        }

        public void RemoveListener(TEventID id, EventHandler<TData> handler)
        {
            _internalDispatcher.RemoveListener(id, handler);
        }

        public void Dispatch(TEventID id, TData data)
        {
            _internalDispatcher.Dispatch(id, data);
        }

        private class FakeComparer : IEqualityComparer<TEventID>
        {
            public bool Equals(TEventID x, TEventID y)
            {
                return false;
            }

            public int GetHashCode(TEventID obj)
            {
                return 0;
            }
        }
    }
    
    
    /// <summary>
    /// EventDispatcher with custom data type, and custom IEqualityComparer for TEventID.
    /// Can use it as T in <see cref = "ZTools.Core.Singleton{T}"/> to make it singleton.
    /// </summary>
    public class EventDispatcher<TEventID, TData, TComparer> : IEventDispatcher<TEventID, TData>
        where TComparer: IEqualityComparer<TEventID>, new()
    {
        private Dictionary<TEventID, List<EventHandler<TData>>> _handlers = null;

        public EventDispatcher(): this(new TComparer()){}
        
        public EventDispatcher(TComparer cp)
        {
            _handlers = new Dictionary<TEventID, List<EventHandler<TData>>>(cp);
        }

        public void AddListener(TEventID eventID, EventHandler<TData> handlerToAdd)
        {
            if (_handlers.ContainsKey(eventID))
            {
                _handlers[eventID].Add(handlerToAdd);
            }
            else
            {
                _handlers[eventID] = new List<EventHandler<TData>> { handlerToAdd };
            }
        }

        public void RemoveListener(TEventID eventID, EventHandler<TData> handlerToRemove)
        {
            if (_handlers.ContainsKey(eventID))
            {
                var handlers = _handlers[eventID];
                for (int i = 0; i < handlers.Count; i++)
                {
                    if (handlerToRemove == handlers[i])
                    {
                        _handlers[eventID].RemoveAt(i);
                    }
                }
            }
        }

        public void Dispatch(TEventID eventID, TData data = default)
        {
            if (_handlers.ContainsKey(eventID))
            {
                for (int j = 0; j < _handlers[eventID].Count; j++)
                {
                    var handler = _handlers[eventID][j];
                    handler(data);
                }
            }
        }
    }
    
}