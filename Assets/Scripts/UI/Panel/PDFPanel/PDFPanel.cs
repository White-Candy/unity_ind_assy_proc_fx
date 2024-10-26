using Cysharp.Threading.Tasks;
using Paroxe.PdfRenderer;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PDFPanel : BasePanel
{
    private List<string> m_PDFPaths;

    public GameObject pdfItem;
    public GameObject m_PDFItemParent;
    public PDFViewer m_PDFViewer;

    // PDF文件item实例列表
    private List<GameObject> m_Items = new List<GameObject>();

    public void Init(List<string> paths, string name)
    {
        SetPDFActive(false);
        m_PDFPaths = paths;
        SpawnPDFItem();
    }

    private void SpawnPDFItem()
    {
        foreach (var path in m_PDFPaths)
        {
            GameObject itemObj = GameObject.Instantiate(pdfItem, m_PDFItemParent.transform);
            itemObj.gameObject.SetActive(true);
            TextMeshProUGUI nameText = itemObj.transform.Find("PDFName").GetComponentInChildren<TextMeshProUGUI>();
            Button pdfButton = itemObj.GetComponentInChildren<Button>();

            nameText.text = name;
            // pdfButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/NewUI/Teaching/Video");
            pdfButton.onClick.AddListener(() => { OnPDFBtnClicked(path); });
            m_Items.Add(itemObj);
        }
    }

    private void OnPDFBtnClicked(string path)
    {
        m_PDFViewer.LoadDocumentFromFile(path);
        m_PDFViewer.transform.SetAsLastSibling();
        m_PDFViewer.gameObject.SetActive(true);
        //SetPDFActive(true);
    }

    private void SetPDFActive(bool b)
    {
        m_PDFViewer.gameObject.SetActive(b);
        ListenerPDFActive();
    }

    private async void ListenerPDFActive()
    {
        //m_PDFToken = new CancellationTokenSource();
        try
        {
            await UniTask.WaitUntilValueChanged(m_PDFViewer, x => x.gameObject.activeSelf);
            bool b = m_PDFViewer.gameObject.activeSelf;
            m_PDFItemParent?.gameObject.SetActive(!b);

            ListenerPDFActive();
        }
        catch
        {

        }
    }

    public void Exit()
    {
        if (this != null)
        {
            //Debug.Log(m_Items.Count);
            foreach (var item in m_Items)
            {
                item.gameObject.SetActive(false);
                Destroy(item.gameObject);
            }
            m_Items.Clear();
        }
    }
}
