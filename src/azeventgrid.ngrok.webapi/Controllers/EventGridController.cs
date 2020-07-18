using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace azeventgrid.ngrok.webapi.Controllers
{
    [Route("api/eventgrid")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]    
    public class EventGridController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            IActionResult result = Ok();

            try
            {
                // using StreamReader due to changes in .Net Core 3 serializer ie ValueKind
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    var jsonContent = await reader.ReadToEndAsync();
                    var eventGridEvents = JsonConvert.DeserializeObject<List<EventGridEvent>>(jsonContent);

                    foreach (var eventGridEvent in eventGridEvents)
                    {
                        // EventGrid validation message
                        if (eventGridEvent.EventType == EventTypes.EventGridSubscriptionValidationEvent)
                        {
                            var eventData = ((JObject)(eventGridEvent.Data)).ToObject<SubscriptionValidationEventData>();
                            var responseData = new SubscriptionValidationResponse()
                            {
                                ValidationResponse = eventData.ValidationCode
                            };
                            return Ok(responseData);
                        }
                        // handle all other events
                        await this.HandleEvent(eventGridEvent);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
        private async Task<IActionResult> HandleEvent(EventGridEvent eventGridEvent)
        {
            if (eventGridEvent.EventType == EventTypes.StorageBlobCreatedEvent)
            {
                // do something
            }

            // delay return by 3 seconds
            await Task.Delay(3000);

            return Ok();
        }
    }
}