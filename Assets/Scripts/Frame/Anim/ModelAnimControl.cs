using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ConstrPtStep
{
    public string step; //当前步骤
    public string constrPt; // 施工要点
}

public class ModelAnimControl : MonoBehaviour
{
    public static ModelAnimControl _Instance;
    public string m_animCameraName; // 动画相机的名字

    private float totalScore = 0;

    public List<ConstrPtStep> m_ConPtStep = new List<ConstrPtStep>();

    private string ModelName; //模型的 adressables Group Default Name

    private void Awake()
    {
        _Instance = this;
        DontDestroyOnLoad(this);

         ModelName = GlobalData.ModelTarget.modelName;
        // 获取 xxx.json 中的 当前步骤_施工要点
        NetworkManager._Instance.DownLoadTextFromServer(Application.streamingAssetsPath + "/ModelExplain/" + ModelName + ".json", (str) => 
        {
            Debug.Log(str);
            JsonData js_data = JsonMapper.ToObject<JsonData>(str);
            foreach (var item in js_data.Keys)
            {
                //Debug.Log(item);
                string[] fields = js_data[item].ToString().Split('_');

                ConstrPtStep step = new ConstrPtStep();
                step.step = fields[0];
                step.constrPt = fields[1];

                m_ConPtStep.Add(step);
            }
        });
    }

    private void Start()
    {

    }
}
