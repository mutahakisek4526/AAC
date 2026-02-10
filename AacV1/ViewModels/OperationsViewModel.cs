using System.Collections.ObjectModel;
using AacV1.Infrastructure;
using AacV1.Models;
using AacV1.Services;
using AacV1.ViewModels.Interfaces;

namespace AacV1.ViewModels;

public class OperationsViewModel : ObservableObject, INavigationAware, IInputTarget
{
    private readonly IComputerControlService _computerControlService;
    private readonly IEnvironmentControlService _environmentControlService;
    private readonly IStorageService _storageService;
    private AacOperationMode _selectedMode = AacOperationMode.AAC;
    private AacEnvironmentAction? _selectedEnvironmentAction;

    public OperationsViewModel(
        IComputerControlService computerControlService,
        IEnvironmentControlService environmentControlService,
        IStorageService storageService)
    {
        _computerControlService = computerControlService;
        _environmentControlService = environmentControlService;
        _storageService = storageService;

        OperationModes = new ObservableCollection<AacOperationMode>(Enum.GetValues<AacOperationMode>());
        EnvironmentActions = new ObservableCollection<AacEnvironmentAction>();

        SendEnterCommand = new RelayCommand(() => _computerControlService.SendKey(AacKeyCode.Enter));
        MouseLeftClickCommand = new RelayCommand(() => _computerControlService.MouseClick(AacMouseButton.Left));
        ExecuteEnvironmentCommand = new AsyncRelayCommand(ExecuteEnvironmentAsync, () => SelectedEnvironmentAction is not null);
    }

    public ObservableCollection<AacOperationMode> OperationModes { get; }
    public ObservableCollection<AacEnvironmentAction> EnvironmentActions { get; }

    public RelayCommand SendEnterCommand { get; }
    public RelayCommand MouseLeftClickCommand { get; }
    public AsyncRelayCommand ExecuteEnvironmentCommand { get; }

    public AacOperationMode SelectedMode
    {
        get => _selectedMode;
        set => SetProperty(ref _selectedMode, value);
    }

    public AacEnvironmentAction? SelectedEnvironmentAction
    {
        get => _selectedEnvironmentAction;
        set
        {
            if (SetProperty(ref _selectedEnvironmentAction, value))
            {
                ExecuteEnvironmentCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public void OnEnter()
    {
        EnvironmentActions.Clear();
        foreach (var action in _storageService.LoadEnvironmentActions())
        {
            EnvironmentActions.Add(action);
        }
    }

    public void OnExit()
    {
    }

    public void OnSelect()
    {
        _ = ExecuteEnvironmentAsync();
    }

    public void OnBack()
    {
        _computerControlService.SendKey(AacKeyCode.Escape);
    }

    private async Task ExecuteEnvironmentAsync()
    {
        if (SelectedEnvironmentAction is not null)
        {
            await _environmentControlService.ExecuteAsync(SelectedEnvironmentAction);
        }
    }
}
