namespace AacV1.Services;

public interface IPredictionService
{
    List<string> GetPredictions(string sourceText);
    void LearnWord(string sourceText);
}
