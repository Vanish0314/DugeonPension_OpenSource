using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using GameFramework;
using System.Threading.Tasks;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dungeon.SkillSystem
{
    public class SkillShooterLayer
    {
        public SkillShooterLayer(string layer)
        {
            if (layer == "Hero")
            {
                Layer = HeroTeamLayer;
            }
            else if (layer == "Monster")
            {
                Layer = MonsterTeamLayer;
            }
            else if (layer == "Trap")
            {
                Layer = MonsterTeamLayer;
            }
            else
            {
                GameFrameworkLog.Error("[SkillShooterLayer] Invalid layer name: " + layer);
            }
        }

        public static SkillShooterLayer GetOppsiteLayer(SkillShooterLayer layer)
        {
            if (layer.Layer == HeroTeamLayer)
            {
                return new SkillShooterLayer("Monster");
            }
            else if (layer.Layer == MonsterTeamLayer)
            {
                return new SkillShooterLayer("Hero");
            }
            else
            {
                throw new System.Exception("[SkillShooterLayer] Invalid layer name: " + layer.Layer);
            }
        }


        public LayerMask Layer { get; set; } = HeroTeamLayer;
        public static readonly LayerMask HeroTeamLayer = LayerMask.GetMask("Hero");
        public static readonly LayerMask MonsterTeamLayer = LayerMask.GetMask("Monster");
    }
    public class SkillShooter : MonoBehaviour
    {
        void Start()
        {
            var layer = LayerMask.LayerToName(gameObject.layer);
            mSelfShooterLayer = new SkillShooterLayer(layer);

            owner = GetComponent<ICombatable>();
        }

        public void Fire(Skill skill)
        {
            if (!isReadyToFire) return;

            isReadyToFire = false;
            currentSkillName = skill.skillData.name;

            float pre = skill.skillData.preCastTimeInSec;
            float mid = skill.skillData.midCastTimeInSec;
            float post = skill.skillData.postCastTimeInSec;

            currentSequence = DOTween.Sequence();
            currentSequence.AppendInterval(pre);
            currentSequence.AppendCallback(() =>
            {
                currentSkillEntity = Instantiate(skill.SkillGO).GetComponentInChildren<SkillEntity>();//FIXME()
                currentSkillEntity.InitAndFire(skill);
            });

            currentSequence.AppendInterval(mid);
            currentSequence.AppendCallback(() =>
            {
                if (currentSkillEntity != null)
                {
                    currentSkillEntity.ReturnToPool();
                    currentSkillEntity = null;
                }
            });

            currentSequence.AppendInterval(post);
            currentSequence.OnComplete(() =>
            {
                isReadyToFire = true;
                currentSequence = null;
                currentSkillName = null;

                GameFrameworkLog.Info("[SkillShooter] Skill casting complete.");
            });

            currentSequence.OnKill(() =>
            {
                if (currentSkillEntity != null)
                {
                    currentSkillEntity.ReturnToPool();
                    currentSkillEntity = null;
                }

                isReadyToFire = true;
                currentSkillName = null;

                GameFrameworkLog.Info("[SkillShooter] Skill was interrupted.");
            });
        }

        public void InterruptSkill()
        {
            if (currentSequence != null && currentSequence.IsActive())
            {
                currentSequence.Kill();
                currentSequence = null;
            }
        }

        public bool CouldFire() => isReadyToFire;

        public void Fire(SkillData skillData, Vector3 posToUseSkill, Vector3 dirToUseSkill)
        {
            var method = SkillDeployMethod.CreateSkillDeployMethod(skillData, this, posToUseSkill, dirToUseSkill);
            var skill = new Skill(skillData, method, owner);
            Fire(skill);
        }
        public void OnStunned()
        {
            isStunned = true;

            if (IsUsingSkill())
            {
                InterruptSkill();
                Debug.Log("[SkillShooter] Interrupted due to stun.");
            }
        }
        public bool IsUsingSkill()
        {
            return currentSequence != null && currentSequence.IsActive();
        }
        public bool IsUsingSkill(string name)
        {
            return IsUsingSkill() && currentSkillName == name;
        }
        public void OnUnStunned() //恢复后不再使用之前未完成的技能
        {
            isStunned = false;
            Debug.Log("[SkillShooter] Unstunned.");
        }

        private ICombatable owner;
        private bool isReadyToFire = true;
        private SkillEntity currentSkillEntity;
        private Sequence currentSequence;
        private string currentSkillName = null;
        private bool isStunned = false;
        private SkillShooterLayer mSelfShooterLayer;

        public Tween CurrentSkillTween => currentSequence;
        public SkillShooterLayer ShooterLayer => mSelfShooterLayer;
    }
}


namespace Dungeon.SkillSystem
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SkillShooter))]
    public class SkillShooterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}