using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

namespace Dungeon.GOAP
{
    /// <summary>
    /// 包括自己
    /// </summary>
    public class GlobalHeroTeamHpBelow70PercentageFriendlyHeroCountKeySensor : GlobalWorldSensorBase
    {
        public override void Created()
        {

        }

        public override SenseValue Sense()
        {
            int count = 0;

            DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentGameProgressingHeroTeam().ForEach(
                hero =>
                {
                    if (hero.IsAlive() && hero.GetHp() < hero.GetMaxHp() * 0.7f)
                    {
                        count++;
                    }
                }
            );

            return count;
        }
    }

    public class GolbalHeroTeamBeingAttackedHeroCountKeySensor : GlobalWorldSensorBase
    {
        public override void Created()
        {
        }

        public override SenseValue Sense()
        {
            int count = 0;

            DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentGameProgressingHeroTeam().ForEach(
                hero =>
                {
                    if (hero.GoapStatus.IsBeingAttacked)
                        count++;
                }
            );

            return count;
        }
    }

    public class GlobalHeroTeamDontHasPositiveBuffHeroCountKeySensor : GlobalWorldSensorBase
    {
        public override void Created()
        {

        }

        public override SenseValue Sense()
        {
            int count = 0;

            DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.GetCurrentGameProgressingHeroTeam().ForEach(
                hero =>
                {
                    if (!hero.GoapStatus.HasPositiveBuff)
                        count++;
                }
            );

            return count;
        }
    }
}
