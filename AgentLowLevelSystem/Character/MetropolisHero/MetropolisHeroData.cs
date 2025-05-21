using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dungeon
{
    [Serializable]
    public class MetropolisHeroData
    {
        // 堕落等级
        public int CorruptLevel = 0;
        public int MaxCorruptLevel = 0;
        
        // 饱食度
        public int HungerLevel = 0;
        public int MaxHungerLevel = 0;
        
        // 精神力
        public int MentalLevel = 0;
        public int MaxMentalLevel = 0;
        
        // 疲劳度
        public int TiredLevel = 0;
        public int MaxTiredLevel = 0;
        
        // 工作效率
        public int Efficiency = 0;
        public int MaxEfficiency = 0;
    }
}
