using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PDFAction : BaseAction
{
    private PDFPanel m_Panel;
    private bool m_Init = false;

    public PDFAction()
    {
        m_Panel = UITools.FindAssetPanel<PDFPanel>();
    }

    public override async UniTask AsyncShow()
    {
        if (!m_Init)
        {
            await NetworkManager._Instance.DownLoadConfig((paths) => 
            {
                if (paths.Count == 0)
                    UITools.ShowMessage("当前模块没有PDF资源");
                m_Panel.Init(paths);
                m_Init = true;
            });
        }
        m_Panel.Active(true);
    }

    public override void UpdateData()
    {
    }


    public override void Exit()
    {
        m_Panel.Active(false);
    }
}
