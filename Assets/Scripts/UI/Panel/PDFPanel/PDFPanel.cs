using Cysharp.Threading.Tasks;
using LitJson;
using Paroxe.PdfRenderer;
using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class PDFPanel : BasePanel
{
    private List<string> m_PDFPaths;

    public GameObject m_PDFItem;
    public GameObject m_PDFItemParent;
    public PDFViewer m_PDFViewer;

    private string moudleName = "";

    // PDF文件item实例列表
    private List<GameObject> m_Items = new List<GameObject>();

    public void Init(List<string> paths, string name)
    {
        moudleName = name;
        SetPDFActive(false);
        m_PDFPaths = paths;
        SpawnPDFItem();
    }

    private async void SpawnPDFItem()
    {
        foreach (var path in m_PDFPaths)
        {
            string[] split = path.Split('/');
            string filename = split[split.Length - 1];
            string suffix = Tools.GetModulePath(moudleName);
            string relaPath = $"{GlobalData.currModuleCode}{suffix}\\{filename}";
            Debug.Log("PDF: " + relaPath);

            JsonData js = new JsonData();
            js["relaPath"] = relaPath;
            NetworkClientTCP.SendAsync(JsonMapper.ToJson(js), EventType.DownLoadEvent);
            await UniTask.WaitUntil(() => GlobalData.IsLatestRes == true);

            GlobalData.IsLatestRes = false;
            GameObject itemObj = GameObject.Instantiate(m_PDFItem, m_PDFItemParent.transform);
            itemObj.gameObject.SetActive(true);
            Button itemBtn = itemObj.GetComponentInChildren<Button>();

            string pdfName = Path.GetFileNameWithoutExtension(path);
            itemBtn.GetComponentInChildren<TextMeshProUGUI>().text = pdfName;
            itemBtn.onClick.AddListener(() => { OnPDFBtnClicked(path); });
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
