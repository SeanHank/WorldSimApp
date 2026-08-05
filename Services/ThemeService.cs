using System;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WorldSimApp.Services;

public partial class ThemeService : ObservableObject
{
    private static ThemeService? _instance;
    public static ThemeService Instance => _instance ??= new ThemeService();

    [ObservableProperty] public bool _isDarkTheme;

    public ThemeService()
    {
        _instance = this;
    }

    public void SetTheme(bool isDark)
    {
        IsDarkTheme = isDark;
        Application.Current!.RequestedThemeVariant = isDark 
            ? ThemeVariant.Dark 
            : ThemeVariant.Light;
    }

    public void ToggleTheme()
    {
        SetTheme(!IsDarkTheme);
    }

    public static class LightTheme
    {
        public static SolidColorBrush Background => new(Color.Parse("#FFFFFF"));
        public static SolidColorBrush BackgroundSecondary => new(Color.Parse("#F3F3F3"));
        public static SolidColorBrush BackgroundTertiary => new(Color.Parse("#F5F5F5"));
        public static SolidColorBrush Surface => new(Color.Parse("#FFFFFF"));
        public static SolidColorBrush SurfaceVariant => new(Color.Parse("#F5F5F5"));
        public static SolidColorBrush Border => new(Color.Parse("#E0E0E0"));
        public static SolidColorBrush BorderStrong => new(Color.Parse("#D1D1D1"));
        
        public static SolidColorBrush TextPrimary => new(Color.Parse("#1A1A1A"));
        public static SolidColorBrush TextSecondary => new(Color.Parse("#616161"));
        public static SolidColorBrush TextTertiary => new(Color.Parse("#9E9E9E"));
        public static SolidColorBrush TextOnPrimary => new(Color.Parse("#FFFFFF"));
        
        public static SolidColorBrush Accent => new(Color.Parse("#0078D4"));
        public static SolidColorBrush AccentLight => new(Color.Parse("#429CE3"));
        
        public static SolidColorBrush Success => new(Color.Parse("#107C10"));
        public static SolidColorBrush SuccessBackground => new(Color.Parse("#DFF6DD"));
        public static SolidColorBrush Warning => new(Color.Parse("#FF8C00"));
        public static SolidColorBrush WarningBackground => new(Color.Parse("#FFF4CE"));
        public static SolidColorBrush Danger => new(Color.Parse("#D13438"));
        public static SolidColorBrush DangerBackground => new(Color.Parse("#FDE7E9"));
        public static SolidColorBrush Info => new(Color.Parse("#0078D4"));
        public static SolidColorBrush InfoBackground => new(Color.Parse("#E1F3FB"));
        public static SolidColorBrush Neutral => new(Color.Parse("#797979"));
        public static SolidColorBrush NeutralBackground => new(Color.Parse("#F0F0F0"));
        
        public static SolidColorBrush ButtonPrimaryBg => new(Color.Parse("#0078D4"));
        public static SolidColorBrush ButtonPrimaryHover => new(Color.Parse("#006CBE"));
        public static SolidColorBrush ButtonPrimaryPressed => new(Color.Parse("#005A9E"));
        public static SolidColorBrush ButtonSecondaryBg => new(Color.Parse("#F3F3F3"));
        public static SolidColorBrush ButtonSecondaryHover => new(Color.Parse("#E5E5E5"));
        public static SolidColorBrush ButtonSecondaryPressed => new(Color.Parse("#D1D1D1"));
        public static SolidColorBrush ButtonNeutralBg => new(Color.Parse("#3D3D3D"));
        public static SolidColorBrush ButtonNeutralHover => new(Color.Parse("#4D4D4D"));
        public static SolidColorBrush ButtonNeutralPressed => new(Color.Parse("#5D5D5D"));
        public static SolidColorBrush ButtonTextPrimary => new(Color.Parse("#FFFFFF"));
        public static SolidColorBrush ButtonTextSecondary => new(Color.Parse("#1A1A1A"));
        public static SolidColorBrush ButtonTextNeutral => new(Color.Parse("#FFFFFF"));
        
