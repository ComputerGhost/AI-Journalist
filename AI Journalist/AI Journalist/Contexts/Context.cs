using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Journalist.Contexts
{
    class Context
    {
        // From: Source
        public Sources.Update Source;
        public DateTime When;

        // From: People
        public class Pronoun
        {
            public string Subject;
            public string Object;
            public string DependentPossessive;
            public string IndependentPossessive;
            public string Reflexive;
        }
        public class Account
        {
            public string Username;
            public string Description;
            public string PersonName;
            public string Emoticon;
            public Pronoun Pronoun;
            public int[] TagIds = Array.Empty<int>();
        }
        public Account Author;
        public List<Account> TaggedInCaption = new List<Account>();
        public List<Account> TaggedInMedia = new List<Account>();

        // From: Calendar
        public struct Event
        {
            public DateTime StartTime;
            public DateTime EndTime;
            public bool IsAllDay;
            public string Title;
            public string Description;
        }
        public List<Event> PastEvents = new List<Event>();
        public List<Event> FutureEvents = new List<Event>();

        // From: Translator
        public string TranslatedCaption;
        public bool IsCaptionKorean;

        // From: Vision
        public struct MediaDescription
        {
            public string Description;
            public double Confidence;
            public Sources.Update.Media AssociatedMedia;
        }
        public List<MediaDescription> MediaDescriptions = new List<MediaDescription>();

        // Useful shortcuts for the template
        public List<MediaDescription> PictureDescriptions {
            get {
                return MediaDescriptions.Where(i => i.AssociatedMedia.IsVideo == false).ToList();
            }
        }
        public List<MediaDescription> VideoDescriptions {
            get {
                return MediaDescriptions.Where(i => i.AssociatedMedia.IsVideo == true).ToList();
            }
        }


        // We only construct from an update
        public Context(Sources.Update source)
        {
            Source = source;
            When = DateTime.UnixEpoch.AddSeconds(source.Timestamp);
        }
    }
}
