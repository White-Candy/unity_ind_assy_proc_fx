using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static EPOOutline.TargetStateListener;
//using UnityEngine.UIElements;

public class DialogPanel : BasePanel
{
    // 对象池
    public PoolList<DialogPanelItem> m_Pool = new PoolList<DialogPanelItem>();

    // Item 列表
    public List<DialogPanelItem> m_List = new List<DialogPanelItem>();

    // Item 面板上的按钮
    public GameObject m_DialogItem;

    // Item 的父物体(Item应该Clone在那个物体的里面)
    public GameObject m_ItemParent;

    // 标题
    public TextMeshProUGUI m_Title;

    // 信息展示
    public TextMeshProUGUI m_Info;

    // 背景
    public Image m_BG;

    // List
    List<Tween> m_TweenList = new List<Tween>();

    public override void Awake()
    {
        base.Awake();

        m_Pool.AddListener(OnInstanceItem);
    }

    /// <summary>
    /// 实例化一个 DialogPanelItem 类型的对象
    /// </summary>
    /// <returns></returns>
    public DialogPanelItem OnInstanceItem()
    {
        GameObject go = GameObject.Instantiate(m_DialogItem);
        go.transform.parent = m_ItemParent.transform;
        go.transform.localScale = Vector3.one;
        
        DialogPanelItem item = go.GetComponent<DialogPanelItem>();
        return item;
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    /// <param name="title"></param>
    /// <param name="info"></param>
    /// <param name="list"></param>
    public void UpdateData(string title, string info, params ButtonData[] list)
    {
        m_Title.text = title;
        m_Info.text = info;
        Clear();

        foreach (ButtonData d in list)
        {
            DialogPanelItem item = m_Pool.Create("Btn-" + d.text);
            item.transform.SetAsLastSibling();
            item.UpdateData(d.text);
            if (d.isClose)
            {
                item.AddListener(() =>
                {
                    this.Active(false);
                    d.method();
                });
            }
            else
            {
                item.AddListener(d.method);
            }

            m_List.Add(item);
        }
    }

    /// <summary>
    /// 打開開關
    /// </summary>
    /// <param name="b"></param>
    public override void Active(bool b)
    {
        foreach (var item in m_TweenList)
        {
            // 如果item動畫在運行，停止他
            if (item.IsPlaying())
            {
                item.Kill();
            }
        }
        m_TweenList.Clear(); //清空

        RectTransform rect = m_Content.transform as RectTransform;
        if (b) // 顯示時，背景動畫漸變顯示
        {
            Tween tween0 = DOTween.To(() => 0, (a) => m_BG.color = new Color(0, 0, 0, a), 0.75f, 0.2f).OnStart(() =>
            {
                m_BG.gameObject.SetActive(true);
            });

            Tween tween1 = DOTween.To(() => Vector3.zero, (a) => rect.localScale = a, Vector3.one, 0.2f).OnStart(() =>
            {
                base.Active(b);
            });

            m_TweenList.Add(tween0);
            m_TweenList.Add(tween1);
        }
        else // 關閉時，背景動畫漸變消失
        {
            Tween tween0 = DOTween.To(() => 0.75f, (a) => m_BG.color = new Color(0, 0, 0, a), 0, 0.2f).OnComplete(() =>
            {
                m_BG.gameObject.SetActive(false);
            });

            Tween tween1 = DOTween.To(() => Vector3.one, (a) => rect.localScale = a, Vector3.zero, 0.2f).OnComplete(() =>
            {
                base.Active(b);
                Clear();
            });

            m_TweenList.Add(tween0);
            m_TweenList.Add(tween1);
        }
    }

    /// <summary>
    /// 清理面板
    /// </summary>
    public void Clear()
    {
        foreach (var item in m_List)
        {
            item.Clear();
            m_Pool.Destroy(item);
        }
        m_List.Clear();
    }

    /// <summary>
    /// 打开对话框
    /// </summary>
    /// <param name="title"></param>
    /// <param name="info"></param>
    /// <param name="callback"></param>
    public static void OpenDialog(string title, string info, Action callback)
    {
        DialogPanel panel = UIConsole.Instance.FindAssetPanel<DialogPanel>();
        if (panel != null)
        {
            panel.UpdateData(title, info, new ButtonData("确定", callback), new ButtonData("取消", () => { panel.Active(false); }));
            panel.Active(true);
        }
        else
        {
            Debug.LogError("对话框面板不存在");
        }
    }
}

public class ButtonData
{
    // 文本显示
    public string text;

    // 函数
    public Action method;

    // 点击后关闭面板
    public bool isClose = false;

    public ButtonData(string txt, bool close = true)
    {
        this.text = txt;
        this.isClose = close;
        this.method = () => { };
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="m"></param>
    public ButtonData(string txt, Action m, bool close = true)
    {
        this.text = txt;
        this.method = m;
        this.isClose = close;
    }

    /// <summary>
    /// 字符串隐式转换
    /// </summary>
    /// <param name="txt"></param>
    /// <returns></returns>
    public static implicit operator ButtonData(string txt)
    {
        return new ButtonData(txt);
    }
}
