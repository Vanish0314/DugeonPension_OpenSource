using System.Collections;
using System.Collections.Generic;
using Dungeon.DungeonGameEntry;
using Dungeon.Vision2D;
using GameFramework;
using UnityEngine;

namespace Dungeon.DungeonEntity.InteractiveObject
{
    public class StandardTorch : DungeonVisibleEntity
    {
        public Sprite LightTorch;
        public Sprite DarkTorch;
        public bool IsLit;
        private SpriteRenderer spriteRenderer;

#if UNITY_EDITOR
        protected override void OnDestroy()
        {
            GameFrameworkLog.Warning("[Torch] Destroyed 清不要使用Destory而是对象池");
            DungeonGameEntry.DungeonGameEntry.DungeonEntityManager.UnregisterDungeonEntity(this);
        }
#endif
        private void Update()
        {
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
            if (IsLit)
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

        public override void OnSpawn(object data)
        {
            DungeonGameEntry.DungeonGameEntry.DungeonEntityManager.RegisterDungeonEntity(this);
        }
        public override void Reset()
        {
            IsLit = false;
        }

        public override void OnReturnToPool()
        {
            DungeonGameEntry.DungeonGameEntry.DungeonEntityManager.UnregisterDungeonEntity(this);
        }

        protected override void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            IsLit = false;
            spriteRenderer.sprite = DarkTorch;
        }

        protected override void OnDisable()
        {
            
        }
    }
}
