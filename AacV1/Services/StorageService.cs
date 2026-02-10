using System.Text.Json;
using AacV1.Models;

namespace AacV1.Services;

public class StorageService : IStorageService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };
    private readonly string _baseDirectory;

    public StorageService()
    {
        _baseDirectory = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AacV1");
        EnsureDirectory();
    }

    public AacAppSettings LoadSettings() => LoadFromFile("settings.json", new AacAppSettings());

    public void SaveSettings(AacAppSettings appSettings) => SaveToFile("settings.json", appSettings);

    public List<AacPhraseItem> LoadPhrases() => LoadFromFile("phrases.json", new List<AacPhraseItem>());

    public void SavePhrases(List<AacPhraseItem> phraseItems) => SaveToFile("phrases.json", phraseItems);

    public List<AacHistoryItem> LoadHistory() => LoadFromFile("history.json", new List<AacHistoryItem>());

    public void SaveHistory(List<AacHistoryItem> historyItems) => SaveToFile("history.json", historyItems);

    public Dictionary<string, List<string>> LoadPredictionDictionary() =>
        LoadFromFile("predictions.json", new Dictionary<string, List<string>>());

    public void SavePredictionDictionary(Dictionary<string, List<string>> predictionDictionary) =>
        SaveToFile("predictions.json", predictionDictionary);

    public List<AacEnvironmentAction> LoadEnvironmentActions() =>
        LoadFromFile("environment_actions.json", new List<AacEnvironmentAction>());

    public void SaveEnvironmentActions(List<AacEnvironmentAction> actions) =>
        SaveToFile("environment_actions.json", actions);

    private T LoadFromFile<T>(string fileName, T fallback)
    {
        try
        {
            var targetPath = System.IO.Path.Combine(_baseDirectory, fileName);
            if (!System.IO.File.Exists(targetPath))
            {
                return fallback;
            }

            var jsonText = System.IO.File.ReadAllText(targetPath);
            var data = JsonSerializer.Deserialize<T>(jsonText, _jsonSerializerOptions);
            return data ?? fallback;
        }
        catch
        {
            return fallback;
        }
    }

    private void SaveToFile<T>(string fileName, T data)
    {
        try
        {
            EnsureDirectory();
            var targetPath = System.IO.Path.Combine(_baseDirectory, fileName);
            var jsonText = JsonSerializer.Serialize(data, _jsonSerializerOptions);
            System.IO.File.WriteAllText(targetPath, jsonText);
        }
        catch
        {
        }
    }

    private void EnsureDirectory()
    {
        try
        {
            System.IO.Directory.CreateDirectory(_baseDirectory);
        }
        catch
        {
        }
    }
}
