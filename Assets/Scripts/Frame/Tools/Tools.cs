using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class Tools
{
    // 中转英字典
    public static Dictionary<string, string> EscDic = new Dictionary<string, string>
    { 
        { @"教学", "TeachingEvent"}, {@"训练", "TrainEvent"},
        {@"考核", "AssessEvent"}, 
        {@"教案", "PDFAction"}, {@"图纸", "PDFAction"}, {@"方案", "PDFAction"}, {@"规范", "PDFAction"}, {@"图片", "PictureAction"}, 
        {@"动画", "VideoAction"}, {@"视频", "VideoAction"}, {@"构造", "ModelAction"}, {@"理论", "TheoreticalExamAction"}, {@"实操", "TrainingAction"},
        {@"建材", "DisplayAction"}
    };

    // 不同子模式对应不同的文件路径
    // (没有写在里面的子模式，要么是网络获取，要么是从Addressabels中获取)
    private static Dictionary<string, string> FileDic = new Dictionary<string, string> 
    {
        {@"教案", FPath.JiaoAnSuffix}, {@"图纸", FPath.TuZhiSuffix}, {@"方案", FPath.FangAnSuffix}, {@"规范", FPath.GuiFanSuffix},
        {@"图片", FPath.PictureSuffix}, {@"动画", FPath.AnimSuffix}, {@"视频", FPath.VideoSuffix}, {@"建材", FPath.ModelSuffix}
    };

    public static bool CheckMessageSuccess(int code)
    {
        return code == GlobalData.SuccessCode;
    }

    /// <summary>
    /// 动态创建类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    static public T CreateObject<T>(string name) where T : class
    {
        object obj = CreateObject(name);
        return obj == null ? null : obj as T;
    }

    /// <summary>
    /// 动态创建类
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static object CreateObject(string name)
    {
        object obj = null;
        try
        {
            Type type = Type.GetType(name, true);
            obj = Activator.CreateInstance(type); //创建指定类型的实例。
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
        return obj;
    }

    public static string LocalPathEncode(string txt)
    {
        return txt;
    }


    /// <summary>
    /// Cn To En Or En To Cn
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string Escaping(string name)
    {
        string TypeName = "";
        if (EscDic.ContainsKey(name))
        {
            TypeName = EscDic[name];
            if (!EscDic.ContainsKey(TypeName))
            {
                EscDic.Add(TypeName, name);
            }
        }
        return TypeName;
    }

    /// <summary>
    /// 获取不同子模式对应的不同文件路径
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetModulePath(string name)
    {
        string path = "";
        if (FileDic.ContainsKey(name))
        {
            path = FileDic[name];
        }
        return path;
    }

    /// <summary>
    /// Object -> Texture2D -> Sprite
    /// </summary>
    /// <returns>The convert.</returns>
    /// <param name="tex">Tex.</param>
    public static Sprite SpriteConvert(Texture2D tex)
    {
        Sprite sprite;
        sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero, 100f);
        return sprite;
    }

    /// <summary>
    /// 检查字符串长度，需要时要换行
    /// </summary>
    /// <param name="text"></param>
    /// <param name="max_len"></param>
    /// <returns></returns>
    public static string checkLength(string text, int max_len)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < text.Length; ++i)
        {
            if (i != 0 && (i % max_len) == 0)
            {
                sb.Append('\n');
            }
            sb.Append(text[i]);
        }
        return sb.ToString();
    }


    /// <summary>
    /// 模型场景异步加载
    /// </summary>
    /// <returns></returns>
    public static async UniTask LoadSceneModel()
    {
        GlobalData.DestroyModel = false;
        await LoadModelAsync();
    }

    /// <summary>
    /// 模型加载
    /// </summary>
    /// <returns></returns>
    private static async UniTask LoadModelAsync()
    {
        // 模型场景异步加载
        // Debug.Log(GlobalData.ModelTarget.modelName);
        AsyncOperationHandle<GameObject> model_async = Addressables.LoadAssetAsync<GameObject>(GlobalData.ModelTarget.modelName);
        await UniTask.WaitUntil(() => model_async.IsDone == true);

        GlobalData.SceneModel = UnityEngine.Object.Instantiate(model_async.Result);
        await AnalysisStepFile();
        await AnalysisEquFile();
    }

    /// <summary>
    /// 解析StepFile获取工具
    /// TODO.. 后续会写一个新的文件专门放置需要的工具和材料
    /// </summary>
    private static async UniTask AnalysisStepFile()
    {
        await NetworkManager._Instance.DownLoadTextFromServer((Application.streamingAssetsPath + "/ModelExplain/" + GlobalData.currModuleCode + "/" + GlobalData.ModelTarget.modelName + "Step.txt"), (dataStr) =>
        {
            // Debug.Log(dataStr);
            List<StepStruct> list = new List<StepStruct>();
            JsonData js_data = JsonMapper.ToObject(dataStr);
            JsonData step = js_data["child"];
            for (int i = 0; i < step.Count; i++)
            {
                StepStruct step_st = new StepStruct();
                string[] field = step[i].ToString().Split("_");
                if (field.Length == 3)
                {
                    // step_st.method = field[0];
                    step_st.tools = new List<string>(field[0].Split("|"));
                    step_st.stepName = field[1];
                    step_st.animLimite = new List<string>(field[2].Split("~"));
                }
                else
                {
                    step_st.tools = new List<string>(field[0].Split("|"));
                    // step_st.stepName = field[1];
                    step_st.animLimite = new List<string>(field[1].Split("~"));
                }
                list.Add(step_st);
            }

            // 动画信息载入
            GlobalData.stepStructs.Clear();
            GlobalData.stepStructs = list;
            //Debug.Log("GlobalData.stepStructs Count: " + GlobalData.stepStructs.Count);
            GlobalData.canClone = true;
            GameMode.Instance.Prepare(); // Step录入完成后，游戏准备

            if (GlobalData.mode == Mode.Examination)
            {
                GameMode.Instance.m_Score = GlobalData.trainingExamscore / GlobalData.stepStructs.Count;
                //Debug.Log("实训： " + GameMode.Instance.m_Score);
            }
            //UIConsole.Instance.FindPanel<SelectStepPanel>().Active(true);
            //UIConsole.Instance.FindPanel<InfoPanel>().Active(true);
            //Debug.Log("InfoPanel is true for active.");

            InfoPanel._instance.Active(true);
            SelectStepPanel._instance.Active(true);
            if (MinMap._instance.canshow)
            {
                MinMap._instance.Active(true);
            }
        });
        // SpawnTask();
    }

    /// <summary>
    /// 解析器材文件
    /// </summary>
    /// <returns></returns>
    private static async UniTask AnalysisEquFile()
    {
        await NetworkManager._Instance.DownLoadTextFromServer(Application.streamingAssetsPath + "/ModelExplain/" + GlobalData.currModuleCode + "/" + GlobalData.ModelTarget.modelName + "Equ.json", (str) => 
        {
            // Debug.Log(str);
            JsonData json_data = JsonMapper.ToObject<JsonData>(str);
            string tools = json_data["Tools"].ToString();
            string materials = json_data["Materials"].ToString();

            // Debug.Log($"Tools: {tools}.");
            // Debug.Log($"Materials: {materials}.");

            string[] arr_tools = tools.Split("_");
            string[] arr_materials = materials.Split("_");

            GlobalData.Tools = arr_tools.ToList();
            GlobalData.Materials = arr_materials.ToList();
        });
        SpawnTask();
    }

    /// <summary>
    /// 拖拽控件的创建
    /// </summary>
    private static void SpawnTask()
    {
        DragTask task;
        task = new DragTask();
        if (!task.IsInit)
        {
            task.Init(GlobalData.Tools, GlobalData.Materials, GameObject.Find("MainCanvas/InfoPanel").transform); //MenuPanel/Content/BG
        }
        task.Show();      
    }

    /// <summary>
    /// 为个位时单位补齐为双位
    /// 1 => 01
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string FillingForTime(string t)
    {
        if (t.Count() == 1)
        {
            return "0" + t;
        }
        return t;
    }

    /// <summary>
    /// 获取主机的ipv4地址
    /// </summary>
    /// <returns></returns>
    public static string GetIPForTypeIPV4()
    {
        string ipv4_ip = "127.0.0.1";
        foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
        {
            NetworkInterfaceType wirless = NetworkInterfaceType.Wireless80211;
            NetworkInterfaceType Ethernet = NetworkInterfaceType.Ethernet;
            NetworkInterfaceType item_networkType = item.NetworkInterfaceType;
            if ((item_networkType == wirless || item_networkType == Ethernet) && item.OperationalStatus == OperationalStatus.Up)
            {
                foreach (var ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ipv4_ip = ip.Address.ToString();
                    }
                }
            }
        }
        return ipv4_ip;
    }

    /// <summary>
    /// 等待器
    /// </summary>
    /// <param name="sec"> 秒数 ex: 1秒 => 1.0f </param>
    /// <param name="callback"> CallBack Action </param>
    /// <returns></returns>
    public static async UniTask OnAwait(float sec, Action callback)
    {
        int duration = (int)(sec * 1000);
        await UniTask.Delay(duration);
        callback();
    }
}
