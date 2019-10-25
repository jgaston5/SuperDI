using System;
using System.Linq;
using System.Reflection;

namespace DI
{
    public class DependencyInjector
    {
        private readonly DependencyInjectorConfiguration _configuration;

        public DependencyInjector()
        {
            _configuration = new DependencyInjectorConfiguration();
        }

        public DependencyInjector(Action<DependencyInjectorConfiguration> configure)
        {
            _configuration = new DependencyInjectorConfiguration();
            configure(_configuration);
        }

        public object Create<TConfiguration>()
        {
            if (_configuration.IsConfigured<TConfiguration>())
            {
                var specificationType = _configuration.GetInjectionSpecification<TConfiguration>();
                return Create(specificationType);
            }
            throw new ArgumentException($"Type is not specified: ${typeof(TConfiguration)}");
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
            var parameterInjectionSpecification = _configuration.GetInjectionSpecification(parameterType);

            if (parameterInjectionSpecification == null)
            {
                throw new ArgumentException($"Dependency of {parameterType.Name} is not configured.");
            }

            return Create(parameterInjectionSpecification);
        }
    }
}