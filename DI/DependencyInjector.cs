using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DI
{
    public class DependencyInjector : IDependencyInjector
    {
        private readonly IDependencyInjectorConfiguration _configuration;
        private readonly IDictionary<Type, object> _singletons = new Dictionary<Type, object>();

        private Dictionary<Type, object> _scopedObjects = new Dictionary<Type, object>();

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

        public TConfiguration Create<TConfiguration>()
        {
            _scopedObjects = new Dictionary<Type, object>();
            var configurationType = typeof(TConfiguration);
            if (_configuration.IsConfigured(configurationType))
            {
                var injectionSpecification = _configuration.GetInjectionSpecification(configurationType);
                return (TConfiguration)Create(injectionSpecification);
            }
            throw new ArgumentException($"Type is not configured: ${typeof(TConfiguration)}");
        }

        private object Create(InjectionSpecification injectionSpecification)
        {
            object result = GetPremadeObject(injectionSpecification);

            if (result != null)
            {
                return result;
            }

            switch (injectionSpecification.ConfigurationStyle)
            {
                case ConfigurationStyle.ConstructorFunction:
                    return injectionSpecification.ConstructorFunction();

                case ConfigurationStyle.SpecificationType:
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

                    result = constructor.Invoke(parameters);
                    break;

                default:
                    throw new ArgumentException("Invalid configuration");
            }

            StoreResult(injectionSpecification, result);
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

        private object GetPremadeObject(InjectionSpecification injectionSpecification)
        {
            if (injectionSpecification.ConfigurationScope == ConfigurationScope.Singleton
                && _singletons.ContainsKey(injectionSpecification.ConfigurationType))
            {
                return _singletons[injectionSpecification.ConfigurationType];
            }

            if (injectionSpecification.ConfigurationScope == ConfigurationScope.Scoped
                && _scopedObjects.ContainsKey(injectionSpecification.ConfigurationType))
            {
                return _scopedObjects[injectionSpecification.ConfigurationType];
            }

            return null;
        }

        private void StoreResult(InjectionSpecification injectionSpecification, object result)
        {
            if (injectionSpecification.ConfigurationScope == ConfigurationScope.Singleton)
            {
                _singletons.Add(injectionSpecification.ConfigurationType, result);
            }

            if (injectionSpecification.ConfigurationScope == ConfigurationScope.Scoped)
            {
                _scopedObjects.Add(injectionSpecification.ConfigurationType, result);
            }
        }
    }
}