using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dungeon
{
    public enum MetropolisHeroAIState
    {
        Idle,
        Moving,
        Eating,
        Sleeping,
        Working,
        Talking,
        Revolting,
        Command     
    }

    public class MetropolisHeroBehaviorTreeHelper : MonoBehaviour
    {
        public MetropolisHeroAIState state;
        public MetropolisHeroAIState targetState;
        public MetropolisHeroAIState previousState;
        public MetropolisHeroAIState commandType;
        
        public int hungerLevel;
        public int energyLevel;
        public int tiredLevel;
        public bool isCommandale = false;
        public bool hasCommand = false;
        public bool hasFoodAvailable = false;
        public bool hasWorkAvailable = false;
        public bool workComplete = false;
        public Vector3 nearestFood;
        public Vector3 dormitoryPosition;
        public Vector3 workPosition;
        public bool canTalk = false;
        public float talkTime;
        public float lastCheckTime;
        private MetropolisHeroBase m_HeroBase;
        private MetropolisHeroMotor m_Motor;
        private float checkCooldown = 30f;
    
        public void Init(MetropolisHeroBase heroBase)
        {
            m_HeroBase = heroBase;
            m_Motor = GetComponent<MetropolisHeroMotor>();
            state = MetropolisHeroAIState.Idle;
        }

        // 带冷却时间的随机检查
        public bool RandomCheckWithCooldown(float randomChance, float cooldown = -1)
        {
            // 如果指定了冷却时间参数，则使用该参数，否则使用默认值
            float actualCooldown = cooldown >= 0 ? cooldown : checkCooldown;
        
            // 检查是否在冷却时间内
            if (Time.time - lastCheckTime < actualCooldown)
            {
                return false;
            }

            // 更新最后检查时间
            lastCheckTime = Time.time;
        
            // 返回随机结果
            return Random.value < randomChance;
        }

        public void Wander()
        {
            m_HeroBase.Wander();
        }
        
        public void MoveToNearestFood()
        {
            nearestFood = m_HeroBase.FindNearestFoodPosition();
            m_Motor.MoveTo(nearestFood);
        }
        
        public bool ReachTargetFood()
        {
            return m_Motor.ReachedTarget(nearestFood);
        }
        
        public void Eat()
        {
            m_HeroBase.Eat();
        }

        public void MoveToNearestDormitory()
        {
            var sleepPlace = m_HeroBase.TryFindNearestDormitory();
            dormitoryPosition = sleepPlace.Item1;
            var dormitoryCollider = sleepPlace.Item2;
            m_Motor.MoveTo(dormitoryPosition, dormitoryCollider);
        }
        
        public bool ReachTargetDormitory()
        {
            var sleepPlace = m_HeroBase.TryFindNearestDormitory();
            dormitoryPosition = sleepPlace.Item1;
            var dormitoryCollider = sleepPlace.Item2;
            return m_Motor.ReachedTarget(dormitoryPosition, dormitoryCollider);
        }
        
        public void Sleep()
        {
            m_HeroBase.Sleep();
        }

        public void Talk()
        {
            m_HeroBase.Talk();
        }

        public void AssignWorkPlace()
        {
            if(!m_HeroBase.RegisterWorkPlace())
                GameFrameworkLog.Warning("没能Register work place");
        }
        
        public void MoveToWorkPosition()
        {
            var workPlace = m_HeroBase.FindHighestPriorityWorkplace();
            workPosition = workPlace.Item1;
            var workPlaceCollider = workPlace.Item2;
            m_Motor.MoveTo(workPosition, workPlaceCollider);
        }
        
        public bool ReachTargetWorkPosition()
        {
            var workPlace = m_HeroBase.FindHighestPriorityWorkplace();
            workPosition = workPlace.Item1;
            var workPlaceCollider = workPlace.Item2;
            return m_Motor.ReachedTarget(workPosition, workPlaceCollider);
        }
    
        public void Work()
        {
            m_HeroBase.Work();
        }

        public void Revolt()
        {
            m_HeroBase.Revolt();
        }

        public void SetUpCommand()
        {
            m_HeroBase.SetUpCommand();
        }
    }
}
