
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// before: 广域网服务器接口
/// now: 局域网需要自己写, 暂时用不到URL类了
/// </summary>
public class URL
{
    public static string IP = "127.0.0.1:8080";

    public static string ROOT_API
    {
        get { return GlobalData.Url + "/prod-api/application/u3dApp/"; }
    }

    /// <summary>
    /// 登录接口
    /// </summary>
    public static string URL_LOGIN
    {
        get { return ROOT_API + "login"; }
    }

    /// <summary>
    /// 考试列表
    /// </summary>
    public static string QUERY_MY_EXAM
    {
        get { return ROOT_API + "queryMyExam"; }
    }

    // 開始考核
    public static string startExam
    {
        get { return ROOT_API + $"startExam?examId={GlobalData.examId}"; }
    }

    // 考试提交
    public static string submitExamInfo
    {
        get { return ROOT_API + "submitExamInfo"; }
    }

    // 考试结束请求
    public static string endExam
    {
        get { return ROOT_API + $"endExam?examSerializeId={GlobalData.examData.data.examSerializeId}"; }
    }

    public static string numberOfPeople
    {
        get { return IP + "NumOfPeople"; }
    }
}
