using System;
using DI;
using NUnit.Framework;
using UnitTest.Models;

/*
 *
 * Create when there is a functional param to pass in
  *need to handle scoping - new dependency everytime it is called for (transient), shared for Create Call (scoped), single dependency for entire runtime of injector
 *
 */

namespace UnitTest
{
    public class DependencyInjectorConfigurationTests
    {
        private DependencyInjectorConfiguration _target;

        [SetUp]
        public void Setup()
        {
            _target = new DependencyInjectorConfiguration();
        }

        [Test]
        public void GivenIsConfigured_WhenInterfaceNotConfigured_ThenReturnFalse()
        {
            Assert.False(_target.IsConfigured<IBasicClass>());
        }

        [Test]
        public void GivenIsConfigured_WhenInterfaceConfigured_ThenReturnTrue()
        {
            _target.Configure<IBasicClass, BasicClass>();

            Assert.True(_target.IsConfigured<IBasicClass>());
        }

        [Test]
        public void GivenCreate_WhenConfigurationIsDoubleSpecified_ThenReturnsLastSpecification()
        {
            _target.Configure<IBasicClass, BasicClass>();
            _target.Configure<IBasicClass, ASecondBasicClass>();

            var specification = _target.GetInjectionSpecification<IBasicClass>();
            Assert.NotNull(specification);
            Assert.AreEqual(typeof(ASecondBasicClass), specification.SpecificationType);
        }

        [Test]
        public void GivenConfigure_WhenSpecificationDoesNotInheritConfigurationInterface_ThenThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _target.Configure<IBasicClass, ComplexClass>());
        }

        [Test]
        public void GivenConfigure_WhenSpecificationClassDoesNotInheritConfigurationClass_ThenThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _target.Configure<BasicClass, ASecondBasicClass>());
        }

        [Test]
        public void GivenConfigure_WhenSpecificationClassIsConfigurationClass_ThenDoesNotThrowArgumentException()
        {
            Assert.DoesNotThrow(() => _target.Configure<BasicClass, BasicClass>());
        }

        [Test]
        public void GivenConfigure_WhenSpecificationClassIsSubClassOfConfigurationClass_ThenDoesNotThrowArgumentException()
        {
            Assert.DoesNotThrow(() => _target.Configure<BasicClass, BasicSubClass>());
        }

        [Test]
        public void GivenConfigure_WhenClassWithTwoConstructorsIsSpecifiedButNotConstructorInfoGiven_ThenUsesConstructorWithFewestParameters()
        {
            _target.Configure<IMultiConstructorClass, MultiConstructorClass>();
            var specification = _target.GetInjectionSpecification<IMultiConstructorClass>();
            Assert.NotNull(specification);
            Assert.AreEqual(0, specification.Constructor.GetParameters().Length);
        }
    }
}