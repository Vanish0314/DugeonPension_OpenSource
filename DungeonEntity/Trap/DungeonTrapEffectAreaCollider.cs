using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Dungeon.Character;
using GameFramework;
using UnityEngine;

namespace Dungeon.DungeonEntity
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class DungeonTrapEffectAreaCollider : MonoBehaviour
    {
        private DungeonTrapBase trap;
        private List<BoxCollider2D> boxes = new();
        private Dictionary<Collider2D, int> map = new();
        private bool isOnceUsed;
        private float trapSkillColdDownTime;
        private bool isUsingSkill = false;
        private float timer = 0.0f;

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

            isOnceUsed = trapBase.IsOnceTrap;
            trapSkillColdDownTime = trapBase.skill.cooldownTimeInSec;
        }


        void FixedUpdate()
        {
            if (isOnceUsed)
                return;

            timer -= Time.fixedDeltaTime;

            if (timer <= 0)
            {
                timer = trapSkillColdDownTime;
                List<Collider2D> targets = new(map.Keys);
                foreach (var col in targets)
                {
                    if (col == null) continue;

                    var low = col.GetComponent<AgentLowLevelSystem>();
                    if (low != null && !low.IsDisarmingTrap(trap))
                    {
                        trap.OnEffectEnter(low);
                    }
                }
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            var low = collision.GetComponent<AgentLowLevelSystem>();
            if (low == null)
            {
                GameFrameworkLog.Error($"[DungeonTrapBase] 非勇者对象进入陷阱: {collision.gameObject.name}, 层级:{LayerMask.LayerToName(collision.gameObject.layer)}");
                return;
            }

            if (!low.IsAlive())
                return;

            if (!map.ContainsKey(collision))
            {
                map[collision] = 1;

                if (isOnceUsed)
                {
                    TryExcuteTrap(low);
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

        private void TryExcuteTrap(AgentLowLevelSystem low)
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
