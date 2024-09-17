using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TCPHelper
{
    /// <summary>
    /// �ļ���������
    /// </summary>
    /// <param name="relative"></param>
    public static void DownLoadResourcesReq(string relative)
    {
        JsonData js = new JsonData();
        js["relaPath"] = relative;
        TCP.SendAsync(JsonMapper.ToJson(js), EventType.DownLoadEvent, OperateType.NONE);
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

        foreach (var path in list)
        {
            DownLoadResourcesReq(path);
        }
        await UniTask.Yield();
    }

    /// <summary>
    /// �������ļ�����
    /// </summary>
    /// <param name="code"></param>
    /// <param name="moduelName"></param>
    public static async void CheckResourceReq(UpdatePackage up, List<string> filesPath)
    {     
        foreach (var path in filesPath)
        {
            var Rsinfo = StorageExpand.FindRsInfo(path);
            up.filesInfo.Add(Rsinfo);
        }
        string body = await JsonHelper.AsyncToJson(up);

        TCP.SendAsync(body, EventType.CheckEvent, OperateType.NONE);
    }

    /// �������ļ�����
    /// </summary>
    /// <param name="code"></param>
    /// <param name="moduelName"></param>
    public async static UniTask CkResourceReqOfList(List<string> paths, string name)
    {
        UpdatePackage up = new UpdatePackage();

        string relative = GlobalData.ProjGroupName + Tools.GetModulePath(name);
        List<string> filesPath = new List<string>();
        foreach (string path in paths)
        {
            string relaPath = Tools.GetFileRelativePath(path, name);
            filesPath.Add(relaPath);
        }
        up.relativePath = relative;

        CheckResourceReq(up, filesPath);

        await UniTask.WaitUntil(() => GlobalData.Checked == true);
        GlobalData.Checked = false;
    }

    /// <summary>
    /// �ļ��ĸ��º���������
    /// </summary>
    /// <param name="paths"> �ļ�·�� </param>
    /// <param name="name"> ģ������ </param>
    public async static UniTask<List<string>> RsCkAndDLReq(List<string> paths, string name)
    {
        List<string> newPaths = new List<string>(paths);

        // �ļ��б���¼������
        await CkResourceReqOfList(newPaths, name);

        // �ļ��б����ص��ڴ�������
        await DLResourcesReqOfList(DownLoadPanel._instance.m_NeedDL);

        Debug.Log($"DownLoadPanel._instance.m_NeedDL.Count : {DownLoadPanel._instance.m_NeedDL.Count}");
        if (DownLoadPanel._instance.m_NeedDL.Count > 0)
        {
            // ����׼��
            await UITools.DownLoadPrepare(DownLoadPanel._instance.m_NeedDL.Count);

            // �ļ����ڴ�д��Ӳ��
            await Tools.WtMem2DiskOfFileList(DownLoadPanel._instance.m_NeedWt);
            
            foreach (var fp in DownLoadPanel._instance.m_NeedWt)
            {
                string path = Application.streamingAssetsPath + "\\Data\\" + fp.relativePath;
                int i = newPaths.FindIndex(x => x == path);
                if (i == -1)
                    newPaths.Add(path);
            }

            await UniTask.WaitUntil(() => DownLoadPanel._instance.m_Finished == true);

            DownLoadPanel._instance.Clear();
        }
        return newPaths;
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

            TCP.SendAsync(JsonMapper.ToJson(inf), EventType.UserLoginEvent, OperateType.NONE);
        });
    }

    /// <summary>
    /// ע������
    /// </summary>
    /// <param name="account"></param>
    /// <param name="pwd"></param>
    /// <param name="verify"></param>
    /// <returns></returns>
    public static void Register(string account, string pwd, string verify, string name, string _className)
    {
        if (UITools.InputFieldCheck(account, "�û�������Ϊ��")) { return; }
        if (UITools.InputFieldCheck(pwd, "���벻��Ϊ��")) { return; }
        if (UITools.InputFieldCheck(verify, "���ٴ���������")) { return; }
        if (UITools.InputFieldCheck(name, "��������Ϊ��")) { return; }

        if (pwd == verify)
        {
            UserInfo inf = new UserInfo
            {
                userName = account,
                password = pwd,
                Name = name,
                className = _className,
                Identity = "ѧ��"
            };

            TCP.SendAsync(JsonMapper.ToJson(inf), EventType.RegisterEvent, OperateType.NONE);
        }
        else
        {
            UITools.ShowMessage("�������벻һ��");
        }
    }

    /// ��ȡ��Ϣ����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void GetInfoReq<T>(EventType type) 
    {
        // TCPBaseHelper helper = new T();
        // helper.GetInfReq();

        List<T> inf = new List<T>();       
        string body = JsonMapper.ToJson(inf);
        TCP.SendAsync(body, type, OperateType.GET);
    }    
}