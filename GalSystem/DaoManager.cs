using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Gal
{
    public class DaoManager : MonoBehaviour
    {
        [ShowInInspector, SerializeReference]
        private Dictionary<string, DungeonGalActor> actors = new();

        public bool TryGetActor(string name, out IDialogueActor actor)
        {
            if (actors.TryGetValue(name, out DungeonGalActor dungeonActor))
            {
                actor = dungeonActor;
                return true;
            }
            else
            {
                GameFrameworkLog.Warning($"[DaoManager] 你嘛的,找不到{name}这个DungeonActor,对话树无法正常显示图片,请检查配置,名字是否正确");

                actor = null;
                return false;
            }
        }
        
    }

    
}
