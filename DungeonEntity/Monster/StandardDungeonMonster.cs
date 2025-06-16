using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using UnityEngine;

namespace Dungeon.DungeonEntity
{
    public class StandardDungeonMonster : DungeonMonsterBase
    {
        public override void OnReturnToPool()
        {
            base.OnReturnToPool();
            basicInfo.hp = basicInfo.maxHp;
            
            GetComponent<Collider2D>().enabled = true;

            m_Animator.SetBool("isDead", false);

            var bto = GetComponent<BehaviourTreeOwner>();
            if (bto != null) bto.enabled = true;
            
            var sprite = m_Animator.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
        }

        public override void OnSpawn(object data)
        {
            basicInfo.hp = basicInfo.maxHp;
        }

        public override void Reset()
        {
           
        }

        protected override void OnDisable()
        {

        }

        protected override void OnTakeSkill(Skill skill)
        {
        }

        protected override void OnUpdate()
        {
            
        }
    }
}
