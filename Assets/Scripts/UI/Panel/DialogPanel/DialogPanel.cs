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

    // ����
    public TextMeshProUGUI m_Title;

    // ��Ϣչʾ
    public TextMeshProUGUI m_Info;

    // ����
    public Image m_BG;

    public TMP_Text titleText;

    public Button closeButton;

    // List
    List<Tween> m_TweenList = new List<Tween>();

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

    /// <summary>
    /// ��������
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
    /// ���_�_�P
    /// </summary>
    /// <param name="b"></param>
    public override void Active(bool b)
    {
        foreach (var item in m_TweenList)
        {
            // ���item�Ӯ����\�У�ֹͣ��
            if (item.IsPlaying())
            {
                item.Kill();
            }
        }
        m_TweenList.Clear(); //���

        RectTransform rect = m_Content.transform as RectTransform;
        if (b) // �@ʾ�r�������Ӯ��u׃�@ʾ
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
        else // �P�]�r�������Ӯ��u׃��ʧ
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
    public static void OpenDialog(string title, string info, Action callback, bool single)
    {        
        DialogPanel panel = UIConsole.FindAssetPanel<DialogPanel>();
        Action action = () => 
        {
            callback();
            Destroy(panel.gameObject);
        };

        if (panel != null && !single)
        {
            panel.UpdateData(title, info, new ButtonData("ȷ��", action), new ButtonData("ȡ��", () => 
            {
                panel.Active(false); 
                Destroy(panel.gameObject);
            }));
            panel.Active(true);
        }
        else if (panel != null && single)
        { 
            panel.UpdateData(title, info, new ButtonData("ȷ��", action));
            panel.Active(true);
        }
        else
        {
            Debug.LogError("�Ի�����岻����");
        }
    }
}

public class ButtonData
{
    // �ı���ʾ
    public string text;

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
    public ButtonData(string txt, Action m, bool close = true)
    {
        this.text = txt;
        this.method = m;
        this.isClose = close;
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
