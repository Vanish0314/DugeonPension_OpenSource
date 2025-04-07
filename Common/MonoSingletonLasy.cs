using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.Common
{
    /// <summary>
    /// Thread safe implementation of the Singleton pattern using a lazy initialization.
    /// </summary>
    /// <typeparam name="T">The type of the singleton.</typeparam>
    public class MonoSingletonLasy<T> : MonoBehaviour where T : MonoBehaviour
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
                        }
                    }
                    return instance;
                }
            }
        }
    }
}
