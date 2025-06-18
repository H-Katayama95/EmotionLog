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

        public List<PastRoutineCheck> GetPastRoutineCheck()
        {
            var list = new List<PastRoutineCheck>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            command.CommandText = @"
            SELECT r.routine_check_id, 
            r.record_date,
            r.is_checked,
            g.goal_content
            FROM routine_check r
            JOIN goal_master g
            ON g.goal_id = r.goal_id
            ORDER BY r.routine_check_id ASC;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new PastRoutineCheck
                {
                    RoutineCheckId = reader.GetInt32(0),
                    RecordDate = reader.GetDateTime(1),
                    IsChecked = reader.GetBoolean(2),
                    GoalContent = reader.GetString(3)
                });
            }
            return list;
        }



        public void InsertRoutineCheck(RoutineCheck routineCheck)
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();
            command.CommandText = "INSERT INTO routine_check (goal_id, record_date, is_checked, total, created_at, updated_at) " +
                                  "VALUES (@GoalId, @RecordDate, @IsChecked, @Total, @CreatedAt, @UpdatedAt);";
            command.Parameters.AddWithValue("GoalId", routineCheck.GoalId);
            command.Parameters.AddWithValue("RecordDate", routineCheck.RecordDate);
            command.Parameters.AddWithValue("IsChecked", routineCheck.IsChecked);
            command.Parameters.AddWithValue("Total", routineCheck.Total);
            command.Parameters.AddWithValue("CreatedAt", DateTime.Now);
            command.Parameters.AddWithValue("UpdatedAt", DateTime.Now);
            command.ExecuteNonQuery();
        }

        public void UpsertRoutineCheck(RoutineCheck routineCheck)
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();
            command.CommandText = @"
                INSERT INTO routine_check (goal_id, record_date, is_checked, total, created_at, updated_at)
                VALUES (@GoalId, @RecordDate, @IsChecked, @Total, NOW(), NOW())
                ON CONFLICT (goal_id, record_date) DO UPDATE SET
                    is_checked = EXCLUDED.is_checked,
                    total = EXCLUDED.total,
                    updated_at = NOW();";
            command.Parameters.AddWithValue("GoalId", routineCheck.GoalId);
            command.Parameters.AddWithValue("RecordDate", routineCheck.RecordDate);
            command.Parameters.AddWithValue("IsChecked", routineCheck.IsChecked);
            command.Parameters.AddWithValue("Total", routineCheck.Total);
            command.ExecuteNonQuery();
        }
    }
}

