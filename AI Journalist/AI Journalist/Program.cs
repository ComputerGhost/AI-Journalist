using Fluid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

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
                var post = BuildArticle(context);
                UploadArticle(post, context);
            }

            if (updates.Count > 0) {
                account.LastUpdate = updates[0].Timestamp;
                Settings.Save(SETTINGS_FILE);
            }
        }

        static Contexts.Context DecipherUpdate(Sources.Update update)
        {
            Console.WriteLine("  +- Deciphering update: {0}", update.Timestamp);

            var context = new Contexts.Context(update);

            Console.WriteLine("    +- Associating usernames with people");
            var people = new Contexts.People(Settings.Contexts.People);
            people.AddContext(context);

            var calendar = new Contexts.Calendar(Settings.Contexts.Calendar);
            foreach (var linked in Settings.Contexts.Calendar.LinkedCalendars) {
                Console.WriteLine("    +- Looking for nearby events in calendar: {0}", linked.Id);
                calendar.AddContext(context, linked);
            }

            Console.WriteLine("    +- Translating caption");
            var translator = new Contexts.Translator(Settings.Contexts.Translator);
            translator.AddContext(context);

            Console.WriteLine("    +- Recognizing objects in the media");
            var vision = new Contexts.Vision(Settings.Contexts.Vision);
            vision.AddContext(context);

            return context;
        }

        static Article.WordPress.Post BuildArticle(Contexts.Context context)
        {
            Console.WriteLine("    +- Building article");
            var post = new Article.WordPress.Post();

            // Generate from our template
            var titleTemplate = Settings.Article.Template.Title;
            post.Title = new Article.Template(titleTemplate).Render(context);
            var bodyTemplate = File.ReadAllText(Settings.Article.Template.BodyFilename);
            post.Body = new Article.Template(bodyTemplate).Render(context);

            // Default tags and categories
            post.CategoryIds.AddRange(Settings.Article.Template.CategoryIds);
            post.TagIds.AddRange(Settings.Article.Template.TagIds);

            // We want to have a tag for anyone in the media
            post.TagIds.AddRange(context.Author.TagIds);
            foreach (var tagged in context.TaggedInCaption)
                post.TagIds.AddRange(tagged.TagIds);
            foreach (var tagged in context.TaggedInMedia)
                post.TagIds.AddRange(tagged.TagIds);

            return post;
        }

        static void UploadArticle(Article.WordPress.Post post, Contexts.Context context)
        {
            Console.WriteLine("    +- Connecting to WordPress");

            var wordpress = new Article.WordPress(Settings.Article.API.Endpoint);
            wordpress.Authenticate(Settings.Article.API.Username, Settings.Article.API.Password);

            Console.WriteLine("    +- Uploading images");
            var uploadIds = new List<int>();
            foreach (var media in context.Source.Medias) {
                var filename = GetDecentNewFilename(context, media);
                var bytes = new Internet().GetBinary(media.DisplayUrl);
                using (var stream = new MemoryStream(bytes)) {
                    var uploadedMedia = wordpress.UploadMedia(filename, stream);

                    // Update old image data with new image data
                    post.Body = post.Body.Replace(media.DisplayUrl, uploadedMedia.Url);
                    post.Body = post.Body.Replace(media.Id, uploadedMedia.Id.ToString());
                    uploadIds.Add(uploadedMedia.Id);
                }
            }

            // Update image data with what we just uploaded
            post.Body = post.Body.Replace("#IMAGE_IDS#", String.Join(',', uploadIds));
            if (post.FeaturedMediaId == 0)
                post.FeaturedMediaId = uploadIds[0];

            Console.WriteLine("    +- Uploading article");
            wordpress.UploadArticle(post);
        }

        static string GetDecentNewFilename(Contexts.Context context, Sources.Update.Media media)
        {
            var author = new Regex(@"[^\w]").Replace(context.Author.Username, "");
            var timestamp = context.When.ToString("yyyyMMddHHmmss");
            var hash = String.Format("{0:X}", media.GetHashCode());
            return String.Format("{0}_{1}_{2}.jpg", author, timestamp, hash);
        }
    }
}
