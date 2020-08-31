using System;
using System.Collections.Generic;
using System.Text;
using static AI_Journalist.Contexts.Context;

namespace AI_Journalist.Contexts
{
    class People
    {
        Settings.ContextsNode.PeopleNode[] KnownAccounts;

        public People(Settings.ContextsNode.PeopleNode[] people)
        {
            KnownAccounts = people;
        }

        public void AddContext(Context context)
        {
            context.Author = FindAccount(context.Source.Author);

            foreach (var tagged in context.Source.Tagged)
                context.TaggedInCaption.Add(FindAccount(tagged));

            foreach (var media in context.Source.Medias)
                foreach (var tagged in media.Tagged)
                    context.TaggedInMedia.Add(FindAccount(tagged));
        }

        // Pulls info for username, uses a sensible default if not found
        Account FindAccount(string username)
        {
            foreach (var person in KnownAccounts) {
                if (person.Username == username) {
                    return new Account {
                        Username = username,
                        Description = person.AccountDescription,
                        PersonName = person.Name,
                        Emoticon = person.Emoticon,
                        Pronoun = CreatePronoun(person.Pronoun ?? "they(s)"),
                        TagIds = person.TagIds ?? Array.Empty<int>(),
                    };
                }
            }
            return new Account {
                Username = username,
                PersonName = username,
                Pronoun = CreatePronoun("they(s)"),
            };
        }

        Pronoun CreatePronoun(string subjectForm)
        {
            switch (subjectForm) {
                case "he":
                    return new Pronoun {
                        Subject = "he",
                        Object = "him",
                        DependentPossessive = "his",
                        IndependentPossessive = "his",
                        Reflexive = "himself",
                    };
                case "i":
                    return new Pronoun {
                        Subject = "i",
                        Object = "me",
                        DependentPossessive = "my",
                        IndependentPossessive = "mine",
                        Reflexive = "myself",
                    };
                case "it":
                    return new Pronoun {
                        Subject = "it",
                        Object = "it",
                        DependentPossessive = "its",
                        IndependentPossessive = "its",
                        Reflexive = "itself",
                    };
                case "she":
                    return new Pronoun {
                        Subject = "she",
                        Object = "her",
                        DependentPossessive = "her",
                        IndependentPossessive = "hers",
                        Reflexive = "herself",
                    };
                case "they(s)":
                    return new Pronoun {
                        Subject = "they",
                        Object = "them",
                        DependentPossessive = "their",
                        IndependentPossessive = "theirs",
                        Reflexive = "themself",
                    };
                case "they(p)":
                    return new Pronoun {
                        Subject = "they",
                        Object = "them",
                        DependentPossessive = "their",
                        IndependentPossessive = "theirs",
                        Reflexive = "themselves",
                    };
                case "you(s)":
                    return new Pronoun {
                        Subject = "you",
                        Object = "you",
                        DependentPossessive = "your",
                        IndependentPossessive = "yours",
                        Reflexive = "yourself",
                    };
                case "you(p)":
                    return new Pronoun {
                        Subject = "you",
                        Object = "you",
                        DependentPossessive = "your",
                        IndependentPossessive = "yours",
                        Reflexive = "yourselves",
                    };
                case "we":
                    return new Pronoun {
                        Subject = "we",
                        Object = "us",
                        DependentPossessive = "our",
                        IndependentPossessive = "ours",
                        Reflexive = "ourselves",
                    };
                default:
                    throw new NotImplementedException("Pronoun not recognized.");
            }
        }
    }
}
