using System.Collections.Generic;

namespace Game
{
    public interface IGame
    {
        Response<Character> CreateCharacter(string name);

        Response<List<Character>> GetCharacters();
    }
}