using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class HPModel : MonoBehaviour
    {
        public static HPModel Instance { get; private set; }
        
        public event Action OnHPChanged;

        private float hp = 0;
        public float HP
        {
            get => hp;
            set
            {
                hp = value;
                OnHPChanged?.Invoke();
            }
        }

        private void Awake() {
            if (Instance == null) Instance = this;
        }
    }
}
