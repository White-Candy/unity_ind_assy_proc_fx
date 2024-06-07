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
        Debug.Log("Awake!");
        base.Awake();
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
        transform.SetAsLastSibling();
    }

    public void Show(string str, float time)
    {
        Active(true);
        m_TipText.text = str;

        // TODO..Timer

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_layoutGruop);
    }
}

