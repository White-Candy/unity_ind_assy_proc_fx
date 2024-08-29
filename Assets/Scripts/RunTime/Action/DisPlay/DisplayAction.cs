using Cysharp.Threading.Tasks;
using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DisplayAction : BaseAction
{
    public List<string> m_ModelName = new List<string>();

    public Dictionary<string, GameObject> m_ModelHandles = new Dictionary<string, GameObject>();

    public ModelPanel m_Panel;

    private GameObject m_currModel; // 当前展示的模型
    public DisplayAction()
    {
        m_Panel = UIConsole.Instance.FindAssetPanel<ModelPanel>();
        m_Token = new CancellationTokenSource();
    }

    public override async UniTask AsyncShow(string name)
    {
        await NetworkManager._Instance.DownLoadTextFromServer(FPath.AssetRootPath + GlobalData.ProjGroupName + FPath.ModelSuffix, 
            (names) => 
            {
                string[] arr_name = names.Split('_');
                foreach (string name in arr_name)
                {
                    // Debug.Log(name);
                    m_ModelName.Add(name);
                }
            });

        if (m_ModelName.Count == 0)
        {
            UITools.ShowMessage("配置文件没有正确的建材名字!");
            return;
        }

        foreach (var n in m_ModelName)
        {
            var go = await Addressables.LoadAssetAsync<GameObject>(n);
            if (!m_ModelHandles.ContainsKey(n))
            {
                m_ModelHandles.Add(n, go);
            }
            else
            {
                m_ModelHandles[n] = go;
            }
        }

        m_Panel.onClick = OnClickToolItem;
        OnClickToolItem(m_ModelHandles.First().Key); // 默认出现第一个模型

        m_Panel.Init(m_ModelName, DisPlayType.DISPLAY);
        m_Panel.transform.SetAsFirstSibling();
        m_Panel.Active(true);


        try
        {
            await UniTask.WaitUntil(() => m_Panel?.m_Content.activeSelf == false);
        }
        catch
        {

        }
    }

    private void OnClickToolItem(string name)
    {
        if (m_currModel != null)
        {
            GameObject.Destroy(m_currModel);
        }

        m_currModel = GameObject.Instantiate(m_ModelHandles[name]);
        m_currModel.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);

        CameraMovementController.Instance.UpdateData(m_currModel.transform);
    }

    public override void Exit(Action callback)
    {
        // Debug.Log("DisPlay Exit");
        base.Exit(callback);

        if (m_currModel)
        {
            GameObject.Destroy(m_currModel);
        }
        m_Panel.Exit();
        m_Panel.Active(false);
        m_ModelName.Clear();
    }
}
