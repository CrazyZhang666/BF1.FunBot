﻿using BF1.FunBot.Models;
using BF1.FunBot.Features.Core;

namespace BF1.FunBot;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    public MainModel MainModel { get; set; } = new();

    // 快捷键
    private HotKeys MainHotKeys = new();

    private int PrimaryScreenWidth = (int)SystemParameters.PrimaryScreenWidth;
    private int PrimaryScreenHeight = (int)SystemParameters.PrimaryScreenHeight;

    /// <summary>
    /// 玩家是否死亡
    /// </summary>
    private bool isPlayerDeath = true;

    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = this;
    }

    private void Window_Main_Loaded(object sender, RoutedEventArgs e)
    {
        // 初始化线程
        Task.Run(() =>
        {
            // 初始化战地1
            if (Memory.Initialize())
            {
                AddRunningLog("战地1内存模块初始化成功");
                AddRunningLog("等待玩家操作...");
                AddRunningLog("");
                AddRunningLog($"进程ID：{Memory.Bf1ProcessID}");
                AddRunningLog($"窗口句柄：{Memory.Bf1WinHandle}");
                AddRunningLog($"进程句柄：{Memory.Bf1ProHandle}");
                AddRunningLog("");
                AddRunningLog($"主显示器屏幕分辨率：{PrimaryScreenWidth} x {PrimaryScreenHeight}");
            }
            else
            {
                AddRunningLog("战地1内存模块初始化失败，请重启程序");
            }
        });

        // 后台更新线程
        var thread1 = new Thread(UpdateData);
        thread1.IsBackground = true;
        thread1.Start();

        // 后台运行机器人线程
        var thread2 = new Thread(RunFunBot);
        thread2.IsBackground = true;
        thread2.Start();

        MainHotKeys.AddKey(WinVK.F5);
        MainHotKeys.AddKey(WinVK.F9);
        MainHotKeys.AddKey(WinVK.F10);
        MainHotKeys.KeyDownEvent += MainHotKeys_KeyDownEvent;
    }

    private void Window_Main_Closing(object sender, CancelEventArgs e)
    {
        Memory.CloseHandle();
        MainHotKeys.Dispose();
    }

    /// <summary>
    /// 输出运行日志
    /// </summary>
    /// <param name="logMsg">日志内容</param>
    private void AddRunningLog(string logMsg)
    {
        this.Dispatcher.Invoke(() =>
        {
            TextBox_RunningLog.AppendText($"[{DateTime.Now:MM/dd HH:mm:ss}] {logMsg}\r\n");
        });
    }

    private void MainHotKeys_KeyDownEvent(int Id, string Name)
    {
        switch (Id)
        {
            case (int)WinVK.F5:
                MainModel.GameDeployX = MainModel.GameMouseX;
                MainModel.GameDeployY = MainModel.GameMouseY;
                Console.Beep(500, 75);
                break;
            case (int)WinVK.F9:
                MainModel.IsRunFunBot = !MainModel.IsRunFunBot;
                if (MainModel.IsRunFunBot)
                {
                    Console.Beep(700, 75);
                }
                else
                {
                    Console.Beep(600, 75);
                }
                break;
            case (int)WinVK.F10:
                Application.Current.Shutdown();
                break;
        }
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void UpdateData()
    {
        while (true)
        {
            var windowData = Memory.GetGameWindowData();
            MainModel.GameResWidth = windowData.Width;
            MainModel.GameResHeight = windowData.Height;

            WinAPI.GetCursorPos(out var point);
            MainModel.GameMouseX = windowData.Width == -1 ? windowData.Left : point.X - windowData.Left;
            MainModel.GameMouseY = windowData.Height == -1 ? windowData.Top : point.Y - windowData.Top;

            MainModel.GameMouseX = MainModel.GameMouseX >= 0 && MainModel.GameMouseX <= windowData.Width ? MainModel.GameMouseX : 0;
            MainModel.GameMouseY = MainModel.GameMouseY >= 0 && MainModel.GameMouseY <= windowData.Height ? MainModel.GameMouseY : 0;

            if (MainModel.GameMouseX == 0 || MainModel.GameMouseY == 0)
                MainModel.GameMouseX = MainModel.GameMouseY = 0;

            // 矩阵信息
            var view_offset = Memory.Read<long>(Offsets.OFFSET_GAMERENDERER, new int[] { 0x60 });
            var view_matrix4x4 = Memory.Read<Matrix4x4>(view_offset + 0x460);

            MainModel.GameCameraX = view_matrix4x4.M41;
            MainModel.GameCameraY = view_matrix4x4.M42;
            MainModel.GameCameraZ = view_matrix4x4.M44;

            var baseAddress = Player.GetLocalPlayer();
            // 士兵基址
            var cse_baseAddress = Memory.Read<long>(baseAddress + 0x1D48);

            isPlayerDeath = cse_baseAddress == 0;

            //////////////////////

            Thread.Sleep(1);
        }
    }

    /// <summary>
    /// 运行机器人
    /// </summary>
    private void RunFunBot()
    {
        bool isShiftW = false;

        while (true)
        {
            if (MainModel.IsRunFunBot)
            {
                var m_isTopWindow = Memory.Read<bool>(Offsets.OFFSET_DXRENDERER, new int[] { 0x820, 0x5F });

                // 战地1窗口在最前方
                if (m_isTopWindow)
                {
                    // 开始奔跑
                    //if (!isShiftW)
                    //{
                    //    WinAPI.Keybd_Event(WinVK.W, WinAPI.MapVirtualKey(WinVK.W, 0), 0, 0);
                    //    WinAPI.Keybd_Event(WinVK.LSHIFT, WinAPI.MapVirtualKey(WinVK.LSHIFT, 0), 0, 0);
                    //    isShiftW = true;
                    //}

                    // 玩家在部署界面
                    while (MainModel.IsRunFunBot && m_isTopWindow && isPlayerDeath && MainModel.GameCameraZ > 430)
                    {
                        MoveGameMouse();

                        Thread.Sleep(1000);
                        WinAPI.Mouse_Event(MouseEventFlag.LeftDown, 0, 0, 0, 0);
                        Thread.Sleep(200);
                        WinAPI.Mouse_Event(MouseEventFlag.LeftUp, 0, 0, 0, 0);

                        Thread.Sleep(1000);
                        WinAPI.Keybd_Event(WinVK.SPACE, WinAPI.MapVirtualKey(WinVK.SPACE, 0), 0, 0);
                        Thread.Sleep(200);
                        WinAPI.Keybd_Event(WinVK.SPACE, WinAPI.MapVirtualKey(WinVK.SPACE, 0), 2, 0);
                    }

                    // 玩家死亡
                    while (MainModel.IsRunFunBot && m_isTopWindow && isPlayerDeath && MainModel.GameCameraZ < 430)
                    {
                        Thread.Sleep(1000);
                        WinAPI.Keybd_Event(WinVK.SPACE, WinAPI.MapVirtualKey(WinVK.SPACE, 0), 0, 0);
                        Thread.Sleep(200);
                        WinAPI.Keybd_Event(WinVK.SPACE, WinAPI.MapVirtualKey(WinVK.SPACE, 0), 2, 0);
                    }
                }
                else
                {
                    Memory.SetForegroundWindow();

                    // 取消奔跑
                    if (isShiftW)
                    {
                        WinAPI.Keybd_Event(WinVK.LSHIFT, WinAPI.MapVirtualKey(WinVK.LSHIFT, 0), 2, 0);
                        WinAPI.Keybd_Event(WinVK.W, WinAPI.MapVirtualKey(WinVK.W, 0), 2, 0);
                        isShiftW = false;
                    }
                }
            }
            else
            {
                // 取消奔跑
                if (isShiftW)
                {
                    WinAPI.Keybd_Event(WinVK.LSHIFT, WinAPI.MapVirtualKey(WinVK.LSHIFT, 0), 2, 0);
                    WinAPI.Keybd_Event(WinVK.W, WinAPI.MapVirtualKey(WinVK.W, 0), 2, 0);
                    isShiftW = false;
                }
            }

            //////////////////////

            Thread.Sleep(1);
        }
    }

    /// <summary>
    /// 移动游戏内鼠标
    /// </summary>
    private void MoveGameMouse()
    {
        var windowData = Memory.GetGameWindowData();
        var point = new POINT
        {
            X = windowData.Left + MainModel.GameDeployX,
            Y = windowData.Top + MainModel.GameDeployY
        };
        WinAPI.SetCursorPos(point.X, point.Y);

        point.X = point.X * 65535 / PrimaryScreenWidth;
        point.Y = point.Y * 65535 / PrimaryScreenHeight;
        WinAPI.Mouse_Event(MouseEventFlag.Move | MouseEventFlag.Absolute, point.X, point.Y, 0, 0);
    }
}