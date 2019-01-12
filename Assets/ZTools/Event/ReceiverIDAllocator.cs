namespace ZTools.Event
{
    /// <summary>
    /// ID generator used by eventsystem
    /// </summary>
    public static class ReceiverIDAllocator
    {
        public const int GLOBALID = 0;
        private static long _curAvaliableID = 1;
        //todo use NeGuid
        //https://docs.microsoft.com/en-us/dotnet/api/system.guid.compareto?view=netframework-4.7.2#System_Guid_CompareTo_System_Guid_
        public static long getID()
        {
            long result = _curAvaliableID;
            _curAvaliableID += 1;
            return result;
        }
        
    }
}