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
        // システム起動日の感情ログを取得
        public EmotionLogs GetEmotionLogs()
        {
            EmotionLogs emotionLogs = new EmotionLogs();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "SELECT * FROM emotion_logs WHERE record_date = CURRENT_DATE;";
            using var reader = command.ExecuteReader();
            // 今日のデータがない場合、空のデータを追加
            if (!reader.Read())
            {
                emotionLogs = new EmotionLogs
                {
                    RecordDate = DateTime.Now,
                    MorningEmotionType = null,
                    NoonEmotionType = null,
                    EveningEmotionType = null,
                    MoringDetail = null,
                    NoonDetail = null,
                    EveningDetail = null,
                    ConsecutiveRecord = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
            }
            else
            {
                emotionLogs = new EmotionLogs
                {
                    EmotionLogId = reader.GetInt32(0),
                    RecordDate = reader.GetDateTime(1),
                    MorningEmotionType = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                    NoonEmotionType = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    EveningEmotionType = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    MoringDetail = reader.IsDBNull(5) ? null : reader.GetString(5),
                    NoonDetail = reader.IsDBNull(6) ? null : reader.GetString(6),
                    EveningDetail = reader.IsDBNull(7) ? null : reader.GetString(7),
                    ConsecutiveRecord = reader.GetInt32(8),
                    CreatedAt = reader.GetDateTime(9),
                    UpdatedAt = reader.GetDateTime(10)
                };
            }

            return emotionLogs;
        }

        // システム起動日の1日前の連続記録日数を取得
        public int GetConsecutiveRecord()
        {
            int consecutiveRecord = 0;

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "SELECT consecutive_record FROM emotion_logs WHERE record_date = CURRENT_DATE - INTERVAL '1 day';";
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                consecutiveRecord = reader.GetInt32(0);
            }
            return consecutiveRecord;
        }

    }
}

