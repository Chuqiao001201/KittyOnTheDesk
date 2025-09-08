using UnityEngine;

namespace Utilities
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }
    
        protected virtual void Awake()
        {
            if (!Instance)
            {
                Instance = (T)this;
                // DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
