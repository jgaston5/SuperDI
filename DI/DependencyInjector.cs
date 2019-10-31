using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DI
{
    public class DependencyInjector
    {
        private readonly IDependencyInjectorConfiguration _configuration;

        private Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
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

        public object Create<TConfiguration>()
        {
            _scopedObjects = new Dictionary<Type, object>();
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
            var retrieveObject = GetPremadeObject(injectionSpecification);

            if (retrieveObject != null)
            {
                return retrieveObject;
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