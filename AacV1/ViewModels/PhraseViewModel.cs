using System.Collections.ObjectModel;
using AacV1.Infrastructure;
using AacV1.Models;
using AacV1.Services;
using AacV1.ViewModels.Interfaces;

namespace AacV1.ViewModels;

public class PhraseViewModel : ObservableObject, INavigationAware, IInputTarget
{
    private readonly IStorageService _storageService;
    private readonly ISpeechService _speechService;
    private string _newPhraseName = string.Empty;
    private string _newPhraseText = string.Empty;
    private AacPhraseItem? _selectedPhrase;

    public PhraseViewModel(IStorageService storageService, ISpeechService speechService)
    {
        _storageService = storageService;
        _speechService = speechService;
        Phrases = new ObservableCollection<AacPhraseItem>(_storageService.LoadPhrases());

        AddPhraseCommand = new RelayCommand(AddPhrase);
        RemovePhraseCommand = new RelayCommand(RemovePhrase, () => SelectedPhrase is not null);
        SpeakPhraseCommand = new AsyncRelayCommand(SpeakPhraseAsync, () => SelectedPhrase is not null);
    }

    public ObservableCollection<AacPhraseItem> Phrases { get; }

    public RelayCommand AddPhraseCommand { get; }
    public RelayCommand RemovePhraseCommand { get; }
    public AsyncRelayCommand SpeakPhraseCommand { get; }

    public string NewPhraseName
    {
        get => _newPhraseName;
        set => SetProperty(ref _newPhraseName, value);
    }

    public string NewPhraseText
    {
        get => _newPhraseText;
        set => SetProperty(ref _newPhraseText, value);
    }

    public AacPhraseItem? SelectedPhrase
    {
        get => _selectedPhrase;
        set
        {
            if (SetProperty(ref _selectedPhrase, value))
            {
                RemovePhraseCommand.RaiseCanExecuteChanged();
                SpeakPhraseCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
        Save();
    }

    public void OnSelect()
    {
        _ = SpeakPhraseAsync();
    }

    public void OnBack()
    {
        RemovePhrase();
    }

    private void AddPhrase()
    {
        if (string.IsNullOrWhiteSpace(NewPhraseText))
        {
            return;
        }

        Phrases.Add(new AacPhraseItem
        {
            Name = string.IsNullOrWhiteSpace(NewPhraseName) ? "新規フレーズ" : NewPhraseName,
            Text = NewPhraseText
        });

        NewPhraseName = string.Empty;
        NewPhraseText = string.Empty;
        Save();
    }

    private void RemovePhrase()
    {
        if (SelectedPhrase is null)
        {
            return;
        }

        Phrases.Remove(SelectedPhrase);
        Save();
    }

    private async Task SpeakPhraseAsync()
    {
        if (SelectedPhrase is not null)
        {
            await _speechService.SpeakAsync(SelectedPhrase.Text);
        }
    }

    private void Save()
    {
        _storageService.SavePhrases(Phrases.ToList());
    }
}
