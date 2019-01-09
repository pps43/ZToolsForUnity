using System;
using System.Collections.Generic;

namespace ZTools.Event
{
    public enum EventID
    {
        onLevelLoad,
        onLevelFinish,

        onPauseGame,
        onExitGame,
        onRestartGame,
        onTurn,// may be used in turn-based game

        //add new event type
    }

    /// <summary>
    /// EventIDComparer is used for eliminating GC caused by Enum as Key
    /// </summary>
    public class EventIDComparer : IEqualityComparer<EventID>
    {
        public bool Equals(EventID x, EventID y)
        {
            return x == y;
        }

        public int GetHashCode(EventID x)
        {
            return (int)x;
        }
    }

    /// <summary>
    /// Just a place-holder
    /// </summary>
    public struct NoEvent { }

    /// <summary>
    /// Inter-module event
    /// </summary>
    public struct CommonEvent
    {
        public EventID eventID;
        public long senderID;
        public long receiverID;
        public object eventData;

        public CommonEvent(EventID ID, long seID, long reID, object data = null)
        {
            eventID = /*(BaseEvent.EventID)*/ID;
            senderID = seID;
            receiverID = reID;
            eventData = data;
        }
        public CommonEvent(EventID ID, object data = null)
        {
            eventID = /*(BaseEvent.EventID)*/ID;
            senderID = -1;
            receiverID = -1;
            eventData = data;
        }

        
    }

}