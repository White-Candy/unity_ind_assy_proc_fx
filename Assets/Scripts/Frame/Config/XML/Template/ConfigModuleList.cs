using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public struct ModuleData
{
    public string moduleName;

    public string moduleCode;
}

public class ConfigModuleList : ConfigTemplate
{
    // 所有文件中的 XML Item都放进去
    public List<ModuleData> m_ModuleList = new List<ModuleData>();

    public override void ReadXML(XDocument doc)
    {
        //Debug.Log("ConfigModuleList ReadXML");
        base.ReadXML(doc);
        foreach (var item in doc.Root.Elements("List")) 
        {
            ModuleData data = new ModuleData();
            data.moduleName = item.Attribute("ModuleName").Value;
            data.moduleCode = item.Attribute("ModuleCode").Value;
            m_ModuleList.Add(data);
        }
    }
}
