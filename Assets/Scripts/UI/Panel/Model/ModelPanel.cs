using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ModelPanel : BasePanel
{

    public GameObject m_EquiItem; // 组件按钮

    public Transform m_EquiItemParent; // 父组件

    public TextMeshProUGUI m_DescriptionText; // 图标

    public Button m_RevertBtn; // 重置按钮

    public Action<string> onClickPart = (str) => { };

    public Action onClickRevert = () => { };

    public void Init(List<string> name)
    {
        SpawnItem(name);
        m_RevertBtn.onClick.AddListener(() => { onClickRevert?.Invoke(); });
    }

    public void SpawnItem(List<string> items)
    {
        foreach (var item in items)
        {
            //Debug.Log("====================SpawnItem: " + item);
            GameObject go = GameObject.Instantiate(m_EquiItem, m_EquiItemParent);
            go.gameObject.SetActive(true);

            Sprite spr = Resources.Load<Sprite>($"Textures/Tools/{item}");
            if (null == spr)
            {
                spr = Resources.Load<Sprite>("Textures/NotFoundTips/NotFoundImage");
            }

            go.GetComponent<Image>().sprite = spr;
            go.GetComponentInChildren<TextMeshProUGUI>().text = item;
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetDescription(item);
                onClickPart?.Invoke(item);
            });
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
}
