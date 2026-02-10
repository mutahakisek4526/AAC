using AacV1.Models;

namespace AacV1.Services;

public interface IPredictionService
{
    List<string> GetPredictions(string inputText, IReadOnlyCollection<AacHistoryItem> historyItems, IReadOnlyCollection<AacPhraseItem> phraseItems);
    void LearnFromText(string sourceText);
}
