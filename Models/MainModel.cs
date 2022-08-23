using CommunityToolkit.Mvvm.ComponentModel;

namespace BF1.FunBot.Models;

public class MainModel : ObservableObject
{
    ////////////////////////////////////////

    private bool _isFunBotEnable;
    /// <summary>
    /// 是否启用机器人
    /// </summary>
    public bool IsFunBotEnable
    {
        get => _isFunBotEnable;
        set => SetProperty(ref _isFunBotEnable, value);
    }

    private bool _isRunFunBot;
    /// <summary>
    /// 是否运行机器人
    /// </summary>
    public bool IsRunFunBot
    {
        get => _isRunFunBot;
        set => SetProperty(ref _isRunFunBot, value);
    }

    private string _currentMapImage;
    /// <summary>
    /// 当前地图图片
    /// </summary>
    public string CurrentMapImage
    {
        get => _currentMapImage;
        set => SetProperty(ref _currentMapImage, value);
    }

    private string _currentMapName;
    /// <summary>
    /// 当前地图名称
    /// </summary>
    public string CurrentMapName
    {
        get => _currentMapName;
        set => SetProperty(ref _currentMapName, value);
    }

    ////////////////////////////////////////

    private float _gameCameraX;
    /// <summary>
    /// 游戏相机坐标X
    /// </summary>
    public float GameCameraX
    {
        get => _gameCameraX;
        set => SetProperty(ref _gameCameraX, value);
    }

    private float _gameCameraY;
    /// <summary>
    /// 游戏相机坐标Y
    /// </summary>
    public float GameCameraY
    {
        get => _gameCameraY;
        set => SetProperty(ref _gameCameraY, value);
    }

    private float _gameCameraZ;
    /// <summary>
    /// 游戏相机坐标Z
    /// </summary>
    public float GameCameraZ
    {
        get => _gameCameraZ;
        set => SetProperty(ref _gameCameraZ, value);
    }

    ////////////////////////////////////////

    private int _screenMouseX;
    /// <summary>
    /// 屏幕鼠标坐标X
    /// </summary>
    public int ScreenMouseX
    {
        get => _screenMouseX;
        set => SetProperty(ref _screenMouseX, value);
    }

    private int _screenMouseY;
    /// <summary>
    /// 屏幕鼠标坐标Y
    /// </summary>
    public int ScreenMouseY
    {
        get => _screenMouseY;
        set => SetProperty(ref _screenMouseY, value);
    }

    ////////////////////////////////////////

    private int _gameDeployX;
    /// <summary>
    /// 游戏部署点坐标X
    /// </summary>
    public int GameDeployX
    {
        get => _gameDeployX;
        set => SetProperty(ref _gameDeployX, value);
    }

    private int _gameDeployY;
    /// <summary>
    /// 游戏部署点坐标Y
    /// </summary>
    public int GameDeployY
    {
        get => _gameDeployY;
        set => SetProperty(ref _gameDeployY, value);
    }

    ////////////////////////////////////////
}
