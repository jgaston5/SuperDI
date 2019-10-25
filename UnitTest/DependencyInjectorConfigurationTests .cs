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
        public void GivenCreate_WhenConfigurationIsDoubleSpecified_ThenCreatesLastSpecification()
        {
            _target.Configure<IBasicClass, BasicClass>();
            _target.Configure<IBasicClass, ASecondBasicClass>();

            var specification = _target.GetInjectionSpecification<IBasicClass>();
            Assert.NotNull(specification);
            Assert.AreEqual(typeof(ASecondBasicClass), specification.SpecificationType);
        }

        [Test]
        public void GivenConstructor_WhenSpecificationDoesNotInheritConfigurationInterface_ThenThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _target.Configure<IBasicClass, ComplexClass>());
        }

        [Test]
        public void GivenConstructor_WhenSpecificationClassDoesNotInheritConfigurationClass_ThenThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _target.Configure<BasicClass, ASecondBasicClass>());
        }

        [Test]
        public void GivenConstructor_WhenSpecificationClassIsConfigurationClass_ThenDoesNotThrowArgumentException()
        {
            Assert.DoesNotThrow(() => _target.Configure<BasicClass, BasicClass>());
        }

        [Test]
        public void GivenConstructor_WhenSpecificationClassIsSubClassOfConfigurationClass_ThenDoesNotThrowArgumentException()
        {
            Assert.DoesNotThrow(() => _target.Configure<BasicClass, BasicSubClass>());
        }
    }
}