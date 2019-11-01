using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DI
{
    public class InjectionSpecification
    {
        public Type ConfigurationType { get; set; }
        public Type SpecificationType { get; set; }
        public ConstructorInfo Constructor { get; set; }
        public ConfigurationScope ConfigurationScope { get; set; }
        public ConfigurationStyle ConfigurationStyle { get; set; }
        public dynamic ConstructorFunction { get; set; }
    }
}