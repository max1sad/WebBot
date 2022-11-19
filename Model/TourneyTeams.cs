using System;
using System.Collections.Generic;
using System.Text;

namespace FootballTelegramBot.Model
{
   public class TourneyTeams
    {
        public int idTourney { get; set; }
        public int idLeague { get; set; }
        public int idNameTourney { get; set; }
        public int idTeams { get; set; }
        public string? statusRequest { get; set; }
        
    }
}
