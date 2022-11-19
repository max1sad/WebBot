//using BotWebHooksProdject.Services;
using BotWebHooksProdject.Services;
using FootballTelegramBot;
using FootballTelegramBot.Model;
using FootballTelegramBot.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace BotWebHooksProdject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminFormsController : Controller
    {
        //имя запроса для метода который мы вызывает в vue для отображения данных.

        [HttpGet("GetPlayer")]
        public async Task<IActionResult> Get([FromServices] DBProvide dBProvide)
        {
            var result =  dBProvide.GetAllPlayer();
            return Ok(result);
        }
        [HttpGet("GetTeams")]
        public async Task<IActionResult> GetTeams([FromServices] DBProvide dBProvide)
        {
            var result = dBProvide.GetAllTeams();
            return Ok(result);
        }
        [HttpPost("CreateTourney")]
        public async Task<IActionResult> PostHtmlTournye([FromBody] NameTourney dat, [FromServices] DBProvide dBProvide)
        {
           // var dat2 = dat as NameTourney;
            dBProvide.CreateTourney(dat);
            return Ok();
        }
        [HttpPost("CreateLeague")]
        public async Task<IActionResult> PostLuague([FromBody] League dat, [FromServices] DBProvide dBProvide)
        {
            dBProvide.CreateLeagues(dat);
            return Ok();
        }
        // принмаем данные от vue формы , данные уже в виде орбьекта поступают..
        [HttpPost("PostPlayer")]
        public async Task<IActionResult> Post([FromBody] Player dat,[FromServices] DBProvide dBProvide)
        {
            dBProvide.CreatePlayerCapitan(dat);
            return Ok();
        }
        /// <summary>
        /// Получает список лиг для админ панели
        /// </summary>
        /// <param name="dBProvide"></param>
        /// <returns></returns>
        [HttpGet("GetLeague")]
        public async Task<IActionResult> GetHtmlLeague([FromServices] DBProvide dBProvide)
        {
            var result = dBProvide.GetAllLeague();
            return Ok(result);
        }
        
        [HttpGet("GetTourney")]
        public async Task<IActionResult> GetHtmlTourney([FromServices] DBProvide dBProvide)
        {
            var result = dBProvide.GetAllTourney();
            return Ok(result);
        }
        [HttpPost("PostTourneyName")]
        public async Task<IActionResult> PostHtmlTourneyName([FromBody] NameTourney dat,[FromServices] DBProvide dBProvide)
        {
           
          dBProvide.nameTourneyPost = dat.nameTourney;
            return Ok();
        }
        [HttpGet("GetNameTeamsTourney")]
        public async Task<IActionResult> GetHtmlNameTeamsTourney([FromServices] DBProvide dBProvide)
        {
            var result = dBProvide.GetNameTeams(dBProvide.nameTourneyPost);
            return Ok(result);
        }
        [HttpGet("GetMatch")]
        public async Task<IActionResult> GetHtmlMatch([FromServices] DBProvide dBProvide)
        {
            var result = dBProvide.GetAllMatch();
            return Ok(result);
        }
        [HttpPost("PostMatchAdd")]
        public async Task<IActionResult> PostHtmlMatchAdd([FromBody] Match dat, [FromServices] DBProvide dBProvide)
        {

            dBProvide.CreateMatchTimeGame(dat);
            return Ok();
        }
        [HttpGet("GetNotTourney")]
        public async Task<IActionResult> GetHtmlNotTourney([FromServices] DBProvide dBProvide)
        {
            var result = dBProvide.GetNotTourneyTeams();
            return Ok(result);
        }
        [HttpPost("PostNotTourney")]
        public async Task<IActionResult> PostHtmlNotTourney([FromBody] TourneyTeams dat, [FromServices] DBProvide dBProvide)
        {

            dBProvide.idTourneyChek = dat.idTourney;
            return Ok();
        }
        [HttpGet("GetNameTeamsTourneyLeague")]
        public async Task<IActionResult> GetHtmlNameTeamsTourneyLeague([FromServices] DBProvide dBProvide)
        {
            var result = dBProvide.GetZayvkaName(dBProvide.idTourneyChek);
            return Ok(result);
        }
        [HttpGet("GetZayvkaFIO")]
        public async Task<IActionResult> GetHtmlZayvkaFIO([FromServices] DBProvide dBProvide)
        {
            var result = dBProvide.GetZayvkaNameFIO(dBProvide.idTourneyChek);
            
            return Ok(result);
        }
        [HttpPost("UpdateStatusRequest")]
            public async Task<IActionResult> PostUpdateStatusRequest( [FromServices] DBProvide dBProvide)
        {

            dBProvide.CreateStatusRequest(dBProvide.idTourneyChek);
            return Ok();
        }
       
        [HttpDelete("DeleteZayvka")]
        public async Task<IActionResult> PostDeleteZayvka([FromServices] DBProvide dBProvide)
        {                     
            //var bot = new TelegramBotClient("5444116745:AAEiHTg9bpEQLHA7sEGj_c7SpCTg6AXMVPY");
            foreach ( var rez in dBProvide.GetPlayerCapitan(dBProvide.idTourneyChek).Select(l => l.idPhone))
            {
                foreach( var nam in dBProvide.GetTeamName(dBProvide.idTourneyChek).Select(l => l.nameTeams))
                {
                    BotSendingNotifications botSending = new BotSendingNotifications();
                    await botSending.CloseZayvkaAsync(rez, nam);
                }
                
                //await bot.SendTextMessageAsync(rez, "Ваша заявка на участие в турнире отклонена ");                  
            }                                   
            foreach (var pl in dBProvide.GetPlayerDelete(dBProvide.idTourneyChek).Select(l => l.idPlayer))
            {
                dBProvide.DeletePlayer(pl);
            }
            foreach (var tm in dBProvide.GetTeamsDelete(dBProvide.idTourneyChek).Select(l => l.idTeams))
            {
                dBProvide.DeleteTeams(tm);
            }
            return Ok();
        }
    }
}
