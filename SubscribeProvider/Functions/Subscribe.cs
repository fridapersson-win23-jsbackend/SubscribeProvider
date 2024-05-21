using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SubscribeProvider.Functions;

public class Subscribe
{
    private readonly ILogger<Subscribe> _logger;
    private readonly DataContext _context;

    public Subscribe(ILogger<Subscribe> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Function("Subscribe")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if (!string.IsNullOrEmpty(body))
            {
                var subscribeEntity = JsonConvert.DeserializeObject<SubscribeEntity>(body);
                if (subscribeEntity != null)
                {
                    var existingSubscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Email == subscribeEntity.Email);
                    if (existingSubscriber != null)
                    {
                        _context.Entry(existingSubscriber).CurrentValues.SetValues(subscribeEntity);
                        await _context.SaveChangesAsync();
                        return new OkObjectResult(new { Status = 200, Message = "Subscriber was updated" });
                    }
                    else
                    {
                        _context.Subscribers.Add(subscribeEntity);
                        await _context.SaveChangesAsync();
                        return new OkObjectResult(new { Status = 200, Message = "Subscriber is now subscribed" });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : Subscribe.Run() :: {ex.Message} ");
        }
        return new BadRequestObjectResult(new { Status = 400, Message = "Unable to subscribe" });
    }
}
