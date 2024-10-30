using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MessagePanel : BasePanel
{
    public RectTransform m_layoutGruop;
    public TextMeshProUGUI m_TipText;

    private Timer timer;

    public override void Awake()
    {
        base.Awake();
        timer = TimerConsole.Instance.CreateTimer();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public override void Active(bool b)
    {
        base.Active(b);
        transform.SetAsLastSibling(); // 设置为组中最后一个
    }

    public void Show(string str, float time)
    {
        Active(true);
        m_TipText.text = str;

        timer.Run(4.0f, () => 
        {
            Active(false);
        });

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_layoutGruop); //强制立即重建布局
    }
}

