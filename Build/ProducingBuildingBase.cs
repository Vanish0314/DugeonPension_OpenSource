using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class ProducingBuildingBase : MonoBehaviour
    {
        [SerializeField] protected BuildingProduceData produceData;
        //[SerializeField] protected SpriteRenderer spriteRenderer;-------------暂时无用

        protected float timeSinceLastProduction;
        protected int assignedHeroCount;
        protected bool isSupervised;

        protected virtual void Update()
        {
            if (TimeManager.Instance.IsPaused) return;

            timeSinceLastProduction += Time.deltaTime;
            float interval = produceData.GetProductionInterval();

            if (timeSinceLastProduction >= interval)
            {
                timeSinceLastProduction = 0f;
                ExecuteProduction();
            }
        }

        protected virtual void ExecuteProduction()
        {
            int amount = produceData.CalculateTotalOutput(assignedHeroCount, isSupervised);
            AddResources(amount);

            // if (ShouldPlayEffect())
            // {
            //     PlayProductionEffect();
            // }
        }

        protected virtual void AddResources(int amount)
        {
            switch (produceData.outputResource)
            {
                case ResourceType.None:
                    break;
                case ResourceType.Gold:
                    ResourceModel.Instance.Gold += amount;
                    string textGold = "+" + amount;
                    FeelSystem.Instance.FloatingText(textGold,transform,produceData.productionGradient);
                    break;
                case ResourceType.Stone:
                    ResourceModel.Instance.Stone += amount;
                    string textStone = "+" + amount;
                    FeelSystem.Instance.FloatingText(textStone,transform,produceData.productionGradient);
                    break;
                case ResourceType.MagicPower:
                    ResourceModel.Instance.MagicPower += amount;
                    string textMagicPower = "+" + amount;
                    FeelSystem.Instance.FloatingText(textMagicPower,transform,produceData.productionGradient);
                    break;
                case ResourceType.Material:
                    ResourceModel.Instance.Material += amount;
                    string textMaterial = "+" + amount;
                    FeelSystem.Instance.FloatingText(textMaterial,transform,produceData.productionGradient);
                    break;
                // case ResourceType.ResearchPoints:
                //     ResearchSystem.Instance.AddPoints(amount);
                //     break;
            }
        }


        #region Feet（未实现）

        //------------------------------------------------暂时无用
        protected virtual bool ShouldPlayEffect()
        {
            return produceData.productionEffectPrefab != null &&
                   Random.value <= produceData.effectSpawnChance;
        }

        //------------------------------------------------暂时无用
        protected virtual void PlayProductionEffect()
        {
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.5f, 0.5f),
                0
            );

            Instantiate(produceData.productionEffectPrefab, spawnPos, Quaternion.identity);

            if (produceData.productionSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    produceData.productionSound,
                    transform.position,
                    0.5f
                );
            }
        }

        #endregion
        
        #region 勇者进驻（未实现）

        // public virtual bool AssignHero(HeroData hero)
        // {
        //     if (!produceData.canAssignHeroes ||
        //         assignedHeroCount >= produceData.maxAssignedHeroes)
        //         return false;
        //
        //     assignedHeroCount++;
        //     return true;
        // }
        //
        // public virtual void RemoveHero(HeroData hero)
        // {
        //     assignedHeroCount = Mathf.Max(0, assignedHeroCount - 1);
        // }

        #endregion
  
        public virtual void SetSupervised(bool supervised)
        {
            isSupervised = supervised;
        }
    }
}
