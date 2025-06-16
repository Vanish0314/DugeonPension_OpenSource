using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
namespace Dungeon.GOAP
{
    public class HeroAgentFactory : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {
            var builder = new AgentTypeBuilder(AgentGoapType.SampleHero.ToString());

            builder.AddCapability<FinishDungeonCapabilityFactory>();
            builder.AddCapability<HelpTeammatesCapabilityFactory>();
            builder.AddCapability<TripSlashCapabilityFactory>();
            builder.AddCapability<HolySlashCapabilityFactory>();
            builder.AddCapability<HealingSpellSkillFactory>();
            builder.AddCapability<FireballSpellSkillFactory>();
            builder.AddCapability<MindShieldSkillFactory>();
            builder.AddCapability<UseSkillCapabilityFactory>();

            return builder.Build();
        }
    }
}
