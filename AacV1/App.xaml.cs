using System.Windows;

namespace AacV1;

public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += (_, e) =>
        {
            e.Handled = true;
            MessageBox.Show($"予期しないエラーを回復しました: {e.Exception.Message}", "AacV1", MessageBoxButton.OK, MessageBoxImage.Warning);
        };
    }
}
