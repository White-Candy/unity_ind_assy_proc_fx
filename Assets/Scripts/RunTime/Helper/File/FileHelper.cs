
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FileHelper
{
    /// <summary>
    /// 获取文件在中指定扩展名的文件信息
    /// </summary>
    /// <param name="path"></param>
    public static FileInfo[] GetDirectoryFileInfo(string path, string extension)
    {
        FileInfo[] filesInfo = new FileInfo[] {};
        if (Directory.Exists(path))
        {
            DirectoryInfo info = new DirectoryInfo(path);
            filesInfo = info.GetFiles("*." + extension, SearchOption.AllDirectories).ToArray();
        }
        return filesInfo;
    }

    /// <summary>
    /// Log打印到本地
    /// </summary>
    /// <param name="file_path"></param>
    /// <param name="file_name"></param>
    /// <param name="str_info"></param>
    public static void WriteFileByLine(string file_path, string file_name, string str_info)  
    {  
        StreamWriter sw;  
        if(!File.Exists(file_path+"//"+file_name))  
        { 
            sw=File.CreateText(file_path+"//"+file_name);//创建一个用于写入 UTF-8 编码的文本  
        }  
        else  
        { 
            sw=File.AppendText(file_path+"//"+file_name);//打开现有 UTF-8 编码文本文件以进行读取  
        }  
        sw.WriteLine(str_info);//以行为单位写入字符串  
        sw.Close ();  
        sw.Dispose ();//文件流释放  
    }

    /// <summary>
    /// Webgl平台Config文件
    /// </summary>
    /// <param name="name"></param>
    /// <param name="configPath"></param>
    /// <returns></returns>
    public async static UniTask<List<string>> DownLoadConfig(string name, string configPath, string suffix)
    {
        List<string> paths = new List<string>();
        await Utilly.DownLoadTextFromServer(configPath, (text) => 
        {
            if (text.Count() > 0)
            {
                string[] strs = text.Split('_');
                for (int i = 0; i < strs.Length; i++)
                {
                    paths.Add(FPath.AssetRootPath + GlobalData.ProjGroupName + Tools.GetModulePath(name) + "/" + strs[i] + suffix);
                }
            }
        });
        return paths;
    }

    /// <summary>
    /// WebGLTargetMode文件读取
    /// </summary>
    public async static void ReadTargetFileOnWebGL()
    {
        await Utilly.DownLoadTextFromServer(Application.streamingAssetsPath + "\\Config\\WebGLTargetMode.txt", (text) =>
        {
            string[] split = text.Split("|");
            GlobalData.columnsName = split[0];
            GlobalData.courseName = split[1];
        });
    }
}

/// <summary>
/// 更新包
/// </summary>
public class UpdatePackage
{
    public string relativePath;
    public List<ResourcesInfo> filesInfo = new List<ResourcesInfo>();
}