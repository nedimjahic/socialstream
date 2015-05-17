using SocialStream.Models;
using SocialStream.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Timers;
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
using InstaSharp;
using SocialStream.DAL;

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
        public void setactivetopic(string hashtag, string user)
        {
            Topic topic = new Topic(hashtag, user);

            BackgroundWorker timerBw = new BackgroundWorker();
            timerBw.DoWork += initiateTimer;
            timerBw.RunWorkerAsync(topic);

        }

        private void initiateTimer(object sender, DoWorkEventArgs e)
        {
            Timer timer = new Timer(5000);
            timer.Elapsed += (s, args) => getPosts(e.Argument as Topic);
            timer.Enabled = true;            
        }

        private void getPosts(Topic topic)
        {
            Debug.WriteLine("Retrieving posts @" + DateTime.Now);
            getTwitterPosts(topic);
            getInstagramPosts(topic);
        }

        private void getInstagramPosts(Topic topic)
        {
            using (DC data = new DC())
            {
                try
                {
                    var config = new InstagramConfig("ca0377d42c79476f8434de96bdd12ed1", "52db7c05852643ada953cb18cf63d581");

                    var tagEndpoint = new InstaSharp.Endpoints.Tags(config);

                    var response = tagEndpoint.Recent(topic.hashtag, null, null, 50);

                    var since = data.getMaxInstagramCreatedAt();

                    foreach (var instagramPost in response.Result.Data)
                    {
                        Debug.WriteLine(instagramPost.Caption.Text);
                        if (since == null || instagramPost.CreatedTime > since)
                        {
                            Post post = new Post();
                            
                            if (instagramPost.Caption.Text.Contains("#" + topic.hashtag))
                            {
                                post.text = instagramPost.Caption.Text.Replace("#" + topic.hashtag, "").Trim();
                            }
                            else if (instagramPost.Caption.Text.Contains(topic.hashtag))
                            {
                                post.text = instagramPost.Caption.Text.Replace(topic.hashtag, "").Trim();
                            }

                            post.created_at = instagramPost.CreatedTime;
                            post.avatar = instagramPost.User.ProfilePicture;
                            post.image = instagramPost.Images.StandardResolution.Url;
                            post.state = "unprocessed";
                            post.source = "instagram";
                            post.username = instagramPost.User.Username;
                            post.userId = Convert.ToInt32(topic.user);

                            data.Post.Add(post);
                            data.SaveChanges();
                        }
                    }
                }
                catch (Exception e)
                {
                    Log log = new Log();
                    log.ErrorMessage = e.Message + " INNER EXCEPTION: " + e.InnerException;
                    log.ErrorStack = e.StackTrace;

                    data.Log.Add(log);
                    data.SaveChanges();
                }
            }
        }

        private void getTwitterPosts(Topic topic)
        {
            using (DC data = new DC())
            {
                try
                {
                    TwitterCredentials.SetCredentials("2968667255-kspusRMoX1DnYhEtYtnT0EFW2ogC0BGCU3CkNAA",
                                                  "dh6ljkbbz88sztYjxxgW0TYJaClUApygTq5BMAg6Z508g",
                                                  "ctWg2q3bG9tjKfilIJo2fvE5N",
                                                  "vVsyqRl3ncoGRi1FPMbLnWkwi7yaqTMrTMHXX0sPnV9dYWfA9G");

                    var searchParameter = Search.CreateTweetSearchParameter(topic.hashtag);
                    var tweets = Search.SearchTweets(searchParameter);
                    var since = data.getMaxTWeetCreatedAt();
                    Debug.WriteLine("--" + since);
                    foreach (var tweet in tweets)
                    {
                        if(since == null || tweet.CreatedAt > since)
                        {
                            Post post = new Post();

                            if (tweet.Text.Contains("#" + topic.hashtag))
                            {
                                post.text = tweet.Text.Replace("#" + topic.hashtag, "").Trim();
                            }
                            else if (tweet.Text.Contains(topic.hashtag))
                            {
                                post.text = tweet.Text.Replace(topic.hashtag, "").Trim();
                            }

                            post.created_at = tweet.CreatedAt;
                            post.avatar = tweet.Creator.ProfileImageUrl;
                            post.state = "unprocessed";
                            post.source = "twitter";
                            post.username = tweet.Creator.UserIdentifier.ScreenName;
                            post.userId = Convert.ToInt32(topic.user);

                            data.Post.Add(post);
                            data.SaveChanges(); 
                        }                                               
                    }
                }
                catch (Exception e)
                {
                    Log log = new Log();
                    log.ErrorMessage = e.Message + " INNER EXCEPTION: " + e.InnerException;
                    log.ErrorStack = e.StackTrace;

                    data.Log.Add(log);
                    data.SaveChanges();
                }                
            }            
        }
                
        private void initiateTwitterStream(object sender, DoWorkEventArgs e)
        {
            using (socialstream_developmentEntities data = new socialstream_developmentEntities())
            {
                
            }
            TwitterCredentials.SetCredentials("2968667255-kspusRMoX1DnYhEtYtnT0EFW2ogC0BGCU3CkNAA",
                                              "dh6ljkbbz88sztYjxxgW0TYJaClUApygTq5BMAg6Z508g",
                                              "ctWg2q3bG9tjKfilIJo2fvE5N",
                                              "vVsyqRl3ncoGRi1FPMbLnWkwi7yaqTMrTMHXX0sPnV9dYWfA9G");
            
            var filteredStream = Stream.CreateFilteredStream();

            Topic topic = e.Argument as Topic;

            filteredStream.AddTrack(topic.hashtag);
            filteredStream.MatchingTweetReceived += (s, args) => tweetReceived(s, args, (sender as BackgroundWorker), topic.hashtag, topic.user);
            {

            };

            filteredStream.StartStreamMatchingAllConditions();
        }

        private void tweetReceived(object s, Tweetinvi.Core.Events.EventArguments.MatchedTweetReceivedEventArgs args, BackgroundWorker backgroundWorker, String topic, String user)
        {
            using (socialstream_developmentEntities data = new socialstream_developmentEntities())
            {
                try
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
                    post.source = "twitter";
                    post.username = tweet.Creator.UserIdentifier.ScreenName;
                    post.userId = Convert.ToInt32(user);

                    data.Post.Add(post);
                    data.SaveChanges();
                }
                catch (Exception e)
                {
                    Log log = new Log();
                    log.ErrorMessage = e.Message + " INNER EXCEPTION: " + e.InnerException;
                    log.ErrorStack = e.StackTrace;

                    data.Log.Add(log);
                    data.SaveChanges();
                }                
            }
        }
    }
}
