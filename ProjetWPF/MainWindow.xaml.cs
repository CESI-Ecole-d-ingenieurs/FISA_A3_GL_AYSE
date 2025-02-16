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
        Visible visible = new Visible();

        LanguageModel languageModel = new LanguageModel();
        LanguagesView languageView = new LanguagesView();

        MenuModel menuModel = new MenuModel();
        Format format = new Format();

        Backup backup = new Backup();
        ControllerBackup controllerBackup = new ControllerBackup();

        Logs logs = new Logs();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            format.ClearScreen();
            format.DisplayActions(menuModel.LogFormats, 0);

            visible.Show("Settings");
        }

        private void ShowLanguages(object sender, RoutedEventArgs e)
        {
            languageView.DisplayLanguages(languageModel.Languages, 0);

            visible.Show("Languages");
        }

        private void ShowCreation(object sender, RoutedEventArgs e)
        {
            visible.Show("Creation");
        }

        private async void ShowExecution(object sender, RoutedEventArgs e)
        {
            await controllerBackup.DisplayBackups();

            visible.Show("Execution");
        }

        private async void ShowLogs(object sender, RoutedEventArgs e)
        {
            await logs.DisplayLogs();

            visible.Show("Logs");
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void LanguageChange(object sender, EventArgs e)
        {
            Language_OK.Text = await Translation.Instance.Translate("Traduction en cours...");

            languageModel.SelectedLanguage = Language.SelectedIndex;

            LanguageController languageController = new LanguageController(languageModel, languageView);

            languageController.ApplyLanguageSelection();

            Exit_b.Content = await Translation.Instance.Translate("Quitter");

            Settings_b.Content = await Translation.Instance.Translate("Paramètrage");
            Format.Text = await Translation.Instance.Translate("Format d'historique :");
            Crypt.Text = await Translation.Instance.Translate("Fichiers à crypter :");
            Software.Text = await Translation.Instance.Translate("Logiciels métier :");

            Create_b.Content = await Translation.Instance.Translate("Création de sauvegarde");
            Name.Text = await Translation.Instance.Translate("Nom :");
            Source.Text = await Translation.Instance.Translate("Source :");
            Destination.Text = await Translation.Instance.Translate("Destination :");
            Type.Text = await Translation.Instance.Translate("Type :");
            Complete.Content = await Translation.Instance.Translate("Complète");
            Differential.Content = await Translation.Instance.Translate("Différentielle");

            Execute_b.Text = (await Translation.Instance.Translate("Exécuter une / plusieurs sauvegardes")).ToString();
            Available.Text = await Translation.Instance.Translate("Sauvegardes disponibles :");
            ToDo.Text = await Translation.Instance.Translate("Sauvegardes à faire :");
            State.Text = await Translation.Instance.Translate("Etat en temps réel :");

            Consult_b.Content = await Translation.Instance.Translate("Consulter l'historique");
            History.Text = await Translation.Instance.Translate("Historique :");

            Language_OK.Text = await Translation.Instance.Translate("La langue a été modifiée avec succès");
        }

        private async void SettingsChange(object sender, EventArgs e)
        {
            MenuController menuController = new MenuController(menuModel, format);

            await menuController.HandleLogFormat(Format_list.SelectedIndex);

            Settings_OK.Text = await Translation.Instance.Translate("Les paramètres ont été modifiés avec succès.");
        }

        private async void BackupCreation(object sender, EventArgs e)
        {
            BackupController backupController = new BackupController(backup);

            await backupController.CreateBackup();

            Creation_OK.Text = await Translation.Instance.Translate("La sauvegarde a été créer avec succès.");
        }

        private void BackupExecution(object sender, EventArgs e)
        {
            BackupController backupController = new BackupController(backup);
            RealTimeState realTimeState = new RealTimeState();

            backupController.ExecuteBackup(ToDo_t.Text, realTimeState);
        }
    }
}