using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Hero;
using Dungeon.SkillSystem;
using Sirenix.OdinInspector;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Dungeon.Overload
{
    public class OverlordPower : MonoBehaviour
    {
        public float MaxCursePower => maxCursePower;
        public float CurrentCursePower => currentCursePower;
        public void SetCursePower(float value)
        {
            currentCursePower = Mathf.Clamp(value, 0f, maxCursePower);
        }

        public void ConsumeCursePower(float amount)
        {
            currentCursePower = Mathf.Max(0f, currentCursePower - amount);
        }

        public void SpellCurse(HeroEntityBase hero)
        {

        }
        public bool IsFull => Mathf.Approximately(currentCursePower, maxCursePower);

        void Update()
        {
            RecoverCursePower(Time.deltaTime);
        }

        private void RecoverCursePower(float deltaTime)
        {
            if (currentCursePower < maxCursePower)
            {
                currentCursePower += curseRegenPerSecond * deltaTime;
                currentCursePower = Mathf.Min(currentCursePower, maxCursePower);
            }
        }


        [Header("咒力属性")]
        [SerializeField,LabelText("最大咒力")] private float maxCursePower = 100f;
        [SerializeField,LabelText("当前咒力"),ReadOnly] private float currentCursePower = 50f;
        [SerializeField,LabelText("咒力恢复速度")] private float curseRegenPerSecond = 0.5f;
        [Header("魔咒")]
        [SerializeField,LabelText("咒语列表")] private List<OverloadCurse> curseList = new ();
    }

    
    [CreateAssetMenu(fileName = "新魔王诅咒", menuName = "Overload/魔王诅咒")]
    public class OverloadCurse : ScriptableObject
    {
        
    }
}
