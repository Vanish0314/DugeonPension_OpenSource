using CrashKonijn.Agent.Core;
using Dungeon.DungeonEntity;
using UnityEngine;

namespace Dungeon.Character
{
    public partial class AgentLowLevelSystem : MonoBehaviour, IAgentLowLevelSystem
    {
        private BubbleController m_BubbleController;

        public void BumpDiceCheckBubbule(string result)
        {
            BubbleManager.Instance.ShowBubble(transform, result, BubbleID.DiceBubble);
        }

        public void BumpActioningBubbule(string action, float progress)
        {
            //BubbleManager.Instance.ShowBubble()
        }

        public void BumpUseSkillBubbule(string skillName)
        {
            BubbleManager.Instance.ShowBubble(transform, skillName, BubbleID.DialogueBubble);
        }

        //TODO(xy)
        public void BumpGetExperienceBubble(int expGet)
        {
            string str = $"经验+ {expGet} ";
            BubbleManager.Instance.ShowBubble(transform, str, BubbleID.ExpBubble);
        }

        //TODO(xy)
        public void BumpLevelUpBubble(int upgradedLevel)
        {
            string str = $"升到了 {upgradedLevel} 级";
            BubbleManager.Instance.ShowBubble(transform, str, BubbleID.ExpBubble);
        }

        //TODO(xy)
        public void BumpSeeAMonsterBubble(DungeonMonsterBase monster)
        {
            string str = $"有{monster.GetName()}！";
            BubbleManager.Instance.ShowBubble(transform, str, BubbleID.DialogueBubble);
        }

        //TODO(xy)
        public void BumpSeeATreasureChestBubble(StandardDungeonTreasureChest treasureChest)
        {
            string str = $"发现了 宝藏 !";
            BubbleManager.Instance.ShowBubble(transform, str, BubbleID.DialogueBubble);
        }

        //TODO(xy)
        public void BumpSeeATrapBubble(DungeonTrapBase trap)
        {
            string msg = $"察觉 {trap.trapName}成功！";
            BubbleManager.Instance.ShowBubble(transform, msg, BubbleID.DialogueBubble);
        }

        //TODO(xy)
        public void BumpSeeATorchBubble(StandardTorch torch)
        {
            string torchString = "火把" + torch;
            BubbleManager.Instance.ShowBubble(transform, torchString, BubbleID.DialogueBubble);
        }
    }


    public class BubbleController
    {

    }
}
