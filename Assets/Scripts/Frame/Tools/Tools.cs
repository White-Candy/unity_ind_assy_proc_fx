using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Random = UnityEngine.Random;

public static class Tools
{
    // ��תӢ�ֵ�
    public static Dictionary<string, string> EscDic = new Dictionary<string, string>
    { 
        { @"��ѧ", "TeachingEvent"}, {@"ѵ��", "TrainEvent"},
        {@"����", "AssessEvent"}, 
        {@"�̰�", "PDFAction"}, {@"ͼֽ", "PDFAction"}, {@"����", "PDFAction"}, {@"�淶", "PDFAction"}, {@"ͼƬ", "PictureAction"}, 
        {@"����", "VideoAction"}, {@"��Ƶ", "VideoAction"}, {@"����", "ModelAction"}, {@"����", "TheoreticalExamAction"}, {@"ʵ��", "TrainingAction"},
        {@"����", "DisplayAction"}
    };

    // ��ͬ��ģʽ��Ӧ��ͬ���ļ�·��
    // (û��д���������ģʽ��Ҫô�������ȡ��Ҫô�Ǵ�Addressabels�л�ȡ)
    private static Dictionary<string, string> FileDic = new Dictionary<string, string> 
    {
        {@"�̰�", FPath.JiaoAnSuffix}, {@"ͼֽ", FPath.TuZhiSuffix}, {@"����", FPath.FangAnSuffix}, {@"�淶", FPath.GuiFanSuffix},
        {@"ͼƬ", FPath.PictureSuffix}, {@"����", FPath.AnimSuffix}, {@"��Ƶ", FPath.VideoSuffix}, {@"����", FPath.ModelSuffix}
    };

    public static bool CheckMessageSuccess(int code)
    {
        return code == GlobalData.SuccessCode;
    }

    /// <summary>
    /// ��̬������
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
    /// ��̬������
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static object CreateObject(string name)
    {
        object obj = null;
        try
        {
            Type type = Type.GetType(name, true);
            obj = Activator.CreateInstance(type); //����ָ�����͵�ʵ����
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
    /// ��ȡ��ͬ��ģʽ��Ӧ�Ĳ�ͬ�ļ�·��
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
    /// ����ַ������ȣ���ҪʱҪ����
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
    /// ģ�ͳ����첽����
    /// </summary>
    /// <returns></returns>
    public static async UniTask LoadSceneModel()
    {
        GlobalData.DestroyModel = false;
        await LoadModelAsync();
    }

    /// <summary>
    /// ģ�ͼ���
    /// </summary>
    /// <returns></returns>
    private static async UniTask LoadModelAsync()
    {
        // ģ�ͳ����첽����
        // Debug.Log(GlobalData.ModelTarget.modelName);
        AsyncOperationHandle<GameObject> model_async = Addressables.LoadAssetAsync<GameObject>(GlobalData.ProjGroupName);
        await UniTask.WaitUntil(() => model_async.IsDone == true);

        GlobalData.SceneModel = UnityEngine.Object.Instantiate(model_async.Result);
        await AnalysisStepFile();
        await AnalysisEquFile();
    }

    /// <summary>
    /// ����StepFile��ȡ����
    /// TODO.. ������дһ���µ��ļ�ר�ŷ�����Ҫ�Ĺ��ߺͲ���
    /// </summary>
    private static async UniTask AnalysisStepFile()
    {
        await NetworkManager._Instance.DownLoadTextFromServer(Application.streamingAssetsPath + "\\ModelExplain\\" + GlobalData.ProjGroupName + "Step.txt", (dataStr) =>
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

            // ������Ϣ����
            GlobalData.stepStructs.Clear();
            GlobalData.stepStructs = list;
            //Debug.Log("GlobalData.stepStructs Count: " + GlobalData.stepStructs.Count);
            GlobalData.canClone = true;
            GameMode.Instance.Prepare(); // Step¼����ɺ���Ϸ׼��

            if (GlobalData.mode == Mode.Examination)
            {
                GameMode.Instance.m_Score = GlobalData.trainingExamscore / GlobalData.stepStructs.Count;
                //Debug.Log("ʵѵ�� " + GameMode.Instance.m_Score);
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
    /// ���������ļ�
    /// </summary>
    /// <returns></returns>
    private static async UniTask AnalysisEquFile()
    {
        await NetworkManager._Instance.DownLoadTextFromServer(Application.streamingAssetsPath + "/ModelExplain/" + GlobalData.ProjGroupName + "Equ.json", (str) => 
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
    /// ��ק�ؼ��Ĵ���
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
    /// Ϊ��λʱ��λ����Ϊ˫λ
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
    /// ��ȡ������ipv4��ַ
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
    /// �ȴ���
    /// </summary>
    /// <param name="sec"> ���� ex: 1�� => 1.0f </param>
    /// <param name="callback"> CallBack Action </param>
    /// <returns></returns>
    public static async UniTask OnAwait(float sec, Action callback)
    {
        int duration = (int)(sec * 1000);
        await UniTask.Delay(duration);
        callback();
    }

    /// <summary>
    /// ���ֽ����� => �ļ���
    /// </summary>
    /// <param name="buffer"> �ļ��ֽ��� </param>
    /// <param name="save_path"> ����·�� </param>
    public static async UniTask Bytes2File(byte[] buffer, string save_path)
    {
        if (File.Exists(save_path)) 
        { 
            File.Delete(save_path);
        }
        else
        {
            string dir = save_path;
            int idx = dir.LastIndexOf('\\');
            dir = dir.Substring(0, idx);
            Directory.CreateDirectory(dir);
        }

        FileStream fs = new FileStream(save_path, FileMode.CreateNew);
        lock (fs)
        {
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();
        }

        await UniTask.WaitUntil(() => File.Exists(save_path) == true);

        DownLoadPanel._instance.SetWritePercent();
        GlobalData.IsLatestRes = true;
    }

    /// <summary>
    /// Ϊ�¡����/���¡����ļ������µİ汾��
    /// </summary>
    /// <returns></returns>
    public static string SpawnRandomCode()
    {
        return $"{Random.Range(1000, 9999)}-{Random.Range(1000, 9999)}-{Random.Range(1000, 9999)}-{Random.Range(1000, 9999)}";
    }

    /// <summary>
    /// ��ȡAction��ÿ���ļ������·��
    /// </summary>
    /// <param name="path"> �ļ�����·�� </param>
    /// <param name="name"> ģ���� </param>
    /// <returns></returns>
    public static string GetFileRelativePath(string path, string name)
    {
        string[] split = path.Split('\\');
        string filename = split[split.Length - 1];
        string suffix = GetModulePath(name);
        string relaPath = $"{GlobalData.ProjGroupName}{suffix}\\{filename}";

        return relaPath;
    }

    /// <summary>
    /// ���ڴ��е��ļ��б�д��Ӳ��
    /// </summary>
    /// <returns></returns>
    public static async UniTask WtMem2DiskOfFileList(List<FilePackage> fpList)
    {
        foreach (var fp in fpList)
        {
            string savePath = Application.streamingAssetsPath + "\\Data\\" + fp.relativePath;
            await Bytes2File(fp.fileData, savePath);

            await UniTask.WaitUntil(() => GlobalData.IsLatestRes == true);
            GlobalData.IsLatestRes = false;
        }
    }
}
