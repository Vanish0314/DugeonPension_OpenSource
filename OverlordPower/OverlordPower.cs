using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dungeon.Character;
using GameFramework;
using GameFramework.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Overload
{
    public class OverlordPower : MonoBehaviour
    {
        public List<OverloadCurse> CurseList => curseList;
        public bool isRedeploy = false;

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

        public bool CanConsumeCursePower(CurseType curseType)
        {
            foreach (OverloadCurse curse in curseList)
            {
                if (curse.curseType == curseType)
                {
                    if (currentCursePower >= curse.curseConsume)
                    {
                        ConsumeCursePower(curse.curseConsume);
                        return true;
                    }
                }
            }
            return false;
        }

        public void SpellCurse(HeroEntityBase hero,CurseType curseType)
        {
            switch (curseType)
            {
                case CurseType.Convince:
                    if (!CanConsumeCursePower(curseType))
                        return;
                    Audio.Instance.PlayAudio("使用魔咒");
                    PersuadeHero(hero);
                    break;
                case CurseType.Capture:
                    CaptureHero(hero);
                    break;
                case CurseType.Redeploy:
                    if (!CanConsumeCursePower(curseType))
                        return;
                    Audio.Instance.PlayAudio("使用魔咒");
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
            GameEntry.Event.Subscribe(OnOneHeroReachedASubmissivenessLevel.EventId,OnOneHeroReachedASubmissivenessLevelHandler);
        }

        private void OnOneHeroReachedASubmissivenessLevelHandler(object sender, GameEventArgs e)
        {
            var args = (OnOneHeroReachedASubmissivenessLevel)e;
            GameFrameworkLog.Info($"[OverlordPower] 勇者{args.MainHero.HeroName} 屈服度达到了{args.SubmissivenessLevel}级,应该被诅咒但是这里还没做好");
        }

        private void OnOverlordEndBeingRedeployed(object sender, GameEventArgs e)
        {
            ResumeGame();

            GameEntry.UI.GetUIForm(EnumUIForm.PlaceArmyForm).Close();
            GameEntry.UI.GetUIForm(EnumUIForm.RoomLimitForm).Close();
            
            isRedeploy = false;
        }

        private void OnOneHeroEndBeingCaptured(object sender, GameEventArgs e)
        {
            ResumeGame();

            if(e is OnOneHeroEndBeingCapturedEventArgs capturedArgs)
            {
                if(capturedArgs.IsCaptured)
                {
                    MetropolisHeroManager.Instance.TransferHero(capturedArgs.HeroEntity);
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
            Time.timeScale = 1.0f;

            StartCoroutine(FuckingRecoveryGame(1.0f));
        }

        private IEnumerator FuckingRecoveryGame(float duration)
        {
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                Time.timeScale = 1.0f;
                yield return null;
            }
        }

        private void UnSubscribeEvents()
        {
            GameEntry.Event.Unsubscribe(OnOneHeroEndBeingPersuadedEventArgs.EventId, OnOneHeroEndBeingCompiled);
            GameEntry.Event.Unsubscribe(OnOneHeroEndBeingCapturedEventArgs.EventId, OnOneHeroEndBeingCaptured);
            GameEntry.Event.Unsubscribe(OnOverlordEndRedeployedEventArgs.EventId, OnOverlordEndBeingRedeployed);
            GameEntry.Event.Unsubscribe(OnOneHeroReachedASubmissivenessLevel.EventId,OnOneHeroReachedASubmissivenessLevelHandler);
        }

        void OnDisable()
        {
            UnSubscribeEvents();
        }



        private void RecoverCursePower(float deltaTime)
        {
            if (currentCursePower < maxCursePower)
            {
                ResourceModel.Instance.CursePower = currentCursePower;
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
            },0,1,1f)
            .SetUpdate(true)
            .OnComplete(()=>{
                DungeonGameEntry.DungeonGameEntry.Event.Fire(
                    this,OnOneHeroStartBeingPersuadedEventArgs.Create(hero)
                );
            });
        }

        private void CaptureHero(HeroEntityBase hero)
        {
            if(!hero.IsFainted())
            {
                GameFrameworkLog.Info($"[OverlordPower] 捕捉勇者: {hero.HeroName},但是勇者还没昏厥");
                return;
            }

            // Pause Game
            DOTween.To((t)=>{
                Time.timeScale = t;
            },1,0,0.3f);

            DOTween.To((t)=>{
            // TODO: Zoom Cam to Hero
            },0,1,1f)
            .SetUpdate(true)
            .OnComplete(()=>{
                DungeonGameEntry.DungeonGameEntry.Event.Fire(
                    this,OnOneHeroStartBeingCapturedEventArgs.Create(hero)
                );
            });

            GameFrameworkLog.Info($"[OverlordPower] 捕捉勇者: {hero.HeroName} 成功开始!");
        }

        private void Redeploy()
        {
            // Pause Game
            DOTween.To((t)=>{
                Time.timeScale = t;
            },1,0,0.3f);
            
            GameEntry.UI.GetUIForm(EnumUIForm.CurseForm).Close();
            GameEntry.UI.OpenUIForm(EnumUIForm.PlaceArmyForm);
            GameEntry.UI.OpenUIForm(EnumUIForm.RoomLimitForm);
            
            isRedeploy = true;
            
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
        public static OnOneHeroEndBeingPersuadedEventArgs Create(HeroEntityBase hero)
        {
            OnOneHeroEndBeingPersuadedEventArgs a = ReferencePool.Acquire<OnOneHeroEndBeingPersuadedEventArgs>();
            a.HeroEntity = hero;
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
        public static readonly int EventId = typeof(OnOverlordEndRedeployedEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnOverlordEndRedeployedEventArgs Create()
        {
            OnOverlordEndRedeployedEventArgs a = ReferencePool.Acquire<OnOverlordEndRedeployedEventArgs>();
            return a;
        }

        public override void Clear()
        {

        }
    }


}