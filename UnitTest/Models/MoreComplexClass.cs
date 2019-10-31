namespace UnitTest.Models
{
    public class MoreComplexClass : IMoreComplexClass
    {
        public IBasicClass MyBasicClass { get; }
        public IComplexClass MyComplexClass { get; }

        public MoreComplexClass(IBasicClass basicClass, IComplexClass complexClass)
        {
            MyBasicClass = basicClass;
            MyComplexClass = complexClass;
        }
    }
}