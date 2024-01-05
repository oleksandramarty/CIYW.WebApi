using Microsoft.Extensions.DependencyInjection;

namespace CIYW.Repositories;

public class ContextServiceLocator
{
    private readonly IServiceProvider serviceProvider;

    public ContextServiceLocator(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public T GetService<T>()
    {
        return serviceProvider.GetService<T>();
    }
}