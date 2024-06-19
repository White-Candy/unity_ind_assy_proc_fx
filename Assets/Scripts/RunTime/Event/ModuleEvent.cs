using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ְl��
public static class ModuleEventSpawn
{

    public static ModuleEvent Spawn<T>(string module_name, string code, MonoBehaviour mono) where T : ModuleEvent
    {
        try
        {
            Type t = Type.GetType(module_name, true);
            object @object = Activator.CreateInstance(t);
            ModuleEvent @event = @object as ModuleEvent;
            if (@event != null)
            {
                @event.m_Name = Tools.Escaping(module_name);
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

// �x��ģʽ��K�¼�
public class ModuleEvent
{
    // ģʽ������
    public string m_Name;

    // ģʽ����
    public string m_Code;

    public MonoBehaviour m_mono;

    /// <summary>
    /// ���ÿ��ģʽ��ͬ���¼�
    /// </summary>
    /// <param name="module_name"></param>
    /// <param name="args"></param>
    public virtual void OnEvent(params object[] args) { }

    public virtual void SwitchSceneAccName(string module_name, string module_code)
    {
        Debug.Log("SwitchSceneAccName: " + module_name);
        GlobalData.currModuleName = module_name;
        GlobalData.currModuleCode = module_code;
        if (GlobalData.mode == Mode.Examination)
        {
            if (GlobalData.FinishExamModule.Contains(module_name))
            {
                //UITools.OpenDialog("", "������ɱ�ģ�鿼�ˣ������ٴν��롣", () => { });
                //Debug.Log(@"������ɱ�ģ�鿼�ˣ������ٴν��롣");
                return;
            }
            else
            {
                GlobalData.FinishExamModule.Add(module_name);
                UITools.Loading("Main", false, module_name);
                return;
            }
        }
        else
        {
            UITools.Loading("Main", false, module_name);
            return;
        }
    }
}