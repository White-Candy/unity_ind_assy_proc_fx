
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPath
{
    private static string assetRootPath;
    public static string AssetRootPath
    {
        get
        {
            assetRootPath = Application.streamingAssetsPath + "/Data/";
            return assetRootPath;
        }
        set
        {
            assetRootPath = value;
        }
    }

    /// <summary>
    /// 教学模式中 教案模式config文件路径
    /// </summary>
    public static string JiaoAnSuffix { get { return $"\\JiaoAn"; } }
    public static string GuiFanSuffix { get { return $"\\GuiFan"; } }
    public static string FangAnSuffix { get { return $"\\FangAn"; } }
    public static string TuZhiSuffix { get { return $"\\TuZhi"; } }
    public static string PictureSuffix { get { return $"\\Picture"; } }
    public static string AnimSuffix { get { return $"\\Animation"; } }
    public static string VideoSuffix { get { return $"\\Video"; } }
    public static string ModelSuffix { get { return $"\\Model\\Config.txt"; } }
    public static string DialogBlue { get { return $"Textures/NewUI/Examine/DialogBlueButton"; } }
    public static string DialogWhite { get { return $"Textures/NewUI/Examine/DialogWhiteButton"; } }
    public static string TeachingRoot { get { return $"Textures/NewUI/Teaching"; } }
}
