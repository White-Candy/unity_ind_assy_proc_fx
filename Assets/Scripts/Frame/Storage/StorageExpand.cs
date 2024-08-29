using Cysharp.Threading.Tasks;
using LitJson;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class StorageExpand
{
    private static bool m_Init = false;

    private static StorageObject m_storage;
    public static StorageObject Storage
    {
        get
        {
            getStorage();
            return m_storage;
        }
    }

    public async static void getStorage()
    {
        await UniTask.SwitchToMainThread();
        if (m_storage == null)
        {
            m_storage = Resources.Load("Storage/Clump") as StorageObject;
        }

        if (File.Exists(Application.persistentDataPath + "\\CliStorage.json") && !m_Init)
        {
            string s_json = File.ReadAllText(Application.persistentDataPath + "\\CliStorage.json");
            JsonUtility.FromJsonOverwrite(s_json, m_storage);
            m_Init = true;
        }
        await UniTask.WaitUntil(() => { return m_storage != null; } );
    }

    /// <summary>
    /// ���Ҷ�Ӧ������ ResourcesInfo
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
    /// ���ͻ��˵���Դ�汾�Ƿ�ʱ���µ�
    /// </summary>
    /// <param name="cli_info"></param>
    public static ResourcesInfo GetThisInfoPkg(ResourcesInfo cli_info)
    {
        return Storage.rsCheck.Find((x) => { return x.relaPath == cli_info.relaPath; });
    }

    /// <summary>
    /// ��������ļ��İ汾��Ϣ
    /// </summary>
    /// <param name="relative"></param>
    public static void UpdateThisFileInfo(ResourcesInfo info)
    {
        int idx = Storage.rsCheck.FindIndex((x) => { return x.relaPath == info.relaPath; });
        if (idx != -1)
        {
            Storage.rsCheck[idx].version_code = info.version_code;
            Storage.rsCheck[idx].need_updata = false;
        }
        else
        {
            ResourcesInfo ri = new ResourcesInfo
            {
                relaPath = info.relaPath,
                version_code = info.version_code
            };
            Storage.rsCheck.Add(ri);
        }
        
        SaveToDisk();
    }

    /// <summary>
    /// �� ScriptableObject �����ݱ��浽Ӳ����
    /// </summary>
    public async static void SaveToDisk()
    {
        await UniTask.SwitchToMainThread();
        string s_json = JsonMapper.ToJson(Storage);
        File.WriteAllText(Application.persistentDataPath + "/CliStorage.json", s_json);
    }
}
