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

public class PDFAction : BaseAction
{
    private PDFPanel m_Panel;

    // �����洢�Ѿ���ʼ��������ģ�����֣���һ�ν��벻�ڳ�ʼ��
    private Dictionary<string, List<string>> m_initList = new Dictionary<string, List<string>>();

    private bool m_init = false; // �Ƿ�׼������

    public PDFAction()
    {
        m_Token = new CancellationTokenSource();
        m_panelToken = new CancellationTokenSource();
    }

    public override async UniTask AsyncShow(string name)
    {
        if (!m_initList.ContainsKey(name))
        {
            List<string> paths = new List<string>();
            string configPath = FPath.AssetRootPath + GlobalData.ProjGroupName + Tools.GetModulePath(name);
            paths = await FileHelper.DownLoadConfig(name, configPath + "\\Config.txt", ".pdf");
            if (paths.Count == 0)
            {
                UITools.ShowMessage("��ǰģ��û��PDF��Դ");
                return;
            }

            m_Panel = UIConsole.FindAssetPanel<PDFPanel>();
            m_Panel.Init(paths, name);
            m_initList.Add(name, paths);
            m_init = true;
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
