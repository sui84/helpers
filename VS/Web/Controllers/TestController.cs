using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mvc4Web.Controllers
{
    public class TestController : ApiController
    {
        // GET api/test/5
        public object Get(int? id)
        {
            WebRequestHelper wr = new WebRequestHelper();
            string url = "http://...";
            object obj = wr.GetPostResponse(url);
            return obj;
        }

        // POST api/test
        public void Post([FromBody]string value)
        {
        }

        // PUT api/test/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/test/5
        public void Delete(int id)
        {
        }
    }
}
