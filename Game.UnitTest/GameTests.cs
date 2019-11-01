using System;
using System.Collections.Generic;
using System.Linq;
using DI;
using FakeItEasy;
using NUnit.Framework;

namespace Game.UnitTest
{
    public class Tests
    {
        private IDependencyInjector mockDependencyInjector;

        [SetUp]
        public void Setup()
        {
            mockDependencyInjector = A.Fake<IDependencyInjector>();
        }

        [Test]
        public void GivenConstructor_WhenDIIsNull_ThenThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Game(null));
        }

        [Test]
        public void GivenCreateCharacter_WhenDIIsNotConfigured_ThenReturnFailedServiceCall()
        {
            A.CallTo(() => mockDependencyInjector.Create<Character>()).Throws<ArgumentException>();

            Assert.Throws<ArgumentException>((() => mockDependencyInjector.Create<Character>()));
            IGame target = new Game(mockDependencyInjector);
            Response<Character> response = target.CreateCharacter("Test Name");

            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Null(response.Result);
        }

        [Test]
        public void GivenCreateCharacter_WhenDIIsConfigured_ThenReturnSuccessServiceCallWithCharacter()
        {
            var testName = "Test Name";
            IGame target = new Game(mockDependencyInjector);
            A.CallTo(() => mockDependencyInjector.Create<Character>()).Returns(new Character());
            Response<Character> response = target.CreateCharacter(testName);

            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.NotNull(response.Result);
            Assert.AreEqual(response.Result.Name, testName);
        }

        [Test]
        public void GivenGetCharacters_WhenCharactersAreMade_ThenListOfCharacters()
        {
            var testName1 = "Test Name 1";
            var testName2 = "Test Name 2";
            IGame target = new Game(mockDependencyInjector);
            A.CallTo(() => mockDependencyInjector.Create<Character>()).Returns(new Character());
            target.CreateCharacter(testName1);
            target.CreateCharacter(testName2);
            Response<List<Character>> charactersResponse = target.GetCharacters();

            Assert.NotNull(charactersResponse);
            Assert.True(charactersResponse.Success);
            Assert.NotNull(charactersResponse.Result);
            Assert.AreEqual(2, charactersResponse.Result.Count);
            Assert.True(charactersResponse.Result.Any(x => x.Name == testName1));
            Assert.True(charactersResponse.Result.Any(x => x.Name == testName2));
        }
    }
}