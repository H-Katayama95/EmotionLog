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
        // 登録済の目標を取得
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

