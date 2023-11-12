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
        }

        [HttpPost("GetExchange")]
        public IActionResult Post([FromBody] Data data)
        {           
                client.BaseAddress = "https://localhost:7053";
                var url= "Exchange/dummyData";
                
                string postData = JsonConvert.SerializeObject(data);                
                string response = client.UploadString(url, postData);            
                var result = JsonConvert.DeserializeObject<Data>(response);
                return Ok(result);                              
        }

        [HttpPost]
        [Route("dummyData")]
        public JsonResult DummyData([FromBody]Data data)
        {
            Thread.Sleep(3000);
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
