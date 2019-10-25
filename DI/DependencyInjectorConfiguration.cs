using System;
using System.Collections.Generic;
using System.Linq;

namespace DI
{
    public class DependencyInjectorConfiguration
    {
        private readonly Dictionary<Type, Type> _typeSpecifications = new Dictionary<Type, Type>();

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

            if (IsConfigured<TConfiguration>())
            {
                _typeSpecifications[typeof(TConfiguration)] = typeof(TSpecification);
            }
            else
            {
                _typeSpecifications.Add(typeof(TConfiguration), typeof(TSpecification));
            }
        }

        public Type GetSpecification<TConfiguration>()
        {
            return IsConfigured<TConfiguration>() ? _typeSpecifications[typeof(TConfiguration)] : null;
        }

        public Type GetSpecification(Type configurationType)
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
    }
}