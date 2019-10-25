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
 * Create when there is a functional param to pass in

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
        public void GivenCreate_WhenConfigurationIsNotSpecified_ThenThrowsArgumentException()
        {
            var dependencyInjector = new DependencyInjector();

            Assert.Throws<ArgumentException>(() => dependencyInjector.Create<IBasicClass>());
        }

        [Test]
        public void GivenCreate_WhenConfigureInterfaceSpecifyBasicClass_ThenCreatesClass()
        {
            var dependencyInjector = new DependencyInjector(configuration =>
            {
                configuration.Configure<IBasicClass, BasicClass>();
            });

            var basicClass = dependencyInjector.Create<IBasicClass>();
            Assert.NotNull(basicClass);
            Assert.AreEqual(typeof(BasicClass), basicClass.GetType());
        }

        [Test]
        public void GivenCreate_WhenConfigureBaseClassSpecifyBasicSubClass_ThenCreatesClass()
        {
            var dependencyInjector = new DependencyInjector(configuration =>
            {
                configuration.Configure<BasicClass, BasicSubClass>();
            });

            var result = dependencyInjector.Create<BasicClass>();
            Assert.NotNull(result);
            Assert.AreEqual(typeof(BasicSubClass), result.GetType());
        }

        [Test]
        public void GivenCreate_WhenInterfaceConfigurationIsDoubleSpecified_ThenCreatesLastSpecification()
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
        public void GivenCreate_WhenSingleDependencyClassIsSpecifiedAndDependencyNotConfigured_ThenThrowsException()
        {
            var dependencyInjector = new DependencyInjector(configuration =>
            {
                configuration.Configure<ComplexClass, ComplexClass>();
            });

            Assert.Throws<ArgumentException>(() => dependencyInjector.Create<ComplexClass>());
        }

        [Test]
        public void GivenCreate_WhenSingleDependencyClassIsSpecifiedAndDependencyIsConfigured_ThenCreatesClass()
        {
            var dependencyInjector = new DependencyInjector(configuration =>
            {
                configuration.Configure<IBasicClass, BasicClass>();
                configuration.Configure<IComplexClass, ComplexClass>();
            });

            var result = dependencyInjector.Create<IComplexClass>();
            Assert.NotNull(result);
        }

        [Test]
        public void GivenCreate_WhenDoubleDependencyClassIsSpecifiedAndDependenciesAreConfigured_ThenCreatesClass()
        {
            var dependencyInjector = new DependencyInjector(configuration =>
            {
                configuration.Configure<IBasicClass, BasicClass>();
                configuration.Configure<IComplexClass, ComplexClass>();
                configuration.Configure<IMoreComplexClass, MoreComplexClass>();
            });

            var result = dependencyInjector.Create<IMoreComplexClass>();
            Assert.NotNull(result);
        }

        [Test]
        public void GivenCreate_WhenClassWithTwoConstructorsIsSpecifiedButNotConstructorInfoGiven_ThenUsesConstructorWithFewestParameters()
        {
            var dependencyInjector = new DependencyInjector(configuration =>
            {
                configuration.Configure<IMultiConstructorClass, MultiConstructorClass>();
            });

            var result = dependencyInjector.Create<IMultiConstructorClass>();
            Assert.NotNull(result);
        }
    }
}