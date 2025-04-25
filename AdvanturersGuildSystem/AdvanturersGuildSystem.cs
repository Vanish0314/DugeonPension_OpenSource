using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Dungeon.Character.Hero;
using Dungeon.DungeonGameEntry;
using Dungeon.Evnents;
using GameFramework.Event;
using Sirenix.OdinInspector;
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

                heroMonoPool.Add(hero.name, pool);
            }
        }
        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);
            GameEntry.Event.Subscribe(OnHeroTeamFinishDungeonExploreEvent.EventId, OnHeroTeamFinishDungeonExploreEventHandler);
            GameEntry.Event.Subscribe(OnHeroTeamDiedInDungeonEvent.EventId, OnHeroTeamDiedInDungeonEventHandler);
        }

        private void OnHeroTeamDiedInDungeonEventHandler(object sender, GameEventArgs e)
        {
            foreach(var hero in currentHeroTeam)
            {
                hero.ReturnToPool();   
            }
        }

        private void OnHeroTeamFinishDungeonExploreEventHandler(object sender, GameEventArgs e)
        {

        }

        private void OnHeroStartExploreDungeonEventHandler(object sender, GameEventArgs e)
        {
            SpawnHero(Vector3.zero);
        }

        private void Update() 
        {
            foreach(var hero in currentHeroTeam)
            {
                if(hero.IsAlive()) return;
            }    

            if(currentHeroTeam.Count != 0)
                DungeonGameEntry.DungeonGameEntry.Event.Fire(this,OnHeroTeamDiedInDungeonEvent.Create());
        }
        public void SpawnHero(Vector3 worldPos)
        {
            heroMonoPool.TryGetValue(heroPrefabs[Random.Range(0, heroPrefabs.Count)].name, out var pool);
            var go = pool.GetItem(null);
            // SceneManager.MoveGameObjectToScene(go.gameObject, SceneManager.GetSceneByName("DungeonGameScene"));
            go.transform.parent = null;

            go.GetComponent<HeroEntityBase>().OnSpawn();
            currentHeroTeam.Add(go.GetComponent<HeroEntityBase>());
        }

        /// <summary>
        /// 是否所有勇者都已经到达地牢出口进入happy状态 
        /// </summary>
        /// <returns></returns>
        public bool IsAllHeroHappyingAtDungeonExit()
        {
            foreach(var hero in currentHeroTeam)
            {
                if(!hero.IsHappyingAtDungeonExit()) return false;
            }

            return true;
        }

        [SerializeField] private List<GameObject> heroPrefabs = new ();
        [ReadOnly] public List<HeroEntityBase> currentHeroTeam = new ();
        private Dictionary<string,MonoPoolComponent> heroMonoPool = new();
    }
}
