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
        public static Settings Settings = Settings.CreateFromFile(SETTINGS_FILE);

        static void Main(string[] args)
        {
            foreach (var account in Settings.FollowedAccounts)
                ProcessAccount(account);
        }

        static void ProcessAccount(Settings.AccountNode account)
        {
            Console.WriteLine("Processing account: {0}", account.Username);

            var updates = new Instagram().GetUpdates(account.Username, account.LastUpdate);
            foreach (var update in updates)
                ProcessUpdate(update);

            if (updates.Count > 0) {
                account.LastUpdate = updates[0].Timestamp;
                Settings.Save(SETTINGS_FILE);
            }
        }

        static void ProcessUpdate(Update update)
        {
            Console.WriteLine(" Processing update: {0}", update.Timestamp);
            foreach (var module in Settings.Modules) {
                //
            }
        }
    }
}
