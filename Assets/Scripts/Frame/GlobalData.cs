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
        Examinaion
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

        public static int SuccessCode = 200;//请求成功
        public static int ErrorCode = 500;//业务异常 
        public static int WarningCode = 601;//系统警告信息

        public static Mode mode = Mode.None;

        // 已经完成本次考核，重新登录后才可再次考试
        public static bool currentExamIsFinish = false;

        // ModuleList窗口中的控件
        public static GameObject TaskListPanel = null; // 考核列表界面
        public static Button Task; // 考核列表中的一個個Task按鈕
        public static Transform TaskParent = null; // 顯示考核内容列表按鈕的Parent Transform
    }
}
