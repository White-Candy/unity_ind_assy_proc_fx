using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public struct Proj
{
    public string ProjName; //项目名字
    public List<Target> targets; // 子项目列表
}

public struct Target
{
    public string modelName; // 用于对应 Addreass中的Default名字，从而异步加载资源
    public int modelCode; // 模式代码
    public string menuName; // 用于显示在UI上的名字，用户可以选择不同的名字体验不同的场景
}


public class TargetModel : ConfigTemplate
{
    public List<Proj> m_Projs = new List<Proj>();

    public override void ReadXML(XDocument doc)
    {
        base.ReadXML(doc);
        var Projs = doc.Root.Elements("Item");
        foreach (var item in Projs)
        {
            Proj proj = new Proj();
            proj.ProjName = item.Attribute("ProjName").Value;

            List<Target> TargetList = new List<Target>();
            foreach (var target in item.Elements("Target"))
            {
                Target t = new Target();
                t.modelName = target.Attribute("ModelName").Value;
                t.modelCode = Convert.ToInt32(target.Attribute("ModelCode").Value);
                t.menuName = target.Attribute("MenuName").Value;
                TargetList.Add(t);
            }
            proj.targets = TargetList;
            m_Projs.Add(proj);
        }

        GlobalData.Projs.Clear();
        GlobalData.Projs = m_Projs;
        // 如果我这么写，那么 m_Targets 几乎就没有什么意义了...
        // ...但是我想保留m_Targets 可能以后用的到
    }
}
