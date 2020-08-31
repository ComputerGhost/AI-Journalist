using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WordPressPCL;

namespace AI_Journalist.Article
{
    class WordPress
    {
        public class Post
        {
            public string Title;
            public string Body;
            public List<int> TagIds = new List<int>();
            public List<int> CategoryIds = new List<int>();
            public int FeaturedMediaId;
        }

        public class Media
        {
            public int Id;
            public string Url;
        }


        WordPressClient Client;

        public WordPress(string endpoint)
        {
            Client = new WordPressClient(endpoint);
        }

        public async Task AuthenticateAsync(string username, string password)
        {
            await Client.RequestJWToken(username, password);
        }

        public async Task<Media> UploadMediaAsync(string filename, Stream stream)
        {
            var mediaItem = await Client.Media.Create(stream, filename);
            return new Media { Id = mediaItem.Id, Url = mediaItem.Guid.Raw };
        }

        public async Task UploadArticleAsync(Post post)
        {
            if (await Client.IsValidJWToken()) {
                await Client.Posts.Create(new WordPressPCL.Models.Post {
                    Title = new WordPressPCL.Models.Title(post.Title),
                    Content = new WordPressPCL.Models.Content(post.Body),
                    Status = WordPressPCL.Models.Status.Pending,
                    Tags = post.TagIds.ToArray(),
                    Categories = post.CategoryIds.ToArray(),
                    FeaturedMedia = post.FeaturedMediaId,
                    
                });
            }
            else
                throw new Exception("Authorization for WordPress is required.");
        }

        // Synchronous versions

        public void Authenticate(string username, string password)
        {
            AuthenticateAsync(username, password).Wait();
        }

        public Media UploadMedia(string filename, Stream stream)
        {
            return UploadMediaAsync(filename, stream).Result;
        }

        public void UploadArticle(Post post)
        {
            UploadArticleAsync(post).Wait();
        }

    }
}
