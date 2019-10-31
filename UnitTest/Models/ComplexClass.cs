namespace UnitTest.Models
{
    public class ComplexClass : IComplexClass
    {
        public IBasicClass MyBasicClass { get; }

        public ComplexClass(IBasicClass basicClass)
        {
            MyBasicClass = basicClass;
        }
    }
}