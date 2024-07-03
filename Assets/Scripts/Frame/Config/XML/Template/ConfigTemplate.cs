using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class ConfigTemplate
{
    // 编号
    public int id;

    // 名称
    public string name;

    // xml 文档
    public XDocument doc;

    // xml 路径
    public string path;

    /// <summary>
    /// 读取配置
    /// </summary>
    /// <param name="path"></param>
    public async void ReadXML(string path)
    {
        await NetworkManager._Instance.DownLoadTextFromServer(path, (text) =>
        {
            XDocument doc = XDocument.Parse(text);
            this.path = path;
            this.doc = doc;
            ReadXML(doc);
        });
    }

    /// <summary>
    /// 解析不同的xml文件，并存储在内存中
    /// </summary>
    /// <param name="doc"></param>
    public virtual void ReadXML(XDocument doc)
    {

    }
}
