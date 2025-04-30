using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using Dungeon.Character.Hero;
using Dungeon.DungeonGameEntry;
using Dungeon.Gal;
using Dungeon.SkillSystem;
using Dungeon.SkillSystem.SkillEffect;
using GameFramework;
using GameFramework.Event;
using Sirenix.OdinInspector;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Dungeon.Overload
{
    public class OverlordPower : MonoBehaviour
    {
        public List<OverloadCurse> CurseList => curseList;

        public float MaxCursePower => maxCursePower;
        public float CurrentCursePower => currentCursePower;
        public void SetCursePower(float value)
        {
            currentCursePower = Mathf.Clamp(value, 0f, maxCursePower);
        }

        public void ConsumeCursePower(float amount)
        {
            currentCursePower = Mathf.Max(0f, currentCursePower - amount);
        }

        public void SpellCurse(HeroEntityBase hero,CurseType curseType)
        {
            switch (curseType)
            {
                case CurseType.Convince:
                    PersuadeHero(hero);
                    break;
                case CurseType.Capture:
                    CaptureHero(hero);
                    break;
                case CurseType.Redeploy:
                    Redeploy();
                    break;
            }
        }
        
        public bool IsFull => Mathf.Approximately(currentCursePower, maxCursePower);

        void Update()
        {
            RecoverCursePower(Time.deltaTime);
        }
        void OnEnable() 
        {
            SubscribeEvents();                
        }
        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnOneHeroEndBeingPersuadedEventArgs.EventId,OnOneHeroEndBeingCompiled);
            GameEntry.Event.Subscribe(OnOneHeroEndBeingCapturedEventArgs.EventId,OnOneHeroEndBeingCaptured);
            GameEntry.Event.Subscribe(OnOverlordEndRedeployedEventArgs.EventId,OnOverlordEndBeingRedeployed);
        }

        private void OnOverlordEndBeingRedeployed(object sender, GameEventArgs e)
        {
            ResumeGame();

            //TODO(xy) : close UI
        }

        private void OnOneHeroEndBeingCaptured(object sender, GameEventArgs e)
        {
            ResumeGame();

            if(e is OnOneHeroEndBeingCapturedEventArgs capturedArgs)
            {
                if(capturedArgs.IsCaptured)
                {
                    //TODO(xy): 把地牢勇者转换成工厂勇者
                }
                else
                {
                    //DoNothing
                }
            }
        }

        private void OnOneHeroEndBeingCompiled(object sender, GameEventArgs e)
        {
            ResumeGame();
        }
        private void ResumeGame()
        {
            DOTween.To((t)=>{
                Time.timeScale = t;
                //TODO cam
            },0,1,0.3f);
        }

        private void UnSubscribeEvents()
        {
            GameEntry.Event.Unsubscribe(OnOneHeroEndBeingPersuadedEventArgs.EventId,OnOneHeroEndBeingCompiled);
            GameEntry.Event.Unsubscribe(OnOneHeroEndBeingCapturedEventArgs.EventId,OnOneHeroEndBeingCaptured);
            GameEntry.Event.Unsubscribe(OnOverlordEndRedeployedEventArgs.EventId,OnOverlordEndBeingRedeployed);
        }

        void OnDisable()
        {
            UnSubscribeEvents();
        }



        private void RecoverCursePower(float deltaTime)
        {
            if (currentCursePower < maxCursePower)
            {
                currentCursePower += curseRegenPerSecond * deltaTime;
                currentCursePower = Mathf.Min(currentCursePower, maxCursePower);
            }
        }

        private void PersuadeHero(HeroEntityBase hero)
        {
            // Pause Game
            DOTween.To((t)=>{
                Time.timeScale = t;
            },1,0,0.3f);

            DOTween.To((t)=>{
            // TODO: Zoom Cam to Hero
            },0,1,1f).OnComplete(()=>{
                DungeonGameEntry.DungeonGameEntry.Event.Fire(
                    this,OnOneHeroStartBeingPersuadedEventArgs.Create(hero)
                );
            });
        }

        private void CaptureHero(HeroEntityBase hero)
        {
            // Pause Game
            DOTween.To((t)=>{
                Time.timeScale = t;
            },1,0,0.3f);

            DOTween.To((t)=>{
            // TODO: Zoom Cam to Hero
            },0,1,1f).OnComplete(()=>{
                DungeonGameEntry.DungeonGameEntry.Event.Fire(
                    this,OnOneHeroStartBeingCapturedEventArgs.Create(hero)
                );
            });
        }

        private void Redeploy()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Fire(
                this, OnOverlordStartRedeployedEventArgs.Create()
            );
        }


        [Header("咒力属性")]
        [SerializeField,LabelText("最大咒力")] private float maxCursePower = 100f;
        [SerializeField,LabelText("当前咒力"),ReadOnly] private float currentCursePower = 50f;
        [SerializeField,LabelText("咒力恢复速度")] private float curseRegenPerSecond = 0.5f;
        [Header("魔咒")]
        [SerializeField,LabelText("咒语列表")] private List<OverloadCurse> curseList = new ();

