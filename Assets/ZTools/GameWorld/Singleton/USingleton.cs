using UnityEngine;

namespace ZTools.Game.Singleton
{
    public abstract class USingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] objs = GameObject.FindObjectsOfType<T>(true);
                    if (objs.Length > 0)
                    {
                        instance = objs[0];
                        for (int i = 1; i < objs.Length; i++)
                        {
                            GameObject.Destroy(objs[i].gameObject);
                        }
                    }
                    else
                    {
                        GameObject newObj = new GameObject(typeof(T).Name);
                        DontDestroyOnLoad(newObj);
                        instance = newObj.AddComponent<T>();
                    }
                }

                return instance;
            }
        }
    }
}
