using AacV1.Infrastructure;
using AacV1.Services;
using AacV1.ViewModels.Interfaces;

namespace AacV1.ViewModels;

public class MainViewModel : ObservableObject, IInputTarget
{
    private readonly INavigationService _navigationService;
    private readonly IInputService _inputService;

    public MainViewModel()
    {
        var storageService = new StorageService();
        var speechService = new SpeechService();
        var predictionService = new PredictionService(storageService);
        var computerControlService = new ComputerControlService();
        var environmentControlService = new EnvironmentControlService(computerControlService);

        _navigationService = new NavigationService();
        _inputService = new InputService();

        HomeViewModel = new HomeViewModel();
        KanaBoardViewModel = new KanaBoardViewModel(speechService, predictionService, storageService);
        PhraseViewModel = new PhraseViewModel(storageService, speechService);
        HistoryViewModel = new HistoryViewModel(storageService);
        OperationsViewModel = new OperationsViewModel(computerControlService, environmentControlService, storageService);
        SettingsViewModel = new SettingsViewModel(storageService);
        SupporterViewModel = new SupporterViewModel(storageService);

        GoHomeCommand = new RelayCommand(() => Navigate(HomeViewModel));
        GoKanaBoardCommand = new RelayCommand(() => Navigate(KanaBoardViewModel));
        GoPhraseCommand = new RelayCommand(() => Navigate(PhraseViewModel));
        GoHistoryCommand = new RelayCommand(() => Navigate(HistoryViewModel));
        GoOperationsCommand = new RelayCommand(() => Navigate(OperationsViewModel));
        GoSettingsCommand = new RelayCommand(() => Navigate(SettingsViewModel));
        GoSupporterCommand = new RelayCommand(() => Navigate(SupporterViewModel));

        Navigate(HomeViewModel);
    }

    public object? CurrentViewModel => _navigationService.CurrentViewModel;

    public HomeViewModel HomeViewModel { get; }
    public KanaBoardViewModel KanaBoardViewModel { get; }
    public PhraseViewModel PhraseViewModel { get; }
    public HistoryViewModel HistoryViewModel { get; }
    public OperationsViewModel OperationsViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }
    public SupporterViewModel SupporterViewModel { get; }

    public RelayCommand GoHomeCommand { get; }
    public RelayCommand GoKanaBoardCommand { get; }
    public RelayCommand GoPhraseCommand { get; }
    public RelayCommand GoHistoryCommand { get; }
    public RelayCommand GoOperationsCommand { get; }
    public RelayCommand GoSettingsCommand { get; }
    public RelayCommand GoSupporterCommand { get; }

    public bool ProcessKey(System.Windows.Input.Key inputKey)
    {
        var target = CurrentViewModel as IInputTarget;
        if (_inputService.ProcessGlobalKey(inputKey, target))
        {
            return true;
        }

        return false;
    }

    public void OnSelect()
    {
        if (CurrentViewModel is IInputTarget target)
        {
            target.OnSelect();
        }
    }

    public void OnBack()
    {
        if (_navigationService.GoBack())
        {
            OnPropertyChanged(nameof(CurrentViewModel));
            return;
        }

        if (CurrentViewModel is IInputTarget target)
        {
            target.OnBack();
        }
    }

    private void Navigate(object targetViewModel)
    {
        _navigationService.NavigateTo(targetViewModel);
        OnPropertyChanged(nameof(CurrentViewModel));
    }
}
