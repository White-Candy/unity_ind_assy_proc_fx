
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
}

/// <summary>
/// 更新包
/// </summary>
public class UpdatePackage
{
    public string relativePath;
    public List<ResourcesInfo> filesInfo = new List<ResourcesInfo>();
}