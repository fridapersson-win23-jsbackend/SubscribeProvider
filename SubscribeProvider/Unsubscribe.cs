using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SubscribeProvider
{
    public class Unsubscribe(ILogger<Unsubscribe> logger, DataContext context)
    {
        private readonly ILogger<Unsubscribe> _logger = logger;
        private readonly DataContext _context = context;

        [Function("Unsubscribe")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                if(!string.IsNullOrEmpty(body))
                {
                    var subscriberEntity = JsonConvert.DeserializeObject<SubscribeEntity>(body);
                    if(subscriberEntity != null)
                    {
                        var existingSubscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Email == subscriberEntity.Email);
                        if(existingSubscriber != null)
                        {
                            _context.Subscribers.Remove(existingSubscriber);
                            await _context.SaveChangesAsync();
                            return new OkObjectResult(new { Status = 200, Message="You are now Unsubscribed" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : Unsubscribe.Run() :: {ex.Message} ");
            }
            return new BadRequestObjectResult(new { Status = 400, Message = "Unable to Unsubscribe" });
        }
    }
}
