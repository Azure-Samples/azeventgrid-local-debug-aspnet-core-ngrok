using System;
using System.IO;
using System.Text;
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
                    var eventGridEvents = EventGridEvent.ParseMany(BinaryData.FromString(jsonContent));

                    foreach (var egEvent in eventGridEvents)
                    {
                        if (egEvent.TryGetSystemEventData(out object data))
                        {
                            // EventGrid validation message
                            if (data is SubscriptionValidationEventData subscriptionData)
                            {
                                var responseData = new SubscriptionValidationResponse()
                                {
                                    ValidationResponse = subscriptionData.ValidationCode
                                };
                                return Ok(responseData);
                            }
                        }
                        // handle all other events
                        await this.HandleEvent(egEvent);
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
        private async Task<IActionResult> HandleEvent(EventGridEvent eventGridEvent)
        {
            if (eventGridEvent.EventType == SystemEventNames.StorageBlobCreated)
            {
                // do something
            }

            // delay return by 3 seconds
            await Task.Delay(3000);

            return Ok();
        }
    }
}