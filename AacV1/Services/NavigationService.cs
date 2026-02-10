using AacV1.Infrastructure;
using AacV1.ViewModels.Interfaces;

namespace AacV1.Services;

public class NavigationService : ObservableObject, INavigationService
{
    private readonly Stack<object> _historyStack = new();
    private object? _currentViewModel;

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public void NavigateTo(object targetViewModel)
    {
        if (CurrentViewModel is not null)
        {
            if (CurrentViewModel is INavigationAware leaving)
            {
                leaving.OnExit();
            }

            _historyStack.Push(CurrentViewModel);
        }

        CurrentViewModel = targetViewModel;
        if (targetViewModel is INavigationAware entering)
        {
            entering.OnEnter();
        }
    }

    public bool GoBack()
    {
        if (_historyStack.Count == 0)
        {
            return false;
        }

        if (CurrentViewModel is INavigationAware leaving)
        {
            leaving.OnExit();
        }

        var previousViewModel = _historyStack.Pop();
        CurrentViewModel = previousViewModel;
        if (previousViewModel is INavigationAware entering)
        {
            entering.OnEnter();
        }

        return true;
    }
}
