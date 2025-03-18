using System.Collections;
using System.Collections.Generic;
using Dungeon.Common;
using UnityEngine;

namespace Dungeon.WorldBlackBoard
{
    public enum WorldBlackBoardKeyVec3
    {
        DungeonExitWorldPosition
    }
    public enum WorldBlackBoardKeyInt
    {

    }
    public enum WorldBlackBoardKeyBool
    {

    }
    public enum WorldBlackBoardKeyFloat
    {

    }
    public class WorldBlackBoardWritePermission
    {

    }
    //TODO(vanish) : Write permission
    public class WorldBlackBoard : MonoSingletonPersistent<WorldBlackBoard>
    {
        public Vector3 GetVector3(WorldBlackBoardKeyVec3 key) => m_Vec3BlackBoard[key];
        public bool GetBool(WorldBlackBoardKeyBool key) => m_BoolBlackBoard[key];
        public int GetInt(WorldBlackBoardKeyInt key) => m_IntBlackBoard[key];
        public float GetFloat(WorldBlackBoardKeyFloat key) => m_FloatBlackBoard[key];

        public void Write(WorldBlackBoardKeyVec3 key, Vector3 value)
        {
            m_Vec3BlackBoard[key] = value;
        }

        public void Write(WorldBlackBoardKeyBool key, bool value)
        {
            m_BoolBlackBoard[key] = value;
        }

        public void Write(WorldBlackBoardKeyInt key, int value)
        {
            m_IntBlackBoard[key] = value;
        }

        public void Write(WorldBlackBoardKeyFloat key, float value)
        {
            m_FloatBlackBoard[key] = value;
        }

        private Dictionary<WorldBlackBoardKeyVec3, Vector3> m_Vec3BlackBoard = new Dictionary<WorldBlackBoardKeyVec3, Vector3>
{
    [WorldBlackBoardKeyVec3.DungeonExitWorldPosition] =new Vector3(0, 0, 0),
};

        private Dictionary<WorldBlackBoardKeyBool, bool> m_BoolBlackBoard = new ();
        private Dictionary<WorldBlackBoardKeyInt, int> m_IntBlackBoard = new ();
        private Dictionary<WorldBlackBoardKeyFloat, float> m_FloatBlackBoard = new ();
    }
}
