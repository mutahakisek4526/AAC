using System.Speech.Synthesis;

namespace AacV1.Services;

public class SpeechService : ISpeechService
{
    private readonly object _speechLock = new();
    private SpeechSynthesizer? _speechSynthesizer;

    public bool IsSpeaking { get; private set; }

    public string LastError { get; private set; } = string.Empty;

    public async Task SpeakAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        await Task.Run(() =>
        {
            lock (_speechLock)
            {
                try
                {
                    LastError = string.Empty;
                    _speechSynthesizer ??= new SpeechSynthesizer();
                    IsSpeaking = true;
                    _speechSynthesizer.Speak(text);
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                }
                finally
                {
                    IsSpeaking = false;
                }
            }
        });
    }

    public void Stop()
    {
        lock (_speechLock)
        {
            try
            {
                _speechSynthesizer?.SpeakAsyncCancelAll();
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
            finally
            {
                IsSpeaking = false;
            }
        }
    }
}
