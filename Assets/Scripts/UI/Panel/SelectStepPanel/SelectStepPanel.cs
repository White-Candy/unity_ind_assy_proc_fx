
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectStepPanel : BasePanel
{
    public Button m_Item;
    public Transform m_Parent;
    public GameObject stepName;
    public static SelectStepPanel _instance;

    private List<Button> m_Items = new List<Button>();

    private bool isSpawnItem = false;

    public List<EStepStatus> stepsStatus = new List<EStepStatus>();
    public List<string> stepNameList = new List<string>();

    public Button leftButton;
    public Button rightButton;
    public Scrollbar scrollBar;

    public override void Awake()
    {
        base.Awake();
        _instance = this;
        Active(false);
    }

    private void Start()
    {
        // this.gameObject.SetActive(false);
        //m_Parent = transform.Find("Content");
        stepName.gameObject.SetActive(false);
        leftButton.onClick.AddListener(() => { scrollBar.value -= 0.05f;});
        rightButton.onClick.AddListener(() => { scrollBar.value += 0.05f; });
    }

    private void Update()
    {
        ShowStepName();
        if (GlobalData.stepStructs.Count > 0 && GlobalData.canClone)
        {
            SpawnStepItem();
            GlobalData.canClone = false;
        }
    }

    public void SpawnStepItem()
    {
        foreach (var item in m_Items)
        {
            Destroy(item.gameObject);
        }
        m_Items.Clear();

        for (int i = 0; i < GlobalData.stepStructs.Count; ++i)
        {
            if (!check(GlobalData.stepStructs[i].stepName)) continue;

            Button clone = Instantiate(m_Item, m_Parent);

            stepsStatus.Add(EStepStatus.UnChecked);
            stepNameList.Add(GlobalData.stepStructs[i].stepName);
            clone.GetComponentInChildren<TextMeshProUGUI>().text = GlobalData.stepStructs[i].stepName;

            int id = i;
            clone.onClick.AddListener(() =>
            {
                GameMode.Instance.SetStep(id);
            });
            m_Items.Add(clone);

            clone.gameObject.SetActive(true);
        }
        isSpawnItem = true;
    }

    /// <summary>
    /// 展示步骤名字
    /// </summary>
    public void ShowStepName()
    {
        if (isSpawnItem)
        {
            bool allNoHightlight = false;
            for (int i = 0; i < m_Items.Count(); ++i)
            {
                var item = m_Items[i];
                bool highlight = RectTransformUtility.RectangleContainsScreenPoint(item.GetComponent<RectTransform>(), Input.mousePosition);
                if (highlight)
                {
                    Vector3 itemPostion = item.transform.position;
                    stepName.transform.position = new Vector3(itemPostion.x, stepName.transform.position.y, stepName.transform.position.z);
                    stepName.GetComponentInChildren<TextMeshProUGUI>().text = stepNameList[i];
                    stepName.gameObject.SetActive(true);
                }
                allNoHightlight |= highlight;
            }

            if (!allNoHightlight)
                stepName.gameObject.SetActive(false);
        }
    }

    public void SetStepButtonStyle(int i, EStepStatus stepStatus)
    {
        Sprite stepImage = null;
        string trainingPath = "Textures/NewUI/Training";

        if (stepStatus == EStepStatus.Doing)
            stepImage = Resources.Load<Sprite>($"{trainingPath}/StepDoing");
        else if (stepStatus == EStepStatus.Finish)
            stepImage = Resources.Load<Sprite>($"{trainingPath}/StepFinish");
        else
            stepImage = Resources.Load<Sprite>($"{trainingPath}/StepUnselected");

        m_Items[i].GetComponent<Image>().sprite = stepImage;
    }

    public void UpdateStepsButton()
    {
        for (int i = 0; i < stepsStatus.Count && i < m_Items.Count; i++)
            SetStepButtonStyle(i, stepsStatus[i]);
    }

    public void SetStepStatus(int i, EStepStatus stepStatus)
    {
        stepsStatus[i] = stepStatus;
        UpdateStepsButton();
    }

    private bool check(string txt)
    {
        return !string.IsNullOrEmpty(txt) && txt.Count() > 0;
    }

    public enum EStepStatus
    {
        None,
        UnChecked,
        Doing,
        Finish
    }

}
