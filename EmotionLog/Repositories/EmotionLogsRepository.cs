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
    public class EmotionLogsRepository : BaseRepository
    {
        public List<EmotionLogs> GetEmotionLogs()
        {
            var list = new List<EmotionLogs>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "SELECT * FROM emotion_logs;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new EmotionLogs
                {
                    EmotionLogId = reader.GetInt32(0),
                    EmotionTypeId = reader.GetInt32(1),
                    RecordDate = reader.GetDateTime(2),
                    TimeZone = reader.GetInt32(3), // 0: Morning, 1: Noon, 2: Evening
                    Detail = reader.IsDBNull(4) ? null : reader.GetString(4),
                    ConsecutiveRecord = reader.GetInt32(5),
                    CreatedAt = reader.GetDateTime(6),
                    UpdatedAt = reader.GetDateTime(7)
                });
            }
            return list;
        }
    }
}

