
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

    private List<Button> m_items = new List<Button>();

    public static SelectStepPanel _instance;

    private bool isSpawnItem = false;

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
    }

    private void Update()
    {
        if (isSpawnItem)
        {
            foreach (var item in m_items)
            {
                bool highlight = RectTransformUtility.RectangleContainsScreenPoint(item.GetComponent<RectTransform>(), Input.mousePosition);
                if (highlight)
                    Debug.Log($"{item.name} hightLight");
            }
        }

        if (GlobalData.stepStructs.Count > 0 && GlobalData.canClone)
        {
            SpawnStepItem();
            GlobalData.canClone = false;
        }
    }

    public void SpawnStepItem()
    {
        foreach (var item in m_items)
        {
            Destroy(item.gameObject);
        }
        m_items.Clear();

        for (int i = 0; i < GlobalData.stepStructs.Count; ++i)
        {
            if (!check(GlobalData.stepStructs[i].stepName)) continue;

            Button clone = Instantiate(m_Item, m_Parent);
            clone.GetComponentInChildren<TextMeshProUGUI>().text = GlobalData.stepStructs[i].stepName;
            int id = i;
            clone.onClick.AddListener(() =>
            {
                // Debug.Log("id : " + id);
                GameMode.Instance.SetStep(id);
            });
            m_items.Add(clone);

            clone.gameObject.SetActive(true);
        }
        isSpawnItem = true;
    }

    private bool check(string txt)
    {
        return !string.IsNullOrEmpty(txt) && txt.Count() > 0;
    }
}
