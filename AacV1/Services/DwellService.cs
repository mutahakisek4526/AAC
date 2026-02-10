using System.Windows.Threading;

namespace AacV1.Services;

public class DwellService
{
    private readonly DispatcherTimer _timer;
    private DateTime _hoverStart;
    private DateTime _lastTriggerAt = DateTime.MinValue;
    private Action? _triggerAction;
    private Action<double>? _progressCallback;
    private Action? _focusOnlyAction;

    public DwellService()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40) };
        _timer.Tick += OnTick;
    }

    public int DwellMilliseconds { get; set; } = 800;
    public int CooldownMilliseconds { get; set; } = 300;
    public int CancelGraceMilliseconds { get; set; } = 200;
    public bool IsFocusOnlyMode { get; set; }
    public bool IsEnabled { get; set; }

    public void BeginHover(Action triggerAction, Action focusOnlyAction, Action<double> progressCallback)
    {
        if (!IsEnabled)
        {
            return;
        }

        _hoverStart = DateTime.Now;
        _triggerAction = triggerAction;
        _focusOnlyAction = focusOnlyAction;
        _progressCallback = progressCallback;
        _timer.Start();
    }

    public void EndHover()
    {
        _timer.Stop();
        _progressCallback?.Invoke(0);
    }

    private void OnTick(object? sender, EventArgs e)
    {
        try
        {
            var elapsed = (DateTime.Now - _hoverStart).TotalMilliseconds;
            var ratio = Math.Min(1.0, elapsed / Math.Max(1, DwellMilliseconds));
            _progressCallback?.Invoke(ratio);

            if (elapsed < CancelGraceMilliseconds)
            {
                return;
            }

            if (elapsed >= DwellMilliseconds && (DateTime.Now - _lastTriggerAt).TotalMilliseconds >= CooldownMilliseconds)
            {
                _lastTriggerAt = DateTime.Now;
                _timer.Stop();
                _progressCallback?.Invoke(0);
                if (IsFocusOnlyMode)
                {
                    _focusOnlyAction?.Invoke();
                }
                else
                {
                    _triggerAction?.Invoke();
                }
            }
        }
        catch
        {
            _timer.Stop();
        }
    }
}
