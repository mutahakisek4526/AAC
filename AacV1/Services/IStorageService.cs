using AacV1.Models;

namespace AacV1.Services;

public interface IStorageService
{
    AacAppSettings LoadSettings();
    void SaveSettings(AacAppSettings appSettings);
    List<AacPhraseItem> LoadPhrases();
    void SavePhrases(List<AacPhraseItem> phraseItems);
    List<AacHistoryItem> LoadHistory();
    void SaveHistory(List<AacHistoryItem> historyItems);
    Dictionary<string, List<string>> LoadPredictionDictionary();
    void SavePredictionDictionary(Dictionary<string, List<string>> predictionDictionary);
    List<AacEnvironmentAction> LoadEnvironmentActions();
    void SaveEnvironmentActions(List<AacEnvironmentAction> actions);
}
