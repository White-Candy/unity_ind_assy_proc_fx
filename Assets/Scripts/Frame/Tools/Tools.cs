using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.AddressableAssets.HostingServices;
using UnityEngine;

public static class Tools
{
    // 中转英字典
    public static Dictionary<string, string> EscDic = new Dictionary<string, string>{ { @"教学", "TeachingEvent"}, {@"训练", "TrainEvent"},
        {@"考核", "AssessEvent"}, 
     {@"教案", "PDFAction"}, {@"图纸", "PDFAction"},{@"方案", "PDFAction"},{@"规范", "PDFAction"}, {@"图片", "PictureAction"}, 
     {@"动画", "VideoAction"},{@"视频", "VideoAction"}, {@"构造", "ModelAction"}, {@"理论", "TheoreticalExamAction"} };

    // 不同子模式对应不同的文件路径
    // (没有写在里面的子模式，要么是网络获取，要么是从Addressabels中获取)
    private static Dictionary<string, string> FileDic = new Dictionary<string, string> 
    {
        {@"教案", FPath.JiaoAnSuffix}, {@"图纸", FPath.TuZhiSuffix}, {@"方案", FPath.FangAnSuffix}, {@"规范", FPath.GuiFanSuffix},
        {@"图片", FPath.PictureSuffix}, {@"动画", FPath.AnimSuffix}, {@"视频", FPath.VideoSuffix}
    };
    public static bool CheckMessageSuccess(int code)
    {
        return code == GlobalData.SuccessCode;
    }

    /// <summary>
    /// 动态创建类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    static public T CreateObject<T>(string name) where T : class
    {
        object obj = CreateObject(name);
        return obj == null ? null : obj as T;
    }

    /// <summary>
    /// 动态创建类
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static object CreateObject(string name)
    {
        object obj = null;
        try
        {
            Type type = Type.GetType(name, true);
            obj = Activator.CreateInstance(type); //创建指定类型的实例。
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
        return obj;
    }

    public static string LocalPathEncode(string txt)
    {
        return txt;
    }


    /// <summary>
    /// Cn To En Or En To Cn
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string Escaping(string name)
    {
        string TypeName = "";
        if (EscDic.ContainsKey(name))
        {
            TypeName = EscDic[name];
            if (!EscDic.ContainsKey(TypeName))
            {
                EscDic.Add(TypeName, name);
            }
        }
        return TypeName;
    }

    /// <summary>
    /// 获取不同子模式对应的不同文件路径
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetModulePath(string name)
    {
        string path = "";
        if (FileDic.ContainsKey(name))
        {
            path = FileDic[name];
        }
        return path;
    }

    /// <summary>
    /// Object -> Texture2D -> Sprite
    /// </summary>
    /// <returns>The convert.</returns>
    /// <param name="tex">Tex.</param>
    public static Sprite SpriteConvert(Texture2D tex)
    {
        Sprite sprite;
        sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero, 100f);
        return sprite;
    }

    /// <summary>
    /// 检查字符串长度，需要时要换行
    /// </summary>
    /// <param name="text"></param>
    /// <param name="max_len"></param>
    /// <returns></returns>
    public static string checkLength(string text, int max_len)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < text.Length; ++i)
        {
            if (i != 0 && (i % max_len) == 0)
            {
                sb.Append('\n');
            }
            sb.Append(text[i]);
        }
        return sb.ToString();
    }
}
