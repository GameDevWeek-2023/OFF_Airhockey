using UnityEngine;

namespace Airhockey.Utils {
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
        private static T m_instance;
        protected virtual bool IsPersistant => false;

        public static T Instance {
            get {
                if (m_instance != null) return m_instance;

                m_instance = FindObjectOfType<T>();
                if (m_instance != null) return m_instance;

                var obj = new GameObject($"{typeof(T)} [Singleton]");
                m_instance = obj.AddComponent<T>();

                if (m_instance.IsPersistant)
                    DontDestroyOnLoad(obj);

                return m_instance;
            }
        }
    }
}