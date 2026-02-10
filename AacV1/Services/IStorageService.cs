using AacV1.Models;

namespace AacV1.Services;

public interface IStorageService
{
    AacAppSettings LoadSettings();
    void SaveSettings(AacAppSettings appSettings);
    List<AacPhraseItem> LoadPhrases();
    void SavePhrases(List<AacPhraseItem> items);
    List<AacHistoryItem> LoadHistory();
    void SaveHistory(List<AacHistoryItem> items);
    AacPredictionDictionary LoadPredictionDictionary();
    void SavePredictionDictionary(AacPredictionDictionary dictionary);
}
