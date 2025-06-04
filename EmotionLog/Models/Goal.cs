using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace EmotionLog.Models
{
    public class Goal
    {
        public int GoalId { get; set; }
        public string GoalContent { get; set; } = string.Empty;
        public int GoalLevel { get; set; }
        public int GoalCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
