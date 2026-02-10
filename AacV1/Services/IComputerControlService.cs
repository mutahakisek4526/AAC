using AacV1.Models;

namespace AacV1.Services;

public interface IComputerControlService
{
    void SendKey(AacKeyCode keyCode);
    void MouseMove(int dx, int dy);
    void MouseClick(AacMouseButton mouseButton);
}
