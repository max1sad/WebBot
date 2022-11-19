using Microsoft.AspNetCore.Mvc;
using FootballTelegramBot.Services;
using Telegram.Bot.Types;

namespace FootballTelegramBot.Controllers
{
    public class WebhookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService,
                                              [FromBody] Update update)
        {
            await handleUpdateService.EchoAsync(update);
            
            return Ok();
        }
        /*[HttpPost]
        public async Task<IActionResult> Post_1([FromServices] HandleUpdateService handleUpdateService,
                                            [FromBody] String idPlayer)
        {
            await handleUpdateService.resultZayvka(idPlayer);
            return Ok();
        }*/
        /* public  Task<HttpContext> ContextAsync(HttpContext context)
         {


             context.Response.ContentType = "text/html; charset=utf-8";

             // если обращение идет по адресу "/postuser", получаем данные формы
             if (context.Request.Path == "/postuser")
             {
                 var form = context.Request.Form;
                 string name = form["name"];
                 string age = form["age"];
                  context.Response.WriteAsync($"<div><p>Name: {name}</p><p>Age: {age}</p></div>");
             }
             else if (context.Request.Path == "/test.html")
             {
                  context.Response.SendFileAsync("html/test.html");
             }
             else
             {
                  context.Response.SendFileAsync("html/Index.html");
             }
             return context;
         }*/
    }
}