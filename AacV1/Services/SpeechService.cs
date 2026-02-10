using System.Speech.Synthesis;

namespace AacV1.Services;

public class SpeechService : ISpeechService
{
    private readonly SpeechSynthesizer _synthesizer = new();
    private readonly object _lockObject = new();

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
            lock (_lockObject)
            {
                try
                {
                    LastError = string.Empty;
                    IsSpeaking = true;
                    _synthesizer.Speak(text);
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
        lock (_lockObject)
        {
            try
            {
                _synthesizer.SpeakAsyncCancelAll();
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
