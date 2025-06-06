using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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

namespace EmotionLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 現在の日時を取得
            DateTime now = DateTime.Now;
            // 英語の曜日を取得
            string dayOfWeek = now.ToString("ddd",new CultureInfo("en-US"));
            DateTextBlock.Text = $"{now:d}({dayOfWeek})";
            // Comboboxに目標マスタのデータを読み込む
            LoadGoals();
            // Comboboxに感情タイプのデータを読み込む
            LoadEmotionTypes();
            // 目標進捗のデータを読み込む
            LoadGoalProgresses();
            // 連続記録日数を読み込む
            LoadConsecutiveRecord();
            // システム起動日の感情ログを読み込む
            LoadEmotionLogs();
            // ルーチンチェックのデータを読み込む
            LoadRoutineCheck();
        }

        private void LoadGoals()
        {
            var repo = new GoalRepository();
            List<Goal> goals = repo.GetGoal();

            GoalComboBox.ItemsSource = goals;
            GoalComboBox.DisplayMemberPath = "GoalContent";
            GoalComboBox.SelectedValuePath = "GoalId";

            // 登録済の場合、目標を取得し、コンボボックスに設定
            var goalProgressRepo = new GoalProgressRepository();
            List<GoalProgress> goalProgresses = goalProgressRepo.GetGoalProgress();
            
            if (goalProgresses != null)
            {
                GoalProgress goalId = goalProgresses.Where(x => !x.GoalStatus).FirstOrDefault();

                // 最も近い目標を選択
                GoalComboBox.SelectedValue = goalId?.GoalId;
            }
        }


        private void LoadEmotionTypes()
        {
            var repo = new EmotionRepository();
            List<EmotionType> emotionTypes = repo.GetEmotionTypes();
            MorningComboBox.ItemsSource = emotionTypes;
            MorningComboBox.DisplayMemberPath = "EmotionName";
            MorningComboBox.SelectedValuePath = "EmotionTypeId";
            NoonComboBox.ItemsSource = emotionTypes;
            NoonComboBox.DisplayMemberPath = "EmotionName";
            NoonComboBox.SelectedValuePath = "EmotionTypeId";
            EveningComboBox.ItemsSource = emotionTypes;
            EveningComboBox.DisplayMemberPath = "EmotionName";
            EveningComboBox.SelectedValuePath = "EmotionTypeId";

            // 登録済の場合、感情ログを取得し、コンボボックスに設定
            var logsRepo = new EmotionLogsRepository();
            EmotionLogs emotionLogs = logsRepo.GetEmotionLogs();
            if (emotionLogs !=  null)
            {
                MorningComboBox.SelectedValue = emotionLogs.MorningEmotionType;
                NoonComboBox.SelectedValue = emotionLogs.NoonEmotionType;
                EveningComboBox.SelectedValue = emotionLogs.EveningEmotionType;
            }
        }
        private void LoadGoalProgresses()
        {
            var repo = new GoalProgressRepository();
            List<GoalProgress> goalProgresses = repo.GetGoalProgress();
            // 登録済のトータル回数を抽出し設定
            GoalProgress total = goalProgresses.Where(x => !x.GoalStatus).FirstOrDefault();
            Total.Text = total?.Total.ToString() ?? "0";    
        }

        // システム起動日の感情ログを取得し、出来事と感情を設定する
        private void LoadEmotionLogs()
        {
            var repo = new EmotionLogsRepository();
            EmotionLogs emotionLogs = repo.GetEmotionLogs();
            // 出来事を設定
            MorningTextBox.Text = emotionLogs.MoringDetail ?? string.Empty;
            NoonTextBox.Text = emotionLogs.NoonDetail ?? string.Empty;
            EveningTextBox.Text = emotionLogs.EveningDetail ?? string.Empty;
        }

        // システム起動日の1日前の連続記録日数を取得
        private void LoadConsecutiveRecord()
        {
            var repo = new EmotionLogsRepository();
            int consecutiveRecord = repo.GetConsecutiveRecord();
            ConsecutiveRecord.Text = consecutiveRecord.ToString("d");
        }

        private void LoadRoutineCheck()
        {
            var repo = new RoutineCheckRepository();
            List<RoutineCheck> routineChecks = repo.GetRoutineCheck();
        }

        private void GoalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGoal = GoalComboBox.SelectedItem as Goal;

            string goalCount = selectedGoal.GoalCount.ToString() ?? "0";
            GoalCount.Text = goalCount;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RoutineCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string total = Total.Text;
            total = (int.Parse(total) + 1).ToString();
            Total.Text = total;
        }
        private void RoutineCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            string total = Total.Text;
            total = (int.Parse(total) - 1).ToString();
            Total.Text = total;
        }


        private void MorningTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void EveningTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}