using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerbotWebUI.Shared.BotConfig
{
    public class EnvFile
    {
        public string Network { get; set; }
        public string Relay { get; set; }
        public string Substrate { get; set; }
        public string Mnemonic { get; set; }
        public bool IsError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

        public EnvFile DeserializeEnvFile(string text)
        {
            EnvFile env = new EnvFile();
            var lines = text.Split('\n');

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split('=');
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                switch (key)
                {
                    case "MNEMONIC":
                        env.Mnemonic = value;
                        break;
                    case "NETWORK":
                        env.Network = value;
                        break;
                    case "RELAY":
                        env.Relay = value;
                        break;
                    case "SUBSTRATE":
                        env.Substrate = value;
                        break;
                }
            }
            return env;
        }

        public string SerializeEnvFile()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MNEMONIC=" + Mnemonic);
            sb.AppendLine("NETWORK=" + Network);
            sb.AppendLine("RELAY=" + Relay);
            sb.AppendLine("SUBSTRATE=" + Substrate);
            return sb.ToString();
        }
    }
}