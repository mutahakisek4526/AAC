namespace AacV1.Services;

public interface IScanService
{
    bool IsAutoScanning { get; }
    int CurrentGroupIndex { get; }
    int CurrentElementIndex { get; }
    void StartAutoScan(int intervalMilliseconds);
    void StopAutoScan();
    void MoveNextManual();
    void ConfirmSelection();
}
