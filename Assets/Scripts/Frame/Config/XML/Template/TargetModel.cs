
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
    }
}
