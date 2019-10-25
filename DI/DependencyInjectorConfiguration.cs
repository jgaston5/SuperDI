using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DI
{
    public class DependencyInjectorConfiguration
    {
        private readonly Dictionary<Type, InjectionSpecification> _typeSpecifications = new Dictionary<Type, InjectionSpecification>();

        public bool IsConfigured<TConfiguration>()
        {
            return IsConfigured(typeof(TConfiguration));
        }

        public void Configure<TConfiguration, TSpecification>()
        {
            if (!IsValidConfiguration<TConfiguration, TSpecification>())
            {
                throw new ArgumentException("The specification type does not inherit or implement the configuration type");
            }

            var specificationType = typeof(TSpecification);
            var configurationType = typeof(TConfiguration);

            if (IsConfigured<TConfiguration>())
            {
                _typeSpecifications[configurationType].SpecificationType = specificationType;
                _typeSpecifications[configurationType].Constructor = GetSimplestConstructor(specificationType);
            }
            else
            {
                var injectionConfiguration = new InjectionSpecification
                {
                    ConfigurationType = configurationType,
                    SpecificationType = specificationType,
                    Constructor = GetSimplestConstructor(specificationType)
                };
                _typeSpecifications.Add(configurationType, injectionConfiguration);
            }
        }

        public InjectionSpecification GetInjectionSpecification<TConfiguration>()
        {
            return IsConfigured<TConfiguration>() ? _typeSpecifications[typeof(TConfiguration)] : null;
        }

        public InjectionSpecification GetInjectionSpecification(Type configurationType)
        {
            return IsConfigured(configurationType) ? _typeSpecifications[configurationType] : null;
        }

        private bool IsValidConfiguration<TConfiguration, TSpecification>()
        {
            var configurationType = typeof(TConfiguration);
            var specificationType = typeof(TSpecification);

            if (configurationType.IsInterface)
            {
                return specificationType.GetInterfaces()
                    .Any((x => x == configurationType));
            }
            return specificationType == configurationType || specificationType.IsSubclassOf(configurationType);
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
        private ConstructorInfo GetSimplestConstructor(Type specificationType)
        {
            var constructors = specificationType.GetConstructors();

            return constructors
                .GroupBy(x => x.GetParameters().Length)
                .OrderBy(x => x.Key)
                .First()
                .First();
        }
    }
}