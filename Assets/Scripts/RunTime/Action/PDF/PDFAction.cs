using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using System.Threading;
using UnityEngine.UIElements;
using Unity.VisualScripting;
public class PDFAction : BaseAction
{
    private PDFPanel m_Panel;

    // �����洢�Ѿ���ʼ��������ģ�����֣���һ�ν��벻�ڳ�ʼ��
    private Dictionary<string, List<string>> m_initList = new Dictionary<string, List<string>>();

    private bool m_init = false; // �Ƿ�׼������

    public PDFAction()
    {
        m_Panel = UITools.FindAssetPanel<PDFPanel>();

        m_Token = new CancellationTokenSource();
    }

    public override async UniTask AsyncShow(string name)
    {
        if (!m_initList.ContainsKey(name))
        {
            await NetworkManager._Instance.DownLoadConfigAsync(name, (paths) =>
            {
                if (paths.Count == 0)
                    UITools.ShowMessage("��ǰģ��û��PDF��Դ");
                m_Panel.Init(paths);
                m_initList.Add(name, paths);
                m_init = true;
            });
        }
        else
        {
            m_Panel.Init(m_initList[name]);
            m_init = true;
        }
        await UniTask.WaitUntil(() => m_init == true, PlayerLoopTiming.Update, m_Token.Token);

        m_Panel.transform.SetAsFirstSibling();
        m_Panel.Active(true);
    }

    public override void UpdateData()
    {
    }


    public override void Exit()
    {
        m_Token.Cancel();
        m_Token.Dispose();
        m_Token = new CancellationTokenSource();

        m_Panel.Exit();
        m_Panel.Active(false);

        m_init = false;
    }
}