using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Evnents;
using Dungeon.Procedure;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public class MetropolisHPManager : MonoBehaviour
    {
        public static MetropolisHPManager Instance { get; private set; }
        
        [SerializeField] private float reduceAmount;
        [SerializeField] private float reduceInterval;
        private Coroutine _mCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            transform.position = Vector3.zero;
        }

        public void Initialize()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnHeroReachEventArgs.EventId, OnHeroReach);
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnReturnGameStartButtonClickEvent.EventId, OnReturnGameStartButtonClick);
        }

        private void OnReturnGameStartButtonClick(object sender, GameEventArgs e)
        {
            RevertHP();
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
                Subscribe();
            }
        }

        private void OnDisable()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnHeroReachEventArgs.EventId, OnHeroReach);
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnReturnGameStartButtonClickEvent.EventId, OnReturnGameStartButtonClick);
        }

        private void OnHeroReach(object sender, GameEventArgs e)
        {
            StartReduceHP(reduceInterval, reduceAmount);
        }

        private void StartReduceHP(float interval, float amount)
        {
            _mCoroutine = StartCoroutine(ReduceHP(interval, amount));
        }
        
        private IEnumerator ReduceHP(float interval, float amount)
        {
            while (GameEntry.Procedure.CurrentProcedure is ProcedureMetropolisStage)
            {
                MetropolisHPModel.Instance.ModifyMetropolisHP(amount);
                yield return new WaitForSeconds(interval);
            }
            
            StopReduceHP();
        }

        private void StopReduceHP()
        {
            if (_mCoroutine != null)
                StopCoroutine(_mCoroutine);
            _mCoroutine = null;
        }

        private void RevertHP()
        {
            MetropolisHPModel.Instance.MetropolisHP = 100;
        }
    }
}
