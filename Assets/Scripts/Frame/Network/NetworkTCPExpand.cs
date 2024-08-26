using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class NetworkTCPExpand
{
    /// <summary>
    /// 文件下载请求
    /// </summary>
    /// <param name="relative"></param>
    public static void DownLoadResourcesReq(string relative)
    {
        JsonData js = new JsonData();
        js["relaPath"] = relative;
        NetworkClientTCP.SendAsync(JsonMapper.ToJson(js), EventType.DownLoadEvent, OperateType.NONE);
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

        Debug.Log("DLResourcesReqOfList: " + list.Count);

        foreach (var path in list)
        {
            DownLoadResourcesReq(path);

            await UniTask.WaitUntil(() => GlobalData.Downloaded == true);
            GlobalData.Downloaded = false;
        }
    }

    /// <summary>
    /// 请求检查文件更新
    /// </summary>
    /// <param name="code"></param>
    /// <param name="moduelName"></param>
    public static void CheckResourceReq(string relative)
    {
        var Rsinfo = StorageExpand.FindRsInfo(relative);
        string s_info = JsonMapper.ToJson(Rsinfo);
        NetworkClientTCP.SendAsync(s_info, EventType.CheckEvent, OperateType.NONE);
    }

    /// 请求检查文件更新
    /// </summary>
    /// <param name="code"></param>
    /// <param name="moduelName"></param>
    public async static UniTask CkResourceReqOfList(List<string> paths, string name)
    {
        foreach (string path in paths)
        {
            string relaPath = Tools.GetFileRelativePath(path, name);

            // 向服务器发送检查文件请求
            // To Future developers:
            // 目前时一次只能请求一个文件的更新...
            // 所以遇到多文件更新只能在循环中一次一次请求
            // 以后可以写成直接传输一个列表
            CheckResourceReq(relaPath);

            await UniTask.WaitUntil(() => GlobalData.Checked == true);
            GlobalData.Checked = false;
        }
    }

    /// <summary>
    /// 文件的更新和下载请求
    /// </summary>
    /// <param name="paths"> 文件路径 </param>
    /// <param name="name"> 模块名字 </param>
    public async static UniTask RsCkAndDLReq(List<string> paths, string name)
    {
        // 文件列表更新检查请求
        await CkResourceReqOfList(paths, name);

        // 文件列表下载到内存中请求
        await DLResourcesReqOfList(DownLoadPanel._instance.m_NeedDL);

        // 文件从内存写入硬盘
        await Tools.WtMem2DiskOfFileList(DownLoadPanel._instance.m_NeedWt);

        await UniTask.WaitUntil(() => DownLoadPanel._instance.m_Finished == true);

        DownLoadPanel._instance.Clear();
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

            NetworkClientTCP.SendAsync(JsonMapper.ToJson(inf), EventType.UserLoginEvent, OperateType.NONE);
        });
    }

    /// <summary>
    /// 注册请求
    /// </summary>
    /// <param name="account"></param>
    /// <param name="pwd"></param>
    /// <param name="verify"></param>
    /// <returns></returns>
    public static void Register(string account, string pwd, string verify)
    {
        if (UITools.InputFieldCheck(account, "用户名不能为空")) { return; }
        if (UITools.InputFieldCheck(pwd, "密码不能为空")) { return; }
        if (UITools.InputFieldCheck(verify, "请再次输入密码")) { return; }

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
            UITools.ShowMessage("两次密码不一样");
        }
    }
}