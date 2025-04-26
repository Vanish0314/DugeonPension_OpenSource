using System;
using System.ComponentModel;
using System.Text;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem,ICombatable
    {
        private TextMeshProUGUI text;
        private GoapActionProvider actionProvider;
        private AgentBehaviour agent;

        private void InitText()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
            actionProvider = GetComponent<GoapActionProvider>();
            agent = GetComponent<AgentBehaviour>();
        }

        private void UpdateText()
        {
            text.text = GetText();
            
            if (performingAction!= null)
            {
                if (performingActionProgress >= 1)
                {
                    performingAction = null;
                    performingActionProgress = 0;
                }
            }

            if (discoveryResult!= null)
            {
                discoveryResultTime -= Time.deltaTime;
                if (discoveryResultTime <= 0)
                {
                    discoveryResult = null;
                }
            }

            if (checkResult!= null)
            {
                checkResultTime -= Time.deltaTime;
                if (checkResultTime <= 0)
                {
                    checkResult = null;
                }
            }

            if (skillUsing!= null)
            {
                skillUsingTime -= Time.deltaTime;
                if (skillUsingTime <= 0)
                {
                    skillUsing = null;
                }
            }
        }

        private string GetText()
        {
            if (actionProvider.CurrentPlan is null)
                return $"{GetTypeText()}\nIdle";
            
            if (actionProvider.Receiver.ActionState.Action is null)
                return $"{GetTypeText()}\nIdle";

            //return $"{GetTypeText()}\n{actionProvider.CurrentPlan.GetType().GetGenericTypeName()}\n{actionProvider.Receiver.ActionState.Action.GetType().GetGenericTypeName()}\n{agent.State}\n";

            StringBuilder b = new StringBuilder();
            b.AppendLine($"当前目标:{actionProvider.CurrentPlan.Goal.GetType().GetGenericTypeName()}");
            b.AppendLine($"当前行为:{actionProvider.Receiver.ActionState.Action.GetType().GetGenericTypeName()}");
            b.AppendLine($"当前行为状态:{agent.State}");

            if(performingAction!= null)
            {
                b.AppendLine($"正在执行行为:{performingAction}");
                b.AppendLine($"执行进度:{performingActionProgress}%");
            }
            
            if(discoveryResult!= null)
            {
                b.AppendLine($"发现了{discoveryResult}");
            }
            
            if(checkResult!= null)
            {
                b.AppendLine($"进行了一次检定，结果如下:\n{checkResult}");
            }

            if(skillUsing!= null)
            {
                b.AppendLine($"正在使用技能:{skillUsing}");
            }


            return b.ToString();
        }

        private string GetTypeText()
        {
            return actionProvider.AgentType.ToString();
        }
        
        private string performingAction;
        private float performingActionProgress;

        private string discoveryResult;
        private float discoveryResultTime = 0;

        private string checkResult;
        private float checkResultTime = 0;

        private string skillUsing;
        private float skillUsingTime = 0;
    }
    
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem
    {
        private BubbleController m_BubbleController;

        public void BumpDiceCheckBubbule(string result)
        {
            checkResult = result;
            checkResultTime = 5f;
            BubbleManager.Instance.ShowBubble(transform, result, BubbleID.DiceBubble);
        }

        public void BumpDiscoveryBubbule(string saying)
        {
            discoveryResult = saying;
            discoveryResultTime = 5f;
            BubbleManager.Instance.ShowBubble(transform, saying, BubbleID.DialogueBubble);
        }

        public void BumpActioningBubbule(string action,float progress)
        {
            performingAction = action;
            performingActionProgress = progress;
            //BubbleManager.Instance.ShowBubble()
        }

        public void BumpUseSkillBubbule(string skillName)
        {
            skillUsing = skillName;
            skillUsingTime = 5f;
            BubbleManager.Instance.ShowBubble(transform, skillUsing, BubbleID.DialogueBubble);
        }

    }

    
    public class BubbleController
    {
        
    }
}
