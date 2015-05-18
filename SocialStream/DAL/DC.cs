using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialStream.Models;
using System.Data.Entity;

namespace SocialStream.DAL
{
    public class DC : socialstream_developmentEntities
    {
        public DbSet<Post> Post { get; set; }
        public DbSet<Thread> Thread { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Log> Log { get; set; }

        internal DateTime? getMaxTWeetCreatedAt()
        {
            return Post.Where(p => p.source == "twitter").Max(p => p.created_at);
        }

        internal DateTime? getMaxInstagramCreatedAt()
        {
            return Post.Where(p => p.source == "instagram").Max(p => p.created_at);
        }

        internal bool getUserStatus(string p)
        {
            int? duration = User.Find(Convert.ToInt32(p)).duration;
            DateTime? created = User.Find(Convert.ToInt32(p)).created_at;

            if (created.Value.AddMinutes(Convert.ToDouble(duration)) > DateTime.Now)
                return true;
            else
                return false;
        }

        internal Models.User getUser(string id)
        {
            return User.Find(Convert.ToInt32(id));                        
        }
    }
}