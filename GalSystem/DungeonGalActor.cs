using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using UnityEngine;

namespace Dungeon.Gal
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Dungeon Gal Actor", menuName = "GalSystem/Dungeon Gal Actor")]
    public class DungeonGalActor : ScriptableObject, IDialogueActor
    {
        // 示例字段供展示
        public string Name;
        public Texture2D Portrait;
        public Sprite PortraitSprite;
        public Color DialogueColor;
        public Vector3 DialoguePosition;

        // 接口实现
        string IDialogueActor.name => Name;
        Texture2D IDialogueActor.portrait => Portrait;
        Sprite IDialogueActor.portraitSprite => PortraitSprite;
        Color IDialogueActor.dialogueColor => DialogueColor;
        Vector3 IDialogueActor.dialoguePosition => DialoguePosition;
        Transform IDialogueActor.transform => DungeonGameEntry.DungeonGameEntry.GalSystem.GetControllerTransform();
    }
}
