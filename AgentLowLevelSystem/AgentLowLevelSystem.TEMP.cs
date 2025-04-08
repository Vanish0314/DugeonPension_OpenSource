using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using Dungeon.DungeonEntity.InteractiveObject;
using Dungeon.DungeonEntity.Monster;
using Dungeon.DungeonEntity.Trap;
using Dungeon.Vision2D;
using GameFramework;
using PlasticPipe.Tube;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    /// <summary>
    /// 由于还没有完全想好该如何存储Hero的记忆
    /// 主要是如，最近的火把，最近的陷阱等
    /// 黑板暂时还没有很好支持Transform
    /// 而且要支持又会稍显麻烦
    /// 暂时先将需要的功能一股脑写这里
    /// 日后再进行重构
    /// </summary>
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem,ICombatable
    {
        public bool GetNearestTorch(out Transform torchTransform)
        {
            Collider2D[] colliders = new Collider2D[100];
            Physics2D.OverlapCircleNonAlloc(transform.position, m_Viewer.radius, colliders);
            foreach (Collider2D collider in colliders)
            {
                if (collider == null)
                    break;

                if (collider.GetComponent<Torch>() != null)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, collider.transform.position - transform.position,
                        (collider.transform.position - transform.position).magnitude, m_Viewer.blockLayerMask);
                    if (hit.collider != null)
                        continue;

                    if (collider.GetComponent<Torch>().IsLightining())
                        continue;

                    torchTransform = collider.transform;
                    return true;
                }
            }

            torchTransform = null;
            return false;
        }

        public bool GetNearestTrap(out Transform trapTransform)
        {
            Collider2D[] colliders = new Collider2D[100];
            Physics2D.OverlapCircleNonAlloc(transform.position, m_Viewer.radius, colliders);
            foreach (Collider2D collider in colliders)
            {
                if (collider == null)
                    break;

                if (collider.GetComponent<DungeonTrapBase>() != null)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, collider.transform.position - transform.position,
                        (collider.transform.position - transform.position).magnitude, m_Viewer.blockLayerMask);
                    if (hit.collider != null)
                        continue;

                    var visit = collider.GetComponent<IVisible>().OnVisited(new VisitInformation(gameObject));

                    if (visit.visited == null)
                        continue;

                    GameFrameworkLog.Warning("[AgentLowLevelSystem.TEMP] 临时方法额外进行了一次检查，可能导致没被看见的陷阱被认为是可见的");

                    trapTransform = collider.transform;
                    return true;
                }
            }

            trapTransform = null;
            return false;
        }

        public bool GetNearestTreasureChest(out Transform treasureChestTransform)
        {
            Collider2D[] colliders = new Collider2D[100];
            Physics2D.OverlapCircleNonAlloc(transform.position, m_Viewer.radius, colliders);
            foreach (Collider2D collider in colliders)
            {
                if (collider == null)
                    break;

                if (collider.GetComponent<DungeonTreasureChest>() != null)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, collider.transform.position - transform.position,
                        (collider.transform.position - transform.position).magnitude, m_Viewer.blockLayerMask);
                    if (hit.collider != null)
                        continue;

                    var visit = collider.GetComponent<IVisible>().OnVisited(new VisitInformation(gameObject));

                    if (visit.visited == null)
                        continue;

                    GameFrameworkLog.Warning("[AgentLowLevelSystem.TEMP] 临时方法额外进行了一次检查，可能导致没被看见的宝箱被认为是可见的");

                    treasureChestTransform = collider.transform;
                    return true;
                }
            }

            treasureChestTransform = null;
            return false;
        }

        public void GetNearestMonster(out Transform monsterTransform)
        {
            Collider2D[] colliders = new Collider2D[100];
            Physics2D.OverlapCircleNonAlloc(transform.position, m_Viewer.radius, colliders);
            foreach (Collider2D collider in colliders)
            {
                if (collider == null)
                    break;

                if (collider.GetComponent<DungeonMonsterBase>() != null)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, collider.transform.position - transform.position,
                        (collider.transform.position - transform.position).magnitude, m_Viewer.blockLayerMask);
                    if (hit.collider != null)
                        continue;

                    var visit = collider.GetComponent<IVisible>().OnVisited(new VisitInformation(gameObject));

                    if (visit.visited == null)
                        continue;

                    GameFrameworkLog.Warning("[AgentLowLevelSystem.TEMP] 临时方法额外进行了一次检查，可能导致没被看见的怪物被认为是可见的");

                    monsterTransform = collider.transform;
                    return;
                }
            }

            monsterTransform = null;
        }

    }
}
