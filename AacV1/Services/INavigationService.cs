using AacV1.ViewModels.Interfaces;

namespace AacV1.Services;

public interface INavigationService
{
    object? CurrentViewModel { get; }
    void NavigateTo(object targetViewModel);
    bool GoBack();
}
