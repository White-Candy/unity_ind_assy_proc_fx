using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssetConsole : Singleton<AssetConsole>
{
    public Dictionary<string, Object> m_List = new Dictionary<string, Object>(); // ��Դ�б�

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// �����Դ�Ƿ��Ѿ����ص��ڴ�����
    /// </summary>
    /// <param name="info"></param>
    /// <param name="reslut"></param>
    private void CheckAssetLoadInMemory(string[] paths, AsyncResult result)
    {
        DontDestroyOnLoad(gameObject);

        bool is_load = false;
        foreach (string path in paths)
        {
            if (m_List.ContainsKey(path))
            {
                result.m_Assets[path] = m_List[path];
            }
            else
            {
                is_load = false;
            }
        }
        result.isLoad = is_load;
    }

    /// <summary>
    /// ����ͼƬ��ʵ�����ڴ���
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    public async UniTask<AsyncResult> LoadTexObject(string[] paths)
    {
        AsyncResult result = new AsyncResult();
        CheckAssetLoadInMemory(paths, result);

        if (!result.isLoad)
            await LoadTexture(paths, result);

        return result;
    }

    /// <summary>
    /// ��������ͼ����Դ
    /// </summary>
    /// <param name="paths"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private async UniTask LoadTexture(string[] paths, AsyncResult result)
    {
        foreach (string path in paths)
        {
            if (result.m_Assets.ContainsKey(path))
            {
                continue;
            }

            using (UnityWebRequest req = new UnityWebRequest(path))
            {
                req.downloadHandler = new DownloadHandlerTexture();
                await req.SendWebRequest();

                await UniTask.WaitUntil(() => req.isDone == true);

                if (!string.IsNullOrEmpty(req.error))
                {
                    continue;
                }

                //�ṹ����
                result.m_Assets[path] = DownloadHandlerTexture.GetContent(req);

                //����
                m_List.Add(path, DownloadHandlerTexture.GetContent(req));

                await UniTask.Yield();
            }
        }
        result.isLoad = true;
    }
}

public class AsyncResult
{
    public bool isLoad; // �Ƿ���ص��ڴ�����

    public Dictionary<string, Object> m_Assets = new Dictionary<string, Object>(); // ��Դ�ֵ�
}
