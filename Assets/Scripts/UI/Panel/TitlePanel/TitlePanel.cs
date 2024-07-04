using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : BasePanel
{
    public Button m_Exit;

    public TextMeshProUGUI m_Title;

    public override void Awake()
    {
        base.Awake();
        m_Title.text = $"模拟演练-{GlobalData.currModuleName}";
        m_Exit.onClick.AddListener(OnExitBtnClicked);
    }

    private void OnExitBtnClicked()
    {
        //CameraControl.SetMain();
        if (GlobalData.isCommit)
        {
            if (GlobalData.mode == Mode.Examination)
            {
                UITools.OpenDialog("", "是否退出考核模式，退出时将提交成绩", async () =>
                {
                    SubmitExamInfoData submitExamInfo = new SubmitExamInfoData();
                    submitExamInfo.examSerializeId = GlobalData.examData.data.examSerializeId;

                    List<UserExamDetailVoListItem> ExamDetailLists = new List<UserExamDetailVoListItem>();

                    // 理论考核封装
                    UserExamDetailVoListItem TheorExamDetailItem = new UserExamDetailVoListItem();
                    TheorExamDetailItem.type = 1;
                    TheorExamDetailItem.answerDetailVoList = GlobalData.m_TheorExamBody;
                    ExamDetailLists.Add(TheorExamDetailItem);

                    // 实训考核封装
                    UserExamDetailVoListItem RealExamDetailItem = new UserExamDetailVoListItem();
                    RealExamDetailItem.type = 2;
                    RealExamDetailItem.answerDetailVoList = GlobalData.m_RealExamBody;
                    ExamDetailLists.Add(RealExamDetailItem);

                    submitExamInfo.userExamDetailVoList = ExamDetailLists;
                    string json = JsonMapper.ToJson(submitExamInfo);

                    // Debug.Log(json);
                    // Debug.Log("------" + URL.submitExamInfo);//StaticData.token
                    // Debug.Log("------" + GlobalData.token);

                    string commit_msg = "";
                    // 请求提交成绩
                    await Client.Instance.m_Server.Post(URL.submitExamInfo, json, (data) =>
                    {
                        JsonData js_data = JsonMapper.ToObject<JsonData>(data);
                        commit_msg = js_data["code"].ToString();
                    });

                    if (commit_msg == "200")
                    {
                        // Debug.Log(URL.endExam);
                        await Client.Instance.m_Server.Post(URL.endExam, json, (data) => 
                        {
                            GlobalData.DestroyModel = true;
                            GlobalData.StepIdx = 0;
                            GlobalData.totalScore = 0f;
                            GlobalData.currentExamIsFinish = true;

                            UITools.Loading("Menu");
                        });
                    }
                });
            }
            else
            {
                GlobalData.DestroyModel = true;
                GlobalData.StepIdx = 0;
            }           
        }
        else
        {
            UITools.Loading("Menu");
        }

    }
}
