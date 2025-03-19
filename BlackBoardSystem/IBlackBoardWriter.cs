namespace Dungeon.BlackBoardSystem
{
    public interface IBlackBoardWriter
    {
        /// <summary>
        /// <= 0 means insistence is not required
        /// > 0 ,return value is the priority of the action
        /// </summary>
        /// <param name="blackboard"></param>
        /// <returns></returns>
        int GetInsistence(Blackboard blackboard);

        /// <summary>
        /// Execute action for the blackboard
        /// Implmentation should only use blackboard.AddAction
        /// </summary>
        /// <param name="blackboard"></param>
        void Execute(Blackboard blackboard);
    }

    public enum InsistenceLevel{
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        VeryHigh = 4,
        Dictator = 5

    }
}