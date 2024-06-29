using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public struct HighlightModel
{
    public GameObject trans;
    public ModelPartHighlightCol col;
    public ModelPartMatCol mat_col;
}

/// <summary>
/// 模型管理
/// 用来控制构造子模式模型的子物体变化
/// </summary>
public class ModelMgr : MonoBehaviour
{
    private Dictionary<string, HighlightModel> m_PartDic = new Dictionary<string, HighlightModel>();

    public List<string> GetPartNameList()
    {
        foreach (Transform transf in transform.Find("Parts"))
        {
            //Debug.Log("=========GetPartNameList: " + transf.name);

            HighlightModel hlm = new HighlightModel();
            ModelPartHighlightCol col = transf.AddComponent<ModelPartHighlightCol>();
            ModelPartMatCol mat_col = transf.AddComponent<ModelPartMatCol>();

            hlm.trans = transf.gameObject;
            hlm.col = col;
            hlm.mat_col = mat_col;

            m_PartDic.Add(transf.name, hlm);
        }

        //AddModelPartMatCol(this.transform);
        return m_PartDic.Keys.ToList();
    }

    private void AddModelPartMatCol(Transform transf)
    {
        foreach (Transform item in transf.Find("Parts"))
        {
            if (item.GetComponent<MeshRenderer>() != null)
            {
                ModelPartMatCol col = item.gameObject.AddComponent<ModelPartMatCol>();
                //Debug.Log("=========AddModelPartMatCol: " + item.name);
                if (m_PartDic.ContainsKey(transf.name))
                {
                    HighlightModel hlm = new HighlightModel();
                    hlm = m_PartDic[item.name];
                    hlm.mat_col = col;
                    m_PartDic[transf.name] = hlm;
                }
            }
            else
            {
                AddModelPartMatCol(item);
            }
        }
    }

    public void ChangeMaterial(string name)
    {
        // 设置为透明
        // 关闭所有的高亮
        foreach (var p in m_PartDic)
        {
            p.Value.mat_col.Transparent();
            p.Value.col.OffHighlight();
        }

        HighlightModel go;
        m_PartDic.TryGetValue(name, out go);
        go.trans.GetComponent<ModelPartMatCol>().Revert();
        go.trans.GetComponent<ModelPartHighlightCol>().OnHighlight();
        CameraMovementController.Instance.UpdateData(go.trans != null ? go.trans.transform : null);
    }

    public void Revert()
    {
        foreach (var p in m_PartDic.Values)
        {
            p.mat_col.Revert();
            p.col.OnHighlight();
        }
    }
}
