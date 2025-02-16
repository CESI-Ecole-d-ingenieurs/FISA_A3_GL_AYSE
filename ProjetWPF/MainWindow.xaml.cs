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
using EasySave.ControllerLib;
using EasySave.IviewLib;
using EasySave.ModelLib;

namespace ProjetWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LanguageModel languageModel = new LanguageModel();
        LanguagesView languageView = new LanguagesView();

        MenuModel menuModel = new MenuModel();
        Format format = new Format();

        Backup backup = new Backup();
        ControllerBackup controllerBackup = new ControllerBackup();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            format.ClearScreen();
            format.DisplayActions(menuModel.LogFormats, 0);

            Settings.Visibility = Visibility.Visible;
            Languages.Visibility = Visibility.Collapsed;
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

        private void ShowLanguages(object sender, RoutedEventArgs e)
        {
            languageView.DisplayLanguages(languageModel.Languages, 0);
            
            Languages.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Collapsed;
            Creation.Visibility = Visibility.Collapsed;
            Execution.Visibility = Visibility.Collapsed;
            Logs.Visibility = Visibility.Collapsed;
        }

        private void ShowCreation(object sender, RoutedEventArgs e)
        {
            Creation.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Collapsed;
            Languages.Visibility = Visibility.Collapsed;
            Execution.Visibility = Visibility.Collapsed;
            Logs.Visibility = Visibility.Collapsed;
        }

        private async void ShowExecution(object sender, RoutedEventArgs e)
        {
            await controllerBackup.DisplayBackups();

            Execution.Visibility = Visibility.Visible;
            Creation.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Collapsed;
            Languages.Visibility = Visibility.Collapsed;
            Logs.Visibility = Visibility.Collapsed;
        }

        private void ShowLogs(object sender, RoutedEventArgs e)
        {
            Logs.Visibility = Visibility.Visible;
            Execution.Visibility = Visibility.Collapsed;
            Creation.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Collapsed;
            Languages.Visibility = Visibility.Collapsed;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void LanguageChange(object sender, EventArgs e)
        {
            languageModel.SelectedLanguage = Language.SelectedIndex;

            LanguageController languageController = new LanguageController(languageModel, languageView);

            languageController.ApplyLanguageSelection();

            Exit_b.Content = await Translation.Instance.Translate("Quitter");

            Settings_b.Content = await Translation.Instance.Translate("Paramètrage");
            Format.Text = await Translation.Instance.Translate("Format de l’historique :");
            Crypt.Text = await Translation.Instance.Translate("Fichiers à crypter :");
            Software.Text = await Translation.Instance.Translate("Logiciels métier :");

            Create_b.Content = await Translation.Instance.Translate("Créer une sauvegarde");
            Name.Text = await Translation.Instance.Translate("Nom :");
            Source.Text = await Translation.Instance.Translate("Source :");
            Destination.Text = await Translation.Instance.Translate("Destination :");
            Type.Text = await Translation.Instance.Translate("Type :");
            Complete.Content = await Translation.Instance.Translate("Complète");
            Differential.Content = await Translation.Instance.Translate("Différentielle");

            Execute_b.Text = (await Translation.Instance.Translate("Exécuter une / plusieurs sauvegardes")).ToString();
            Available.Text = await Translation.Instance.Translate("Sauvegardes disponibles :");
            ToDo.Text = await Translation.Instance.Translate("Sauvegardes à effectuer :");
            State.Text = await Translation.Instance.Translate("Etat en temps réel :");

            Consult_b.Content = await Translation.Instance.Translate("Consulter l'historique");
            History.Text = await Translation.Instance.Translate("Historique :");
        }

        private async void SettingsChange(object sender, EventArgs e)
        {
            MenuController menuController = new MenuController(menuModel, format);

            await menuController.HandleLogFormat(Format_list.SelectedIndex);
        }

        private async void BackupCreation(object sender, EventArgs e)
        {
            BackupController backupController = new BackupController(backup);

            await backupController.CreateBackup();
        }

        private void BackupExecution(object sender, EventArgs e)
        {
            
        }
    }
}