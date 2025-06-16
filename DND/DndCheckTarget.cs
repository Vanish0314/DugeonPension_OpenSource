using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "New DndCheckTarget", menuName = "DND/DndCheckTarget")]
    public class DndCheckTarget : ScriptableObject
    {
        [LabelText("检定目标名称")][SerializeField] private string mName; // 目标名称，如陷阱、魔法、挑战
        [LabelText("检定DC")][SerializeField][UnityEngine.Range(1, 20)] private int difficultyClass = 1; // 目标的DC（挑战难度）[1-20]
        [LabelText("检定所需技能")][SerializeField] private DndSkill requiredSkill; // 需要的技能类型（如“敏捷”或“解除装置”）

        public string Name { get => mName; private set => mName = value; }
        public int DifficultyClass { get => difficultyClass; private set => difficultyClass = value; }
        public DndSkill RequiredSkill { get => requiredSkill; private set => requiredSkill = value; }

        public DndCheckTarget(string name, int dc, DndSkill requiredSkill)
        {
            Name = name;
            DifficultyClass = dc;
            RequiredSkill = requiredSkill;
        }

        public override string ToString()
        {
            return $"{Name} (DC: {DifficultyClass}, Skill: {RequiredSkill})";
        }
    }
}
