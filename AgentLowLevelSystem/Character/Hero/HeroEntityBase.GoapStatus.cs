using System;
using System.Collections.Generic;
using Dungeon.DungeonEntity;

namespace Dungeon.Character
{
    public class HeroGoapStatus
    {
        private readonly HashSet<object> attackers = new();
        private readonly HashSet<object> positiveBuffSources = new();

        public bool IsBeingAttacked => attackers.Count > 0;
        public bool HasPositiveBuff => positiveBuffSources.Count > 0;

        public void AddAttacker(object source)
        {
            attackers.Add(source);
        }

        public void RemoveAttacker(object source)
        {
            attackers.Remove(source);
        }

        public void AddPositiveBuff(object buffSource)
        {
            positiveBuffSources.Add(buffSource);
        }

        public void RemovePositiveBuff(object buffSource)
        {
            positiveBuffSources.Remove(buffSource);
        }
    }

    public partial class HeroEntityBase : DungeonVisitorEntity
    {
        public HeroGoapStatus GoapStatus { get; private set; } = new();
    }
}
