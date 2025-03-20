using System.Collections;
using System.Collections.Generic;
using Dungeon.GridSystem;
using UnityEngine;

namespace Dungeon.DungeonActor
{

    /// <summary>
    /// 注意: 任何在场景中的实例对象都必须是叶子类型!!!
    /// 任何继承此接口的类都必须是DungeonBehavior子类
    /// </summary>
    public interface IDungeonBehavior
    {
    }
    /// <summary>
    /// 注意: 任何在场景中的实例对象都必须是叶子类型!!!
    /// </summary>
    public abstract class DungeonBehavior : MonoBehaviour , IDungeonBehavior
    {
    }
}