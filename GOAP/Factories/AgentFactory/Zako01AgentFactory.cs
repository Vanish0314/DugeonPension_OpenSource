using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class Zako01AgentFactory : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {
          
            var builder = new AgentTypeBuilder(AgentGoapType.Zako01.ToString());

            builder.AddCapability<Zako01CapabilityFactory>();
            builder.AddCapability<ZakuCapabilityFactory>();
            builder.AddCapability<UseSkillCapabilityFactory>();
            builder.AddCapability<TauntSkillFactory>(); 

            return builder.Build();
        
        }
    }
}
