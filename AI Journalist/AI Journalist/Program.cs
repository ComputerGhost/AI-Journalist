using AI_Journalist.External;
using AI_Journalist.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace AI_Journalist
{
    class Program
    {
        const string SETTINGS_FILE = "settings.json";

        static async void Main(string[] args)
        {
            var settings = Settings.CreateFromFile(SETTINGS_FILE);

            // Load in any updates
            var updates = new List<Update>();
            foreach (var account in settings.FollowedAccounts) {
                foreach (var update in await new Instagram().GetUpdates(account)) {
                    if (update.When <= account.LastUpdate)
                        break;
                    updates.Add(update);
                }
            }

            // Call out to modules
            //

            // Send article out
            //

            Console.WriteLine(JsonConvert.SerializeObject(updates));

            settings.Save(SETTINGS_FILE);
        }
    }
}
