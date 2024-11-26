using System;
using System.Collections.Generic;
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

namespace MarcroRecord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MacroRecorderService _macroService = new MacroRecorderService();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _macroService;
        }

        private void StartRecording_Click(object sender, RoutedEventArgs e)
        {
            _macroService.StartRecording();
        }

        private void StopRecording_Click(object sender, RoutedEventArgs e)
        {
            _macroService.StopRecording();
        }

        private async void PlayMacro_Click(object sender, RoutedEventArgs e)
        {
            await _macroService.PlayMacroAsync();
            MessageBox.Show("Playback finished.");
        }
    }
}
