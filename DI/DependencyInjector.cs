using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DI
{
    public class DependencyInjector
    {
        private readonly IDependencyInjectorConfiguration _configuration;

        private Dictionary<Type, object> singletons = new Dictionary<Type, object>();

        public DependencyInjector()
        {
            _configuration = new DependencyInjectorConfiguration();
        }

        public DependencyInjector(Action<IDependencyInjectorConfiguration> configure)
        {
            _configuration = new DependencyInjectorConfiguration();
            configure(_configuration);
        }

        public DependencyInjector(IDependencyInjectorConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object Create<TConfiguration>()
        {
            var configurationType = typeof(TConfiguration);
            if (_configuration.IsConfigured(configurationType))
            {
                var injectionSpecification = _configuration.GetInjectionSpecification(configurationType);
                return Create(injectionSpecification);
            }
            throw new ArgumentException($"Type is not configured: ${typeof(TConfiguration)}");
        }

        private object Create(InjectionSpecification injectionSpecification)
        {
            if (singletons.ContainsKey(injectionSpecification.SpecificationType) &&
                injectionSpecification.ConfigurationScope == ConfigurationScope.Singleton)
            {
                return singletons[injectionSpecification.SpecificationType];
            }

            var constructor = injectionSpecification.Constructor;
            var parameterInfos = constructor.GetParameters();
            object[] parameters = null;

            if (parameterInfos.Length > 0)
            {
                parameters = new object[parameterInfos.Length];
                for (int i = 0; i < parameterInfos.Length; i++)
                {
                    parameters[i] = CreateParameter(parameterInfos[i]);
                }
            }

            var result = constructor.Invoke(parameters);
            if (injectionSpecification.ConfigurationScope == ConfigurationScope.Singleton)
            {
                singletons.Add(injectionSpecification.SpecificationType, result);
            }
            return result;
        }

        private object CreateParameter(ParameterInfo parameterInfo)
        {
            var parameterType = parameterInfo.ParameterType;

            if (!_configuration.IsConfigured(parameterType))
            {
                throw new ArgumentException($"Dependency of {parameterType.Name} is not configured.");
            }

            var parameterInjectionSpecification = _configuration.GetInjectionSpecification(parameterType);

            return Create(parameterInjectionSpecification);
        }
    }
}