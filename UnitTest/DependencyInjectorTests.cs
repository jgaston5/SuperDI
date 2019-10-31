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
            MockConfigureTransient<IBasicClass, BasicClass>();
            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var basicClass = dependencyInjector.Create<IBasicClass>();
            Assert.NotNull(basicClass);
            Assert.AreEqual(typeof(BasicClass), basicClass.GetType());
        }

        [Test]
        public void GivenCreate_WhenConfigureBaseClassSpecifyBasicSubClass_ThenCreatesClass()
        {
            MockConfigureTransient<BasicClass, BasicSubClass>();
            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var result = dependencyInjector.Create<BasicClass>();
            Assert.NotNull(result);
            Assert.AreEqual(typeof(BasicSubClass), result.GetType());
        }

        [Test]
        public void GivenCreate_WhenSingleDependencyClassIsSpecifiedAndDependencyNotConfigured_ThenThrowsException()
        {
            MockConfigureTransient<IComplexClass, ComplexClass>();
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
            MockConfigureTransient<IBasicClass, BasicClass>();
            MockConfigureTransient<IComplexClass, ComplexClass>();

            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var result = dependencyInjector.Create<IComplexClass>();
            Assert.NotNull(result);
        }

        [Test]
        public void GivenCreate_WhenDoubleDependencyClassIsSpecifiedAndDependenciesAreConfigured_ThenCreatesClass()
        {
            MockConfigureTransient<IBasicClass, BasicClass>();
            MockConfigureTransient<IComplexClass, ComplexClass>();
            MockConfigureTransient<IMoreComplexClass, MoreComplexClass>();

            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var result = dependencyInjector.Create<IMoreComplexClass>();
            Assert.NotNull(result);
        }

        [Test]
        public void GivenCreate_WhenConfigureAsSingleton_ThenCreatesOneObjectForEntireDIFlow()
        {
            MockConfigureTransient<IBasicClass, BasicClass>(ConfigurationScope.Singleton);
            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var result = dependencyInjector.Create<IBasicClass>();
            var secondResult = dependencyInjector.Create<IBasicClass>();
            Assert.AreEqual(result, secondResult);
        }

        #region helper

        public void MockConfigureTransient<TConfiguration, TSpecification>(ConfigurationScope scope = ConfigurationScope.Transient)
        {
            var configurationType = typeof(TConfiguration);
            var specificationType = typeof(TSpecification);
            var injection = new InjectionSpecification
            {
                ConfigurationType = configurationType,
                SpecificationType = specificationType,
                Constructor = specificationType.GetConstructors()[0],
                ConfigurationScope = scope
            };

            A.CallTo(() => _mockConfiguration.IsConfigured(configurationType)).Returns(true);
            A.CallTo(() => _mockConfiguration.GetInjectionSpecification(configurationType)).Returns(injection);
        }

        #endregion helper
    }
}