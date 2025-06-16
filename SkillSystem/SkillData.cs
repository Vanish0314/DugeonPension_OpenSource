using System;
using UnityEngine;
using CrashKonijn.Agent.Core;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using GameFramework;
using Dungeon.SkillSystem.SkillEffect;
using Dungeon.GameFeel;

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
        [LabelText("火焰伤害")] Fire,
        [LabelText("冰冻伤害")] Ice,
        [LabelText("圣洁伤害")] Holy,
        [LabelText("毒素伤害")] Poison,
    }

    [Serializable]
    public class SkillDeployDesc
    {
        [Serializable]
        public enum SkillShootType
        {
            [LabelText("方向式"), Tooltip("hitbox中心为技能释放者")] Directional,
            [LabelText("落点式"), Tooltip("hitbox中心为技能释放点")] Point,
            [LabelText("原地式"), Tooltip("hitbox中心为技能释放者,方向已经不重要")] Origin,
            [LabelText("子弹式"), Tooltip("从释放者位置发射一个子弹")] Bullet
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
        [LabelText("作用层级")] public SkillUseageLayer useageLayer = SkillUseageLayer.Enemy;
        [Required, LabelText("HitBox")] public GameObject hitBoxPrefab;
        [Obsolete("希望使用这个来作为施法半径的乘数,但现在没用")][LabelText("技能范围乘数"), Tooltip("默认是1,hitbox多大作用范围多大")] public float range = 1;//TODO: not used yet
        [LabelText("施法半径"), Tooltip("施法半径是可以放技能的最远距离")] public float radius = 1;
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
        [InfoBox(
         "配置技能需要注意:\n" +
         "1. 技能前摇,中摇,后摇都需要配置,时长上尽量与动画匹配"
        )]
        [LabelText("冷却时间(秒)")] public float cooldownTimeInSec;
        [LabelText("技能前摇时间")] public float preCastTimeInSec;
        [LabelText("技能中摇时间")] public float midCastTimeInSec;
        [LabelText("技能后摇时间")] public float postCastTimeInSec;
        public float TotalUsageTime => preCastTimeInSec + midCastTimeInSec + postCastTimeInSec;
        [LabelText("法力消耗")] public float magicCost;

        [Space]
        [Header("施放方式")]
        [LabelText("技能施放方式")] public SkillDeployDesc deployMethodDesc;
        [Header("以下属性只在释放方式为子弹式时有效")]
        [LabelText("子弹速度"), Tooltip("子弹飞行的速度")] public float bulletSpeed;
        [LabelText("子弹射程"), Tooltip("子弹射程,当到达射程时,子弹消失")] public float bulletRange;
        [LabelText("子弹存活时间"), Tooltip("子弹存活时间,当子弹飞行时间超过这个时间,子弹消失")] public float bulletLifeTime;

        [Space]
        [Header("动画 & 特效")]
        [InfoBox(
            "技能命中特效的Go需要附带一个 SpriteVisualEffectController 组件\n" +
            "在组件中进行特效的设置."
        )]

        [LabelText("技能施放特效")]
        [Tooltip("以hitbox为中心的一个特效,当释放方式是子弹式时,这个特效会随着子弹移动")]
        public GameObject effectPrefab;

        [LabelText("命中特效")]
        [Tooltip("以技能命中点为中心的一个特效")]
        public GameObject hitEffectPrefab;

        [LabelText("音效")] public string soundEffect;

        [Space]
        [Header("技能效果")]
        [Required, LabelText("技能效果")] public List<SkillEffectBase> skillEffects;

        //==============HIDE IN INSPECTOR=================

        public void UpdateCoolDown(float deltaTime)
        {
            timeToColdDown -= deltaTime;
        }
        public void ResetCoolDown()
        {
            timeToColdDown = cooldownTimeInSec;
        }
        public bool IsCooledDown()
        {
            return timeToColdDown <= 0f;
        }
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
        public bool IsInRange(Vector3 fromInWorldCoord, Vector3 targetInWorldCoord)
        {
            return Vector3.Distance(fromInWorldCoord, targetInWorldCoord) <= deployMethodDesc.radius;
        }
        public bool IsInRange(float distance)
        {
            return distance <= deployMethodDesc.radius;
        }

        [Button("检查技能配置是否正确")]
        private void Validate()
        {
            {
                if (effectPrefab != null && effectPrefab.GetComponent<SpriteVisualEffectController>() == null)
                {
                    GameFrameworkLog.Error("[SkillData] 技能配置错误: 特效GO没有SpriteVisualEffectController组件");
                }

                if (hitEffectPrefab != null && hitEffectPrefab.GetComponent<SpriteVisualEffectController>() == null)
                {
                    GameFrameworkLog.Error("[SkillData] 技能配置错误: 命中特效GO没有SpriteVisualEffectController组件");
                }
            }

            {
                if (String.IsNullOrEmpty(soundEffect))
                {
                    GameFrameworkLog.Warning("[SkillData] 技能配置警告: 音效为空");
                }

                if (skillEffects.Count == 0)
                {
                    GameFrameworkLog.Error("[SkillData] 技能配置错误: 无效的技能效果列表");
                }
            }
        }

        private float timeToColdDown = 0f;

    }

}