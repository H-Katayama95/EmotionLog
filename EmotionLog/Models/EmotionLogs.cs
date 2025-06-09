using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionLog.Models
{
    public class EmotionLogs
    {
        public int EmotionLogId { get; set; }
        public DateTime RecordDate { get; set; }
        public int? MorningEmotionType { get; set; }
        public int? NoonEmotionType { get; set; }
        public int? EveningEmotionType { get; set; }
        public string? MorningDetail { get; set; } ="";
        public string? NoonDetail { get; set; } = "";
        public string? EveningDetail { get; set; } = "";
        public int ConsecutiveRecord { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
