using AacV1.Infrastructure;
using AacV1.ViewModels.Interfaces;

namespace AacV1.ViewModels;

public class HomeViewModel : ObservableObject, INavigationAware, IInputTarget
{
    private string _welcomeMessage = "ホームへようこそ";

    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }

    public void OnEnter()
    {
        WelcomeMessage = "ホーム画面です。左のメニューから機能を選択してください。";
    }

    public void OnExit()
    {
    }

    public void OnSelect()
    {
    }

    public void OnBack()
    {
    }
}
