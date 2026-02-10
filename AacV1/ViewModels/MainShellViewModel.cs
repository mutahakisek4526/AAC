using System.Collections.ObjectModel;
using AacV1.Infrastructure;
using AacV1.Models;
using AacV1.Services;

namespace AacV1.ViewModels;

public class MainShellViewModel : ObservableObject
{
    private readonly IStorageService _storageService;
    private readonly IPredictionService _predictionService;
    private readonly ISpeechService _speechService;
    private readonly IComputerControlService _computerControlService;
    private readonly InputEngineService _inputEngineService;

    private readonly List<AacHistoryItem> _historyItems;
    private readonly List<AacPhraseItem> _phraseItems;
    private AacAppSettings _settings;
    private string _inputText = string.Empty;
    private string _statusText = "準備完了";
    private string _selectedCategory = "日常";
    private bool _supporterMode;

    private string _supporterCategory = "日常";
    private string _supporterPhraseText = string.Empty;

    public MainShellViewModel(
        IStorageService storageService,
        IPredictionService predictionService,
        ISpeechService speechService,
        IComputerControlService computerControlService,
        InputEngineService inputEngineService)
    {
        _storageService = storageService;
        _predictionService = predictionService;
        _speechService = speechService;
        _computerControlService = computerControlService;
        _inputEngineService = inputEngineService;

        _settings = _storageService.LoadSettings();
        _historyItems = _storageService.LoadHistory();
        _phraseItems = _storageService.LoadPhrases();

        KanaKeys = new ObservableCollection<string>(CreateKanaKeys());
        PredictionItems = new ObservableCollection<string>();
        CategoryItems = new ObservableCollection<string>(_phraseItems.Select(p => p.Category).Distinct().OrderBy(x => x));
        PhraseItems = new ObservableCollection<AacPhraseItem>();
        HistoryItems = new ObservableCollection<AacHistoryItem>();
        InputModes = new ObservableCollection<AacInputMode>(Enum.GetValues<AacInputMode>());

        AppendKanaCommand = new RelayCommand(() => { });
        BackspaceCommand = new RelayCommand(Backspace);
        UndoCommand = new RelayCommand(Undo);
        ClearCommand = new RelayCommand(ClearAll);
        NewLineCommand = new RelayCommand(() => InputText += Environment.NewLine);
        CommitCommand = new RelayCommand(CommitText);
        SpeakCommand = new AsyncRelayCommand(SpeakAsync);
        StopCommand = new RelayCommand(() => _speechService.Stop());
        ApplySettingsCommand = new RelayCommand(ApplySettings);
        ToggleSupporterModeCommand = new RelayCommand(() => SupporterMode = !SupporterMode);
        AddSupporterPhraseCommand = new RelayCommand(AddSupporterPhrase);
        DeleteSupporterPhraseCommand = new RelayCommand(DeleteSupporterPhrase);

        RefreshAll();
        ApplySettings();
    }

    public ObservableCollection<string> KanaKeys { get; }
    public ObservableCollection<string> PredictionItems { get; }
    public ObservableCollection<string> CategoryItems { get; }
    public ObservableCollection<AacPhraseItem> PhraseItems { get; }
    public ObservableCollection<AacHistoryItem> HistoryItems { get; }
    public ObservableCollection<AacInputMode> InputModes { get; }

    public RelayCommand AppendKanaCommand { get; }
    public RelayCommand BackspaceCommand { get; }
    public RelayCommand UndoCommand { get; }
    public RelayCommand ClearCommand { get; }
    public RelayCommand NewLineCommand { get; }
    public RelayCommand CommitCommand { get; }
    public AsyncRelayCommand SpeakCommand { get; }
    public RelayCommand StopCommand { get; }
    public RelayCommand ApplySettingsCommand { get; }
    public RelayCommand ToggleSupporterModeCommand { get; }
    public RelayCommand AddSupporterPhraseCommand { get; }
    public RelayCommand DeleteSupporterPhraseCommand { get; }

