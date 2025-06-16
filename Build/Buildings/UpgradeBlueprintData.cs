using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.SkillSystem;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "UpgradeBlueprintData", menuName = "Build/Upgrade Blueprint Data")]
    public class UpgradeBlueprintData : ScriptableObject
    {
        public MonsterType monsterType;
        public int blueprintID;
        
        [Header("General")]
        public string blueprintName;
        public Sprite blueprintIcon;
        
        [Header("Lock")]
        [TextArea(1,8)]
        public string blueprintDescription0;
        public int unlockPointCost;
        public int unlockExpCost;
        public bool isMonster = true;

        [Header("Level1")] 
        [TextArea(1,8)]
        public string blueprintDescription1;
        public int upgradeExpCost1;
        
        [Header("Level2")]
        [TextArea(1,8)]
        public string blueprintDescription2;
        public int upgradeExpCost2;
        public int maxHp2;
        public float attackSpeed2;
        public ResistanceLevel physicalResistance2;
        public ResistanceLevel fireResistance2;
        public ResistanceLevel iceResistance2;
        public ResistanceLevel holyResistance2;
        public ResistanceLevel posionResistance2;
        public SkillData skillData2;
        
        [Header("Level3")]
        [TextArea(1,8)]
        public string blueprintDescription3;
        public int maxHp3;
        public float attackSpeed3;
        public ResistanceLevel physicalResistance3;
        public ResistanceLevel fireResistance3;
        public ResistanceLevel iceResistance3;
        public ResistanceLevel holyResistance3;
        public ResistanceLevel posionResistance3;
        public SkillData skillData3;
        
        [Header("Status")]
        public bool isUnlocked = false;
        public int blueprintLevel = 0;
    }
}
