using Dungeon.Character.Interfaces;
using Dungeon.Character.Struct;
using CharacterInfo = Dungeon.Character.Struct.CharacterInfo;

namespace Dungeon.Character.Hero
{
    public interface IHero : ICharacter,IAttackable, IDefensible
    {
    }
}