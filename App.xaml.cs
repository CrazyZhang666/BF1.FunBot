using BF1.FunBot.Common.Utils;

namespace BF1.FunBot;

/// <summary>
/// App.xaml 的交互逻辑
/// </summary>
public partial class App : Application
{
    public static Mutex AppMainMutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        AppMainMutex = new Mutex(true, ResourceAssembly.GetName().Name, out var createdNew);

        if (createdNew)
        {
            // 如果战地1运行则启动程序
            if (ProcessUtil.IsBf1Run())
            {
                base.OnStartup(e);
            }
            else
            {
                MsgBoxUtil.Warning("未发现《战地1》进程！\n请先启动《战地1》游戏后，再打开本程序");
                Current.Shutdown();
            }
        }
        else
        {
            MessageBox.Show("请不要重复打开，程序已经运行\n如果一直提示，请到\"任务管理器-详细信息（win7为进程）\"里结束本程序",
                "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            Current.Shutdown();
        }
    }
}
