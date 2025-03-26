using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Dungeon.GOAP.Keys.WorldKeys.Local
{
    public class LocalHeroPropertyPointOf<T> : WorldKeyBase where T : IProperty
    {

    }

    public interface IProperty{}
    public interface IHealthPointProperty : IProperty{}
    public interface IMagicPointProperty : IProperty{}
}
