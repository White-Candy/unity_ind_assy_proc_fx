using Cysharp.Threading.Tasks;
using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ModelAction : BaseAction
{
    private ModelPanel m_Panel;

    public List<string> m_PartNames = new List<string>();

    private ModelMgr m_Mgr;

    public override async UniTask AsyncShow(string name)
    {
        m_Panel = UITools.FindAssetPanel<ModelPanel>();
        LoadModel();
        await UniTask.Yield();
        m_Panel.gameObject.SetActive(true);
    }

    public override void Exit()
    {
        GameObject.Destroy(m_Panel);
        GameObject.Destroy(m_Mgr.gameObject);
        CameraMovementController.Instance.Clear();
    }

    private async void LoadModel()
    {
        await UniTask.WaitUntil(() => 
        {
            return Addressables.LoadAssetAsync<GameObject>(GlobalData.currModuleCode).IsDone == true;
        });

        Addressables.LoadAssetAsync<GameObject>(GlobalData.currModuleCode).Completed += (handle) =>
        {
            GameObject go = handle.Result;
            if (go != null)
            {
                GameObject model = GameObject.Instantiate(go);
                ModelMgr mgr = model.AddComponent<ModelMgr>();
                m_Mgr = mgr;
                m_PartNames = mgr.GetPartNameList();

                m_Panel.onClickPart = OnClickPart;
                m_Panel.onClickRevert = onClickRevert;
                m_Panel.Init(m_PartNames);
                m_Panel.transform.SetAsFirstSibling();
                m_Panel.Active(true);

                CameraMovementController.Instance.UpdateData(model.transform);
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
