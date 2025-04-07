using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Dungeon.GOAP.Enums;
using Dungeon.GOAP.Factories.CapabilityFactory;
using UnityEngine;

namespace Dungeon.GOAP.Factories.AgentFactory
{
    public class HeroAgentFactory : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {
            var builder = new AgentTypeBuilder(AgentTypeIDs.Hero);

            builder.AddCapability<FinishDungeonCapabilityFactory>();

            return builder.Build();
        }
    }
}
