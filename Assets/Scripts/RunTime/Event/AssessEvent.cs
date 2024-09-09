using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 点击考核按钮的事件
public class AssessEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        base.OnEvent(args);
        // Debug.Log("Assess Event!");

        //if (GlobalData.currentExamIsFinish)
        //{
        //    UITools.OpenDialog("", "您已完成本次考核!", () => { });
        //    //Debug.Log("您已完成本次考核!");
        //    return;
        //}

        // 獲取考核類型
        // 因为以前的服务器不用了QAQ，所以这里的接口也不能用了qwq，
        // 所以这一部分代码要屏蔽掉了！
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

        // TODO。。现在先这么写，后面开发了新服务器要对应新的接口。-2024/08
        // TODO...获取栏目和课程数据 以及 考核数据。
        GlobalData.mode = Mode.Examination;
        TCP.SendAsync("[]", EventType.GetProjInfo, OperateType.NONE);
        TCPHelper.GetInfoReq<ExamineInfo>(EventType.ExamineEvent);
        SwitchSceneAccName(m_Name);

        await UniTask.Yield();
    }
}

/// <summary>
///  考核信息包
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
/// 单选题包
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
/// 多选
/// </summary>
public class MulitChoice
{
    public string Topic;
    public List<MulitChoiceItem> Options = new List<MulitChoiceItem>(); // {{"A", "xxxxx", true}, {"B", "xxxxxxx", false}}
    public string Answer;
    public int Score;
}

/// <summary>
/// 判断题
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
/// 理论模式中 一个选项的信息
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
/// 因为服务器端没有办法 序列化 字典类型，所以为了保存多选题的选项，需要自定义一个类
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
