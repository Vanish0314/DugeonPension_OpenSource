using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Common;
using Dungeon.Evnents;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Dungeon.DungeonGameEntry
{

    /// <summary>
    /// The GameEntry
    /// Since there already has a GameEntry class in GF
    /// We name our Dungeon Game's GameEntry class as DungeonGameEntry
    /// in which we manage the whole game's state and progress
    /// for the idea case , the entry scene would only have one GO with this class
    /// </summary>
    public partial class DungeonGameEntry : MonoBehaviour
    {
        void Awake()
        {
            CheckValidation();

            InitGameFramwork();

            InitDungeonComponents();

            InitDungeonDebuggers();
         
            InitInstance();

            SubscribleEvents();
        }

        void Start()
        {
            InitGameFramwork(); //Prevent init order issue
            
            InitUniversalSystem();
        }
    }
}
