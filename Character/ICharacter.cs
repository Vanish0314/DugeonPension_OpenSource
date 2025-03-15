using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Struct;

namespace Dungeon.Character
{
    /// <summary>
    /// A character is an GO that could interact with heros or itself is hero.
    /// like a hero,a monster,a NPC,a chest,a door,a trap,a trapChest etc.
    /// </summary>
    public interface ICharacter
    {
        public CharacterInfo GetCharacterInfo();
    }
}
