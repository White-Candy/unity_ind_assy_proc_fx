using Cysharp.Threading.Tasks;
using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ModelAction : BaseAction
{
    private ModelPanel m_Panel;

    public List<string> m_PartNames = new List<string>();

    private ModelMgr m_Mgr;

    private GameObject m_currModel;

    public ModelAction()
    {
        m_Panel = UIConsole.FindAssetPanel<ModelPanel>();

        m_Token = new CancellationTokenSource();
        m_panelToken = new CancellationTokenSource();
    }

    public override async UniTask AsyncShow(string name)
    {
        //m_Panel = UITools.FindAssetPanel<ModelPanel>();
        LoadModel();
        await UniTask.Yield();
        m_Panel.gameObject.SetActive(true);

        try
        { 
            await UniTask.WaitUntil(() => m_Panel?.m_Content.activeSelf == false);
        }
        catch 
        {

        }
    }

    public override void Exit(Action callback)
    {
        // Debug.Log("Model Exit");
        base.Exit(callback);

        if (m_currModel)
        {
            GameObject.Destroy(m_currModel);
        }
        // GameObject.Destroy(m_Panel);
        // GameObject.Destroy(m_Mgr);
        //CameraMovementController.Instance.Clear();
        m_Panel.Exit();
        m_Panel.Active(false);
    }

    private async void LoadModel()
    {
        await UniTask.WaitUntil(() => 
        {
            return Addressables.LoadAssetAsync<GameObject>(GlobalData.ProjGroupName).IsDone == true;
        });

        Addressables.LoadAssetAsync<GameObject>(GlobalData.ProjGroupName).Completed += (handle) =>
        {
            GameObject go = handle.Result;
            if (go != null)
            {
                m_currModel = GameObject.Instantiate(go);
                ModelMgr mgr = m_currModel.AddComponent<ModelMgr>();
                m_Mgr = mgr;
                m_PartNames = mgr.GetPartNameList();

                m_Panel.onClick = OnClickPart;
                m_Panel.onClickRevert = onClickRevert;
                m_Panel.Init(m_PartNames);
                m_Panel.transform.SetAsFirstSibling();
                m_Panel.Active(true);

                CameraMovementController.Instance.UpdateData(m_currModel.transform);
            }
        };
    }

    public void OnClickPart(string name)
    {
        m_Mgr.ChangeMaterial(name);
    }

    public void onClickRevert()
    {
        CameraMovementController.Instance.ResetData();
        m_Mgr.Revert();
    }

}
