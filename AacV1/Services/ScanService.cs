using System.Windows.Controls;
using System.Windows.Threading;

namespace AacV1.Services;

public class ScanService : IScanService
{
    private readonly DispatcherTimer _timer = new();
    private readonly List<Button> _targets = new();
    private bool _rowColumnMode;
    private int _currentRow = -1;
    private bool _rowPhase = true;

    public ScanService()
    {
        _timer.Tick += (_, _) => MoveNext();
    }

    public bool IsRunning { get; private set; }
    public int HighlightIndex { get; private set; } = -1;

    public void ConfigureTargets(IReadOnlyList<Button> buttons)
    {
        _targets.Clear();
        _targets.AddRange(buttons.Where(b => b.IsEnabled && b.IsVisible));
        HighlightIndex = _targets.Count > 0 ? 0 : -1;
        RefreshHighlight();
    }

    public void Start(int intervalMilliseconds, bool rowColumnMode)
    {
        _rowColumnMode = rowColumnMode;
        _timer.Interval = TimeSpan.FromMilliseconds(Math.Max(250, intervalMilliseconds));
        _rowPhase = true;
        _currentRow = -1;
        IsRunning = true;
        _timer.Start();
    }

    public void Stop()
    {
        IsRunning = false;
        _timer.Stop();
        ClearHighlight();
    }

    public void MoveNext()
    {
        if (_targets.Count == 0)
        {
            return;
        }

        if (_rowColumnMode)
        {
            MoveNextRowColumn();
        }
        else
        {
            HighlightIndex = (HighlightIndex + 1) % _targets.Count;
            RefreshHighlight();
        }
    }

    public void ActivateCurrent()
    {
        if (_targets.Count == 0 || HighlightIndex < 0)
        {
            return;
        }

        if (_rowColumnMode && _rowPhase)
        {
            _rowPhase = false;
            return;
        }

        try
        {
            _targets[HighlightIndex].RaiseEvent(new System.Windows.RoutedEventArgs(Button.ClickEvent));
        }
        catch
        {
        }
        finally
        {
            _rowPhase = true;
        }
    }

    private void MoveNextRowColumn()
    {
        const int rowSize = 8;
        var rowCount = (int)Math.Ceiling(_targets.Count / (double)rowSize);

        if (_rowPhase)
        {
            _currentRow = (_currentRow + 1) % rowCount;
            HighlightIndex = _currentRow * rowSize;
        }
        else
        {
            var rowStart = _currentRow * rowSize;
            var rowEnd = Math.Min(_targets.Count, rowStart + rowSize);
            HighlightIndex++;
            if (HighlightIndex < rowStart || HighlightIndex >= rowEnd)
            {
                HighlightIndex = rowStart;
            }
        }

        RefreshHighlight();
    }

    private void RefreshHighlight()
    {
        ClearHighlight();
        if (HighlightIndex >= 0 && HighlightIndex < _targets.Count)
        {
            _targets[HighlightIndex].BorderThickness = new System.Windows.Thickness(4);
            _targets[HighlightIndex].BorderBrush = System.Windows.Media.Brushes.OrangeRed;
        }
    }

    private void ClearHighlight()
    {
        foreach (var target in _targets)
        {
            target.ClearValue(Button.BorderThicknessProperty);
            target.ClearValue(Button.BorderBrushProperty);
        }
    }
}
