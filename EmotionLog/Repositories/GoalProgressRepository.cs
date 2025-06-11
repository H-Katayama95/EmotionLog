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
    public class GoalProgressRepository : BaseRepository
    {
        // 目標進捗を取得
        public List<GoalProgress> GetGoalProgress()
        {
            var list = new List<GoalProgress>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            // 目標進捗を最新１００件取得する
            // メモ：データ増えたら処理重くなるから改善する必要あり
            command.CommandText = @"
                SELECT * FROM goal_progress
                WHERE goal_status = false
                ORDER BY record_date DESC LIMIT 100;";

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

        // 進行中の目標を取得
        public List<GoalProgress> GetRecentGoalProgress()
        {
            var list = new List<GoalProgress>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            // 目標進捗を取得し、現在の日付に近い順に並べる
            command.CommandText = @"
                SELECT * FROM goal_progress
                WHERE goal_status = false
                ORDER BY ABS(record_date - CURRENT_DATE) ASC LIMIT 1;";

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


        // 登録済みの目標を取得
        public int GetGoalId()
        {
            int goalId = 0;

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            command.CommandText = @"
                SELECT goal_id FROM goal_progress
                ORDER BY ABS(record_date - CURRENT_DATE) ASC
                LIMIT 1;";
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                goalId = reader.GetInt32(0);
            }
            return goalId;
        }

        // 目標を登録
        public void InsertGoalProgress(GoalProgress goalProgress)
        {
            try
            {
                using var connection = GetConnection();
                using var command = connection.CreateCommand();
                connection.Open();
                command.CommandText = @"
                    INSERT INTO goal_progress (
                        goal_id,
                        goal_set_date,
                        goal_achieved_date,
                        total,
                        goal_status,
                        created_at,
                        updated_at,
                        record_date
                    ) VALUES (
                        @goalId,
                        @goalSetDate,
                        @goalAchievedDate, 
                        @total, 
                        @goalStatus, 
                        @createdAt, 
                        @updatedAt,
                        @recordDate);";

                command.Parameters.AddWithValue("goalId", goalProgress.GoalId);
                command.Parameters.AddWithValue("goalSetDate", DateTime.Now);
                command.Parameters.AddWithValue("goalAchievedDate", (object)goalProgress.GoalAchievedDate ?? DBNull.Value);
                command.Parameters.AddWithValue("total", goalProgress.Total);
                command.Parameters.AddWithValue("goalStatus", goalProgress.GoalStatus);
                command.Parameters.AddWithValue("createdAt", DateTime.Now);
                command.Parameters.AddWithValue("updatedAt", DateTime.Now);
                command.Parameters.AddWithValue("recordDate", DateTime.Now);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"目標の登録に失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 目標の更新
        public void UpdateGoalProgress(GoalProgress goalProgress)
        {
            try
            {
                using var connection = GetConnection();
                using var command = connection.CreateCommand();
                connection.Open();
                command.CommandText = @"
                    UPDATE goal_progress
                    SET 
                        goal_id = @goalId,
                        goal_set_date = @goalSetDate,
                        goal_achieved_date = @goalAchievedDate,
                        total = @total,
                        goal_status = @goalStatus,
                        updated_at = @updatedAt,
                        record_date = @recordDate
                    WHERE goal_progress_id = @goalProgressId;";
                command.Parameters.AddWithValue("goalId", goalProgress.GoalId);
                command.Parameters.AddWithValue("goalSetDate", goalProgress.GoalSetDate);
                command.Parameters.AddWithValue("goalAchievedDate", (object)goalProgress.GoalAchievedDate ?? DBNull.Value);
                command.Parameters.AddWithValue("total", goalProgress.Total);
                command.Parameters.AddWithValue("goalStatus", goalProgress.GoalStatus);
                command.Parameters.AddWithValue("updatedAt", DateTime.Now);
                command.Parameters.AddWithValue("recordDate", DateTime.Now);
                command.Parameters.AddWithValue("goalProgressId", goalProgress.GoalProgressId);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"目標の更新に失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 目標の設定チェック
        public bool IsGoalSelected(int selectedGoalId)
        {
            if (selectedGoalId == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

