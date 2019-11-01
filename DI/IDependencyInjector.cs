namespace DI
{
    public interface IDependencyInjector
    {
        TConfiguration Create<TConfiguration>();
    }
}