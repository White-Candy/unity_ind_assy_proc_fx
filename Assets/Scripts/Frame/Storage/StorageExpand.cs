using Cysharp.Threading.Tasks;
using LitJson;
using System.IO;
using UnityEngine;

public static class StorageExpand
{
    private static bool m_Init = false;

    private static StorageObject m_storage;
    public static StorageObject Storage
    {
        get
        {
            if (m_storage == null)
            {
                m_storage = Resources.Load("Storage/Clump") as StorageObject;
            }

            if (File.Exists(Application.streamingAssetsPath + "\\CliStorage.json") && !m_Init)
            {
                string s_json = File.ReadAllText(Application.streamingAssetsPath + "\\CliStorage.json");
                m_storage = JsonMapper.ToObject<StorageObject>(s_json);
                m_Init = true;
            }
            return m_storage;
        }
    }

    /// <summary>
    /// 查找对应条件的 ResourcesInfo
    /// </summary>
    /// <param name="code"></param>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    public static ResourcesInfo FindRsInfo(string relative)
    {
        ResourcesInfo info = Storage.rsCheck.Find((x) => x.relaPath == relative);
        if (info == null)
        {
            info = new ResourcesInfo() { relaPath = relative, version_code = Tools.SpawnRandomCode() };
            Storage.rsCheck.Add(info);
        }

        return info;
    }

    /// <summary>
    /// 检查客户端的资源版本是否时最新的
    /// </summary>
    /// <param name="cli_info"></param>
    public static ResourcesInfo GetThisInfoPkg(ResourcesInfo cli_info)
    {
        return Storage.rsCheck.Find((x) => { return x.relaPath == cli_info.relaPath; });
    }

    /// <summary>
    /// 保存这个文件的版本信息
    /// </summary>
    /// <param name="relative"></param>
    public static void UpdateThisFileInfo(ResourcesInfo info)
    {
        int idx = Storage.rsCheck.FindIndex((x) => { return x.relaPath == info.relaPath; });
        if (idx != -1)
        {
            Storage.rsCheck.RemoveAt(idx);
        }

        ResourcesInfo ri = new ResourcesInfo();
        ri.relaPath = info.relaPath;
        ri.version_code = info.version_code;
        Storage.rsCheck.Add(ri);
        SaveToDisk();
    }

    /// <summary>
    /// 将 ScriptableObject 的内容保存到硬盘中
    /// </summary>
    public async static void SaveToDisk()
    {
        await UniTask.SwitchToMainThread();
        string s_json = JsonMapper.ToJson(Storage);
        File.WriteAllText(Application.streamingAssetsPath + "/CliStorage.json", s_json);
    }
}
