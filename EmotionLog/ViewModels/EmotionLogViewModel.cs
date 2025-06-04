using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmotionLog.Models;
using EmotionLog.Repositories;

namespace EmotionLog.ViewModels
{
    public class EmotionLogViewModel
    {
        private EmotionRepository repo = new EmotionRepository();

        public ObservableCollection<EmotionType> EmotionTypes { get; set; }

        public EmotionLogViewModel()
        {
        }
    }
}
