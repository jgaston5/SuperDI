using System;

namespace DI
{
    public interface IDependencyInjectorConfiguration
    {
        void ConfigureTransient<TConfiguration, TSpecification>();

        void ConfigureScoped<TConfiguration, TSpecification>();

        void ConfigureSingleton<TConfiguration, TSpecification>();

        InjectionSpecification GetInjectionSpecification(Type configurationType);

        bool IsConfigured(Type configurationType);
    }
}