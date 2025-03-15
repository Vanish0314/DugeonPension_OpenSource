using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.Common
{
    public class Singleton<T> where T : new()
{
    private static readonly object lockObject = new object();
    private static T instance;

    public static T Instance
    {
        get
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }
}
}
