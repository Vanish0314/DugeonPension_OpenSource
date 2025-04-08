using System;
using Dungeon.SkillSystem;
using Dungeon.SkillSystem.SkillEffect;
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

        public static SkillDeployMethod CreateSkillDeployMethod(SkillData skillData, SkillShooter user, Vector3 posOrDirToUseSkill)
        {
            var desc = skillData.deployMethodDesc;
            var result = new SkillDeployMethod();

            switch (desc.shootType)
            {
                case SkillDeployDesc.SkillShootType.Directional:
                    {
                        result.SkillPosition = user.transform.position;
                        result.SkillDirection = posOrDirToUseSkill.normalized;
                        break;
                    }
                case SkillDeployDesc.SkillShootType.Point:
                    {
                        result.SkillPosition = posOrDirToUseSkill;
                        result.SkillDirection = (posOrDirToUseSkill - user.transform.position).normalized;
                        break;
                    }
                case SkillDeployDesc.SkillShootType.Origin:
                    {
                        result.SkillPosition = user.transform.position;
                        result.SkillDirection = Vector3.up;
                        break;
                    }
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
            result.SkillRangeScaler = desc.range;

            return result;
        }
    }

    public readonly struct Skill
    {
        public readonly ICombatable attacker;
        public readonly SkillData skillData;
        public readonly SkillDeployMethod skillDeployMethod;
        public readonly GameObject SkillGO => skillData.deployMethodDesc.hitBoxPrefab;
        public void FuckMe(ICombatable target)
        {
            var calculator = new SkillCalculator(attacker, target, skillDeployMethod);

            foreach (var effect in skillData.skillEffects)
            {
                effect.Fuck(calculator);
            }

            calculator.Calculate();
        }

        public Skill(SkillData skillData, SkillDeployMethod skillDeployMethod, ICombatable attacker)
        {
            this.attacker = attacker;
            this.skillData = skillData;
            this.skillDeployMethod = skillDeployMethod;
        }
    }



}
