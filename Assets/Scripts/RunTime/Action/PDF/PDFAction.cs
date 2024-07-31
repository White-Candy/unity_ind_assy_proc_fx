using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using System.Threading;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System;
using sugar;
public class PDFAction : BaseAction
{
    private PDFPanel m_Panel;

    // 用来存储已经初始化过的子模块名字，下一次进入不在初始化
    private Dictionary<string, List<string>> m_initList = new Dictionary<string, List<string>>();

    private bool m_init = false; // 是否准备好了

    public PDFAction()
    {
        m_Token = new CancellationTokenSource();
        m_panelToken = new CancellationTokenSource();
    }

    public override async UniTask AsyncShow(string name)
    {
        if (!m_initList.ContainsKey(name))
        {
            await NetworkManager._Instance.DownLoadConfigAsync(name, async (paths) =>
            {
                if (paths.Count == 0)
                    UITools.ShowMessage("当前模块没有PDF资源");

                await NetworkTCPExpand.RsCkAndDLReq(paths, name);

                m_Panel = UIConsole.Instance.FindAssetPanel<PDFPanel>();
                m_Panel.Init(paths, name);
                m_initList.Add(name, paths);
                m_init = true;
            });
        }
        else
        {
            m_Panel.Init(m_initList[name], name);
            m_init = true;
        }
        await UniTask.WaitUntil(() => m_init == true, PlayerLoopTiming.Update, m_Token.Token).SuppressCancellationThrow();

        m_Panel.transform.SetAsFirstSibling();
        m_Panel.Active(true);
        try
        { 
            await UniTask.WaitUntil(() => m_Panel?.m_Content.activeSelf == false);
            //Debug.Log("await finish");
        }
        catch { }
    }

    public override void UpdateData()
    {
    }


    public override void Exit(Action callback)
    {
        base.Exit(callback);

        m_Token.Cancel();
        m_Token.Dispose();
        m_Token = new CancellationTokenSource();

        m_Panel.Exit();
        m_Panel.Active(false);

        m_init = false;
    }
}
