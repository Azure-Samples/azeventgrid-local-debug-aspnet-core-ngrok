using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                    var eventGridEvents = JsonSerializer.Deserialize<List<EventGridEvent>>(jsonContent);

                    foreach (var eventGridEvent in eventGridEvents)
                    {
                        // EventGrid validation message
                        if (eventGridEvent.EventType == "Microsoft.EventGrid.SubscriptionValidationEvent")
                        {
                            var eventData = eventGridEvent.GetData<SubscriptionValidationEventData>();

                            return Ok(eventData.ValidationCode);
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
            if (eventGridEvent.EventType == "Microsoft.Storage.BlobCreated")
            {
                // do something
            }

            // delay return by 3 seconds
            await Task.Delay(3000);

            return Ok();
        }
    }
}