using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Journalist.Models
{
    class Media
    {
        public string SourceUrl;
        public string DisplayUrl;
        public bool IsVideo;
        public List<Account> Tagged = new List<Account>();
    }
}