        public static SolidColorBrush Purple => new(Color.Parse("#881798"));
        public static SolidColorBrush PurpleLight => new(Color.Parse("#9B4DCA"));
        public static SolidColorBrush Teal => new(Color.Parse("#008272"));
        public static SolidColorBrush TealLight => new(Color.Parse("#00A386"));
        public static SolidColorBrush SpecialBlue => new(Color.Parse("#4361EE"));
        public static SolidColorBrush SpecialBlueLight => new(Color.Parse("#5A7AFF"));
    }

    public static class DarkTheme
    {
        public static SolidColorBrush Background => new(Color.Parse("#1F1F1F"));
        public static SolidColorBrush BackgroundSecondary => new(Color.Parse("#2D2D2D"));
        public static SolidColorBrush BackgroundTertiary => new(Color.Parse("#383838"));
        public static SolidColorBrush Surface => new(Color.Parse("#252525"));
        public static SolidColorBrush SurfaceVariant => new(Color.Parse("#333333"));
        public static SolidColorBrush Border => new(Color.Parse("#3D3D3D"));
        public static SolidColorBrush BorderStrong => new(Color.Parse("#4D4D4D"));
        
        public static SolidColorBrush TextPrimary => new(Color.Parse("#FFFFFF"));
        public static SolidColorBrush TextSecondary => new(Color.Parse("#B3B3B3"));
        public static SolidColorBrush TextTertiary => new(Color.Parse("#808080"));
        public static SolidColorBrush TextOnPrimary => new(Color.Parse("#FFFFFF"));
        
        public static SolidColorBrush Accent => new(Color.Parse("#60CDFF"));
        public static SolidColorBrush AccentLight => new(Color.Parse("#8ED6FF"));
        
        public static SolidColorBrush Success => new(Color.Parse("#6CCB5F"));
        public static SolidColorBrush SuccessBackground => new(Color.Parse("#1F3D1F"));
        public static SolidColorBrush Warning => new(Color.Parse("#FCE100"));
        public static SolidColorBrush WarningBackground => new(Color.Parse("#433D1F"));
        public static SolidColorBrush Danger => new(Color.Parse("#FF6B6B"));
        public static SolidColorBrush DangerBackground => new(Color.Parse("#3D1F1F"));
        public static SolidColorBrush Info => new(Color.Parse("#60CDFF"));
        public static SolidColorBrush InfoBackground => new(Color.Parse("#1F3442"));
        public static SolidColorBrush Neutral => new(Color.Parse("#A0A0A0"));
        public static SolidColorBrush NeutralBackground => new(Color.Parse("#2D2D2D"));
        
        public static SolidColorBrush ButtonPrimaryBg => new(Color.Parse("#60CDFF"));
        public static SolidColorBrush ButtonPrimaryHover => new(Color.Parse("#4DC3FF"));
        public static SolidColorBrush ButtonPrimaryPressed => new(Color.Parse("#3BB8FF"));
        public static SolidColorBrush ButtonSecondaryBg => new(Color.Parse("#3D3D3D"));
        public static SolidColorBrush ButtonSecondaryHover => new(Color.Parse("#4D4D4D"));
        public static SolidColorBrush ButtonSecondaryPressed => new(Color.Parse("#5D5D5D"));
        public static SolidColorBrush ButtonNeutralBg => new(Color.Parse("#E5E5E5"));
        public static SolidColorBrush ButtonNeutralHover => new(Color.Parse("#D1D1D1"));
        public static SolidColorBrush ButtonNeutralPressed => new(Color.Parse("#C0C0C0"));
        public static SolidColorBrush ButtonTextPrimary => new(Color.Parse("#1A1A1A"));
        public static SolidColorBrush ButtonTextSecondary => new(Color.Parse("#FFFFFF"));
        public static SolidColorBrush ButtonTextNeutral => new(Color.Parse("#1A1A1A"));
        
        public static SolidColorBrush Purple => new(Color.Parse("#B4009E"));
        public static SolidColorBrush PurpleLight => new(Color.Parse("#D53FD6"));
        public static SolidColorBrush Teal => new(Color.Parse("#00B294"));
        public static SolidColorBrush TealLight => new(Color.Parse("#33C9B0"));
        public static SolidColorBrush SpecialBlue => new(Color.Parse("#6B8AFF"));
        public static SolidColorBrush SpecialBlueLight => new(Color.Parse("#8DA3FF"));
    }
}
