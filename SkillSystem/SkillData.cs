using System;
using UnityEngine;
using CrashKonijn.Agent.Core;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Mono.Cecil;
using GameFramework;
using System.Text;
using Dungeon.SkillSystem.SkillEffect;
using System.Reflection;
using System.Linq;
#if UNITY_EDITOR
using Dungeon.SkillSystem;
using UnityEditor;
#endif
namespace Dungeon.SkillSystem
{
    [Serializable]
    public struct NDX
    {
        [LabelText("N:几个骰子")] public UInt16 N;
        [LabelText("X:骰子面数")] public UInt16 X;

        public readonly int Claculate() => N * UnityEngine.Random.Range(1, X + 1);
    }

    [Serializable]
    public enum DamageType
    {
        [LabelText("物理伤害")] Physical,
        [LabelText("魔法伤害")] Magical,
    }

    [Serializable]
    public struct SkillDeployDesc
    {
        [Serializable]
        public enum SkillShootType
        {
            [LabelText("方向式")] Directional,
            [LabelText("落点式")] Point
        }
        public enum SkillAoeType
        {
            [LabelText("单体")] Single,
            [LabelText("群体")] Area
        }
        public enum SkillUseageLayer
        {
            [LabelText("友方")] Friendly,
            [LabelText("敌方")] Enemy,
            [LabelText("无差别不分敌我")] All
        }

        [LabelText("施放方式")] public SkillShootType shootType;
        [LabelText("AOE类型")] public SkillAoeType aoeType;
        [Required,LabelText("HitBox")] public readonly GameObject hitBoxPrefab;
        [LabelText("施法半径")] public float radius;
    }
    public class Buff
    {

    }

    [CreateAssetMenu(fileName = "New SkillData", menuName = "Skill System/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Space]
        [Header("基本信息")]
        [LabelText("技能ID")] public UInt32 skillID;
        [LabelText("技能名称")] public string skillName;
        [LabelText("技能描述")][TextArea] public string skillDescription;
        [LabelText("技能图标")] public Sprite skillIcon;

        [Space]
        [Header("技能消耗 & 冷却")]
        [LabelText("冷却时间(秒)")] public float cooldownTimeInSec;
        [LabelText("技能前摇时间")] public float preCastTimeInSec;
        [LabelText("技能中摇时间")] public float midCastTimeInSec;
        [LabelText("技能后摇时间")] public float postCastTimeInSec;
        [LabelText("法力消耗")] public float magicCost;

        [Space]
        [Header("施放方式")]
        [LabelText("技能施放方式")] public SkillDeployDesc deployMethodDesc;

        [Space]
        [Header("动画 & 特效")]
        [LabelText("技能施放特效")] public GameObject effectPrefab;
        [LabelText("命中特效")] public GameObject hitEffectPrefab;
        [LabelText("音效")] public AudioClip soundEffect;

        [Space]
        [Header("技能效果")]
        [Required,LabelText("技能效果")] public List<SkillEffectBase> skillEffects;


        //==============HIDE IN INSPECTOR=================


        public static SkillData GetFromSkillDesc(SkillDesc desc)
        {
            var skillData = Resources.Load<SkillData>("Skills/" + desc.name);
#if UNITY_EDITOR
            if (skillData == null)
            {
                GameFrameworkLog.Error("[SkillData] SkillData not found: " + desc.name + "\n"
                    + "Load path: Resources/Skills/" + desc.name);
            }
#endif

           return skillData;
        }
        public bool IsInRange(Vector3 fromInWorldCoord,Vector3 targetInWorldCoord)
        {
            return Vector3.Distance(fromInWorldCoord, targetInWorldCoord) <= deployMethodDesc.radius;
        }
    }
}