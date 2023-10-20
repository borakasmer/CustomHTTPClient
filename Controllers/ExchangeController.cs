using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace CustomHTTPClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeController : ControllerBase
    {
        CustomWebClient client;
        public ExchangeController(CustomWebClient _client)
        {         
            client = _client;
            //client.Timeout = 1;
        }

        [HttpPost("GetExchange")]
        public IActionResult Post([FromBody] Data data)
        {
            //using (var client = new CustomWebClient(1))
            //{
                int idParameter = 555;
                client.BaseAddress = "https://localhost:7053";
                var url= "Exchange/dummyData";
                
            //Config'den geliyor...
            //client.Tries = 5; //Number of tries that i want to get some page

                //string postUrl = "https://localhost:7053/Exchange/dummyData";
                //Data data = new Data() { Id = 555, Name = "Kasmer" };
                string postData = JsonConvert.SerializeObject(data);
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                //client.QueryString.Add("id", idParameter.ToString());   
                string response = client.UploadString(url, postData);
                var result = JsonConvert.DeserializeObject<Data>(response);
                return Ok(result);
                //string result = client.UploadString(postUrl, "POST");
            //}
           
        }

        [HttpPost]
        [Route("dummyData")]
        public JsonResult DummyData([FromBody]Data data)
        {
            Thread.Sleep(2000);
            var jsonData = new
            {
                name = data.Name,
                surname = "kasmer",
                Id = data.Id,
                isMale = true
            };
            return new JsonResult(jsonData);
        }
    }

    public class Data
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
