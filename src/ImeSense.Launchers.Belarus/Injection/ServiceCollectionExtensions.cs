using System.Net.Http.Headers;

using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;
using ImeSense.Launchers.Belarus.Core.Validators;
using ImeSense.Launchers.Belarus.Manager;
using ImeSense.Launchers.Belarus.ViewModels;
using ImeSense.Launchers.Belarus.ViewModels.Validators;
using ImeSense.Launchers.Belarus.Views;

using Microsoft.Extensions.DependencyInjection;

namespace ImeSense.Launchers.Belarus.Injection;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddSingleton<IWindowManager, WindowManager>();
        services.AddHttpClient<IFileDownloadManager, FileDownloadManager>()
           .ConfigurePrimaryHttpMessageHandler(() => {
               return new HttpClientHandler {
                   SslProtocols = System.Security.Authentication.SslProtocols.Tls12
               }; ;
           }).ConfigureHttpClient(ConfigureClient);
        services.AddSingleton<UserManager>();
        services.AddSingleton<InitializerManager>();
        services.AddSingleton<IApplicationLocaleManager, AxamlLocaleManager>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddHttpClient<IGitStorageApiService, GitHubApiService>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler {
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                }; ;
            })
            .ConfigureHttpClient(ConfigureClient);
        services.AddTransient<IReleaseComparerService<GitHubRelease>, ReleaseComparerService>();
        services.AddTransient<IUpdaterService, UpdaterService>();
        services.AddTransient<IDownloadResourcesService, DownloadResourcesService>();

        return services;
    }

    public static IServiceCollection AddPresetationServices(this IServiceCollection services)
    {
        services.AddViewModels();
        services.AddViews();

        return services;
    }

    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<SplashScreenViewModel>();
        services.AddTransient<LinkViewModel>();
        services.AddTransient<NewsSliderViewModel>();
        services.AddSingleton<LauncherViewModel>();
        services.AddTransient<DownloadMenuViewModel>();
        services.AddTransient<GameMenuViewModel>();
        services.AddTransient<AuthorizationViewModel>();
        services.AddSingleton<StartGameViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        return services;
    }

    public static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddTransient<AuthorizationView>();
        services.AddSingleton<DownloadMenuView>();
        services.AddSingleton<GameMenuView>();
        services.AddSingleton<LauncherView>();
        services.AddSingleton<LinkView>();
        services.AddSingleton<NewsSliderView>();
        services.AddTransient<NewsView>();
        services.AddTransient<SplashScreenView>();
        services.AddSingleton<StartGameView>();

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddTransient<GameDirectoryValidator>();
        services.AddTransient<AuthenticationViewModelValidator>();
        services.AddTransient<StartGameViewModelValidator>();
        services.AddTransient<IAuthenticationValidator, AuthenticationValidator>();
        services.AddTransient<IStartGameValidator, StartGameValidator>();
        return services;
    }

    private static void ConfigureClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = UriStorage.BelarusApiUri;
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
    }

}
