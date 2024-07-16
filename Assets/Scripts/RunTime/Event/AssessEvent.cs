using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 点击考核按钮的事件
public class AssessEvent : ModuleEvent
{
    public override async void OnEvent(params object[] args)
    {
        base.OnEvent(args);
        //Debug.Log("Assess Event!");

        GlobalData.mode = Mode.Examination;
        if (GlobalData.currentExamIsFinish)
        {
            UITools.OpenDialog("", "您已完成本次考核!", () => { });
            //Debug.Log("您已完成本次考核!");
            return;
        }

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

        // TODO。。现在先这么写，后面开发了新服务器要对应新的接口。
        SwitchSceneAccName(m_Name);
    }
}
