using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MarcroRecord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isRecording = false;
        private MacroRecorderService _macroService = new MacroRecorderService();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _macroService;
            _macroService.KeyDown += MacroService_KeyDown;
        }

        private void MacroService_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (isRecording)
            {
                Button keyButton = new Button
                {
                    Content = e.KeyValue.ToString(),
                    Background = System.Windows.Media.Brushes.Gray,
                    Foreground = System.Windows.Media.Brushes.White,
                    Width = 60,
                    Height = 40,
                    Margin = new Thickness(5)
                };

                KeyWrapPanel.Children.Add(keyButton);
            }
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isRecording)
            {
                Button mouseButton = new Button
                {
                    Content = "Left Click",
                    Background = System.Windows.Media.Brushes.Orange,
                    Foreground = System.Windows.Media.Brushes.White,
                    Width = 100,
                    Height = 40,
                    Margin = new Thickness(5)
                };

                KeyWrapPanel.Children.Add(mouseButton);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            KeyWrapPanel.Children.Clear();
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRecording)
            {
                RecordButton.Background = System.Windows.Media.Brushes.Green;
                RecordButton.Content = "+";
                RecordingText.Visibility = Visibility.Collapsed;
                SaveButton.IsEnabled = true;
                _macroService.StopRecording();
            }
            else
            {
                RecordButton.Background = System.Windows.Media.Brushes.Red;
                RecordButton.Content = "Stop";
                RecordingText.Visibility = Visibility.Visible;
                SaveButton.IsEnabled = false;
                _macroService.StartRecording();
            }

            isRecording = !isRecording;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Saved!");
        }

        private void MainBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected override void OnClosed(EventArgs e)
        {
            _macroService?.KeyEvents?.Dispose();
            base.OnClosed(e);
        }
    }
}