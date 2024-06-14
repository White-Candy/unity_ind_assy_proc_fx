using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : BasePanel
{
    public Button m_Exit;

    public TextMeshProUGUI m_Title;

    public override void Awake()
    {
        base.Awake();
        m_Title.text = $"模拟演练-{GlobalData.currModuleName}";
        m_Exit.onClick.AddListener(OnExitBtnClicked);
    }

    private void OnExitBtnClicked()
    {
        if (GlobalData.mode == Mode.Examination)
        {
            // TODO..考核模式下退出提交成績
            UITools.Loading("Menu", false);
            GlobalData.currentExamIsFinish = true;
        }
        else
        {
            UITools.Loading("Menu", false);
            //删除已经实例化的模型数据
            UnityEventCenter.DistributeEvent(EnumDefine.EventKey.DataRecycling, null);
        }
    }
}
