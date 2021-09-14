using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZTools.Debug;

namespace ZTools.Game
{
    public enum DataLocation
    {
        memory,
        local,//local file
        remote,//network
    }

    /// <summary>
    /// BaseClass can be serialized and saved by BasePersistDataHelper
    /// </summary>
    [Serializable]
    public abstract class BaseSerializableData
    {
        public string dataTag; //usually is filename

    }
    

    /// <summary>
    /// can load/save data by BasePersistDataHelper
    /// </summary>
    public interface IDataOwner<T> where T: BaseSerializableData
    {
        T data { get; set; }
    }

    
    /// <summary>
    /// help owner save/load its data.
    /// can save/load from different place.
    /// </summary>
    [Serializable]
    public abstract class BasePersistDataHelper<OWNER, DATA> where OWNER: IDataOwner<DATA> where DATA : BaseSerializableData
    {
        public BasePersistDataHelper()
        {
            datafileName = System.Guid.NewGuid().ToString();
        }

        public BasePersistDataHelper(OWNER owner)
        {
            datafileName = System.Guid.NewGuid().ToString();
            _owner = owner;
        }

        public BasePersistDataHelper(OWNER owner, string fileName)
        {
            datafileName = fileName;
            _owner = owner;
        }

        public string datafileName; //also used as id
        public DataLocation saveLocation = DataLocation.local;
        public DataLocation loadLocation = DataLocation.local;
        [SerializeField]private OWNER _owner;

        /// <summary>
        /// pre-process of serialization
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual DATA encoder(OWNER obj)
        {
            return obj.data;
        }

        /// <summary>
        /// post-process of deserialization,
        /// </summary>
        /// <param name="data"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual bool decoder(DATA data, ref OWNER obj)
        {
            if(data != null)
            {
                obj.data = data;
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool loadData()
        {
            if (PersistDataManager.instance.load(datafileName, loadLocation) is DATA data)
            {
                return decoder(data, ref _owner);
            }
            else
            {
                ZLog.error("loadData fail. tag = ", datafileName);
                return false;
            }
        }

        public void saveData()
        {
            DATA data = encoder(_owner);
            if (data != null)
            {
                PersistDataManager.instance.save(datafileName, data, saveLocation);
            }
        }

    }




}
