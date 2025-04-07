using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "New BuildingProduceData", menuName = "Building/BuildingProduceData")]
    public class BuildingProduceData : ScriptableObject
    {
        [Header("Production Settings")]
        public ResourceType outputResource;
        public int baseOutputAmount = 1;// 生产倍率
        public float productionInterval = 1f;// 生产速率
        public bool useRandomInterval = false;// 呃，随机生产速率（用到再说）
        public Vector2 randomIntervalRange = new Vector2(0.8f, 1.2f);
        public Gradient productionGradient;// 冒泡文字颜色
    
        // 勇者进驻相关（暂时用不到）
        [Header("Hero Bonus")]
        public bool canAssignHeroes = false;
        public int maxAssignedHeroes = 1;
        public float heroOutputMultiplier = 0.2f;
    
        // 特效相关（暂时也用不到）
        [Header("Special Effects")]
        public GameObject productionEffectPrefab;
        public AudioClip productionSound;
        public float effectSpawnChance = 1f;
    
        // 监视塔相关
        [Header("Monitor Settings")]
        public bool affectedByMonitor = false;
        public float monitorBonusMultiplier = 0.2f;
    
        // 获取实际生产间隔
        public float GetProductionInterval()
        {
            return useRandomInterval ? 
                Random.Range(randomIntervalRange.x, randomIntervalRange.y) : 
                productionInterval;
        }
    
        // 计算总产出量
        public int CalculateTotalOutput(int assignedHeroCount, bool isSupervised)
        {
            float output = baseOutputAmount;
        
            if (canAssignHeroes)
            {
                output += baseOutputAmount * (heroOutputMultiplier * assignedHeroCount);
            }
        
            if (affectedByMonitor && isSupervised)
            {
                output *= (1f + monitorBonusMultiplier);
            }
        
            return Mathf.FloorToInt(output);
        }
    }

    public enum ResourceType
    {
        Gold,
        Stone,
        MagicPower,
        Material,
        ResearchPoints,
        None
    }
}
