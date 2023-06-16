namespace FarmerbotWebUI.Client.Views
{
    public class ConfigureBotView
    {
        public string BotName { get; set; } = "New FarmerBot";
        public int Id { get; set; } = 0;
        public ComposeYamlView ComposeYamlView { get; set; } = new ComposeYamlView();
    }

    public class ComposeYamlView
    {
        public bool ComposeSwitch { get; set; } = false;
        public bool BusyLoadingYaml { get; set; } = false;
        public string TempComposeDlUrl { get; set; } = string.Empty;
        public string TempComposeYaml { get; set; } = string.Empty;
    }
}
