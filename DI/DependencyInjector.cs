using System;
using System.Linq;
using System.Reflection;

namespace DI
{
    public class DependencyInjector
    {
        private readonly IDependencyInjectorConfiguration _configuration;

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
            if (_configuration.IsConfigured<TConfiguration>())
            {
                var injectionSpecification = _configuration.GetInjectionSpecification<TConfiguration>();
                return Create(injectionSpecification);
            }
            throw new ArgumentException($"Type is not configured: ${typeof(TConfiguration)}");
        }

        private object Create(InjectionSpecification injectionSpecification)
        {
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

            return constructor.Invoke(parameters);
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