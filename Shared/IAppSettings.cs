﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using FarmerBotWebUI.Shared;
//
//    var appsettings = Appsettings.FromJson(jsonString);

namespace FarmerBotWebUI.Shared
{

    public interface IAppSettings
    {
        string AllowedHosts { get; set; } 
        GeneralSettings GeneralSettings { get; set; }
        DockerSettings DockerSettings { get; set; }
        FarmerBotSettings FarmerBotSettings { get; set; }
        ThreefoldFarmSettings ThreefoldFarmSettings { get; set; }
        ThreefoldApiSettings ThreefoldApiSettings { get; set; }
        SecuritySettings SecuritySettings { get; set; }
        NotificationSettings NotificationSettings { get; set; }
        event EventHandler<AppSettings> OnAppSettingsChanged;
        void InvokeOnAppSettingsChanged(AppSettings appSettings);
    }
}
