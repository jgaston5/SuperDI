using System;
using System.Collections.Generic;
using System.Text;
using DI;

namespace Game
{
    public class Game : IGame
    {
        private readonly IDependencyInjector _dependencyInjector;
        private readonly List<Character> _characters = new List<Character>();

        public Game(IDependencyInjector dependencyInjector)
        {
            if (dependencyInjector == null)
            {
                throw new ArgumentException("Dependency injector cannot be null.");
            }

            _dependencyInjector = dependencyInjector;
        }

        public Response<Character> CreateCharacter(string name)
        {
            var response = new Response<Character>();
            try
            {
                var character = _dependencyInjector.Create<Character>();
                character.Name = name;
                response.Success = true;
                response.Result = character;
                _characters.Add(character);
            }
            catch (ArgumentException)
            {
                response.Success = false;
            }

            return response;
        }

        public Response<List<Character>> GetCharacters()
        {
            return new Response<List<Character>> { Success = true, Result = _characters };
        }
    }
}