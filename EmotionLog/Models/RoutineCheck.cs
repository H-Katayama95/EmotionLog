using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionLog.Models
{
    public class RoutineCheck
    {
        public int RoutineCheckId { get; set; }
        public int GoalId { get; set; }
        public DateTime RecordDate { get; set; }
        public bool IsChecked { get; set; }
        public int Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
