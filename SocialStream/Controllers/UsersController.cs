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
using Facebook;

namespace SocialStream.Controllers
{
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
                catch (Exception e)
                {
                    Log log = new Log();
                    log.ErrorMessage = e.Message + " INNER EXCEPTION: " + e.InnerException;
                    log.ErrorStack = e.StackTrace;

                    data.Log.Add(log);
                    data.SaveChanges();
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
    }
}
