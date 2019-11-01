using System;

namespace DI
{
    public interface IDependencyInjectorConfiguration
    {
        void ConfigureTransient<TConfiguration, TSpecification>(Func<TConfiguration> constructorFunction = null)
            where TSpecification : TConfiguration;

        void ConfigureScoped<TConfiguration, TSpecification>(Func<TConfiguration> constructorFunction = null)
            where TSpecification : TConfiguration;

        void ConfigureSingleton<TConfiguration, TSpecification>(Func<TConfiguration> constructorFunction = null)
            where TSpecification : TConfiguration;

        InjectionSpecification GetInjectionSpecification(Type configurationType);

        bool IsConfigured(Type configurationType);
    }
}