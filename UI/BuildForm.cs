using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Dungeon
{
    public class BuildForm : UGuiForm
    {
        public Button castleButton;
        public Text castleCount;
        public Button quarryButton;
        public Text quarryCount; 
        public Button factoryButton;
        public Text factoryCount;
        public Button monsterLairButton;
        public Text monsterLairCount;
        public Button monitorButton;
        public Text monitorCount;
        public Button dormitoryButton;
        public Text dormitoryCount;
        public Button trapButton;
        public Text trapCount;



        public void PlaceArmyUI()
        {
            castleButton.gameObject.SetActive(false);
            quarryButton.gameObject.SetActive(false);
            factoryButton.gameObject.SetActive(false);
            monsterLairButton.gameObject.SetActive(false);
            monitorButton.gameObject.SetActive(false);
            dormitoryButton.gameObject.SetActive(false);
            trapButton.gameObject.SetActive(true);
        }

        public void BuildUI()
        {
            castleButton.gameObject.SetActive(true);
            quarryButton.gameObject.SetActive(true);
            factoryButton.gameObject.SetActive(true);
            monsterLairButton.gameObject.SetActive(true);
            monitorButton.gameObject.SetActive(true);
            dormitoryButton.gameObject.SetActive(true);
            trapButton.gameObject.SetActive(false);
        }
        
        // 更新建筑的UI（count <= 0 时隐藏Button）
        public void UpdateCastleUI(int count)
        {
            castleCount.text ="X " + count.ToString();
            if (count <= 0)
                castleButton.gameObject.SetActive(false); // 仅count>0时显示
        }
        
        public void UpdateQuarryUI(int count)
        {
            quarryCount.text ="X " +  count.ToString();
            if (count <= 0)
                quarryButton.gameObject.SetActive(false); 
        }

        public void UpdateFactoryUI(int count)
        {
            factoryCount.text ="X " +  count.ToString();
            if (count <= 0)
                factoryButton.gameObject.SetActive(false);
        }

        public void UpdateMonsterLairUI(int count)
        {
            monsterLairCount.text ="X " +  count.ToString();
            if (count <= 0)
                monsterLairButton.gameObject.SetActive(false);
        }

        public void UpdateMonitorUI(int count)
        {
            monitorCount.text ="X " +  count.ToString();
            if (count <= 0)
                monitorButton.gameObject.SetActive(false);
        }

        public void UpdateDormitoryUI(int count)
        {
            dormitoryCount.text ="X " +  count.ToString();
            if (count <= 0)
                dormitoryButton.gameObject.SetActive(false);
        }
        
        public void UpdateTrapUI(int count)
        {
            trapCount.text ="X " +  count.ToString();
            if (count <= 0)
                trapButton.gameObject.SetActive(false);
        }
    }
}
