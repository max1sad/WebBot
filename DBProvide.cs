using Dapper;
using FootballTelegramBot.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FootballTelegramBot
{
   public class DBProvide
    {
        const string ConnectionString = "Data Source=DESKTOP-2MK3795;Initial Catalog=dbfootbal;Integrated Security=True";
        //Model.Player player = new Model.Player();
       // Model.Team team = new Model.Team();
       // Model.League league = new Model.League();
       // Model.PlayerTeams playerTeams = new Model.PlayerTeams();
       // Model.NameTourney nameTourney = new Model.NameTourney();

       
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
                var sqlQuery = "insert into tourneyTeams (idLeague,idNameTourney,idTeams) values (@idLeague,@idNameTourney,@idTeams);";
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
                var sqlQuery = "insert into Match (idTeams1,idTeams2,dateGames) values (@idTeams1,@idTeams2,convert(datetime,@dateGame,5))";
                db.Execute(sqlQuery, match);
            }
        }
    }
}
