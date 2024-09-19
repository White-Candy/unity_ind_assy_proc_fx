using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using sugar;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 当前3D模型窗口是展现 ‘展示’ 功能，还是 ‘构造’ 功能
/// </summary>
public enum DisPlayType
{
    STRUCTURE,
    DISPLAY,
    NONE
}

public class ModelPanel : BasePanel
{

    public GameObject m_EquiItem; // 组件按钮

    public Transform m_EquiItemParent; // 父组件

    public TextMeshProUGUI m_DescriptionText; // 图标

    public Button m_RevertBtn; // 重置按钮

    public Action<string> onClick = (str) => { };

    public Action onClickRevert = () => { };

    public DisPlayType m_type = DisPlayType.NONE;

    // 工具面板的Item存储
    private List<GameObject> m_list = new List<GameObject>();

    public void Init(List<string> name, DisPlayType type = DisPlayType.STRUCTURE)
    {
        SpawnItem(name);
        m_type = type;
        if (type == DisPlayType.DISPLAY)
        {
            m_RevertBtn.gameObject.SetActive(false);
        }
        else
        {
            m_RevertBtn.gameObject.SetActive(true);
            m_RevertBtn.onClick.AddListener(() => { onClickRevert?.Invoke(); });
        }
    }

    public void SpawnItem(List<string> items)
    {
        foreach (var item in items)
        {
            //Debug.Log("====================SpawnItem: " + item);
            GameObject go = GameObject.Instantiate(m_EquiItem, m_EquiItemParent);
            go.gameObject.SetActive(true);

            Sprite spr = Resources.Load<Sprite>($"Textures/Tools/{GlobalData.courseName}\\{item}");
            if (null == spr)
            {
                spr = Resources.Load<Sprite>("Textures/NotFoundTips/NotFoundImage");
            }

            go.GetComponent<Image>().sprite = spr;
            go.GetComponentInChildren<TextMeshProUGUI>().text = item;
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetDescription(item);
                onClick?.Invoke(item);
            });

            m_list.Add(go);
        }
    }

    public void SetDescription(string name)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Word/Tools/{name}");
        if (null == textAsset)
        {
            m_DescriptionText.text = "描述文件丢失，请联系后台管理员";
        }
        else
        {
            m_DescriptionText.text = textAsset.text;
        }
    }

    public void Exit()
    {
        if (null != this)
        {
            foreach (var item in m_list)
            {
                item?.SetActive(false);
                Destroy(item);
            }
            m_list.Clear();
            m_list = new List<GameObject>();
        }
    }
}
