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
    public class GoalProgressRepository : BaseRepository
    {
        public List<GoalProgress> GetGoalProgress()
        {
            var list = new List<GoalProgress>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            command.CommandText = @"
                SELECT * FROM goal_progress
                ORDER BY ABS(record_date - CURRENT_DATE) ASC
                LIMIT 1;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new GoalProgress
                {
                    GoalProgressId = reader.GetInt32(0),
                    GoalId = reader.GetInt32(1),
                    GoalSetDate = reader.GetDateTime(2),
                    GoalAchievedDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                    Total = reader.GetInt32(4),
                    GoalStatus = reader.GetBoolean(5),
                    CreatedAt = reader.GetDateTime(6),
                    UpdatedAt = reader.GetDateTime(7),
                    RecordDate = reader.GetDateTime(8)
                });
            }
            return list;
        }
    }
}

