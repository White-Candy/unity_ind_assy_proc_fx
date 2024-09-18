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

        public static UserInfo usrInfo; // �û�����Ϣ

        public static string token;
        public static int examId; // ����id

        public static int SuccessCode = 200;//����ɹ�
        public static int ErrorCode = 500;//ҵ���쳣 
        public static int WarningCode = 601;//ϵͳ������Ϣ

        public static Mode mode = Mode.None;
        public static string currModuleName = "";
        public static string columnsName = ""; // ��ǰ��Ŀ
        public static string courseName = ""; // ��ǰ�γ�
        public static List<string> CurrActionPathList = new List<string>(); // ��ǰ��ѧģʽ��ĳ��ģ����ļ����·��

        /// <summary>
        /// �˵��洢�б�
        ///  key: �ǲ˵���ťʵ���� value: �ò˵��µĿͻ����б�
        ///  �ñ�����Ϊ��ʵ���������ܶ��������û���ͨ�������������γ�������Ҫȥ��������ֵ䣬Ȼ���ҵ������Ŀγ��б�͸��˵���Ŀʵ����
        ///  Ȼ���Զ�����˵���Ŀ��ť����ʾ���ڸ���Ŀ���б�UI��
        /// </summary>
        public static Dictionary<GameObject, List<string>> CurrModeMenuList = new Dictionary<GameObject, List<string>>();
        public static string ProjGroupName {get {return $"{columnsName}\\{courseName}";} } //����
        public static List<Proj> Projs = new List<Proj>(); // ѵ��/ʵѵ���˵�ģ�ͳ��� Addressables Groups Default Name ���б�.
        public static List<ClassInfo> classList = new List<ClassInfo>(); // ��������ע��İ༶��

        public static Transform TaskParent = null; // �@ʾ���������б��o��Parent Transform

        /// <summary>
        /// ��ѡ���꿼���б���������Լ�ģ�����Ϣ�����ڴ�������...
        /// ...��ϰģʽ�� ͨ��ģ���Ӧ��SoftwareID��ȡ���⣬ ��һ������Ϊģ����� �ڶ�������Ϊ SoftwareID
        /// </summary>
        public static ExamJsonData examData;

        public static string currItemMode; // ��ģʽ
        public static bool isLoadModel { get { return (currModuleName == "ѵ��") || (currItemMode == "ʵ��" && currModuleName == "����"); } } // ģ�ͼ��ص�����
        public static bool DestroyModel = false; // ѵ��ģʽ�˳�����ģ�ͣ��������
        public static int StepIdx = 0; // ��������
        public static List<StepStruct> stepStructs = new List<StepStruct>(); // ����������Ϣ
        public static List<string> Tools = new List<string>(); // ��������
        public static List<string> Materials = new List<string>(); // ��������
        public static bool canClone = false; // ��stepStructesˢ�� ��ʾ����������UI��[SelectStepPanel]���Զ�stepStructes����Clone

        // �������ݽṹ[���ۿ�����⣬ʵѵ���˷���]
        public static List<ExamineInfo> ExamineesInfo = new List<ExamineInfo>();
        public static ExamineInfo currExamsInfo = new ExamineInfo(); // ��ǰ���˿γ���Ϣ
        public static ScoreInfo currScoreInfo = new ScoreInfo(); // ��ǰ�û���ѡ���Կγ̵ĳɼ���Ϣ
        public static int ExamTime = 10; // ����ʱ�䣨���ӣ�
        public static List<ScoreInfo> scoresInfo = new List<ScoreInfo>(); // �ɼ���Ϣ

        // ����ģ��ʵ��
        public static GameObject SceneModel;

        // �����ļ��Ƿ���¼�����(PDF, ��Ƶ��ͼƬ)
        public static bool Checked = false;

        // �����ļ��Ƿ�������ϴ洢���ڴ���(PDF, ��Ƶ��ͼƬ)
        public static bool Downloaded = false;

        // �Ƿ�Ϊ���µ���Դ(PDF, ��Ƶ��ͼƬ)
        public static bool IsLatestRes = false;
    }
}
