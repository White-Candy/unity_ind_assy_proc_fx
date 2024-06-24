using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
public struct Target
{
    public string modelName; // 用于对应 Addreass中的Default名字，从而异步加载资源
    public int modelCode; // 模式代码
    public string menuName; // 用于显示在UI上的名字，用户可以选择不同的名字体验不同的场景
}


public class TargetModel : ConfigTemplate
{
    public List<Target> m_Targets = new List<Target>();

    public override void ReadXML(XDocument doc)
    {
        base.ReadXML(doc);
        var targets = doc.Root.Elements("Target");
        foreach (var target in targets )
        {
            Target t = new Target();
            t.modelName = target.Attribute("ModelName").Value;
            t.modelCode = Convert.ToInt32(target.Attribute("ModelCode").Value);
            t.menuName = target.Attribute("MenuName").Value;
            m_Targets.Add(t);
        }

        GlobalData.Targets.Clear();
        GlobalData.Targets = m_Targets;
        // 如果我这么写，那么 m_Targets 几乎就没有什么意义了...
        // ...但是我想保留m_Targets 可能以后用的到
    }
}
