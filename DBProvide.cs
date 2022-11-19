using BotWebHooksProdject.Model;
using Dapper;
using FootballTelegramBot.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using Telegram.Bot.Types;

namespace FootballTelegramBot
{
   public class DBProvide
    {
        const string ConnectionString = "Data Source=DESKTOP-2MK3795;Initial Catalog=dbfootbal_new;Integrated Security=True";
        public string nameTourneyPost = "";
        public int idTourneyChek;
        //Model.Player player = new Model.Player();
        // Model.Team team = new Model.Team();
        // Model.League league = new Model.League();
        // Model.PlayerTeams playerTeams = new Model.PlayerTeams();
        // Model.NameTourney nameTourney = new Model.NameTourney();
        /// <summary>
        /// получение номера заявок на турнир, которые отправлены на рассмотрение, вызывать в html странице 
        /// </summary>
        /// <returns></returns>
        public List<TourneyTeams> GetNotTourneyTeams()
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<TourneyTeams>("select idTourney from tourneyTeams where statusRequest = 'Waiting for confirmation'").ToList();
            }
        }
        /// <summary>
        /// Добавление название футбольной лиги
        /// </summary>
        /// <param name="league"></param>
        public void CreateLeagues(League league)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "insert into league (nameLeague) values (@nameLeague);";
                db.Execute(sqlQuery, league);
            }
        }
        public List<TeamName> GetNameTeams(string nameChek)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<TeamName>("select Team.idTeams,nameTeams from nameTourney inner join tourneyTeams on nameTourney.idNameTourney = tourneyTeams.idNameTourney\n" +
                    "inner join Team on tourneyTeams.idTeams = Team.idTeams\n" +
                    "where nameTourney.nameTourney like '"+nameChek+"'").ToList();
            }
        }

         public List<NameTeamsTourneyLeague> GetZayvkaName(int idZayvka)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<NameTeamsTourneyLeague>("select nameTeams, nameTourney.nameTourney, nameLeague  from tourneyTeams inner join Team on tourneyTeams.idTeams = Team.idTeams\n"+
"inner join nameTourney on tourneyTeams.idNameTourney = nameTourney.idNameTourney\n"+
"inner join league on tourneyTeams.idLeague = league.idLeague\n"+
"where idTourney = " + idZayvka).ToList();
            }
        }
        public List<NameTeamsTourneyLeague> GetZayvkaNameFIO(int idZayvka)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<NameTeamsTourneyLeague>("select FIO  from tourneyTeams inner join Team on tourneyTeams.idTeams = Team.idTeams\n" +
"inner join playerTeams on Team.idTeams = playerTeams.idTeams inner join player\n" +
"on playerTeams.idPlayer = player.idPlayer\n" +
"where idTourney = " + idZayvka).ToList();
            }
        }
        
            public void CreateStatusRequest(int idZayvka)
            {
                using (IDbConnection db = new SqlConnection(ConnectionString))
                {
                    var sqlQuery = "update tourneyTeams set statusRequest = 'application accepted' where idTourney = "+ idZayvka;
                    db.Execute(sqlQuery, idZayvka);
                }
            }
        public List<Player> GetPlayerCapitan(int idZayvka)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<Player>("select idPhone from player\n" +
                    "where idPlayer = (select idPlayer from tourneyTeams inner join Team\n" +
                    "on tourneyTeams.idTeams = Team.idTeams inner join playerTeams on Team.idTeams = playerTeams.idTeams\n" +
                    "where idTourney = "+ idZayvka +"and teamCaptain is not null) ").ToList();
            }
        }
        public List<Team> GetTeamName(int idZayvka)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<Team>("select nameTeams from tourneyTeams inner join Team on tourneyTeams.idTeams = Team.idTeams\n" +
                    "where idTourney = "+idZayvka).ToList();
            }
        }
        public List<Player> GetPlayerDelete(int idZayvka)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<Player>("select idPlayer from tourneyTeams inner join Team\n" +
                    "on tourneyTeams.idTeams = Team.idTeams inner join playerTeams\n" +
                    "on Team.idTeams = playerTeams.idTeams where idTourney = "+ idZayvka).ToList();
            }
        }
        public List<Team> GetTeamsDelete(int idZayvka)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<Team>("select Team.idTeams from tourneyTeams inner join Team\n" +
                    "on tourneyTeams.idTeams = Team.idTeams where idTourney = "+idZayvka).ToList();
            }
        }
        public void DeletePlayer(int player)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "delete from playerTeams where idPlayer in (select idPlayer from player where idPlayer =" +player+");\n" +
                    "delete from player where idPlayer in (select idPlayer from player where idPlayer = "+player+");";
                db.Execute(sqlQuery, player);
            }
               
        }
        public void DeleteTeams(int teams)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "delete from playerTeams where idTeams = " + teams + ";\n" +
                    //"delete from leagueTable where idTourney in (select idTourney from tourneyTeams where idTeams = "+ teams + ");\n" +
                    "delete from tourneyTeams where idTeams = " + teams + ";\n" +
                    "delete from Team where idTeams = " + teams + ";";
                    db.Execute(sqlQuery, teams);
            }

        }

        public void CreateTourney(NameTourney tourney)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "insert into nameTourney(nameTourney, startDate, endDate) values(@nameTourney, @startDate, @endDate)";
                //var sqlQuery = "insert into Match (idTeams1,idTeams2,dateGames) values (@idTeams1,@idTeams2,convert(datetime,@dateGame,5))";
                db.Execute(sqlQuery, tourney);
            }
        }
        public List<League> GetAllLeague()
        {           
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<League>("SELECT * FROM league").ToList();

            }                           
        }
        public void CreateTeams(Team team)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "insert into Team (nameTeams) values (@nameTeams);";
                db.Execute(sqlQuery, team);
            }
        }
        public void CreateTourneyTeams(TourneyTeams tourneyTeams)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "insert into tourneyTeams (idLeague,idNameTourney,idTeams,statusRequest) values (@idLeague,@idNameTourney,@idTeams,@statusRequest);";
                db.Execute(sqlQuery, tourneyTeams);
            }
        }
        public void CreatePlayerCapitan(Player player)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "insert into player (FIO,idPhone) values (@FIO,@idPhone)";
                db.Execute(sqlQuery, player);
            }
        }
        public void CreatePlayer(Player player)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "insert into player (FIO) values (@FIO)";
                db.Execute(sqlQuery, player);
            }
        }
        public void CreatePlayerTeamsCapitan(PlayerTeams playerTeams)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "insert into playerTeams (idTeams,teamCaptain,idPlayer) values (@idTeams,@teamCaptain,@idPlayer)";
                db.Execute(sqlQuery, playerTeams);
            }
        }
        public void CreatePlayerTeams(PlayerTeams playerTeams)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "insert into playerTeams (idTeams,idPlayer) values (@idTeams,@idPlayer)";
                db.Execute(sqlQuery, playerTeams);
            }
        }
        public List<Team> GetAllTeams()
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<Team>("SELECT * FROM team").ToList();

            }
        }
        public List<Player> GetAllPlayer()
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<Player>("SELECT * FROM player").ToList();
            }
        }
        public List<NameTourney> GetAllTourney()
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<NameTourney>("SELECT * FROM nameTourney where statusToutney is null").ToList();
            }
        }
        public List<Match> GetAllMatch()
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<Match>("SELECT * FROM Match").ToList();
            }
        }
        public List<TourneyTeams> GetTourneyTeams()
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<TourneyTeams>("SELECT * FROM tourneyTeams").ToList();
            }
        }

        public void CreateMatchTimeGame(Match match)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var sqlQuery = "insert into Match (idTeams1,idTeams2,dateGames) values (@idTeams1,@idTeams2,convert(datetime,@dateGames,5))";
                db.Execute(sqlQuery, match);
            }
        }

    }
}
