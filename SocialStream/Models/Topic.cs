using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialStream.Models
{
    public class Topic
    {
        public String hashtag { get; set; }
        public String user { get; set; }

        public Topic(String hashtag, String user)
        {
            this.hashtag = hashtag;
            this.user = user;
        }
    }
}