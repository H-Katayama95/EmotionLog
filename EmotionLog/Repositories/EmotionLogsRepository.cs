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
                    MorningDetail = null,
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
                    MorningDetail = reader.IsDBNull(5) ? null : reader.GetString(5),
                    NoonDetail = reader.IsDBNull(6) ? null : reader.GetString(6),
                    EveningDetail = reader.IsDBNull(7) ? null : reader.GetString(7),
                    ConsecutiveRecord = reader.GetInt32(8),
                    CreatedAt = reader.GetDateTime(9),
                    UpdatedAt = reader.GetDateTime(10)
                };
            }

            return emotionLogs;
        }

        // 感情ログを挿入もしくは更新
        public void UpsertEmotionLogs(EmotionLogs emotionLogs)
        {
           // EmotionLogs emotionLogs = new EmotionLogs();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            command.Parameters.AddWithValue("recordDate", DateTime.Now);
            command.Parameters.AddWithValue("morningEmotionType", emotionLogs.MorningEmotionType);
            command.Parameters.AddWithValue("noonEmotionType", emotionLogs.NoonEmotionType);
            command.Parameters.AddWithValue("eveningEmotionType", emotionLogs.EveningEmotionType);
            command.Parameters.AddWithValue("morningDetail", emotionLogs.MorningDetail);
            command.Parameters.AddWithValue("noonDetail", emotionLogs.NoonDetail);
            command.Parameters.AddWithValue("eveningDetail", emotionLogs.EveningDetail);
            command.Parameters.AddWithValue("consecutiveRecord", emotionLogs.ConsecutiveRecord);

            command.CommandText = @"
                INSERT INTO emotion_logs (
                    record_date,
                    morning_emotion_type,
	                noon_emotion_type,
	                evening_emotion_type,
                    morning_detail,
	                noon_detail,
	                evening_detail,
                    consecutive_record,
                    created_at,
                    updated_at
                ) VALUES (
                    @recordDate,
                    @morningEmotionType,
	                @noonEmotionType,
	                @eveningEmotionType,
                    @morningDetail,
	                @noonDetail,
	                @eveningDetail,
                    @consecutiveRecord,
                    NOW(),
                    NOW()
                ) ON CONFLICT (record_date) DO UPDATE SET
                    morning_emotion_type = EXCLUDED.morning_emotion_type,
                    noon_emotion_type = EXCLUDED.noon_emotion_type,
                    evening_emotion_type = EXCLUDED.evening_emotion_type,
                    morning_detail = EXCLUDED.morning_detail,
                    noon_detail = EXCLUDED.noon_detail,
                    evening_detail = EXCLUDED.evening_detail,
                    updated_at = NOW();
                ";

            command.ExecuteNonQuery();
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

