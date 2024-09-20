using LitJson;

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : BasePanel
{
    public Button m_Exit;

    public TextMeshProUGUI m_Title;

    public static TitlePanel _instance;

    public override void Awake()
    {
        base.Awake();
        _instance = this;
        m_Title.text = $"ģ������-{GlobalData.currModuleName}";
        m_Exit.onClick.AddListener(OnExitBtnClicked);
    }

    private void OnExitBtnClicked()
    {
        //CameraControl.SetMain();
        if (GlobalData.SceneModel != null)
        {
            // ����ģʽ�����Լ����ύ��ť�����Բ���Ҫ�ڹرյ�ʱ�����ύ��
            /*if (GlobalData.isSubmit && GlobalData.mode == Mode.Examination)
            {
                UITools.OpenDialog("", "�Ƿ��˳�����ģʽ���˳�ʱ���ύ�ɼ�", async () =>
                {
                    SubmitExamInfoData submitExamInfo = new SubmitExamInfoData();
                    submitExamInfo.examSerializeId = GlobalData.examData.data.examSerializeId;

                    List<UserExamDetailVoListItem> ExamDetailLists = new List<UserExamDetailVoListItem>();

                    // ���ۿ��˷�װ
                    UserExamDetailVoListItem TheorExamDetailItem = new UserExamDetailVoListItem();
                    TheorExamDetailItem.type = 1;
                    TheorExamDetailItem.answerDetailVoList = GlobalData.m_TheorExamBody;
                    ExamDetailLists.Add(TheorExamDetailItem);

                    // ʵѵ���˷�װ
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
                    // �����ύ�ɼ�
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
                            GlobalData.currModuleName = "";

                            UITools.Loading("Menu");
                        });
                    }
                });
            }*/
            //Debug.Log("SceneModel != null");

            //GlobalData.currModuleName = "";
            //UITools.Loading("Menu");
            Utilly.ExitModeSceneAction();
        }
        else
        {
            GlobalData.currModuleName = "";
            UITools.Loading("Menu");
        }

    }

    public void SetTitle(string title)
    {
        m_Title.text = $"ģ������-{title}";
    }
}
