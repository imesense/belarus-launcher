using ImeSense.Launchers.Belarus.Core.Http;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;
using ImeSense.Launchers.Belarus.Core.Validators;
using ImeSense.Launchers.Belarus.Manager;
using ImeSense.Launchers.Belarus.ViewModels;
using ImeSense.Launchers.Belarus.ViewModels.Validators;
using ImeSense.Launchers.Belarus.Views;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImeSense.Launchers.Belarus.Injection;

internal static class ServiceCollectionExtensions
{
    private static readonly Action<IServiceProvider, HttpClient> _configureClient;

    static ServiceCollectionExtensions()
    {
        _configureClient = (serviceProvider, httpClient) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var serviceApiToken = configuration["SecretsBelarus:GitHubToken"];

            if (string.IsNullOrEmpty(serviceApiToken))
            {
                throw new InvalidOperationException("Failed to get the GitHub token");
            }

            HttpClientConfiguration.Configure(httpClient, UriStorage.BelarusApiUri, serviceApiToken);
        };
    }

    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddSingleton<IWindowManager, WindowManager>();
        services.AddHttpClient<IFileDownloadManager, FileDownloadManager>()
           .ConfigurePrimaryHttpMessageHandler(HttpClientConfiguration.CreateHttpHandler)
           .ConfigureHttpClient(_configureClient);
        services.AddSingleton<UserManager>();
        services.AddHttpClient<InitializerManager>()
           .ConfigurePrimaryHttpMessageHandler(HttpClientConfiguration.CreateHttpHandler)
           .ConfigureHttpClient(_configureClient);
        services.AddSingleton<IApplicationLocaleManager, LocalizationManager>();
        services.AddScoped<ISplashScreenManager, SplashScreenManager>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddHttpClient<IGitStorageApiService, GitHubApiService>()
            .ConfigurePrimaryHttpMessageHandler(HttpClientConfiguration.CreateHttpHandler)
            .ConfigureHttpClient(_configureClient);
        services.AddTransient<IReleaseComparerService<GitHubRelease>, ReleaseComparerService>();
        services.AddTransient<IUpdaterService, UpdaterService>();
        services.AddTransient<IDownloadResourcesService, DownloadResourcesService>();

        return services;
    }

    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
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
        services.AddTransient<DownloadMenuView>();
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
}
