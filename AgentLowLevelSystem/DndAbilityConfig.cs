using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Character
{
    [System.Serializable]
    public struct DndAbilityConfig
    {
        [LabelText("力量")][Range(1, 20)] public int Strength;
        [LabelText("敏捷")][Range(1, 20)] public int Dexterity;
        [LabelText("体质")][Range(1, 20)] public int Constitution;
        [LabelText("智力")][Range(1, 20)] public int Intelligence;
        [LabelText("知识")][Range(1, 20)] public int Wisdom;
        [LabelText("魅力")][Range(1, 20)] public int Charisma;
    }
}
