using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DI
{
    public class DependencyInjectorConfiguration : IDependencyInjectorConfiguration
    {
        private readonly Dictionary<Type, InjectionSpecification> _typeSpecifications = new Dictionary<Type, InjectionSpecification>();

        public bool IsConfigured<TConfiguration>()
        {
            return IsConfigured(typeof(TConfiguration));
        }

        public void ConfigureTransient<TConfiguration, TSpecification>(Func<TConfiguration> constructorFunction = null) where TSpecification : TConfiguration
        {
            if (constructorFunction != null)
            {
                Configure<TConfiguration, TSpecification>(constructorFunction, ConfigurationScope.Transient);
            }
            else
            {
                Configure<TConfiguration, TSpecification>(ConfigurationScope.Transient);
            }
        }

        public void ConfigureScoped<TConfiguration, TSpecification>(Func<TConfiguration> constructorFunction = null) where TSpecification : TConfiguration
        {
            if (constructorFunction != null)
            {
                Configure<TConfiguration, TSpecification>(constructorFunction, ConfigurationScope.Scoped);
            }
            else
            {
                Configure<TConfiguration, TSpecification>(ConfigurationScope.Scoped);
            }
        }

        public void ConfigureSingleton<TConfiguration, TSpecification>(Func<TConfiguration> constructorFunction = null) where TSpecification : TConfiguration
        {
            if (constructorFunction != null)
            {
                Configure<TConfiguration, TSpecification>(constructorFunction, ConfigurationScope.Singleton);
            }
            else
            {
                Configure<TConfiguration, TSpecification>(ConfigurationScope.Singleton);
            }
        }

        public InjectionSpecification GetInjectionSpecification(Type configurationType)
        {
            return IsConfigured(configurationType) ? _typeSpecifications[configurationType] : null;
        }

        public bool IsConfigured(Type configurationType)
        {
            return _typeSpecifications.ContainsKey(configurationType);
        }

        /// <summary>
        /// Gets the constructor with teh fewest params, in cases of multiple constructors with the same parameter count, it takes the first.
        /// </summary>
        /// <param name="constructors"></param>
        /// <returns></returns>
        private ConstructorInfo GetSimplestConstructor<TSpecification>()
        {
            var specificationType = typeof(TSpecification);
            var constructors = specificationType.GetConstructors();

            return constructors
                .GroupBy(x => x.GetParameters().Length)
                .OrderBy(x => x.Key)
                .First()
                .First();
        }

        private void Configure<TConfiguration, TSpecification>(ConfigurationScope scope) where TSpecification : TConfiguration
        {
            var injectionSpecification =
                GetNewInjectionSpecification<TConfiguration, TSpecification>(scope,
                    ConfigurationStyle.SpecificationType);

            injectionSpecification.Constructor = GetSimplestConstructor<TSpecification>();

            StoreInjectionSpecification(injectionSpecification);
        }

        private void Configure<TConfiguration, TSpecification>(Func<TConfiguration> constructorFunction, ConfigurationScope scope) where TSpecification : TConfiguration
        {
            if (constructorFunction == null)
            {
                throw new ArgumentException("Constructor function cannot be null.");
            }
            var injectionSpecification =
                GetNewInjectionSpecification<TConfiguration, TSpecification>(scope,
                    ConfigurationStyle.ConstructorFunction);

            injectionSpecification.ConstructorFunction = constructorFunction();

            StoreInjectionSpecification(injectionSpecification);
        }

        private void StoreInjectionSpecification(InjectionSpecification injectionSpecification)
        {
            if (IsConfigured(injectionSpecification.ConfigurationType))
            {
                _typeSpecifications[injectionSpecification.ConfigurationType] = injectionSpecification;
            }
            else
            {
                _typeSpecifications.Add(injectionSpecification.ConfigurationType, injectionSpecification);
            }
        }

        private InjectionSpecification GetNewInjectionSpecification<TConfiguration, TSpecification>(ConfigurationScope scope, ConfigurationStyle style)
        {
            var configurationType = typeof(TConfiguration);
            var specificationType = typeof(TSpecification);
            var injectionSpecification = new InjectionSpecification
            {
                ConfigurationType = configurationType,
                SpecificationType = specificationType,
                ConfigurationScope = scope,
                ConfigurationStyle = style,
            };

            return injectionSpecification;
        }
    }
}