using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ModuleEventSpawn
{

    public static ModuleEvent Spawn<T>(string name, string code, MonoBehaviour mono) where T : ModuleEvent
    {
        try
        {
            Type t = Type.GetType(name, true);
            object @object = Activator.CreateInstance(t);
            ModuleEvent @event = @object as ModuleEvent;
            if (@event != null)
            {
                @event.m_Name = name;
                @event.m_Code = code;
                @event.m_mono = mono;
                return @event;
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        return null;
    }
}

public class ModuleEvent
{
    // 模式的名字
    public string m_Name;

    // 模式代码
    public string m_Code;

    public MonoBehaviour m_mono;

    /// <summary>
    /// 点击每个模式不同的事件
    /// </summary>
    /// <param name="module_name"></param>
    /// <param name="args"></param>
    public virtual void OnEvent(params object[] args) { }
}
