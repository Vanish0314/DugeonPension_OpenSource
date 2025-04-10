using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Runtime;
using Dungeon.BlackBoardSystem;
using Dungeon.GridSystem;
using GameFramework;
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
        public static BlackboardController WorldBlackboard
        {
            get;
            private set;
        }
        public static GridSystem.GridSystem GridSystem
        {
            get;
            private set;
        }
        private static void InitDungeonComponents()
        {
            // GOAP
            GOAP = FindObjectOfType<GoapBehaviour>();

            // World Blackboard
            {
                var allBlackboards = FindObjectsOfType<BlackboardController>();
                for (int i = 0; i < allBlackboards.Length; i++)
                {
                    if (allBlackboards[i].gameObject.name == "WorldBlackboard")
                    {
                        WorldBlackboard = allBlackboards[i];
                        break;
                    }
                }
                if (WorldBlackboard == null)
                {
                    GameFrameworkLog.Error("[DungeonGameEntry] WorldBlackboard not found!");
                }
            }

            // Grid System
            GridSystem = FindObjectOfType<GridSystem.GridSystem>();
        }
    }

    public static class DungeonGameWorldBlackboardEnum
    {
        public static readonly string DungeonExitWorldPositionVector3 = "DungeonExitWorldPositionVector3";
    }
}
