using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using MoreMountains.Tools;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Gal
{
    [Serializable]
    public class ActorEntry
    {
        public string name;
        public DungeonGalActor actor;
    }

    public class DaoManager : MonoBehaviour
    {
        [SerializeField]
        private List<ActorEntry> actorEntries = new();

        [ShowInInspector, ReadOnly]
        private Dictionary<string, DungeonGalActor> actors = new();
        private void Awake()
        {
            foreach (var entry in actorEntries)
            {
                if (!string.IsNullOrEmpty(entry.name) && entry.actor != null)
                {
                    if (!actors.ContainsKey(entry.name))
                    {
                        actors.Add(entry.name, entry.actor);
                    }
                    else
                    {
                        GameFrameworkLog.Warning($"[DaoManager] 怎么有多个名字相同的DungeonActor?: {entry.name}");
                    }
                }
            }
        }
        public bool TryGetActor(string name, out IDialogueActor actor)
        {
            if (actors.TryGetValue(name, out DungeonGalActor dungeonActor))
            {
                actor = dungeonActor;
                return true;
            }
            else
            {
                GameFrameworkLog.Error($"[DaoManager][对话树] 你嘛的,找不到{name}这个DungeonActor,对话树无法正常显示图片,请检查配置,名字是否正确");

                actor = null;
                return false;
            }
        }
    }


}
