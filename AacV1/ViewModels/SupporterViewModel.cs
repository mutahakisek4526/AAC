using System.Collections.ObjectModel;
using AacV1.Infrastructure;
using AacV1.Models;
using AacV1.Services;
using AacV1.ViewModels.Interfaces;

namespace AacV1.ViewModels;

public class SupporterViewModel : ObservableObject, INavigationAware, IInputTarget
{
    private readonly IStorageService _storageService;
    private string _actionName = string.Empty;
    private string _actionType = "Keyboard";
    private string _actionArgument = string.Empty;
    private AacEnvironmentAction? _selectedAction;

    public SupporterViewModel(IStorageService storageService)
    {
        _storageService = storageService;
        Actions = new ObservableCollection<AacEnvironmentAction>();
        AddActionCommand = new RelayCommand(AddAction);
        RemoveActionCommand = new RelayCommand(RemoveAction, () => SelectedAction is not null);
    }

    public ObservableCollection<AacEnvironmentAction> Actions { get; }
    public RelayCommand AddActionCommand { get; }
    public RelayCommand RemoveActionCommand { get; }

    public string ActionName
    {
        get => _actionName;
        set => SetProperty(ref _actionName, value);
    }

    public string ActionType
    {
        get => _actionType;
        set => SetProperty(ref _actionType, value);
    }

    public string ActionArgument
    {
        get => _actionArgument;
        set => SetProperty(ref _actionArgument, value);
    }

    public AacEnvironmentAction? SelectedAction
    {
        get => _selectedAction;
        set
        {
            if (SetProperty(ref _selectedAction, value))
            {
                RemoveActionCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public void OnEnter()
    {
        Actions.Clear();
        foreach (var action in _storageService.LoadEnvironmentActions())
        {
            Actions.Add(action);
        }
    }

    public void OnExit()
    {
        Save();
    }

    public void OnSelect()
    {
        AddAction();
    }

    public void OnBack()
    {
        RemoveAction();
    }

    private void AddAction()
    {
        if (string.IsNullOrWhiteSpace(ActionName))
        {
            return;
        }

        Actions.Add(new AacEnvironmentAction
        {
            Name = ActionName,
            ActionType = ActionType,
            Argument = ActionArgument
        });

        ActionName = string.Empty;
        ActionType = "Keyboard";
        ActionArgument = string.Empty;
        Save();
    }

    private void RemoveAction()
    {
        if (SelectedAction is null)
        {
            return;
        }

        Actions.Remove(SelectedAction);
        Save();
    }

    private void Save()
    {
        _storageService.SaveEnvironmentActions(Actions.ToList());
    }
}
