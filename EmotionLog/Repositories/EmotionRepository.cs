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
    public class EmotionRepository : BaseRepository
    {
        public List<EmotionType> GetEmotionTypes()
        {
            var list = new List<EmotionType>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "SELECT * FROM emotion_master ORDER BY emotion_type_id ASC;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new EmotionType
                {
                    EmotionTypeId = reader.GetInt32(0),
                    EmotionName = reader.GetString(1),
                    CreatedAt = reader.GetDateTime(2),
                    UpdatedAt = reader.GetDateTime(3)
                });
            }
            return list;
        }
    }
}

