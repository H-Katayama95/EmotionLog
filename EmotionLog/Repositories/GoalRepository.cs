using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using EmotionLog.Models;
using Npgsql;

namespace EmotionLog.Repositories
{
    public class GoalRepository : BaseRepository
    {
        public List<Goal> GetGoal()
        {
            var list = new List<Goal>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "SELECT * FROM goal_master;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Goal
                {
                    GoalId = reader.GetInt32(0),
                    GoalContent = reader.GetString(1),
                    GoalLevel = reader.GetInt32(2),
                    GoalCount = reader.GetInt32(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5)
                });
            }
            return list;
        }
    }
}

