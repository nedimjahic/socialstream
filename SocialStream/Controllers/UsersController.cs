using SocialStream.Models;
using SocialStream.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

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
        public User login(string email, string password)
        {
            using (socialstream_developmentEntities data = new socialstream_developmentEntities())
            {
                try
                {
                    string hashedPassword = Hash.getSHA1String(password);
                    User user = data.User.Where(usr => usr.email == email && usr.password == hashedPassword && usr.status == "active").FirstOrDefault();
                    return user;
                }
                catch (Exception)
                {
                    return null;
                }                
            }
        }

        [HttpPost]
        public User create(string email, string password, int duration)
        {
            using (socialstream_developmentEntities data = new socialstream_developmentEntities())
            {
                User user = new User();
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
        public void setactivetopic(string topic)
        {
            //TODO implement setactivetopic which sets active topic
        }
    }
}
