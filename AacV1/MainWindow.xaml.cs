using System.Windows;
using System.Windows.Input;
using AacV1.ViewModels;

namespace AacV1;

public partial class MainWindow : Window
{
    private readonly MainViewModel _mainViewModel;

    public MainWindow()
    {
        InitializeComponent();
        _mainViewModel = new MainViewModel();
        DataContext = _mainViewModel;
        PreviewKeyDown += OnPreviewKeyDown;
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (_mainViewModel.ProcessKey(e.Key))
        {
            e.Handled = true;
        }
    }
}
