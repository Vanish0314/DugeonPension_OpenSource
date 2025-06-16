using System;
using Dungeon.GameFeel;
using GameFramework;
using UnityEngine;

namespace Dungeon.SkillSystem
{
    [RequireComponent(typeof(Collider2D),typeof(Rigidbody2D))]
    public class SkillEntity : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Validate()
        {

        }
#endif
        public void InitAndFire(Skill skill)
        {
#if UNITY_EDITOR
            Validate();
#endif
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            GetComponent<Collider2D>().isTrigger = true;

            var deploy = skill.skillDeployMethod;

            transform.parent = null;
            transform.position = deploy.SkillPosition;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            transform.localScale *= deploy.SkillRangeScalers;

            LayerToShoot = deploy.SkillLayerToShootMask.value;
            this.skill = skill;

            m_AliveTime = skill.skillData.midCastTimeInSec;

            var desc = skill.skillData.deployMethodDesc;
            if (desc.shootType == SkillDeployDesc.SkillShootType.Bullet)
            {
                isBullet = true;
                bulletDirection = deploy.SkillDirection.normalized;
                bulletSpeed = skill.skillData.bulletSpeed;
                bulletMaxRange = skill.skillData.bulletRange;
                bulletStartPosition = transform.position;
                m_AliveTime = skill.skillData.bulletLifeTime;

                float angle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        void Update()
        {
            if (isBullet)
            {
                float distanceToMove = bulletSpeed * Time.deltaTime;
                Vector3 moveVector = bulletDirection * distanceToMove;
                transform.position += moveVector;
                bulletTravelledDistance += distanceToMove;

                if (bulletTravelledDistance >= bulletMaxRange)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            m_AliveTime -= Time.deltaTime;
            if (m_AliveTime <= 0)
            {
                Destroy(gameObject);
            }
        }


        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & LayerToShoot) > 0)
            {
                var obj = collision.GetComponent<ICombatable>();
                if (obj != null)
                {
                    OnSkillHitEvent?.Invoke(obj);
                    if (obj.TakeSkill(skill))
                    {
                        OnSkillKilledEvent?.Invoke(obj);
                    }

                    OnSkillHitEvent = null;
                    OnSkillKilledEvent = null;

                    if (skill.skillData.hitEffectPrefab != null)
                    {
                        var hitEffect = Instantiate(skill.skillData.hitEffectPrefab, collision.transform.position, Quaternion.identity);
                        hitEffect.GetComponent<SpriteVisualEffectController>().Play();
                    }

                    if (isBullet)
                    {
                        Destroy(gameObject);
                    }
                }

                GameFrameworkLog.Info($"[SkillEntity] {collision.gameObject.name} 收到了技能 {skill.skillData.skillName}, 攻击者是 {skill.attacker.GetGameObject().name}");
            }
        }

        public void OTriggerExit2D(Collider2D collision)
        {

        }

        public void ReturnToPool()//TODO
        {
            Destroy(gameObject);
        }

        public event Action<ICombatable> OnSkillHitEvent;
        public event Action<ICombatable> OnSkillKilledEvent;

        private int LayerToShoot;
        private float m_AliveTime;
        private Skill skill;

        private bool isBullet = false;
        private Vector3 bulletDirection;
        private float bulletSpeed;
        private float bulletMaxRange;
        private float bulletTravelledDistance = 0f;
        private Vector3 bulletStartPosition;


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (skill.skillData == null)
                return;

            Vector3 attackerPos = skill.attacker.GetGameObject().transform.position;
            Vector3 skillPos = skill.skillDeployMethod.SkillPosition;
            Vector3 direction = skill.skillDeployMethod.SkillDirection.normalized;
            float radius = skill.skillData.deployMethodDesc.radius;

            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawAAPolyLine(4f, attackerPos, attackerPos + direction * radius);

            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawAAPolyLine(3f, attackerPos, skillPos);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackerPos, radius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackerPos, 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(skillPos, 0.2f);

            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(skillPos + Vector3.up * 0.5f,
                $"技能: {skill.SkillName}\n施法者: {skill.attacker.GetGameObject().name}");
        }
#endif
    }
}
