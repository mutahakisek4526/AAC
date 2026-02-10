using System.Runtime.InteropServices;
using AacV1.Models;

namespace AacV1.Services;

public class ComputerControlService : IComputerControlService
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint numberOfInputs, AacNativeInput[] inputs, int sizeOfInputStructure);

    public void SendKey(AacKeyCode keyCode)
    {
        try
        {
            var virtualKey = keyCode switch
            {
                AacKeyCode.Enter => 0x0D,
                AacKeyCode.Escape => 0x1B,
                AacKeyCode.Space => 0x20,
                AacKeyCode.Tab => 0x09,
                AacKeyCode.Up => 0x26,
                AacKeyCode.Down => 0x28,
                AacKeyCode.Left => 0x25,
                AacKeyCode.Right => 0x27,
                _ => 0
            };

            if (virtualKey == 0)
            {
                return;
            }

            var downInput = BuildKeyboardInput(virtualKey, 0);
            var upInput = BuildKeyboardInput(virtualKey, 0x0002);
            SendInput(2, new[] { downInput, upInput }, Marshal.SizeOf<AacNativeInput>());
        }
        catch
        {
        }
    }

    public void MouseMove(int dx, int dy)
    {
        try
        {
            var input = new AacNativeInput
            {
                Type = 0,
                Data = new AacInputUnion
                {
                    MouseInput = new AacMouseInput
                    {
                        Dx = dx,
                        Dy = dy,
                        MouseData = 0,
                        DwFlags = 0x0001,
                        Time = 0,
                        DwExtraInfo = IntPtr.Zero
                    }
                }
            };

            SendInput(1, new[] { input }, Marshal.SizeOf<AacNativeInput>());
        }
        catch
        {
        }
    }

    public void MouseClick(AacMouseButton mouseButton)
    {
        try
        {
            uint downFlag = mouseButton == AacMouseButton.Right ? 0x0008u : 0x0002u;
            uint upFlag = mouseButton == AacMouseButton.Right ? 0x0010u : 0x0004u;

            var downInput = new AacNativeInput
            {
                Type = 0,
                Data = new AacInputUnion
                {
                    MouseInput = new AacMouseInput { DwFlags = downFlag }
                }
            };

            var upInput = new AacNativeInput
            {
                Type = 0,
                Data = new AacInputUnion
                {
                    MouseInput = new AacMouseInput { DwFlags = upFlag }
                }
            };

            SendInput(2, new[] { downInput, upInput }, Marshal.SizeOf<AacNativeInput>());
        }
        catch
        {
        }
    }

    private static AacNativeInput BuildKeyboardInput(ushort virtualKey, uint flags)
    {
        return new AacNativeInput
        {
            Type = 1,
            Data = new AacInputUnion
            {
                KeyboardInput = new AacKeyboardInput
                {
                    WVk = virtualKey,
                    WScan = 0,
                    DwFlags = flags,
                    Time = 0,
                    DwExtraInfo = IntPtr.Zero
                }
            }
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AacNativeInput
    {
        public int Type;
        public AacInputUnion Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct AacInputUnion
    {
        [FieldOffset(0)]
        public AacMouseInput MouseInput;

        [FieldOffset(0)]
        public AacKeyboardInput KeyboardInput;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AacMouseInput
    {
        public int Dx;
        public int Dy;
        public uint MouseData;
        public uint DwFlags;
        public uint Time;
        public IntPtr DwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AacKeyboardInput
    {
        public ushort WVk;
        public ushort WScan;
        public uint DwFlags;
        public uint Time;
        public IntPtr DwExtraInfo;
    }
}
