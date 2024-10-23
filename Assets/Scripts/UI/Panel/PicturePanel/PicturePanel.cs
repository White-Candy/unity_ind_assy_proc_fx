using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PicturePanel : BasePanel
{
    public GameObject m_Item;
    public GameObject m_ItemParent;

    public GameObject m_MaxItemMask;
    public Image m_MaxImage;

    //private List<GameObject> m_Items = new List<GameObject>();

    public void Init(List<Sprite> list, List<string> nameList)
    {
        //foreach (var item in m_Items)
        //{
        //    item.gameObject.SetActive(false);
        //    Destroy(item.gameObject);
        //}
        //m_Items.Clear();

        for (int i = 0; i < list.Count && i < nameList.Count; ++i)
        {
            Sprite sprite = list[i];

            string[] filesName = nameList[i].Split("/");
            string fileName = filesName[filesName.Count() - 1];
            Debug.Log(fileName);

            GameObject obj = Instantiate(m_Item, m_ItemParent.transform);
            obj.SetActive(true);
            Button btn = obj.transform.Find("Button").gameObject.GetComponent<Button>();
            btn.GetComponent<Image>().sprite = sprite;
            btn.onClick.AddListener(() => { OnItemBtnClicked(sprite); });

            Button pictureNameButton = obj.transform.Find("PictureName").gameObject.GetComponent<Button>();
            TextMeshProUGUI nameText = pictureNameButton.GetComponentInChildren<TextMeshProUGUI>();
            nameText.text = fileName;
            //m_Items.Add(obj);
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
