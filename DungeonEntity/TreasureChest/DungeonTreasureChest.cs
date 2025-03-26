using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon
{
    public class DungeonTreasureChest : DungeonVisibleEntity
    {
        public Sprite unOpenedSprite;
        public Sprite openedSprite;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = unOpenedSprite;
        }
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
    }
}
