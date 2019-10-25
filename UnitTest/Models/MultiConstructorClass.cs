using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.Models
{
    internal class MultiConstructorClass : IMultiConstructorClass
    {
        public MultiConstructorClass(IBasicClass basicClass)
        {
        }

        public MultiConstructorClass()
        {
        }
    }
}