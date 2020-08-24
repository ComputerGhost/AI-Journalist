using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Journalist.Sources
{
    class Update
    {
        public class Media
        {
            public string SourceUrl;
            public string DisplayUrl;
            public bool IsVideo;
            public List<string> Tagged = new List<string>();
        }

        public string SourceUrl;
        public int Timestamp;
        public string Author;
        public string Caption;
        public List<string> Tagged = new List<string>();
        public List<Media> Medias = new List<Media>();
    }
}
