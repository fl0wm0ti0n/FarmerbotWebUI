﻿using Microsoft.Extensions.Logging;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radzen;

namespace FarmerbotWebUI.Shared
{
    public static class LogLevelStyleMapper
    {
        public static LogLevel StyleToLogLevel(this Style progressBarStyle)
        {
            return progressBarStyle switch
            {
                Style.Light => LogLevel.Trace,
                Style.Secondary => LogLevel.Debug,
                Style.Info => LogLevel.Information,
                Style.Warning => LogLevel.Warning,
                Style.Danger => LogLevel.Error,
                Style.Primary => LogLevel.Critical,
                Style.Success => LogLevel.None,
                Style.Dark => LogLevel.None,
                _ => LogLevel.None,
            };
        }

        public static Style LogLevelToStyle(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => Style.Light,
                LogLevel.Debug => Style.Secondary,
                LogLevel.Information => Style.Info,
                LogLevel.Warning => Style.Warning,
                LogLevel.Error => Style.Danger,
                LogLevel.Critical => Style.Primary,
                _ => Style.Info,
            };
        }
        public static LogLevel ProgressBarStyleToLogLevel(ProgressBarStyle progressBarStyle)
        {
            return progressBarStyle switch
            {
                ProgressBarStyle.Light => LogLevel.Trace,
                ProgressBarStyle.Secondary => LogLevel.Debug,
                ProgressBarStyle.Info => LogLevel.Information,
                ProgressBarStyle.Warning => LogLevel.Warning,
                ProgressBarStyle.Danger => LogLevel.Error,
                ProgressBarStyle.Primary => LogLevel.Critical,
                ProgressBarStyle.Success => LogLevel.None,
                ProgressBarStyle.Dark => LogLevel.None,
                _ => LogLevel.None,
            };
        }

        public static ProgressBarStyle LogLevelToProgressBarStyle(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => ProgressBarStyle.Light,
                LogLevel.Debug => ProgressBarStyle.Secondary,
                LogLevel.Information => ProgressBarStyle.Info,
                LogLevel.Warning => ProgressBarStyle.Warning,
                LogLevel.Error => ProgressBarStyle.Danger,
                LogLevel.Critical => ProgressBarStyle.Primary,
                _ => ProgressBarStyle.Info,
            };
        }

        public static LogLevel BadgeStyleToLogLevel(BadgeStyle badgeStyle)
        {
            return badgeStyle switch
            {
                BadgeStyle.Light => LogLevel.Trace,
                BadgeStyle.Secondary => LogLevel.Debug,
                BadgeStyle.Info => LogLevel.Information,
                BadgeStyle.Warning => LogLevel.Warning,
                BadgeStyle.Danger => LogLevel.Error,
                BadgeStyle.Primary => LogLevel.Critical,
                BadgeStyle.Success => LogLevel.None,
                BadgeStyle.Dark => LogLevel.None,
                _ => LogLevel.None,
            };
        }

        public static BadgeStyle LogLevelToBadgeStyle(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => BadgeStyle.Light,
                LogLevel.Debug => BadgeStyle.Secondary,
                LogLevel.Information => BadgeStyle.Info,
                LogLevel.Warning => BadgeStyle.Warning,
                LogLevel.Error => BadgeStyle.Danger,
                LogLevel.Critical => BadgeStyle.Primary,
                _ => BadgeStyle.Info,
            };
        }

        public static LogLevel RadzenCssToLogLevel(string css)
        {
            return css switch
            {
                "rz-background-color-series-8" => LogLevel.Trace,
                "rz-background-color-secondary-dark" => LogLevel.Debug,
                "rz-background-color-info-dark" => LogLevel.Information,
                "rz-background-color-warning-dark" => LogLevel.Warning,
                "rz-background-color-danger-dark" => LogLevel.Error,
                "rz-background-color-primary-dark" => LogLevel.Critical,
                _ => LogLevel.None,
            };
        }

        public static string LogLevelToRadzenCss(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => "rz-background-color-series-8",
                LogLevel.Debug => "rz-background-color-secondary-dark",
                LogLevel.Information => "rz-background-color-info-dark",
                LogLevel.Warning => "rz-background-color-warning-dark",
                LogLevel.Error => "rz-background-color-danger-dark",
                LogLevel.Critical => "rz-background-color-primary-dark",
                _ => "rz-background-color-white",
            };
        }

        public static LogLevel NotificationSeverityToLogLevel(NotificationSeverity badgeStyle)
        {
            return badgeStyle switch
            {
                NotificationSeverity.Success => LogLevel.Trace,
                NotificationSeverity.Info => LogLevel.Information,
                NotificationSeverity.Warning => LogLevel.Warning,
                NotificationSeverity.Error => LogLevel.Error,
                _ => LogLevel.None,
            };
        }

        public static NotificationSeverity LogLevelToNotificationSeverity(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => NotificationSeverity.Success,
                LogLevel.Information => NotificationSeverity.Info,
                LogLevel.Warning => NotificationSeverity.Warning,
                LogLevel.Error => NotificationSeverity.Error,
                LogLevel.Critical => NotificationSeverity.Error,
                LogLevel.Debug => NotificationSeverity.Info,
                LogLevel.None => NotificationSeverity.Info,
                _ => NotificationSeverity.Info,
            };
        }

    }

    public enum Style
    {
        Primary,
        Secondary,
        Light,
        Dark,
        Success,
        Danger,
        Warning,
        Info
    }
}
