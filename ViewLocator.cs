using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using WorldSimApp.ViewModels;
using WorldSimApp.Views;

namespace WorldSimApp;

public class ViewLocator : IDataTemplate
{
    private static readonly Dictionary<System.Type, System.Func<Control>> ViewFactory = new()
    {
        [typeof(MainWindowViewModel)] = () => new MainWindow()
    };

    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        if (ViewFactory.TryGetValue(param.GetType(), out var factory))
        {
            return factory();
        }

        return new TextBlock { Text = "Not Found: " + param.GetType().Name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
