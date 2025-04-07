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

        void Start()
        {
            var layer = LayerMask.LayerToName(gameObject.layer);
            mSelfShooterLayer = new SkillShooterLayer(layer);
        }

        public bool CouldFire()
        {
            return isReadyToFire;
        }
        public void Fire(Skill skillToFire)
        {
            isReadyToFire = false;
            StartCoroutine(DelayFire(skillToFire.skillData.preCastTimeInSec, skillToFire));

            Task.Run(async () =>{
                await Task.Delay(System.TimeSpan.FromSeconds(skillToFire.skillData.preCastTimeInSec));
                await Task.Delay(System.TimeSpan.FromSeconds(skillToFire.skillData.midCastTimeInSec));
                await Task.Delay(System.TimeSpan.FromSeconds(skillToFire.skillData.postCastTimeInSec));

                isReadyToFire = true;
            });
        }
        public void Fire(SkillData skillData, SkillShooter shooter, Vector3 posOrDirToUseSkill)
        {
            var method = SkillDeployMethod.CreateSkillDeployMethod(skillData, shooter, posOrDirToUseSkill);
            var skill = new Skill(skillData, method);

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

        private bool isReadyToFire;
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