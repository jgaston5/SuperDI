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
    public class DependencyInjectorIntegrationTests
    {
        [Test]
        public void GivenCreate_WhenConfigureInterfaceSpecifyBasicClass_ThenCreatesClass()
        {
            var dependencyInjector = new DependencyInjector(configuration =>
            {
                configuration.ConfigureTransient<IBasicClass, BasicClass>();
            });

            var basicClass = dependencyInjector.Create<IBasicClass>();
            Assert.NotNull(basicClass);
            Assert.AreEqual(typeof(BasicClass), basicClass.GetType());
        }

        [Test]
        public void GivenCreate_WhenConfigureBaseClassSpecifyBasicSubClass_ThenCreatesClass()
        {
            var configuration = new DependencyInjectorConfiguration();
            configuration.ConfigureTransient<BasicClass, BasicSubClass>();
            var dependencyInjector = new DependencyInjector(configuration);

            var result = dependencyInjector.Create<BasicClass>();
            Assert.NotNull(result);
            Assert.AreEqual(typeof(BasicSubClass), result.GetType());
        }
    }
}