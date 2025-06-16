using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using GameFramework;
using GameFramework.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Character
{
    public partial class AgentLowLevelSystem : MonoBehaviour
    {
        public int GetSubmissiveness() => m_Properties.Submissiveness;

        public void ModifySubmissiveness(int value)
        {
#if UNITY_EDITOR
            if (Hp <= 0)
            {
                GameFrameworkLog.Warning($"屈服度修改失败，因为勇者已经死亡:{gameObject.name}");
            }
#endif
            if (Hp <= 0)
                return;

            int oldValue = m_Properties.Submissiveness;

            if (value > 0)
            {
                float hpRatio = (float)Hp / MaxHp;
                float multiplier;

                if (hpRatio > 0.7f)
                    multiplier = 0.5f;
                else if (hpRatio > 0.3f)
                    multiplier = 1f;
                else
                    multiplier = 1f;

                int charismaAdjusted = value - m_Properties.dndSkillData.CharismaModifyValue;
                int finalDelta = Mathf.RoundToInt(charismaAdjusted * multiplier);

                int newValue = Mathf.Clamp(oldValue + finalDelta, 0, 100);
                m_Properties.Submissiveness = newValue;

                if (newValue > oldValue)
                {
                    CheckAndFireSubmissivenessEvent(newValue, oldValue);
                }
            }
            else if (value < 0)
            {
                int newValue = Mathf.Clamp(oldValue + value, 0, 100);
                m_Properties.Submissiveness = newValue;
            }
        }

        public void ModifySubmissiveness_Directly(int value)
        {
#if UNITY_EDITOR
            if (Hp <= 0)
            {
                GameFrameworkLog.Warning($"屈服度修改失败，因为勇者已经死亡:{gameObject.name}");
            }
#endif

            if (Hp <= 0)
                return;

            var oldValue = m_Properties.Submissiveness;
            var newValue = Mathf.Clamp(oldValue + value, 0, 100);

            m_Properties.Submissiveness = newValue;
            CheckAndFireSubmissivenessEvent(newValue, oldValue);
         }

        private void CheckAndFireSubmissivenessEvent(int newValue, int oldValue)
        {
            // FIXME: 没有考虑一次性触发多个阈值事件的情况
            foreach (int threshold in new[] { 25, 50, 75 })
            {
                if (oldValue < threshold && newValue >= threshold && !triggeredThresholds.Contains(threshold))
                {
                    triggeredThresholds.Add(threshold);
                    DungeonGameEntry.DungeonGameEntry.Event.Fire(this,
                        OnOneHeroReachedASubmissivenessLevel.Create(
                            GetComponent<HeroEntityBase>(),
                            threshold
                        ));
                }
            }

            if (newValue == 100)
            {
                DungeonGameEntry.DungeonGameEntry.Event.Fire(this,
                    OnHeroFaintedBySubmissiveness.Create(
                        GetComponent<HeroEntityBase>()
                    ));

                FaintMeBySubmissiveness();
            }
        }


        private void UpdateSubmissiveness()
        {
            if (IsInSafeState())
            {
                submissivenessTickTimer += Time.deltaTime;
                if (submissivenessTickTimer >= 5f)
                {
                    submissivenessTickTimer = 0f;
                    ReduceSubmissivenessInSafeZone();
                }
            }
            else
            {
                submissivenessTickTimer = 0f;
            }
        }

        private bool IsInSafeState()
        {
            return m_BrainMemory.monstersInVision.Count == 0 && m_BrainMemory.trapInVision.Count == 0;
        }

        private void ReduceSubmissivenessInSafeZone()
        {
            int value = -SubmissivenessDecreaseAmount - m_Properties.dndSkillData.CharismaModifyValue;
            ModifySubmissiveness(value);

            GameFrameworkLog.Info($"[AgentLowLevelSystem] 勇者:{m_Properties.heroName} 屈服度减少:{value}");
        }

        private void FaintMeBySubmissiveness()
        {
            FaintMe(AgentLowLevelSystem.FaintDuration);
        }

        [Header("屈服度设置")]
        [ShowInInspector, LabelText("晕厥时长")] public static float FaintDuration = 30f;
        [SerializeField, LabelText("屈服度更新间隔")] private float SubmissivenessUpdateInterval = 5f;
        [SerializeField, LabelText("屈服度每次更新减少量")] private int SubmissivenessDecreaseAmount = 1;

        public bool IsFainted { get; private set; } = false;
        private float submissivenessTickTimer;
        private readonly HashSet<int> triggeredThresholds = new(); 
    }

    public class OnOneHeroReachedASubmissivenessLevel : GameEventArgs
    {
        public static readonly int EventId = typeof(OnOneHeroReachedASubmissivenessLevel).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public HeroEntityBase MainHero { get; private set; }

        /// <summary>
        /// 1-100, 代表当前勇者达到的屈服度阈值
        /// </summary>
        public int SubmissivenessLevel { get; private set; }

        public static OnOneHeroReachedASubmissivenessLevel Create(HeroEntityBase mainHero, int submissivenessLevel)
        {
            OnOneHeroReachedASubmissivenessLevel onHeroArrivedInDungeonEventArgs = ReferencePool.Acquire<OnOneHeroReachedASubmissivenessLevel>();

            onHeroArrivedInDungeonEventArgs.MainHero = mainHero;

            onHeroArrivedInDungeonEventArgs.SubmissivenessLevel = submissivenessLevel;

            return onHeroArrivedInDungeonEventArgs;
        }

        public override void Clear()
        {
        }
    }

    public class OnHeroFaintedBySubmissiveness : GameEventArgs
    {
        public static readonly int EventId = typeof(OnHeroFaintedBySubmissiveness).GetHashCode();
        public override int Id => EventId;

        public HeroEntityBase MainHero { get; private set; }

        public static OnHeroFaintedBySubmissiveness Create(HeroEntityBase hero)
        {
            var e = ReferencePool.Acquire<OnHeroFaintedBySubmissiveness>();
            e.MainHero = hero;
            return e;
        }

        public override void Clear()
        {
            MainHero = null;
        }
    }

}
