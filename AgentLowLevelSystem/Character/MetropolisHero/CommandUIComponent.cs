using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeon.Gal;
using GameFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Dungeon
{
    public class CommandUIComponent : MonoBehaviour
    {
        [Header("主面板")]
        [SerializeField] private GameObject mainPanel;
        
        [Header("子面板")]
        [SerializeField] private GameObject commandPanel;
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private GameObject workPanel;

        [Header("主面板按钮")]
        [SerializeField] private Button commandButton;
        [SerializeField] private Button interactButton;
        [SerializeField] private Button infoButton;

        [Header("指令面板按钮")] 
        [SerializeField] private Button workButton;
        [SerializeField] private Button sleepButton;
        [SerializeField] private Button eatButton;
        
        [Header("工作面板按钮")]
        [SerializeField] private Button constructionButton;
        [SerializeField] private Button quarryButton;
        [SerializeField] private Button loggingCampButton;
        [SerializeField] private Button farmButton;
        [SerializeField] private Button castleButton;
        [SerializeField] private Button monsterLairButton;
        [SerializeField] private Button trapFactoryButton;
        
        [Header("信息面板")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI tiredText;
        [SerializeField] private TextMeshProUGUI hungerText;
        [SerializeField] private TextMeshProUGUI corruptText;
        [SerializeField] private TextMeshProUGUI efficiencyText;
        
        [Header("交互设置")]
        [SerializeField] private float interactCooldown = 5f; 
        [SerializeField] private int corruptLevelChange = 5;

        private MetropolisHeroBase targetHero;
        private float yOffset;
        private List<WorkplaceType> m_WorkplaceTypesToCheck;

        public static CommandUIComponent Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Subscribe(OnOneHeroCorruptLevelChangeEventArgs.EventId, ShowCorruptBubble);
        }

        private void OnDisable()
        {
            DungeonGameEntry.DungeonGameEntry.Event.Unsubscribe(OnOneHeroCorruptLevelChangeEventArgs.EventId, ShowCorruptBubble);
        }

        public void Setup(MetropolisHeroBase target, float offset)
        {
            // 先移除所有旧监听器
            commandButton.onClick.RemoveAllListeners();
            interactButton.onClick.RemoveAllListeners();
            infoButton.onClick.RemoveAllListeners();
            workButton.onClick.RemoveAllListeners();
            sleepButton.onClick.RemoveAllListeners();
            eatButton.onClick.RemoveAllListeners();
            constructionButton.onClick.RemoveAllListeners();
            quarryButton.onClick.RemoveAllListeners();
            loggingCampButton.onClick.RemoveAllListeners();
            farmButton.onClick.RemoveAllListeners();
            castleButton.onClick.RemoveAllListeners();
            monsterLairButton.onClick.RemoveAllListeners();
            trapFactoryButton.onClick.RemoveAllListeners();
            
            // 初始化
            targetHero = target;
            yOffset = offset;
            Initialize();

            // 主面板按钮绑定
            commandButton.onClick.AddListener(ShowCommandPanel);
            interactButton.onClick.AddListener(Interact);
            infoButton.onClick.AddListener(ShowInfoPanel);
            
            // 指令面板按钮绑定
            workButton.onClick.AddListener(ShowWorkPanel);
            sleepButton.onClick.AddListener(() => SendCommand("sleep"));
            eatButton.onClick.AddListener(() => SendCommand("eat"));
            
            // 工作面板按钮绑定
            constructionButton.onClick.AddListener(()=>SendCommand("construction"));
            quarryButton.onClick.AddListener(() => SendCommand("quarry"));
            loggingCampButton.onClick.AddListener(() => SendCommand("loggingcamp"));
            farmButton.onClick.AddListener(() => SendCommand("farm"));
            castleButton.onClick.AddListener(() => SendCommand("castle"));
            monsterLairButton.onClick.AddListener(() => SendCommand("monsterlair"));
            trapFactoryButton.onClick.AddListener(() => SendCommand("trapfactory"));
            
            // 信息面板
            nameText.text = targetHero.HeroName;
            tiredText.text = "疲劳度：" + targetHero.TiredLevel;
            hungerText.text = "饥饿度：" + targetHero.HungerLevel;
            corruptText.text = "屈服度：" + targetHero.CorruptLevel;
            efficiencyText.text = "效率： " + targetHero.Efficiency;

            // 默认显示主面板
            ShowMainPanel();
        }

        private void Initialize()
        {
            // 在初始化时预先获取有效工作类型（排除None类型）
            m_WorkplaceTypesToCheck = Enum.GetValues(typeof(WorkplaceType))
                .Cast<WorkplaceType>()
                .Where(t => t != WorkplaceType.None)
                .ToList();
        }

        private void Update()
        {
            if (targetHero != null)
            {
                // UI跟随角色
                Vector3 uiPosition = targetHero.transform.position + Vector3.up * yOffset;
                transform.position = uiPosition;
                
                // 更新信息
                tiredText.text = "疲劳度：" + targetHero.TiredLevel;
                hungerText.text = "饥饿度：" + targetHero.HungerLevel;
                if (targetHero.CorruptLevel < 30)
                {
                    corruptText.text = "屈服度：等级0";
                }
                else if (targetHero.CorruptLevel >= 30 && targetHero.CorruptLevel < 60)
                {
                    corruptText.text = "屈服度：等级1";
                }
                else if (targetHero.CorruptLevel >= 60 && targetHero.CorruptLevel < 100)
                {
                    corruptText.text = "屈服度：等级2";
                }
                else
                {
                    corruptText.text = "屈服度：等级3";
                }
                efficiencyText.text = "效率： " + targetHero.Efficiency;
                
                // 冷却检查
                if (Time.time < _lastInteractTime + interactCooldown)
                {
                    interactButton.interactable = false;
                }
                else
                {
                    interactButton.interactable = true;
                }
            }
        }

        #region 面板控制方法
        private void ShowMainPanel()
        {
            mainPanel.SetActive(true);
            commandPanel.SetActive(false);
            infoPanel.SetActive(false);
            workPanel.SetActive(false);
        }

        private void ShowCommandPanel()
        {
            mainPanel.SetActive(false);
            commandPanel.SetActive(true);
            infoPanel.SetActive(false);
            workPanel.SetActive(false);
            
            UpdateCommandButtons();
        }
        
        private void ShowWorkPanel()
        {
            mainPanel.SetActive(false);
            commandPanel.SetActive(false);
            infoPanel.SetActive(false);
            workPanel.SetActive(true);

            UpdateWorkPanel();
        }

        private void ShowInfoPanel()
        {
            mainPanel.SetActive(false);
            commandPanel.SetActive(false);
            infoPanel.SetActive(true);
            workPanel.SetActive(false);
        }

        private void CloseAllPanels()
        {
            commandPanel.SetActive(false);
            infoPanel.SetActive(false);
            workPanel.SetActive(false);
            mainPanel.SetActive(false);
        }
        #endregion

        private void SendCommand(string command)
        {
            targetHero.ReceiveCommand(command);
            gameObject.SetActive(false);
        }
        
        private void UpdateWorkPanel()
        {
            if (targetHero != null)
            {
                constructionButton.interactable = targetHero.IsWorkplaceTypeAvailable(WorkplaceType.Construction);
                quarryButton.interactable = targetHero.IsWorkplaceTypeAvailable(WorkplaceType.Quarry);
                loggingCampButton.interactable = targetHero.IsWorkplaceTypeAvailable(WorkplaceType.LoggingCamp);
                farmButton.interactable = targetHero.IsWorkplaceTypeAvailable(WorkplaceType.Farm);
                castleButton.interactable = targetHero.IsWorkplaceTypeAvailable(WorkplaceType.Castle);
                monsterLairButton.interactable = targetHero.IsWorkplaceTypeAvailable(WorkplaceType.MonsterLair);
                trapFactoryButton.interactable = targetHero.IsWorkplaceTypeAvailable(WorkplaceType.TrapFactory);
            }
        }
        
        private void UpdateCommandButtons()
        {
            if (targetHero != null)
            {
                // 检查是否有任意可用工作场所
                bool hasAnyAvailable = m_WorkplaceTypesToCheck
                    .Any(type => targetHero.IsWorkplaceTypeAvailable(type));
        
                // 设置通用工作按钮状态
                workButton.interactable = hasAnyAvailable;
                
                eatButton.interactable = targetHero.HasFoodAvailable();
            }
        }
        
        
        private int _currentCorruptLevel = 0; 
        private bool _hasShownLevelUpDialogue = true; // 是否已显示当前等级的特殊对话
        private float _lastInteractTime = -Mathf.Infinity; // 上次交互时间（初始设为最小值）
        private void Interact()
        {
            CloseAllPanels();
            
            int newLevel = targetHero.AbsoluteCorruptLevel;
            if (newLevel != _currentCorruptLevel)
            {
                _currentCorruptLevel = newLevel;
                _hasShownLevelUpDialogue = false;
            }

            //优先播放等级提升对话（仅触发一次）
            if (!_hasShownLevelUpDialogue)
            {
                PlayLevelDialogue(_currentCorruptLevel);
                _hasShownLevelUpDialogue = true;
                return;
            }
            
            // 更新最后交互时间
            _lastInteractTime = Time.time;
            
            ShowBubble();
        }

        private void ShowBubble()
        {
            // 安全检查
            if (targetHero.heroBubbles == null || targetHero.heroBubbles.Length == 0)
            {
                Debug.LogWarning("heroBubbles 未设置或为空！");
                return;
            }

            // 随机选择气泡内容
            string content = targetHero.heroBubbles[Random.Range(0, targetHero.heroBubbles.Length)];
            BubbleManager.Instance.ShowBubble(targetHero.transform, content, BubbleID.DialogueBubble);
    
            // 增加屈服度
            targetHero.CorruptLevel += corruptLevelChange;
        }

        private void ShowCorruptBubble(object sender, GameEventArgs e)
        {
            string content = "屈服度+" + corruptLevelChange;
            BubbleManager.Instance.ShowBubble(targetHero.transform, content, BubbleID.ExpBubble);
            targetHero.HideCommandUI();
        }

        private void PlayLevelDialogue(int level)
        {
            Time.timeScale = 0.01f;
            switch (level)
            {
                case 1:
                    DungeonGameEntry.DungeonGameEntry.GalSystem.PlayCorruptLevelDialogue(targetHero,DialogueType.CorruptLevel1);
                    targetHero.HideCommandUI();
                    break;
                case 2:
                    DungeonGameEntry.DungeonGameEntry.GalSystem.PlayCorruptLevelDialogue(targetHero,DialogueType.CorruptLevel2);
                    targetHero.HideCommandUI();
                    break;
                case 3:
                    DungeonGameEntry.DungeonGameEntry.GalSystem.PlayCorruptLevelDialogue(targetHero,DialogueType.CorruptLevel3);
                    targetHero.HideCommandUI();
                    break;
                default:
                    break;
            }
        }
        
    }
}
