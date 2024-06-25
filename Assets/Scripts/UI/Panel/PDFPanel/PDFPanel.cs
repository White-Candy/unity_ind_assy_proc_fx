using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PDFPanel : BasePanel
{
    private List<string> m_PDFPaths;

    public GameObject m_PDFItem;
    public Transform m_PDFItemParent;

    public void Init(List<string> paths)
    {
        m_PDFPaths = paths;
        SpawnPDFItem();
    }

    private void SpawnPDFItem()
    {
        foreach (var path in m_PDFPaths)
        {
            GameObject itemObj = GameObject.Instantiate(m_PDFItem, m_PDFItemParent);
            itemObj.gameObject.SetActive(true);
            Button itemBtn = itemObj.GetComponentInChildren<Button>();

            int SlashIdx = path.LastIndexOf('/');
            int suffixIdx = path.LastIndexOf('.');
            int length = (suffixIdx - SlashIdx - 1) > 0 ? suffixIdx - SlashIdx - 1 : 0;
            string pdfName = path.Substring(SlashIdx + 1, length);

            itemBtn.GetComponentInChildren<TextMeshProUGUI>().text = pdfName;
            itemBtn.onClick.AddListener(() => { OnPDFBtnClicked(path); });
        }
    }

    private void OnPDFBtnClicked(string path)
    {
        // TODO...
        Application.OpenURL(path);
    }
}
