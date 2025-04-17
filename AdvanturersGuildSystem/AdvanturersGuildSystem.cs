using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Hero;
using Dungeon.DungeonGameEntry;
using Dungeon.Evnents;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dungeon
{
    public class AdvanturersGuildSystem : MonoBehaviour
    {
        void Start()
        {
            SubscribeEvents();
        }
        private void SubscribeEvents()
        {
            GameEntry.Event.Subscribe(OnHeroStartExploreDungeonEvent.EventId, OnHeroStartExploreDungeonEventHandler);
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
            var go = Instantiate(heroPrefabs[Random.Range(0, heroPrefabs.Count)], worldPos, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(go, SceneManager.GetSceneByName("DungeonGameScene"));
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
        public List<HeroEntityBase> currentHeroTeam = new ();
    }
}
