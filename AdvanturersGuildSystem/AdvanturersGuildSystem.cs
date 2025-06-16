using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeon.Character;
using Dungeon.DungeonGameEntry;
using Dungeon.Evnents;
using Dungeon.Overload;
using GameFramework;
using GameFramework.Event;
using ParadoxNotion;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

namespace Dungeon
{
    public class AdvanturersGuildSystem : MonoBehaviour
    {
        void Start()
        {
            SubscribeEvents();

            SpawnNewHeroTeam();
        }
        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);
            GameEntry.Event.Subscribe(OnDungeonCalculationFinishedEvent.EventId, OnDungeonCalculationFinishEventHandler);
            GameEntry.Event.Subscribe(OnDungeonCalculationLeaveAndHeroTeamDiedEventArgs.EventId, OnHeroTeamDiedInDungeonEventHandler);
            GameEntry.Event.Subscribe(OnOneHeroDiedInDungeonEvent.EventId, OnOneHeroDiedInDungeonEventHandler);
            GameEntry.Event.Subscribe(OnOneHeroEndBeingCapturedEventArgs.EventId, OnOneHeroEndBeingCapturedEventHandler);
        }

        private void OnOneHeroEndBeingCapturedEventHandler(object sender, GameEventArgs e)
        {
            if (e is OnOneHeroEndBeingCapturedEventArgs)
            {
                var args = e as OnOneHeroEndBeingCapturedEventArgs;

                if (args.IsCaptured)
                {
                    currentBeCapturedHeroInTeam.Add(args.HeroEntity);
                    currentBehavouringHeroTeam.Remove(args.HeroEntity);
                    currentGameProgressingHeroTeam.Remove(args.HeroEntity);
                }
            }
        }


        private void OnOneHeroDiedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            currentBehavouringHeroTeam.Remove((e as OnOneHeroDiedInDungeonEvent).diedHero);
            //TODO(vanish): 降低所有活着勇者的san值
        }

        private void OnHeroTeamDiedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            foreach (var hero in currentGameProgressingHeroTeam)
            {
                hero.ReturnToPool();
            }
            currentGameProgressingHeroTeam.Clear();
            currentBehavouringHeroTeam.Clear();

            SpawnNewHeroTeam();
        }

        private void OnDungeonCalculationFinishEventHandler(object sender, GameEventArgs e)
        {
            if (currentBehavouringHeroTeam.Count == 0)
            {
                return;
            }

            currentBehavouringHeroTeam.Clear();

            foreach (var hero in currentGameProgressingHeroTeam)
            {
                hero.GetComponent<AgentLowLevelSystem>().Hp = hero.GetComponent<AgentLowLevelSystem>().MaxHp;
                hero.GetComponent<AgentLowLevelSystem>().Mp = hero.GetComponent<AgentLowLevelSystem>().MaxMp;

                hero.GetComponent<AgentLowLevelSystem>().RevivalMe();

                hero.gameObject.SetActive(false);
                currentBehavouringHeroTeam.Add(hero);
            }

            {
                GameFrameworkLog.Warning($"[AdvanturersGuildSystem] 冒险者工会系统: 注意,目前没有将被捕获的勇者移除出流程队伍");
            }
        }

        private void OnHeroStartExploreDungeonEventHandler(object sender, GameEventArgs e)
        {
            ReleaseHeroTeam(HeroTeamSpawnPositionInWorldCoord);
        }
        
        // 测试用
        [DebuggerComponent.DungeonGridWindow("AddHeroSubmissiveness")]
        private static void AddHeroSubmissiveness()
        {
            DungeonGameEntry.DungeonGameEntry.AdvanturersGuildSystem.currentMainHero.GetComponent<AgentLowLevelSystem>().ModifySubmissiveness(100);
            GameFrameworkLog.Info("[AdvanturersGuildSystem]勇者屈服度+100");
        }

        private void Update()
        {
            bool flag = true;
            foreach (var hero in currentBehavouringHeroTeam)
            {
                if (!hero.IsHappyingAtDungeonExit())
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                DungeonGameEntry.DungeonGameEntry.Event.Fire(this, OnHeroTeamFinishDungeonExploreEvent.Create());
                foreach (var hero in currentBehavouringHeroTeam)
                {
                    hero.SetIsHappyingAtDungeonExit(false);
                }
            }

            foreach (var hero in currentGameProgressingHeroTeam)
            {
                if (hero.IsAlive()) return;
            }

            if (currentGameProgressingHeroTeam.Count != 0 || currentBeCapturedHeroInTeam.Count != 0)
            {   
                Time.timeScale = 1; // fucking code
                DungeonGameEntry.DungeonGameEntry.Event.Fire(this, OnHeroTeamDiedInDungeonEvent.Create());
            }
        }
        public HeroEntityBase GetCurrentMainHero()
        {
            return currentMainHero;
        }
        public List<HeroEntityBase> GetCurrentGameProgressingHeroTeam() => currentGameProgressingHeroTeam;
        public List<HeroEntityBase> GetCurrentBehavouringHeroTeam() => currentBehavouringHeroTeam;
        public List<HeroEntityBase> GetCurrentBeCapturedHeroInTeam() => currentBeCapturedHeroInTeam;
        private void ReleaseHeroTeam(Vector3 worldPos)
        {
            Queue<Vector3> positionQueue = new();
            HashSet<Vector3> visited = new();

            Vector3[] directions = new Vector3[] {
                new (1,0,0),
                new (-1,0,0),
                new (0,1,0),
                new (0,-1,0),
            };

            positionQueue.Enqueue(worldPos);
            visited.Add(worldPos);

            foreach (var hero in currentGameProgressingHeroTeam)
            {
                if (positionQueue.Count == 0)
                {
                    GameFrameworkLog.Error("[AdvanturersGuildSystem] 勇者出生位置不够.");
                    break;
                }

                Vector3 currentPos = positionQueue.Dequeue();

                hero.transform.position = currentPos;
                hero.gameObject.SetActive(true);

                foreach (var dir in directions)
                {
                    Vector3 newPos = currentPos + dir;
                    if (!visited.Contains(newPos))
                    {
                        visited.Add(newPos);
                        positionQueue.Enqueue(newPos);
                    }
                }

                {
                    hero.OnSpawn();
                    var low = hero.GetComponent<AgentLowLevelSystem>();

                    low.Hp = low.MaxHp;
                    low.Mp = low.MaxMp;
                    low.RevivalMe();
                }
            }
        }
        private void SpawnNewHeroTeam()
        {
            if (currentHeroWave >= heroWaves.Count)
            {
                GameFrameworkLog.Error($"[AdvanturersGuildSystem] 冒险者工会系统: 勇者波次已经达到最大值,无法生成新的勇者小队.");
                return;
            }
            else
            {
                GameFrameworkLog.Info($"[AdvanturersGuildSystem] 冒险者工会系统生成了一支新的勇者小队.\n勇者波次:{currentHeroWave},勇者数量:{heroWaves[currentHeroWave].heroes.Count},主角名称:{heroWaves[currentHeroWave].mainHero.name}");
            }

            currentBeCapturedHeroInTeam.Clear();
            currentBehavouringHeroTeam.Clear();
            currentGameProgressingHeroTeam.Clear();
            currentMainHero = null;

            var wave = heroWaves[currentHeroWave++];

            InitHero(wave.mainHero);
            currentMainHero = currentGameProgressingHeroTeam.First();

            foreach (var zaku in wave.heroes)
            {
                InitHero(zaku);
            }

            var sb = new StringBuilder();
            sb.AppendLine("[AdvanturersGuildSystem]冒险者工会系统生成完毕了一支新的勇者小队,小队成员:");
            sb.AppendLine($"小队主角:{wave.mainHero.name}");
            foreach (var zaku in wave.heroes)
                sb.AppendLine($"小队杂兵:{zaku.name}");

            GameFrameworkLog.Info(sb.ToString());
            
            DungeonGameEntry.DungeonGameEntry.Event.Fire(this, OnNewHeroTeamSpawnEvent.Create());
        }

        private void InitZaku(GameObject zakuPrefab, int targetLevel)
        {
            InitHero(zakuPrefab);

            var rule = DungeonGameEntry.DungeonGameEntry.DungeonResultCalculator.UpgradeExperienceNeedRule;

            int totalExp = rule.LevelExpList
                               .Where(pair => pair.Level > 1 && pair.Level <= targetLevel)
                               .Sum(pair => pair.RequiredExp);
                               
            zakuPrefab.GetComponent<AgentLowLevelSystem>().GetExperience(totalExp);
        }

        private void InitHero(GameObject heroPrefab)
        {
            var go = GameObject.Instantiate(heroPrefab, Vector3.zero, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(go.gameObject, SceneManager.GetSceneByName("DungeonGameScene"));
            go.transform.parent = null;

            //FIXME
            go.GetComponent<HeroEntityBase>().OnSpawn();
            currentBehavouringHeroTeam.Add(go.GetComponent<HeroEntityBase>());
            currentGameProgressingHeroTeam.Add(go.GetComponent<HeroEntityBase>());

            go.gameObject.SetActive(false);

            try
            {
                //勇者背包赋值
                go.GetComponent<AgentLowLevelSystem>().InitializeBackpack();
            }
            catch (Exception e)
            {
                GameFrameworkLog.Error(e.Message);
            }
        }

        /// <summary>
        /// 是否所有活着的勇者都已经到达地牢出口进入happy状态 
        /// </summary>
        /// <returns></returns>
        public bool IsAllHeroHappyingAtDungeonExit()
        {
            foreach (var hero in currentBehavouringHeroTeam)
            {
                if (!hero.IsHappyingAtDungeonExit()) return false;
            }

            return true;
        }

        [Header("勇者小队生成设置")]
        [SerializeField, LabelText("勇者小队出生位置(世界坐标)")] private Vector2 HeroTeamSpawnPositionInWorldCoord;

        [SerializeField, LabelText("勇者波次设置")] private List<HeroTeamWave> heroWaves = new();
        private int currentHeroWave = 0;
        [ReadOnly] public List<HeroEntityBase> currentBeCapturedHeroInTeam = new(); // 当前小队中被捕获的勇者
        [ReadOnly] public List<HeroEntityBase> currentBehavouringHeroTeam = new(); // 地牢中还能动的勇者
        [ReadOnly] public List<HeroEntityBase> currentGameProgressingHeroTeam = new(); // 当前流程的勇者小队
        [ReadOnly] private HeroEntityBase currentMainHero = null; // 当前主角
    }

    [System.Serializable]
    public class HeroTeamWave
    {
        [LabelText("小队主角")] public GameObject mainHero;
        
        [InfoBox("杂兵预制体的等级请务必设置为1")]
        [LabelText("小队杂兵等级"), Range(1, 20)] public int heroLevel = 1;
        [LabelText("小队杂兵成员")] public List<GameObject> heroes = new();
    }
    
    /// <summary>
    /// 当整个勇者小队完成了探险,终点房安全,该结算时触发
    /// </summary>
    public sealed class OnNewHeroTeamSpawnEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(OnNewHeroTeamSpawnEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnNewHeroTeamSpawnEvent Create()
        {
            OnNewHeroTeamSpawnEvent a = ReferencePool.Acquire<OnNewHeroTeamSpawnEvent>();
            return a;
        }

        public override void Clear()
        {
        }
    }
}
