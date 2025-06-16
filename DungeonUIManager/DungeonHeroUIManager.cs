using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Character;
using Dungeon.Character;
using Dungeon.Common;
using Dungeon.Evnents;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public class DungeonHeroUIManager : MonoBehaviour
    {
        [SerializeField] private MonoPoolItem m_CaptureButtonPrefab;
        [SerializeField] private int m_InitialPoolSize = 20;
        [SerializeField] private Vector3 m_ButtonOffset = new Vector3(0, 0.5f, 0); // 按钮位置偏移
        
        private MonoPoolComponent m_captureButtonPool;
        private List<CaptureButton> m_ActiveCaptureButtons = new List<CaptureButton>();
        
        public static DungeonHeroUIManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            transform.position = Vector3.zero;
            Initialize();
        }

        private void Update()
        {
            if(m_ActiveCaptureButtons.Count > 0)
            {
                foreach (var captureButton in m_ActiveCaptureButtons)
                {
                    captureButton.UpdatePosition();
                }
            }
        }

        public void Initialize()
        {
            m_captureButtonPool = gameObject.GetOrAddComponent<MonoPoolComponent>();
            m_captureButtonPool.Init("CaptureButtonPool", m_CaptureButtonPrefab, transform, m_InitialPoolSize);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnHeroFaintedBySubmissiveness.EventId, OnHeroFainted);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnLeaveHeroExploringDungeonProcedureEvent.EventId, OnLeaveHeroExploringDungeon);
        }

        private int flag = 0;
        private void OnEnable()
        {
            if (flag == 0)
            {
                flag = 1;
            }
            else
            {
                DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnHeroFaintedBySubmissiveness.EventId, OnHeroFainted);
                DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnLeaveHeroExploringDungeonProcedureEvent.EventId, OnLeaveHeroExploringDungeon);
            }
        }
        
        private void OnDisable()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnHeroFaintedBySubmissiveness.EventId, OnHeroFainted);
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnLeaveHeroExploringDungeonProcedureEvent.EventId, OnLeaveHeroExploringDungeon);
        }
        
        private void OnHeroFainted(object sender, GameEventArgs e)
        {
            OnHeroFaintedBySubmissiveness eventData = (OnHeroFaintedBySubmissiveness)e;
            CreateCaptureButton(eventData.MainHero);
        }

        private void OnLeaveHeroExploringDungeon(object sender, GameEventArgs e)
        {
            ReturnAllCaptureButtons();
        }

        private void CreateCaptureButton(HeroEntityBase hero)
        {
            var button = m_captureButtonPool.GetItem(hero) as CaptureButton;
            if (button == null)
            {
                Debug.LogError("获取的按钮不是CaptureButton类型!");
                return;
            }
            
            button.Initialize(hero, m_ButtonOffset);
            m_ActiveCaptureButtons.Add(button);
        }

        private void ReturnAllCaptureButtons()
        {
            foreach (var button in m_ActiveCaptureButtons)
            {
                button.ReturnToPool();
            }
            m_ActiveCaptureButtons.Clear();
        }
    }
}