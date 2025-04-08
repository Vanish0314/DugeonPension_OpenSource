using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using GameFramework;
using System.Threading.Tasks;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dungeon.SkillSystem
{
    public class SkillShooterLayer
    {
        public LayerMask Layer { get; set; } = HeroTeamLayer;
        public static readonly LayerMask HeroTeamLayer = LayerMask.GetMask("Hero");
        public static readonly LayerMask MonsterTeamLayer = LayerMask.GetMask("Monster");
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
    }
    public class SkillShooter : MonoBehaviour
    {
        public SkillShooterLayer mSelfShooterLayer;

        //TODO (vanish): Register skills could fire to control whether a skill is colded down or not.

        void Start()
        {
            var layer = LayerMask.LayerToName(gameObject.layer);
            mSelfShooterLayer = new SkillShooterLayer(layer);
            owner = GetComponent<ICombatable>();
#if UNITY_EDITOR
            if (owner == null)
            {
                GameFrameworkLog.Error("[SkillShooter] owner is null,which is not allowed");
            }
#endif
        }

        public bool CouldFire()
        {
            return isReadyToFire;
        }
        public void Fire(Skill skillToFire)
        {
            if (!isReadyToFire)
            {
                GameFrameworkLog.Info("[SkillShooter] SkillShooter is not ready to fire");
                return;
            }

            isReadyToFire = false;
            StartCoroutine(DelayFire(skillToFire.skillData.preCastTimeInSec, skillToFire));

            Task.Run(async () =>
            {
                await Task.Delay(System.TimeSpan.FromSeconds(skillToFire.skillData.preCastTimeInSec));
                await Task.Delay(System.TimeSpan.FromSeconds(skillToFire.skillData.midCastTimeInSec));
                await Task.Delay(System.TimeSpan.FromSeconds(skillToFire.skillData.postCastTimeInSec));

                isReadyToFire = true;
            });
        }
        public void Fire(SkillData skillData, Vector3 posOrDirToUseSkill)
        {
            var method = SkillDeployMethod.CreateSkillDeployMethod(skillData, this, posOrDirToUseSkill);
            var skill = new Skill(skillData, method, owner);

            Fire(skill);
        }

        private IEnumerator DelayFire(float delayTime, Skill skillToFire)
        {
            yield return new WaitForSeconds(delayTime);

            var skillGo = Instantiate(skillToFire.SkillGO)?.GetComponent<SkillEntity>();
#if UNITY_EDITOR
            if (skillGo == null)
            {
                GameFrameworkLog.Error("SkillShooter.Fire: skillGo is null");
            }
#endif
            skillGo.InitAndFire(skillToFire);
        }
        private bool isReadyToFire = true;
        private ICombatable owner;

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;
            if (mSelfShooterLayer == null || owner == null) return;

            Handles.color = mSelfShooterLayer.Layer == SkillShooterLayer.HeroTeamLayer ? Color.cyan : Color.red;
            Gizmos.color = new Color(1f, 0.5f, 0f, 1f); // 橙色

            var mockTargetPos = transform.position + transform.forward * 3f;

            Gizmos.DrawLine(transform.position, mockTargetPos);

            Gizmos.DrawSphere(transform.position, 0.2f);

            Gizmos.DrawWireSphere(mockTargetPos, 1.5f);

            Handles.Label(transform.position + Vector3.up * 1.5f, $"Shooter Layer: {LayerMask.LayerToName(gameObject.layer)}");
        }
#endif
    }


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