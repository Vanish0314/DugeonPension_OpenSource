using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Hero;
using UnityEngine;

namespace Dungeon.Character.Hero
{
    public partial class Hero : MonoBehaviour,IHero
    {
        void Awake()
        {
            InitICharacter();

            InitGOAP();
        }

        void Update()
        {
            UpdateGOAP();
        }
    }
}
