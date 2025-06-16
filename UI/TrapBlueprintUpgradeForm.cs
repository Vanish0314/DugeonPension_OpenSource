using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class TrapBlueprintUpgradeForm : BlueprintUpgradeForm
    {
        protected override void OnReturnButtonClicked()
        {
            GameEntry.UI.GetUIForm(EnumUIForm.TrapBlueprintUpgradeForm).Close();
        }
    }
}
