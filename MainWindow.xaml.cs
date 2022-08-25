using BF1.FunBot.Models;
using BF1.FunBot.Common.Utils;
using BF1.FunBot.Features.Core;
using BF1.FunBot.Features.Utils;
using BF1.FunBot.Features.Config;
using BF1.FunBot.Features.Client;

namespace BF1.FunBot;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// 主窗口数据模型
    /// </summary>
    public MainModel MainModel { get; set; } = new();
    /// <summary>
    /// 快捷键
    /// </summary>
    private HotKeys HotKeys = new();

    /// <summary>
    /// 主屏幕宽度
    /// </summary>
    private int PrimaryScreenWidth = (int)SystemParameters.PrimaryScreenWidth;
    /// <summary>
    /// 主屏幕高度
    /// </summary>
    private int PrimaryScreenHeight = (int)SystemParameters.PrimaryScreenHeight;

    /// <summary>
    /// 玩家是否死亡
    /// </summary>
    private bool IsPlayerDeath = true;
    /// <summary>
    /// 地图相机高度
    /// </summary>
    private float MapCameraZ = 0;

    /// <summary>
    /// 部署点配置文件
    /// </summary>
    private List<DeployConfig> DeployConfigs = new();

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
                AddRunningLog($"进程ID：{Memory.Bf1ProcessID}");
                AddRunningLog($"窗口句柄：{Memory.Bf1WinHandle}");
                AddRunningLog($"进程句柄：{Memory.Bf1ProHandle}");
                AddRunningLog($"主显示器屏幕分辨率：{PrimaryScreenWidth} x {PrimaryScreenHeight}");

                // 创建文件夹
                Directory.CreateDirectory(FileUtil.D_Config_Path);
                Directory.CreateDirectory(FileUtil.D_Log_Path);
                // 如果配置文件不存在则创建
                if (!File.Exists(FileUtil.F_Deploy_Path))
                {
                    foreach (var item in MapData.AllMapInfo)
                    {
                        if (item.Chinese == "大厅菜单")
                            continue;

                        DeployConfigs.Add(new DeployConfig()
                        {
                            MapName = item.Chinese,
                            DeployX = 0,
                            DeployY = 0
                        });
                    }

                    File.WriteAllText(FileUtil.F_Deploy_Path, JsonUtil.JsonSeri(DeployConfigs));
                }
                // 如果配置文件存在则读取
                if (File.Exists(FileUtil.F_Deploy_Path))
                {
                    using (var streamReader = new StreamReader(FileUtil.F_Deploy_Path))
                    {
                        DeployConfigs = JsonUtil.JsonDese<List<DeployConfig>>(streamReader.ReadToEnd());

                        var index = DeployConfigs.FindIndex(var => var.MapName == MainModel.CurrentMapName);
                        if (index != -1)
                        {
                            MainModel.GameDeployX = DeployConfigs[index].DeployX;
                            MainModel.GameDeployY = DeployConfigs[index].DeployY;
                        }
                    }
                }
            }
            else
            {
                AddRunningLog("战地1内存模块初始化失败，请重启程序");
                Task.Delay(2000).Wait();
                this.Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown();
                    return;
                });
            }
        });

        InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");

        // 后台更新线程
        var thread1 = new Thread(UpdateData)
        {
            IsBackground = true
        };
        thread1.Start();

        // 后台运行机器人线程
        var thread2 = new Thread(RunFunBot)
        {
            IsBackground = true
        };
        thread2.Start();

        var timerNoAFK = new System.Timers.Timer()
        {
            AutoReset = true,
            Interval = TimeSpan.FromSeconds(30).TotalMilliseconds
        };
        timerNoAFK.Elapsed += TimerNoAFK_Elapsed;
        timerNoAFK.Start();

        HotKeys.AddKey(WinVK.F5);
        HotKeys.AddKey(WinVK.F9);
        HotKeys.AddKey(WinVK.F10);
        HotKeys.KeyDownEvent += MainHotKeys_KeyDownEvent;
    }

    private void Window_Main_Closing(object sender, CancelEventArgs e)
    {
        Memory.CloseHandle();
        HotKeys.Dispose();

        File.WriteAllText(FileUtil.F_Deploy_Path, JsonUtil.JsonSeri(DeployConfigs));
    }

    /// <summary>
    /// 输出运行日志
    /// </summary>
    /// <param name="logMsg">日志内容</param>
    private void AddRunningLog(string logMsg)
    {
        this.Dispatcher.Invoke(() =>
        {
            if (TextBox_RunningLog.LineCount >= 1000)
                TextBox_RunningLog.Clear();

            TextBox_RunningLog.AppendText($"[{DateTime.Now:MM/dd HH:mm:ss}] {logMsg}\r\n");
            TextBox_RunningLog.ScrollToEnd();
        });
    }

    private void MainHotKeys_KeyDownEvent(int Id, string Name)
    {
        switch (Id)
        {
            case (int)WinVK.F5:
                RecordGameDeployPoint();
                break;
            case (int)WinVK.F9:
                MainModel.IsRunFunBot = !MainModel.IsRunFunBot;
                if (MainModel.IsRunFunBot)
                    Console.Beep(700, 75);
                else
                    Console.Beep(600, 75);
                break;
            case (int)WinVK.F10:
                Console.Beep(400, 75);
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
            WinAPI.GetCursorPos(out var point);
            MainModel.ScreenMouseX = point.X;
            MainModel.ScreenMouseY = point.Y;

            // 矩阵信息
            var view_offset = Memory.Read<long>(Offsets.OFFSET_GAMERENDERER, new int[] { 0x60 });
            var view_matrix4x4 = Memory.Read<Matrix4x4>(view_offset + 0x460);

            MainModel.GameCameraX = view_matrix4x4.M41;
            MainModel.GameCameraY = view_matrix4x4.M42;
            MainModel.GameCameraZ = view_matrix4x4.M44;

            var baseAddress = Player.GetLocalPlayer();
            // 判断玩家是否死亡
            IsPlayerDeath = Memory.Read<long>(baseAddress + 0x1D48) == 0;

            // 服务器地图名称
            var mapName = Memory.ReadString(Offsets.OFFSET_CLIENTGAMECONTEXT, Offsets.ServerMapName, 64);
            MainModel.CurrentMapName = PlayerUtil.GetMapChsName(mapName);
            MainModel.CurrentMapImage = PlayerUtil.GetMapPrevImage(mapName);

            MapCameraZ = PlayerUtil.GetMapCameraZ(mapName);
            if (MapCameraZ == 0)
            {
                MainModel.IsRunFunBot = false;
                MainModel.IsFunBotEnable = false;
            }
            else
            {
                MainModel.IsFunBotEnable = true;
            }

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
                    if (!isShiftW)
                    {
                        WinAPI.Keybd_Event(WinVK.W, WinAPI.MapVirtualKey(WinVK.W, 0), 0, 0);
                        WinAPI.Keybd_Event(WinVK.LSHIFT, WinAPI.MapVirtualKey(WinVK.LSHIFT, 0), 0, 0);
                        isShiftW = true;
                    }

                    // 玩家在部署界面
                    while (MainModel.IsRunFunBot && m_isTopWindow && IsPlayerDeath && MainModel.GameCameraZ > MapCameraZ)
                    {
                        AddRunningLog("玩家处于部署界面");

                        AddRunningLog("尝试移动鼠标到自定义游戏部署点");
                        WinAPI.SetCursorPos(MainModel.GameDeployX, MainModel.GameDeployY);
                        Thread.Sleep(200);

                        // 判断鼠标是否到了指定位置
                        WinAPI.GetCursorPos(out var point);
                        if (point.X == MainModel.GameDeployX && point.Y == MainModel.GameDeployY)
                        {
                            Thread.Sleep(1000);
                            AddRunningLog("尝试按下鼠标左键");
                            WinAPI.Mouse_Event(MouseEventFlag.LeftDown, 0, 0, 0, 0);
                            Thread.Sleep(200);
                            WinAPI.Mouse_Event(MouseEventFlag.LeftUp, 0, 0, 0, 0);

                            Thread.Sleep(1000);
                            AddRunningLog("尝试按下空格键");
                            WinAPI.Keybd_Event(WinVK.SPACE, WinAPI.MapVirtualKey(WinVK.SPACE, 0), 0, 0);
                            Thread.Sleep(200);
                            WinAPI.Keybd_Event(WinVK.SPACE, WinAPI.MapVirtualKey(WinVK.SPACE, 0), 2, 0);
                        }
                    }

                    // 玩家死亡
                    while (MainModel.IsRunFunBot && m_isTopWindow && IsPlayerDeath && MainModel.GameCameraZ < MapCameraZ)
                    {
                        AddRunningLog("玩家死亡");

                        Thread.Sleep(1000);
                        AddRunningLog("尝试按下空格键");
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

    private void TimerNoAFK_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (MainModel.IsRunNoAFK)
        {
            Memory.SetForegroundWindow();
            Thread.Sleep(50);

            WinAPI.Keybd_Event(WinVK.TAB, WinAPI.MapVirtualKey(WinVK.TAB, 0), 0, 0);
            Thread.Sleep(3000);
            WinAPI.Keybd_Event(WinVK.TAB, WinAPI.MapVirtualKey(WinVK.TAB, 0), 2, 0);
            Thread.Sleep(50);
        }
    }

    /// <summary>
    /// 记录游戏部署点坐标
    /// </summary>
    private void RecordGameDeployPoint()
    {
        var index = DeployConfigs.FindIndex(var => var.MapName == MainModel.CurrentMapName);
        if (index != -1)
        {
            MainModel.GameDeployX = MainModel.ScreenMouseX;
            MainModel.GameDeployY = MainModel.ScreenMouseY;

            DeployConfigs[index].DeployX = MainModel.GameDeployX;
            DeployConfigs[index].DeployY = MainModel.GameDeployY;

            File.WriteAllText(FileUtil.F_Deploy_Path, JsonUtil.JsonSeri(DeployConfigs));

            Console.Beep(500, 75);
        }
    }
}
