using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionLog.Models
{
    public class GoalProgress
    {
        public int GoalProgressId { get; set; }
        public int GoalId { get; set; }
        public DateTime GoalSetDate { get; set; }
        public DateTime? GoalAchievedDate { get; set; }
        public int Total { get; set; }
        public bool IsAchieved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
