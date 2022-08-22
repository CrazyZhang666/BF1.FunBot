namespace BF1.FunBot.Features.Core;

public static class Memory
{
    /// <summary>
    /// 战地1进程
    /// </summary>
    public static Process Bf1Process { get; private set; }
    /// <summary>
    /// 战地1窗口句柄
    /// </summary>
    public static IntPtr Bf1WinHandle { get; private set; }
    /// <summary>
    /// 战地1进程句柄
    /// </summary>
    public static IntPtr Bf1ProHandle { get; private set; }
    /// <summary>
    /// 战地1进程ID
    /// </summary>
    public static int Bf1ProcessID { get; private set; }
    /// <summary>
    /// 战地1进程基地址
    /// </summary>
    public static long Bf1ProBaseAddress { get; private set; }

    public static bool Initialize()
    {
        try
        {
            var pArray = Process.GetProcessesByName("bf1");
            if (pArray.Length > 0)
            {
                // 默认取第一个
                Bf1Process = pArray[0];
                // 二次验证
                foreach (var item in pArray)
                {
                    if (item.MainWindowTitle.Equals("Battlefield™ 1"))
                        Bf1Process = item;
                }

                Bf1WinHandle = Bf1Process.MainWindowHandle;
                Bf1ProcessID = Bf1Process.Id;
                Bf1ProHandle = WinAPI.OpenProcess(ProcessAccessFlags.All, false, Bf1ProcessID);
                if (Bf1Process.MainModule != null)
                {
                    Bf1ProBaseAddress = Bf1Process.MainModule.BaseAddress.ToInt64();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static void CloseHandle()
    {
        if (Bf1ProHandle != IntPtr.Zero)
            WinAPI.CloseHandle(Bf1ProHandle);
    }

    public static bool IsForegroundWindow()
    {
        return Bf1WinHandle == WinAPI.GetForegroundWindow();
    }

    public static void SetForegroundWindow()
    {
        WinAPI.SetForegroundWindow(Bf1WinHandle);
    }

    public static WindowData GetGameWindowData()
    {
        // 获取指定窗口句柄的窗口矩形数据和客户区矩形数据
        WinAPI.GetWindowRect(Bf1WinHandle, out W32RECT windowRect);
        WinAPI.GetClientRect(Bf1WinHandle, out W32RECT clientRect);

        // 计算窗口区的宽和高
        int windowWidth = windowRect.Right - windowRect.Left;
        int windowHeight = windowRect.Bottom - windowRect.Top;

        // 处理窗口最小化
        if (windowRect.Left == -32000)
        {
            return new WindowData()
            {
                Left = 0,
                Top = 0,
                Width = -1,
                Height = -1
            };
        }

        // 计算客户区的宽和高
        int clientWidth = clientRect.Right - clientRect.Left;
        int clientHeight = clientRect.Bottom - clientRect.Top;

        // 计算边框
        int borderWidth = (windowWidth - clientWidth) / 2;
        int borderHeight = windowHeight - clientHeight - borderWidth;

        return new WindowData()
        {
            Left = windowRect.Left += borderWidth,
            Top = windowRect.Top += borderHeight,
            Width = clientWidth,
            Height = clientHeight
        };
    }

    private static long GetPtrAddress(long pointer, int[] offset)
    {
        if (offset != null)
        {
            byte[] buffer = new byte[8];
            WinAPI.ReadProcessMemory(Bf1ProHandle, pointer, buffer, buffer.Length, out _);

            for (int i = 0; i < (offset.Length - 1); i++)
            {
                pointer = BitConverter.ToInt64(buffer, 0) + offset[i];
                WinAPI.ReadProcessMemory(Bf1ProHandle, pointer, buffer, buffer.Length, out _);
            }

            pointer = BitConverter.ToInt64(buffer, 0) + offset[offset.Length - 1];
        }

        return pointer;
    }

    public static T Read<T>(long basePtr, int[] offsets) where T : struct
    {
        byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
        WinAPI.ReadProcessMemory(Bf1ProHandle, GetPtrAddress(basePtr, offsets), buffer, buffer.Length, out _);
        return ByteArrayToStructure<T>(buffer);
    }

    public static T Read<T>(long address) where T : struct
    {
        byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
        WinAPI.ReadProcessMemory(Bf1ProHandle, address, buffer, buffer.Length, out _);
        return ByteArrayToStructure<T>(buffer);
    }

    public static void Write<T>(long basePtr, int[] offsets, T value) where T : struct
    {
        byte[] buffer = StructureToByteArray(value);
        WinAPI.WriteProcessMemory(Bf1ProHandle, GetPtrAddress(basePtr, offsets), buffer, buffer.Length, out _);
    }

    public static void Write<T>(long address, T value) where T : struct
    {
        byte[] buffer = StructureToByteArray(value);
        WinAPI.WriteProcessMemory(Bf1ProHandle, address, buffer, buffer.Length, out _);
    }

    public static string ReadString(long address, int size)
    {
        byte[] buffer = new byte[size];
        WinAPI.ReadProcessMemory(Bf1ProHandle, address, buffer, size, out _);

        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i] == 0)
            {
                byte[] _buffer = new byte[i];
                Buffer.BlockCopy(buffer, 0, _buffer, 0, i);
                return Encoding.ASCII.GetString(_buffer);
            }
        }

        return Encoding.ASCII.GetString(buffer);
    }

    public static string ReadString(long basePtr, int[] offsets, int size)
    {
        byte[] buffer = new byte[size];
        WinAPI.ReadProcessMemory(Bf1ProHandle, GetPtrAddress(basePtr, offsets), buffer, size, out _);

        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i] == 0)
            {
                byte[] _buffer = new byte[i];
                Buffer.BlockCopy(buffer, 0, _buffer, 0, i);
                return Encoding.ASCII.GetString(_buffer);
            }
        }

        return Encoding.ASCII.GetString(buffer);
    }

    public static void WriteString(long basePtr, int[] offsets, string str)
    {
        byte[] buffer = new ASCIIEncoding().GetBytes(str);
        WinAPI.WriteProcessMemory(Bf1ProHandle, GetPtrAddress(basePtr, offsets), buffer, buffer.Length, out _);
    }

    public static void WriteStringUTF8(long basePtr, int[] offsets, string str)
    {
        byte[] buffer = new UTF8Encoding().GetBytes(str);
        WinAPI.WriteProcessMemory(Bf1ProHandle, GetPtrAddress(basePtr, offsets), buffer, buffer.Length, out _);
    }

    //////////////////////////////////////////////////////////////////

    public static bool IsValid(long Address)
    {
        return Address >= 0x10000 && Address < 0x000F000000000000;
    }

    private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
    {
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            var obj = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            if (obj != null)
                return (T)obj;
            else
                return default(T);
        }
        finally
        {
            handle.Free();
        }
    }

    private static byte[] StructureToByteArray(object obj)
    {
        int length = Marshal.SizeOf(obj);
        byte[] array = new byte[length];
        IntPtr pointer = Marshal.AllocHGlobal(length);
        Marshal.StructureToPtr(obj, pointer, true);
        Marshal.Copy(pointer, array, 0, length);
        Marshal.FreeHGlobal(pointer);
        return array;
    }
}

public struct WindowData
{
    public int Left;
    public int Top;
    public int Width;
    public int Height;
}
