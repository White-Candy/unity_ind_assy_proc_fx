using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using System.Threading;
public class PDFAction : BaseAction
{
    private PDFPanel m_Panel;
    private bool m_Init = false;

    public PDFAction()
    {
        m_Panel = UITools.FindAssetPanel<PDFPanel>();

        m_Token = new CancellationTokenSource();
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
        await UniTask.WaitUntil(() => m_Init == true, PlayerLoopTiming.Update, m_Token.Token);

        m_Panel.transform.SetAsFirstSibling();
        m_Panel.Active(true);
    }

    public override void UpdateData()
    {
    }


    public override void Exit()
    {
        //m_Token.Cancel();
        //m_Token.Dispose();
        //m_Token = new CancellationTokenSource();
        m_Panel.Active(false);
    }
}
