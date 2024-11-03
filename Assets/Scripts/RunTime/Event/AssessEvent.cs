using Cysharp.Threading.Tasks;
using System.Collections.Generic;

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

        GlobalData.mode = Mode.Examination;
#if UNITY_STANDALONE_WIN
        HTTPConsole.SendAsyncPost("[]", EventType.GetProjInfo, OperateType.NONE);
#endif
        NetHelper.GetInfoReq<ExamineInfo>(EventType.ExamineEvent);
        NetHelper.GetInfoReq<ScoreInfo>(EventType.ScoreEvent);

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
    public string TrainingScore;
    public string TheoryTime = "5"; // ����
    public string TrainingTime = "5"; // ����
    public int PNum;
    public int SingleNum;
    public int MulitNum;
    public int TOFNum;
    public bool Status = false;
    public List<SingleChoice> SingleChoices = new List<SingleChoice>();
    public List<MulitChoice> MulitChoices = new List<MulitChoice>();
    public List<TOFChoice> TOFChoices = new List<TOFChoice>();

    public ExamineInfo() {}
    public ExamineInfo Clone ()
    {
        ExamineInfo inf = new ExamineInfo();
        inf.id = id;
        inf.ColumnsName = ColumnsName;
        inf.CourseName = CourseName;
        inf.RegisterTime = RegisterTime;
        inf.TrainingScore = TrainingScore;
        inf.PNum = PNum;
        inf.SingleNum = SingleNum;
        inf.MulitNum = MulitNum;
        inf.TOFNum = TOFNum;
        inf.TheoryTime = TheoryTime;
        inf.TrainingTime = TrainingTime;        
        inf.Status = Status;
        foreach (var Option in SingleChoices) { inf.SingleChoices.Add(Option.Clone()); }
        foreach (var Option in MulitChoices) { inf.MulitChoices.Add(Option.Clone()); }
        foreach (var Option in TOFChoices) { inf.TOFChoices.Add(Option.Clone()); }
        return inf;
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
    public string Score = "";

    public SingleChoice Clone()
    {
        SingleChoice single = new SingleChoice();
        single.Topic = Topic;
        single.toA = toA.Clone();
        single.toB = toB.Clone();
        single.toC = toB.Clone();
        single.toD = toB.Clone();
        single.Answer = Answer;
        single.Score = Score;
        return single;
    }    
}

/// <summary>
/// ��ѡ
/// </summary>
public class MulitChoice
{
    public string Topic;
    public List<MulitChoiceItem> Options = new List<MulitChoiceItem>(); // {{"A", "xxxxx", true}, {"B", "xxxxxxx", false}}
    public string Answer;
    public string Score = "";

    public MulitChoice Clone()
    {
        MulitChoice mulit = new MulitChoice();
        mulit.Topic = Topic;
        foreach (var Option in Options) { mulit.Options.Add(Option.Clone()); }
        mulit.Answer = Answer;
        mulit.Score = Score;
        return mulit;
    }
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
    public string Score = "";

    public TOFChoice Clone()
    {
        TOFChoice tof = new TOFChoice();
        tof.Topic = Topic;
        tof.toA = toA.Clone();
        tof.toB = toB.Clone();
        tof.Answer = Answer;
        tof.Score = Score;
        return tof;
    }
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

    public ItemChoice Clone()
    {
        ItemChoice item = new ItemChoice();
        item.m_content = m_content;
        item.m_isOn = m_isOn;
        return item;
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

    public MulitChoiceItem Clone()
    {
        MulitChoiceItem item = new MulitChoiceItem();
        item.Serial = Serial;
        item.Content = Content;
        item.isOn = isOn;
        return item;
    }
}


/// <summary>
/// �ɼ�������Ϣ
/// </summary>
public class ScoreInfo
{
    public string className;
    public string columnsName;
    public string courseName;
    public string registerTime; // �ôο��Ե�ע��ʱ��
    public string userName;
    public string Name;
    public string theoryScore = "";
    public string trainingScore = "";
    public bool theoryFinished = false; //�������ۿ����Ƿ����
    public bool trainingFinished = false; //����ʵѵ�����Ƿ����

    public ScoreInfo Clone()
    {
        ScoreInfo inf = new ScoreInfo();
        inf.className = className;
        inf.columnsName = columnsName;
        inf.courseName =courseName;
        inf.registerTime = registerTime;
        inf.userName = userName;
        inf.Name = Name;
        inf.theoryScore = theoryScore;
        inf.trainingScore = trainingScore;
        inf.theoryFinished = theoryFinished;
        inf.trainingFinished = trainingFinished;
        return inf;
    }
}