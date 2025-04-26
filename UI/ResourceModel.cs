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
    }
}
