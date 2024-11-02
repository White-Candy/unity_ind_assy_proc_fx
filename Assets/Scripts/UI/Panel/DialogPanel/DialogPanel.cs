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

    // 信息展示
    public TextMeshProUGUI m_Info;

    // 背景
    public Image bg;

    // 标题
    public TMP_Text titleText;

    public Button closeButton;

    // List
    // List<Tween> m_TweenList = new List<Tween>();

    public override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);

        m_Pool.AddListener(OnInstanceItem);
    }

    public void Start()
    {
        closeButton.onClick.AddListener(() => 
        {
            Active(false);
        });
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

    public void Init(string title, string info)
    {
        titleText.text = title;
        m_Info.text = info;
        Clear();
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    /// <param name="title"></param>
    /// <param name="info"></param>
    /// <param name="list"></param>
    public void AddItem(ButtonData data)
    {
        DialogPanelItem item = m_Pool.Create("Btn-" + data.text);
        item.transform.SetAsLastSibling();
        item.UpdateData(data.text, data.imgPath);
        if (data.isClose)
        {
            item.AddListener(() =>
            {
                data.method();
                this.Active(false);
            });
        }
        else
            item.AddListener(data.method);

        m_List.Add(item);
    }

    /// <summary>
    /// 打開開關
    /// </summary>
    /// <param name="b"></param>
    public override void Active(bool b)
    {
        //foreach (var item in m_TweenList)
        //{
        //    如果item動畫在運行，停止他
        //    if (item.IsPlaying())
        //    {
        //        item.Kill();
        //    }
        //}
        //m_TweenList.Clear(); //清空

        RectTransform rect = m_Content.transform as RectTransform;
        base.Active(b);
        if (!b)
            Clear();
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
    public static void OpenDialog(string title, string info, params ButtonData[] buttonsData)
    {        
        DialogPanel panel = UIConsole.FindAssetPanel<DialogPanel>();

        if (panel != null)
        {
            panel.Init(title, info);
            foreach (var data in buttonsData)
            {
                panel.AddItem(data);
            }
        }
        panel.Active(true);
    }
}

public class ButtonData
{
    // 文本显示
    public string text;

    // 背景图片
    public string imgPath;

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
    //public ButtonData(string txt, Action m, bool close = true)
    //{
    //    this.text = txt;
    //    this.method = m;
    //    this.isClose = close;
    //}

    public ButtonData(string txt, string imgPath, Action m, bool close = true)
    {
        this.text = txt;
        this.method = m;
        this.isClose = close;
        this.imgPath = imgPath;
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
