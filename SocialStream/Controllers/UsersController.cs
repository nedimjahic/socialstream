using SocialStream.Models;
using SocialStream.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Tweetinvi;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Tweetinvi.Core.Interfaces.Models;
using Tweetinvi.Core.Enum;
using System.Diagnostics;
using Tweetinvi.Core.Interfaces;
using System.ComponentModel;
using System.Web.Helpers;

namespace SocialStream.Controllers
{
    [EnableCors(origins: "http://localhost", headers: "*", methods: "*")]
    public class UsersController : ApiController
    {
        // GET api/users
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/users/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/users
        public string Post([FromBody]string value)
        {
            return "Hello World";
        }

        // PUT api/users/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/users/5
        public void Delete(int id)
        {
        }

        [HttpPost]
        public SocialStream.Models.User login(string email, string password)
        {
            using (socialstream_developmentEntities data = new socialstream_developmentEntities())
            {
                try
                {
                    string hashedPassword = Hash.getSHA1String(password);
                    SocialStream.Models.User user = data.User.Where(usr => usr.email == email && usr.password == hashedPassword && usr.status == "active").FirstOrDefault();
                    return user;
                }
                catch (Exception)
                {
                    return null;
                }                
            }
        }

        [HttpPost]
        public SocialStream.Models.User create(string email, string password, int duration)
        {
            using (socialstream_developmentEntities data = new socialstream_developmentEntities())
            {
                SocialStream.Models.User user = new SocialStream.Models.User();
                user.email = email;
                user.password = Hash.getSHA1String(password);
                user.created_at = DateTime.Now;
                user.duration = duration;
                user.status = "active";

                data.User.Add(user);
                data.SaveChanges();

                return user;
            }
        }

        [HttpPost]
        public void setactivetopic(string topic, string user)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += initiateTwitterStream;
            bw.RunWorkerAsync(topic);

        }

        private void initiateTwitterStream(object sender, DoWorkEventArgs e)
        {
            TwitterCredentials.SetCredentials("2968667255-kspusRMoX1DnYhEtYtnT0EFW2ogC0BGCU3CkNAA",
                                              "dh6ljkbbz88sztYjxxgW0TYJaClUApygTq5BMAg6Z508g",
                                              "ctWg2q3bG9tjKfilIJo2fvE5N",
                                              "vVsyqRl3ncoGRi1FPMbLnWkwi7yaqTMrTMHXX0sPnV9dYWfA9G");

            var filteredStream = Stream.CreateFilteredStream();

            var topic = e.Argument as String;
            
            filteredStream.AddTrack(topic);
            filteredStream.MatchingTweetReceived += (s, args) => tweetReceived(s, args, (sender as BackgroundWorker), topic);{
                
            };

            filteredStream.StartStreamMatchingAllConditions();
        }

        private void tweetReceived(object s, Tweetinvi.Core.Events.EventArguments.MatchedTweetReceivedEventArgs args, BackgroundWorker backgroundWorker, String topic)
        {
            using (socialstream_developmentEntities data = new socialstream_developmentEntities())
            {
                ITweet tweet = args.Tweet;
                Post post = new Post();

                if (args.Tweet.Text.Contains("#" + topic))
                {
                    post.text = tweet.Text.Replace("#" + topic, "").Trim();
                }
                else if (args.Tweet.Text.Contains(topic))
                {
                    post.text = tweet.Text.Replace(topic, "").Trim();
                }

                post.created_at = tweet.CreatedAt;
                post.avatar = tweet.Creator.ProfileImageUrl;
                post.state = "unprocessed";
                post.source = tweet.Source;
                post.username = tweet.Creator.UserIdentifier.ScreenName;

                data.Post.Add(post);
                data.SaveChanges();
            }
        }
    }
}
