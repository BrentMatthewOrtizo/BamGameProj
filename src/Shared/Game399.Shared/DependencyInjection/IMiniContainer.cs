namespace DependencyInjection
{
    public interface IMiniContainer
    {
        T Resolve<T>();
    }
}