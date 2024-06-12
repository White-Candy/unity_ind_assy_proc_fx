using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

public class ConfigConsole : Singleton<ConfigConsole>
{
    public static string m_RootPath;

    //配置文件列表
    public Dictionary<string, ConfigTemplate> m_ConfigList = new Dictionary<string, ConfigTemplate>();

    // 是否已經加載配置
    private bool m_Loaded = false;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

        if (!m_Loaded)
        {
            m_RootPath = Application.streamingAssetsPath;
            LoadConfig(m_RootPath);
        }
    }

    public void LoadConfig(string root_path)
    {
        if (m_Loaded)
            return;

        // Web端讀取
        NetworkManager._Instance.DownLoadTextFromServer(Tools.LocalPathEncode(Path.Combine(root_path, @"Config/Config.xml")), (text) =>
        {
            //Debug.Log("Config text: " + text);
            XDocument doc = XDocument.Parse(text);
            XElement root = doc.Root;

            int idx = 0;
            foreach (var item in root.Elements("Item"))
            {
                string name = item.Attribute("name").Value;
                string path = Path.Combine(m_RootPath, item.Attribute("path").Value);

                ConfigTemplate template = Tools.CreateObject<ConfigTemplate>(name); // 把 XML 中的name字段变成相应的 实例(基类都是 ConfigTemplate)。
                template.id = idx;
                template.name = name;
                template.path = path;

                try
                {
                    template.ReadXML(path);
                }
                catch
                {
                    Debug.LogError(string.Format("读取{0}失败，原因:解析失败 , 路径:{1}", name, path));
                    continue;
                }

                if (!m_ConfigList.ContainsKey(name))
                {
                    m_ConfigList.Add(name, template);
                }
                else
                {
                    Debug.LogError(string.Format("添加{0}失败，原因:名称重复 , 路径:{1}", name, path));
                }
                idx++;
            }
            m_Loaded = true;
        });
    }

    /// <summary>
    /// 通過類型獲取配置文件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T FindConfig<T>() where T : ConfigTemplate
    {
        ConfigTemplate template = FindConfig(typeof(T).ToString());
        return template == null ? null : template as T;
    }

    public ConfigTemplate FindConfig(string name)
    {
        if (m_ConfigList.ContainsKey(name))
        {
            return m_ConfigList[name];
        }
        else
        {
            return null;
        }
    }
}
