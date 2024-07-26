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

            if (File.Exists(Application.streamingAssetsPath + "Storage.json") && !m_Init)
            {
                string s_json = File.ReadAllText(Application.persistentDataPath + "/Data.json");
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
    public static ResourcesInfo FindRsInfo(string code, string moduleName, string relative)
    {
        ResourcesInfo info = Storage.rsCheck.Find((x) => x.id == code && x.moduleName == moduleName);
        return info == null ? new ResourcesInfo() { id = code, moduleName = moduleName, relaPath = relative } : info;
    }

    /// <summary>
    /// 检查客户端的资源版本是否时最新的
    /// </summary>
    /// <param name="cli_info"></param>
    public static ResourcesInfo GetThisInfoPkg(ResourcesInfo cli_info)
    {
        return Storage.rsCheck.Find((x) => { return (x.id == cli_info.id) && (x.moduleName == cli_info.moduleName); });
    }

    /// <summary>
    /// 保存这个文件的版本信息
    /// </summary>
    /// <param name="relative"></param>
    public static void UpdateThisFileInfo(ResourcesInfo info)
    {
        int idx = Storage.rsCheck.FindIndex((x) => { return (x.id == info.id) && (x.moduleName == info.moduleName); });
        if (idx != -1)
        {
            Storage.rsCheck.RemoveAt(idx);
        }

        ResourcesInfo ri = new ResourcesInfo();
        ri.id = info.id;
        ri.moduleName = info.moduleName;
        ri.version_code = info.version_code;
        Storage.rsCheck.Add(ri);
        SaveToDisk();
    }

    /// <summary>
    /// 将 ScriptableObject 的内容保存到硬盘中
    /// </summary>
    public static void SaveToDisk()
    {
        // Debug.Log(Application.persistentDataPath);
        string s_json = JsonMapper.ToJson(Storage);
        File.WriteAllText(Application.persistentDataPath + "/Storage.json", s_json);
    }
}
