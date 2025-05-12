using CrashKonijn.Agent.Core;
using Dungeon.DungeonEntity.InteractiveObject;
using Dungeon.DungeonEntity.Monster;
using Dungeon.DungeonEntity.Trap;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
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
            string expString = expGet + " exp";
            BubbleManager.Instance.ShowBubble(transform, expString, BubbleID.ExpBubble);
        }

        //TODO(xy)
        public void BumpLevelUpBubble(int upgradedLevel)
        {
            string upgradedString = upgradedLevel + " upgraded";
            BubbleManager.Instance.ShowBubble(transform, upgradedString, BubbleID.ExpBubble);
        }

        //TODO(xy)
        public void BumpSeeAMonsterBubble(DungeonMonsterBase monster)
        {
            string monsterString = "怪物：" + monster;
            BubbleManager.Instance.ShowBubble(transform, monsterString, BubbleID.DialogueBubble);
        }

        //TODO(xy)
        public void BumpSeeATreasureChestBubble(StandardDungeonTreasureChest treasureChest)
        {
            string treasureString = "宝物：" + treasureChest;
            BubbleManager.Instance.ShowBubble(transform, treasureString, BubbleID.DialogueBubble);
        }

        //TODO(xy)
        public void BumpSeeATrapBubble(DungeonTrapBase trap)
        {
            string msg = $"察觉 {trap.trapName}";
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
