using System;
using DI;
using FakeItEasy;
using NUnit.Framework;
using UnitTest.Models;

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
        public void GivenCreate_WhenConfigurationStyleIsNone_ThenCreatesClass()
        {
            var configurationType = typeof(IBasicClass);
            var injection = new InjectionSpecification
            {
                ConfigurationType = configurationType,
                ConfigurationStyle = ConfigurationStyle.None
            };

            A.CallTo(() => _mockConfiguration.IsConfigured(configurationType)).Returns(true);
            A.CallTo(() => _mockConfiguration.GetInjectionSpecification(configurationType)).Returns(injection);

            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            Assert.Throws<ArgumentException>(() => dependencyInjector.Create<IBasicClass>());
        }

        [Test]
        public void GivenCreate_WhenConfigureInterfaceSpecifyBasicClass_ThenCreatesClass()
        {
            MockConfigureSpecify<IBasicClass, BasicClass>();
            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var basicClass = dependencyInjector.Create<IBasicClass>();
            Assert.NotNull(basicClass);
            Assert.AreEqual(typeof(BasicClass), basicClass.GetType());
        }

        [Test]
        public void GivenCreate_WhenConfigureBaseClassSpecifyBasicSubClass_ThenCreatesClass()
        {
            MockConfigureSpecify<BasicClass, BasicSubClass>();
            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var result = dependencyInjector.Create<BasicClass>();
            Assert.NotNull(result);
            Assert.AreEqual(typeof(BasicSubClass), result.GetType());
        }

        [Test]
        public void GivenCreate_WhenSingleDependencyClassIsSpecifiedAndDependencyNotConfigured_ThenThrowsException()
        {
            MockConfigureSpecify<IComplexClass, ComplexClass>();
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
            MockConfigureSpecify<IBasicClass, BasicClass>();
            MockConfigureSpecify<IComplexClass, ComplexClass>();

            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var result = dependencyInjector.Create<IComplexClass>();
            Assert.NotNull(result);
        }

        [Test]
        public void GivenCreate_WhenDoubleDependencyClassIsSpecifiedAndDependenciesAreConfigured_ThenCreatesClass()
        {
            MockConfigureSpecify<IBasicClass, BasicClass>();
            MockConfigureSpecify<IComplexClass, ComplexClass>();
            MockConfigureSpecify<IMoreComplexClass, MoreComplexClass>();

            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var result = dependencyInjector.Create<IMoreComplexClass>();
            Assert.NotNull(result);
        }

        [Test]
        public void GivenCreate_WhenConfigureAsSingleton_ThenCreatesOneObjectForEntireDILifecycle()
        {
            MockConfigureSpecify<IBasicClass, BasicClass>(ConfigurationScope.Singleton);
            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var result = dependencyInjector.Create<IBasicClass>();
            var secondResult = dependencyInjector.Create<IBasicClass>();
            Assert.AreEqual(result, secondResult);
        }

        [Test]
        public void GivenCreate_WhenConfigureAsScoped_ThenCreatesOneObjectForRequestFlow()
        {
            MockConfigureSpecify<IBasicClass, BasicClass>(ConfigurationScope.Scoped);
            MockConfigureSpecify<IComplexClass, ComplexClass>(ConfigurationScope.Scoped);
            MockConfigureSpecify<IMoreComplexClass, MoreComplexClass>(ConfigurationScope.Scoped);

            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var moreComplexClass = (IMoreComplexClass)dependencyInjector.Create<IMoreComplexClass>();
            var anotherMoreComplexClass = (IMoreComplexClass)dependencyInjector.Create<IMoreComplexClass>();

            Assert.AreNotEqual(moreComplexClass, anotherMoreComplexClass);
            Assert.AreNotEqual(moreComplexClass.MyBasicClass, anotherMoreComplexClass.MyBasicClass);
            Assert.AreEqual(moreComplexClass.MyBasicClass, moreComplexClass.MyComplexClass.MyBasicClass);
        }

        [Test]
        public void GivenCreate_WhenConfigureAsTransient_ThenCreatesNewObjectObjectEachTime()
        {
            MockConfigureSpecify<IBasicClass, BasicClass>();
            MockConfigureSpecify<IComplexClass, ComplexClass>();
            MockConfigureSpecify<IMoreComplexClass, MoreComplexClass>();

            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var moreComplexClass = (IMoreComplexClass)dependencyInjector.Create<IMoreComplexClass>();
            var anotherMoreComplexClass = (IMoreComplexClass)dependencyInjector.Create<IMoreComplexClass>();

            Assert.AreNotEqual(moreComplexClass, anotherMoreComplexClass);
            Assert.AreNotEqual(moreComplexClass.MyBasicClass, anotherMoreComplexClass.MyBasicClass);
            Assert.AreNotEqual(moreComplexClass.MyBasicClass, moreComplexClass.MyComplexClass.MyBasicClass);
        }

        [Test]
        public void GivenCreate_WhenConfigurationIsConstructorFunction_ThenCreatesClass()
        {
            MockConfigureConstructorFunction<IBasicClass>(() => new BasicClass());
            var dependencyInjector = new DependencyInjector(_mockConfiguration);

            var basicClass = dependencyInjector.Create<IBasicClass>();
            Assert.NotNull(basicClass);
            Assert.AreEqual(typeof(BasicClass), basicClass.GetType());
        }

        #region helper

        public void MockConfigureSpecify<TConfiguration, TSpecification>(ConfigurationScope scope = ConfigurationScope.Transient)
        {
            var configurationType = typeof(TConfiguration);
            var specificationType = typeof(TSpecification);
            var injection = new InjectionSpecification
            {
                ConfigurationType = configurationType,
                SpecificationType = specificationType,
                Constructor = specificationType.GetConstructors()[0],
                ConfigurationScope = scope,
                ConfigurationStyle = ConfigurationStyle.SpecificationType
            };

            A.CallTo(() => _mockConfiguration.IsConfigured(configurationType)).Returns(true);
            A.CallTo(() => _mockConfiguration.GetInjectionSpecification(configurationType)).Returns(injection);
        }

        public void MockConfigureConstructorFunction<TConfiguration>(Func<TConfiguration> constructorFunction, ConfigurationScope scope = ConfigurationScope.Transient)
        {
            var configurationType = typeof(TConfiguration);
            var injection = new InjectionSpecification
            {
                ConfigurationType = configurationType,
                ConstructorFunction = constructorFunction,
                ConfigurationScope = scope,
                ConfigurationStyle = ConfigurationStyle.ConstructorFunction
            };

            A.CallTo(() => _mockConfiguration.IsConfigured(configurationType)).Returns(true);
            A.CallTo(() => _mockConfiguration.GetInjectionSpecification(configurationType)).Returns(injection);
        }

        #endregion helper
    }
}