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
        public int EmotionTypeId { get; set; }
        public DateTime RecordDate { get; set; }
        public int TimeZone { get; set; } // 0: Morning, 1: Noon, 2: Evening
        public string? Detail { get; set; }
        public int ConsecutiveRecord { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
