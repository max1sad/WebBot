using System;
using System.Collections.Generic;
using System.Text;

namespace FootballTelegramBot.Model
{
   public class Match
    {
        public int idTeams1 { get; set; }
        public int idTeams2 { get; set; }
        public DateTime? dateGames { get; set; }
        public int? countTeams1 { get; set; }
        public int? countTeams2 { get; set; }
        public string? statusGames { get; set; }
        public string? chekUpdate { get; set; }
        public string? chekAlert { get; set; }
    }
}
