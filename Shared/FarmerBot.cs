using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerbotWebUI.Shared
{
    public class FarmerBot
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public FarmerBotConfig FarmerBotConfig { get; set; }
        public DockerCompose DockerCompose { get; set; }
        public EnvFile EnvFile { get; set; }
        public bool IsError
        {
            get
            {
                return (FarmerBotConfig?.IsError ?? false) ||
                       (DockerCompose?.IsError ?? false) ||
                       (EnvFile?.IsError ?? false);
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

                if (EnvFile?.IsError ?? false)
                {
                    errorMessageBuilder.AppendLine(EnvFile.ErrorMessage);
                }

                return errorMessageBuilder.ToString();
            }
        }
    }
}