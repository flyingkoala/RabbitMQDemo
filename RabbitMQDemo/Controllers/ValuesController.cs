using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly MQApp _mqApp;
        public ValuesController(MQApp mqApp)
        {
            _mqApp = mqApp;
        }

        // GET api/values

        [HttpGet]
        public ActionResult<string> Get()
        {
            string message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _mqApp.PublishMessage(message);
            return "ok";
        }

    }
}
