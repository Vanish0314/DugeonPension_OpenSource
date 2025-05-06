using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameFramework;
using UnityEngine;

namespace Dungeon.DungeonEntity.Trap
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class DungeonTrapEffectAreaCollider : MonoBehaviour
    {
        private DungeonTrapBase trap;
        private List<BoxCollider2D> boxes = new();
        private Dictionary<Collider2D, int> map = new();

        public void Init(List<Vector2Int> area, DungeonTrapBase trapBase)
        {
#if UNITY_EDITOR
            if (LayerMask.NameToLayer("TrapTrigger") == -1)
                GameFrameworkLog.Error("[DungeonTrapEffectAreaCollider] No Collider Layer named 'TrapTrigger'");
#endif
            trap = trapBase;
            gameObject.layer = LayerMask.NameToLayer("TrapTrigger");

            foreach (var pos in area)
            {
                var box = gameObject.AddComponent<BoxCollider2D>();
                box.size = Vector2.one;
                box.offset += (Vector2)pos * Vector2.one;
                box.isTrigger = true;
                boxes.Add(box);
            }

            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!map.ContainsKey(collision))
            {
                map[collision] = 1;
                var lowLevel = collision.GetComponent<AgentLowLevelSystem.AgentLowLevelSystem>();
                if (lowLevel != null)
                {
                    TryExcuteTrap(lowLevel);
                }
                else
                {
                    GameFrameworkLog.Error($"[DungeonTrapBase] 有物体不是勇者,但检测了与陷阱的碰撞,请检查1. 层级设置是否正确, 2. 层级相交规则是否正确. 物体名称:{collision.gameObject.name},所在层级:{LayerMask.LayerToName(collision.gameObject.layer)}");
                }
            }
            else
            {
                map[collision]++;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (map.ContainsKey(collision))
            {
                map[collision]--;
                if (map[collision] <= 0)
                {
                    map.Remove(collision);
                }
            }
        }

        private void TryExcuteTrap(AgentLowLevelSystem.AgentLowLevelSystem low)
        {
#if UNITY_EDITOR
            if (low == null)
            {
                StringBuilder sb = new();
                sb.AppendLine("[DungeonTrapBase] 检测到与非勇者对象的碰撞。这种情况不应该发生");
                sb.AppendLine("请检查是否有非勇者对象被错误地设置到了错误的图层，或者碰撞图层设置有误");
                sb.AppendLine("对象信息:" + low.gameObject.name);
                GameFrameworkLog.Error(sb.ToString());
            }
            else if (!low.IsDisarmingTrap(trap))
            {
                StringBuilder sb = new();
                sb.AppendLine("[DungeonTrapBase] 检测到勇者触发了陷阱");
                sb.AppendLine("勇者名称:" + low.gameObject.name);
                sb.AppendLine("触发陷阱:" + trap.gameObject.name);
                GameFrameworkLog.Info(sb.ToString());
            }
            else
            {
                StringBuilder sb = new();
                sb.AppendLine("[DungeonTrapBase] 检测到勇者触发了陷阱");
                sb.AppendLine("勇者名称:" + low.gameObject.name);
                sb.AppendLine("触发陷阱:" + trap.gameObject.name);
                sb.AppendLine("但勇者正在解除陷阱,无法触发");
                GameFrameworkLog.Info(sb.ToString());
            }
#endif
            if (!low.IsDisarmingTrap(trap))
                trap.OnEffectEnter(low);
        }
    }
}
