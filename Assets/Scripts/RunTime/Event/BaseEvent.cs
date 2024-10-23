using Cysharp.Threading.Tasks;

using System;
using UnityEngine;

// 分發器
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

// 選擇模式板塊事件
public class BaseEvent
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
    public virtual async void OnEvent(params object[] args) { await UniTask.Yield(); }

    public virtual void SwitchSceneAccName(string module_name)
    {
        GlobalData.currModuleName = module_name;
        UITools.Loading("Main");
    }
}

public enum OperateType
{
    NONE = 0, GET, ADD, REVISE, DELETE,
}

public enum EventType
{
    None = 0,
    UploadEvent,
    DownLoadEvent,
    CheckEvent,
    UserLoginEvent,
    RegisterEvent,
    GetProjInfo,
    ClassEvent,
    ExamineEvent,
    ScoreEvent
}