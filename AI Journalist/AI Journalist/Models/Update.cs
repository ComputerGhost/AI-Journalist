using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Journalist.Models
{
    class Update
    {
        public string SourceUrl;
        public int Timestamp;
        public Account Author;
        public string Caption;
        public List<Account> Tagged = new List<Account>();
        public List<Media> Medias = new List<Media>();
    }
}
