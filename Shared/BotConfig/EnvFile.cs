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

        public void Deserialize(string text)
        {
            var lines = text.Split('\n');
            try
            {
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
                            Mnemonic = value;
                            break;
                        case "NETWORK":
                            Network = value;
                            break;
                        case "RELAY":
                            Relay = value;
                            break;
                        case "SUBSTRATE":
                            Substrate = value;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                IsError = true;
                ErrorMessage = ex.Message;
            }
        }

        public string Serialize()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("MNEMONIC=" + Mnemonic);
                sb.AppendLine("NETWORK=" + Network);
                sb.AppendLine("RELAY=" + Relay);
                sb.AppendLine("SUBSTRATE=" + Substrate);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                IsError = true;
                ErrorMessage = ex.Message;
                return ex.Message;
            }
        }
    }
}