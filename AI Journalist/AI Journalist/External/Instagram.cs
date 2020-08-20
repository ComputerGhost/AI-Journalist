using AI_Journalist.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AI_Journalist.External
{
    class Instagram
    {
        public async Task<IEnumerable<Update>> GetUpdates(Settings.AccountNode account)
        {
            var url = String.Format("https://www.instagram.com/{0}?__a=1", account.Username);
            var responseText = await new Internet().GetTextAsync(url);
            var responseData = JsonConvert.DeserializeObject(responseText);

            // todo: iterate updates and yield return
        }
    }
}
