using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

public class ConfigConsole : Singleton<ConfigConsole>
{
    public static string m_RootPath;

    //�����ļ��б�
    public Dictionary<string, ConfigTemplate> m_ConfigList = new Dictionary<string, ConfigTemplate>();

    // �Ƿ��ѽ����d����
    private bool m_Loaded = false;

    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        DontDestroyOnLoad(this);

        if (!m_Loaded)
        {
            m_RootPath = Application.streamingAssetsPath;
            LoadConfig(m_RootPath);
        }
    }

    public async void LoadConfig(string root_path)
    {
        if (m_Loaded)
            return;

        // Web���xȡ
        await NetworkManager._Instance.DownLoadTextFromServer(Tools.LocalPathEncode(Path.Combine(root_path, @"Config/Config.xml")), (text) =>
        {
            //Debug.Log("Config text: " + text);
            XDocument doc = XDocument.Parse(text);
            XElement root = doc.Root;

            int idx = 0;
            foreach (var item in root.Elements("Item"))
            {
                string name = item.Attribute("name").Value;
                string path = Path.Combine(m_RootPath, item.Attribute("path").Value);

                ConfigTemplate template = Tools.CreateObject<ConfigTemplate>(name); // �� XML �е�name�ֶα����Ӧ�� ʵ��(���඼�� ConfigTemplate)��
                template.id = idx;
                template.name = name;
                template.path = path;

                try
                {
                    template.ReadXML(path);
                }
                catch
                {
                    Debug.LogError(string.Format("��ȡ{0}ʧ�ܣ�ԭ��:����ʧ�� , ·��:{1}", name, path));
                    continue;
                }

                if (!m_ConfigList.ContainsKey(name))
                {
                    m_ConfigList.Add(name, template);
                }
                else
                {
                    Debug.LogError(string.Format("���{0}ʧ�ܣ�ԭ��:�����ظ� , ·��:{1}", name, path));
                }
                idx++;
            }
            m_Loaded = true;
        });
    }

    /// <summary>
    /// ͨ�^��ͫ@ȡ�����ļ�
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
