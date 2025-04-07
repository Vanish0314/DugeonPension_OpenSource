using Dungeon.Common;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon.BlackBoardSystem
{
    public class BlackboardController : MonoBehaviour
    {
        [InlineEditor, SerializeField] private BlackboardData blackboardData;
        [ShowInInspector] private static int refreshGapFrames = 5;
        readonly private Blackboard blackboard = new ();
        readonly private Arbiter arbiter = new ();

        void Awake()
        {
            blackboardData?.SetValuesOnBlackboard(blackboard);
        }

        public Blackboard GetBlackboard() => blackboard;
        public BlackboardData GetBlackboardData() => blackboardData;
        public bool TryGetValue<T>(string keyName,out T value)
        {
            if (blackboard.CheckIfKeyExists(keyName))
                 return blackboard.TryGetValue(blackboard.GetOrRegisterKey(keyName), out value);

            value = default;
            return false;
        }
        public bool TryGetValue<T>(BlackboardKey key, out T value)
        {
            return blackboard.TryGetValue(key, out value);
        } 
        /// <summary>
        /// [CAUTION] When you sure there must have the key,normally the key is defined by a statc class, use this method to get it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public void GetValue<T>(string keyName,out T value)
        {
            blackboard.TryGetValue(blackboard.GetOrRegisterKey(keyName),out value);
        }
        


        public void RegisterExpert(IBlackBoardWriter expert) => arbiter.RegisterExpert(expert);
        public void DeregisterExpert(IBlackBoardWriter expert) => arbiter.DeregisterExpert(expert);

        void Update()
        {
            if (Time.frameCount % refreshGapFrames != 0) return;

            foreach (var action in arbiter.BlackboardIteration(blackboard))
            {
                action();
            }
        }
    }
}