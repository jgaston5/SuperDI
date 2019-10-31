namespace UnitTest.Models
{
    public interface IMoreComplexClass
    {
        IBasicClass MyBasicClass { get; }
        IComplexClass MyComplexClass { get; }
    }
}