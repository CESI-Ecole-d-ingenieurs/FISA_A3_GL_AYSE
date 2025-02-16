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

namespace ProjetWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Visible;
            Language.Visibility = Visibility.Collapsed;
            Creation.Visibility = Visibility.Collapsed;
            Execution.Visibility = Visibility.Collapsed;
            Logs.Visibility = Visibility.Collapsed;
        }

        /*private void FileEncryptTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FileEncryptPlaceholder.Visibility = string.IsNullOrWhiteSpace(FileEncryptTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BusinessSoftwareTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BusinessSoftwarePlaceholder.Visibility = string.IsNullOrWhiteSpace(BusinessSoftwareTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }*/

        private void ShowLanguage(object sender, RoutedEventArgs e)
        {
            Language.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Collapsed;
            Creation.Visibility = Visibility.Collapsed;
            Execution.Visibility = Visibility.Collapsed;
            Logs.Visibility = Visibility.Collapsed;
        }

        private void ShowCreation(object sender, RoutedEventArgs e)
        {
            Creation.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Collapsed;
            Language.Visibility = Visibility.Collapsed;
            Execution.Visibility = Visibility.Collapsed;
            Logs.Visibility = Visibility.Collapsed;
        }

        private void ShowExecution(object sender, RoutedEventArgs e)
        {
            Execution.Visibility = Visibility.Visible;
            Creation.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Collapsed;
            Language.Visibility = Visibility.Collapsed;
            Logs.Visibility = Visibility.Collapsed;
        }

        private void ShowLogs(object sender, RoutedEventArgs e)
        {
            Logs.Visibility = Visibility.Visible;
            Execution.Visibility = Visibility.Collapsed;
            Creation.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Collapsed;
            Language.Visibility = Visibility.Collapsed;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}