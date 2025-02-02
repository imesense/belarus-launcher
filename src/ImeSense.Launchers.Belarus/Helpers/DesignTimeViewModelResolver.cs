using Microsoft.Extensions.DependencyInjection;

namespace ImeSense.Launchers.Belarus.Helpers;

public class DesignTimeViewModelResolver<T>
    where T : notnull
{
    // ReSharper disable once UnusedMember.Global
    public T? ProvideValue(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetService<T>();
    }
}
