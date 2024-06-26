using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PicturePanel : BasePanel
{
    public GameObject m_Item;
    public GameObject m_ItemParent;

    public GameObject m_MaxItemMask;
    public Image m_MaxImage;

    private List<GameObject> m_Items = new List<GameObject>();
    public void Init(List<Sprite> list)
    {
        foreach (var item in m_Items)
        {
            item.gameObject.SetActive(false);
            Destroy(item.gameObject);
        }
        m_Items.Clear();

        foreach (var sprite in list)
        {
            //Debug.Log(sprite.name);
            GameObject obj = Instantiate(m_Item, m_ItemParent.transform);
            obj.SetActive(true);
            Button btn = obj.GetComponentInChildren<Button>();
            btn.GetComponent<Image>().sprite = sprite;
            btn.onClick.AddListener(() => { OnItemBtnClicked(sprite); });
            m_Items.Add(obj);
        }
    }

    private void OnItemBtnClicked(Sprite sprite)
    {
        m_MaxItemMask.gameObject.SetActive(true);
        m_MaxImage.sprite = sprite;
        m_MaxImage.gameObject.SetActive(true);
        m_MaxImage.SetNativeSize(); // 将Image设置为贴图的原始尺寸
    }
}
