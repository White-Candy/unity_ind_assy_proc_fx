using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace sugar
{
    public enum Mode
    {
        None,
        Practice,
        Examination
    }

    public static class GlobalData
    {
        public static string IP = "127.0.0.1";
        public static string Port = "8096";
        public static string Url = $"http://{IP}:{Port}";

        public static void SetUrl(string _ip, string _port)
        {
            Url = $"http://{_ip}:{_port}";
        }

        public static string token;
        public static int examId; // 考核id

        public static int SuccessCode = 200;//请求成功
        public static int ErrorCode = 500;//业务异常 
        public static int WarningCode = 601;//系统警告信息

        public static Mode mode = Mode.None;
        public static string currModuleCode = "";
        public static string currModuleName = "";
        public static List<Proj> Projs = new List<Proj>(); // 训练/实训考核的模型场景 Addressables Groups Default Name 的列表.
        public static Target ModelTarget; // 训练/实训考核的模型场景 Addressables Groups Default Name.

        // 已经完成本次考核，重新登录后才可再次考试
        public static bool currentExamIsFinish = false;

        // ModuleList窗口中的控件
        public static GameObject TaskListPanel = null; // 考核列表界面
        public static Button Task; // 考核列表中的一個個Task按鈕
        public static Transform TaskParent = null; // 顯示考核内容列表按鈕的Parent Transform

        /// <summary>
        /// 在选择完考试列表后考试试题以及模块等信息都存在此类型中...
        /// ...练习模式下 通过模块对应的SoftwareID获取试题， 第一个参数为模块编码 第二个参数为 SoftwareID
        /// </summary>
        public static ExamJsonData examData;

        public static Dictionary<string, int> codeVSidDic = new Dictionary<string, int>(); //模型Code值与资源ID的字典

        // 裏面存儲的是不同項目模式的名字和號碼
        //public static List<string[]> moduleContent = new List<string[]> { new string[] {"", "10022"} }; 

        public static List<string> FinishExamModule = new List<string>(); //記錄已經完成的考核内容

        public static string currItemMode; // 子模式
        public static bool isLoadModel { get { return (currModuleName == "训练") || (currItemMode == "实操" && currModuleName == "考核"); } } // 模型加载的条件
        public static bool isCommit { get { return (currModuleName == "考核"); } } // 点击右方按钮是否触发提交窗口
        public static bool DestroyModel = false; // 训练模式退出销毁模型，重置相机
        public static int StepIdx = 0; // 步骤索引
        public static List<StepStruct> stepStructs = new List<StepStruct>(); // 动画步骤信息
        public static List<string> Tools = new List<string>(); // 工具名称
        public static List<string> Materials = new List<string>(); // 材料名称
        public static bool canClone = false; // 当stepStructes刷新 表示其他监听的UI类[SelectStepPanel]可以对stepStructes进行Clone

        public static int ExamTime = 5000; // 考核时间（秒）
        public static float totalScore;
        public static bool isFinishTheoreticalExam; // 是否完成考核
        public static float theoreticalExamscore = 50f; // 理论考核满分是50分
        public static float trainingExamscore = 50f; // 实训考核满分是50分
        public static List<AnswerDetailVoListItem> m_TheorExamBody = new List<AnswerDetailVoListItem>(); // 理论考核结束时的内容
        public static List<AnswerDetailVoListItem> m_RealExamBody = new List<AnswerDetailVoListItem>(); // 实训考核结束时的内容
    }
}
