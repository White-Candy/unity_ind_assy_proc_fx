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
    /// 文件下载请求
    /// </summary>
    /// <param name="relative"></param>
    public static void DownLoadResourcesReq(string relative)
    {
        JsonData js = new JsonData();
        js["relaPath"] = relative;
        TCP.SendAsync(JsonMapper.ToJson(js), EventType.DownLoadEvent, OperateType.NONE);
    }

    /// <summary>
    /// 文件列表下载请求
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
    /// 请求检查文件更新
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

    /// 请求检查文件更新
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
    /// 文件的更新和下载请求
    /// </summary>
    /// <param name="paths"> 文件路径 </param>
    /// <param name="name"> 模块名字 </param>
    public async static UniTask<List<string>> RsCkAndDLReq(List<string> paths, string name)
    {
        List<string> newPaths = new List<string>(paths);

        // 文件列表更新检查请求
        await CkResourceReqOfList(newPaths, name);

        // 文件列表下载到内存中请求
        await DLResourcesReqOfList(DownLoadPanel._instance.m_NeedDL);

        Debug.Log($"DownLoadPanel._instance.m_NeedDL.Count : {DownLoadPanel._instance.m_NeedDL.Count}");
        if (DownLoadPanel._instance.m_NeedDL.Count > 0)
        {
            // 下载准备
            await UITools.DownLoadPrepare(DownLoadPanel._instance.m_NeedDL.Count);

            // 文件从内存写入硬盘
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
    /// 用户登录请求
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
    /// 注册请求
    /// </summary>
    /// <param name="account"></param>
    /// <param name="pwd"></param>
    /// <param name="verify"></param>
    /// <returns></returns>
    public static void Register(string account, string pwd, string verify, string name, string _className)
    {
        if (UITools.InputFieldCheck(account, "用户名不能为空")) { return; }
        if (UITools.InputFieldCheck(pwd, "密码不能为空")) { return; }
        if (UITools.InputFieldCheck(verify, "请再次输入密码")) { return; }
        if (UITools.InputFieldCheck(name, "姓名不能为空")) { return; }

        if (pwd == verify)
        {
            UserInfo inf = new UserInfo
            {
                userName = account,
                password = pwd,
                Name = name,
                className = _className,
                Identity = "学生"
            };

            TCP.SendAsync(JsonMapper.ToJson(inf), EventType.RegisterEvent, OperateType.NONE);
        }
        else
        {
            UITools.ShowMessage("两次密码不一样");
        }
    }

    /// 获取信息请求
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