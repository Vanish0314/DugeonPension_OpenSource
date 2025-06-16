using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.GridSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class RoomLimitForm : UGuiForm
    {
        [SerializeField] private Text magicPowerLimit;
        [SerializeField] private Text magicPowerCurrent;
        [SerializeField] private Text materialLimit;
        [SerializeField] private Text materialCurrent;

        public void UpdateForm(DungeonRoom room)
        {
            if (room == null)
                return;
            var roomCapacity = room.GetCapacity();
            magicPowerLimit.text = "/" + roomCapacity.maxMagic;
            magicPowerCurrent.text = roomCapacity.currentMagic.ToString();
            materialLimit.text = "/" + roomCapacity.maxMaterial;
            materialCurrent.text = roomCapacity.currentMaterial.ToString();
        }
    }
}
