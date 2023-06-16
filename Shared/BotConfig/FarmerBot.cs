using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerbotWebUI.Shared.BotConfig
{
    public class FarmerBot
    {
        public string Name { get; set; } = "New FarmerBot";
        public int Id { get; set; } = 0;
        public FarmerBotConfig FarmerBotConfig { get; set; } = new FarmerBotConfig();
        public DockerCompose DockerCompose { get; set; } = new DockerCompose();
        public EnvFile Env { get; set; } = new EnvFile();        
        public string WorkingDirectory { get; set; } = string.Empty;
        public string ComposeFile { get; set; } = string.Empty;
        public string EnvFile { get; set; } = string.Empty;
        public string FarmerBotConfigFile { get; set; } = string.Empty;
        public string FarmerBotLogFile { get; set; } = string.Empty;
        public long FarmId { get; set; } = 0;
        public string Network { get; set; } = string.Empty;
        public string NetworkRelay { get; set; } = string.Empty;

        public bool IsError
        {
            get
            {
                return (FarmerBotConfig?.IsError ?? false) ||
                       (DockerCompose?.IsError ?? false) ||
                       (Env?.IsError ?? false);
            }
        }

        public string ErrorMessage
        {
            get
            {
                var errorMessageBuilder = new StringBuilder();

                if (FarmerBotConfig?.IsError ?? false)
                {
                    errorMessageBuilder.AppendLine(FarmerBotConfig.ErrorMessage);
                }

                if (DockerCompose?.IsError ?? false)
                {
                    errorMessageBuilder.AppendLine(DockerCompose.ErrorMessage);
                }

                if (Env?.IsError ?? false)
                {
                    errorMessageBuilder.AppendLine(Env.ErrorMessage);
                }

                return errorMessageBuilder.ToString();
            }
        }
    }
}