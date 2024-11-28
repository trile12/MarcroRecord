using MarcroRecord.Helper;
using MarcroRecord.Model;
using MarcroRecord.ViewModel;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
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
        public string _currentMacroName;
        private MacroRecorderService _macroService = new MacroRecorderService();
        private MainViewModel _mainViewModel = new MainViewModel();
        private readonly string dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _mainViewModel;
            _macroService.KeyDown += MacroService_KeyDown;
            _macroService.MouseDown += MacroService_MouseDown;
        }

        private void MacroService_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (_mainViewModel.IsRecording)
            {
                Button keyButton = new Button
                {
                    Content = e.KeyCode.ToString(),
                    Background = System.Windows.Media.Brushes.Gray,
                    Foreground = System.Windows.Media.Brushes.White,
                    Width = 60,
                    Height = 40,
                    Margin = new Thickness(5)
                };

                KeyWrapPanel.Children.Add(keyButton);
            }
        }

        private void MacroService_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_mainViewModel.IsRecording)
            {
                Button mouseButton = new Button
                {
                    Content = e.Button == System.Windows.Forms.MouseButtons.Left ? "Left Click" : "Right Click",
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
            if (_mainViewModel.IsRecording)
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

            _mainViewModel.IsRecording = !_mainViewModel.IsRecording;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveToJson(_currentMacroName);
            ResetView();
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

        private void OnNameInputOK(object sender, RoutedEventArgs e)
        {
            _currentMacroName = NameInputTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(_currentMacroName))
            {
                return;
            }

            if (!_currentMacroName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                _currentMacroName += ".json";
            }

            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            string filePath = Path.Combine(dataFolder, _currentMacroName);
            if (File.Exists(filePath))
            {
                return;
            }

            NameInputPanel.Visibility = Visibility.Collapsed;
            RecordPanel.Visibility = Visibility.Visible;
        }

        private void ResetView()
        {
            _currentMacroName = "";
            _mainViewModel?.MacroEvents?.Clear();
            KeyWrapPanel.Children.Clear();

            NameInputPanel.Visibility = Visibility.Visible;
            RecordPanel.Visibility = Visibility.Collapsed;
            LoadSavedMacroModels();
        }

        public void SaveToJson(string fileName)
        {
            try
            {
                if (!Directory.Exists(dataFolder))
                {
                    Directory.CreateDirectory(dataFolder);
                }

                string filePath = Path.Combine(dataFolder, fileName);
                MacroModel macroModel = new MacroModel();
                macroModel.Name = Path.GetFileNameWithoutExtension(fileName);
                macroModel.MacroEvents = _mainViewModel.MacroEvents.ToList();
                string json = JsonConvert.SerializeObject(macroModel, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
            }
        }

        private void LoadSavedMacroModels()
        {
            if (Directory.Exists(dataFolder))
            {
                var files = Directory.GetFiles(dataFolder, "*.json");

                foreach (var file in files)
                {
                    var model = LogicHelper.GetMarcroModelFromJson(file);
                    if (model != null)
                        _mainViewModel.MacroModels.Add(new MacroModel { Name = model.Name, Key = model.Key });
                }
                SavedMacrosList.ItemsSource = _mainViewModel.MacroModels;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSavedMacroModels();
        }
    }
}