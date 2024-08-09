using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using Microsoft.Win32;
using CountTimer;

namespace CountTimer
{
    struct Timer
    {
        public StackPanel stackPanel;
        public TimeSpan timeSpan;
        public Label nameLabel;
        public TextBox nameTextBox;
        public Label timeLabel;
        public TextBox timeTextBox;
        public Button startButton;
        public Button deleteButton;
        public CancellationTokenSource cts;
    }
    public partial class MainWindow : Window
    {
        private Button buttonADD;
        private MediaPlayer m_mediaPlayer;
        private int margintop = 0;
        private int numberTimer = 0;
        private int addheight = 85;
        private int count = 0;
        List<string> newData = new List<string>();
        List<Timer> timers = new List<Timer>();
        public MainWindow()
        {
            InitializeComponent();
            buttonADD = (Button)grid1.Children[0];
            Application.Current.MainWindow.Width = 373;
            Application.Current.MainWindow.Height = 90;
            Application.Current.MainWindow.Topmost = true;
            if (File.Exists("Data.txt") == true)
            {
                string[] Data = File.ReadAllLines("Data.txt");
                newData = Data.ToList();
                for (int i = 0; i < Data.Length; i++)
                {
                    CreateTimer();
                    newData.RemoveAt(0);
                }
            }
        }

