using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class FightSettlementModel : MonoBehaviour
    {
        public static FightSettlementModel Instance { get; private set; }
        
        private void Awake() 
        {
            if (Instance == null) Instance = this;
        }
        
        public void SetCount()
        {
            
        }

        public void ModifyCount()
        {

        }
    }
}