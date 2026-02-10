namespace AacV1.Models;

public class AacAppSettings
{
    public double FontSize { get; set; } = 24;
    public bool HighContrast { get; set; }
    public int ScanIntervalMilliseconds { get; set; } = 1200;
    public AacInputMode InputMode { get; set; } = AacInputMode.文字入力;
    public AacOperationMode OperationMode { get; set; } = AacOperationMode.AAC;
}
