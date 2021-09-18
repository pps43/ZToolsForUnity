namespace ZTools.Core
{
    using System;
    /// <summary>
    /// Multithread-safe singleton of type T.
    /// </summary>
    /// <typeparam name="T">Your type.</typeparam>
    public class Singleton<T> where T: new()
    {
        private Singleton() {}
        
#if UNITY_2019_1_OR_NEWER
        private static Lazy<T> _instance = new Lazy<T>(() => new T());

        public static T Instance => _instance.Value;
#else
        private static T _instance;
        private static object _syncRoot = new object();
        
        public static T Instance
        {
            get 
            {
                if (_instance == null) 
                {
                    lock (_syncRoot) 
                    {
                        if (_instance == null) 
                            _instance = new T();
                    }
                }
                return _instance;
            }
        }
#endif
    }
}