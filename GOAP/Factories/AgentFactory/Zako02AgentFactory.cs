using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class Zako02AgentFactory : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {

            var builder = new AgentTypeBuilder(AgentGoapType.Zako02.ToString());

            builder.AddCapability<Zako02CapabilityFactory>();
            builder.AddCapability<ZakuCapabilityFactory>();
            builder.AddCapability<UseSkillCapabilityFactory>();
            

            return builder.Build();

        }
    }
}
