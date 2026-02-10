using System.Windows.Controls;

namespace AacV1.Services;

public interface IScanService
{
    bool IsRunning { get; }
    int HighlightIndex { get; }
    void ConfigureTargets(IReadOnlyList<Button> buttons);
    void Start(int intervalMilliseconds, bool rowColumnMode);
    void Stop();
    void MoveNext();
    void ActivateCurrent();
}