#if UNITY_EDITOR
        void OnValidate()
        {
            Dictionary<CurseType,OverloadCurse> curseDic = new ();

            foreach(var curse in curseList)
            {
                if(curseDic.ContainsKey(curse.curseType))
                {
                    GameFrameworkLog.Error($"[OverlordPower] 存在重复的咒语类型 {curse.curseType}");
                }
            }
        }
#endif
    }


    public enum CurseType
    {
        [LabelText("说服")] Convince,
        [LabelText("捕获")] Capture,
        [LabelText("再部署")] Redeploy,
    }

    public struct CompilationResult
    {

    }
}


namespace Dungeon.Overload
{
    /// <summary>
    /// 当一个英雄开始被说服时触发的事件
    /// </summary>
    public class OnOneHeroStartBeingPersuadedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnOneHeroStartBeingPersuadedEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public HeroEntityBase HeroEntity { get; private set; }

        public static OnOneHeroStartBeingPersuadedEventArgs Create(HeroEntityBase hero)
        {
            OnOneHeroStartBeingPersuadedEventArgs a = ReferencePool.Acquire<OnOneHeroStartBeingPersuadedEventArgs>();
            a.HeroEntity = hero;
            return a;
        }

        public override void Clear()
        {
        }

    }

    /// <summary>
    /// 当一个英雄结束被捕获对话时触发的事件
    /// </summary>
    public class OnOneHeroEndBeingPersuadedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnOneHeroEndBeingPersuadedEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public HeroEntityBase HeroEntity { get; private set; }
        public CompilationResult Result { get; private set; }

        public static OnOneHeroEndBeingPersuadedEventArgs Create(HeroEntityBase hero,CompilationResult result)
        {
            OnOneHeroEndBeingPersuadedEventArgs a = ReferencePool.Acquire<OnOneHeroEndBeingPersuadedEventArgs>();
            a.HeroEntity = hero;
            a.Result = result;
            return a;
        }

        public override void Clear()
        {

        }
    }

    public class OnOneHeroStartBeingCapturedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnOneHeroStartBeingCapturedEventArgs).GetHashCode();

        public override int Id  
        {
            get
            {
                return EventId;
            }
        }

        public HeroEntityBase HeroEntity { get; private set; }

        public static OnOneHeroStartBeingCapturedEventArgs Create(HeroEntityBase hero)
        {
            OnOneHeroStartBeingCapturedEventArgs a = ReferencePool.Acquire<OnOneHeroStartBeingCapturedEventArgs>();
            a.HeroEntity = hero;
            return a;
        }

        public override void Clear()
        {

        }
    }

    public class OnOneHeroEndBeingCapturedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnOneHeroEndBeingCapturedEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public HeroEntityBase HeroEntity { get; private set; }
        public bool IsCaptured { get; private set; }

        public static OnOneHeroEndBeingCapturedEventArgs Create(HeroEntityBase hero,bool captured)
        {
            OnOneHeroEndBeingCapturedEventArgs a = ReferencePool.Acquire<OnOneHeroEndBeingCapturedEventArgs>();
            a.HeroEntity = hero;
            a.IsCaptured = captured;
            return a;
        }

        public override void Clear()
        {

        }
    }

    public class OnOverlordStartRedeployedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnOverlordStartRedeployedEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnOverlordStartRedeployedEventArgs Create()
        {
            OnOverlordStartRedeployedEventArgs a = ReferencePool.Acquire<OnOverlordStartRedeployedEventArgs>();
            return a;
        }

        public override void Clear()
        {

        }
    }

    public class OnOverlordEndRedeployedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnOverlordStartRedeployedEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnOverlordStartRedeployedEventArgs Create()
        {
            OnOverlordStartRedeployedEventArgs a = ReferencePool.Acquire<OnOverlordStartRedeployedEventArgs>();
            return a;
        }

        public override void Clear()
        {

        }
    }


}