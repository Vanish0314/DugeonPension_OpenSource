using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.DungeonGameEntry;
using UnityEngine;

namespace Dungeon.GOAP
{
    public class LocalDistanceToMainHeroKeySensor : LocalWorldSensorBase
    {
        public override void Created()
        {
        }

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            var mainHero = DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentMainHero();
            var low = references.GetCachedComponent<Transform>();

            if (mainHero == null || low == null)
            {
                return 0;
            }
            else
            { 
                var distance = DungeonGameEntry.DungeonGameEntry.GridSystem.GetGridDistance(low.position, mainHero.transform.position);
                return distance;
            }
        }

        public override void Update()
        {
        }
    }
}
