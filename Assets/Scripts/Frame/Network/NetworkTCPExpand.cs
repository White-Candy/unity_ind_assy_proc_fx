using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class NetworkTCPExpand
{
    /// <summary>
    /// �ļ���������
    /// </summary>
    /// <param name="relative"></param>
    public static void DownLoadResourcesReq(string relative)
    {
        JsonData js = new JsonData();
        js["relaPath"] = relative;
        NetworkClientTCP.SendAsync(JsonMapper.ToJson(js), EventType.DownLoadEvent, OperateType.NONE);
    }

    /// <summary>
    /// �ļ��б���������
    /// </summary>
    /// <param name="list"></param>
    public async static UniTask DLResourcesReqOfList(List<string> list)
    {
        if (list.Count == 0)
        {
            DownLoadPanel._instance.m_Finished = true;
        }
        else
        {
            DownLoadPanel._instance.Active(true);
        }

        Debug.Log("DLResourcesReqOfList: " + list.Count);

        foreach (var path in list)
        {
            DownLoadResourcesReq(path);

            await UniTask.WaitUntil(() => GlobalData.Downloaded == true);
            GlobalData.Downloaded = false;
        }
    }

    /// <summary>
    /// �������ļ�����
    /// </summary>
    /// <param name="code"></param>
    /// <param name="moduelName"></param>
    public static void CheckResourceReq(string relative)
    {
        var Rsinfo = StorageExpand.FindRsInfo(relative);
        string s_info = JsonMapper.ToJson(Rsinfo);
        NetworkClientTCP.SendAsync(s_info, EventType.CheckEvent, OperateType.NONE);
    }

    /// �������ļ�����
    /// </summary>
    /// <param name="code"></param>
    /// <param name="moduelName"></param>
    public async static UniTask CkResourceReqOfList(List<string> paths, string name)
    {
        foreach (string path in paths)
        {
            string relaPath = Tools.GetFileRelativePath(path, name);

            // ����������ͼ���ļ�����
            // To Future developers:
            // Ŀǰʱһ��ֻ������һ���ļ��ĸ���...
            // �����������ļ�����ֻ����ѭ����һ��һ������
            // �Ժ����д��ֱ�Ӵ���һ���б�
            CheckResourceReq(relaPath);

            await UniTask.WaitUntil(() => GlobalData.Checked == true);
            GlobalData.Checked = false;
        }
    }

    /// <summary>
    /// �ļ��ĸ��º���������
    /// </summary>
    /// <param name="paths"> �ļ�·�� </param>
    /// <param name="name"> ģ������ </param>
    public async static UniTask RsCkAndDLReq(List<string> paths, string name)
    {
        // �ļ��б���¼������
        await CkResourceReqOfList(paths, name);

        // �ļ��б����ص��ڴ�������
        await DLResourcesReqOfList(DownLoadPanel._instance.m_NeedDL);

        // �ļ����ڴ�д��Ӳ��
        await Tools.WtMem2DiskOfFileList(DownLoadPanel._instance.m_NeedWt);

        await UniTask.WaitUntil(() => DownLoadPanel._instance.m_Finished == true);

        DownLoadPanel._instance.Clear();
    }

    /// <summary>
    /// �û���¼����
    /// </summary>
    /// <returns></returns>
    public async static UniTask UserLoginReq(string account, string pwd)
    {
        await UniTask.RunOnThreadPool(() =>
        {
            UserInfo inf = new UserInfo();
            inf.userName = account;
            inf.password = pwd;

            NetworkClientTCP.SendAsync(JsonMapper.ToJson(inf), EventType.UserLoginEvent, OperateType.NONE);
        });
    }

    /// <summary>
    /// ע������
    /// </summary>
    /// <param name="account"></param>
    /// <param name="pwd"></param>
    /// <param name="verify"></param>
    /// <returns></returns>
    public static void Register(string account, string pwd, string verify)
    {
        if (UITools.InputFieldCheck(account, "�û�������Ϊ��")) { return; }
        if (UITools.InputFieldCheck(pwd, "���벻��Ϊ��")) { return; }
        if (UITools.InputFieldCheck(verify, "���ٴ���������")) { return; }

        if (pwd == verify)
        {
            UserInfo inf = new UserInfo();
            inf.userName = account;
            inf.password = pwd;
            inf.level = 0;

            NetworkClientTCP.SendAsync(JsonMapper.ToJson(inf), EventType.RegisterEvent, OperateType.NONE);
        }
        else
        {
            UITools.ShowMessage("�������벻һ��");
        }
    }
}