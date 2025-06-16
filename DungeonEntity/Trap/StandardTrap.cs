using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using Dungeon.Vision2D;
using GameFramework;
using UnityEngine;

namespace Dungeon.DungeonEntity
{
    public class StandardTrap : DungeonTrapBase
    {
        [Header("作为一个陷阱,下面的内容可以不需要设置")]
        public CombatorData basicInfo;
        public StatusBarSetting statusBarSetting;

        public override int Hp { get => BasicInfo.hp; set => BasicInfo.hp = value; }
        public override int MaxHp { get => BasicInfo.maxHp; set => BasicInfo.maxHp = value; }
        public override int Mp { get => BasicInfo.mp; set => BasicInfo.mp = value; }
        public override int MaxMp { get => BasicInfo.maxMp; set => BasicInfo.maxMp = value; }
        public override float AttackSpeed { get => BasicInfo.attackSpeed; set => BasicInfo.attackSpeed = value; }
        public override CombatorData BasicInfo { get => basicInfo; set => basicInfo = value; }
        public override StatusBarSetting StatusBarSetting { get => statusBarSetting; set => statusBarSetting = value; }

        public override GameObject GetGameObject()
        {
            return gameObject;
        }

        public override bool IsAlive()
        {
            return true;
        }

        public override void OnKillSomebody(ICombatable killed)
        {
        }

        public override void OnSpawn(object data)
        {
            //throw new System.NotImplementedException();
        }

        public override void Reset()
        {
            //throw new System.NotImplementedException();
        }

        public override void Stun(float duration)
        {
        }

        public override bool TakeSkill(Skill skill)
        {
            GameFrameworkLog.Warning($"[StandardTrap] 陷阱不应该被技能攻击,但确实有技能攻击了它,信息如下:\n{skill.attacker.GetGameObject().name} 使用了 {skill.SkillName} 技能攻击了 {name}");

            return false;
        }
    }
}
