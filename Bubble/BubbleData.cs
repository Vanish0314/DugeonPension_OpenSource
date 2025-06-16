using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "New BubbleData", menuName = "Hero/气泡编辑")]
    [Serializable]
    public class BubbleData : ScriptableObject
    {
        public string beCommand;
        public string hunger;
        public string tired;
        public string work;
    }
}
