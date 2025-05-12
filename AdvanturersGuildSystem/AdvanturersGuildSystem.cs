using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Dungeon.Character.Hero;
using Dungeon.DungeonGameEntry;
using Dungeon.Evnents;
using GameFramework;
using GameFramework.Event;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

namespace Dungeon
{
    public class AdvanturersGuildSystem : MonoBehaviour
    {
        void Start()
        {
            SubscribeEvents();

            foreach(var hero in heroPrefabs)
            {
                var heroType = hero.GetComponent<HeroEntityBase>();

                var pool = gameObject.AddComponent<MonoPoolComponent>();
                pool.Init(hero.name, heroType, transform, 10);

                heroMonoPool.Add(heroType.HeroName, pool);
            }
            
            SpawnNewHeroTeam();
        }
        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);
            GameEntry.Event.Subscribe(OnHeroTeamFinishDungeonExploreEvent.EventId, OnHeroTeamFinishDungeonExploreEventHandler);
            GameEntry.Event.Subscribe(OnHeroTeamDiedInDungeonEvent.EventId, OnHeroTeamDiedInDungeonEventHandler);
            GameEntry.Event.Subscribe(OnOneHeroDiedInDungeonEvent.EventId, OnOneHeroDiedInDungeonEventHandler);
        }

        private void OnOneHeroDiedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            currentBehavouringHeroTeam.Remove((e as OnOneHeroDiedInDungeonEvent).diedHero);
            //TODO(vanish): 降低所有活着勇者的san值
        }

        private void OnHeroTeamDiedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            foreach(var hero in currentGameProgressingHeroTeam)
            {
                hero.ReturnToPool();
            }
            currentGameProgressingHeroTeam.Clear();
            currentBehavouringHeroTeam.Clear();
        }

        private void OnHeroTeamFinishDungeonExploreEventHandler(object sender, GameEventArgs e)
        {

        }

        private void OnHeroStartExploreDungeonEventHandler(object sender, GameEventArgs e)
        {
            ReleaseHeroTeam(HeroTeamSpawnPositionInWorldCoord);
        }

        private void Update() 
        {
            foreach(var hero in currentGameProgressingHeroTeam)
            {
                if(hero.IsAlive()) return;
            }    

            if(currentGameProgressingHeroTeam.Count != 0)
                DungeonGameEntry.DungeonGameEntry.Event.Fire(this,OnHeroTeamDiedInDungeonEvent.Create());
        }
        public HeroEntityBase GetCurrentMainHero()
        {
            return currentMainHero;
        }
        private void ReleaseHeroTeam(Vector3 worldPos)
        {
            foreach(var hero in currentBehavouringHeroTeam)
            {
                hero.transform.position = worldPos;
                hero.gameObject.SetActive(true);
            }
        }
        private void SpawnNewHeroTeam()
        {
            currentBehavouringHeroTeam.Clear();
            currentGameProgressingHeroTeam.Clear();
            currentMainHero = null;

            var rnd = new System.Random();
            var num = rnd.Next(0, heroPrefabs.Count);
            var prefab = heroPrefabs[num];
            var heroname = prefab.GetComponent<HeroEntityBase>().HeroName;

            heroMonoPool.TryGetValue(heroname, out var pool);
            var go = pool.GetItem(null);
            // SceneManager.MoveGameObjectToScene(go.gameObject, SceneManager.GetSceneByName("DungeonGameScene"));
            go.transform.parent = null;

            //FIXME
            go.GetComponent<HeroEntityBase>().OnSpawn();
            currentBehavouringHeroTeam.Add(go.GetComponent<HeroEntityBase>());
            currentGameProgressingHeroTeam.Add(go.GetComponent<HeroEntityBase>());
            currentMainHero = go.GetComponent<HeroEntityBase>();

            go.gameObject.SetActive(false);
        }

        /// <summary>
        /// 是否所有活着的勇者都已经到达地牢出口进入happy状态 
        /// </summary>
        /// <returns></returns>
        public bool IsAllHeroHappyingAtDungeonExit()
        {
            foreach(var hero in currentBehavouringHeroTeam)
            {
                if(!hero.IsHappyingAtDungeonExit()) return false;
            }

            return true;
        }

        [Header("勇者小队生成设置")]
        [SerializeField,LabelText("勇者小队出生位置(世界坐标)")] private Vector2 HeroTeamSpawnPositionInWorldCoord;

        [SerializeField] private List<GameObject> heroPrefabs = new ();
        [ReadOnly] public List<HeroEntityBase> currentBehavouringHeroTeam = new ();//地牢中还能动的勇者
        [ReadOnly] public List<HeroEntityBase> currentGameProgressingHeroTeam = new();//当前流程的勇者小队
        [ReadOnly] private HeroEntityBase currentMainHero = null; // 当前主角
        private Dictionary<string,MonoPoolComponent> heroMonoPool = new();
    }
}
