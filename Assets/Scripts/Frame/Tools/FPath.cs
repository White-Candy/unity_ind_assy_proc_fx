using sugar;
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
            assetRootPath = Application.streamingAssetsPath;
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
    public static string JiaoAnPath { get { return $"{AssetRootPath}/Data/{GlobalData.currModuleCode}/JiaoAn"; } }
}
