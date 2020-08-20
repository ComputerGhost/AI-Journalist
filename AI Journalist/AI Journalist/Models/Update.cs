using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Journalist.Models
{
    struct Update
    {
        public string SourceUrl;
        public DateTime When;
        public Account Author;
        public Account[] OthersTagged;
        public Media[] Medias;
        public string Caption;
    }
}
