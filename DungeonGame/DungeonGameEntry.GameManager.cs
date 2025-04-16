using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dungeon.DungeonGameEntry
{
    public partial class DungeonGameEntry : MonoBehaviour
    {
        public void PauseDungeon()
        {
            if(mDungeonScene == null || mDungeonScene.buildIndex == -1)
                GetScene();   
            PauseScene(mDungeonScene);
        }
        public void ResumeDungeon()
        {
            if(mDungeonScene == null || mDungeonScene.buildIndex == -1)
                GetScene();
            ResumeScene(mDungeonScene);
        }
        public void DisableDungeon()
        {
            if(mDungeonScene == null || mDungeonScene.buildIndex == -1)
                GetScene();
            DisableScene(mDungeonScene);
        }
        public void EnableDungeon()
        {
            if(mDungeonScene == null || mDungeonScene.buildIndex == -1)
                GetScene();
            EnableScene(mDungeonScene);
        }
        public void PauseMetroplis()
        {
            if(mMetropolisScene == null || mMetropolisScene.buildIndex == -1)
                GetScene();
            PauseScene(mMetropolisScene);
        }
        public void ResumeMetroplis()
        {
            if(mMetropolisScene == null || mMetropolisScene.buildIndex == -1)
                GetScene();
            ResumeScene(mMetropolisScene);
        }   
        public void DisableMetroplis()
        {
            if(mMetropolisScene == null || mMetropolisScene.buildIndex == -1)
                GetScene();
            DisableScene(mMetropolisScene);
        }
        public void EnableMetroplis()
        {
            if(mMetropolisScene == null || mMetropolisScene.buildIndex == -1)
                GetScene();
            EnableScene(mMetropolisScene);
        }

        private void DisableScene(UnityEngine.SceneManagement.Scene scene)
        {
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                go.SetActive(false);
            }
        }
        private void EnableScene(UnityEngine.SceneManagement.Scene scene)
        {
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                go.SetActive(true);
            }
        }
        private void PauseScene(UnityEngine.SceneManagement.Scene scene)
        {
            GameFrameworkLog.Warning("[DungeonGameEntry] PauseScene only set timeScale to 0");
            Time.timeScale = 0;
        }
        private void ResumeScene(UnityEngine.SceneManagement.Scene scene)
        {
            GameFrameworkLog.Warning("[DungeonGameEntry] ResumeScene only set timeScale to 1");
            Time.timeScale = 1;
        }

        private void OnProcedureInitGameMainLeaveEventHandler(object sender, GameEventArgs e)
        {
            GetScene();
        }
        private void GetScene()
        {
            mDungeonScene = SceneManager.GetSceneByName("DungeonGameScene");
            mMetropolisScene = SceneManager.GetSceneByName("MetroplisGameScene");
        }

        private UnityEngine.SceneManagement.Scene mDungeonScene;
        private UnityEngine.SceneManagement.Scene mMetropolisScene;
    }
}

