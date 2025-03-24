using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Hero;
using Dungeon.Character.Struct;
using Dungeon.Vision2D;
using UnityEngine;

namespace Dungeon.Character.Hero
{
    public partial class HeroEntityBase : DungeonVisitorEntity
    {
        private void InitICharacter(){}

        public Damage Attack()
        {
            throw new System.NotImplementedException();
        }

        public Struct.CharacterInfo GetCharacterInfo()
        {
            throw new System.NotImplementedException();
        }

        public void TakeDamage(Damage damage)
        {
            
        }

    }
}
