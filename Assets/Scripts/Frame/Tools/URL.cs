using sugar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URL
{
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
}
