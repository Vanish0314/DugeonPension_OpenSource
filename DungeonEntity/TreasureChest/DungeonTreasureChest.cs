using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using Dungeon.DungeonGameEntry;
using Dungeon.Vision2D;
using GameFramework;
using UnityEngine;

namespace Dungeon.DungeonEntity.InteractiveObject
{
    public class StandardDungeonTreasureChest : DungeonVisibleEntity
    {
        public Sprite unOpenedSprite;
        public Sprite openedSprite;
        private SpriteRenderer spriteRenderer;

#if UNITY_EDITOR
        protected override void OnDestroy()
        {
            GameFrameworkLog.Warning("[DungeonTreasureChestBase] 请不要Destroy DungeonTreasureChestBase,而使用对象池");
            DungeonGameEntry.DungeonGameEntry.DungeonEntityManager.UnregisterDungeonEntity(this);
        }
#endif
        private bool isOpened = false;
        public bool IsOpened() => isOpened;

        public void Open(IAgentLowLevelSystem agent)
        {
            isOpened = true;
            spriteRenderer.sprite = openedSprite;
        }
        public override VisitInformation OnUnvisited(VisitInformation visiter)
        {
            if (isOpened)
                return visiter;

            visiter.visited = this.gameObject;
            return visiter;
        }

        public override VisitInformation OnVisited(VisitInformation visiter)
        {
            if (isOpened)
                return visiter;

            visiter.visited = this.gameObject;
            return visiter;
        }

        public override void OnSpawn(object data)
        {
            
        }

        public override void Reset()
        {
            isOpened = false;
        }

        public override void OnReturnToPool()
        {

        }

        protected override void OnEnable()
        {
            DungeonGameEntry.DungeonGameEntry.DungeonEntityManager.RegisterDungeonEntity(this);

            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = unOpenedSprite;
        }

        protected override void OnDisable()
        {
            DungeonGameEntry.DungeonGameEntry.DungeonEntityManager.UnregisterDungeonEntity(this);
        }
    }
}
