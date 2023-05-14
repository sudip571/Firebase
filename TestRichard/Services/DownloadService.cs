using Bometh.FirebaseDatabase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using TestRichard.Enums;
using TestRichard.Models;
using IHttpClientFactory = System.Net.Http.IHttpClientFactory;

namespace TestRichard.Services
{
    public interface IDownloadService
    {
        Task<List<DownloadHistory>> Download(DownloadRequest request);
    }
    public class DownloadService : IDownloadService
    {
        private readonly ILogger<DownloadService> _logger;       
        private readonly FirebaseSetting _firebaseSetting;       
        private readonly IHttpClientFactory _httpClientFactory;

        public DownloadService(ILogger<DownloadService> logger, IConfiguration configuration, IOptions<FirebaseSetting> options, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;          
            _firebaseSetting = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<DownloadHistory>> Download(DownloadRequest request)
        {
            var semaphore = new SemaphoreSlim(request.NoOfConcurentDownload,request.NoOfConcurentDownload);
            var tasks = new List<Task<DownloadHistory>>();
            foreach (var url in request.Urls)
            {
                // simulating fast or slow download speed
                var speed = string.Equals(request.DownloadSpeed,DownloadSpeed.Fast.ToString(),StringComparison.OrdinalIgnoreCase) ? 1000 : 5000;
                await Task.Delay(speed);
                tasks.Add(MakePararrelRequest(url, semaphore));
            }
            var downloadResponse = (await Task.WhenAll(tasks)).ToList();
            await AddDownloadHistoryData(downloadResponse);
            return downloadResponse;
        }
        private async Task<DownloadHistory> MakePararrelRequest(string url, SemaphoreSlim semaphoreSlim)
        {
            try
            {
                var downloadFolderPath = @"C://TestFolderr";
                await semaphoreSlim.WaitAsync();

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(10);
                //var request = new HttpRequestMessage(HttpMethod.Head, url);
                // var response = await httpClient.SendAsync(request);
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                Directory.CreateDirectory(downloadFolderPath);                
                var fileExtension = Path.GetExtension(url);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var downloadFilePath = Path.Combine(downloadFolderPath, fileName);
                using (var fileStream = new FileStream(downloadFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
              
                return new DownloadHistory
                {
                    Url = url,
                    Id = Guid.NewGuid(),
                    Status = response.IsSuccessStatusCode ? Status.Success.ToString() : Status.Failed.ToString(),
                    FileName = fileName,
                    DownloadedDate = DateTime.Now
                };
            }
            catch
            {
                _logger.LogError("Error occured while processing {url}", url.ToString());
                return new DownloadHistory
                {
                    Url = url,
                    Id = Guid.NewGuid(),
                    Status = Status.Failed.ToString()
                };
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        private async Task AddDownloadHistoryData(List<DownloadHistory> downloadHistory)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSetting.ApiKey));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(_firebaseSetting.UserName, _firebaseSetting.Password);

            var firebaseClient = new FirebaseClient(
                _firebaseSetting.DatabaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
                });

            // var firebaseClients = new FirebaseClient(_firebaseSetting.DatabaseUrl);
            
            var tableName = "DownloadHistory";
            // Check if the table already exists
            var tableExists = await firebaseClient.Child(tableName).OnceAsync<object>();
            if (tableExists != null && tableExists.Count == 0)
            {
                // create new table
                await firebaseClient.Child(tableName).PutAsync<object>(null);
                // Table does not exist, create new table and add data
               // await firebaseClient.Child(tableName).PostAsync(downloadHistory);
            }
            await firebaseClient.Child(tableName).PostAsync(downloadHistory);
        }

    }
    /* Download file showing status in browser
     * 
     * public async Task<IActionResult> DownloadFile(string url)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsByteArrayAsync();
                    var contentType = response.Content.Headers.ContentType.ToString();
                    var fileName = Path.GetFileName(url);
                    return File(content, contentType, fileName);
                }
            }
        }

    */
}
