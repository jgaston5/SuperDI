using System;
using System.Collections.Generic;
using System.Text;

namespace DI
{
    public enum ConfigurationScope
    {
        None = 0,
        Transient = 1,
        Scoped = 2,
        Singleton = 3
    }
}