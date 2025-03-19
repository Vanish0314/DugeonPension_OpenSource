using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework;

namespace Dungeon.BlackBoardSystem
{
    public class Arbiter
    {
        /// in naive implementation, we call it "experts"
        readonly List<IBlackBoardWriter> experts = new();

        public void RegisterExpert(IBlackBoardWriter expert)
        {
            if (expert == null) return;
            experts.Add(expert);
        }

        public void DeregisterExpert(IBlackBoardWriter expert)
        {
            if (expert == null) return;
            experts.Remove(expert);
        }

        public List<Action> BlackboardIteration(Blackboard blackboard)
        {
            var activeExperts = new List<(IBlackBoardWriter Expert, int Insistence)>();

            foreach (var expert in experts)
            {
                var insistence = expert.GetInsistence(blackboard);
                if (insistence > 0)
                {
                    activeExperts.Add((expert, insistence));
                }
            }
#if UNITY_EDITOR
            if(activeExperts.Count > 1)
            {
                GameFrameworkLog.Warning("[Arbiter] More than one expert has insistence. This is not recommended.");
                foreach (var expert in activeExperts)
                {
                    GameFrameworkLog.Warning($"[Arbiter] Expert: {expert.Expert.GetType().Name}, Insistence: {expert.Insistence}");
                }
            }
#endif
            var orderedExperts = activeExperts
                .OrderByDescending(x => x.Insistence)
                .Select(x => x.Expert)
                .ToList();

            foreach (var expert in orderedExperts)
            {
                expert.Execute(blackboard);
            }

            var actions = new List<Action>(blackboard.PassedActions);
            blackboard.ClearActions();

            return actions;
        }
    }
}