using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        // 過去の感情ログを取得
        public List<PastEmotionLogs> GetPastEmotionLogs()
        {
            List<PastEmotionLogs> pastEmotionLogsList = new List<PastEmotionLogs>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            command.CommandText = @"
            SELECT
                logs.record_date,
                logs.morning_detail,
                mas1.emotion_name AS moring_emoname,
                logs.noon_detail,
                mas2.emotion_name AS noon_emoname,
                logs.evening_detail,
                mas3.emotion_name AS evening_emoname
            FROM emotion_logs logs
                LEFT JOIN emotion_master AS mas1 ON mas1.emotion_type_id = logs.morning_emotion_type
                LEFT JOIN emotion_master AS mas2 ON mas2.emotion_type_id = logs.noon_emotion_type
                LEFT JOIN emotion_master AS mas3 ON mas3.emotion_type_id = logs.evening_emotion_type
             ORDER BY logs.record_date ASC ;";
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                PastEmotionLogs pastEmotionLogs = new PastEmotionLogs
                {
                    RecordDate = reader.GetDateTime(0),
                    MorningDetail = reader.IsDBNull(1) ? null : reader.GetString(1),
                    MorningEmoName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    NoonDetail = reader.IsDBNull(3) ? null : reader.GetString(3),
                    NoonEmoName = reader.IsDBNull(4) ? null : reader.GetString(4),
                    EveningDetail = reader.IsDBNull(5) ? null : reader.GetString(5),
                    EveningEmoName = reader.IsDBNull(6) ? null : reader.GetString(6)
                };
                pastEmotionLogsList.Add(pastEmotionLogs);   
            }

            return pastEmotionLogsList;
        }

        // 感情ログを挿入もしくは更新
        public void UpsertEmotionLogs(EmotionLogs emotionLogs)
        {
            try
            {
                using var connection = GetConnection();
                using var command = connection.CreateCommand();
                connection.Open();

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

                command.Parameters.AddWithValue("recordDate", DateTime.Now);
                command.Parameters.AddWithValue("morningEmotionType", emotionLogs.MorningEmotionType);
                command.Parameters.AddWithValue("noonEmotionType", emotionLogs.NoonEmotionType);
                command.Parameters.AddWithValue("eveningEmotionType", emotionLogs.EveningEmotionType);
                command.Parameters.AddWithValue("morningDetail", emotionLogs.MorningDetail);
                command.Parameters.AddWithValue("noonDetail", emotionLogs.NoonDetail);
                command.Parameters.AddWithValue("eveningDetail", emotionLogs.EveningDetail);
                command.Parameters.AddWithValue("consecutiveRecord", emotionLogs.ConsecutiveRecord);

                command.ExecuteNonQuery();

                MessageBox.Show("感情ログを保存しました。", "保存完了", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"感情ログの保存に失敗しました: {ex.Message}", "保存失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        // 感情ログの入力チェック
        public bool ValidateEmotionLogs(EmotionLogs emotionLogs)
        {
            if (emotionLogs.MorningDetail != null && emotionLogs.MorningDetail.Length > 140 ||
                emotionLogs.NoonDetail != null && emotionLogs.NoonDetail.Length > 140 ||
                emotionLogs.EveningDetail != null && emotionLogs.EveningDetail.Length > 140)
            {
                return false;
            }
            else
            {
                return true;
            }
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
            // 1日前のデータがない場合は0を返す
            if (reader.Read())
            {
                consecutiveRecord = reader.GetInt32(0);
            }
            return consecutiveRecord;
        }
        
        public ConsecutiveRecord GetConsecutiveRecords()
        {
            ConsecutiveRecord recordStreak = new ConsecutiveRecord();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            command.CommandText = @"
            WITH
            yesterday_data AS(
            SELECT consecutive_record
            FROM public.emotion_logs
            WHERE record_date = CURRENT_DATE - INTERVAL '1 day'
            LIMIT 1),
            today_data AS(
            SELECT consecutive_record
            FROM public.emotion_logs
            WHERE record_date = CURRENT_DATE
            LIMIT 1)
            SELECT 
              COALESCE((SELECT consecutive_record FROM yesterday_data), 0) AS Yesterday_consecutive_record,
              COALESCE((SELECT consecutive_record FROM today_data), 0) AS Today_consecutive_record;";
            
            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return recordStreak = new ConsecutiveRecord
                {
                    Yesterday_Consecutive_Record = 0,
                    Today_Consecutive_Record = 0
                };
            }
            else
            {
                return recordStreak = new ConsecutiveRecord
                {
                    Yesterday_Consecutive_Record = reader.GetInt32(0),
                    Today_Consecutive_Record = reader.GetInt32(1)
                };
            }
        }
    }

}

