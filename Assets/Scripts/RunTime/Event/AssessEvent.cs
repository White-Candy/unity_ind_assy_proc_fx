using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ������˰�ť���¼�
public class AssessEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        base.OnEvent(args);
        // Debug.Log("Assess Event!");

        //if (GlobalData.currentExamIsFinish)
        //{
        //    UITools.OpenDialog("", "������ɱ��ο���!", () => { });
        //    //Debug.Log("������ɱ��ο���!");
        //    return;
        //}

        // �@ȡ�������
        // ��Ϊ��ǰ�ķ�����������QAQ����������Ľӿ�Ҳ��������qwq��
        // ������һ���ִ���Ҫ���ε��ˣ�
        /*await Client.Instance.m_Server.Get_SetHeader(URL.QUERY_MY_EXAM, (dataServer) =>
        {
            //Debug.Log(dataServer);
            GlobalData.TaskListPanel.gameObject.SetActive(true);
            JsonData js_data = JsonMapper.ToObject(dataServer);
        
            for (int i = 0; i < js_data["data"].Count; ++i)
            {
                Button item = Object.Instantiate(GlobalData.Task, GlobalData.TaskParent);
                item.gameObject.SetActive(true);
                int examID = int.Parse(js_data["data"][i]["examId"].ToString());
                item.GetComponentInChildren<TextMeshProUGUI>().text = js_data["data"][i]["examName"].ToString();
                item.onClick.AddListener(async () =>
                {
                    GlobalData.examId = examID;
                    await Client.Instance.m_Server.Get_SetHeader(URL.startExam, (dataExam) =>
                    {
                        //Debug.Log(dataExam);
                        ExamJsonData ex_js_data = JsonMapper.ToObject<ExamJsonData>(dataExam);
                        GlobalData.examData = ex_js_data;
                        GlobalData.TaskListPanel.gameObject.SetActive(false);
        
                        SwitchSceneAccName(m_Name);
                    });
                });
            }
        });*/

        // TODO������������ôд�����濪�����·�����Ҫ��Ӧ�µĽӿڡ�-2024/08
        // TODO...��ȡ��Ŀ�Ϳγ����� �Լ� �������ݡ�
        GlobalData.mode = Mode.Examination;
        TCP.SendAsync("[]", EventType.GetProjInfo, OperateType.NONE);
        TCPHelper.GetInfoReq<ExamineInfo>(EventType.ExamineEvent);
        SwitchSceneAccName(m_Name);

        await UniTask.Yield();
    }
}

/// <summary>
///  ������Ϣ��
/// </summary>
public class ExamineInfo
{
    public string id;
    public string ColumnsName;
    public string CourseName;
    public string RegisterTime;
    public int TrainingScore;
    public int ClassNum;
    public int SingleNum;
    public int MulitNum;
    public int TOFNum;
    public bool Status = false;
    public List<SingleChoice> SingleChoices = new List<SingleChoice>();
    public List<MulitChoice> MulitChoices = new List<MulitChoice>();
    public List<TOFChoice> TOFChoices = new List<TOFChoice>();

    public ExamineInfo() {}
    public ExamineInfo(ExamineInfo info)
    {
        id = info.id;
        ColumnsName = info.ColumnsName;
        CourseName = info.CourseName;
        RegisterTime = info.RegisterTime;
        TrainingScore = info.TrainingScore;
        ClassNum = info.ClassNum;
        SingleNum = info.SingleNum;
        MulitNum = info.MulitNum;
        TOFNum = info.TOFNum;
        Status = info.Status;
        SingleChoices = new List<SingleChoice>(info.SingleChoices);
        MulitChoices = new List<MulitChoice>(info.MulitChoices);
        TOFChoices = new List<TOFChoice>(info.TOFChoices);
    }
}

/// <summary>
/// ��ѡ���
/// </summary>
public class SingleChoice
{
    public string Topic;
    public ItemChoice toA = new ItemChoice();
    public ItemChoice toB = new ItemChoice();
    public ItemChoice toC = new ItemChoice();
    public ItemChoice toD = new ItemChoice();
    public string Answer;
    public int Score = 0;
}

/// <summary>
/// ��ѡ
/// </summary>
public class MulitChoice
{
    public string Topic;
    public List<MulitChoiceItem> Options = new List<MulitChoiceItem>(); // {{"A", "xxxxx", true}, {"B", "xxxxxxx", false}}
    public string Answer;
    public int Score;
}

/// <summary>
/// �ж���
/// </summary>
public class TOFChoice
{
    public string Topic;
    public ItemChoice toA = new ItemChoice();
    public ItemChoice toB = new ItemChoice();
    public string Answer;
    public int Score;
}

/// <summary>
/// ����ģʽ�� һ��ѡ�����Ϣ
/// </summary>
public class ItemChoice
{
    public string m_content = "";
    public bool m_isOn = false;

    public ItemChoice() {}

    public ItemChoice(string content, bool ison)
    {
        m_content = content;
        m_isOn = ison;
    }
}

/// <summary>
/// ��Ϊ��������û�а취 ���л� �ֵ����ͣ�����Ϊ�˱����ѡ���ѡ���Ҫ�Զ���һ����
/// </summary>
public class MulitChoiceItem
{
    public string Serial = "A";
    public string Content = "";
    public bool isOn = false;

    public MulitChoiceItem() {}
    public MulitChoiceItem(string serial, string content, bool isOn)
    {
        Serial = serial;
        Content = content;
        this.isOn = isOn;
    }
}
