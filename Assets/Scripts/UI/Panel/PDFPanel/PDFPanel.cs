using Cysharp.Threading.Tasks;
using Paroxe.PdfRenderer;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public void Init(List<string> paths)
    {
        SetPDFActive(false);
        m_PDFPaths = paths;
        SpawnPDFItem();
    }

    private void SpawnPDFItem()
    {
        foreach (var path in m_PDFPaths)
        {
            GameObject itemObj = GameObject.Instantiate(m_PDFItem, m_PDFItemParent.transform);
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
}
