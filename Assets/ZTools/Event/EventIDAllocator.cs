using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZTools.Event
{
    /// <summary>
    /// ID generator
    /// 
    /// </summary>
    public static class IDAllocator
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
        public static void returnID(long id)
        {
            //todo 归还id
        }
    }
}