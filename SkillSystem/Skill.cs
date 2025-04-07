using System;
using Dungeon.SkillSystem;
using GameFramework;
using UnityEngine;

namespace Dungeon
{
    [Serializable]
    public struct SkillDeployMethod
    {
        public Vector3 SkillPosition;
        public Vector3 SkillDirection;
        public float SkillRangeScaler;

        public LayerMask SkillLayerToShootMask; // Layer to use skill on

        public static SkillDeployMethod CreateSkillDeployMethod(SkillData skillData,SkillShooter user,Vector3 posOrDirToUseSkill)
        {
            var desc = skillData.deployMethodDesc;
            var result = new SkillDeployMethod();

            if(desc.shootType == SkillDeployDesc.SkillShootType.Directional)
            {
                result.SkillPosition = user.transform.position;
                result.SkillDirection = posOrDirToUseSkill.normalized;
            }
            else
            {
                result.SkillPosition = posOrDirToUseSkill;
                result.SkillDirection = (posOrDirToUseSkill - user.transform.position).normalized;
            }

            switch (desc.aoeType)
            {
                case SkillDeployDesc.SkillAoeType.Single:
                    GameFrameworkLog.Warning("Single AOE not implemented yet");
                    break;
                case SkillDeployDesc.SkillAoeType.Area:
                    GameFrameworkLog.Warning("Single AOE not implemented yet");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var layer = SkillShooterLayer.GetOppsiteLayer(user.mSelfShooterLayer).Layer;
            result.SkillLayerToShootMask = layer;

            return result;
        }
    }

    public readonly struct Skill
    {        
        public readonly SkillData skillData;
        public readonly SkillDeployMethod skillDeployMethod;
        public readonly GameObject SkillGO => skillData.deployMethodDesc.hitBoxPrefab;
        public void FuckMe(ICombatable target)
        {
            foreach (var effect  in skillData.skillEffects)
            {
                effect.Fuck(target,skillDeployMethod);
            }
        }

        public Skill(SkillData skillData, SkillDeployMethod skillDeployMethod)
        {
            this.skillData = skillData;
            this.skillDeployMethod = skillDeployMethod;
        }

        
    }

}
