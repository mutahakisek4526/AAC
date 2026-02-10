using System.Windows.Input;
using AacV1.ViewModels.Interfaces;

namespace AacV1.Services;

public interface IInputService
{
    bool ProcessGlobalKey(Key inputKey, IInputTarget? inputTarget);
}
