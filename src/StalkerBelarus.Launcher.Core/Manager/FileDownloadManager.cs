using System.Net.Http.Headers;

using Microsoft.Extensions.Logging;

namespace StalkerBelarus.Launcher.Core.Manager;

public class FileDownloadManager : IFileDownloadManager {
    private readonly ILogger<FileDownloadManager> _logger;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Default constructor
    /// </summary>
    public FileDownloadManager(ILogger<FileDownloadManager> logger, HttpClient httpClient) {
        _logger = logger;
        _httpClient = httpClient;
    }
    
    /// <summary>
    /// Downloads a file from the specified URL and saves it to the specified path.
    /// </summary>
    /// <param name="url">The URL of the file to download.</param>
    /// <param name="filePath">The local file path where the downloaded file will be saved.</param>
    /// <param name="status">An optional progress reporting mechanism to report download progress.</param>
    /// <param name="token">A cancellation token to cancel the download operation.</param>
    /// <returns>A task representing the asynchronous download operation.</returns>
    /// <remarks>
    /// This method supports resuming an interrupted download if the server supports the 'Range' header.
    /// It uses the 'Accept-Ranges' response header to check for server support before attempting to resume.
    /// If the server does not support resuming, the download will start from the beginning.
    /// </remarks>
    public async Task DownloadAsync(string url, string filePath, IProgress<int>? status, CancellationToken token = default) {
        try {
            _logger.LogInformation("Url: {Url}", url);

            const int bufferLength = 8192;
            
            // Determine the current position of the file for possible resuming (resume)
            var currentPosition = File.Exists(filePath) ? new FileInfo(filePath).Length : 0;

            // Add the 'Range' header only if the server supports resuming (Accept-Ranges).
            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            using var headResponse = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url), 
                HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
            var serverSupportsRange = headResponse.Headers.AcceptRanges.Contains("bytes");

            if (serverSupportsRange) {
                // If the server supports resuming, set the 'Range' header.
                request.Headers.Range = new RangeHeaderValue(currentPosition, null);
            }
            
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        
            await using var responseStream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            
            await using var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None);
        
            // Get the content length (file size) that will be downloaded
            var contentLength = currentPosition + response.Content.Headers.ContentLength ?? 0;
            var progress = -1;
            var buffer = new byte[bufferLength];
            int bytesReceived;
        
            while ((bytesReceived = await responseStream.ReadAsync(buffer.AsMemory(0, bufferLength), token)
                       .ConfigureAwait(false)) > 0) {
                // Write the received data to the file
                await fs.WriteAsync(buffer.AsMemory(0, bytesReceived), token).ConfigureAwait(false);
                
                // Update the current position of the file
                currentPosition += bytesReceived;
                
                // Calculate the download progress in percentage
                var oldProgress = progress;
                progress = (int)(currentPosition * 100 / contentLength);
                
                // Since the value ranges from 0 to 100, there is no need to update the interface
                // if the value has not changed. Notify the progress change using IProgress<int>
                if (oldProgress == progress) {
                    continue;
                }
                status?.Report(progress);
                _logger.LogInformation("URL [{Progress}]: {Url}", progress, url);
            }
        } catch (HttpRequestException ex) {
            _logger.LogError("{Message}", ex.Message);
            throw;
        } 
        catch (Exception ex) {
            _logger.LogError("{Message}", ex.Message);
            throw;
        }
    }
}
