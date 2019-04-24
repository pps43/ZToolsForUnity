using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using ZTools.DebugUtil;
namespace ZTools.PersistData
{
    public enum SerializationMethod
    {
        json,//doto
        bin,
        //xml,
        //yaml,
    }
    /// <summary>
    /// serialization and persist data
    /// </summary>
    public class PersistDataManager
    {
        public static PersistDataManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PersistDataManager();
                }
                return _instance;
            }
        }
        public static PersistDataManager _instance;
        private PersistDataManager() { }

        public SerializationMethod serializationMethod = SerializationMethod.bin;

        //in memory
        private Dictionary<string/*data id*/, BaseSerializableData> _dataCache = new Dictionary<string, BaseSerializableData>();

#if UNITY_EDITOR
        private readonly string _persistPath = Application.dataPath;//rw

#else
        private readonly string _persistPath = Application.persistentDataPath;//rw
        
#endif
        private readonly string _streamPath = Application.streamingAssetsPath;//r

        #region public

        public void save(string tag, BaseSerializableData data, DataLocation location)
        {
            data.dataTag = tag;

            if (location == DataLocation.memory)
            {
                writeCache(tag, data);
            }
            else if (location == DataLocation.local)
            {
                writeFile(_persistPath, tag, data);
            }
            else if (location == DataLocation.remote)
            {
                ZLog.error("not yet");
            }
        }


        public BaseSerializableData load(string tag, DataLocation location)
        {
            BaseSerializableData data = null;

            if (location == DataLocation.memory)
            {
                if (!readCache(tag, out data))
                {
                    ZLog.log("TODO: cache miss, read from file");

                }
            }
            else if (location == DataLocation.local)
            {
                if (!readFile(_persistPath, tag, out data))
                {
                    ZLog.warn("TODO: file miss, read from net");

                }
            }
            else if (location == DataLocation.remote)
            {
                ZLog.error("not yet");
            }

            return data;
        }


        /// <summary>
        /// 一次性将cache所有条目保存到某个位置，然后清除缓存。
        /// </summary>
        public void saveCacheTo(DataLocation location)
        {
            if (location == DataLocation.local)
            {
                Directory.CreateDirectory(_persistPath);//如果已经有了就不会创建
                string path = _persistPath + "/" + "cache.bytes";
                FileStream stream = new FileStream(path, FileMode.Create);//create new or overwritten
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, _dataCache);
                _dataCache.Clear();
                stream.Close();

            }
            else if (location == DataLocation.remote)
            {
                ZLog.error("not yet");

            }
        }

        public void loadCacheFrom()
        {
            string path = _persistPath + "/" + "cache.bytes";

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                _dataCache = formatter.Deserialize(stream) as Dictionary<string, BaseSerializableData>;
                stream.Close();
            }

        }

        #endregion

        #region private

        private void writeFile(string folder, string tag, BaseSerializableData data)
        {
            Directory.CreateDirectory(folder);//如果已经有了就不会创建

            string path = _persistPath + "/" + tag + ".bytes";
            FileStream stream = new FileStream(path, FileMode.Create);//create new or overwritten

            if (serializationMethod == SerializationMethod.bin)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
            }
            else if(serializationMethod == SerializationMethod.json)
            {
                //string content = JsonUtility.ToJson(data);

            }
            
            stream.Close();
        }

        private void writeCache(string tag, BaseSerializableData data)
        {
            if (_dataCache.ContainsKey(tag))
            {
                _dataCache[tag] = data;
            }
            else
            {
                _dataCache.Add(tag, data);
            }
        }

        private bool readCache(string tag, out BaseSerializableData data)
        {
            data = null;

            if (_dataCache.ContainsKey(tag))
            {
                data = _dataCache[tag];
                return true;
            }

            return false;
        }

        private bool readFile(string folder, string tag, out BaseSerializableData data)
        {
            data = null;

            string path = folder + "/" + tag + ".bytes";

            if (File.Exists(path))
            {
                FileStream stream = new FileStream(path, FileMode.Open);
                if (serializationMethod == SerializationMethod.bin)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    data = formatter.Deserialize(stream) as BaseSerializableData;

                }
                else if(serializationMethod == SerializationMethod.json)
                {

                }

                stream.Close();
                return true;
            }

            return false;
        }

        //private bool readNetwork()//TODO 异步回调

        #endregion


    }
}