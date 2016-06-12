using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TileEvents
{
    public class SliderCtrlClient : IDisposable
    {
        private HttpClient _client;

        public SliderCtrlClient(string uri)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(uri);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        ~SliderCtrlClient()
        {
            Dispose();
        }

        public async Task<int> PageAsync()
        {
            var response = await _client.GetAsync("api/slide/page");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<int>();
            return await Task.FromResult<int>(-1);
        }

        public async Task<bool> StartAsync()
        {
            var response = await _client.PostAsync("api/slide/start", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> StopAsync()
        {
            var response = await _client.PostAsync("api/slide/stop", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> NextAsync()
        {
            var response = await _client.PostAsync("api/slide/next", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PrevAsync()
        {
            var response = await _client.PostAsync("api/slide/prev", null);
            return response.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            _client?.Dispose();
            _client = null;
        }
    }
}
