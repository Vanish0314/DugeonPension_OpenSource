using System;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public class MetropolisHPModel : MonoBehaviour
    {
        public static MetropolisHPModel Instance { get; private set; }
        
        public event Action<float> OnMetropolisHPChanged;

        private float metropolisHP = 100;

        public float MetropolisHP
        {
            get => metropolisHP;
            set
            {
                metropolisHP = value;
                OnMetropolisHPChanged?.Invoke(value);
                if (value <= 0)
                {
                    metropolisHP = 0;
                    DungeonGameEntry.DungeonGameEntry.Event.Fire(this, OnMetropolisBeDestroyEventArgs.Create());
                }
            }
        }

        private void Awake() {
            if (Instance == null) Instance = this;
            MetropolisHP = 100;
        }

        public void ModifyMetropolisHP(float amount)
        {
            MetropolisHP += amount;
        }
    }
    
    public class OnMetropolisBeDestroyEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnMetropolisBeDestroyEventArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static OnMetropolisBeDestroyEventArgs Create()
        {
            OnMetropolisBeDestroyEventArgs onMetropolisBeDestroyEventArgs = ReferencePool.Acquire<OnMetropolisBeDestroyEventArgs>();
            return onMetropolisBeDestroyEventArgs;
        }

        public override void Clear()
        {
            
        }
    }
}
