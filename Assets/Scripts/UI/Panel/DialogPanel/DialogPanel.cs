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
    // �����
    public PoolList<DialogPanelItem> m_Pool = new PoolList<DialogPanelItem>();

    // Item �б�
    public List<DialogPanelItem> m_List = new List<DialogPanelItem>();

    // Item ����ϵİ�ť
    public GameObject m_DialogItem;

    // Item �ĸ�����(ItemӦ��Clone���Ǹ����������)
    public GameObject m_ItemParent;

    // ��Ϣչʾ
    public TextMeshProUGUI m_Info;

    // ����
    public Image bg;

    // ����
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
    /// ʵ����һ�� DialogPanelItem ���͵Ķ���
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
    /// ��������
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
    /// ���_�_�P
    /// </summary>
    /// <param name="b"></param>
    public override void Active(bool b)
    {
        //foreach (var item in m_TweenList)
        //{
        //    ���item�Ӯ����\�У�ֹͣ��
        //    if (item.IsPlaying())
        //    {
        //        item.Kill();
        //    }
        //}
        //m_TweenList.Clear(); //���

        RectTransform rect = m_Content.transform as RectTransform;
        base.Active(b);
        if (!b)
            Clear();
    }

    /// <summary>
    /// �������
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
    /// �򿪶Ի���
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
    // �ı���ʾ
    public string text;

    // ����ͼƬ
    public string imgPath;

    // ����
    public Action method;

    // �����ر����
    public bool isClose = false;

    public ButtonData(string txt, bool close = true)
    {
        this.text = txt;
        this.isClose = close;
        this.method = () => { };
    }

    /// <summary>
    /// ���캯��
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
    /// �ַ�����ʽת��
    /// </summary>
    /// <param name="txt"></param>
    /// <returns></returns>
    public static implicit operator ButtonData(string txt)
    {
        return new ButtonData(txt);
    }
}
