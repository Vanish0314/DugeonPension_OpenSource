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
                case ResourceType.MagicPower:
                    MagicPower += amount;
                    break;
                case ResourceType.TrapMaterial:
                    Material += amount;
                    break;
            }
        }
    }
    
    public enum ResourceType
    {
        Gold,
        Stone,
        Wood,
        ResearchPoints,
        FoodMaterial,
        ExpBall,
        MagicPower,
        TrapMaterial,
        Conjuration,
        None
    }
}
