using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.DungeonGameEntry
{
    public partial class DungeonGameEntry : MonoBehaviour
    {
        private void InitUniversalSystem()
        {
            PlaceManager.Instance.Initialize();
            
            HPManager.Instance.Initialize();
            
            CursesManager.Instance.Initialize();
            
            TechnologyTreeManager.Instance.Initialize();
            
            BuildModel.Instance.Initialize();
            
            MetropolisHeroManager.Instance.Initialize(null);
            
            MetropolisBuildingManager.Instance.Initialize(null);
            
            MetropolisHeroTransverter.Instance.Initialize();
            
            MetropolisHPManager.Instance.Initialize();
        }
    }
}
