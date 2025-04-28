using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Codice.Client.Commands;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon
{
    public interface ICombatable
    {
        public int Hp{get;set;}
        public int MaxHp{get;set;}
        public int Mp{get;set;}
        public int MaxMp{get;set;}
        public float AttackSpeed{get;set;}
        public CombatorData BasicInfo{get;set;}
        public StatusBarSetting StatusBarSetting{get;set;}
        
        public GameObject GetGameObject();
        public bool IsAlive();
        public void OnKillSomebody(ICombatable killed);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skill"></param>
        /// <returns>返回是否死亡</returns> //FIXME: 有的技能不会立即死亡
        public bool TakeSkill(Skill skill);

        /// <summary>
        /// 进入僵直状态
        /// </summary>
        /// <param name="duration"></param>
        public void Stun(float duration);
    }


    [Serializable]
    public class DndSkillData
    {
        [LabelText("力量"),Range(1,20)] public int Strength;
        [LabelText("敏捷"),Range(1,20)] public int Dexterity;
        [LabelText("体质"),Range(1,20)] public int Constitution;
        [LabelText("智力"),Range(1,20)] public int Intelligence;
        [LabelText("感知"),Range(1,20)] public int Wisdom;
        [LabelText("魅力"),Range(1,20)] public int Charisma;
    }

    [Serializable]
    public class CombatorData
    {
        public int hp;
        public int maxHp;
        public int mp;
        public int maxMp;
        
        /// <summary>
        /// 0代表无法攻击，1代表正常攻击速度,2代表双倍攻击速度...
        /// </summary>
        public float attackSpeed;

        /// <summary>
        /// 当前的等级
        /// </summary>
        [LabelText("等级"),Range(1,20)] public int currentLevel;

        /// <summary>
        /// 物理属性抗性
        /// </summary>
        public ResistanceLevel physicalResistance;

        /// <summary>
        /// 火属性抗性
        /// </summary>
        public ResistanceLevel fireResistance;

        /// <summary>
        /// 冰属性抗性
        /// </summary>
        public ResistanceLevel iceResistance;

        /// <summary>
        /// 圣属性抗性
        /// </summary>
        public ResistanceLevel holyResistance;

        /// <summary>
        /// 毒属性抗性
        /// </summary>
        public ResistanceLevel posionResistance;
    }
    [Serializable]
    public struct StatusBarSetting
    {
        [LabelText("血条位置偏移")] public Vector3 offset;
    }

    [Serializable]
    public enum ResistanceLevel
    {
        [LabelText("弱点")]Weak,
        [LabelText("普通")]Normal,
        [LabelText("抵抗")]Strong,
        [LabelText("免疫")]Immunity
    }
    
}
