using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZTools.Test
{
    
    public class classA : MonoBehaviour
    {
        public static classA instance;

        public List<int> myList = new List<int>() { 1, 2, 3 };
        //public AType aType = AType.small;
        //public Dictionary<string, int> dic = new Dictionary<string, int>();

        public string newData = "ddd";

        public enum AType
        {
            small,
            big,
        }

        [System.Serializable]
        public class SerializedClassA
        {
            public float[] position;
            public List<int> list;
            //public Dictionary<string, int> dic;

            //public AType type;
            public string newd;

            public SerializedClassA(classA a)
            {
                position = new float[3];
                position[0] = a.transform.position.x;
                position[1] = a.transform.position.y;
                position[2] = a.transform.position.z;


                list = new List<int>(a.myList);

                //dic = a.dic;

                //type = a.aType;

                newd = a.newData;
            }

            public void deserialize(classA a)
            {
                a.transform.position = new Vector3(position[0], position[1],position[2]);
                a.myList = list;
                //a.aType = type;
                //a.dic = dic;
                a.newData = newd;
            }
        }

        private void Awake()
        {
            instance = this;
        }
    
        public void printDic()
        {
            //foreach (var kvp in dic)
            //{
            //    Debug.Log("k=" + kvp.Key.ToString() + ",v=" + kvp.Value.ToString());
            //}
        }
    }
}