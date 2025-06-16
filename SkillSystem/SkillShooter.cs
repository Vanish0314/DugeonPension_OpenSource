using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using GameFramework;
using System.Threading.Tasks;
using DG.Tweening;
using System;
using Dungeon.GameFeel;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dungeon.SkillSystem
{
    public class SkillShooterLayer
    {
        public SkillShooterLayer(string layer, string name = "unknown")
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
                GameFrameworkLog.Error("[SkillShooterLayer] Invalid layer name: " + layer + " 名字: " + name);
            }
        }

        public static SkillShooterLayer GetLayer(SkillShooterLayer layer)
        {
            if (layer.Layer == HeroTeamLayer)
            {
                return new SkillShooterLayer("Hero");
            }
            else if (layer.Layer == MonsterTeamLayer)
            {
                return new SkillShooterLayer("Monster");
            }
            else
            {
                throw new System.Exception("[SkillShooterLayer] Invalid layer name: " + layer.Layer);
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
            mSelfShooterLayer = new SkillShooterLayer(layer, gameObject.name);

            owner = GetComponent<ICombatable>();
        }

        public void Fire(Skill skill)
        {
            if (!isReadyToFire) return;

            GameFrameworkLog.Info($"[SkillShooter] {owner.GetGameObject().name} 准备使用技能: {skill.skillData.name}");

            bool SequenceIsEndedByKillFlag = true;
            isReadyToFire = false;
            currentSkillName = skill.skillData.name;

            float pre = skill.skillData.preCastTimeInSec;
            float mid = skill.skillData.midCastTimeInSec;
            float post = skill.skillData.postCastTimeInSec;

            currentSequence = DOTween.Sequence();

            // 1. play effect
            currentSequence.AppendCallback(() =>
            {
                Audio.Instance.PlayAudio(skill.skillData.soundEffect);

                if (skill.skillData.deployMethodDesc.shootType != SkillDeployDesc.SkillShootType.Bullet)
                {
                    var hitboxEffect = Instantiate(skill.skillData.effectPrefab, skill.skillDeployMethod.SkillPosition, Quaternion.identity);
                    hitboxEffect.GetComponent<SpriteVisualEffectController>().Play();
                }
            });

            // 2. pre cast
            currentSequence.AppendInterval(pre);

            // 3. trigger skill entity
            currentSequence.AppendCallback(() =>
            {
                GameFrameworkLog.Info($"[SkillShooter] {owner.GetGameObject().name} 技能前摇结束, 开始施放技能: {skill.skillData.name}");

                currentSkillEntity = Instantiate(skill.SkillGO).GetComponentInChildren<SkillEntity>();
#if UNITY_EDITOR
                if (currentSkillEntity == null)
                {
                    GameFrameworkLog.Error($"[SkillShooter] {owner.GetGameObject().name} 技能实体为空, 无法施放技能: {skill.skillData.name}");
                }
#endif
                currentSkillEntity.InitAndFire(skill);
            });

            // 4. mid cast
            currentSequence.AppendInterval(mid);

            // 5. return skill entity
            currentSequence.AppendCallback(() =>
            {
                GameFrameworkLog.Info($"[SkillShooter] {owner.GetGameObject().name} 技能中摇结束, 进入技能后摇: {skill.skillData.name}");

                if (currentSkillEntity != null && skill.skillData.deployMethodDesc.shootType != SkillDeployDesc.SkillShootType.Bullet)
                {
                    currentSkillEntity.ReturnToPool();
                    currentSkillEntity = null;
                }
            });

            // 6. post cast
            currentSequence.AppendInterval(post);

            currentSequence.OnComplete(() =>
            {
                isReadyToFire = true;
                currentSequence = null;
                currentSkillName = null;

                SequenceIsEndedByKillFlag = false;

                GameFrameworkLog.Info($"[SkillShooter] {owner.GetGameObject().name} 技能后摇结束, 技能: {skill.skillData.name} 释放完毕.");

                OnSkillEnded?.Invoke();
            });

            currentSequence.OnKill(() =>
            {
                if (!SequenceIsEndedByKillFlag) //TODO(vanish): 必须保证OnComplete先于OnKill执行
                    return;

                if (currentSkillEntity != null)
                {
                    currentSkillEntity.ReturnToPool();
                    currentSkillEntity = null;
                }

                isReadyToFire = true;
                currentSkillName = null;

                GameFrameworkLog.Info($"[SkillShooter] {owner.GetGameObject().name} 技能被打断, 技能: {skill.skillData.name} 结束释放.");
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
#if UNITY_EDITOR
            if (skillData == null)
            {
                GameFrameworkLog.Error("[SkillShooter] 传入的技能数据为空.请检查是否是没有配置技能\n物体:" + name);
                return;
            }
#endif

            var method = SkillDeployMethod.CreateSkillDeployMethod(skillData, this, posToUseSkill, dirToUseSkill);
            var skill = new Skill(skillData, method, owner);
            Fire(skill);
        }
        public event Action OnSkillEnded;
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
