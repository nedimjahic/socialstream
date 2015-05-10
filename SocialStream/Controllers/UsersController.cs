using SocialStream.Models;
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
            socialstream_developmentEntities db = new socialstream_developmentEntities();

            try
            {                
                User user = db.User.Where(usr => usr.email == email && usr.password == password).FirstOrDefault();
                return user;
            }
            catch (Exception)
            {
                return null;
            }

            
        }
        [HttpPost]
        public User create(string email, string password)
        {
            socialstream_developmentEntities db = new socialstream_developmentEntities();

            User user = new User();
            user.email = email;
            user.password = password;

            db.User.Add(user);
            db.SaveChanges();

            return user;
        }

        [HttpPost]
        public void setactivetopic(string topic)
        {
            //TODO implement setactivetopic which sets active topic
        }
    }
}
