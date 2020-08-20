using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AI_Journalist
{
    class Settings
    {
        public struct AccountNode
        {
            public string Username;
            public DateTime LastUpdate;
        }
        public AccountNode[] FollowedAccounts;

        public struct PeopleNode
        {
            public string Username;
            public string PersonName;
            public char PersonEmoticon;
            public string AccountDescription;
        }
        public PeopleNode[] People;

        public struct ModuleNode
        {
            public string TemplateName;
            public string Parameters;
            public string Path;
        }
        public ModuleNode[] Modules;


        public static Settings CreateFromFile(string filename)
        {
            var contents = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Settings>(contents);
        }

        public void Save(string filename)
        {
            var contents = JsonConvert.SerializeObject(this);
            File.WriteAllText(filename, contents);
        }

    }
}
