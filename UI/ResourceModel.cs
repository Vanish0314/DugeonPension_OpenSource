using System;
using System.Collections;
using System.Collections.Generic;
using DungoenProcedure;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class ResourceModel : MonoBehaviour
    {
        public static ResourceModel Instance { get; private set; }
        
        public event Action<float> OnGoldChanged;
        public event Action CannotAffordGold; 
        public event Action<float> OnExpBallChanged;
        public event Action CannotAffordExpBall;
        public event Action<float> OnStoneChanged;
        public event Action CannotAffordStone; 
        public event Action<float> OnWoodChanged;
        public event Action CannotAffordWood;
        public event Action<float> OnMagicPowerChanged;
        public event Action CannotAffordMagicPower; 
        public event Action<float> OnMaterialChanged;
        public event Action CannotAffordMaterial;
        public event Action<float> OnCursePowerChanged;
        public event Action CannotAffordCursePower;

        [SerializeField] private int gold = 0;
        public int Gold
        {
            get => gold;
            set
            {
                float change = value - gold;
                gold = value;
                OnGoldChanged?.Invoke(change);    
            }
        }
        
        [SerializeField] private int expBall = 10;
        public int ExpBall
        {
            get => expBall;
            set
            {
                float change = value - expBall;
                expBall = value;
                OnExpBallChanged?.Invoke(change);
            }
        }

        [SerializeField] private int stone = 0;
        public int Stone
        {
            get => stone;
            set
            {
                float change = value - stone;
                stone = value;
                OnStoneChanged?.Invoke(change);
            }
        }
        
        [SerializeField] private int wood = 0;
        public int Wood
        {
            get => wood;
            set
            {
                float change = value - wood;
                wood = value;
                OnWoodChanged?.Invoke(change);
            }
        }

        [SerializeField] private int magicPower = 0;
        public int MagicPower
        {
            get => magicPower;
            set
            {
                float change = value - magicPower;
                magicPower = value;
                OnMagicPowerChanged?.Invoke(change);
            }
        }

        [SerializeField] private int material = 0;
        public int Material
        {
            get => material;
            set
            {
                float change = value - material;
                material = value;
                OnMaterialChanged?.Invoke(change);
            }
        }
        
        [SerializeField] private float cursePower = 0;
        public float CursePower
        {
            get => DungeonGameEntry.DungeonGameEntry.OverloadPower.CurrentCursePower;
            set
            {
                float change = value - cursePower;
                cursePower = value;
                OnCursePowerChanged?.Invoke(change);
            }
        }

        private void Awake() {
            if (Instance == null) Instance = this;

            InitializeResources(); // 初始化种子存储
        }
        
        // 新增方法：检查是否有足够资源
        public bool HasEnoughResources(Cost cost)
        {
            bool hasEnoughResources = true;
            if (gold < cost.gold)
            {
                hasEnoughResources = false;
                CannotAffordGold?.Invoke();
            }

            if (expBall < cost.expBall)
            {
                hasEnoughResources = false;
                CannotAffordExpBall?.Invoke();
            }

            if (stone < cost.stone)
            {
                hasEnoughResources = false;
                CannotAffordStone?.Invoke();
            }

            if (wood < cost.wood)
            {
                hasEnoughResources = false;
                CannotAffordWood?.Invoke();
            }

            if (magicPower < cost.magicPower)
            {
                hasEnoughResources = false;
                CannotAffordMagicPower?.Invoke();
            }

            if (material < cost.trapMaterial)
            {
                hasEnoughResources = false;
                CannotAffordMaterial?.Invoke();
            }

            if (cursePower < cost.conjuration)
            {
                hasEnoughResources = false;
                CannotAffordCursePower?.Invoke();
            }
            
            return hasEnoughResources;
        }

        // 新增方法：安全消费资源
        public bool TryConsumeResources(Cost cost)
        {
            if (!HasEnoughResources(cost))
                return false;

            Gold -= cost.gold;
            Stone -= cost.stone;
            Wood -= cost.wood;
            ExpBall -= cost.expBall;
            MagicPower -= cost.magicPower;
            Material -= cost.trapMaterial;
            cursePower -= cost.conjuration;
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
                case ResourceType.Material:
                    Material += amount;
                    break;
                case ResourceType.ExpBall:
                    ExpBall += amount;
                    break;
                default:
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
        
        [Serializable]
        public class ResourceIcon
        {
            public ResourceType resourceType;
            public Sprite icon;
            public RectTransform iconTransform;
        }
        [Serializable]
        public class CropIcon
        {
            public CropType cropType;
            public Sprite icon;
            public RectTransform iconTransform;
        }
        
        [Header("资源图标")]
        [SerializeField] private ResourceIcon[] resourceIcons;
        [Header("种子图标")]
        [SerializeField] private CropIcon[] cropIcons;

        public Sprite GetResourceSprite(string resourceName)
        {
            foreach (var icon in resourceIcons)
            {
                if (icon.resourceType.ToString() == resourceName)
                    return icon.icon;
            }
            return null;
        }

        public Sprite GetCropSprite(string cropName)
        {
            foreach (var icon in cropIcons)
            {
                if (icon.cropType.ToString() == cropName)
                    return icon.icon;
            }
            return null;
        }

        public Vector2 GetResourceTransform(string resourceName)
        {
            foreach (var icon in resourceIcons)
            {
                if (icon.resourceType.ToString() == resourceName)
                    // 获取UI元素在屏幕空间中的位置
                    return RectTransformUtility.WorldToScreenPoint(
                        GatherEffectHelper.Instance.mainCanvas.worldCamera,
                        icon.iconTransform.position);
            }
            return Vector2.zero;
        }

        public void SetRectTransform(ResourceType type,RectTransform rectTransform)
        {
            foreach (var icon in resourceIcons)
            {
                if(icon.resourceType == type)
                    icon.iconTransform = rectTransform;
            }
        }
    }
    
    public enum ResourceType
    {
        Gold,
        Stone,
        Wood,
        ExpBall,
        MagicPower,
        Material,
        None
    }
}
