namespace FarmerbotWebUI.Server
{
    public class StartupService
    {
        private readonly ISettingsService _settingsService;

        public StartupService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task LoadDatabaseData()
        {
            // Laden Sie die Datenbankdaten hier
        }

        public async Task LoadSettingsFromFile()
        {
            // Rufen Sie die entsprechende Methode in _settingsService auf, um die Einstellungen aus der Datei zu laden
        }

        public async Task RunStartupTasks()
        {
            await LoadDatabaseData();
            await LoadSettingsFromFile();
        }
    }
}