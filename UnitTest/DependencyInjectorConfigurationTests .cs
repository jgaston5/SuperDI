using System;
using DI;
using NUnit.Framework;
using UnitTest.Models;

namespace UnitTest
{
    public class DependencyInjectorConfigurationTests
    {
        private IDependencyInjectorConfiguration _target;

        [SetUp]
        public void Setup()
        {
            _target = new DependencyInjectorConfiguration();
        }

        [Test]
        public void GivenIsConfigured_WhenInterfaceNotConfigured_ThenReturnFalse()
        {
            Assert.False(_target.IsConfigured(typeof(IBasicClass)));
        }

        #region ConfigureSpecify

        [Test]
        public void GivenConfigureTransient_WhenSpecificationClassIsConfigurationClass_ThenDoesNotThrowArgumentException()
        {
            Assert.DoesNotThrow(() => _target.ConfigureTransient<BasicClass, BasicClass>());
        }

        [Test]
        public void GivenConfigureTransient_WhenSpecificationClassIsSubClassOfConfigurationClass_ThenDoesNotThrowArgumentException()
        {
            Assert.DoesNotThrow(() => _target.ConfigureTransient<BasicClass, BasicSubClass>());
        }

        [Test]
        public void GivenConfigureTransient_WhenInterfaceConfigured_ThenIsConfiguredReturnTrue()
        {
            _target.ConfigureTransient<IBasicClass, BasicClass>();

            Assert.True(_target.IsConfigured(typeof(IBasicClass)));
        }

        [Test]
        public void GivenConfigureTransient_WhenConfigurationIsDoubleSpecified_ThenReturnsLastSpecification()
        {
            var configurationType = typeof(IBasicClass);
            _target.ConfigureTransient<IBasicClass, BasicClass>();
            _target.ConfigureTransient<IBasicClass, ASecondBasicClass>();

            var specification = _target.GetInjectionSpecification(configurationType);

            Assert.NotNull(specification);
            Assert.AreEqual(typeof(IBasicClass), specification.ConfigurationType);
            Assert.AreEqual(typeof(ASecondBasicClass), specification.SpecificationType);
            Assert.AreEqual(ConfigurationScope.Transient, specification.ConfigurationScope);
        }

        [Test]
        public void GivenConfigureTransient_WhenConfigurationIsValid_ThenInjectionSpecificationIsTransient()
        {
            var configurationType = typeof(IBasicClass);

            _target.ConfigureTransient<IBasicClass, BasicClass>();

            var specification = _target.GetInjectionSpecification(configurationType);
            Assert.NotNull(specification);
            Assert.AreEqual(ConfigurationScope.Transient, specification.ConfigurationScope);
        }

        [Test]
        public void GivenConfigureScoped_WhenConfigurationIsValid_ThenInjectionSpecificationIsScoped()
        {
            var configurationType = typeof(IBasicClass);

            _target.ConfigureScoped<IBasicClass, BasicClass>();

            var specification = _target.GetInjectionSpecification(configurationType);
            Assert.NotNull(specification);
            Assert.AreEqual(ConfigurationScope.Scoped, specification.ConfigurationScope);
        }

        [Test]
        public void GivenConfigureSingleton_WhenConfigurationIsValid_ThenInjectionSpecificationIsSingleton()
        {
            var configurationType = typeof(IBasicClass);

            _target.ConfigureSingleton<IBasicClass, BasicClass>();

            var specification = _target.GetInjectionSpecification(configurationType);
            Assert.NotNull(specification);
            Assert.AreEqual(ConfigurationScope.Singleton, specification.ConfigurationScope);
        }

        [Test]
        public void GivenConfigureTransient_WhenClassWithTwoConstructorsIsSpecifiedButNotConstructorInfoGiven_ThenUsesConstructorWithFewestParameters()
        {
            var configurationType = typeof(IMultiConstructorClass);

            _target.ConfigureTransient<IMultiConstructorClass, MultiConstructorClass>();

            var specification = _target.GetInjectionSpecification(configurationType);
            Assert.NotNull(specification);
            Assert.AreEqual(0, specification.Constructor.GetParameters().Length);
        }

        #endregion ConfigureSpecify

        #region ConstructorFunctions

        [Test]
        public void GivenConfigureTransient_WhenConfigurationIsConstructorFunction_ThenInjectionSpecificationIsTransient()
        {
            var configurationType = typeof(IBasicClass);
            var sepecificationType = typeof(BasicClass);
            _target.ConfigureTransient<IBasicClass, BasicClass>(() => new BasicClass());
            var specification = _target.GetInjectionSpecification(configurationType);

            Assert.NotNull(specification);
            Assert.AreEqual(configurationType, specification.ConfigurationType);
            Assert.AreEqual(sepecificationType, specification.SpecificationType);
            Assert.AreEqual(ConfigurationScope.Transient, specification.ConfigurationScope);
            Assert.AreEqual(ConfigurationStyle.ConstructorFunction, specification.ConfigurationStyle);
            Assert.NotNull(specification.ConstructorFunction);
        }

        [Test]
        public void GivenConfigureScoped_WhenConfigurationIsIsConstructorFunction_ThenInjectionSpecificationIsScoped()
        {
            var configurationType = typeof(IBasicClass);
            var sepecificationType = typeof(BasicClass);
            _target.ConfigureScoped<IBasicClass, BasicClass>(() => new BasicClass());
            var specification = _target.GetInjectionSpecification(configurationType);

            Assert.NotNull(specification);
            Assert.AreEqual(configurationType, specification.ConfigurationType);
            Assert.AreEqual(sepecificationType, specification.SpecificationType);
            Assert.AreEqual(ConfigurationScope.Scoped, specification.ConfigurationScope);
            Assert.AreEqual(ConfigurationStyle.ConstructorFunction, specification.ConfigurationStyle);
            Assert.NotNull(specification.ConstructorFunction);
        }

        [Test]
        public void GivenConfigureSingleton_WhenConfigurationIsConstructorFunction_ThenInjectionSpecificationIsSingleton()
        {
            var configurationType = typeof(IBasicClass);
            var sepecificationType = typeof(BasicClass);
            _target.ConfigureSingleton<IBasicClass, BasicClass>(() => new BasicClass());
            var specification = _target.GetInjectionSpecification(configurationType);

            Assert.NotNull(specification);
            Assert.AreEqual(configurationType, specification.ConfigurationType);
            Assert.AreEqual(sepecificationType, specification.SpecificationType);
            Assert.AreEqual(ConfigurationScope.Singleton, specification.ConfigurationScope);
            Assert.AreEqual(ConfigurationStyle.ConstructorFunction, specification.ConfigurationStyle);
            Assert.NotNull(specification.ConstructorFunction);
        }

        #endregion ConstructorFunctions
    }
}