        private void drag(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void closeApp(object sender, MouseEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void minimizeApp(object sender, MouseEventArgs e)
        {
            try
            {
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddTimer(object sender, RoutedEventArgs e)
        {
            CreateTimer();
        }

        private void CreateTimer()
        {
            if (count == 0)
            {
                addheight = 90;
            }
            else
            {
                addheight += 30;
            }

            count++;
            Application.Current.MainWindow.Height = addheight;
            Thickness margin = buttonADD.Margin;
            margin.Top = margintop + 60;
            buttonADD.Margin = margin;
            buttonADD.Height = 20;
            buttonADD.Width = 46;
            buttonADD.Content = "[ADD]";
            buttonADD.Foreground = Brushes.White;
            var brush = new SolidColorBrush(Color.FromArgb(255, 31, 62, 101));
            buttonADD.Background = brush;
            buttonADD.FontSize = 14;
            buttonADD.FontFamily = new FontFamily("Verdana");

            buttonADD.HorizontalAlignment = HorizontalAlignment.Left;
            buttonADD.VerticalAlignment = VerticalAlignment.Top;

            StackPanel stackPanel = new StackPanel();
            stackPanel.Name = "N" + (timers.Count);
            stackPanel.Margin = new Thickness(0, margintop - 7, 0, 0);
            stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanel.VerticalAlignment = VerticalAlignment.Top;
            grid1.Children.Add(stackPanel);

            Timer timer = new Timer
            {
                stackPanel = stackPanel,
                timeSpan = newData.Count > 0 ? TimeSpan.FromSeconds(Int32.Parse(newData[0].Split(';')[1])) : TimeSpan.Zero
            };

            CreateLabelName(ref timer);
            CreateTexBoxName(ref timer);
            CreateLabelTime(ref timer);
            CreateTexBoxTime(ref timer);
            CreateButtonStart(ref timer);
            CreateButtonDelete(ref timer);

            margintop += 30;

            timers.Add(timer);
            SaveFile();
        }

        private void CreateLabelName(ref Timer timer)
        {
            Label label = new Label();
            if (newData.Count > 0)
            {
                string name = newData[0].Split(';')[0];
                label.Content = name;
            }
            else
            {
                label.Content = "Таймер";
            }
            label.Margin = new Thickness(2, 37, 0, 0);
            label.Foreground = Brushes.White;
            label.FontSize = 14;
            label.FontFamily = new FontFamily("Verdana");
            label.Width = 100;
            label.MouseLeftButtonUp += new MouseButtonEventHandler(NameClick);
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;
            timer.stackPanel.Children.Add(label);
            timer.nameLabel = label;
        }

        private void CreateTexBoxName(ref Timer timer)
        {
            TextBox textBox = new TextBox();
            textBox.Foreground = Brushes.White;
            textBox.FontSize = 14;
            textBox.FontFamily = new FontFamily("Verdana");

            var brush = new SolidColorBrush(Color.FromArgb(255, 7, 7, 7));

            textBox.Background = brush;
            textBox.Width = 100;
            textBox.KeyUp += new KeyEventHandler(TextBoxNameKeyUp);
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.Margin = new Thickness(4, -23, 0, 0);
            textBox.Visibility = Visibility.Hidden;
            timer.stackPanel.Children.Add(textBox);
            timer.nameTextBox = textBox;
        }

        private void CreateLabelTime(ref Timer timer)
        {
            Label label = new Label();

            label.FontSize = 14;
            label.FontFamily = new FontFamily("Verdana");
            label.Foreground = Brushes.White;

            label.MouseLeftButtonUp += new MouseButtonEventHandler(TimeClick);
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;
            label.Margin = new Thickness(150, -27, 0, 0);
            label.Content = string.Format("{0}:{1}:{2}", timer.timeSpan.Hours.ToString().ToString().PadLeft(2, '0'), timer.timeSpan.Minutes.ToString().ToString().PadLeft(2, '0'), timer.timeSpan.Seconds.ToString().ToString().PadLeft(2, '0'));
            timer.stackPanel.Children.Add(label);
            timer.timeLabel = label;
        }

        private void CreateTexBoxTime(ref Timer timer)
        {
            TextBox textBox = new TextBox();
            textBox.FontSize = 14;
            textBox.FontFamily = new FontFamily("Verdana");
            textBox.Foreground = Brushes.White;
            var brush = new SolidColorBrush(Color.FromArgb(255, 7, 7, 7));
            textBox.Background = brush;
            textBox.Width = 70;
            textBox.KeyUp += new KeyEventHandler(TextBoxTimeKeyUp);
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.Margin = new Thickness(155, -25, 0, 0);
            textBox.Visibility = Visibility.Hidden;
            timer.stackPanel.Children.Add(textBox);
            timer.timeTextBox = textBox;
        }

        private void CreateButtonStart(ref Timer timer)
        {
            Button button = new Button();
            button.FontSize = 12;
            button.Width = 80;
            button.Height = 20;


            var brush = new SolidColorBrush(Color.FromArgb(255, 31, 62, 101));
            button.Background = brush;
            button.Foreground = Brushes.White;
            button.FontFamily = new FontFamily("Verdana");

            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.Margin = new Thickness(250, -23, 0, 0);
            button.Content = "[Start]";
            button.Click += new RoutedEventHandler(StartStopTimer);
            timer.stackPanel.Children.Add(button);
            timer.startButton = button;
        }

        private void CreateButtonDelete(ref Timer timer)
        {
            Button button = new Button();
            button.FontSize = 20;
            button.Height = 20;
            button.Width = 24;
            button.FontSize = 12;
            var brush = new SolidColorBrush(Color.FromArgb(255, 31, 62, 101));
            button.Background = brush;
            button.Foreground = Brushes.White;
            button.FontFamily = new FontFamily("Verdana");

            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.Margin = new Thickness(340, -23, 0, 0);
            button.Content = "[x]";
            button.Click += new RoutedEventHandler(DeleteTimer);
            timer.stackPanel.Children.Add(button);
            timer.deleteButton = button;
        }

        private int GetTimerIndexFromSender(object sender)
        {
            var element = sender as FrameworkElement;

            if (element?.Parent is StackPanel stackPanel &&
                !string.IsNullOrEmpty(stackPanel.Name) &&
                stackPanel.Name.StartsWith("N") &&
                int.TryParse(stackPanel.Name.Substring(1), out int index) &&
                index >= 0 && index < timers.Count)
            {
                return index;
            }

            throw new InvalidOperationException("Failed to obtain a valid index from the sender.");
        }

        private void NameClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int index = GetTimerIndexFromSender(sender);

                if (index >= 0 && index < timers.Count)
                {
                    Label nameLabel = timers[index].nameLabel;
                    TextBox nameTextBox = timers[index].nameTextBox;
                    nameLabel.Visibility = Visibility.Hidden;
                    nameTextBox.Visibility = Visibility.Visible;
                    nameTextBox.Text = nameLabel.Content.ToString();
                    nameTextBox.Focus();
                    nameTextBox.SelectionStart = nameTextBox.Text.Length;
                }
                else
                {
                    MessageBox.Show("Invalid timer index.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void TextBoxNameKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int index = GetTimerIndexFromSender(sender);
                TextBox textBoxName = timers[index].nameTextBox;
                Label labelName = timers[index].nameLabel;
                labelName.Content = textBoxName.Text;
                labelName.Visibility = Visibility.Visible;
                textBoxName.Visibility = Visibility.Hidden;
                SaveFile();
            }
        }

        private void TimeClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int index = GetTimerIndexFromSender(sender);
                if (index < 0 || index >= timers.Count)
                {
                    MessageBox.Show("Index out of range.");
                    return;
                }

                Label timeLabel = timers[index].timeLabel;
                TextBox timeTextBox = timers[index].timeTextBox;
                timeLabel.Visibility = Visibility.Hidden;
                timeTextBox.Visibility = Visibility.Visible;
                timeTextBox.Text = "";
                timeTextBox.Focus();
                timeTextBox.SelectionStart = timeTextBox.Text.Length;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void TextBoxTimeKeyUp(object sender, KeyEventArgs e)
        {
            int index = GetTimerIndexFromSender(sender);
            TextBox textBoxTime = timers[index].timeTextBox;
            Label labelTime = timers[index].timeLabel;
            string tempText = "";

            for (int i = 0; i < textBoxTime.Text.Length; i++)
            {
                if (Char.IsNumber(textBoxTime.Text[i]))
                {
                    tempText += textBoxTime.Text[i];
                }
            }

            if (e.Key == Key.Enter)
            {
                if (int.TryParse(tempText, out int totalSeconds))
                {
                    int h = (totalSeconds / 3600) % 24;
                    int m = (totalSeconds / 60) % 60;
                    int s = totalSeconds % 60;

                    if (totalSeconds > 86400)
                    {
                        h = 23;
                        m = 59;
                        s = 59;
                    }

                    TimeSpan timeSpan = new TimeSpan(h, m, s);
                    labelTime.Content = string.Format("{0}:{1}:{2}", h.ToString().PadLeft(2, '0'), m.ToString().PadLeft(2, '0'), s.ToString().PadLeft(2, '0'));
                    ChangeTime(index, timeSpan);
                    labelTime.Visibility = Visibility.Visible;
                    textBoxTime.Visibility = Visibility.Hidden;
                    SaveFile();
                }
                else
                {
                    textBoxTime.Text = "";
                }
                return;
            }

            textBoxTime.Text = tempText;
            textBoxTime.SelectionStart = textBoxTime.Text.Length;
        }

        private void StartStopTimer(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = GetTimerIndexFromSender(sender);

                if (index >= 0 && index < timers.Count)
                {
                    Button button = timers[index].startButton;
                    string name = button.Content.ToString();
                    var brush = new SolidColorBrush(Color.FromArgb(255, 31, 62, 101));

                    if (name == "[Start]")
                    {
                        timers[index].timeLabel.Content = string.Format("{0}:{1}:{2}", timers[index].timeSpan.Hours.ToString().PadLeft(2, '0'), timers[index].timeSpan.Minutes.ToString().PadLeft(2, '0'), timers[index].timeSpan.Seconds.ToString().PadLeft(2, '0'));
                        button.Content = "[Reset]";
                        button.Background = brush;
                        CancellationTokenSource cts = new CancellationTokenSource();
                        ChangeToken(index, cts);
                        WorkTimer(index, cts.Token);
                    }
                    else
                    {
                        button.Background = brush;
                        timers[index].cts.Cancel();
                        button.Content = "[Start]";
                        timers[index].timeLabel.Content = string.Format("{0}:{1}:{2}", timers[index].timeSpan.Hours.ToString().PadLeft(2, '0'), timers[index].timeSpan.Minutes.ToString().PadLeft(2, '0'), timers[index].timeSpan.Seconds.ToString().PadLeft(2, '0'));
                    }
                }
                else
                {
                    MessageBox.Show("Invalid timer index.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void DeleteTimer(object sender, RoutedEventArgs e)
        {
            Button deleteButton = (Button)sender;
            Timer timerToRemove = timers.FirstOrDefault(t => t.deleteButton == deleteButton);

            if (timerToRemove.stackPanel != null)
            {
                grid1.Children.Remove(timerToRemove.stackPanel);
            }

            if (timerToRemove.cts != null)
            {
                timerToRemove.cts.Cancel();
            }

            int index = timers.IndexOf(timerToRemove);
            if (index >= 0)
            {
                timers.RemoveAt(index);
                count--;

                margintop = 0;
                for (int i = 0; i < timers.Count; i++)
                {
                    StackPanel stackPanel = timers[i].stackPanel;
                    if (stackPanel != null)
                    {
                        stackPanel.Margin = new Thickness(0, margintop - 7, 0, 0);
                        margintop += 30;
                    }
                }

                if (count == 0)
                {
                    Application.Current.MainWindow.Height = 90;
                    addheight = 90;
                    buttonADD.Margin = new Thickness(7, 30, 0, 0);
                }
                else
                {
                    Application.Current.MainWindow.Height = addheight - 30;
                    addheight -= 30;
                    buttonADD.Margin = new Thickness(7, margintop + 30, 0, 0);
                }
                buttonADD.Visibility = Visibility.Visible;
                SaveFile();
            }
        }

        void ChangeToken(int index, CancellationTokenSource cts)
        {
            Timer timer = timers[index];
            timers.RemoveAt(index);
            timer.cts = cts;
            timers.Insert(index, timer);
        }

        void ChangeTime(int index, TimeSpan timeSpan)
        {
            Timer timer = timers[index];
            timers.RemoveAt(index);
            timer.timeSpan = timeSpan;
            timers.Insert(index, timer);
        }

        private async void WorkTimer(int index, CancellationToken token)
        {
            TimeSpan newTS = timers[index].timeSpan;
            while (newTS.TotalSeconds > 0)
            {
                await Task.Delay(1000);
                try
                {
                    token.ThrowIfCancellationRequested();
                    newTS -= TimeSpan.FromSeconds(1);
                    timers[index].timeLabel.Content = string.Format("{0}:{1}:{2}", newTS.Hours.ToString().ToString().PadLeft(2, '0'), newTS.Minutes.ToString().ToString().PadLeft(2, '0'), newTS.Seconds.ToString().ToString().PadLeft(2, '0'));
                }
                catch
                {
                    return;
                }
            }
            ChangeButtonColor(index, token);
        }

        private async void ChangeButtonColor(int index, CancellationToken token)
        {
            string path = Directory.GetCurrentDirectory();
            m_mediaPlayer = new MediaPlayer();
            m_mediaPlayer.Open(new Uri(path + "/sound.wav"));
            float volume = LoadVolumeSetting();
            m_mediaPlayer.Volume = volume;
            m_mediaPlayer.Play();

            while (true)
            {
                await Task.Delay(500);
                try
                {
                    token.ThrowIfCancellationRequested();
                    timers[index].startButton.Background = Brushes.Yellow;
                    await Task.Delay(500);
                }
                catch
                {
                    return;
                }

                await Task.Delay(500);
                try
                {
                    token.ThrowIfCancellationRequested();
                    timers[index].startButton.Background = Brushes.Red;
                }
                catch
                {
                    return;
                }
            }
        }

        private float LoadVolumeSetting()
        {
            try
            {
                if (File.Exists("config.txt"))
                {
                    var lines = File.ReadAllLines("config.txt");
                    foreach (var line in lines)
                    {
                        var parts = line.Split('=');
                        if (parts.Length == 2 && parts[0].Trim() == "Volume")
                        {
                            if (float.TryParse(parts[1].Trim(), out float volumePercent))
                            {

                                return volumePercent / 100.0f;
                            }
                        }
                    }
                }
                return 0.5f;
            }
            catch
            {
                return 0.5f;
            }
        }

        void SaveFile()
        {
            string data = "";
            for (int i = 0; i < timers.Count; i++)
            {
                var timer = timers[i];
                data += timer.nameLabel.Content + ";";
                data += timer.timeSpan.TotalSeconds + "\n";
            }
            File.WriteAllText("Data.txt", data);
        }

    }
}