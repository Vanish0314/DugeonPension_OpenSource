using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Dungeon.GOAP
{
    public interface ISkill { }
    public class LocalSkillCooldownReadyKey<T> : WorldKeyBase where T : ISkill { }

    #region 技能要用到的TargetKey

    /// <summary>
    /// 目范围内生命值低于70%的友军
    /// </summary>
    public class NearestHpBelow70PercentageFriendlyHeroTargetKey : TargetKeyBase{}
    /// <summary>
    /// 最近的队友
    /// </summary>
    public class NearestTeammateTargetKey : TargetKeyBase { }

    #endregion

    #region 技能要用到的WorldKey
    /// <summary>
    /// 技能范围内低于70% HP的友方单位数量 ·
    /// </summary>
    public class GlobalHeroTeamHpBelow70PercentageFriendlyHeroCountKey : WorldKeyBase { }

    #endregion

    #region  技能,每个技能都有一个对应的接口
    public interface TripSlashSkill : ISkill { } // 摔绊斩击
    public interface HolySlashSkill : ISkill { } // 至圣斩
    public interface HealSkill : ISkill { } // 治疗术
    public interface FlurryOfBlowsSkill : ISkill { } // 动作如潮
    public interface TauntSkill : ISkill { } // 嘲讽
    public interface FireballSkill : ISkill { } // 火球术
    public interface MindShieldSkill : ISkill{ } // 心灵护盾
    #endregion
}
