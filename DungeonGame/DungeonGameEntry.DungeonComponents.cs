using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Dungeon.DungeonGameEntry
{
    public partial class DungeonGameEntry : MonoBehaviour
    {
        /// <summary>
        /// 获取GOAP Behaviour组件
        /// </summary>
        public static GoapBehaviour GOAP
        {
            get;
            private set;
        }
        private static void InitDungeonComponents()
        {
            GOAP = GameObject.FindObjectOfType<GoapBehaviour>();
        }
    }
}
