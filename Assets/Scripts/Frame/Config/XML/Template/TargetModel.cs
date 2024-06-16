using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
public struct Target
{
    public string modelName;
    public int modelCode;
}


public class TargetModel : ConfigTemplate
{
    public Target m_Target;

    public override void ReadXML(XDocument doc)
    {
        base.ReadXML(doc);
        var target = doc.Root.Element("Target");
        m_Target.modelName = target.Attribute("ModelName").Value;
        m_Target.modelCode = Convert.ToInt32(target.Attribute("ModelCode").Value);
        GlobalData.ModelTarget = m_Target;
        // 如果我这么写，那么 m_Target 几乎就没有什么意义了...
        // ...但是我想保留m_Target 可能以后用的到
    }
}
