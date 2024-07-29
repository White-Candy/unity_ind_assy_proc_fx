using Cysharp.Threading.Tasks;
using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ְl��
public static class ModuleEventSpawn
{

    public static BaseEvent Spawn<T>(string module_name, string code, MonoBehaviour mono) where T : BaseEvent
    {
        try
        {
            Type t = Type.GetType(module_name, true);
            object @object = Activator.CreateInstance(t);
            BaseEvent @event = @object as BaseEvent;
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
public class BaseEvent
{
    // ģʽ������
    public string m_Name;

    // ģʽ����
    public string m_Code;

    public MonoBehaviour m_mono;
    
    /// <summary>
    /// [limite] network unity socket tcp server message: �¼���ʼǰ����ǰ�ð�����
    /// </summary>
    public virtual async void OnPrepare(params object[] args) { await UniTask.Yield(); }

    /// <summary>
    /// ���ÿ��ģʽ��ͬ���¼�
    /// </summary>
    /// <param name="module_name"></param>
    /// <param name="args"></param>
    public virtual async void OnEvent(params object[] args) { await UniTask.Yield(); }

    public virtual void SwitchSceneAccName(string module_name)
    {
        //Debug.Log("SwitchSceneAccName: " + module_name);
        GlobalData.currModuleName = module_name;
        UITools.Loading("Main", module_name);
        //GlobalData.currModuleCode = module_code;
        //if (GlobalData.mode == Mode.Examination)
        //{
        //    if (GlobalData.FinishExamModule.Contains(module_name))
        //    {
        //        //UITools.OpenDialog("", "������ɱ�ģ�鿼�ˣ������ٴν��롣", () => { });
        //        //Debug.Log(@"������ɱ�ģ�鿼�ˣ������ٴν��롣");
        //        return;
        //    }
        //    else
        //    {
        //        GlobalData.FinishExamModule.Add(module_name);
        //        UITools.Loading("Main", module_name);
        //        return;
        //    }
        //}
        //else
        //{
        //    UITools.Loading("Main", module_name);
        //    return;
        //}
    }
}