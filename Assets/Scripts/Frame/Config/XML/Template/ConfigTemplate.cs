using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class ConfigTemplate
{
    // ±‡∫≈
    public int id;

    // √˚≥∆
    public string name;

    // xml Œƒµµ
    public XDocument doc;

    // xml ¬∑æ∂
    public string path;

    /// <summary>
    /// ∂¡»°≈‰÷√
    /// </summary>
    /// <param name="path"></param>
    public void ReadXML(string path)
    {
        NetworkManager._Instance.DownLoadTextFromServer(path, (text) =>
        {
            XDocument doc = XDocument.Parse(text);
            this.path = path;
            this.doc = doc;
            ReadXML(doc);
        });
    }

    /// <summary>
    /// Ω‚Œˆ≈‰÷√
    /// </summary>
    /// <param name="doc"></param>
    public virtual void ReadXML(XDocument doc)
    {

    }
}
