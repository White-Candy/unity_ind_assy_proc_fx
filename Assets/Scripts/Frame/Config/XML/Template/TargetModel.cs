
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public struct Proj
{
    public string Columns; //��Ŀ����
    public List<string> Courses; // �γ��б�
}


public class TargetModel : ConfigTemplate
{
    public List<Proj> m_Projs = new List<Proj>();

    public override void ReadXML(XDocument doc)
    {
        base.ReadXML(doc);
        // var Projs = doc.Root.Elements("Item");
        // foreach (var item in Projs)
        // {
        //     Proj proj = new Proj
        //     {
        //         ProjName = item.Attribute("ProjName").Value
        //     };

        //     List<Target> TargetList = new List<Target>();
        //     foreach (var target in item.Elements("Target"))
        //     {
        //         Target t = new Target
        //         {
        //             modelName = target.Attribute("ModelName").Value,
        //             modelCode = Convert.ToInt32(target.Attribute("ModelCode").Value),
        //             menuName = target.Attribute("MenuName").Value
        //         };
        //         TargetList.Add(t);
        //     }
        //     proj.targets = TargetList;
        //     m_Projs.Add(proj);
        // }

        // GlobalData.Projs.Clear();
        // GlobalData.Projs = m_Projs;
        // // �������ôд����ô m_Targets ������û��ʲô������...
        // // ...�������뱣��m_Targets �����Ժ��õĵ�
    }
}
