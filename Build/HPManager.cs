using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dungeon
{
    public class HPManager : MonoBehaviour
    {
        public GameObject HPPrefab;
        private Dictionary<ICombatable, HPBar> combatableHPBars = new Dictionary<ICombatable, HPBar>();
        
        private void Start()
        {
            // 获取场景中所有实现ICombatable的组件
            var combatables = FindObjectsOfType<MonoBehaviour>().OfType<ICombatable>().ToArray();
            
            foreach (var combatable in combatables)
            {
                AddHPBarToCombatable(combatable);
            }
        }

        private void Update()
        {
            // 实时更新所有血条位置
            foreach (var kvp in combatableHPBars)
            {
                UpdateHPBarPosition(kvp.Key, kvp.Value);
            }
        }

        // 为可战斗对象添加血条
        private void AddHPBarToCombatable(ICombatable combatable)
        {
            if (combatableHPBars.ContainsKey(combatable)) return;
            
            // 实例化血条预制体
            var hpBarObj = Instantiate(HPPrefab, transform); // 作为HPManager的子对象
            
            // 获取HPBar组件
            var hpBar = hpBarObj.GetComponent<HPBar>();
            if (hpBar == null)
            {
                Debug.LogError("HPPrefab缺少HPBar组件!");
                Destroy(hpBarObj);
                return;
            }
            
            // 初始化血条
            hpBar.Initialize(combatable);
            
            // 添加到字典
            combatableHPBars.Add(combatable, hpBar);
            
            // 更新初始位置
            UpdateHPBarPosition(combatable, hpBar);
        }

        // 更新血条位置（跟随战斗对象）
        private void UpdateHPBarPosition(ICombatable combatable, HPBar hpBar)
        {
            // 假设ICombatable是MonoBehaviour，可以获取Transform
            if (combatable is MonoBehaviour monoBehaviour)
            {
                // 计算血条在头顶的位置
                Vector3 worldPosition = monoBehaviour.transform.position + Vector3.up * 1.5f;
                
                // 转换为屏幕坐标
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
                
                // 设置血条位置
                hpBar.transform.position = screenPosition;
            }
        }

        // 当新的可战斗对象被创建时调用
        public void RegisterCombatable(ICombatable combatable)
        {
            if (!combatableHPBars.ContainsKey(combatable))
            {
                AddHPBarToCombatable(combatable);
            }
        }

        // 当可战斗对象被销毁时调用
        public void UnregisterCombatable(ICombatable combatable)
        {
            if (combatableHPBars.TryGetValue(combatable, out var hpBar))
            {
                Destroy(hpBar.gameObject);
                combatableHPBars.Remove(combatable);
            }
        }
    }
}