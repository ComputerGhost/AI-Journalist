using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AI_Journalist
{
    class Internet
    {
        static HttpClient Client = new HttpClient();

        /* Generic request code */

        public string RequestText(HttpRequestMessage request)
        {
            return RequestTextAsync(request).Result;
        }
        public async Task<string> RequestTextAsync(HttpRequestMessage request)
        {
            var response = await Client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        /* Shortcuts for GET requests */

        public string GetText(string url)
        {
            return GetTextAsync(url).Result;
        }
        public async Task<string> GetTextAsync(string url)
        {
            var response = await Client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public byte[] GetBinary(string url)
        {
            return GetBinaryAsync(url).Result;
        }
        public async Task<byte[]> GetBinaryAsync(string url)
        {
            var response = await Client.GetAsync(url);
            return await response.Content.ReadAsByteArrayAsync();
        }

        public Stream GetStream(string url)
        {
            return GetStreamAsync(url).Result;
        }
        public async Task<Stream> GetStreamAsync(string url)
        {
            return await Client.GetStreamAsync(url);
        }
    }
}
