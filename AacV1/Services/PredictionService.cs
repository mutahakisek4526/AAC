using System.Text.RegularExpressions;
using AacV1.Models;

namespace AacV1.Services;

public class PredictionService : IPredictionService
{
    private readonly IStorageService _storageService;
    private readonly AacPredictionDictionary _dictionary;

    public PredictionService(IStorageService storageService)
    {
        _storageService = storageService;
        _dictionary = _storageService.LoadPredictionDictionary();
    }

    public List<string> GetPredictions(string inputText, IReadOnlyCollection<AacHistoryItem> historyItems, IReadOnlyCollection<AacPhraseItem> phraseItems)
    {
        var key = inputText.Trim();
        var fromDictionary = _dictionary.WordFrequency
            .Where(pair => pair.Key.StartsWith(key, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(pair => pair.Value)
            .Select(pair => pair.Key);

        var fromHistory = historyItems
            .Where(item => item.Text.StartsWith(key, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(item => item.UseCount)
            .ThenByDescending(item => item.CreatedAt)
            .Select(item => item.Text);

        var fromPhrases = phraseItems
            .Where(item => item.Text.StartsWith(key, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(item => item.UseCount)
            .Select(item => item.Text);

        return fromDictionary
            .Concat(fromHistory)
            .Concat(fromPhrases)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .Distinct()
            .Take(8)
            .ToList();
    }

    public void LearnFromText(string sourceText)
    {
        try
        {
            var words = Regex.Split(sourceText, @"[\u3000\s,、。．.!！？?]+")
                .Where(word => !string.IsNullOrWhiteSpace(word));

            foreach (var word in words)
            {
                if (!_dictionary.WordFrequency.TryAdd(word, 1))
                {
                    _dictionary.WordFrequency[word]++;
                }
            }

            _storageService.SavePredictionDictionary(_dictionary);
        }
        catch
        {
        }
    }
}
