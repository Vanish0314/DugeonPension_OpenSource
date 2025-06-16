using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class TrapFactoryUI : MonsterLairUI
    {
        protected override void OpenUpgradeUI()
        {
            GameEntry.UI.OpenUIForm(EnumUIForm.TrapBlueprintUpgradeForm);
        }
    }
}
