namespace BF1.FunBot.Features.Client;

public static class MapData
{
    public struct MapName
    {
        public string English;
        public string Chinese;
        public float CameraZ;
    }

    /// <summary>
    /// 地图数据
    /// </summary>
    public readonly static List<MapName> AllMapInfo = new()
    {
        new() { English="ID_M_LEVEL_MENU", Chinese="大厅菜单", CameraZ=0 },
        new() { English="ID_M_MP_LEVEL_MOUNTAINFORT", Chinese="格拉巴山", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_FOREST", Chinese="阿尔贡森林", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_ITALIANCOAST", Chinese="帝国边境", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_CHATEAU", Chinese="流血宴厅", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_SCAR", Chinese="圣康坦的伤痕", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_DESERT", Chinese="西奈沙漠", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_AMIENS", Chinese="亚眠", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_SUEZ", Chinese="苏伊士", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_FAOFORTRESS", Chinese="法欧堡", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_GIANT", Chinese="庞然暗影", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_FIELDS", Chinese="苏瓦松", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_GRAVEYARD", Chinese="决裂", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_UNDERWORLD", Chinese="法乌克斯要塞", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_VERDUN", Chinese="凡尔登高地", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_TRENCH", Chinese="尼维尔之夜", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_SHOVELTOWN", Chinese="攻占托尔", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_BRIDGE", Chinese="勃鲁希洛夫关口", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_ISLANDS", Chinese="阿尔比恩", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_RAVINES", Chinese="武普库夫山口", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_VALLEY", Chinese="加利西亚", CameraZ=520 },
        new() { English="ID_M_MP_LEVEL_TSARITSYN", Chinese="察里津", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_VOLGA", Chinese="窝瓦河", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_BEACHHEAD", Chinese="海丽丝岬", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_HARBOR", Chinese="泽布吕赫", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_NAVAL", Chinese="黑尔戈兰湾", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_RIDGE", Chinese="阿奇巴巴", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_OFFENSIVE", Chinese="索姆河", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_HELL", Chinese="帕斯尚尔", CameraZ=450 },
        new() { English="ID_M_MP_LEVEL_RIVER", Chinese="卡波雷托", CameraZ=450 }
    };
}
