using System;

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
                var specificationType = _configuration.GetSpecification<TConfiguration>();
                var constructorInfo = specificationType.GetConstructors();

                return constructorInfo[0].Invoke(null);
            }
            throw new ArgumentException($"Type is not specified: ${typeof(TConfiguration)}");
        }
    }
}