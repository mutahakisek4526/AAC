namespace AacV1.Services;

public interface ISpeechService
{
    bool IsSpeaking { get; }
    string LastError { get; }
    Task SpeakAsync(string text);
    void Stop();
}
