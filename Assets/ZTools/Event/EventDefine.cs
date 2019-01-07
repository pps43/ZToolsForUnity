using System;
using System.Collections.Generic;

namespace ZTools.Event
{
    /// <summary>
    /// Just a place-holder
    /// </summary>
    public struct NoEvent { }

    /// <summary>
    /// Inter-module event
    /// </summary>
    public struct CommonEvent
    {
        public ID eventID;
        public long senderID;
        public long receiverID;
        public object eventData;

        public CommonEvent(ID ID, long seID, long reID, object data = null)
        {
            eventID = /*(BaseEvent.EventID)*/ID;
            senderID = seID;
            receiverID = reID;
            eventData = data;
        }
        public CommonEvent(ID ID, object data = null)
        {
            eventID = /*(BaseEvent.EventID)*/ID;
            senderID = -1;
            receiverID = -1;
            eventData = data;
        }

        public enum ID
        {
            onLevelLoad,
            onLevelFinish,

            onPauseGame,
            onExitGame,
            onRestartGame,

            //add new event type
        }
    }

}