using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class ResourceModel : MonoBehaviour
    {
        public static ResourceModel Instance { get; private set; }
        
        public event Action OnGoldChanged;
        public event Action OnStoneChanged;
        public event Action OnWoodChanged;
        public event Action OnMagicPowerChanged;
        public event Action OnMaterialChanged;

        private int gold = 0;
        public int Gold
        {
            get => gold;
            set
            {
                gold = value;
                OnGoldChanged?.Invoke();
            }
        }

        private int stone = 0;
        public int Stone
        {
            get => stone;
            set
            {
                stone = value;
                OnStoneChanged?.Invoke();
            }
        }
        
        private int wood = 0;

        public int Wood
        {
            get => wood;
            set
            {
                wood = value;
                OnWoodChanged?.Invoke();
            }
        }

        private int magicPower = 0;
        public int MagicPower
        {
            get => magicPower;
            set
            {
                magicPower = value;
                OnMagicPowerChanged?.Invoke();
            }
        }

        private int material = 0;
        public int Material
        {
            get => material;
            set
            {
                material = value;
                OnMaterialChanged?.Invoke();
            }
        }
        private void Awake() {
            if (Instance == null) Instance = this;

            InitializeResources(); // 初始化种子存储
        }
        
        // 新增方法：检查是否有足够资源
        public bool HasEnoughResources(Cost cost)
        {
            return gold >= cost.gold && 
                   stone >= cost.stone && 
                   magicPower >= cost.magicPower && 
                   material >= cost.trapMaterial;
        }

        // 新增方法：安全消费资源
        public bool TryConsumeResources(Cost cost)
        {
            if (!HasEnoughResources(cost))
                return false;

            Gold -= cost.gold;
            Stone -= cost.stone;
            MagicPower -= cost.magicPower;
            Material -= cost.trapMaterial;
            return true;
        }

        // 收集资源方法
        public void GatherResource(ResourceType resourceType, int amount)
        {
            switch (resourceType)
            {
                case ResourceType.Gold:
                    Gold += amount;
                    break;
                case ResourceType.Stone:
                    Stone += amount;
                    break;
                case ResourceType.Wood:
                    Wood += amount;
                    break;
                case ResourceType.MagicPower:
                    MagicPower += amount;
                    break;
                case ResourceType.TrapMaterial:
                    Material += amount;
                    break;
            }
        }
        
        
        // 新增种子和作物存储
        private Dictionary<CropType, int> seeds = new Dictionary<CropType, int>();
        private Dictionary<CropType, int> crops = new Dictionary<CropType, int>();

        // 种子事件
        public event Action<CropType> OnSeedChanged;
        // 作物事件
        public event Action<CropType> OnCropChanged;
        
        [Header("初始种子")]
        [SerializeField] private CropType[] initialSeeds;
        [SerializeField] private int[] initialSeedCounts;
        
        private void InitializeResources()
        {
            // 初始化字典
            seeds = new Dictionary<CropType, int>();
            crops = new Dictionary<CropType, int>();

            // 加载初始种子
            for (int i = 0; i < initialSeeds.Length; i++)
            {
                if (i < initialSeedCounts.Length)
                {
                    AddSeed(initialSeeds[i], initialSeedCounts[i]);
                }
            }
        }

        #region 种子管理
        public void AddSeed(CropType type, int amount)
        {
            seeds.TryGetValue(type, out int current);
            seeds[type] = current + amount;
            OnSeedChanged?.Invoke(type);
        }

        public bool ConsumeSeed(CropType type, int amount)
        {
            if (GetSeedCount(type) < amount) return false;
            
            seeds[type] -= amount;
            if (seeds[type] <= 0) seeds.Remove(type);
            OnSeedChanged?.Invoke(type);
            return true;
        }

        public int GetSeedCount(CropType type)
        {
            return seeds.TryGetValue(type, out int count) ? count : 0;
        }
        #endregion

        #region 作物管理
        public void AddCrop(CropType type, int amount)
        {
            crops.TryGetValue(type, out int current);
            crops[type] = current + amount;
            OnCropChanged?.Invoke(type);
        }

        public bool ConsumeCrop(CropType type, int amount)
        {
            if (GetCropCount(type) < amount) return false;

            crops[type] -= amount;
            if (crops[type] <= 0) crops.Remove(type);
            OnCropChanged?.Invoke(type);
            return true;
        }

        public int GetCropCount(CropType type)
        {
            return crops.TryGetValue(type, out int count) ? count : 0;
        }
        #endregion
    }
    
    public enum ResourceType
    {
        Gold,
        Stone,
        Wood,
        ResearchPoints,
        ExpBall,
        MagicPower,
        TrapMaterial,
        Conjuration,
        None
    }
}
