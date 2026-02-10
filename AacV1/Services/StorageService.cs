using System.IO;
using System.Text.Json;
using AacV1.Models;

namespace AacV1.Services;

public class StorageService : IStorageService
{
    private readonly string _basePath;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
    public string LastError { get; private set; } = string.Empty;

    public StorageService()
    {
        _basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AacV1");
        EnsureFolder();
    }

    public AacAppSettings LoadSettings() => Load("settings.json", new AacAppSettings());
    public void SaveSettings(AacAppSettings appSettings) => Save("settings.json", appSettings);
    public List<AacPhraseItem> LoadPhrases() => Load("phrases.json", GetDefaultPhrases());
    public void SavePhrases(List<AacPhraseItem> items) => Save("phrases.json", items);
    public List<AacHistoryItem> LoadHistory() => Load("history.json", new List<AacHistoryItem>());
    public void SaveHistory(List<AacHistoryItem> items) => Save("history.json", items.Take(200).ToList());
    public AacPredictionDictionary LoadPredictionDictionary() => Load("predictions.json", new AacPredictionDictionary());
    public void SavePredictionDictionary(AacPredictionDictionary dictionary) => Save("predictions.json", dictionary);

    private T Load<T>(string fileName, T fallback)
    {
        try
        {
            EnsureFolder();
            var path = Path.Combine(_basePath, fileName);
            if (!File.Exists(path))
            {
                return fallback;
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json, _options) ?? fallback;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return fallback;
        }
    }

    private void Save<T>(string fileName, T value)
    {
        try
        {
            EnsureFolder();
            var path = Path.Combine(_basePath, fileName);
            File.WriteAllText(path, JsonSerializer.Serialize(value, _options));
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
        }
    }

    private void EnsureFolder()
    {
        try
        {
            Directory.CreateDirectory(_basePath);
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
        }
    }

    private static List<AacPhraseItem> GetDefaultPhrases()
    {
        return new List<AacPhraseItem>
        {
            new() { Category = "日常", Text = "ありがとうございます" },
            new() { Category = "日常", Text = "おはようございます" },
            new() { Category = "介助", Text = "水をください" },
            new() { Category = "介助", Text = "体位を変えてください" }
        };
    }
}
