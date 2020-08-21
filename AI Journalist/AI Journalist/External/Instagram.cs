using AI_Journalist.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AI_Journalist.External
{
    class Instagram
    {
        public List<Update> GetUpdates(string username, int since_timestamp)
        {
            var updates = new List<Update>();
            foreach (var updateNode in DownloadUpdates(username)) {
                if (updateNode.taken_at_timestamp <= since_timestamp)
                    break;
                updates.Add(ParseUpdate(updateNode));
            }
            return updates;
        }

        IEnumerable<dynamic> DownloadUpdates(string username)
        {
            var fetchUri = String.Format("https://www.instagram.com/{0}/?__a=1", username);
            var responseText = new Internet().GetText(fetchUri);
            dynamic data = JsonConvert.DeserializeObject(responseText);
            foreach (var update in data.graphql.user.edge_owner_to_timeline_media.edges)
                yield return update.node;
        }

        Update ParseUpdate(dynamic updateNode)
        {
            var update = new Update {
                SourceUrl = string.Format("https://instagram.com/p/{0}/", updateNode.shortcode),
                Timestamp = updateNode.taken_at_timestamp,
                Author = Account.CreateFromUsername((string)updateNode.owner.username)
            };

            // Caption is not always set
            if (updateNode.edge_media_to_caption.edges.Count > 0) {
                update.Caption = updateNode.edge_media_to_caption.edges[0].node.text;
                foreach (Match match in Regex.Matches(update.Caption, @"@[\w\.]*"))
                    update.Tagged.Add(Account.CreateFromUsername(match.Value));
            }

            // Sidecar is used for multi-image posts; otherwise, data is in post.
            if (updateNode["edge_sidecar_to_children"] != null) {
                foreach (var media in updateNode.edge_sidecar_to_children.edges)
                    update.Medias.Add(ParseMedia(media.node));
            }
            else {
                update.Medias.Add(ParseMedia(updateNode));
            }

            return update;
        }

        Media ParseMedia(dynamic mediaNode)
        {
            var media = new Media {
                SourceUrl = (bool)mediaNode.is_video ? mediaNode.video_url : mediaNode.display_url,
                DisplayUrl = mediaNode.display_url,
                IsVideo = mediaNode.is_video,
            };

            foreach (var tag in mediaNode.edge_media_to_tagged_user.edges)
                media.Tagged.Add(Account.CreateFromUsername((string)tag.node.user.username));

            return media;
        }

    }
}
