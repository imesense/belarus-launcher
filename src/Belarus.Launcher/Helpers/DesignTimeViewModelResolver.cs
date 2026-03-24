using Microsoft.Extensions.DependencyInjection;

namespace Belarus.Launcher.Helpers;

public class DesignTimeViewModelResolver<T>
    where T : notnull
{
    // ReSharper disable once UnusedMember.Global
    public T? ProvideValue(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetService<T>();
    }
}
