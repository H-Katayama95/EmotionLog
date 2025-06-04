using System.Globalization;
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
using EmotionLog.ViewModels;

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
            // 感情ログのデータを読み込む
            LoadEmotionLogs();
        }

        private void LoadGoals()
        {
            var repo = new GoalRepository();
            List<Goal> goals = repo.GetGoal();

            GoalComboBox.ItemsSource = goals;
            GoalComboBox.DisplayMemberPath = "GoalContent";
            GoalComboBox.SelectedValuePath = "GoalId";
            GoalComboBox.SelectedValuePath = "GoalCount";
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
        }
        private void LoadGoalProgresses()
        {
            var repo = new GoalProgressRepository();
            List<GoalProgress> goalProgresses = repo.GetGoalProgress();
            // 仮でトータル回数を抽出し設定
            Total.Text = goalProgresses.Where(x => x.GoalProgressId == 1).First().Total.ToString();
        }

        private void LoadEmotionLogs()
        {
            var repo = new EmotionLogsRepository();
            List<EmotionLogs> emotionLogs = repo.GetEmotionLogs();
            // 仮で連続記録日数を抽出し設定
            ConsecutiveRecord.Text = emotionLogs.Where(x => x.EmotionLogId == 1).First().ConsecutiveRecord.ToString("d");
        }

        private void GoalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string goalCount = GoalComboBox.SelectedValue?.ToString() ?? "0";
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