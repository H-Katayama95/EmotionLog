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
            LoadConsecutiveRecords();
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

            // 目標を登録済の場合、目標コンボボックスに設定
            var goalProgressRepo = new GoalProgressRepository();
            int goalId = goalProgressRepo.GetGoalId();

            if (goalId != 0)
            {
                // 登録済の目標を設定
                GoalComboBox.SelectedValue = goalId;
                GoalSaveButton.Content = "変更"; // ボタンのテキストを変更
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

        // 目標進捗の初期表示
        private void LoadGoalProgresses()
        {
            var repo = new GoalProgressRepository();
            List<GoalProgress> goalProgressList = repo.GetRecentGoalProgress();
            GoalProgress goalProgress = goalProgressList.Where(x => x.GoalStatus == false).FirstOrDefault();

            // 登録済のトータル回数を抽出し設定
            if (goalProgress != null)
            {
                GoalComboBox.SelectedValue = goalProgress.GoalId;
                Total.Text = goalProgress.Total.ToString();
            }
            else
            {
                Total.Text = "0"; // 初期値は0
            }
        }

        // システム起動日の感情ログを取得し、出来事と感情を設定する
        private void LoadEmotionLogs()
        {
            var repo = new EmotionLogsRepository();
            EmotionLogs emotionLogs = repo.GetEmotionLogs();
            // 出来事を設定
            MorningTextBox.Text = emotionLogs.MorningDetail ?? string.Empty;
            NoonTextBox.Text = emotionLogs.NoonDetail ?? string.Empty;
            EveningTextBox.Text = emotionLogs.EveningDetail ?? string.Empty;
        }

        // システム起動日の1日前の連続記録日数を取得
        private void LoadConsecutiveRecords()
        {
            var repo = new EmotionLogsRepository();
            ConsecutiveRecord consecutiveRecord = repo.GetConsecutiveRecords();
            if (consecutiveRecord.Today_Consecutive_Record > 0)
            {
                ConsecutiveRecord.Text = consecutiveRecord.Today_Consecutive_Record.ToString("d");
            }
            else if (consecutiveRecord.Yesterday_Consecutive_Record > 0)
            {
                ConsecutiveRecord.Text = consecutiveRecord.Yesterday_Consecutive_Record.ToString("d");
            }
            else
            {
                ConsecutiveRecord.Text = "0";
            }
        }


        private void LoadRoutineCheck()
        {
            var repo = new RoutineCheckRepository();
            List<RoutineCheck> routineChecks = repo.GetRoutineCheck();
        }

        private void GoalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Goal? selectedGoal = GoalComboBox.SelectedItem as Goal;

            string goalCount = selectedGoal?.GoalCount.ToString() ?? "0";
            GoalCount.Text = goalCount;
        }

        // 朝の記録と感情を保存
        private void MorningSaveButton_Click(object sender, RoutedEventArgs e)
        {
            saveButton_Click(sender, e);
        }

        // 昼の記録と感情を保存
        private void NoonSaveButton_Click(object sender, RoutedEventArgs e)
        {
            saveButton_Click(sender, e);
        }

        // 夜の記録と感情を保存
        private void EveningSaveButton_Click(object sender, RoutedEventArgs e)
        {
            saveButton_Click(sender, e);
        }

        // 共通処理
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            var repo = new EmotionLogsRepository();

            EmotionLogs emotionLogs = new EmotionLogs
            {
                MorningDetail = MorningTextBox.Text != null ? MorningTextBox.Text : string.Empty,
                MorningEmotionType = MorningComboBox.SelectedValue != null ? (int)MorningComboBox.SelectedValue : 0,
                NoonDetail = NoonTextBox.Text != null ? NoonTextBox.Text : string.Empty,
                NoonEmotionType = NoonComboBox.SelectedValue != null ? (int)NoonComboBox.SelectedValue : 0,
                EveningDetail = EveningTextBox.Text != null ? EveningTextBox.Text : string.Empty,
                EveningEmotionType = EveningComboBox.SelectedValue != null ? (int)EveningComboBox.SelectedValue : 0,
                ConsecutiveRecord = ConsecutiveRecord.Text != null ? int.Parse(ConsecutiveRecord.Text) + 1 : 0,
            };
            // 文字数の入力チェック
            bool isValid = repo.ValidateEmotionLogs(emotionLogs);
            if (!isValid)
            {
                MessageBox.Show("140字以内で入力してください", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                // 感情ログを保存
                repo.UpsertEmotionLogs(emotionLogs);
                //　連続記録日数の表示を更新
                ConsecutiveRecord todayConsectiveRecord = repo.GetConsecutiveRecords();
                ConsecutiveRecord.Text = todayConsectiveRecord.Today_Consecutive_Record.ToString("d");
            }
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
            if(int.Parse(total) > 0)
            {
                total = (int.Parse(total) - 1).ToString();
            }
            Total.Text = total;
        }


        private void MorningTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void EveningTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void GoalSaveButton_Click(object sender, RoutedEventArgs e)
        {
            var repo = new GoalProgressRepository();

            int selectedGoalId = GoalComboBox.SelectedValue != null ? (int)GoalComboBox.SelectedValue : 0;

            // 目標が選択されているかチェック
            bool isChecked = repo.IsGoalSelected(selectedGoalId);

            if (!isChecked)
            {
                MessageBox.Show("目標を選択してください。", "選択エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                List<GoalProgress> goalProgressList = repo.GetGoalProgress();

                // すでに目標が登録されている場合、目標の変更処理を行う
                if (goalProgressList.Count > 0)
                {
                    ChangeGoal(repo, goalProgressList, selectedGoalId);
                }
                else
                {
                    if (GoalComboBox.SelectedValue is int selectedId)
                    {
                        repo.InsertGoalProgress(new GoalProgress
                        {
                            GoalId = (int)GoalComboBox.SelectedValue,
                            GoalSetDate = DateTime.Now,
                            Total = 0, // 初期値は0
                            GoalStatus = false, // 初期状態は未達成
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            RecordDate = DateTime.Now
                        });
                    }
                    MessageBox.Show("目標を登録しました。", "登録完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                // 目標を登録したので、ボタンのテキストを変更
                GoalSaveButton.Content = "変更";
            }
        }

        //　目標の変更処理
        private void ChangeGoal(GoalProgressRepository repo, List<GoalProgress> goalProgressList, int selectedGoalId)
        {
            if (MessageBoxResult.Cancel == MessageBox.Show("回数がリセットされてしまいますが、目標を変更してもよろしいでしょうか？", "変更確認", MessageBoxButton.OKCancel, MessageBoxImage.Information))
            {
                return; // キャンセルされた場合は処理を中止
            }
            else
            {
                GoalProgress latestGoalProgress = goalProgressList.FirstOrDefault();

                // 前の目標のステータスを済に更新する
                latestGoalProgress.GoalStatus = true; // 目標は達成済みとする
                repo.UpdateGoalProgress(latestGoalProgress);

                // 新しい目標を登録
                repo.InsertGoalProgress(new GoalProgress
                {
                    GoalId = selectedGoalId,
                    GoalSetDate = DateTime.Now,
                    Total = 0, // 初期値は0
                    GoalStatus = false, // 初期状態は未達成
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    RecordDate = DateTime.Now
                });

                Total.Text = "0"; // トータル回数をリセット
            }
        }

        private void RoutineSaveButton_Click(object sender, RoutedEventArgs e)
        {
            var repo = new GoalProgressRepository();
            var goalProgress = repo.GetGoalProgress().FirstOrDefault();
            goalProgress.Total = int.Parse(Total.Text);

            if(goalProgress.Total >= int.Parse(GoalCount.Text))
            {
                goalProgress.GoalAchievedDate = DateTime.Now; // 目標達成日を設定
                goalProgress.GoalStatus = true; // 目標は達成済み
                MessageBox.Show("目標を達成しました！", "目標達成", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                goalProgress.GoalStatus = false; // 目標は未達成のまま
                MessageBox.Show("今日の日課を登録しました！", "日課登録", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            repo.UpdateGoalProgress(goalProgress);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}