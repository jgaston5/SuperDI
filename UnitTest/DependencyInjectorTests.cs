using System;
using DI;
using FakeItEasy;
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
 *need to handle scoping - new dependency everytime it is called for, shared for create Call, single dependency for entire runtime of injector
 */

namespace UnitTest
{
    public class DependencyInjectorTests
    {
        private IDependencyInjectorConfiguration _mockConfiguration;

        [SetUp]
        public void Setup()
        {
            _mockConfiguration = A.Fake<IDependencyInjectorConfiguration>();
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
            MockConfigure<IBasicClass, BasicClass>();
            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var basicClass = dependencyInjector.Create<IBasicClass>();
            Assert.NotNull(basicClass);
            Assert.AreEqual(typeof(BasicClass), basicClass.GetType());
        }

        [Test]
        public void GivenCreate_WhenConfigureBaseClassSpecifyBasicSubClass_ThenCreatesClass()
        {
            MockConfigure<BasicClass, BasicSubClass>();
            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var result = dependencyInjector.Create<BasicClass>();
            Assert.NotNull(result);
            Assert.AreEqual(typeof(BasicSubClass), result.GetType());
        }

        [Test]
        public void GivenCreate_WhenSingleDependencyClassIsSpecifiedAndDependencyNotConfigured_ThenThrowsException()
        {
            MockConfigure<IComplexClass, ComplexClass>();
            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            try
            {
                dependencyInjector.Create<IComplexClass>();
                Assert.True(false);
            }
            catch (ArgumentException ex)
            {
                Assert.True(true);
                Assert.True(ex.Message.Contains("Dependency of"));
            }
        }

        [Test]
        public void GivenCreate_WhenSingleDependencyClassIsSpecifiedAndDependencyIsConfigured_ThenCreatesClass()
        {
            MockConfigure<IBasicClass, BasicClass>();
            MockConfigure<IComplexClass, ComplexClass>();

            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var result = dependencyInjector.Create<IComplexClass>();
            Assert.NotNull(result);
        }

        [Test]
        public void GivenCreate_WhenDoubleDependencyClassIsSpecifiedAndDependenciesAreConfigured_ThenCreatesClass()
        {
            MockConfigure<IBasicClass, BasicClass>();
            MockConfigure<IComplexClass, ComplexClass>();
            MockConfigure<IMoreComplexClass, MoreComplexClass>();

            var dependencyInjector = new DependencyInjector(_mockConfiguration);

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

        #region helper

        public void MockConfigure<TConfiguration, TSpecification>()
        {
            var configurationType = typeof(TConfiguration);
            var specificationType = typeof(TSpecification);
            var injection = new InjectionSpecification
            {
                ConfigurationType = configurationType,
                SpecificationType = specificationType,
                Constructor = specificationType.GetConstructors()[0]
            };

            A.CallTo(() => _mockConfiguration.IsConfigured<TConfiguration>()).Returns(true);
            A.CallTo(() => _mockConfiguration.IsConfigured(configurationType)).Returns(true);

            A.CallTo(() => _mockConfiguration.GetInjectionSpecification<TConfiguration>()).Returns(injection);
            A.CallTo(() => _mockConfiguration.GetInjectionSpecification(configurationType)).Returns(injection);
        }

        #endregion helper
    }
}