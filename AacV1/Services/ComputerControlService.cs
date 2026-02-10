using System.Runtime.InteropServices;
using AacV1.Models;

namespace AacV1.Services;

public class ComputerControlService : IComputerControlService
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, AacNativeInput[] pInputs, int cbSize);

    public void SendKey(AacKeyCode keyCode)
    {
        try
        {
            if (keyCode == AacKeyCode.AltTab)
            {
                SendChord(0x12, 0x09);
                return;
            }

            if (keyCode == AacKeyCode.WinD)
            {
                SendChord(0x5B, 0x44);
                return;
            }

            var vk = keyCode switch
            {
                AacKeyCode.Enter => (ushort)0x0D,
                AacKeyCode.Escape => (ushort)0x1B,
                AacKeyCode.Tab => (ushort)0x09,
                AacKeyCode.Up => (ushort)0x26,
                AacKeyCode.Down => (ushort)0x28,
                AacKeyCode.Left => (ushort)0x25,
                AacKeyCode.Right => (ushort)0x27,
                AacKeyCode.VolumeUp => (ushort)0xAF,
                AacKeyCode.VolumeDown => (ushort)0xAE,
                AacKeyCode.Mute => (ushort)0xAD,
                _ => (ushort)0
            };

            if (vk == 0)
            {
                return;
            }

            SendSingleKey(vk);
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
                    MouseInput = new AacMouseInput { Dx = dx, Dy = dy, DwFlags = 0x0001 }
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
            var down = new AacNativeInput
            {
                Type = 0,
                Data = new AacInputUnion { MouseInput = new AacMouseInput { DwFlags = 0x0002 } }
            };
            var up = new AacNativeInput
            {
                Type = 0,
                Data = new AacInputUnion { MouseInput = new AacMouseInput { DwFlags = 0x0004 } }
            };
            SendInput(2, new[] { down, up }, Marshal.SizeOf<AacNativeInput>());
        }
        catch
        {
        }
    }

    private void SendSingleKey(ushort vk)
    {
        var down = CreateKeyboard(vk, 0);
        var up = CreateKeyboard(vk, 0x0002);
        SendInput(2, new[] { down, up }, Marshal.SizeOf<AacNativeInput>());
    }

    private void SendChord(ushort modifierVk, ushort keyVk)
    {
        var modDown = CreateKeyboard(modifierVk, 0);
        var keyDown = CreateKeyboard(keyVk, 0);
        var keyUp = CreateKeyboard(keyVk, 0x0002);
        var modUp = CreateKeyboard(modifierVk, 0x0002);
        SendInput(4, new[] { modDown, keyDown, keyUp, modUp }, Marshal.SizeOf<AacNativeInput>());
    }

    private static AacNativeInput CreateKeyboard(ushort vk, uint flags)
    {
        return new AacNativeInput
        {
            Type = 1,
            Data = new AacInputUnion
            {
                KeyboardInput = new AacKeyboardInput
                {
                    WVk = vk,
                    WScan = (ushort)0,
                    DwFlags = flags
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
