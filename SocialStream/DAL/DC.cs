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
        public DbSet<UserHashtag> UserHashtag { get; set; }
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

        internal bool getUserStatus(int p)
        {
            int? duration = User.Find(p).duration;
            DateTime? created = User.Find(p).created_at;

            if (created.Value.AddHours(Convert.ToDouble(duration)) > DateTime.Now)
                return true;
            else
                return false;
        }

        internal Models.User getUser(int userId)
        {
            return User.Find(userId);                        
        }

        internal string getActiveTopicForUser(int userId)
        {
            return UserHashtag.Where(uh => uh.user_id == userId).Select(uh => uh.hashtag).FirstOrDefault();
        }

        internal IEnumerable<Models.Post> getAllPosts(int userId)
        {
            return Post.Where(pt => pt.userId == userId).ToList();
        }

        internal IEnumerable<Models.Post> getUnprocessesPosts()
        {
            return Post.Where(pt => pt.state == "unprocessed").OrderByDescending(pt => pt.created_at).ToList();
        }

        internal void allowPost(int postId)
        {
            Post post = Post.Find(postId);
            post.state = "allowed";
            SaveChanges();
        }

        internal void blockPost(int postId)
        {
            Post post = Post.Find(postId);
            post.state = "blocked";
            SaveChanges();
        }
    }
}