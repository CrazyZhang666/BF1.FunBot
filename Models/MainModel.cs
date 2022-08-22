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

    private string _playerState;
    /// <summary>
    /// 玩家状态
    /// </summary>
    public string PlayerState
    {
        get => _playerState;
        set => SetProperty(ref _playerState, value);
    }

    ////////////////////////////////////////

    private int _gameResWidth;
    /// <summary>
    /// 游戏分辨率宽度
    /// </summary>
    public int GameResWidth
    {
        get => _gameResWidth;
        set => SetProperty(ref _gameResWidth, value);
    }

    private int _gameResHeight;
    /// <summary>
    /// 游戏分辨率高度
    /// </summary>
    public int GameResHeight
    {
        get => _gameResHeight;
        set => SetProperty(ref _gameResHeight, value);
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

    private int _gameMouseX;
    /// <summary>
    /// 游戏鼠标坐标X
    /// </summary>
    public int GameMouseX
    {
        get => _gameMouseX;
        set => SetProperty(ref _gameMouseX, value);
    }

    private int _gameMouseY;
    /// <summary>
    /// 游戏鼠标坐标Y
    /// </summary>
    public int GameMouseY
    {
        get => _gameMouseY;
        set => SetProperty(ref _gameMouseY, value);
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
