using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public struct Menu
{
    public string menuName;
    public List<SubMenu> subMenuList;
}

public struct SubMenu
{
    public string subMenuName;
    public int enumID;
}

public class ConfigMenuList : ConfigTemplate
{
    public List<Menu> m_MenuList = new List<Menu>();

    public override void ReadXML(XDocument doc)
    {
        base.ReadXML(doc);
        foreach (var item in doc.Root.Elements("Item"))
        {
            Menu menu = new Menu();
            menu.menuName = item.Attribute("MenuName").Value;

            List<SubMenu> subMenuList = new List<SubMenu>();
            foreach( var sub_item in item.Elements("SubMenuItems"))
            {
                SubMenu sub = new SubMenu
                {
                    subMenuName = sub_item.Attribute("SubMenuName").Value,
                    enumID = Convert.ToInt32(sub_item.Attribute("Enum").Value)
                };

                subMenuList.Add(sub);
            }
            menu.subMenuList = subMenuList;
            m_MenuList.Add(menu);
        }
    }
}
