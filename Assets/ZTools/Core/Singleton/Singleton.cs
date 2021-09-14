namespace ZTools.Core
{
    using System;
    /// <summary>
    /// Multithread-safe singleton of type T.
    /// </summary>
    /// <typeparam name="T">Your type.</typeparam>
    public class Singleton<T>
    {
        private Singleton() {}
        
#if UNITY_2019_1_OR_NEWER
        private static Lazy<Singleton<T>> _instance = new Lazy<Singleton<T>>(() => new Singleton<T>());

        public static Singleton<T> Instance => _instance.Value;
#else
        private static volatile Singleton<T> _instance;
        private static object _syncRoot = new object();
        
        public static Singleton<T> Instance
        {
            get 
            {
                if (_instance == null) 
                {
                    lock (_syncRoot) 
                    {
                        if (_instance == null) 
                            _instance = new Singleton<T>();
                    }
                }
                return _instance;
            }
        }
#endif
    }
}