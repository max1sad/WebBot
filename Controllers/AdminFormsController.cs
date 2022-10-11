using FootballTelegramBot;
using FootballTelegramBot.Model;
using FootballTelegramBot.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace BotWebHooksProdject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminFormsController : ControllerBase
    {
        //имя запроса для метода который мы вызывает в vue для отображения данных.

        [HttpGet("GetPlayer")]
        public async Task<IActionResult> Get([FromServices] DBProvide dBProvide)
        {
            var result =  dBProvide.GetAllPlayer();
            return Ok(result);
        }
        // принмаем данные от vue формы , данные уже в виде орбьекта поступают..
        [HttpPost("PostPlayer")]
        public async Task<IActionResult> Post([FromBody] Player dat,[FromServices] DBProvide dBProvide)
        {
            dBProvide.CreatePlayerCapitan(dat);
            return Ok();
        }
    }
}
