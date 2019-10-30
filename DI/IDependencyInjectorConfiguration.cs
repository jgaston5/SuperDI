using System;

namespace DI
{
    public interface IDependencyInjectorConfiguration
    {
        bool IsConfigured<TConfiguration>();

        void Configure<TConfiguration, TSpecification>();

        InjectionSpecification GetInjectionSpecification<TConfiguration>();

        InjectionSpecification GetInjectionSpecification(Type configurationType);

        bool IsConfigured(Type configurationType);
    }
}