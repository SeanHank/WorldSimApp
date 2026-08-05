using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using WorldSimApp.Services;

namespace WorldSimApp.Views;

public class ThemeResourceConverter : IValueConverter
{
    public static readonly ThemeResourceConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string? resourceKey = parameter as string;
        if (string.IsNullOrEmpty(resourceKey))
            resourceKey = value as string;
        
        if (string.IsNullOrEmpty(resourceKey))
            return ThemeService.LightTheme.Background;
        
        bool isDark = ThemeService.Instance.IsDarkTheme;
        
        return isDark ? GetDarkBrush(resourceKey) : GetLightBrush(resourceKey);
    }
    
    private static object GetDarkBrush(string key)
    {
        return key switch
        {
            "Background" or "HeaderBg" => ThemeService.DarkTheme.Background,
            "BackgroundSecondary" => ThemeService.DarkTheme.BackgroundSecondary,
            "BackgroundTertiary" => ThemeService.DarkTheme.BackgroundTertiary,
            "Surface" or "CardBg" => ThemeService.DarkTheme.Surface,
            "SurfaceVariant" or "ItemBg" => ThemeService.DarkTheme.SurfaceVariant,
            "Border" => ThemeService.DarkTheme.Border,
            "BorderStrong" => ThemeService.DarkTheme.BorderStrong,
            "TextPrimary" or "TextColor" => ThemeService.DarkTheme.TextPrimary,
            "TextSecondary" => ThemeService.DarkTheme.TextSecondary,
            "TextTertiary" or "MutedText" => ThemeService.DarkTheme.TextTertiary,
            "TextOnPrimary" or "ButtonText" => ThemeService.DarkTheme.TextOnPrimary,
            "Accent" => ThemeService.DarkTheme.Accent,
            "AccentLight" => ThemeService.DarkTheme.AccentLight,
            "Success" or "SuccessColor" => ThemeService.DarkTheme.Success,
            "SuccessBackground" => ThemeService.DarkTheme.SuccessBackground,
            "Warning" or "WarningColor" => ThemeService.DarkTheme.Warning,
            "WarningBackground" => ThemeService.DarkTheme.WarningBackground,
            "Danger" or "DangerColor" => ThemeService.DarkTheme.Danger,
            "DangerBackground" or "WarBg" => ThemeService.DarkTheme.DangerBackground,
            "Info" or "InfoColor" => ThemeService.DarkTheme.Info,
            "InfoBackground" => ThemeService.DarkTheme.InfoBackground,
            "Neutral" => ThemeService.DarkTheme.Neutral,
            "NeutralBackground" => ThemeService.DarkTheme.NeutralBackground,
            "ButtonPrimaryBg" => ThemeService.DarkTheme.ButtonPrimaryBg,
            "ButtonPrimaryHover" => ThemeService.DarkTheme.ButtonPrimaryHover,
            "ButtonPrimaryPressed" => ThemeService.DarkTheme.ButtonPrimaryPressed,
            "ButtonSecondaryBg" => ThemeService.DarkTheme.ButtonSecondaryBg,
            "ButtonSecondaryHover" => ThemeService.DarkTheme.ButtonSecondaryHover,
            "ButtonSecondaryPressed" => ThemeService.DarkTheme.ButtonSecondaryPressed,
            "ButtonNeutralBg" => ThemeService.DarkTheme.ButtonNeutralBg,
            "ButtonTextPrimary" => ThemeService.DarkTheme.ButtonTextPrimary,
            "ButtonTextSecondary" => ThemeService.DarkTheme.ButtonTextSecondary,
            "ButtonTextNeutral" => ThemeService.DarkTheme.ButtonTextNeutral,
            "Purple" or "PurpleButtonBg" => ThemeService.DarkTheme.Purple,
            "PurpleLight" or "PurpleButtonHover" => ThemeService.DarkTheme.PurpleLight,
            "Teal" => ThemeService.DarkTheme.Teal,
            "TealLight" => ThemeService.DarkTheme.TealLight,
            "SpecialBlue" or "SpecialButtonBg" => ThemeService.DarkTheme.SpecialBlue,
            "SpecialBlueLight" or "SpecialButtonHover" => ThemeService.DarkTheme.SpecialBlueLight,
            "ComboBoxBg" => ThemeService.DarkTheme.Surface,
            "ComboBoxBorder" => ThemeService.DarkTheme.Border,
            _ => Brushes.Transparent
        };
    }
    
    private static object GetLightBrush(string key)
    {
        return key switch
        {
            "Background" or "HeaderBg" => ThemeService.LightTheme.Background,
            "BackgroundSecondary" => ThemeService.LightTheme.BackgroundSecondary,
            "BackgroundTertiary" => ThemeService.LightTheme.BackgroundTertiary,
            "Surface" or "CardBg" => ThemeService.LightTheme.Surface,
            "SurfaceVariant" or "ItemBg" => ThemeService.LightTheme.SurfaceVariant,
            "Border" => ThemeService.LightTheme.Border,
            "BorderStrong" => ThemeService.LightTheme.BorderStrong,
            "TextPrimary" or "TextColor" => ThemeService.LightTheme.TextPrimary,
            "TextSecondary" => ThemeService.LightTheme.TextSecondary,
            "TextTertiary" or "MutedText" => ThemeService.LightTheme.TextTertiary,
            "TextOnPrimary" or "ButtonText" => ThemeService.LightTheme.TextOnPrimary,
            "Accent" => ThemeService.LightTheme.Accent,
            "AccentLight" => ThemeService.LightTheme.AccentLight,
            "Success" or "SuccessColor" => ThemeService.LightTheme.Success,
            "SuccessBackground" => ThemeService.LightTheme.SuccessBackground,
            "Warning" or "WarningColor" => ThemeService.LightTheme.Warning,
            "WarningBackground" => ThemeService.LightTheme.WarningBackground,
            "Danger" or "DangerColor" => ThemeService.LightTheme.Danger,
            "DangerBackground" or "WarBg" => ThemeService.LightTheme.DangerBackground,
            "Info" or "InfoColor" => ThemeService.LightTheme.Info,
            "InfoBackground" => ThemeService.LightTheme.InfoBackground,
            "Neutral" => ThemeService.LightTheme.Neutral,
            "NeutralBackground" => ThemeService.LightTheme.NeutralBackground,
            "ButtonPrimaryBg" => ThemeService.LightTheme.ButtonPrimaryBg,
            "ButtonPrimaryHover" => ThemeService.LightTheme.ButtonPrimaryHover,
            "ButtonPrimaryPressed" => ThemeService.LightTheme.ButtonPrimaryPressed,
            "ButtonSecondaryBg" => ThemeService.LightTheme.ButtonSecondaryBg,
            "ButtonSecondaryHover" => ThemeService.LightTheme.ButtonSecondaryHover,
            "ButtonSecondaryPressed" => ThemeService.LightTheme.ButtonSecondaryPressed,
            "ButtonNeutralBg" => ThemeService.LightTheme.ButtonNeutralBg,
            "ButtonTextPrimary" => ThemeService.LightTheme.ButtonTextPrimary,
            "ButtonTextSecondary" => ThemeService.LightTheme.ButtonTextSecondary,
            "ButtonTextNeutral" => ThemeService.LightTheme.ButtonTextNeutral,
            "Purple" or "PurpleButtonBg" => ThemeService.LightTheme.Purple,
            "PurpleLight" or "PurpleButtonHover" => ThemeService.LightTheme.PurpleLight,
            "Teal" => ThemeService.LightTheme.Teal,
            "TealLight" => ThemeService.LightTheme.TealLight,
            "SpecialBlue" or "SpecialButtonBg" => ThemeService.LightTheme.SpecialBlue,
            "SpecialBlueLight" or "SpecialButtonHover" => ThemeService.LightTheme.SpecialBlueLight,
            "ComboBoxBg" => ThemeService.LightTheme.Surface,
            "ComboBoxBorder" => ThemeService.LightTheme.Border,
            _ => Brushes.Transparent
        };
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SortTextConverter : IValueConverter
{
    public static readonly SortTextConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool ascending = value is bool b && b;
        return ascending ? "Oldest" : "Newest";
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
