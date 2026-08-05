using Avalonia.Controls;
using Avalonia.Input;
using WorldSimApp.Models;
using WorldSimApp.ViewModels;

namespace WorldSimApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnCountrySelect(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is Country country)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.SelectCountry(country);
            }
        }
    }

    private void OnPlayerCountrySelect(object? sender, PointerPressedEventArgs e)
    {
        if (sender is TextBlock textBlock && textBlock.DataContext is Country country)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.SelectPlayerCountryCommand.Execute(country);
            }
        }
    }
}
