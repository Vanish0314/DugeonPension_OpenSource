using Dungeon.Character;
using Dungeon.Character;
using CharacterInfo = Dungeon.Character.CharacterInfo;

namespace Dungeon.Character
{
    public interface IHero : ICharacter,IAttackable, IDefensible
    {
    }
}