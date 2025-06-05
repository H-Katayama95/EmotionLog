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
    public class RoutineCheckRepository : BaseRepository
    {
        public List<RoutineCheck> GetRoutineCheck()
        {
            var list = new List<RoutineCheck>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "SELECT * FROM routine_check;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new RoutineCheck
                {
                    RoutineCheckId = reader.GetInt32(0),
                    GoalId = reader.GetInt32(1),
                    RecordDate = reader.GetDateTime(2),
                    IsChecked = reader.GetBoolean(3),
                    Total = reader.GetInt32(4),
                    CreatedAt = reader.GetDateTime(5),
                    UpdatedAt = reader.GetDateTime(6)
                });
            }
            return list;
        }
    }
}

