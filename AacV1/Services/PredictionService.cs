using System.Text.RegularExpressions;

namespace AacV1.Services;

public class PredictionService : IPredictionService
{
    private readonly IStorageService _storageService;
    private readonly Dictionary<string, List<string>> _dictionary;

    public PredictionService(IStorageService storageService)
    {
        _storageService = storageService;
        _dictionary = _storageService.LoadPredictionDictionary();
    }

    public List<string> GetPredictions(string sourceText)
    {
        var key = sourceText.Trim();
        if (_dictionary.TryGetValue(key, out var candidates))
        {
            return candidates.Take(8).ToList();
        }

        return _dictionary.Keys
            .Where(word => word.StartsWith(key, StringComparison.Ordinal))
            .OrderBy(word => word)
            .Take(8)
            .ToList();
    }

    public void LearnWord(string sourceText)
    {
        var tokens = Regex.Split(sourceText, @"[\u3000\s,、。．.!！？?]+")
            .Where(token => !string.IsNullOrWhiteSpace(token));

        foreach (var token in tokens)
        {
            if (!_dictionary.ContainsKey(token))
            {
                _dictionary[token] = new List<string>();
            }
        }

        _storageService.SavePredictionDictionary(_dictionary);
    }
}
