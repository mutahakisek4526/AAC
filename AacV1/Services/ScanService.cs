using System.Windows.Threading;
using AacV1.Infrastructure;

namespace AacV1.Services;

public class ScanService : ObservableObject, IScanService
{
    private readonly DispatcherTimer _dispatcherTimer = new();
    private bool _isGroupSelection = true;
    private int _currentGroupIndex;
    private int _currentElementIndex;
    private bool _isAutoScanning;

    public ScanService()
    {
        _dispatcherTimer.Tick += (_, _) => MoveNext();
    }

    public bool IsAutoScanning
    {
        get => _isAutoScanning;
        private set => SetProperty(ref _isAutoScanning, value);
    }

    public int CurrentGroupIndex
    {
        get => _currentGroupIndex;
        private set => SetProperty(ref _currentGroupIndex, value);
    }

    public int CurrentElementIndex
    {
        get => _currentElementIndex;
        private set => SetProperty(ref _currentElementIndex, value);
    }

    public void StartAutoScan(int intervalMilliseconds)
    {
        _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Math.Max(300, intervalMilliseconds));
        IsAutoScanning = true;
        _dispatcherTimer.Start();
    }

    public void StopAutoScan()
    {
        _dispatcherTimer.Stop();
        IsAutoScanning = false;
    }

    public void MoveNextManual()
    {
        MoveNext();
    }

    public void ConfirmSelection()
    {
        if (_isGroupSelection)
        {
            _isGroupSelection = false;
            CurrentElementIndex = 0;
        }
        else
        {
            _isGroupSelection = true;
            CurrentGroupIndex = 0;
            CurrentElementIndex = 0;
        }
    }

    private void MoveNext()
    {
        if (_isGroupSelection)
        {
            CurrentGroupIndex = (CurrentGroupIndex + 1) % 10;
        }
        else
        {
            CurrentElementIndex = (CurrentElementIndex + 1) % 10;
        }
    }
}
