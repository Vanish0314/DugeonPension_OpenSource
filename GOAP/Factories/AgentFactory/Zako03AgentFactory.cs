using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class Zako03AgentFactory : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {

            var builder = new AgentTypeBuilder(AgentGoapType.Zako03.ToString());

            builder.AddCapability<Zako03CapabilityFactory>();
            builder.AddCapability<ZakuCapabilityFactory>();
            builder.AddCapability<UseSkillCapabilityFactory>();
            builder.AddCapability<HealingSpellSkillFactory>();
            builder.AddCapability<HelpTeammatesCapabilityFactory>();
  
            return builder.Build();

        }
    }
}
