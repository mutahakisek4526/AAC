using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AacV1.Models;
using AacV1.Services;
using AacV1.ViewModels;

namespace AacV1.Views;

public partial class MainShellView : Window
{
    private readonly MainShellViewModel _viewModel;

    public MainShellView()
    {
        InitializeComponent();

        var storageService = new StorageService();
        var predictionService = new PredictionService(storageService);
        var speechService = new SpeechService();
        var computerControlService = new ComputerControlService();
        var dwellService = new DwellService();
        var scanService = new ScanService();
        var inputEngineService = new InputEngineService(dwellService, scanService);

        _viewModel = new MainShellViewModel(storageService, predictionService, speechService, computerControlService, inputEngineService);
        DataContext = _viewModel;

        Loaded += (_, _) => RefreshScanTargets();
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (_viewModel.HandleGlobalKey(e.Key))
        {
            e.Handled = true;
        }
    }

    private void OnKanaClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            _viewModel.AppendText(button.Content?.ToString() ?? string.Empty);
        }
    }

    private void OnPredictionClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            _viewModel.ApplyPrediction(button.Content?.ToString() ?? string.Empty);
        }
    }

    private void OnPhraseClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is AacPhraseItem item)
        {
            _viewModel.ApplyPhrase(item);
        }
    }

    private void OnHistoryClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            _viewModel.ApplyPrediction(button.Content?.ToString() ?? string.Empty);
        }
    }

    private void OnBackspaceClick(object sender, RoutedEventArgs e) => _viewModel.BackspaceCommand.Execute(null);
    private void OnUndoClick(object sender, RoutedEventArgs e) => _viewModel.UndoCommand.Execute(null);
    private void OnClearClick(object sender, RoutedEventArgs e) => _viewModel.ClearCommand.Execute(null);
    private void OnNewLineClick(object sender, RoutedEventArgs e) => _viewModel.NewLineCommand.Execute(null);
    private void OnCommitClick(object sender, RoutedEventArgs e) => _viewModel.CommitCommand.Execute(null);
    private void OnSpeakClick(object sender, RoutedEventArgs e) => _viewModel.SpeakCommand.Execute(null);
    private void OnStopClick(object sender, RoutedEventArgs e) => _viewModel.StopCommand.Execute(null);
    private void OnApplySettingsClick(object sender, RoutedEventArgs e) => _viewModel.ApplySettingsCommand.Execute(null);
    private void OnToggleSupporterModeClick(object sender, RoutedEventArgs e) => _viewModel.ToggleSupporterModeCommand.Execute(null);
    private void OnSupporterAddPhraseClick(object sender, RoutedEventArgs e) => _viewModel.AddSupporterPhraseCommand.Execute(null);
    private void OnSupporterDeletePhraseClick(object sender, RoutedEventArgs e) => _viewModel.DeleteSupporterPhraseCommand.Execute(null);

    private void OnOpenHistoryTabClick(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 2;
    private void OnOpenPhraseTabClick(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 1;

    private void OnPcKeyClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && Enum.TryParse<AacKeyCode>(button.Tag?.ToString(), out var keyCode))
        {
            _viewModel.PcActionKey(keyCode);
        }
    }

    private void OnPcMoveClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        var value = button.Tag?.ToString() ?? "0,0";
        var parts = value.Split(',');
        if (parts.Length == 2 && int.TryParse(parts[0], out var dx) && int.TryParse(parts[1], out var dy))
        {
            _viewModel.PcMove(dx, dy);
        }
    }

    private void OnPcLeftClick(object sender, RoutedEventArgs e)
    {
        _viewModel.PcClick();
    }

    private void OnSelectableLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is Button)
        {
            RefreshScanTargets();
        }
    }

    private void OnSelectableMouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.Tag?.ToString() == "SafeArea")
        {
            return;
        }

        _viewModel.BeginHover(button, () =>
        {
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }, progress => DwellProgressBar.Value = progress);
    }

    private void OnSelectableMouseLeave(object sender, MouseEventArgs e)
    {
        _viewModel.EndHover();
        DwellProgressBar.Value = 0;
    }

    private void OnTabChanged(object sender, SelectionChangedEventArgs e)
    {
        RefreshScanTargets();
    }

    private void RefreshScanTargets()
    {
        try
        {
            var buttons = FindVisualChildren<Button>(MainTabControl)
                .Where(button => button.IsEnabled && button.IsVisible)
                .ToList();
            _viewModel.AttachScanTargets(buttons);
        }
        catch
        {
        }
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject dependencyObject) where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
        {
            var child = VisualTreeHelper.GetChild(dependencyObject, i);
            if (child is T target)
            {
                yield return target;
            }

            foreach (var nested in FindVisualChildren<T>(child))
            {
                yield return nested;
            }
        }
    }
}
