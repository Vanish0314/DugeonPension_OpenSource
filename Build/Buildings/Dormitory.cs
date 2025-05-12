using UnityEngine;

namespace Dungeon
{
    public class Dormitory : MetropolisBuildingBase
    {
        [Header("宿舍设置")]
        [SerializeField] private int maxResidents = 4;
        [SerializeField] private int currentResidents;

        public bool CanAcceptResident()
        {
            return currentResidents < maxResidents;
        }

        public void CheckIn(MetropolisHeroBase hero)
        {
            currentResidents++;
            Debug.Log($"{hero.name} 入住 {name}");
        }

        public void CheckOut(MetropolisHeroBase hero)
        {
            currentResidents = Mathf.Max(0, currentResidents - 1);
            Debug.Log($"{hero.name} 离开 {name}");
        }
    }
}