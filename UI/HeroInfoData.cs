using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "NewHeroInfoData", menuName = "Hero/HeroInfoData")]
    public class HeroInfoData : ScriptableObject
    {
        public HeroType heroType; // 英雄唯一标识
        public Sprite heroSprite;
        public BaseAttribute baseAttribute;
        public SixDimensionalAttribute sixDimensionalAttribute;
        public Skill skill;
        
        [System.Serializable]
        public struct BaseAttribute
        {
            public int HP;
            public int ATK;
            public int DEF;
            public int MP;
            public int MoveSpeed;
            public float AttackSpeed;
            public int RecoverMPSpeed;
            public int Pressure;
            public int Miss;
        }
        [System.Serializable]
        public struct SixDimensionalAttribute
        {
            public int Strength;
            public int Intelligence;
            public int Sensor;
            public int Agility;
            public int Constitution;
            public int Charisma;
        }
        [System.Serializable]
        public struct Skill
        {
            public string Skill1;
            public string Skill2;
        }
    }
}
