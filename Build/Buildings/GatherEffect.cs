using System.Collections;
using System.Collections.Generic;
using Dungeon.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon
{
    public class GatherEffect : MonoPoolItem
    {
        [SerializeField] private Image image;
        public override void OnSpawn(object data)
        {
            if (data is Sprite sprite)
                image.sprite = sprite;
        }

        public override void Reset()
        {
            
        }
    }
}
