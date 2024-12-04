
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// before: �������������ӿ�
/// now: ��������Ҫ�Լ�д, ��ʱ�ò���URL����
/// </summary>
public class URL
{
    public static string IP = "127.0.0.1:8080";

    public static string ROOT_API
    {
        get { return GlobalData.Url + "/prod-api/application/u3dApp/"; }
    }

    /// <summary>
    /// ��¼�ӿ�
    /// </summary>
    public static string URL_LOGIN
    {
        get { return ROOT_API + "login"; }
    }

    /// <summary>
    /// �����б�
    /// </summary>
    public static string QUERY_MY_EXAM
    {
        get { return ROOT_API + "queryMyExam"; }
    }

    // �_ʼ����
    public static string startExam
    {
        get { return ROOT_API + $"startExam?examId={GlobalData.examId}"; }
    }

    // �����ύ
    public static string submitExamInfo
    {
        get { return ROOT_API + "submitExamInfo"; }
    }

    // ���Խ�������
    public static string endExam
    {
        get { return ROOT_API + $"endExam?examSerializeId={GlobalData.examData.data.examSerializeId}"; }
    }

    public static string numberOfPeople
    {
        get { return IP + "NumOfPeople"; }
    }
}
