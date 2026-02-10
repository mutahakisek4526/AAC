using System.Windows.Controls;
using System.Windows.Input;
using AacV1.Models;

namespace AacV1.Services;

public class InputEngineService
{
    private readonly DwellService _dwellService;
    private readonly IScanService _scanService;
    private Button? _focusedButton;

    public InputEngineService(DwellService dwellService, IScanService scanService)
    {
        _dwellService = dwellService;
        _scanService = scanService;
    }

    public AacInputMode CurrentInputMode { get; set; } = AacInputMode.通常操作;

    public void ApplySettings(AacAppSettings settings)
    {
        CurrentInputMode = settings.InputMode;
        _dwellService.DwellMilliseconds = settings.DwellMilliseconds;
        _dwellService.CooldownMilliseconds = settings.DwellCooldownMilliseconds;
        _dwellService.CancelGraceMilliseconds = settings.DwellCancelGraceMilliseconds;
        _dwellService.IsEnabled = settings.InputMode is AacInputMode.視線のみ or AacInputMode.視線プラススイッチ;
        _dwellService.IsFocusOnlyMode = settings.InputMode == AacInputMode.視線プラススイッチ;

        if (settings.InputMode == AacInputMode.スイッチのみ)
        {
            _scanService.Start(settings.ScanIntervalMilliseconds, settings.UseRowColumnScan);
        }
        else
        {
            _scanService.Stop();
        }
    }

    public void SetScanTargets(IReadOnlyList<Button> buttons)
    {
        _scanService.ConfigureTargets(buttons);
    }

    public void BeginHover(Button button, Action clickAction, Action<double> progressAction)
    {
        _dwellService.BeginHover(
            () => clickAction(),
            () => _focusedButton = button,
            progressAction);
    }

    public void EndHover()
    {
        _dwellService.EndHover();
    }

    public bool HandleKeyDown(Key inputKey, Action backAction)
    {
        try
        {
            if (CurrentInputMode == AacInputMode.スイッチのみ)
            {
                if (inputKey == Key.Tab)
                {
                    _scanService.MoveNext();
                    return true;
                }

                if (inputKey == Key.Enter)
                {
                    _scanService.ActivateCurrent();
                    return true;
                }

                if (inputKey == Key.Space)
                {
                    if (_scanService.IsRunning)
                    {
                        _scanService.Stop();
                    }
                    else
                    {
                        _scanService.Start(900, false);
                    }

                    return true;
                }
            }

            if (CurrentInputMode == AacInputMode.視線プラススイッチ && inputKey == Key.Enter)
            {
                _focusedButton?.RaiseEvent(new System.Windows.RoutedEventArgs(Button.ClickEvent));
                return true;
            }

            if (inputKey == Key.Escape)
            {
                backAction();
                return true;
            }
        }
        catch
        {
        }

        return false;
    }
}
