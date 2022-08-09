using Microsoft.AspNetCore.Mvc;
using FootballTelegramBot.Services;
using Telegram.Bot.Types;

namespace FootballTelegramBot.Controllers;
public class WebhookController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService,
                                          [FromBody] Update update)
    {
        await handleUpdateService.EchoAsync(update);
        return Ok();
    }
}