using BF1.FunBot.Features.Client;

namespace BF1.FunBot.Features.Utils;

public static class PlayerUtil
{
    /// <summary>
    /// 获取地图对应中文名称
    /// </summary>
    /// <param name="originMapName"></param>
    /// <returns></returns>
    public static string GetMapChsName(string originMapName)
    {
        var index = MapData.AllMapInfo.FindIndex(var => var.English == originMapName);
        if (index != -1)
            return MapData.AllMapInfo[index].Chinese;
        else
            return originMapName;
    }

    /// <summary>
    /// 获取地图对应预览图
    /// </summary>
    /// <param name="originMapName"></param>
    /// <returns></returns>
    public static string GetMapPrevImage(string originMapName)
    {
        var index = MapData.AllMapInfo.FindIndex(var => var.English == originMapName);
        if (index != -1)
            return MapData.AllMapInfo[index].Image;
        else
            return "";
    }

    /// <summary>
    /// 获取对应地图相机Z高度
    /// </summary>
    /// <param name="originMapName"></param>
    /// <returns></returns>
    public static float GetMapCameraZ(string originMapName)
    {
        var index = MapData.AllMapInfo.FindIndex(var => var.English == originMapName);
        if (index != -1)
            return MapData.AllMapInfo[index].CameraZ;
        else
            return 0f;
    }
}
