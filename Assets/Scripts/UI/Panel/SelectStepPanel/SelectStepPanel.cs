using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectStepPanel : BasePanel
{
    public Button m_Item;
    public Transform m_Parent;

    private List<Button> m_items = new List<Button>();

    private void Start()
    {
        this.gameObject.SetActive(false);
        //m_Parent = transform.Find("Content");
    }

    private void Update()
    {
        if(GlobalData.stepStructs.Count > 0 && GlobalData.canClone)
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
            Button clone = Instantiate(m_Item, m_Parent);
            clone.GetComponentInChildren<TextMeshProUGUI>().text = GlobalData.stepStructs[i].stepName;
            int id = i;
            clone.onClick.AddListener(() =>
            {
                //Debug.Log("id : " + id);
                GameMode.Instance.SetStep(id);
            });
            m_items.Add(clone);

            clone.gameObject.SetActive(true);
        }
    }
}
