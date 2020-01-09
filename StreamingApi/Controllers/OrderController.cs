using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using Newtonsoft.Json;
using StreamingService.Models;
using StreamingService.Service;

namespace StreamingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ProducerConfig config;
        private IProducerService _producerService;

        public OrderController(ProducerConfig config, IProducerService producerService)
        {
            this.config = config;
            this._producerService = producerService;
        }

        [HttpGet]
        public ActionResult GetAsync()
        {
            return Ok(string.Format("Assembly: {0}", GetType().Assembly.GetName()));
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody]OrderRequest value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Serialize 
            string serializedOrder = JsonConvert.SerializeObject(value);

            var res = await _producerService.SendMessage("orderBookRequests", serializedOrder, true, 1);
           
            return Created("TransactionId", "Your order is in progress");
        }

    }
}