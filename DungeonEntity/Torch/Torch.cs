using System.Collections;
using System.Collections.Generic;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon.DungeonEntity.InteractiveObject
{
    public class Torch : DungeonVisibleEntity
    {
        public Sprite LightTorch;
        public Sprite DarkTorch;
        public bool IsLit;
        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            IsLit = false;
            spriteRenderer.sprite = DarkTorch;
        }
        private void Update() {
            if (IsLit)
            {
                spriteRenderer.color = new Color(1, 1, 1, 1);
                spriteRenderer.sprite = LightTorch;
            }
            else
            {
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                spriteRenderer.sprite = DarkTorch;
            }
        }

        public void LightUp() => IsLit = true;
        public void TurnOff() => IsLit = false;
        public bool IsLightining() => IsLit;
        public override VisitInformation OnUnvisited(VisitInformation visiter)
        {
            if(IsLit)
                return visiter;

            visiter.visited = this.gameObject;
            return visiter;
        }

        public override VisitInformation OnVisited(VisitInformation visiter)
        {
            if (IsLit)
                return visiter;

            visiter.visited = this.gameObject;
            return visiter;
        }
    }
}
