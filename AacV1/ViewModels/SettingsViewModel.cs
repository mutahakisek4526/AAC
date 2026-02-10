using AacV1.Infrastructure;
using AacV1.Models;
using AacV1.Services;
using AacV1.ViewModels.Interfaces;

namespace AacV1.ViewModels;

public class SettingsViewModel : ObservableObject, INavigationAware, IInputTarget
{
    private readonly IStorageService _storageService;
    private AacAppSettings _settings = new();

    public SettingsViewModel(IStorageService storageService)
    {
        _storageService = storageService;
        SaveCommand = new RelayCommand(Save);
    }

    public RelayCommand SaveCommand { get; }

    public double FontSize
    {
        get => _settings.FontSize;
        set
        {
            if (Math.Abs(_settings.FontSize - value) > 0.1)
            {
                _settings.FontSize = value;
                OnPropertyChanged();
            }
        }
    }

    public bool HighContrast
    {
        get => _settings.HighContrast;
        set
        {
            if (_settings.HighContrast != value)
            {
                _settings.HighContrast = value;
                OnPropertyChanged();
            }
        }
    }

    public int ScanIntervalMilliseconds
    {
        get => _settings.ScanIntervalMilliseconds;
        set
        {
            if (_settings.ScanIntervalMilliseconds != value)
            {
                _settings.ScanIntervalMilliseconds = value;
                OnPropertyChanged();
            }
        }
    }

    public void OnEnter()
    {
        _settings = _storageService.LoadSettings();
        OnPropertyChanged(nameof(FontSize));
        OnPropertyChanged(nameof(HighContrast));
        OnPropertyChanged(nameof(ScanIntervalMilliseconds));
    }

    public void OnExit()
    {
        Save();
    }

    public void OnSelect()
    {
        Save();
    }

    public void OnBack()
    {
    }

    private void Save()
    {
        _storageService.SaveSettings(_settings);
    }
}
