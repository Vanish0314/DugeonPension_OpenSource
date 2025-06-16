using System.Collections;
using System.Collections.Generic;
using Dungeon;
using GameFramework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class ResourceFrom : UGuiForm
    {
        //获取UI组件
        [SerializeField] private Text GoldText;
        [SerializeField] private Text StoneText;
        [SerializeField] private Text MagicPowerText;
        [SerializeField] private Text MaterialText;
        [SerializeField] private Text ExpBallText;
        [SerializeField] private Text WoodText;
        [SerializeField] private Text CursePowerText;

        [SerializeField] private float delayDuration = 1f;
        [SerializeField] private Color warningColor = Color.red;
        
        public void UpdateGoldText(int goldValue)
        {
            GoldText.text = goldValue.ToString();
        }
        
        public void UpdateGoldTextDelay(int goldValue)
        {
            StartCoroutine(UpdateGoldTextDelay(goldValue,delayDuration));
        }

        private IEnumerator UpdateGoldTextDelay(int goldValue, float duration)
        {
            yield return new WaitForSeconds(duration);
            GoldText.text = goldValue.ToString();
            ShakeGoldText();
        }
        
        public void ShakeGoldText()
        {
            FeelSystem.Instance.ShakeText(GoldText);
        }

        public void ShakeWarningGoldText()
        {
            FeelSystem.Instance.ShakeTextWithColor(GoldText, warningColor);
        }
        
        public void UpdateStoneText(int stoneValue)
        {
            StoneText.text = stoneValue.ToString();
        }

        public void UpdateStoneTextDelay(int stoneValue)
        {
            StartCoroutine(UpdateStoneTextDelay(stoneValue, delayDuration));
        }

        private IEnumerator UpdateStoneTextDelay(int stoneValue, float duration)
        {
            yield return new WaitForSeconds(duration);
            StoneText.text = stoneValue.ToString();
            ShakeStoneText();
        }

        public void ShakeStoneText()
        {
            FeelSystem.Instance.ShakeText(StoneText);
        }

        public void ShakeWarningStoneText()
        {
            FeelSystem.Instance.ShakeTextWithColor(StoneText, warningColor);
        }

        public void UpdateMagicPowerText(int magicPowerValue)
        {
            MagicPowerText.text = magicPowerValue.ToString();
        }

        public void UpdateMagicPowerTextDelay(int magicPowerValue)
        {
            StartCoroutine(UpdateMagicPowerTextDelay(magicPowerValue, delayDuration));
        }

        private IEnumerator UpdateMagicPowerTextDelay(int magicPowerValue, float duration)
        {
            yield return new WaitForSeconds(duration);
            MagicPowerText.text = magicPowerValue.ToString();
            ShakeMagicPowerText();
        }

        public void ShakeMagicPowerText()
        {
            FeelSystem.Instance.ShakeText(MagicPowerText);
        }

        public void ShakeWarningMagicPowerText()
        {
            FeelSystem.Instance.ShakeTextWithColor(MagicPowerText, warningColor);
        }
        
        public void UpdateMaterialText(int materialValue)
        {
            MaterialText.text = materialValue.ToString();
        }

        public void UpdateMaterialTextDelay(int materialValue)
        {
            StartCoroutine(UpdateMaterialTextDelay(materialValue, delayDuration));
        }

        private IEnumerator UpdateMaterialTextDelay(int materialValue, float duration)
        {
            yield return new WaitForSeconds(duration);
            MaterialText.text = materialValue.ToString();
            ShakeMaterialText();
        }
        
        public void ShakeMaterialText()
        {
            FeelSystem.Instance.ShakeText(MaterialText);
        }

        public void ShakeWarningMaterialText()
        {
            FeelSystem.Instance.ShakeTextWithColor(MaterialText, warningColor);
        }
        
        public void UpdateExpBallText(int expBallValue)
        {
            ExpBallText.text = expBallValue.ToString();
        }

        public void UpdateExpBallTextDelay(int expBallValue)
        {
            StartCoroutine(UpdateExpBallTextDelay(expBallValue, delayDuration));
        }

        private IEnumerator UpdateExpBallTextDelay(int expBallValue, float duration)
        {
            yield return new WaitForSeconds(duration);
            ExpBallText.text = expBallValue.ToString();
            ShakeExpBallText();
        }
        
        public void ShakeExpBallText()
        {
            FeelSystem.Instance.ShakeText(ExpBallText);
        }

        public void ShakeWarningExpBallText()
        {
            FeelSystem.Instance.ShakeTextWithColor(ExpBallText, warningColor);
        }
        
        public void UpdateWoodText(int woodValue)
        {
            WoodText.text = woodValue.ToString();
        }

        public void UpdateWoodTextDelay(int woodValue)
        {
            StartCoroutine(UpdateWoodTextDelay(woodValue, delayDuration));
        }

        private IEnumerator UpdateWoodTextDelay(int woodValue, float duration)
        {
            yield return new WaitForSeconds(duration);
            WoodText.text = woodValue.ToString();
            ShakeWoodText();
        }
        
        public void ShakeWoodText()
        {
            FeelSystem.Instance.ShakeText(WoodText);
        }

        public void ShakeWarningWoodText()
        {
            FeelSystem.Instance.ShakeTextWithColor(WoodText, warningColor);
        }

        public void UpdateCursePowerText(float cursePowerValue)
        {
            CursePowerText.text = ((int)cursePowerValue).ToString();
            ShakeCursePowerText();
        }

        public void ShakeCursePowerText()
        {
            FeelSystem.Instance.ShakeText(CursePowerText);
        }

        public void ShakeWarningCursePowerText()
        {
            FeelSystem.Instance.ShakeTextWithColor(CursePowerText, warningColor);
        }

        public void SetSomeUIActive(bool active)
        {
            GoldText.transform.parent.gameObject.SetActive(active);
            StoneText.transform.parent.gameObject.SetActive(active);
            ExpBallText.transform.parent.gameObject.SetActive(active);
            WoodText.transform.parent.gameObject.SetActive(active);
        }

        public void SetCursePowerUIActive(bool active)
        {
            CursePowerText.transform.parent.gameObject.SetActive(active);
        }
    }
}
