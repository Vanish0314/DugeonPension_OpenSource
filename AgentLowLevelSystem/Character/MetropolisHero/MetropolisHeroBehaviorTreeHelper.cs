using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

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
        Building,
        Command     
    }

    public class MetropolisHeroBehaviorTreeHelper : MonoBehaviour
    {
        public MetropolisHeroAIState state;
        public MetropolisHeroAIState previousState;
        public MetropolisHeroAIState commandType;
        
        public int hungerLevel;
        public int energyLevel;
        public int tiredLevel;
        public bool isCommandale = false;
        public bool hasCommand = false;
        public bool hasFoodAvailable = false;
        public bool hasDormitoryAvailable = false;
        public bool hasWorkAvailable = false;
        public bool workComplete = false;
        public Vector3 nearestFood;
        public Vector3 nearestDormitory;
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
            nearestDormitory = m_HeroBase.FindNearestDormitoryPosition();
            m_Motor.MoveTo(nearestDormitory);
        }
        
        public bool ReachTargetDormitory()
        {
            return m_Motor.ReachedTarget(nearestDormitory);
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
            m_HeroBase.FindNearestWorkplacePosition();
        }
        
        public void MoveToWorkPosition()
        {
            var workPlace = m_HeroBase.FindNearestWorkplacePosition();
            workPosition = workPlace.Item1;
            var workPlaceCollider = workPlace.Item2;
            m_Motor.MoveTo(workPosition, workPlaceCollider);
        }

        public void MoveToCommandWorkPosition()
        {
            workPosition = m_HeroBase.FindWorkplacePositionOfType();
            m_Motor.MoveTo(workPosition);
        }
        
        public bool ReachTargetWorkPosition()
        {
            return m_Motor.ReachedTarget(workPosition);
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