    public string InputText
    {
        get => _inputText;
        set
        {
            if (SetProperty(ref _inputText, value))
            {
                RefreshPredictions();
            }
        }
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                RefreshPhrases();
            }
        }
    }

    public AacInputMode SelectedInputMode
    {
        get => _settings.InputMode;
        set
        {
            if (_settings.InputMode != value)
            {
                _settings.InputMode = value;
                OnPropertyChanged();
            }
        }
    }

    public int DwellMilliseconds
    {
        get => _settings.DwellMilliseconds;
        set { _settings.DwellMilliseconds = value; OnPropertyChanged(); }
    }

    public int ScanIntervalMilliseconds
    {
        get => _settings.ScanIntervalMilliseconds;
        set { _settings.ScanIntervalMilliseconds = value; OnPropertyChanged(); }
    }

    public bool UseRowColumnScan
    {
        get => _settings.UseRowColumnScan;
        set { _settings.UseRowColumnScan = value; OnPropertyChanged(); }
    }

    public bool UseTwoSwitchMode
    {
        get => _settings.UseTwoSwitchMode;
        set { _settings.UseTwoSwitchMode = value; OnPropertyChanged(); }
    }

    public bool HighContrast
    {
        get => _settings.HighContrast;
        set { _settings.HighContrast = value; OnPropertyChanged(); }
    }

    public bool SupporterMode
    {
        get => _supporterMode;
        set => SetProperty(ref _supporterMode, value);
    }

    public string SupporterCategory
    {
        get => _supporterCategory;
        set => SetProperty(ref _supporterCategory, value);
    }

    public string SupporterPhraseText
    {
        get => _supporterPhraseText;
        set => SetProperty(ref _supporterPhraseText, value);
    }

    public void AppendText(string text)
    {
        InputText += text;
    }

    public void ApplyPrediction(string text)
    {
        InputText = text;
    }

    public void ApplyPhrase(AacPhraseItem phraseItem)
    {
        InputText += phraseItem.Text;
        phraseItem.UseCount++;
        SavePhrases();
        RefreshPhrases();
    }

    public bool HandleGlobalKey(System.Windows.Input.Key key)
    {
        return _inputEngineService.HandleKeyDown(key, Undo);
    }

    public void AttachScanTargets(IReadOnlyList<System.Windows.Controls.Button> buttons)
    {
        _inputEngineService.SetScanTargets(buttons);
    }

    public void BeginHover(System.Windows.Controls.Button button, Action clickAction, Action<double> progressAction)
    {
        _inputEngineService.BeginHover(button, clickAction, progressAction);
    }

    public void EndHover()
    {
        _inputEngineService.EndHover();
    }

    public void PcActionKey(AacKeyCode keyCode)
    {
        _computerControlService.SendKey(keyCode);
    }

    public void PcMove(int dx, int dy)
    {
        _computerControlService.MouseMove(dx, dy);
    }

    public void PcClick()
    {
        _computerControlService.MouseClick(AacMouseButton.Left);
    }

    private async Task SpeakAsync()
    {
        await _speechService.SpeakAsync(InputText);
        if (!string.IsNullOrWhiteSpace(_speechService.LastError))
        {
            StatusText = $"読み上げエラー: {_speechService.LastError}";
        }
    }

    private void Backspace()
    {
        if (InputText.Length > 0)
        {
            InputText = InputText[..^1];
        }
    }

    private void Undo()
    {
        Backspace();
    }

    private void ClearAll()
    {
        InputText = string.Empty;
    }

    private void CommitText()
    {
        if (string.IsNullOrWhiteSpace(InputText))
        {
            return;
        }

        _predictionService.LearnFromText(InputText);
        var existing = _historyItems.FirstOrDefault(h => h.Text == InputText);
        if (existing is null)
        {
            _historyItems.Insert(0, new AacHistoryItem { Text = InputText, UseCount = 1, CreatedAt = DateTime.Now });
        }
        else
        {
            existing.UseCount++;
            existing.CreatedAt = DateTime.Now;
        }

        _storageService.SaveHistory(_historyItems.Take(200).ToList());
        RefreshHistory();
        StatusText = "文章を確定しました";
    }

    private void RefreshAll()
    {
        RefreshPredictions();
        RefreshPhrases();
        RefreshHistory();
    }

    private void RefreshPredictions()
    {
        PredictionItems.Clear();
        foreach (var item in _predictionService.GetPredictions(InputText, _historyItems, _phraseItems))
        {
            PredictionItems.Add(item);
        }
    }

    private void RefreshPhrases()
    {
        PhraseItems.Clear();
        foreach (var item in _phraseItems.Where(p => p.Category == SelectedCategory))
        {
            PhraseItems.Add(item);
        }
    }

    private void RefreshHistory()
    {
        HistoryItems.Clear();
        foreach (var item in _historyItems.OrderByDescending(h => h.CreatedAt).Take(20))
        {
            HistoryItems.Add(item);
        }
    }

    private void ApplySettings()
    {
        _storageService.SaveSettings(_settings);
        _inputEngineService.ApplySettings(_settings);
        StatusText = "入力方式設定を適用しました";
    }

    private void SavePhrases()
    {
        _storageService.SavePhrases(_phraseItems);
    }


    private void AddSupporterPhrase()
    {
        if (!SupporterMode || string.IsNullOrWhiteSpace(SupporterPhraseText))
        {
            return;
        }

        _phraseItems.Add(new AacPhraseItem { Category = string.IsNullOrWhiteSpace(SupporterCategory) ? "日常" : SupporterCategory, Text = SupporterPhraseText });
        if (!CategoryItems.Contains(SupporterCategory))
        {
            CategoryItems.Add(SupporterCategory);
        }

        SupporterPhraseText = string.Empty;
        SavePhrases();
        RefreshPhrases();
    }

    private void DeleteSupporterPhrase()
    {
        if (!SupporterMode || string.IsNullOrWhiteSpace(SupporterPhraseText))
        {
            return;
        }

        var target = _phraseItems.FirstOrDefault(x => x.Text == SupporterPhraseText && x.Category == SupporterCategory);
        if (target is null)
        {
            return;
        }

        _phraseItems.Remove(target);
        SavePhrases();
        RefreshPhrases();
    }

    private static List<string> CreateKanaKeys()
    {
        return new List<string>
        {
            "あ","い","う","え","お","か","き","く","け","こ","さ","し","す","せ","そ","た","ち","つ","て","と",
            "な","に","ぬ","ね","の","は","ひ","ふ","へ","ほ","ま","み","む","め","も","や","ゆ","よ","ら","り",
            "る","れ","ろ","わ","を","ん","゛","゜","ゃ","ゅ","ょ","ー","。","、","？","！","A","1","2","3"
        };
    }
}
