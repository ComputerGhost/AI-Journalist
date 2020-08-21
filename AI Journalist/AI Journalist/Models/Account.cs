using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Journalist.Models
{
    class Account
    {
        public string Username;
        public string Description;
        public string PersonName;
        public string Emoticon;
        public string Pronoun;

        public static Account CreateFromUsername(string username)
        {
            // Search in our database
            foreach (var person in Program.Settings.People) {
                if (person.Username != username)
                    continue;
                return new Account {
                    Username = person.Username,
                    Description = person.AccountDescription,
                    PersonName = person.Name,
                    Emoticon = person.Emoticon,
                    Pronoun = person.Pronoun,
                };
            }

            // Not found
            return new Account {
                Username = username,
                PersonName = username,
            };
        }
    }
}
