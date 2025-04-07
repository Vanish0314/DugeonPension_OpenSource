using UnityEngine;

namespace Dungeon.SkillSystem
{
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

            var deploy = skill.skillDeployMethod;

            {
                transform.parent = null;
                transform.position = deploy.SkillPosition + new Vector3(0.5f, 0f, 0f);
                transform.rotation = Quaternion.Euler(Vector3.zero);//TODO
                transform.localScale *= deploy.SkillRangeScaler;
            }

            {
                LayerToShoot = deploy.SkillLayerToShootMask.value;
                this.skill = skill;
            }

            {
                m_AliveTime = skill.skillData.midCastTimeInSec;
            }
        }

        void Update()
        {
            m_AliveTime -= Time.deltaTime;
            if (m_AliveTime <= 0)
            {
                Destroy(gameObject);
            }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer)&LayerToShoot) > 0)
            {
                var obj = collision.GetComponent<ICombatable>();
                if (obj != null)
                    obj.TakeSkill(skill);
            }
        }
        public void OTriggerExit2D(Collider2D collision)
        {

        }

        private int LayerToShoot;
        private float m_AliveTime;
        private Skill skill;
    }
}
