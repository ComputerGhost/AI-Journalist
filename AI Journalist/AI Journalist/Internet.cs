using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AI_Journalist
{
    class Internet
    {
        static HttpClient Client = new HttpClient();


        public async Task<string> GetTextAsync(string url)
        {
            var response = await Client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<byte[]> GetBinaryAsync(string url)
        {
            var response = await Client.GetAsync(url);
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
