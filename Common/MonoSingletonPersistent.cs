using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.Common
{
    /// <summary>
    /// Persistent while application is running.
    /// Thread safe implementation of Singleton pattern using lasy initialization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingletonPersistent<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        private static readonly object lockObject = new object();

        public static T Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType<T>();

                        if (instance == null)
                        {
                            GameObject singletonObject = new GameObject(typeof(T).Name);
                            instance = singletonObject.AddComponent<T>();
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                    return instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
