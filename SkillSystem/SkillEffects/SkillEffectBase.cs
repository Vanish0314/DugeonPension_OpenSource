using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.SkillSystem.SkillEffect
{
    public abstract class SkillEffectBase : ScriptableObject
    {
        public abstract void Fuck(ICombatable target,SkillDeployMethod deployDesc);
    }

    [Serializable]
    public class Damage
    {
        public NDX value;

        public int Claculate(ICombatable target) => value.Claculate();
    }
}
