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
        public void GivenConfigureTransient_WhenInterfaceNotConfigured_ThenReturnFalse()
        {
            Assert.False(_target.IsConfigured(typeof(IBasicClass)));
        }

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
        public void GivenConfigureTransient_WhenSpecificationDoesNotInheritConfigurationInterface_ThenThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _target.ConfigureTransient<IBasicClass, ComplexClass>());
        }

        [Test]
        public void GivenConfigureTransient_WhenSpecificationClassDoesNotInheritConfigurationClass_ThenThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _target.ConfigureTransient<BasicClass, ASecondBasicClass>());
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
    }
}