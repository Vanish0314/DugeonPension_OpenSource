using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon
{
    public interface ICombatable
    {
        // public UInt16 GetCurrentHP();
        // public UInt16 GetMaxHP();
        // public UInt16 GetCurrentMP();
        // public UInt16 GetMaxMP();
        public int Hp{get;set;}
        public int MaxHp{get;set;}
        public int Mp{get;set;}
        public int MaxMp{get;set;}
        public float AttackSpeed{get;set;}
        public CombatorData BasicInfo{get;set;}
        
        public GameObject GetGameObject();
        public void TakeSkill(Skill skill);

        /// <summary>
        /// 进入僵直状态
        /// </summary>
        /// <param name="duration"></param>
        public void Stun(float duration);
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

    public enum ResistanceLevel
    {
        [LabelText("弱点")]Weak,
        [LabelText("普通")]Normal,
        [LabelText("抵抗")]Strong,
        [LabelText("免疫")]Immunity
    }
    
}
