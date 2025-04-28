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

        }

        //TODO(xy)
        public void BumpLevelUpBubble(int upgradedLevel)
        {

        }

        //TODO(xy)
        public void BumpSeeAMonsterBubble(DungeonMonsterBase monster)
        {

        }

        //TODO(xy)
        public void BumpSeeATreasureChestBubble(StandardDungeonTreasureChest treasureChest)
        {

        }

        //TODO(xy)
        public void BumpSeeATrapBubble(DungeonTrapBase trap)
        {

        }

        //TODO(xy)
        public void BumpSeeATorchBubble(StandardTorch torch)
        {

        }
    }


    public class BubbleController
    {

    }
}
