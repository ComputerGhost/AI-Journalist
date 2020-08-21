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

        public struct AccountNode
        {
            public string Username;
            public int LastUpdate;
        }
        public AccountNode[] FollowedAccounts;

        public struct PeopleNode
        {
            public string Username;
            public string AccountDescription;
            public string Name;
            public string Emoticon;
            public string Pronoun;
        }
        public PeopleNode[] People;

        public struct ModuleNode
        {
            public string TemplateName;
            public string Parameters;
            public string Path;
        }
        public ModuleNode[] Modules;

        public struct ArticleTemplateBlock
        {
            public string Type;
            public string Value;
        }
        public ArticleTemplateBlock ArticleTemplate;

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
