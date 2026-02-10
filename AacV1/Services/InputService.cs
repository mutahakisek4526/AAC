using System.Windows.Input;
using AacV1.ViewModels.Interfaces;

namespace AacV1.Services;

public class InputService : IInputService
{
    public bool ProcessGlobalKey(Key inputKey, IInputTarget? inputTarget)
    {
        if (inputTarget is null)
        {
            return false;
        }

        if (inputKey == Key.Enter)
        {
            inputTarget.OnSelect();
            return true;
        }

        if (inputKey == Key.Escape)
        {
            inputTarget.OnBack();
            return true;
        }

        return false;
    }
}
