namespace AacV1.Models;

public class AacAppSettings
{
    public double FontSize { get; set; } = 30;
    public bool HighContrast { get; set; }
    public AacInputMode InputMode { get; set; } = AacInputMode.通常操作;
    public int DwellMilliseconds { get; set; } = 800;
    public int DwellCooldownMilliseconds { get; set; } = 300;
    public int DwellCancelGraceMilliseconds { get; set; } = 200;
    public int ScanIntervalMilliseconds { get; set; } = 1000;
    public bool UseTwoSwitchMode { get; set; }
    public bool UseRowColumnScan { get; set; }
    public int MouseStep { get; set; } = 30;
    public int MouseDelayMilliseconds { get; set; } = 40;
    public bool EnableSafeArea { get; set; }
}
