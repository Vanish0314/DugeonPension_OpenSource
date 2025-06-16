using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class KnightAgentFactory : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {
            var builder = new AgentTypeBuilder(AgentGoapType.Knight.ToString());

            builder.AddCapability<KnightCapabilityFactory>();
            builder.AddCapability<TripSlashCapabilityFactory>();
            builder.AddCapability<UseSkillCapabilityFactory>();


            return builder.Build();
        }
    }
}
