using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AI_Journalist
{
    class Settings
    {

        // The compiler warns that our properties are never assigned,
        // but they are assigned via deserialization, so we disable the warning.
#pragma warning disable 0649

        public class AccountNode
        {
            public string Username;
            public int LastUpdate;
        }
        public AccountNode[] FollowedAccounts;

        public class ContextsNode
        {
            public struct PeopleNode
            {
                public string Username;
                public string AccountDescription;
                public string Name;
                public string Emoticon;
                public string Pronoun;
                public int[] TagIds;
            }
            public PeopleNode[] People;

            public class TranslatorNode
            {
                public string ClientId;
                public string ClientSecret;
            }
            public TranslatorNode Translator;

            public class CalendarNode
            {
                public class LinkedNode
                {
                    public string Id;
                    public string Timezone; // user-specified, because pulling from Google requires app review
                }
                public string ApplicationName;
                public string ApiKey;
                public int PastDays;
                public int FutureDays;
                public LinkedNode[] LinkedCalendars;
            }
            public CalendarNode Calendar;

            public class VisionNode
            {
                public string Endpoint;
                public string ApiKey;
            }
            public VisionNode Vision;
        }
        public ContextsNode Contexts;

        // Btw, we use Fluid, which is similar to Liquid template language
        public class ArticleNode
        {
            public class TemplateNode
            {
                public string Title;
                public string BodyFilename;
                public int[] CategoryIds;
                public int[] TagIds;
            }
            public TemplateNode Template;

            public class APINode
            {
                public string Endpoint;
                public string Username;
                public string Password;
            }
            public APINode API;

        }
        public ArticleNode Article;

        // Leaving the part where the compiler is wrong,
        // so we restore the warning checking.
#pragma warning restore 0649


        public static Settings CreateFromFile(string filename)
        {
            var contents = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Settings>(contents);
        }

        public void Save(string filename)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var contents = JsonConvert.SerializeObject(this, Formatting.Indented, settings);
            File.WriteAllText(filename, contents);
        }

    }
}
