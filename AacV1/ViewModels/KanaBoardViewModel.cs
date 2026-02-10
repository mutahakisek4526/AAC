using System.Collections.ObjectModel;
using AacV1.Infrastructure;
using AacV1.Models;
using AacV1.Services;
using AacV1.ViewModels.Interfaces;

namespace AacV1.ViewModels;

public class KanaBoardViewModel : ObservableObject, INavigationAware, IInputTarget
{
    private readonly ISpeechService _speechService;
    private readonly IPredictionService _predictionService;
    private readonly IStorageService _storageService;

    private string _inputText = string.Empty;
    private string _selectedKanaGroup = "あ";

    public KanaBoardViewModel(ISpeechService speechService, IPredictionService predictionService, IStorageService storageService)
    {
        _speechService = speechService;
        _predictionService = predictionService;
        _storageService = storageService;

        KanaGroups = new ObservableCollection<string>(new[] { "あ", "か", "さ", "た", "な", "は", "ま", "や", "ら", "わ" });
        Predictions = new ObservableCollection<string>();

        AddCharCommand = new RelayCommand(AddSelectedKana);
        DeleteCommand = new RelayCommand(DeleteLastCharacter);
        ClearCommand = new RelayCommand(ClearInput);
        SpeakCommand = new AsyncRelayCommand(SpeakAsync);
        StopCommand = new RelayCommand(() => _speechService.Stop());
    }

    public ObservableCollection<string> KanaGroups { get; }
    public ObservableCollection<string> Predictions { get; }

    public RelayCommand AddCharCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand ClearCommand { get; }
    public AsyncRelayCommand SpeakCommand { get; }
    public RelayCommand StopCommand { get; }

    public string InputText
    {
        get => _inputText;
        set
        {
            if (SetProperty(ref _inputText, value))
            {
                UpdatePredictions();
            }
        }
    }

    public string SelectedKanaGroup
    {
        get => _selectedKanaGroup;
        set => SetProperty(ref _selectedKanaGroup, value);
    }

    public void OnEnter()
    {
        UpdatePredictions();
    }

    public void OnExit()
    {
        _predictionService.LearnWord(InputText);
        _storageService.SaveHistory(new List<AacHistoryItem>
        {
            new() { CreatedAt = DateTime.Now, Text = InputText }
        });
    }

    public void OnSelect()
    {
        AddSelectedKana();
    }

    public void OnBack()
    {
        DeleteLastCharacter();
    }

    private async Task SpeakAsync()
    {
        await _speechService.SpeakAsync(InputText);
    }

    private void AddSelectedKana()
    {
        InputText += SelectedKanaGroup;
    }

    private void DeleteLastCharacter()
    {
        if (InputText.Length > 0)
        {
            InputText = InputText[..^1];
        }
    }

    private void ClearInput()
    {
        InputText = string.Empty;
    }

    private void UpdatePredictions()
    {
        Predictions.Clear();
        foreach (var prediction in _predictionService.GetPredictions(InputText))
        {
            Predictions.Add(prediction);
        }
    }
}
