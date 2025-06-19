using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EmotionLog.Models;
using EmotionLog.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmotionLog
{
    /// <summary>
    /// PastRecordsPage.xaml の相互作用ロジック
    /// </summary>
    public partial class PastRecordsPage : Page
    {
        private bool _isInitialized = true;
        public PastRecordsPage(List<PastEmotionLogs> pastEmotionLogs)
        {
            InitializeComponent();
            LoadPastEmotionLogs(pastEmotionLogs);
            _isInitialized = false;
        }

        private void LoadPastEmotionLogs(List<PastEmotionLogs> pastEmotionLogList)
        {
            var list = pastEmotionLogList.Any() ? pastEmotionLogList.Last() : null;
            if (list == null)
            {
                return;
            }
            else
            {
                RecordDate.Text = list.RecordDate.ToString("d");
                SetPastEmotionLogs(pastEmotionLogList, RecordDate.Text);
            }
        }

        private void SetPastEmotionLogs(List<PastEmotionLogs> pastEmotionLogList, string date)
        {
            var today = pastEmotionLogList.Where(x => x.RecordDate.ToString("d") == date).FirstOrDefault();

            if (today == null)
            {
                MoringRecord.Text = "記録はありません。";
                return;
            }
            else
            {
                MoringRecord.Text = $"朝：{(today.MorningDetail == string.Empty ? "記録はありません。" : today.MorningDetail)}";
                MoringEmotion.Text = $"気持ち：{(today.MorningEmoName == string.Empty ? "記録はありません。" : today.MorningEmoName)}";
                NoonRecord.Text = $"昼：{(today.NoonDetail == string.Empty ? "記録はありません。" : today.NoonDetail)}";
                NoonEmotion.Text = $"気持ち：{(today.NoonEmoName == string.Empty ? "記録はありません。" : today.NoonEmoName)}";
                EveningRecord.Text = $"夕方：{(today.EveningDetail == string.Empty ? "記録はありません。" : today.EveningDetail)}";
                EveningEmotion.Text = $"気持ち：{(today.EveningEmoName == string.Empty ? "記録はありません。" : today.EveningEmoName)}";
                LoadRoutineCheck(date);
            }
        }

        private void LoadRoutineCheck(string date)
        {
            var repo = new RoutineCheckRepository();
            PastRoutineCheck routineCheck = repo.GetPastRoutineCheck().LastOrDefault(x => x.RecordDate.ToString("d") == date);
            if(routineCheck == null)
            {
                RoutineText.Text = "記録していません";
                return;
            }
            else
            {
                RoutineText.Text = $"{routineCheck.GoalContent}：{(routineCheck.IsChecked ? "○" : "×")}";
            }
        }

        private void RecordDate_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
            {
                return;
            }
            else
            {
                var date = RecordDate.Text;
                if (date != null)
                {
                    var repo = new EmotionLogsRepository();
                    var selectedData = repo.GetPastEmotionLogs().Where(x => x.RecordDate.ToString("d") == date).ToList();
                    if(selectedData.Count == 0)
                    {
                        MoringRecord.Text = "記録はありません。";
                        MoringEmotion.Text = string.Empty;
                        NoonRecord.Text = string.Empty;
                        NoonEmotion.Text = string.Empty;
                        EveningEmotion.Text = string.Empty;
                        EveningRecord.Text = string.Empty;
                        RoutineText.Text = "記録はありません。";
                        return;
                    }
                    else
                    {
                        SetPastEmotionLogs(selectedData, date);
                    }
                }
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Home());

        }
    }
}
