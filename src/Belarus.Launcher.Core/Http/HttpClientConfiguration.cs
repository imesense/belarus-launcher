using System.Net.Http.Headers;
using System.Security.Authentication;

namespace Belarus.Launcher.Core.Http;

public static class HttpClientConfiguration
{
    public static HttpClientHandler CreateHttpHandler() => new()
    {
        SslProtocols = SslProtocols.Tls12
    };

    public static void Configure(HttpClient httpClient, Uri uri, string token, string acceptHeader = "application/json")
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(uri);

        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentException($"'{nameof(token)}' cannot be null or empty", nameof(token));
        }

        httpClient.BaseAddress = uri;
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Belarus.Launcher/1.0");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
