using Fluid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Encodings.Web;

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

            var updates = new Sources.Instagram().GetUpdates(account.Username, account.LastUpdate);
            foreach (var update in updates) {
                var context = DecipherUpdate(update);
                var article = WriteArticle(context);
            }

            if (updates.Count > 0) {
                account.LastUpdate = updates[0].Timestamp;
                //Settings.Save(SETTINGS_FILE);
            }
        }

        static Contexts.Context DecipherUpdate(Sources.Update update)
        {
            Console.WriteLine("  +- Deciphering update: {0}", update.Timestamp);

            var context = new Contexts.Context(update);

            Console.WriteLine("    +- Associating usernames with people", update.Timestamp);
            var people = new Contexts.People(Settings.Contexts.People);
            people.AddContext(context);

            var calendar = new Contexts.Calendar(Settings.Contexts.Calendar);
            foreach (var linked in Settings.Contexts.Calendar.LinkedCalendars) {
                Console.WriteLine("    +- Looking for nearby events in calendar: {0}", linked.Id);
                calendar.AddContext(context, linked);
            }

            Console.WriteLine("    +- Translating caption", update.Timestamp);
            var translator = new Contexts.Translator(Settings.Contexts.Translator);
            translator.AddContext(context);

            Console.WriteLine("    +- Recognizing objects in the media", update.Timestamp);
            var vision = new Contexts.Vision(Settings.Contexts.Vision);
            vision.AddContext(context);

            return context;
        }

        static string WriteArticle(Contexts.Context context)
        {
            Console.WriteLine("  +- Writing article");

            var parser = new Article.Template(Settings.ArticleTemplateFilename);
            return parser.Render(context);
        }
    }
}
