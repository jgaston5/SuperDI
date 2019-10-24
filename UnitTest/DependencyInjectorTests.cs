using System;
using DI;
using NUnit.Framework;
using UnitTest.Models;

/*
 *
 * Register interface/class
 *      class/class
 * If they duplicate a interface/class spec then override
 * create from interface specification
 * create from class specification.
 * Create when there are no dependencies
 * Create when there is one depenedencey
 *
 *

    Where to test that a specification must inherit the configuration type?
        would seem to be about the ConfigurationClass but that class is just used as an implementation feature...
 *
 *
 */

namespace UnitTest
{
    public class DependencyInjectorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GivenCreate_WhenNonDependentClassIsSpecified_ThenCreatesClass()
        {
            var dependencyInjector = new DependencyInjector(configuration =>
            {
                configuration.Configure<IBasicClass, BasicClass>();
                configuration.Configure<ComplexClass, ComplexClass>();
            });

            var basicClass = dependencyInjector.Create<IBasicClass>();
            Assert.NotNull(basicClass);
            Assert.AreEqual(typeof(BasicClass), basicClass.GetType());

            var anotherBasicClass = dependencyInjector.Create<ComplexClass>();
            Assert.NotNull(anotherBasicClass);
            Assert.AreEqual(typeof(ComplexClass), anotherBasicClass.GetType());
        }

        [Test]
        public void GivenCreate_WhenConfigurationIsDoubleSpecified_ThenCreatesLastSpecification()
        {
            var dependencyInjector = new DependencyInjector(configuration =>
            {
                configuration.Configure<IBasicClass, BasicClass>();
                configuration.Configure<IBasicClass, ASecondBasicClass>();
            });

            var basicClass = dependencyInjector.Create<IBasicClass>();
            Assert.NotNull(basicClass);
            Assert.AreEqual(typeof(ASecondBasicClass), basicClass.GetType());
        }

        [Test]
        public void GivenCreate_WhenConfigurationIsNotSpecified_ThenThrowsArgumentException()
        {
            var dependencyInjector = new DependencyInjector();

            Assert.Throws<ArgumentException>(() => dependencyInjector.Create<IBasicClass>());
        }
    }
}