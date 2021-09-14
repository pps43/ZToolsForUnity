using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ZTools.Test
{
    public class SerializeUtil : MonoBehaviour
    {
        public classA obj;

        public string relativePath = "/classAData.bytes";

        public bool useJson = false;

        public void save()
        {
            FileStream stream = new FileStream(Application.persistentDataPath + relativePath, FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, new classA.SerializedClassA(obj));

            stream.Close();
        
        }

        public void load()
        {
            string path = Application.persistentDataPath + relativePath;
            if(File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                classA.SerializedClassA data = formatter.Deserialize(stream) as classA.SerializedClassA;

                stream.Close();

                data.deserialize(obj);
            }
            else
            {
                Debug.LogError("file not exist on:" + path);
            }
        }
    }
}